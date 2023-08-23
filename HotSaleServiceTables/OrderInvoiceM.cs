namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class OrderInvoiceM
    {
        public decimal Amt { get; set; }

        public decimal AmtDisc { get; set; }

        public decimal AmtReceipt { get; set; }

        public decimal AmtVat { get; set; }

        public string BranchCode { get; set; }

        public DateTime DocDate { get; set; }

        public string DocNo { get; set; }

        public string EntityCode { get; set; }

        public string SourceApp { get; set; }

        public int SourceMId { get; set; }
    }
}

