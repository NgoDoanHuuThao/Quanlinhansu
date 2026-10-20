using QuanLyNhanSu.Models.DTO;
namespace QuanLyNhanSu.Repositories
{
    public interface IImageRepository
    {
        Task<ImageDTO> Upload(ImageUploadRequestDTO request);

        Task<List<ImageDTO>> GetInfoAllImages();

        Task<ImageDTO?> GetInfo(string fileName);

        Task<FileStream?> Download(string fileName);
    }
}
