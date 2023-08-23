namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class PriceList
    {
        public string BranchCode { get; set; }

        public decimal DiscRate1 { get; set; }

        public decimal DiscRate2 { get; set; }

        public string EntityGroupCode { get; set; }

        public string ItemCode { get; set; }

        public decimal Price { get; set; }

        public int PriceListMId { get; set; }

        public string UnitCode { get; set; }

        public decimal VatRate { get; set; }

        public bool VatStatus { get; set; }

        public string RuleCode { get; set; }
    }
}

