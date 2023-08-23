namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class OrderInvoiceD
    {
        public decimal DiscRate1 { get; set; }

        public decimal DiscRate2 { get; set; }

        public decimal DiscRate3 { get; set; }

        public string ItemCode { get; set; }

        public string LoadingCardNo { get; set; }

        public decimal Price { get; set; }

        public decimal Qty { get; set; }

        public decimal QtyPrm { get; set; }

        public decimal QtyPrm2 { get; set; }

        public string SourceApp { get; set; }

        public int SourceDId { get; set; }

        public int SourceMId { get; set; }

        public string UnitCode { get; set; }

        public string UnitCode2 { get; set; }

        public int VatRate { get; set; }

        public bool VatStatus { get; set; }
    }
}

