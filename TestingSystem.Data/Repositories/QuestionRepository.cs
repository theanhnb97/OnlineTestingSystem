using System;
using System.Collections.Generic;
using System.Linq;
using TestingSystem.Data.Infrastructure;
using TestingSystem.DataTranferObject.Question;
using TestingSystem.Models;

namespace TestingSystem.Data.Repositories
{
    public interface IQuestionRepository : IRepository<Question>
    {
        /// <summary>
        /// Fuction Get Question 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        QuestionDto GetQuestionInQuestionDTO(int? id, QuestionFilterModel searchModel);
        string GetNameLevelByQuestionID(int id);
        IEnumerable<Level> GetAlLevels();
        IQueryable<QuestionDto> GetAllQuestionDtos(QuestionFilterModel searchModel);
        bool UpdateQuestion(Question question);
        int AddQuestion(Question question);
        int DeleteQuestion(int id);
        Question FindID(int? id);
        bool CheckQuestionInExamPaperQuesion(int id);
        IEnumerable<Question> SearchByContent(string input);
        IQueryable<Question> FilterQuestions(QuestionFilterModel searchModel);

        IEnumerable<QuestionDto> GetQuestionsByExamPaperId(int examPaperId);

        IEnumerable<QuestionDto> GetQuestionsByQuestionCategoryIdAndExamPaperId(int categoryId, int examPaperId);

        IEnumerable<QuestionDto> RandomQuestionsByCategoryIdAndExamPaperIdAndNumber(int categoryId, int examPaperId, int number);
        IEnumerable<Answer> GetAnswersByQuestionId(int? id);
        IEnumerable<Question> GetAllQuestions();
        IEnumerable<Answer> GetAllAnswers();

    }
    public class QuestionRepository : RepositoryBase<Question>, IQuestionRepository
    {
        private readonly IQuestionCategoryRepository questionCategory;
        private readonly IExamPaperQuestionRepository examPaperQuestionRepository;


