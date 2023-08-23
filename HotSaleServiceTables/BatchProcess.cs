namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class BatchProcess
    {
        public decimal Amt { get; set; }

        public int BatchProcessId { get; set; }

        public string BranchCode { get; set; }

        public string EntityCode { get; set; }

        public decimal InvoiceQty { get; set; }

        public bool IsCreateInvoice { get; set; }

        public bool IsCreateOneToOne { get; set; }

        public bool IsCreateWaybillReturn { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public decimal OneToOneQty { get; set; }

        public decimal OrderQty { get; set; }

        public decimal ShelfQty { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal WaybillReturnQty { get; set; }

        public string WhouseCode { get; set; }
    }
}

