namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class WaybillM
    {
        public decimal Amt { get; set; }

        public decimal AmtDisc { get; set; }

        public decimal AmtVat { get; set; }

        public decimal AmtWaybill { get; set; }

        public string BranchCode { get; set; }

        public string CatCode1 { get; set; }

        public string CatCode2 { get; set; }

        public DateTime DocDate { get; set; }

        public string DocNo { get; set; }

        public DateTime DueDate { get; set; }

        public int DueDay { get; set; }

        public string EntityCode { get; set; }

        public int ERPWaybillMId { get; set; }

        public bool IsDeleted { get; set; }

        public string IslemSaati { get; set; }

        public bool IsOpen { get; set; }

        public bool IsPrint { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public decimal MasterDiscRate { get; set; }

        public string Note { get; set; }

        public string PurchaseSales { get; set; }

        public DateTime ShippingDate { get; set; }

        public string SourceApp { get; set; }

        public string SourceGuid { get; set; }

        public int SourceMId { get; set; }

        public WaybillD[] WaybillDList { get; set; }

        public int WaybillMId { get; set; }

        public string WhouseCode { get; set; }
    }
}

