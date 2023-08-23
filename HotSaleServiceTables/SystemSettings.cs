namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class SystemSettings
    {
        public string BranchCode { get; set; }

        public bool DiscountPriority { get; set; }

        public bool IsActivityEnabled { get; set; }

        public bool IsFastOrderEnabled { get; set; }

        public bool IsOneToOne { get; set; }

        public bool IsPayment { get; set; }

        public bool IsSalesInvoice { get; set; }

        public bool IsSalesOrder { get; set; }

        public bool IsSalesReturnWaybill { get; set; }

        public bool IsSalesWaybill { get; set; }

        public bool IsSaveRealOrder { get; set; }

        public DateTime SystemDate { get; set; }
    }
}

