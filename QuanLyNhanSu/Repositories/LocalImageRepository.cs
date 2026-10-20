using QuanLyNhanSu.Models.DTO;
namespace QuanLyNhanSu.Repositories
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalImageRepository(
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        // Upload ảnh
        public async Task<ImageDTO> Upload(ImageUploadRequestDTO request)
        {
            var localPath = Path.Combine(
                _webHostEnvironment.ContentRootPath,
                "Images");

            // Nếu thư mục Images chưa tồn tại thì tạo
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }

            // Lấy phần mở rộng của file
            var extension = Path.GetExtension(request.File.FileName);

            // Tạo tên file mới để tránh trùng
            var fileName = $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(localPath, fileName);

            // Lưu file
            using var stream = new FileStream(
                filePath,
                FileMode.Create);

            await request.File.CopyToAsync(stream);

            // Tạo đường dẫn truy cập ảnh
            var requestHttp = _httpContextAccessor.HttpContext?.Request;

            var fileUrl =
                $"{requestHttp?.Scheme}://{requestHttp?.Host}/Images/{fileName}";

            return new ImageDTO
            {
                FileName = fileName,
                FilePath = fileUrl,
                FileExtension = extension,
                FileSizeInBytes = request.File.Length
            };
        }

        // Lấy thông tin tất cả ảnh
        public Task<List<ImageDTO>> GetInfoAllImages()
        {
            var localPath = Path.Combine(
                _webHostEnvironment.ContentRootPath,
                "Images");

            if (!Directory.Exists(localPath))
            {
                return Task.FromResult(new List<ImageDTO>());
            }

            var files = Directory.GetFiles(localPath);

            var requestHttp = _httpContextAccessor.HttpContext?.Request;

            var images = files.Select(filePath => new ImageDTO
            {
                FileName = Path.GetFileName(filePath),

                FilePath =
                    $"{requestHttp?.Scheme}://{requestHttp?.Host}/Images/{Path.GetFileName(filePath)}",

                FileExtension = Path.GetExtension(filePath),

                FileSizeInBytes = new FileInfo(filePath).Length
            }).ToList();

            return Task.FromResult(images);
        }

        // Lấy thông tin một ảnh
        public Task<ImageDTO?> GetInfo(string fileName)
        {
            var localPath = Path.Combine(
                _webHostEnvironment.ContentRootPath,
                "Images",
                fileName);

            if (!File.Exists(localPath))
            {
                return Task.FromResult<ImageDTO?>(null);
            }

            var fileInfo = new FileInfo(localPath);

            var image = new ImageDTO
            {
                FileName = fileInfo.Name,
                FilePath = localPath,
                FileExtension = fileInfo.Extension,
                FileSizeInBytes = fileInfo.Length
            };

            return Task.FromResult<ImageDTO?>(image);
        }

        // Download ảnh
        public Task<FileStream?> Download(string fileName)
        {
            var localPath = Path.Combine(
                _webHostEnvironment.ContentRootPath,
                "Images",
                fileName);

            if (!File.Exists(localPath))
            {
                return Task.FromResult<FileStream?>(null);
            }

            var stream = new FileStream(
                localPath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);

            return Task.FromResult<FileStream?>(stream);
        }
    }
}
