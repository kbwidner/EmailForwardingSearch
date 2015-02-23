using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

                var cloudOffice = new CloudOffice(apiKey, secretKey);

                Report(string.Format("Searching Account: {0} for Email Forwarding", accountNumber) + Environment.NewLine);

                SearchAccount(cloudOffice);

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

        private static void SearchAccount(CloudOffice cloudOffice)
        {
            var accounts = cloudOffice.GetAccounts();

            foreach (Customer customer in accounts)
            {
                Report(string.Format(Environment.NewLine + "AccountNumber: {0}", customer.accountNumber));

                // get domains
                var domains = cloudOffice.GetDomains(customer);

                foreach (Domain domain in domains)
                {
                    Report(string.Format(Environment.NewLine + "Domain: {0}", domain.name));

                    // get RSE mailboxes
                    var rsMailboxes = cloudOffice.GetRSMailboxes(customer, domain);

                    foreach (Rsmailbox rsMailbox in rsMailboxes)
                    {
                        var urlMailbox = string.Format("customers/{0}/domains/{1}/rs/mailboxes/{2}", customer.accountNumber, domain.name, rsMailbox.name);
                        var rseMailbox = JsonConvert.DeserializeObject<RSEMailbox>(cloudOffice.Get(urlMailbox));

                        // if mailbox has forwards, log data
                        if (rseMailbox.emailForwardingAddressList.Count() > 0)
                        {
                            Report(string.Format("RSE Mailbox: {0}, Forwarding: {1}", rseMailbox.name, string.Join(", ", rseMailbox.emailForwardingAddressList)));
                        }
                    }

                    // get EX mailboxes
                    var exMailboxes = cloudOffice.GetEXMailboxes(customer, domain);

                    foreach (Mailbox mailbox in exMailboxes)
                    {
                        var urlEXMailbox = string.Format("customers/{0}/domains/{1}/ex/mailboxes/{2}", customer.accountNumber, domain.name, mailbox.name);
                        var exMailbox = JsonConvert.DeserializeObject<EXMailbox>(cloudOffice.Get(urlEXMailbox));

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
}
