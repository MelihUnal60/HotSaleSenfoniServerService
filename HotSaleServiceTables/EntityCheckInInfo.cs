namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class EntityCheckInInfo
    {
        public DateTime CheckinDate { get; set; }

        public string EntityCode { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string SalesPersonCode { get; set; }
    }
}

