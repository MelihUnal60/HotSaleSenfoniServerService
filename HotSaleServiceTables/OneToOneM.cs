namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class OneToOneM
    {
        public DateTime DocDate { get; set; }

        public string DocNo { get; set; }

        public string EntityCode { get; set; }

        public string IslemSaati { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public OneToOneD[] OneToOneDetailList { get; set; }

        public int OneToOneMId { get; set; }

        public string SourceApp { get; set; }

        public string SourceGuid { get; set; }

        public string WhouseCodeIn { get; set; }

        public string WhouseCodeOut { get; set; }
    }
}

