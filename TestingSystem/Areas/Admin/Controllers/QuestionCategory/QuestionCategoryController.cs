using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TestingSystem.BaseController;
using TestingSystem.Sevice;

namespace TestingSystem.Areas.Admin.Controllers.QuestionCategory
{
    public class QuestionCategoryController : AdminController
    {
        // GET: Admin/QuestionCategory
        public readonly IQuestionCategorySevice questionCategorySevice;

        public QuestionCategoryController(IUserService a,IQuestionCategorySevice questionCategorySevice):base(a)
        {
            this.questionCategorySevice = questionCategorySevice;
        }
        public ActionResult Index()
        {

            var listCategories = new List<Models.QuestionCategory>();
            listCategories = questionCategorySevice.GetAll().ToList();
            ViewBag.ListCategories = listCategories;
            return View();
        }

        [HttpPost]
        public ActionResult Index(string KeySearch)
        {
            var listCategories = new List<Models.QuestionCategory>();
            listCategories = questionCategorySevice.SearchCategories(KeySearch).ToList();
            ViewBag.ListCategories = listCategories;
            return View();
        }
        //[ActionName("GetCategories")]
        public ActionResult GetCategories( /*string txtSearch, int? page*/)
        {
            var listCategories = new List<Models.QuestionCategory>();
            listCategories = questionCategorySevice.GetAll().ToList();
            ViewBag.ListCategories = listCategories;
            return View();
        }

        public ActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddCategory(Models.QuestionCategory category)
        {
            category.ModifiedBy = 1;
            category.CreatedBy = 1;
            try
            {
                if (ModelState.IsValid)
                {
                    if (category.CategoryID == 0)
                    {
                        if (questionCategorySevice.AddCategoryQuestion(category) > 0)
                        {
                            Success = "Insert question category successfully!";
                            return RedirectToAction("AddCategory", "QuestionCategory");
                        }
                    }
                    else
                    {
                        category.ModifiedBy = 1;
                        if (questionCategorySevice.UpdateCategoryQuestion(category) > 0)
                        {
                            Success = "Update exam paper successfully!";
                            return RedirectToAction("AddCategory", "QuestionCategory");
                        }
                    }
                }
                Success = "Update exam paper successfully!";
                return RedirectToAction("AddCategory", "QuestionCategory");
            }
            catch (Exception exception)
            {
                Failure = exception.Message;
                return View(category);
            }
        }
       
        public ActionResult EditCategory(int questionCategory)
        {
            var list = questionCategorySevice.GetById(questionCategory);
            return View(list);
        }
        [HttpPost]
        public ActionResult EditCategory(Models.QuestionCategory questionCategory)
        {
            questionCategory.ModifiedBy = 1;
            questionCategory.CreatedBy = 1;
            try
            {
               
                    if (questionCategorySevice.UpdateCategoryQuestion(questionCategory) > 0)
                    {
                        Success = "Update question category successfully!";
                        return RedirectToAction("Index", "QuestionCategory");
                    }

                    else
                    {
                        Success = "Update question category false!";
                    }
                
                Failure = "Something went wrong, please try again!";
                return RedirectToAction("EditCategory", "QuestionCategory");
            }
            catch (Exception exception)
            {
                Failure = exception.Message;
                return View(questionCategory);
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
                        if (questionCategorySevice.QuestionCategoryID(Convert.ToInt32(id)) == false)
                        {
                            Success = "No Delete";
                        }
                        else
                        {
                            if (questionCategorySevice.Delete(id) > 0)
                            {
                                i++;
                                continue;
                            }
                            else
                            {

                                break;
                            }
                        }

                    }
                    if (i > 0)
                    {
                        Success = "Delete Question Category Successfully!";
                        return RedirectToAction("Index", "QuestionCategory");
                    }
                    else
                    {
                        Failure = "Can not delete Category because Category in Question";
                        return RedirectToAction("Index", "QuestionCategory");
                    }
                }
                else
                {
                    Failure = "Can not delete Category because Category in Question";
                    return RedirectToAction("Index", "QuestionCategory");
                }

            }
            catch (Exception exception)
            {
                Failure = exception.Message;
                return RedirectToAction("Index", "QuestionCategory");
            }
        }


    }
}