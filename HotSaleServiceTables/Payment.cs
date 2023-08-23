namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class Payment
    {
        public string AccCode { get; set; }

        public decimal Amt { get; set; }

        public string BankBranchCode { get; set; }

        public string BankCode { get; set; }

        public string BankDescription { get; set; }

        public string BranchCode { get; set; }

        public string CatCode1 { get; set; }

        public string CatCode2 { get; set; }

        public DateTime DocDate { get; set; }

        public string DocNo { get; set; }

        public DateTime DueDate { get; set; }

        public string EntityCode { get; set; }

        public string EntityName { get; set; }

        public string InvoiceNo { get; set; }

        public bool IsDelete { get; set; }

        public bool IsIndorsement { get; set; }

        public string IslemSaati { get; set; }

        public bool IsOpen { get; set; }

        public bool IsPrint { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string Note { get; set; }

        public int PortNo { get; set; }

        public string Series { get; set; }

        public string SourceGuid { get; set; }
    }
}

