namespace HotSaleServiceTables
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class SurveyTemplateM
    {
        public string BranchCode { get; set; }

        public int BranchId { get; set; }

        public string Description { get; set; }

        public int Id { get; set; }

        public string SurveyTemplateCode { get; set; }

        public List<SurveyTemplateD> SurveyTemplateDList { get; set; }

        public int SurveyType { get; set; }
    }
}

