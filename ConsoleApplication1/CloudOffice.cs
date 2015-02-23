using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Net;
using Newtonsoft.Json;

namespace ConsoleApplication1
{
    class CloudOffice
    {
        private WebClient client;
        private string baseUrl;
        private string apiKey;
        private string secretKey;
        private string format;
        private int size = 250;

        public CloudOffice(string apiKey, string secretKey, string format = "application/json")// "text/xml"
        {
            this.client = new System.Net.WebClient();
            this.baseUrl = "https://api.emailsrvr.com/v1/";
            this.apiKey = apiKey;
            this.secretKey = secretKey;
            this.format = format;
        }

        public IList<Customer> GetAccounts()
        {
            int total = int.MaxValue;
            var offset = 0;

            List<Customer> customers = new List<Customer>();

            while (offset < total)
            {
                var urlAccounts = string.Format("customers?size={0}&offset={1}", size, offset);
                var response = JsonConvert.DeserializeObject<AccountListing>(Get(urlAccounts));
                customers.AddRange(response.customers);

                total = response.total;
                offset = offset + size;
            }

            return customers;
        }

        public IList<Domain> GetDomains(Customer customer)
        {
            int total = int.MaxValue;
            var offset = 0;

            List<Domain> domains = new List<Domain>();

            while (offset < total)
            {
                var urldomains = string.Format("customers/{0}/domains?size={1}&offset={2}", customer.accountNumber, size, offset);
                var response = JsonConvert.DeserializeObject<DomainListing>(Get(urldomains));
                domains.AddRange(response.domains);

                total = response.total;
                offset = offset + size;
            }

            return domains;
        }

        public IList<Rsmailbox> GetRSMailboxes(Customer customer, Domain domain)
        {
            int total = int.MaxValue;
            var offset = 0;

            List<Rsmailbox> rsMailboxes = new List<Rsmailbox>();

            while (offset < total)
            {
                var urlRSMailboxes = string.Format("customers/{0}/domains/{1}/rs/mailboxes?size={2}&offset={3}", customer.accountNumber, domain.name, size, offset);
                var response = JsonConvert.DeserializeObject<RsMailboxListing>(Get(urlRSMailboxes));
                rsMailboxes.AddRange(response.rsMailboxes);

                total = response.total;
                offset = offset + size;
            }

            return rsMailboxes;
        }

        public IList<Mailbox> GetEXMailboxes(Customer customer, Domain domain)
        {
            int total = int.MaxValue;
            var offset = 0;

            List<Mailbox> exMailboxes = new List<Mailbox>();

            while (offset < total)
            {
                var urlEXmailboxes = string.Format("customers/{0}/domains/{1}/ex/mailboxes?size={2}&offset={3}", customer.accountNumber, domain.name, size, offset);


                var urlRSMailboxes = string.Format("customers/{0}/domains/{1}/rs/mailboxes?size={2}&offset={3}", customer.accountNumber, domain.name, size, offset);
                var response = JsonConvert.DeserializeObject<EXMailboxListing>(Get(urlEXmailboxes));

                exMailboxes.AddRange(response.mailboxes);
                total = response.total;
                offset = offset + size;
            }

            return exMailboxes;
        }

        public void AddCustomer(string name)
        {
            var data = new NameValueCollection();
            data.Add("name", name);
            Post("customers", data);
        }

        public virtual string Get(string url)
        {
            return MakeRemoteCall((client) =>
            {
                return client.DownloadString(baseUrl + url);
            },
            format);
        }

        public virtual string Post(string url, System.Collections.Specialized.NameValueCollection data)
        {
            return MakeRemoteCall((client) =>
            {
                var bytes = client.UploadValues(baseUrl + url, data);
                return Encoding.UTF8.GetString(bytes);
            },
            format);
        }

        private void SignMessage()
        {
            var userAgent = "C# Client";
            client.Headers["User-Agent"] = userAgent;

            var dateTime = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

            var dataToSign = apiKey + userAgent + dateTime + secretKey;
            var hash = System.Security.Cryptography.SHA1.Create();
            var signedBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
            var signature = Convert.ToBase64String(signedBytes);

            client.Headers["X-Api-Signature"] = apiKey + ":" + dateTime + ":" + signature;
        }

        private void AssignFormat(string format)
        {
            client.Headers["Accept"] = format;
        }

        private string MakeRemoteCall(Func<WebClient, string> remoteCall, string format)
        {
            var retries = 0;

            while (retries < 3)
            {
                try
                {
                    SignMessage();
                    AssignFormat(format);
                    return remoteCall.Invoke(client);
                }
                catch (System.Net.WebException ex)
                {
                    if (ex.Message.Contains("Exceeded request limits"))
                    {
                        System.Threading.Thread.Sleep(20000);
                        retries++;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            throw new Exception("Number of retries exceeded.");
        }
    }
}
