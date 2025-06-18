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
    public class LoaiBangLaiService
    {
        private ApplicationDbContext _dbContext;
        private IRepository<LoaiBangLais> _loaiBangLaiRepository;

        public LoaiBangLaiService(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
            this._loaiBangLaiRepository = new LoaiBangLaiRepository(dbContext);
        }

        public List<LoaiBangLais> GetAllLoaiBangLais()
        {
            return _loaiBangLaiRepository.GetAll().ToList();
        }

        public LoaiBangLais GetLoaiBangLaiById(Guid id)
        {
            return _loaiBangLaiRepository.GetById(id);
        }

        public LoaiBangLais GetLoaiBangLaiByName(string tenLoaiBangLai)
        {
            return _loaiBangLaiRepository.GetByCondition(l => l.TenLoaiBangLai == tenLoaiBangLai);
        }

        public LoaiBangLais GetLoaiBangLaiByLoaiXe(string loaiXe)
        {
            return _loaiBangLaiRepository.GetByCondition(l => l.LoaiXe == loaiXe);
        }

        public List<LoaiBangLais> GetLoaiBangLaisWithQuestions()
        {
            return _loaiBangLaiRepository.GetList(
                includeProperties: "Questions"
            ).ToList();
        }

        public LoaiBangLais GetLoaiBangLaiWithQuestions(Guid id)
        {
            var loaiBangLai = _loaiBangLaiRepository.GetList(
                filter: l => l.Id == id,
                includeProperties: "Questions"
            ).FirstOrDefault();

            return loaiBangLai;
        }

        public void CreateLoaiBangLai(LoaiBangLais loaiBangLai)
        {
            if (loaiBangLai.Id == Guid.Empty)
            {
                loaiBangLai.Id = Guid.NewGuid();
            }
            _loaiBangLaiRepository.Add(loaiBangLai);
            _loaiBangLaiRepository.Save();
        }

        public void UpdateLoaiBangLai(LoaiBangLais loaiBangLai)
        {
            _loaiBangLaiRepository.Update(loaiBangLai);
            _loaiBangLaiRepository.Save();
        }

        public void DeleteLoaiBangLai(Guid id)
        {
            var loaiBangLai = _loaiBangLaiRepository.GetById(id);
            if (loaiBangLai != null)
            {
                _loaiBangLaiRepository.Delete(loaiBangLai);
                _loaiBangLaiRepository.Save();
            }
        }

        public bool IsLoaiBangLaiExists(Guid id)
        {
            return _loaiBangLaiRepository.Any(l => l.Id == id);
        }
    }
}