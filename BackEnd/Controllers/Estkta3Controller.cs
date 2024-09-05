using BackEnd.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Estkta3Controller : ControllerBase
    {

        private readonly HcfiDBContext _dbcontext;

        public Estkta3Controller(HcfiDBContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpGet]

        public async Task<IActionResult> get()
        {
            return Ok(await _dbcontext.Estkta3s.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> post([FromForm] Estkta3 estkta3)
        {
            await _dbcontext.Estkta3s.AddAsync(new Estkta3()
            {
                Name = estkta3.Name,
                Description = estkta3.Description,
            });
            _dbcontext.SaveChanges();
            return Ok("added successfully");
        }
    }
}
