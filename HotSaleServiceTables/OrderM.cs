namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class OrderM
    {
        public decimal Amt { get; set; }

        public decimal AmtDisc { get; set; }

        public decimal AmtOrder { get; set; }

        public decimal AmtVat { get; set; }

        public string BranchCode { get; set; }

        public string CatCode1 { get; set; }

        public string CatCode2 { get; set; }

        public DateTime DocDate { get; set; }

        public string DocNo { get; set; }

        public DateTime DueDate { get; set; }

        public int DueDay { get; set; }

        public string EntityCode { get; set; }

        public string EntityCode2 { get; set; }

        public string EntityDocNo { get; set; }

        public int ERPOrderMId { get; set; }

        public bool IsConsigned { get; set; }

        public bool IsDeleted { get; set; }

        public string IslemSaati { get; set; }

        public decimal Latitude { get; set; }

        public string LoadingCard { get; set; }

        public decimal Longitude { get; set; }

        public decimal MasterDiscRate { get; set; }

        public OrderD[] OrderDList { get; set; }

        public int OrderMId { get; set; }

        public string PurchaseSales { get; set; }

        public DateTime ShippingDate { get; set; }

        public string SourceGuid { get; set; }

        public string WhouseCode { get; set; }
    }
}

