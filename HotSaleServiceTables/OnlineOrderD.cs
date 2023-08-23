namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class OnlineOrderD
    {
        public string BranchCode { get; set; }

        public DateTime DocDate { get; set; }

        public string DocNo { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public decimal Qty { get; set; }

        public string UnitCode { get; set; }
    }
}