        public QuestionRepository(IDbFactory dbFactory, IQuestionCategoryRepository questionCategory, IExamPaperQuestionRepository examPaperQuestionRepository) : base(dbFactory)
        {
            this.questionCategory = questionCategory;
            this.examPaperQuestionRepository = examPaperQuestionRepository;
        }
        public Question FindID(int? id)
        {
            var question = this.DbContext.Questions.SingleOrDefault(x => x.QuestionID == id);
            return question;
        }
        public int DeleteQuestion(int id)
        {
            if (CheckQuestionInExamPaperQuesion(id) == false)
            {
                var question = this.DbContext.Questions.Find(id);
                if (question != null)
                {
                    this.DbContext.Questions.Remove(question);
                    return DbContext.SaveChanges();
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }

        }

        public IQueryable<Question> FilterQuestions(QuestionFilterModel searchModel)
        {
            var result = this.DbContext.Questions.AsQueryable();
            if (searchModel != null)
            {
                if (searchModel.QuestionID.HasValue)
                    result = result.Where(x => x.QuestionID == searchModel.QuestionID);

                if (!string.IsNullOrEmpty(searchModel.Content))
                    result = result.Where(x => x.Content.Contains(searchModel.Content));

                if (searchModel.Level.HasValue)
                    result = result.Where(x => x.Level == searchModel.Level);

                if (searchModel.CategoryID.HasValue)
                    result = result.Where(x => x.CategoryID == searchModel.CategoryID);

                if (searchModel.CreatedBy.HasValue)
                    result = result.Where(x => x.CreatedBy == searchModel.CreatedBy);

                if (searchModel.CreatedDate.HasValue)
                    result = result.Where(x => x.CreatedDate == searchModel.CreatedDate);
            }

            return result;
        }
        public IEnumerable<Question> SearchByContent(string input)
        {
            var search = this.DbContext.Questions.OrderByDescending(x => x.QuestionID)
                .Where(x => x.Content.Contains(input.ToLower().Trim())).ToList();
            return search;
        }

        public int AddQuestion(Question question)
        {
            question.CreatedDate = DateTime.Now;
            DbContext.Questions.Add(question);
            DbContext.SaveChanges();
            return question.QuestionID;
        }

        public bool UpdateQuestion(Question question)
        {
            var objQuestion = this.DbContext.Questions.Find(question.QuestionID);
            if (objQuestion != null)
            {
                objQuestion.Content = question.Content;
                objQuestion.Image = question.Image;
                objQuestion.Level = question.Level;
                objQuestion.CategoryID = question.CategoryID;
                objQuestion.IsActive = question.IsActive;
                objQuestion.CreatedBy = question.CreatedBy;
                objQuestion.CreatedDate = objQuestion.CreatedDate;
                objQuestion.ModifiedBy = question.ModifiedBy;
                objQuestion.ModifiedDate = DateTime.Now;
                this.DbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public IQueryable<QuestionDto> GetAllQuestionDtos(QuestionFilterModel searchModel)
        {
            var listQuestionDTOs = new List<QuestionDto>();
            foreach (var item in FilterQuestions(searchModel))
            {
                listQuestionDTOs.Add(new QuestionDto
                {
                    QuestionID = item.QuestionID,
                    IsActive = item.IsActive,
                    Content = item.Content,
                    Image = item.Image,
                    CreatedBy = item.CreatedBy,
                    CreatedDate = item.CreatedDate,
                    ModifiedBy = item.ModifiedBy,
                    ModifiedDate = item.ModifiedDate,
                    CategoryID = item.CategoryID,
                    CategoryName = questionCategory.FindCategoryByID(item.CategoryID).Name,
                    Level = item.Level,
                    LevelName = GetNameLevelByQuestionID(item.QuestionID)
                });
            }

            var listQuestion = listQuestionDTOs.AsQueryable();
            return listQuestion.OrderByDescending(x => x.CreatedDate);
        }

        public IEnumerable<QuestionDto> GetQuestionsByExamPaperId(int examPaperId)
        {
            DbContext.Configuration.ProxyCreationEnabled = false;
            var examPaperQuestions = DbContext.ExamPaperQuesions.Where(e => e.ExamPaperID == examPaperId).ToList();
            List<QuestionDto> questionsDto = new List<QuestionDto>();
            foreach (var item in examPaperQuestions)
            {
                var question = new Question();
                var questionDto = new QuestionDto();
                question = DbContext.Questions.SingleOrDefault(e => e.QuestionID == item.QuestionID);
                questionDto.IsActive = question.IsActive;
                questionDto.Content = question.Content;
                questionDto.Image = question.Image;
                questionDto.QuestionID = question.QuestionID;
                questionDto.CreatedBy = question.CreatedBy;
                questionDto.CreatedDate = question.CreatedDate;
                questionDto.ModifiedBy = question.ModifiedBy;
                questionDto.ModifiedDate = question.ModifiedDate;
                questionDto.CategoryID = question.CategoryID;
                questionDto.CategoryName = DbContext.QuestionCategories.SingleOrDefault(q => q.CategoryID == question.CategoryID).Name;
                questionDto.ExamPaperQuestionID = item.ExamPaperQuesionID;
                questionsDto.Add(questionDto);
            }
            return questionsDto;
        }

        public IEnumerable<QuestionDto> GetQuestionsByQuestionCategoryIdAndExamPaperId(int categoryId, int examPaperId)
        {
            DbContext.Configuration.ProxyCreationEnabled = false;

            List<int> temQuestionId = new List<int>();
            List<ExamPaperQuesion> examPaperQuesions = new List<ExamPaperQuesion>();
            examPaperQuesions = examPaperQuestionRepository.GetExamPaperQuesionsByExamPaperId(examPaperId).ToList();
            foreach (var item in examPaperQuesions)
            {
                temQuestionId.Add(item.QuestionID);
            }

            var questions = DbContext.Questions.Where(e => e.CategoryID == categoryId).ToList();
            List<QuestionDto> questionsDto = new List<QuestionDto>();
            foreach (var item in questions)
            {
                int i = 0;
                foreach (var id in temQuestionId)
                {
                    if (item.QuestionID == id)
                    {
                        i++;
                        break;
                    }
                }
                if (i == 0)
                {
                    var questionDto = new QuestionDto();
                    questionDto.IsActive = item.IsActive;
                    questionDto.Content = item.Content;
                    questionDto.Image = item.Image;
                    questionDto.CreatedBy = item.CreatedBy;
                    questionDto.CreatedDate = item.CreatedDate;
                    questionDto.ModifiedBy = item.ModifiedBy;
                    questionDto.ModifiedDate = item.ModifiedDate;
                    questionDto.CategoryID = item.CategoryID;
                    questionDto.CategoryName = DbContext.QuestionCategories.SingleOrDefault(q => q.CategoryID == item.CategoryID).Name;
                    questionDto.QuestionID = item.QuestionID;
                    questionsDto.Add(questionDto);
                }
            }
            return questionsDto;
        }

        public IEnumerable<QuestionDto> RandomQuestionsByCategoryIdAndExamPaperIdAndNumber(int categoryId, int examPaperId, int number)
        {
            List<QuestionDto> tempQuestionDtos = new List<QuestionDto>();
            tempQuestionDtos = GetQuestionsByQuestionCategoryIdAndExamPaperId(categoryId, examPaperId).ToList();
            if (tempQuestionDtos.Count <= number)
            {
                return tempQuestionDtos;
            }
            else
            {
                List<QuestionDto> questionDtos = new List<QuestionDto>();
                int length = tempQuestionDtos.Count();
                List<int> indexs = new List<int>();
                for (int i = 0; i < number; i++)
                {
                    int index = 0;
                    Random rnd = new Random();
                    do
                    {
                        index = rnd.Next(0, length);
                    }
                    while (indexs.Contains(index));
                    indexs.Add(index);
                    questionDtos.Add(tempQuestionDtos[index]);
                }
                return questionDtos;
            }

        }
        public IEnumerable<Answer> GetAnswersByQuestionId(int? id)
        {
            var listAnswer = DbContext.Answers.Where(x => x.QuestionID == id);
            return listAnswer.ToList();
        }

        public IEnumerable<Question> GetAllQuestions()
        {
            return DbContext.Questions.ToList();
        }

        public IEnumerable<Answer> GetAllAnswers()
        {
            return DbContext.Answers.ToList();
        }

        public IEnumerable<Level> GetAlLevels()
        {
            List<Level> listLevels = new List<Level>();
            listLevels.Add(new Level { LevelId = 1, LevelStep = 1, Name = "Easy" });
            listLevels.Add(new Level { LevelId = 2, LevelStep = 2, Name = "Normal" });
            listLevels.Add(new Level { LevelId = 3, LevelStep = 3, Name = "Hard" });
            return listLevels;
        }

        public string GetNameLevelByQuestionID(int id)
        {
            var name = DbContext.Questions.Find(id);
            if (name.Level == 1)
            {
                return "Easy";
            }
            if (name.Level == 2)
            {
                return "Normal";
            }
            if (name.Level == 3)
            {
                return "Hard";
            }
            else
            {
                return "None";
            }
        }

        public QuestionDto GetQuestionInQuestionDTO(int? id, QuestionFilterModel searchModel)
        {
            var question = GetAllQuestionDtos(searchModel).SingleOrDefault(x => x.QuestionID == id);
            return question;
        }

        public bool CheckQuestionInExamPaperQuesion(int id)
        {
            var question = DbContext.ExamPaperQuesions.SingleOrDefault(x => x.QuestionID == id);
            if (question == null)
            {
                return false;//Not Exist
            }
            else
            {
                return true;// Exist
            }
        }
    }
}
