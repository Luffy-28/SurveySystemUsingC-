using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication10
{
    public class Question
    {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public int? ParentQuestionID { get; set; }
        public string Condition { get; set; }
        public int SequenceOrder { get; set; }
        public string ValidationRule { get; set; }
        public string ValidationMsg { get; set; }
    }
}