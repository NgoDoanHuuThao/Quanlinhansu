namespace QuanLyNhanSu.Models.DTO
{
    public class ImageDTO
    {
        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public string FileExtension { get; set; } = string.Empty;

        public long FileSizeInBytes { get; set; }
    }
}
