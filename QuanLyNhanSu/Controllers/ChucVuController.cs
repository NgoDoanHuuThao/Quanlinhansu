using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhanSu.Data;
using QuanLyNhanSu.Models.Domain;
using QuanLyNhanSu.Models.DTO;
using QuanLyNhanSu.Repositories;

namespace QuanLyNhanSu.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChucVuController : ControllerBase
    {
        private readonly IChucVuRepository _repository;

        public ChucVuController(IChucVuRepository repository)
        {
            _repository = repository;
        }

        // GET: api/ChucVu
        [HttpGet]
        public IActionResult GetAll()
        {
            var chucVus = _repository.GetAllChucVus();

            return Ok(chucVus);
        }

        // GET: api/ChucVu/1
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var chucVu = _repository.GetChucVuById(id);

            if (chucVu == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy chức vụ"
                });
            }

            return Ok(chucVu);
        }

        // POST: api/ChucVu
        [HttpPost]
        public IActionResult Add(
            [FromBody] ChucVuDTO chucVuDTO)
        {
            var chucVu = _repository.AddChucVu(chucVuDTO);

            return CreatedAtAction(
                nameof(GetById),
                new { id = chucVu.ChucVuId },
                chucVu);
        }

        // PUT: api/ChucVu/1
        [HttpPut("{id:int}")]
        public IActionResult Update(
            int id,
            [FromBody] ChucVuDTO chucVuDTO)
        {
            var chucVu =
                _repository.UpdateChucVuById(
                    id,
                    chucVuDTO);

            if (chucVu == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy chức vụ"
                });
            }

            return Ok(chucVu);
        }

        // DELETE: api/ChucVu/1
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var result =
                _repository.DeleteChucVuById(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy chức vụ"
                });
            }

            return Ok(new
            {
                message = "Xóa chức vụ thành công"
            });
        }
    }
}
