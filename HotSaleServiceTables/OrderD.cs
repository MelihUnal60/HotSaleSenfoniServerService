namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class OrderD
    {
        public string BranchCode { get; set; }

        public int CampaignId { get; set; }

        public decimal DiscRate1 { get; set; }

        public decimal DiscRate2 { get; set; }

        public decimal DiscRate3 { get; set; }

        public int DueDay { get; set; }

        public int ERPOrderDId { get; set; }

        public int ERPOrderMId { get; set; }

        public string ItemCode { get; set; }

        public string LoadingCardNo { get; set; }

        public int OrderMId { get; set; }

        public decimal OrderQty { get; set; }

        public decimal Price { get; set; }

        public decimal Qty { get; set; }

        public decimal QtyPrm { get; set; }

        public decimal QtyPrm2 { get; set; }

        public string UnitCode { get; set; }

        public string UnitCode2 { get; set; }

        public decimal VatRate { get; set; }

        public bool VatStatus { get; set; }

        public string WhouseCode { get; set; }

        public string ReasonCode { get; set; } // 14.03.2022
    }
}

