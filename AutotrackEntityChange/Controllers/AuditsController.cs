using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutotrackEntityChange.DBContext;

namespace AutotrackEntityChange.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuditsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Audits/5
        [HttpGet("{id}")]
        /*
         * The get audit method returns list of audit history assocaited with the entity defined in the id.
         */
        public async Task<ActionResult<List<Audit>>> GetAudit(Guid id)
        {
            var audit = await _context.Audits.Where(k=>k.EntityId == id).ToListAsync();

            if (audit == null)
            {
                return NotFound();
            }
            return audit;
        }
    }
}
