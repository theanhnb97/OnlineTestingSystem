using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingSystem.DataTranferObject.Question
{
    public class QuestionFilterModel
    {
        public int? QuestionID { get; set; }
        public string Content { get; set; }
        public int? Level { get; set; }
        public int? CategoryID { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
