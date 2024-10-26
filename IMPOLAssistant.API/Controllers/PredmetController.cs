using FONAssistant.API.Data;
using FONAssistant.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FONAssistant.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredmetController : ControllerBase
    {
        private readonly FonAssistentDbContext _context;

        public PredmetController(FonAssistentDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Predmet>>> GetPredmeti()
        {
            return await _context.Predmeti.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Predmet>> GetPredmet(int id)
        {
            var predmet = await _context.Predmeti.FindAsync(id);

            if (predmet == null)
            {
                return NotFound(); 
            }

            return predmet; 
        }
    }
}


