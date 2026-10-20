using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models.Domain;
using QuanLyNhanSu.Models.DTO;
namespace QuanLyNhanSu.Repositories
{
    public class SQLPhongBanRepository : IPhongBanRepository
    {
        private readonly AppDbContext _context;

        public SQLPhongBanRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<PhongBanDTO> GetAllPhongBans()
        {
            return _context.PhongBans
                .Select(p => new PhongBanDTO
                {
                    PhongBanId = p.PhongBanId,
                    TenPhongBan = p.TenPhongBan,
                    MoTa = p.MoTa
                })
                .ToList();
        }

        public PhongBanDTO? GetPhongBanById(int id)
        {
            return _context.PhongBans
                .Where(p => p.PhongBanId == id)
                .Select(p => new PhongBanDTO
                {
                    PhongBanId = p.PhongBanId,
                    TenPhongBan = p.TenPhongBan,
                    MoTa = p.MoTa
                })
                .FirstOrDefault();
        }

        public PhongBanDTO AddPhongBan(PhongBanDTO phongBanDTO)
        {
            var phongBan = new PhongBan
            {
                TenPhongBan = phongBanDTO.TenPhongBan,
                MoTa = phongBanDTO.MoTa
            };

            _context.PhongBans.Add(phongBan);
            _context.SaveChanges();

            phongBanDTO.PhongBanId = phongBan.PhongBanId;

            return phongBanDTO;
        }

        public PhongBanDTO? UpdatePhongBanById(
            int id,
            PhongBanDTO phongBanDTO)
        {
            var phongBan = _context.PhongBans
                .FirstOrDefault(p => p.PhongBanId == id);

            if (phongBan == null)
            {
                return null;
            }

            phongBan.TenPhongBan = phongBanDTO.TenPhongBan;
            phongBan.MoTa = phongBanDTO.MoTa;

            _context.SaveChanges();

            phongBanDTO.PhongBanId = id;

            return phongBanDTO;
        }

        public bool DeletePhongBanById(int id)
        {
            var phongBan = _context.PhongBans
                .FirstOrDefault(p => p.PhongBanId == id);

            if (phongBan == null)
            {
                return false;
            }

            _context.PhongBans.Remove(phongBan);
            _context.SaveChanges();

            return true;
        }
    }
}
