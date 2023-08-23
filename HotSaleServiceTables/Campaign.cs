namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class Campaign
    {
        public string BranchCode { get; set; }

        public string CampaignCode { get; set; }

        public string CampaignDesc { get; set; }

        public int CampaignId { get; set; }

        public DateTime EndDate { get; set; }

        public string EntityGrpCode1 { get; set; }

        public string EntityGrpCode2 { get; set; }

        public string EntityGrpCode3 { get; set; }

        public string EntityGrpName1 { get; set; }

        public string EntityGrpName2 { get; set; }

        public string EntityGrpName3 { get; set; }

        public int Id { get; set; }

        public bool IsAll { get; set; }

        public bool IsPassive { get; set; }

        public bool IsResetOtherDisc { get; set; }

        public string Note { get; set; }

        public int ResultItemApplySource { get; set; }

        public DateTime StartDate { get; set; }
    }
}

