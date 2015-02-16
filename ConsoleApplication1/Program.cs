using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace ConsoleApplication1
{
    class Program
    {
        private static string fileName = string.Format("MailboxForwardingInfo_{0}.txt", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));

        static void Main(string[] args)
        {
            try
            {
                var accountNumber = string.Empty;
                var apiKey = string.Empty;
                var secretKey = string.Empty;


                if (args.Count() > 0)
                {
                    accountNumber = args[0];
                }
                else
                {
                    Console.Write("Enter AccountNumber: ");
                    accountNumber = Console.ReadLine();
                }

                if (args.Count() > 1)
                {
                    apiKey = args[1];
                }
                else
                {
                    Console.Write("Enter APIKey: ");
                    apiKey = Console.ReadLine();
                }
                if (args.Count() > 2)
                {
                    secretKey = args[2];
                }
                else
                {
                    Console.Write("Enter SecretKey: ");
                    secretKey = Console.ReadLine();
                }

                var wm = new WebMethods(new System.Net.WebClient(), "https://api.emailsrvr.com/v1/", apiKey, secretKey);

                Report(string.Format("Searching Account: {0} for Email Forwarding", accountNumber) + Environment.NewLine);

                SearchAccount(wm);

                Report(Environment.NewLine + "Completed Search!");
            }
            catch (Exception ex)
            {
                Report(Environment.NewLine + "Encountered errors while trying to run.");

                var sbException = new StringBuilder(ex.Message);
                while (ex.InnerException != null)
                {
                    sbException.AppendLine(ex.InnerException.Message);
                    ex = ex.InnerException;
                }

                Report(sbException.ToString());
            }
            finally
            {
                Console.ReadKey();
            }
        }

        private static void SearchAccount(WebMethods wm)
        {
            var urlAccounts = "customers";
            var accounts = JsonConvert.DeserializeObject<AccountListing>(wm.Get(urlAccounts)).customers.ToList<Customer>();

            foreach (Customer customer in accounts)
            {
                Report(string.Format(Environment.NewLine + "AccountNumber: {0}", customer.accountNumber));

                // get domains
                var urldomains = string.Format("customers/{0}/domains", customer.accountNumber);
                var domains = JsonConvert.DeserializeObject<DomainListing>(wm.Get(urldomains)).domains.ToList<Domain>();

                foreach (Domain domain in domains)
                {
                    Report(string.Format(Environment.NewLine + "Domain: {0}", domain.name));

                    // get RSE mailboxes
                    var urlRSMailboxes = string.Format("customers/{0}/domains/{1}/rs/mailboxes", customer.accountNumber, domain.name);
                    var rsMailboxes = JsonConvert.DeserializeObject<RSEMailboxListing>(wm.Get(urlRSMailboxes)).rsMailboxes.ToList<Rsmailbox>();

                    foreach (Rsmailbox rsMailbox in rsMailboxes)
                    {
                        var urlMailbox = string.Format("customers/{0}/domains/{1}/rs/mailboxes/{2}", customer.accountNumber, domain.name, rsMailbox.name);
                        var rseMailbox = JsonConvert.DeserializeObject<RSEMailbox>(wm.Get(urlMailbox));

                        // if mailbox has forwards, log data
                        if (rseMailbox.emailForwardingAddressList.Count() > 0)
                        {
                            Report(string.Format("RSE Mailbox: {0}, Forwarding: {1}", rseMailbox.name, string.Join(", ", rseMailbox.emailForwardingAddressList)));
                        }
                    }

                    // get EX mailboxes
                    var urlEXmailboxes = string.Format("customers/{0}/domains/{1}/ex/mailboxes", customer.accountNumber, domain.name);
                    var exMailboxes = JsonConvert.DeserializeObject<EXMailboxListing>(wm.Get(urlEXmailboxes)).mailboxes.ToList<Mailbox>();

                    foreach (Mailbox mailbox in exMailboxes)
                    {
                        var urlEXMailbox = string.Format("customers/{0}/domains/{1}/ex/mailboxes/{2}", customer.accountNumber, domain.name, mailbox.name);
                        var exMailbox = JsonConvert.DeserializeObject<EXMailbox>(wm.Get(urlEXMailbox));

                        // if mailbox has forwards, log data
                        if (exMailbox.emailForwardingAddress != string.Empty)
                        {
                            Report(string.Format("HEX Mailbox: {0}, Forwarding: {1}", exMailbox.name, exMailbox.emailForwardingAddress));
                        }
                    }
                }
            }
        }

        private static void Report(string msg)
        {
            Console.WriteLine(msg);
            File.AppendAllText(fileName, msg + Environment.NewLine);
        }
    }



    public class WebMethods
    {
        private WebClient client;
        private string baseUrl;
        private string apiKey;
        private string secretKey;
        private string format;

        public WebMethods(WebClient client, string baseUrl, string apiKey, string secretKey, string format = "application/json")// "text/xml"
        {
            this.client = client;
            this.baseUrl = baseUrl;
            this.apiKey = apiKey;
            this.secretKey = secretKey;
            this.format = format;
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
            SignMessage();
            AssignFormat(format);
            return remoteCall.Invoke(client);
        }
    }
}
