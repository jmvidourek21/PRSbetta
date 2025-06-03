using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRSbetta.Data;
using PRSbetta.Models;
using PRSbetta.DTO;

namespace PRSbetta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly PrsbettaContext _context;

        public UsersController(PrsbettaContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }
      
        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754


        [HttpPut("{id}")]           ////correction from mike 5/19 pm
        public async Task<IActionResult> PutUser(int id, User updatesUser)
        {
            if (id != updatesUser.Id)
            {
                return BadRequest();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            user.Password = updatesUser.Password;
            user.FirstName = updatesUser.FirstName;
            user.LastName = updatesUser.LastName;
            user.PhoneNumber = updatesUser.PhoneNumber;
            user.Email = updatesUser.Email;
            user.Admin = updatesUser.Admin;
            user.Reviewer = updatesUser.Reviewer;

            await _context.SaveChangesAsync();

            return Ok(user);
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754

        [HttpPost]
        public async Task<ActionResult<User>> AddUser(User newUser)
        {
            if (newUser == null)
                return BadRequest();

                 _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();
            //////try
            //////{
            //////    await _context.SaveChangesAsync();
            //////}
            //////catch (DbUpdateException)
            //////{
            //////    if (UserExists(User.Id))
            //////    {
            //////        return Conflict();
            //////    }
            //////    else
            //////    {
            //////        throw;
            //////    }
            //////}

            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
        }



        // DELETE: api/Users/5
         [HttpDelete("{id}")]
 
  public async Task<IActionResult> DeleteUser(int id)
  {
      var user = await _context.Users.FindAsync(id);
      if (user == null)
 
          return NotFound();
 
 
      _context.Users.Remove(user);
      await _context.SaveChangesAsync();
 
      return NoContent();
  }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }








        //AUTHENTICATE USER BY USERNAME AND PASSWORD

        // User Login
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == loginDto.Username && u.Password == loginDto.Password);
            if (user != null)
                return Ok(user);
            return NotFound();
        }
    }
}
