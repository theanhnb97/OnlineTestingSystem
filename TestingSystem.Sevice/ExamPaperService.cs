﻿using System;
using System.Collections.Generic;
using System.Linq;
using TestingSystem.Data.Infrastructure;
using TestingSystem.Data.Repositories;
using TestingSystem.Models;
using TestingSystem.DataTranferObject;
namespace TestingSystem.Sevice
{
    public interface IExamPaperService
    {
        IQueryable<ExamPaper> Filter(ExamPaperFilterModel examPaperFilterModel);
        List<ExamPaper> Search(string keySearch);
        IEnumerable<ExamPaper> GetAll();
        int Create(ExamPaper examPaper);

        int Edit(ExamPaper examPaper);

        ExamPaper GetExamPaperById(int id);

        int Delete(int id);
    }
    public class ExamPaperService : IExamPaperService
    {
        private readonly IExamPaperRepository examPaperRepository;
        private readonly IUnitOfWork unitOfWork;
        public ExamPaperService(IExamPaperRepository examPaperRepository, IUnitOfWork unitOfWork)
        {
            this.examPaperRepository = examPaperRepository;
            this.unitOfWork = unitOfWork;
        }

        public int Create(ExamPaper examPaper)
        {
           return examPaperRepository.Create(examPaper);
        }

        public int Edit(ExamPaper examPaper)
        {
            return examPaperRepository.Edit(examPaper);
        }

        public ExamPaper GetExamPaperById(int id)
        {
            return examPaperRepository.FindById(id);
        }


        public IQueryable<ExamPaper> Filter(ExamPaperFilterModel examPaperFilterModel)
        {
            return examPaperRepository.Filter(examPaperFilterModel);
        }

        public IEnumerable<ExamPaper> GetAll()
        {
            return examPaperRepository.GetAll();
        }

        public List<ExamPaper> Search(string keySearch)
        {
            return examPaperRepository.Search(keySearch);
        }

        public int Delete(int id)
        {
           return examPaperRepository.Delete(id);
        }
    }
}
