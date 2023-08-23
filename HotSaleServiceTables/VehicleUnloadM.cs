namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class VehicleUnloadM
    {
        public string BranchCode { get; set; }

        public DateTime DocDate { get; set; }

        public string DocNo { get; set; }

        public bool IsDeleted { get; set; }

        public string IslemSaati { get; set; }

        public bool IsOpen { get; set; }

        public bool IsPrint { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public HotSaleServiceTables.PurchaseSales PurchaseSales { get; set; }

        public string SourceGuid { get; set; }

        public VehicleUnloadD[] VehicleUnloadDList { get; set; }

        public int VehicleUnloadMId { get; set; }

        public string WhouseCode { get; set; }

        public string WhouseCode2 { get; set; }
    }
}

