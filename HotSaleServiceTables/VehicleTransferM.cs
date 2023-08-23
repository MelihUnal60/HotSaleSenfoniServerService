namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class VehicleTransferM
    {
        public DateTime DocDate { get; set; }

        public string DocNo { get; set; }

        public string IslemSaati { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string SourceGuid { get; set; }

        public VehicleTransferD[] VehicleDetailList { get; set; }

        public int VehicleTransferMId { get; set; }

        public string WhouseCodeIn { get; set; }

        public string WhouseCodeOut { get; set; }
    }
}

