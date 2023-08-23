namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class DueDisc
    {
        public string CoCode { get; set; }

        public decimal DiscRate1 { get; set; }

        public decimal DiscRate2 { get; set; }

        public decimal DiscRate3 { get; set; }

        public int DueDay { get; set; }

        public string DueDiscCode { get; set; }

        public string ItemCode { get; set; }

        public string ItemDueDiscCode { get; set; }

        public string ItemName { get; set; }

        public int LineNo { get; set; }
    }
}

