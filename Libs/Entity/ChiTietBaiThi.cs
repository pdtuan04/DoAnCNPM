namespace Libs.Entity
{
    public class ChiTietBaiThi
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid BaiThiId { get; set; }            // ✅ Đổi sang Guid
        public Guid CauHoiId { get; set; }            // ✅ Đổi sang Guid

        public char? CauTraLoi { get; set; }
        public bool? DungSai { get; set; }

        // Navigation properties
        public virtual BaiThi BaiThi { get; set; }
        public virtual CauHoi CauHoi { get; set; }
    }
}
