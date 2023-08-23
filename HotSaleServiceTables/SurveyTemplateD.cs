namespace HotSaleServiceTables
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class SurveyTemplateD
    {
        public string Description { get; set; }

        public int Id { get; set; }

        public bool IsRequired { get; set; }

        public int QuestInputType { get; set; }

        public int QuestNo { get; set; }

        public List<SurveyTemplateAnswer> SurveyTemplateAnswerList { get; set; }

        public int SurveyTemplateMId { get; set; }
    }
}

