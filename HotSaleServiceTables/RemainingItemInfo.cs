namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class RemainingItemInfo
    {
        public DateTime DocDate { get; set; }

        public string EntityCode { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public decimal Qty { get; set; }

        public string SourceGuid { get; set; }

        public string UnitCode { get; set; }
    }
}

