using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libs.Data;
using Libs.Entity;
using Libs.Repositories;

namespace Libs.Service
{
    public class ChuDeService
    {
        private ApplicationDbContext _dbContext;
        private IRepository<ChuDes> _chuDeRepository;

        public ChuDeService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
            this._chuDeRepository = new ChuDeRepository(dbContext);
        }

        public List<ChuDes> GetAllChuDes()
        {
            return _chuDeRepository.GetAll().ToList();
        }

        public ChuDes GetChuDeById(Guid id)
        {
            return _chuDeRepository.GetById(id);
        }

        public ChuDes GetChuDeWithQuestions(Guid id)
        {
            var chuDe = _chuDeRepository.GetList(
                filter: c => c.Id == id,
                includeProperties: "Questions"
            ).FirstOrDefault();

            return chuDe;
        }

        public List<ChuDes> GetChuDesWithQuestions()
        {
            return _chuDeRepository.GetList(
                includeProperties: "Questions"
            ).ToList();
        }

        public void CreateChuDe(ChuDes chuDe)
        {
            if (chuDe.Id == Guid.Empty)
            {
                chuDe.Id = Guid.NewGuid();
            }
            _chuDeRepository.Add(chuDe);
            _chuDeRepository.Save();
        }

        public void UpdateChuDe(ChuDes chuDe)
        {
            _chuDeRepository.Update(chuDe);
            _chuDeRepository.Save();
        }

        public void DeleteChuDe(Guid id)
        {
            var chuDe = _chuDeRepository.GetById(id);
            if (chuDe != null)
            {
                _chuDeRepository.Delete(chuDe);
                _chuDeRepository.Save();
            }
        }

        public bool IsChuDeExists(Guid id)
        {
            return _chuDeRepository.Any(c => c.Id == id);
        }
    }
}