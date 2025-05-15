using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRSbetta.Data;
using PRSbetta.Models;

namespace PRSbetta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController : ControllerBase
    {
        private readonly PrsbettaContext _context;

        public VendorsController(PrsbettaContext context)
        {
            _context = context;
        }

        // GET: api/Vendors
        [HttpGet]
        public async Task<ActionResult<List<Vendor>>> GetVendors()
        {
            return Ok(await _context.Vendors.ToListAsync());
        }

        // GET: api/Vendors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> GetVendor(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);

            if (vendor == null)
                return NotFound();
            // returns an object with 200 status
            return Ok(vendor);
        }

       
        
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVendor(int id, Vendor vendor)
        {
            if (id != vendor.Id)
            {
                return BadRequest();
            }

            _context.Entry(vendor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VendorExists(id))
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

        [HttpPut("Vendors/{id}")]
        public async Task<IActionResult> UpdateVendor(int id, Vendor updateVendor)
        {
            if (id != updateVendor.Id)
                return BadRequest();

            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
                return NotFound();

            vendor.Name = updateVendor.Name;
            vendor.Address = updateVendor.Address;
            vendor.City = updateVendor.City;
            vendor.State = updateVendor.State;
            vendor.Zip = updateVendor.Zip;
            vendor.PhoneNumber = updateVendor.PhoneNumber;
            vendor.Email = updateVendor.Email;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Vendors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Vendor>> AddVendor(Vendor newVendor)
        {
            if (newVendor == null)
                return BadRequest();

            _context.Vendors.Add(newVendor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(AddVendor), new { id = newVendor.Id }, newVendor);
        }

        // DELETE: api/Vendors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null)
                return NotFound();


            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VendorExists(int id)
        {
            return _context.Vendors.Any(e => e.Id == id);
        }
    }
}
