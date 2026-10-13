using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTO.Output;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SportsController : ControllerBase
    {
        private readonly ScoreHubDbContext _context;

        public SportsController(ScoreHubDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<List<NavMenuSportsDTO>>> GetSports(CancellationToken ct)
        {
            var sports = await _context.Sports.
                OrderBy(x=>x.Id)
                .Select(x=> new NavMenuSportsDTO
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToListAsync(ct);

            return Ok(sports);
        }
    }
}
