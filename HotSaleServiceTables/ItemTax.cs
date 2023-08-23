namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class ItemTax
    {
        public DateTime EndDate { get; set; }

        public int ItemId { get; set; }

        public DateTime StartDate { get; set; }

        public int TaxId { get; set; }

        public decimal VatRate { get; set; }
    }
}

