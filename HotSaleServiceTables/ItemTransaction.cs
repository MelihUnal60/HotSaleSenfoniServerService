namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class ItemTransaction
    {
        public string BranchCode { get; set; }

        public string CatCode1 { get; set; }

        public string CatCode2 { get; set; }

        public string ItemCode { get; set; }

        public HotSaleServiceTables.PlusMinus PlusMinus { get; set; }

        public decimal Qty { get; set; }

        public decimal QtyPrm { get; set; }

        public string UnitCode { get; set; }

        public string WhouseCode { get; set; }
    }
}

