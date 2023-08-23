namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class VehicleUnloadD
    {
        public string BranchCode { get; set; }

        public string ItemCode { get; set; }

        public decimal Qty { get; set; }

        public decimal QtyPrm { get; set; }

        public string UnitCode { get; set; }

        public int VehicleUnloadDId { get; set; }

        public int VehicleUnloadMId { get; set; }
    }
}

