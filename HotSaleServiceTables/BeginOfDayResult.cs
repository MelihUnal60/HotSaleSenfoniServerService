namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class BeginOfDayResult
    {
        public HotSaleServiceTables.BeginOfDay BeginOfDay { get; set; }

        public string ErrorMessage { get; set; }

        public bool Success { get; set; }
    }
}

