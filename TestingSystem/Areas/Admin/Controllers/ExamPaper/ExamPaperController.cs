using System.Collections.Generic;
using System.Web.Mvc;
using TestingSystem.Sevice;
using System.Linq;
using System;
using System.IO;
using System.Web;
using Excel = Microsoft.Office.Interop.Excel;
using TestingSystem.Models;
using Rotativa.MVC;
using TestingSystem.BaseController;
using TestingSystem.DataTranferObject.Question;

namespace TestingSystem.Areas.Admin.Controllers.ExamPaper
{
    public class ExamPaperController : AdminController
    {
        private readonly IExamPaperService examPaperService;
        private readonly IQuestionService questionService;
        private readonly IAnswerService answerService;
        private readonly IExamPaperQuestionService examPaperQuestionService;



        public ExamPaperController(IUserService userService,
            IExamPaperService examPaperService, IQuestionService questionService, 
            IAnswerService answerService, IExamPaperQuestionService examPaperQuestionService):base(userService)
        {
            this.examPaperService = examPaperService;
            this.questionService = questionService;
            this.answerService = answerService;
            this.examPaperQuestionService = examPaperQuestionService;
        }


        public ActionResult ExamPapers()
        {
            return View();
        }

        [ActionName("GetExamPapers")]
        public ActionResult GetExamPapers()
        {
            var examPapers = new List<TestingSystem.Models.ExamPaper>();
            examPapers = examPaperService.GetAll().ToList();
            return Json(new { data = examPapers }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [ActionName("ExamPaper")]
        public ActionResult ExamPaper(int? examPaperId)
        {
            var model = new Models.ExamPaper();

            if (examPaperId == null || examPaperId == 0)
            {
                ViewBag.IsUpdate = false;
                return View(model);
            }
            model = examPaperService.GetExamPaperById(examPaperId.Value);
            if (model != null)
            {

            }
            ViewBag.Status = model.Status;
            ViewBag.IsUpdate = true;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("ExamPaper")]
        public ActionResult ExamPaper(Models.ExamPaper examPaper)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (examPaper.ExamPaperID == 0)
                    {
                        examPaper.CreatedDate = DateTime.Now;
                        examPaper.CreatedBy = 1;
                        examPaper.ModifiedBy = 1;
                        if (examPaperService.Create(examPaper) > 0)
                        {
                            Success = "Insert exam paper successfully!";
                            return RedirectToAction("ExamPapers");
                        }
                    }
                    else
                    {
                        examPaper.ModifiedDate = DateTime.Now;
                        examPaper.ModifiedBy = 1;
                        if (examPaperService.Edit(examPaper) > 0)
                        {
                            Success = "Update exam paper successfully!";
                            return RedirectToAction("ExamPapers");
                        }
                    }
                }
                Failure = "Something went wrong, please try again!";
                return new JsonResult { Data = new { status = false } };
            }
            catch (Exception exception)
            {
                Failure = exception.Message;
                return View(examPaper);
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
                        if (examPaperService.Delete(id) > 0)
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

        public ActionResult ImportExamPaper()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportExamPaper(HttpPostedFileBase excelfile)
        {
            if (excelfile == null)
            {
                Failure = "Please choose excel file to import exam paper";
                return RedirectToAction("ImportExamPaper");

            }
            else
            {
                if (excelfile.FileName.EndsWith("xls") || excelfile.FileName.EndsWith("xlsx"))
                {
                    try
                    {
                        string path = Path.Combine(Server.MapPath("~/FileExcel/"), Guid.NewGuid().ToString() + Path.GetExtension(excelfile.FileName));
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
                        int examPaperId = examPaperService.Create(examPaper);
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
                            examPaperQuestionService.InsertExamPaperQuestion(examPaperId, questionId);

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
                    }
                    catch (Exception ex)
                    {
                        Failure = ex.Message;
                        return RedirectToAction("ImportExamPaper");
                    }
                    Success = "Import exam paper successfully!";
                    return RedirectToAction("ExamPapers");
                }
                else
                {
                    Failure = "Please choose excel file to import exam paper";
                    return RedirectToAction("ImportExamPaper");
                }
            }
        }

        public ActionResult ExportToPdf(int examPaperId)
        {
            try
            {
                Models.ExamPaper examPaper = new Models.ExamPaper();
                examPaper = examPaperService.GetExamPaperById(examPaperId);
                List<QuestionDto> questions = new List<QuestionDto>();
                questions = questionService.GetQuestionsByExamPaperId(examPaper.ExamPaperID).ToList();
                List<Answer> answers = new List<Answer>();
                foreach (var item in questions)
                {
                    var answesTemp = questionService.GetAnswersByQuestionId(item.QuestionID);
                    answers.AddRange(answesTemp);
                }
                ViewBag.Answers = answers;
                ViewBag.ExamPaper = examPaper;
                return View(questions);
            }
            catch (Exception e)
            {
                Failure = e.Message;
                return Json(new { status = false }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult ExportToPdfView(int examPaperId)
        {
            try
            {
                Models.ExamPaper examPaper = new Models.ExamPaper();
                examPaper = examPaperService.GetExamPaperById(examPaperId);
                return new ActionAsPdf("ExportToPdf", new { examPaperId = examPaperId })
                {
                    FileName = Server.MapPath(examPaper.Title + ".pdf")
                };
            }
            catch (Exception e)
            {
                Failure = e.Message;
                return Json(new {status = false }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}