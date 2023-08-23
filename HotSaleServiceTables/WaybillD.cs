namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class WaybillD
    {
        public string BranchCode { get; set; }

        public int CampaignId { get; set; }

        public decimal DiscRate1 { get; set; }

        public decimal DiscRate2 { get; set; }

        public decimal DiscRate3 { get; set; }

        public int DueDay { get; set; }

        public int ERPWaybillDId { get; set; }

        public int ERPWaybillMId { get; set; }

        public string ItemCode { get; set; }

        public string NedenKod { get; set; }

        public decimal Price { get; set; }

        public decimal Qty { get; set; }

        public decimal QtyPrm { get; set; }

        public int SourceDId { get; set; }

        public int SourceMId { get; set; }

        public decimal SVatRate { get; set; }

        public string UnitCode { get; set; }

        public bool VatStatus { get; set; }

        public int WaybillDId { get; set; }

        public int WaybillMId { get; set; }
    }
}

