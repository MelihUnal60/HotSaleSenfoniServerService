namespace HotSaleServiceTables
{
    using System;
    using System.Runtime.CompilerServices;

    public class SurveyAnswer
    {
        public string AnswerNo { get; set; }

        public int Id { get; set; }

        public bool IsSelected { get; set; }

        public string SelectAnswer { get; set; }

        public int SurveyAnswerDId { get; set; }

        public int SurveyAnswerMId { get; set; }
    }
}

