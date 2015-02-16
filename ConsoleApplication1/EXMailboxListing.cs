using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    public class EXMailboxListing
    {
        public Mailbox[] mailboxes { get; set; }
        public int offset { get; set; }
        public int size { get; set; }
        public int total { get; set; }
    }

    public class Mailbox
    {
        public string name { get; set; }
        public string displayName { get; set; }
    }

}
