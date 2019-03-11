using System.Collections.Generic;
using System.Web.Mvc;
using TestingSystem.Sevice;
using TestingSystem.Models;
using TestingSystem.DataTranferObject.Question;
using System.Net;
using System.Web;
using System.IO;
using System.Linq;
using System;
using TestingSystem.BaseController;
using Excel = Microsoft.Office.Interop.Excel;

namespace TestingSystem.Areas.Admin.Controllers.Question
{
    public class QuestionController : AdminController, IDisposable
    {
        private readonly IQuestionService questionService;
        private readonly IAnswerService answerService;
        private readonly IQuestionCategorySevice questionCategorySevice;
        private readonly IExamPaperService examPaperService;


        public QuestionController(IUserService user,
            IQuestionService questionService, IAnswerService answerService,
            IQuestionCategorySevice questionCategorySevice, IExamPaperService examPaperService) : base(user)
        {
            this.questionService = questionService;
            this.answerService = answerService;
            this.questionCategorySevice = questionCategorySevice;
            this.examPaperService = examPaperService;
            //
        }

        [HttpPost]
        public JsonResult AddCategory(Models.QuestionCategory category)
        {
            category.ModifiedBy = int.Parse(Session["Name"].ToString());
            category.CreatedBy = int.Parse(Session["Name"].ToString());
            // Default is true when create in CreateQuesiton View
            category.IsActive = true;
            return Json(questionCategorySevice.AddCategoryQuestion(category), JsonRequestBehavior.AllowGet);
        }
        public JsonResult _CheckCategoryNameAvailableCreate(string userdata)
        {
            try
            {
                var SeachData = questionCategorySevice.SearchCategories(userdata);
                if (SeachData.Count() > 0)
                {
                    return Json(1);
                }
                else
                {
                    return Json(0);
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public ActionResult Index(QuestionFilterModel searchModel)
        {
            var listCategory = questionCategorySevice.GetAllQuestionCategoriesActive();
            var listLevels = questionService.GetAlLevels();
            ViewData["Category"] = listCategory;
            ViewData["Level"] = listLevels;

            var listQuestionDtos = questionService.GetAllQuestionDtos(searchModel);
            ViewBag.listQuestionDtos = listQuestionDtos;
            return View();
        }
        //[ActionName("GetQuestions")]
        //public ActionResult GetQuestions(QuestionFilterModel searchModel)
        //{
        //    var listQuestionDtos = questionService.GetAllQuestionDtos(searchModel);
        //    return Json(new { data = listQuestionDtos.OrderBy(x => x.CategoryID) }, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult Search(string keySearch, QuestionFilterModel searchModel)
        {
            var listCategory = questionCategorySevice.GetAllQuestionCategoriesActive();
            var listLevels = questionService.GetAlLevels();
            ViewData["Category"] = listCategory;
            ViewData["Level"] = listLevels;

            var listQuestionDtos = questionService.SearchByContent(keySearch, searchModel);
            return View(listQuestionDtos);
        }
        public ActionResult Detail(int? id, QuestionFilterModel searchModel)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                ViewBag.listAnswerByQuestion = answerService.GetAnswersByQuestionID(id);
                var question = questionService.GetQuestionInQuestionDTO(id, searchModel);
                if (question == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                {
                    return View(question);
                }
            }

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
                        if (questionService.DeleteQuestion(id) > 0)
                        {
                            i++;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (i > 0)
                    {
                        Success = "Delete exam paper successfully!";
                        return RedirectToAction("Index", "Question");
                    }
                }
                Failure = "Something went wrong, please try again!";
                return RedirectToAction("Index", "Question");
            }
            catch (System.Exception exception)
            {
                Failure = exception.Message;
                return RedirectToAction("Index", "Question");
            }
        }

        public ActionResult Create()
        {
            // This is only for show by default one row for insert data to the database
            List<Answer> answers = new List<Answer>
            {
                new Answer() { AnswerID = 0, AnswerContent = "", IsCorrect = false },
                new Answer() { AnswerID = 0, AnswerContent = "", IsCorrect = false },
            };
            var listCategory = questionCategorySevice.GetAllQuestionCategoriesActive();
            var listLevels = questionService.GetAlLevels();
            ViewData["Category"] = listCategory;
            ViewData["Level"] = listLevels;
            return View(answers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Models.Question question, HttpPostedFileBase Image, List<Answer> listAnswers)
        {
            question.CreatedBy = int.Parse(Session["Name"].ToString());
            question.ModifiedBy = int.Parse(Session["Name"].ToString());
            if (Image != null && Image.ContentLength > 0)
            {
                string filePath = Path.Combine(Server.MapPath("~/Content/QuestionUpload/Images/"),
                    Path.GetFileName(Image.FileName));
                Image.SaveAs(filePath);
                question.Image = Image.FileName;
            }
            else
            {
                question.Image = null;
            }

            var addQuestion = questionService.AddQuestion(question);
            TranferID.ID = addQuestion;
            // Create Answer
            foreach (var i in listAnswers)
            {
                i.QuestionID = TranferID.ID;
                if (i.QuestionID <= 0)
                {
                    return RedirectToAction("Create", "Question");
                }
                else
                {
                    answerService.AddAnswer(i);
                }
            }

            return RedirectToAction("Index");

        }
        public ActionResult Edit(int id)
        {
            var listCategory = questionCategorySevice.GetAllQuestionCategoriesActive();
            var listLevels = questionService.GetAlLevels();
            ViewBag.listCategory = listCategory;
            ViewBag.listLevel = listLevels;
            //Get Answer.
            var listAnswerByQuestionID = questionService.GetAnswersByQuestionId(id);
            ViewBag.listAnswerByQuestionID = listAnswerByQuestionID;

            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                // dto
                QuestionAnswerDTO mymodel = new QuestionAnswerDTO();
                var question = questionService.FindID(id);
                var answer = questionService.GetAnswersByQuestionId(id);
                ViewBag.Answer = answer;
                mymodel.Question = question;
                mymodel.Answers = answer.ToList();

                if (question == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                {
                    return View(question);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Models.Question question, HttpPostedFileBase Image, int[] AnswerID, string[] AnswerContent, string[] IsCorrect)
        {
            List<Answer> listAnswer = new List<Answer>();
            for (int i = 0; i < AnswerID.Length; i++)
            {
                Answer answer = new Answer();
                answer.AnswerID = AnswerID[i];
                answer.AnswerContent = AnswerContent[i];
                answer.IsCorrect = IsCorrect.Contains(AnswerContent[i]);
                listAnswer.Add(answer);
            }
            question.ModifiedBy = int.Parse(Session["Name"].ToString());
            if (Image != null && Image.ContentLength > 0)
            {
                string filePath = Path.Combine(Server.MapPath("~/Content/QuestionUpload/Images/"),
                    Path.GetFileName(Image.FileName));
                Image.SaveAs(filePath);
                question.Image = Image.FileName;
            }
            else
            {
                var img = questionService.FindID(question.QuestionID).Image;
                question.Image = img;
            }

            questionService.UpdateQuestion(question);
            //
            answerService.DelteAnswerbyQuestionID(question.QuestionID);

            foreach (var item in listAnswer)
            {
                item.QuestionID = question.QuestionID;
                if (item.QuestionID <= 0)
                {
                    return RedirectToAction("Edit", "Question");
                }
                else
                {

                    answerService.AddAnswer(item);
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult GetQuestionsByExamPaperId(int examPaperId)
        {
            var questions = new List<TestingSystem.DataTranferObject.Question.QuestionDto>();
            questions = questionService.GetQuestionsByExamPaperId(examPaperId).ToList();

            return Json(new { data = questions }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetQuestionsByQuestionCategoryIdAndExamPaperId(int categoryId, int examPaperId)
        {
            var questions = new List<TestingSystem.DataTranferObject.Question.QuestionDto>();
            questions = questionService.GetQuestionsByQuestionCategoryIdAndExamPaperId(categoryId, examPaperId)
                .ToList();

            return Json(new { data = questions }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QuestionExcelAnswer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult QuestionExcelAnswer(HttpPostedFileBase excelfile)
        {
            if (excelfile == null)
            {
                ViewBag.ThongBao = "Please choose excel file to import exam paper!";
                return View();
            }
            else
            {
                if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
                {
                    string path = Path.Combine(Server.MapPath("~/FileExcel/"),
                        Guid.NewGuid().ToString() + Path.GetExtension(excelfile.FileName));
                    excelfile.SaveAs(path);
                    Excel.Application application = new Excel.Application
                    {
                        Visible = true
                    };
                    Excel.Workbook workbook = application.Workbooks.Open(path);
                    Excel.Worksheet worksheet = workbook.Sheets[@"ExamPaper"];
                    Excel.Range range = worksheet.UsedRange;


                    Models.ExamPaper examPaper = new Models.ExamPaper();
                    examPaper.Title = ((Excel.Range)range.Cells[3, 1]).Text;
                    examPaper.Time = int.Parse(((Excel.Range)range.Cells[4, 1]).Text);
                    examPaper.Status = Boolean.Parse(((Excel.Range)range.Cells[6, 1]).Text);
                    examPaper.IsActive = Boolean.Parse(((Excel.Range)range.Cells[5, 1]).Text);
                    examPaper.CreatedBy = 1;
                    examPaper.CreatedDate = DateTime.Now;
                    examPaper.ModifiedBy = 1;
                    examPaper.ModifiedDate = DateTime.Now;
                    int result = examPaperService.Create(examPaper);

                    for (int row = 11; row <= range.Rows.Count; row++)
                    {
                        Models.Question question = new Models.Question
                        {
                            Content = ((Excel.Range)range.Cells[row, 1]).Text,
                            Level = int.Parse(((Excel.Range)range.Cells[row, 2]).Text),
                            CategoryID = int.Parse(((Excel.Range)range.Cells[row, 3]).Text),
                            IsActive = true,
                            CreatedBy = 1,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = 1,
                            ModifiedDate = DateTime.Now
                        };
                        int questionId = questionService.AddQuestion(question);

                        Answer answer = new Answer();
                        int j = 5;
                        for (int i = 4; i <= 13; i += 2)
                        {
                            string content = ((Excel.Range)range.Cells[row, i]).Text;
                            if (content != "")
                            {
                                answer.AnswerContent = content;
                                answer.IsCorrect = Boolean.Parse(((Excel.Range)range.Cells[row, j]).Text);
                                answer.QuestionID = questionId;
                                answerService.AddAnswer(answer);
                            }
                            else
                            {
                                continue;
                            }
                            j += 2;
                        }

                    }
                    return RedirectToAction("Questions");
                }
                else
                {
                    ViewBag.ThongBao = "Please choose excel file to import exam paper!";
                    return View();
                }
            }
        }
    }
}