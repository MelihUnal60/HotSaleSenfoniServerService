namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class VehicleTransferD
    {
        public bool IsEntry { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public decimal Qty { get; set; }

        public decimal QtyPrm { get; set; }

        public string UnitCode { get; set; }

        public int VehicleTransferMId { get; set; }
    }
}

