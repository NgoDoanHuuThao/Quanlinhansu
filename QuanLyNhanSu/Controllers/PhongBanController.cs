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
    public class PhongBanController : ControllerBase
    {
        private readonly IPhongBanRepository _repository;

        public PhongBanController(IPhongBanRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_repository.GetAllPhongBans());
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var phongBan = _repository.GetPhongBanById(id);

            if (phongBan == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy phòng ban"
                });
            }

            return Ok(phongBan);
        }

        [HttpPost]
        public IActionResult Add(
            [FromBody] PhongBanDTO phongBanDTO)
        {
            var phongBan = _repository.AddPhongBan(phongBanDTO);

            return CreatedAtAction(
                nameof(GetById),
                new { id = phongBan.PhongBanId },
                phongBan);
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(
            int id,
            [FromBody] PhongBanDTO phongBanDTO)
        {
            var phongBan =
                _repository.UpdatePhongBanById(
                    id,
                    phongBanDTO);

            if (phongBan == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy phòng ban"
                });
            }

            return Ok(phongBan);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var result =
                _repository.DeletePhongBanById(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy phòng ban"
                });
            }

            return Ok(new
            {
                message = "Xóa phòng ban thành công"
            });
        }
    }
    
}
