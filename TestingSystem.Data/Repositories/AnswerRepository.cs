using System.Collections.Generic;
using System.Linq;
using TestingSystem.Data.Infrastructure;
using TestingSystem.Models;

namespace TestingSystem.Data.Repositories
{
    public interface IAnswerRepository : IRepository<Answer>
    {
        List<Answer> GetAnswersByQuestionID(int? id);
        bool UpdateAnswer(Answer answer);
        int AddAnswer(Answer answer);
    }

    public class AnswerRepository : RepositoryBase<Answer>, IAnswerRepository
    {
        public AnswerRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }

        public int AddAnswer(Answer answer)
        {
            {
                DbContext.Answers.Add(answer);
                this.DbContext.SaveChanges();
                return 1;
            }
        }

        public List<Answer> GetAnswersByQuestionID(int? id)
        {
            var listAnswer = DbContext.Answers.Where(x => x.QuestionID == id).ToList();
            return listAnswer;
        }

        public bool UpdateAnswer(Answer answer)
        {
            var objQuestion = this.DbContext.Answers.Find(answer.AnswerID);
            if (objQuestion != null)
            {
                objQuestion.AnswerContent = answer.AnswerContent;
                objQuestion.IsCorrect = answer.IsCorrect;
                objQuestion.QuestionID = answer.QuestionID;
                this.DbContext.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
