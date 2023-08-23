namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class Activity
    {
        public int Category1 { get; set; }

        public string Category1Code { get; set; }

        public int Category2 { get; set; }

        public string Category2Code { get; set; }

        public int Category3 { get; set; }

        public string Category3Code { get; set; }

        public ActivityDetail[] DetailList { get; set; }

        public DateTime EndDate { get; set; }

        public string EntityCode { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string Note { get; set; }

        public int SalesPersonId { get; set; }

        public string SourceGuid { get; set; }

        public DateTime StartDate { get; set; }

        public string Topic { get; set; }
    }
}

