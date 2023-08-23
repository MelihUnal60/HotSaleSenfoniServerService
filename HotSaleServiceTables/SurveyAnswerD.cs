namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class SurveyAnswerD
    {
        public string Answer { get; set; }

        public string DateField { get; set; }

        public int Id { get; set; }

        public int IntegerField { get; set; }

        public decimal NumberField { get; set; }

        public string StringField { get; set; }

        public SurveyAnswer[] SurveyAnswerList { get; set; }

        public int SurveyAnswerMId { get; set; }

        public int SurveyTemplateDId { get; set; }
    }
}

