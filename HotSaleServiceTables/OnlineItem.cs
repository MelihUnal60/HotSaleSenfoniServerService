namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class OnlineItem
    {
        public string BranchCode { get; set; }

        public decimal FreeQty { get; set; }

        public string ItemCode { get; set; }

        public decimal PurchaseOrderQty { get; set; }

        public decimal QtyPrime { get; set; }

        public decimal ReservationQty { get; set; }

        public decimal SalesOrderQty { get; set; }

        public decimal UseQty { get; set; }

        public string WhouseCode { get; set; }
    }
}

