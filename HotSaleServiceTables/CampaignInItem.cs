namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class CampaignInItem
    {
        public decimal Amt { get; set; }

        public string BrandCode { get; set; }

        public int CampaignId { get; set; }

        public string Categories1Code { get; set; }

        public string Categories2Code { get; set; }

        public string Categories3Code { get; set; }

        public string Categories4Code { get; set; }

        public bool IsAll { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public int LineNo { get; set; }

        public decimal QtyPrm { get; set; }

        public string UnitCode { get; set; }
    }
}

