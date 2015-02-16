using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    public class EXMailbox
    {
        public EXContactinfo contactInfo { get; set; }
        public string createdDate { get; set; }
        public int currentUsage { get; set; }
        public string displayName { get; set; }
        public Emailaddresslist[] emailAddressList { get; set; }
        public string emailForwardingAddress { get; set; }
        public bool enabled { get; set; }
        public bool hasActiveSyncMobileService { get; set; }
        public bool hasBlackBerryMobileService { get; set; }
        public bool isHidden { get; set; }
        public bool isPublicFolderAdmin { get; set; }
        public string lastLogin { get; set; }
        public string name { get; set; }
        public string samAccountName { get; set; }
        public int size { get; set; }
        public bool visibleInRackspaceEmailCompanyDirectory { get; set; }
    }

    public class EXContactinfo
    {
        public string addressLine1 { get; set; }
        public string businessNumber { get; set; }
        public string city { get; set; }
        public string company { get; set; }
        public string country { get; set; }
        public string customID { get; set; }
        public string department { get; set; }
        public string faxNumber { get; set; }
        public string firstName { get; set; }
        public string homeNumber { get; set; }
        public string jobTitle { get; set; }
        public string lastName { get; set; }
        public string mobileNumber { get; set; }
        public string notes { get; set; }
        public string pagerNumber { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
    }

    public class Emailaddresslist
    {
        public string address { get; set; }
        public bool replyTo { get; set; }
    }

}
