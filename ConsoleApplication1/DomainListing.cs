using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    public class DomainListing
    {
        public Domain[] domains { get; set; }
        public int offset { get; set; }
        public int size { get; set; }
        public int total { get; set; }
    }

    public class Domain
    {
        public string accountNumber { get; set; }
        public int exchangeMaxNumMailboxes { get; set; }
        public int exchangeUsedStorage { get; set; }
        public string name { get; set; }
        public int rsEmailMaxNumberMailboxes { get; set; }
        public int rsEmailUsedStorage { get; set; }
        public string serviceType { get; set; }
    }

}
