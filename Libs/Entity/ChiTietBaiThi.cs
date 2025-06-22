namespace Libs.Entity
{
    public class ChiTietBaiThi
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid BaiThiId { get; set; }           
        public Guid CauHoiId { get; set; }           
      
        public virtual BaiThi BaiThi { get; set; }
        public virtual CauHoi CauHoi { get; set; }
    }
}
