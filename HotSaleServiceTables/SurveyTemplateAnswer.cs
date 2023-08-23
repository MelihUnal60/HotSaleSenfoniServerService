namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class SurveyTemplateAnswer
    {
        public string Answer { get; set; }

        public string AnswerNo { get; set; }

        public int DependencyQuestNo { get; set; }

        public int Id { get; set; }

        public int SurveyTemplateDId { get; set; }

        public int SurveyTemplateMId { get; set; }
    }
}

