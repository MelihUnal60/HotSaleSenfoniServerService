namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class EntityRouteItem
    {
        public string EntityCode { get; set; }

        public bool IsInvoice { get; set; }

        public bool IsInvoiceReturn { get; set; }

        public bool IsOneToOne { get; set; }

        public bool IsOrder { get; set; }

        public bool IsWaybill { get; set; }

        public bool IsWaybillReturn { get; set; }

        public string ItemCode { get; set; }
    }
}

