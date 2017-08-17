using System;

namespace LotStart.ViewModels
{
    //object for audit trail
    // rherejias 4/26/2017
    public class AuditTrailObject
    {

        public string ProjectCode { get; set; }
        public string Module { get; set; }
        public string Operation { get; set; }
        public string Object { get; set; }
        public string ObjectId { get; set; }
        public string ObjectCode { get; set; }
        public int UserCode { get; set; }
        public string IP { get; set; }
        public string MAC { get; set; }
        public DateTime DateAdded { get; set; }
    }
}