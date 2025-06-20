namespace Libs.Entity
{
    public class GiaoDich
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public string MaGiaoDich { get; set; }
        public decimal SoTien { get; set; }
        public DateTime NgayThanhToan { get; set; }
        public bool DaThanhToan { get; set; }

    }

}
