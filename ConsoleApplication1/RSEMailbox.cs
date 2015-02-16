using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    public class RSEMailbox
    {
        public RSEContactinfo contactInfo { get; set; }
        public string createdDate { get; set; }
        public int currentUsage { get; set; }
        public string[] emailForwardingAddressList { get; set; }
        public bool enableVacationMessage { get; set; }
        public bool enabled { get; set; }
        public string lastLogin { get; set; }
        public string name { get; set; }
        public bool saveForwardedEmail { get; set; }
        public int size { get; set; }
        public string vacationMessage { get; set; }
        public bool visibleInExchangeGAL { get; set; }
        public bool visibleInRackspaceEmailCompanyDirectory { get; set; }
    }

    public class RSEContactinfo
    {
        public string businessCity { get; set; }
        public string businessCountry { get; set; }
        public string businessNumber { get; set; }
        public string businessPostalCode { get; set; }
        public string businessState { get; set; }
        public string businessStreet { get; set; }
        public string customID { get; set; }
        public string employeeType { get; set; }
        public string faxNumber { get; set; }
        public string firstName { get; set; }
        public string generationQualifier { get; set; }
        public string homeCity { get; set; }
        public string homeCountry { get; set; }
        public string homeFaxNumber { get; set; }
        public string homeNumber { get; set; }
        public string homePostalCode { get; set; }
        public string homeState { get; set; }
        public string homeStreet { get; set; }
        public string initials { get; set; }
        public string lastName { get; set; }
        public string mobileNumber { get; set; }
        public string notes { get; set; }
        public string organizationUnit { get; set; }
        public string organizationalStatus { get; set; }
        public string pagerNumber { get; set; }
        public string title { get; set; }
        public string userID { get; set; }
    }

}
