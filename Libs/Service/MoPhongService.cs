using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Libs.Entity;
using Libs.Repositories;

namespace Libs.Service
{
    public class MoPhongService
    {
        private ApplicationDbContext dbContext;
        private IMoPhongRepository moPhongRepository;
        public MoPhongService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.moPhongRepository = new MoPhongRepository(dbContext);
        }
        public void Save()
        {
            dbContext.SaveChanges();
        }
        public IEnumerable<MoPhong> GetMoPhongByLoaiBangLaiId(Guid loaiBangLai)
        {
            return moPhongRepository.GetMoPhongsByLoaiBangLaiId(loaiBangLai);
        }
        public IEnumerable<MoPhong> GetAllMoPhong()
        {
            return moPhongRepository.GetAllMoPhong();
        }
        public MoPhong GetMoPhongById(Guid id)
        {

            return moPhongRepository.GetMoPhongById(id);
        }
    }
}
