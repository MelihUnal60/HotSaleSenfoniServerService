namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class InvoiceD
    {
        public int CampaignId { get; set; }

        public decimal DiscRate1 { get; set; }

        public decimal DiscRate2 { get; set; }

        public decimal DiscRate3 { get; set; }

        public int DueDay { get; set; }

        public int InvoiceMId { get; set; }

        public string ItemCode { get; set; }

        public decimal Price { get; set; }

        public decimal Qty { get; set; }

        public decimal QtyPrm { get; set; }

        public int SourceDId { get; set; }

        public int SourceMId { get; set; }

        public string UnitCode { get; set; }

        public int VatRate { get; set; }

        public bool VatStatus { get; set; }

        public string ReasonCode { get; set; } // 23.03.2022
    }
}

