namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class OneToOneD
    {
        public decimal DiscRate1 { get; set; }

        public decimal DiscRate2 { get; set; }

        public int ERPSourceDId { get; set; }

        public int ERPSourceMId { get; set; }

        public string ItemCode { get; set; }

        public int OneToOneMId { get; set; }

        public decimal Price { get; set; }

        public decimal Qty { get; set; }

        public decimal QtyPrm { get; set; }

        public string SourceApp { get; set; }

        public string UnitCode { get; set; }
    }
}

