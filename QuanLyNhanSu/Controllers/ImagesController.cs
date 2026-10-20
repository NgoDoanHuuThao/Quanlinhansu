using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhanSu.Models.DTO;
using QuanLyNhanSu.Repositories;


namespace QuanLyNhanSu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        // POST: api/Images/upload
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(
            [FromForm] ImageUploadRequestDTO request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest("Vui lòng chọn ảnh.");
            }

            var allowedExtensions = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".gif"
            };

            var extension =
                Path.GetExtension(request.File.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(
                    "Chỉ cho phép file JPG, JPEG, PNG hoặc GIF.");
            }

            var result = await _imageRepository.Upload(request);

            return Ok(result);
        }

        // GET: api/Images
        [HttpGet]
        public async Task<IActionResult> GetAllImages()
        {
            var images =
                await _imageRepository.GetInfoAllImages();

            return Ok(images);
        }

        // GET: api/Images/{fileName}
        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetInfo(
            string fileName)
        {
            var image =
                await _imageRepository.GetInfo(fileName);

            if (image == null)
            {
                return NotFound("Không tìm thấy ảnh.");
            }

            return Ok(image);
        }

        // GET: api/Images/download/{fileName}
        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> Download(
            string fileName)
        {
            var file =
                await _imageRepository.Download(fileName);

            if (file == null)
            {
                return NotFound("Không tìm thấy ảnh.");
            }

            var extension =
                Path.GetExtension(fileName).ToLower();

            var contentType = extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };

            return File(
                file,
                contentType,
                fileName);
        }
    }
}
