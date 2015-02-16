using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    public class AccountListing
    {
        public Customer[] customers { get; set; }
        public int offset { get; set; }
        public int size { get; set; }
        public int total { get; set; }
    }

    public class Customer
    {
        public string accountNumber { get; set; }
        public string name { get; set; }
        public object referenceNumber { get; set; }
    }

}
