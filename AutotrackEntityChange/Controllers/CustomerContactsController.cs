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
    public class CustomerContactsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomerContactsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CustomerContacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerContact>>> GetCustomerContacts()
        {
            return await _context.CustomerContacts.ToListAsync();
        }

        // GET: api/CustomerContacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerContact>> GetCustomerContact(Guid id)
        {
            var customerContact = await _context.CustomerContacts.FindAsync(id);

            if (customerContact == null)
            {
                return NotFound();
            }

            return customerContact;
        }

        // PUT: api/CustomerContacts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomerContact(Guid id, CustomerContact customerContact)
        {
            if (id != customerContact.Id)
            {
                return BadRequest();
            }

            _context.Entry(customerContact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerContactExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/CustomerContacts
        [HttpPost]
        public async Task<ActionResult<CustomerContact>> PostCustomerContact(CustomerContact customerContact)
        {
            _context.CustomerContacts.Add(customerContact);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomerContact", new { id = customerContact.Id }, customerContact);
        }

        // DELETE: api/CustomerContacts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CustomerContact>> DeleteCustomerContact(Guid id)
        {
            var customerContact = await _context.CustomerContacts.FindAsync(id);
            if (customerContact == null)
            {
                return NotFound();
            }

            _context.CustomerContacts.Remove(customerContact);
            await _context.SaveChangesAsync();

            return customerContact;
        }

        private bool CustomerContactExists(Guid id)
        {
            return _context.CustomerContacts.Any(e => e.Id == id);
        }
    }
}
