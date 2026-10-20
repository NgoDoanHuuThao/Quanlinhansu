using Microsoft.EntityFrameworkCore;
using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models.Domain;
using QuanLyNhanSu.Models.DTO;
namespace QuanLyNhanSu.Repositories
{
    public class SQLThongBaoRepository : IThongBaoRepository
    {
        private readonly AppDbContext db;

        public SQLThongBaoRepository(AppDbContext db)
        {
            this.db = db;
        }

        public List<ThongBaoDTO> GetThongBaoCuaNhanVien(int nhanVienId)
        {
            var homNay = DateTime.Now;

            return db.ThongBaoNguoiNhans
                .Where(x =>
                    x.NhanVienId == nhanVienId &&
                    x.ThongBao != null &&
                    x.ThongBao.TrangThai &&
                    (x.ThongBao.NgayHetHan == null ||
                     x.ThongBao.NgayHetHan >= homNay))
                .OrderByDescending(x => x.ThongBao!.NgayTao)
                .Select(x => new ThongBaoDTO
                {
                    ThongBaoId = x.ThongBaoId,
                    TieuDe = x.ThongBao!.TieuDe,
                    NoiDung = x.ThongBao.NoiDung,
                    DoiTuongNhan = x.ThongBao.DoiTuongNhan,
                    PhongBanId = x.ThongBao.PhongBanId,
                    NguoiGuiId = x.ThongBao.NguoiGuiId,
                    NgayTao = x.ThongBao.NgayTao,
                    NgayHetHan = x.ThongBao.NgayHetHan,
                    TrangThai = x.ThongBao.TrangThai,
                    DaDoc = x.DaDoc
                })
                .ToList();
        }

        public List<ThongBaoDTO> GetAllThongBao()
        {
            return db.ThongBaos
                .OrderByDescending(x => x.NgayTao)
                .Select(x => new ThongBaoDTO
                {
                    ThongBaoId = x.ThongBaoId,
                    TieuDe = x.TieuDe,
                    NoiDung = x.NoiDung,
                    DoiTuongNhan = x.DoiTuongNhan,
                    PhongBanId = x.PhongBanId,
                    NguoiGuiId = x.NguoiGuiId,
                    NgayTao = x.NgayTao,
                    NgayHetHan = x.NgayHetHan,
                    TrangThai = x.TrangThai,
                    DaDoc = false
                })
                .ToList();
        }

        public ThongBaoDTO? GetThongBaoById(int id)
        {
            return db.ThongBaos
                .Where(x => x.ThongBaoId == id)
                .Select(x => new ThongBaoDTO
                {
                    ThongBaoId = x.ThongBaoId,
                    TieuDe = x.TieuDe,
                    NoiDung = x.NoiDung,
                    DoiTuongNhan = x.DoiTuongNhan,
                    PhongBanId = x.PhongBanId,
                    NguoiGuiId = x.NguoiGuiId,
                    NgayTao = x.NgayTao,
                    NgayHetHan = x.NgayHetHan,
                    TrangThai = x.TrangThai,
                    DaDoc = false
                })
                .FirstOrDefault();
        }

        public ThongBaoDTO AddThongBao(
            AddThongBaoRequestDTO request,
            int nguoiGuiId)
        {
            var doiTuong = request.DoiTuongNhan.Trim().ToUpper();

            if (doiTuong != "ALL" && doiTuong != "PHONGBAN")
                throw new ArgumentException(
                    "Đối tượng nhận phải là ALL hoặc PHONGBAN.");

            if (doiTuong == "PHONGBAN" &&
                request.PhongBanId == null)
                throw new ArgumentException(
                    "Vui lòng chọn phòng ban nhận thông báo.");

            if (doiTuong == "ALL" &&
                request.PhongBanId != null)
                throw new ArgumentException(
                    "Thông báo gửi tất cả không cần chọn phòng ban.");

            var thongBao = new ThongBao
            {
                TieuDe = request.TieuDe.Trim(),
                NoiDung = request.NoiDung.Trim(),
                DoiTuongNhan = doiTuong,
                PhongBanId = request.PhongBanId,
                NguoiGuiId = nguoiGuiId,
                NgayTao = DateTime.Now,
                NgayHetHan = request.NgayHetHan,
                TrangThai = true
            };

            db.ThongBaos.Add(thongBao);
            db.SaveChanges();

            var nhanViens = db.NhanViens.AsQueryable();

            if (doiTuong == "PHONGBAN")
            {
                nhanViens = nhanViens.Where(
                    n => n.PhongBanId == request.PhongBanId);
            }

            var danhSachNguoiNhan = nhanViens
                .Select(n => n.NhanVienId)
                .ToList()
                .Select(id => new ThongBaoNguoiNhan
                {
                    ThongBaoId = thongBao.ThongBaoId,
                    NhanVienId = id,
                    DaDoc = false
                })
                .ToList();

            if (danhSachNguoiNhan.Count > 0)
            {
                db.ThongBaoNguoiNhans.AddRange(danhSachNguoiNhan);
                db.SaveChanges();
            }

            return GetThongBaoById(thongBao.ThongBaoId)!;
        }

        public bool CapNhatDaDoc(int thongBaoId, int nhanVienId)
        {
            var nguoiNhan = db.ThongBaoNguoiNhans
                .FirstOrDefault(x =>
                    x.ThongBaoId == thongBaoId &&
                    x.NhanVienId == nhanVienId);

            if (nguoiNhan == null)
                return false;

            nguoiNhan.DaDoc = true;
            nguoiNhan.NgayDoc = DateTime.Now;

            db.SaveChanges();
            return true;
        }

        public bool DeleteThongBao(int id)
        {
            var thongBao = db.ThongBaos
                .FirstOrDefault(x => x.ThongBaoId == id);

            if (thongBao == null)
                return false;

            db.ThongBaos.Remove(thongBao);
            db.SaveChanges();
            return true;
        }
    }
}
