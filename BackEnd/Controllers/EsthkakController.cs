using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EsthkakController : ControllerBase
    {
        private readonly HcfiDBContext _dbcontext;

        public EsthkakController(HcfiDBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpGet]

        public async Task<IActionResult> get()
        {
            return Ok( await _dbcontext.Esthkaks.ToListAsync());
        }

        [HttpPost]
        public async  Task<IActionResult> post([FromForm] Esthkak esthkak)
        {
          await  _dbcontext.Esthkaks.AddAsync(new Esthkak()
            {
                Name = esthkak.Name,
                Description = esthkak.Description,
            });
            _dbcontext.SaveChanges();
            return Ok("added successfully");
        }
    }
}
