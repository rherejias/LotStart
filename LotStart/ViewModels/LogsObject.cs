using System;

namespace LotStart.ViewModels
{
    public class LogsObject
    {
        public string MoveOrderNbr { get; set; }
        public string Org { get; set; }
        public string Item { get; set; }
        public string TargetCAS { get; set; }
        public string LotNumber { get; set; }
        public string Planner { get; set; }
        public string Quantity { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateRequired { get; set; }
        public string SubInventory { get; set; }
        public string Package { get; set; }
        public int User { get; set; }
        public DateTime DateSubmitted { get; set; }
        public string Status { get; set; }

    }
}