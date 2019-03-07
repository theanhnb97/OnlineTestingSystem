using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestingSystem.BaseController;
using TestingSystem.DataTranferObject.Question;
using TestingSystem.Sevice;

namespace TestingSystem.Areas.Admin.Controllers.ExamPaperQuestion
{
    public class ExamPaperQuestionController : AdminController
    {
        private readonly IExamPaperQuestionService examPaperQuestionService;
        private readonly IQuestionService questionService;


        public ExamPaperQuestionController(IUserService a,
            IExamPaperQuestionService examPaperQuestionService,IQuestionService questionService):base(a)
        {
            this.examPaperQuestionService = examPaperQuestionService;
            this.questionService = questionService;
        }

        public ActionResult GetExamPaperQuestionsByExamPaperId(int examPaperId)
        {
            var examPaperQuestions = new List<TestingSystem.Models.ExamPaperQuesion>();
            examPaperQuestions = examPaperQuestionService.GetExamPaperQuestionsByExamPaperId(examPaperId).ToList();
            return Json(new { data = examPaperQuestions }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(List<int> ids)
        {
            try
            {
                if (ids.Count > 0)
                {
                    int i = 0;
                    foreach (var id in ids)
                    {
                        if (examPaperQuestionService.DeleteExamPaperQuestion(id) > 0)
                        {
                            i++;
                            continue;
                        }
                        else
                        {
                            //!!!!!!!!!!! break nhưng mà những cái record trc đó vẫn đã bị xóa
                            break;
                        }

                    }
                    if (i > 0)
                    {
                        Success = "Delete exam paper successfully!";
                        return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                    }
                }
                Failure = "Something went wrong, please try again!";
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Failure = exception.Message;
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Insert(int examPaperId,int questionId)
        {
            try
            {
                if (examPaperQuestionService.InsertExamPaperQuestion(examPaperId, questionId) > 0)
                {
                    Success = "Add question into exam paper successfully!";
                    return Json(new { status = true }, JsonRequestBehavior.AllowGet);
                }

                Failure = "Something went wrong, please try again!";
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Failure = exception.Message;
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RandomQuestionsByCategoryIdAndExamPaperIdAndNumber(int categoryId,int examPaperId, int number)
        {
            try
            {
                List<QuestionDto> questionDtos = new List<QuestionDto>();
                questionDtos = questionService.RandomQuestionsByCategoryIdAndExamPaperIdAndNumber(categoryId, examPaperId, number).ToList();
                foreach(var item in questionDtos)
                {
                    examPaperQuestionService.InsertExamPaperQuestion(examPaperId, item.QuestionID);
                }
                Success = "Add question into exam paper successfully!";
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Failure = exception.Message;
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}