namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class SurveyAnswerM
    {
        public string BranchCode { get; set; }

        public int BranchId { get; set; }

        public string EntityCode { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public string SourceGuid { get; set; }

        public SurveyAnswerD[] SurveyAnswerDList { get; set; }

        public string SurveyDate { get; set; }

        public int SurveyTemplateMId { get; set; }
    }
}

