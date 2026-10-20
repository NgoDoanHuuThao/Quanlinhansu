using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models.Domain;
using QuanLyNhanSu.Models.DTO;
namespace QuanLyNhanSu.Repositories
{
    public class SQLChucVuRepository : IChucVuRepository
    {
        private readonly AppDbContext _context;

        public SQLChucVuRepository(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        public List<ChucVuDTO> GetAllChucVus()
        {
            return _context.ChucVus
                .Select(c => new ChucVuDTO
                {
                    ChucVuId = c.ChucVuId,
                    TenChucVu = c.TenChucVu,
                    MoTa = c.MoTa
                })
                .ToList();
        }

        // GET BY ID
        public ChucVuDTO? GetChucVuById(int id)
        {
            return _context.ChucVus
                .Where(c => c.ChucVuId == id)
                .Select(c => new ChucVuDTO
                {
                    ChucVuId = c.ChucVuId,
                    TenChucVu = c.TenChucVu,
                    MoTa = c.MoTa
                })
                .FirstOrDefault();
        }

        // ADD
        public ChucVuDTO AddChucVu(ChucVuDTO chucVuDTO)
        {
            var chucVu = new ChucVu
            {
                TenChucVu = chucVuDTO.TenChucVu,
                MoTa = chucVuDTO.MoTa
            };

            _context.ChucVus.Add(chucVu);
            _context.SaveChanges();

            chucVuDTO.ChucVuId = chucVu.ChucVuId;

            return chucVuDTO;
        }

        // UPDATE
        public ChucVuDTO? UpdateChucVuById(
            int id,
            ChucVuDTO chucVuDTO)
        {
            var chucVu = _context.ChucVus
                .FirstOrDefault(c => c.ChucVuId == id);

            if (chucVu == null)
            {
                return null;
            }

            chucVu.TenChucVu = chucVuDTO.TenChucVu;
            chucVu.MoTa = chucVuDTO.MoTa;

            _context.SaveChanges();

            chucVuDTO.ChucVuId = id;

            return chucVuDTO;
        }

        // DELETE
        public bool DeleteChucVuById(int id)
        {
            var chucVu = _context.ChucVus
                .FirstOrDefault(c => c.ChucVuId == id);

            if (chucVu == null)
            {
                return false;
            }

            _context.ChucVus.Remove(chucVu);
            _context.SaveChanges();

            return true;
        }
    }
}
