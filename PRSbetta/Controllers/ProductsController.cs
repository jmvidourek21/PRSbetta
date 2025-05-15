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
    public class ProductsController : ControllerBase
    {
        private readonly PrsbettaContext _context;

        public ProductsController(PrsbettaContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProducts()
        {
            var products = _context.Products.Include(p => p.Vendor);

            return Ok(await products.ToListAsync());
        }

        // GET: api/Products/5
        [HttpGet("{productId}")]
        public async Task<ActionResult<Product>> GetProductDetails(int productId)
        {
            var productDetails = _context.Products
                .Where(p => p.Id == productId)
                .Include(p => p.Vendor);
            if (productDetails == null)
                return NotFound();
            return Ok(await productDetails.ToListAsync());
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
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

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]  ////////////////autocode
        //public async Task<ActionResult<Product>> PostProduct(Product product)
        //{
        //    _context.Products.Add(product);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        //}

        ////////[HttpPost]
        ////////public async Task<ActionResult<Product>> AddProduct(Product newProduct)
        ////////{
        ////////    bool vendorExists = await _context.Vendors.AnyAsync(v => v.Id == newProduct.VendorId);
        ////////    if (!vendorExists)
        ////////        return BadRequest();


        ////////    _context.Products.Add(newProduct);
        ////////    await _context.SaveChangesAsync();

        ////////    var productWithVendor = await _context.Products
        ////////        .Include(p => p.Vendor)
        ////////        .FirstOrDefaultAsync(p => p.Id == newProduct.Id);

        ////////    return CreatedAtAction(nameof(GetProducts), new { id = newProduct.Id }, productWithVendor);
        ////////}

        [HttpPost]

        public async Task<ActionResult<Product>> AddProduct(Product newProduct)

        {

            bool vendorExists = await _context.Vendors.AnyAsync(v => v.Id == newProduct.VendorId);

            if (!vendorExists)

                return BadRequest();


            _context.Products.Add(newProduct);

            await _context.SaveChangesAsync();

            var productWithVendor = await _context.Products

                .Include(p => p.Vendor)

                .FirstOrDefaultAsync(p => p.Id == newProduct.Id);

            return CreatedAtAction(nameof(GetProducts), new { id = newProduct.Id }, productWithVendor);

        }



        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
