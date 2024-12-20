using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Survivor.Data.Context;
using Survivor.Data.Entity;

namespace Survivor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompetitorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CompetitorsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Competitor>>> GetCompetitors()
        {
            return await _context.Competitors.Include(c => c.Category).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Competitor>> GetCompetitor(int id)
        {
            var competitor = await _context.Competitors.Include(c => c.Category).FirstOrDefaultAsync(c => c.Id == id);
            if (competitor == null) return NotFound();
            return competitor;
        }

        [HttpGet("categories/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Competitor>>> GetCompetitorsByCategory(int categoryId)
        {
            return await _context.Competitors.Where(c => c.CategoryId == categoryId).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Competitor>> CreateCompetitor(Competitor competitor)
        {
            _context.Competitors.Add(competitor);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCompetitor), new { id = competitor.Id }, competitor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompetitor(int id, Competitor competitor)
        {
            if (id != competitor.Id) return BadRequest();

            _context.Entry(competitor).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Competitors.Any(c => c.Id == id)) return NotFound();
                throw;
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompetitor(int id)
        {
            var competitor = await _context.Competitors.FindAsync(id);
            if (competitor == null) return NotFound();

            _context.Competitors.Remove(competitor);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

}
