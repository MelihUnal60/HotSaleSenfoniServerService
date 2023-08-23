namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class DepositTransaction
    {
        public string BranchCode { get; set; }

        public string CreateTime { get; set; }

        public string DepositCode { get; set; }

        public string EntityCode { get; set; }

        public bool IsCession { get; set; }

        public string LoadingCard { get; set; }

        public decimal QtyDrop { get; set; }

        public decimal QtyTake { get; set; }

        public string RegionCode { get; set; }

        public string SourceGuid { get; set; }

        public string WhouseCode { get; set; }
    }
}

