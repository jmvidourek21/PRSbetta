using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRSbetta.Data;
using PRSbetta.Models;
using Request = PRSbetta.Models.Request;


namespace PRSbetta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly PrsbettaContext _context;

        public RequestsController(PrsbettaContext context)
        {
            _context = context;
        }  

        // GET: api/Requests
        [HttpGet("Requests")]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {
            return Ok(await _context.Requests.ToListAsync());
        }

        // GET: api/Requests/5
        [HttpGet("Users/{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);

            if (request == null)
            {
                return NotFound();
            }

            return Ok(request);
        }

        // GET: api/Requests/5
        ////[HttpGet("Requests/{id}/total")]
        ////public async Task<ActionResult<decimal>> GetTotalForRequest(int requestID)
        ////{
        ////    var lineItems = await _context.Lineitems
        ////        .Include(li => li.Product)
        ////        .Where(li => li.RequestId == requestID &&
        ////                       li.Request.Status != "REJECTED" ||
        ////                       li.Request.Status != "APPROVED")
        ////        .ToListAsync();
            
        ////    if (!lineItems.Any())
        ////    {
        ////        return NotFound();
        ////    }

        ////    var total = lineItems.Sum(li => li.Quantity * li.Product.Price);

        ////    return Ok(total);
        ////}


        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754


        //REQUEST NUMBER Formatting:
        // =RMMDDYY####
        // R is constant, so ....
        // string = " R+(today "MMDDYY" from DateTime) +  0000 (need ++ from last req#)"
        // " R+((DateTime.Now)->string)+ (count req# +1)
        [HttpPost] //api/Requests/<newRequestId> 
        public async Task<IActionResult> CreateRequest(Request newRequest)
        {
            if (newRequest == null)
                return BadRequest();

            string todayPrefix = "R" + (DateOnly.FromDateTime(DateTime.Now).ToString("yyMMdd"));

            int newReqNum = await _context.Requests
                .CountAsync(r => r.RequestNumber.StartsWith(todayPrefix));

            string sequence = (newReqNum + 1).ToString("D4");
            newRequest.RequestNumber = todayPrefix + sequence;

            // Add and save the new request
            _context.Requests.Add(newRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRequest), new { id = newRequest.Id }, newRequest);
            // Client gets a 201, Header is created /api/requests/123 
        }


        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            if (request.Status == "APPROVED")
            {
                return BadRequest();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return Ok();
        }
        // PUT: api/Requests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("approve/{id}")]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null) 
                return NotFound();

            request.Status = "APPROVED";
            await _context.SaveChangesAsync();

            return Ok(request);
        }

        [HttpPut("submit-review/{id}")]
        public async Task<IActionResult> SubmitForReview(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null) 
                return NotFound();

            request.Status = (request.Total >= 50 ? "REVIEW" : "APPROVE");
            request.SubmittedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(request);
        }

        [HttpGet("list-review/{userId}")]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequestsForReview(int userId)
        {
            return await _context.Requests
                .Where(r => r.Status == "REVIEW" && r.UserId != userId)
                .ToListAsync();
        }


        [HttpPut("reject/{id}")]
        public async Task<IActionResult> RejectRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null) return NotFound();

            // Read body manually
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            // Parse JSON to get the "reason" field
            var json = JsonDocument.Parse(body);
            if (!json.RootElement.TryGetProperty("reason", out var reasonElement))
            {
                return BadRequest(new { error = "Missing 'reason' in body" });
            }

            var reason = reasonElement.GetString();

            // Apply changes
            request.Status = "REJECTED";
            request.ReasonForRejection = reason;

            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}


//REQUEST NUMBER Formatting:
// =RMMDDYY####
// R is constant, so ....
// string = " R+(today MMDDYY from DateTime) + 0000 (need ++ from last req#)"
// " R+(DateOnly.FromDateTime(DateTime.Now))+ 










//LIMIT REQUEST FIELDS ENTERED BY USER (REQ. CREATE/POST)

//SUBMIT FOR REVIEW (PURCH. REQ.)

//REQUEST REVIEW (PURCH. REQ.)
////SHOW REQUESTS IN REVIEW STATUS AND NOT ASSIGNED TO LOGGED IN USER


//REQUEST APPROVE (PURCH. REQ.)
////REQUEST PASSED IN BODY---SET STATUS TO "APPROVED"---RETURN REQUEST

//REQUEST REJECT (PURCH. REQ.)
////REQUEST PASSED IN BODY---SET STATUS TO "REJECTED"---REASONFORREJECTION WILL COME FROM FRONT END RETURN REQUEST
///


////////guidance from Kory////////
////LIMIT REQUEST FIELDS ENTERED BY USER (REQ. CREATE/POST)
//this is where you would use a DTO to choose the fields you would like to pass rather than all of them . 

////SUBMIT FOR REVIEW (PURCH. REQ.)
//you are going to create a variable. Then use a ternary operator to set the status based on if the total is over or under 50.



////REQUEST REVIEW (PURCH. REQ.)

//////SHOW REQUESTS IN REVIEW STATUS AND NOT ASSIGNED TO LOGGED IN USER
//create a variable that finds the userId in the user table.
//then check for null and if so, return NotFound().
//then create another variable that checks Request with a where statement to match the status field for REVIEW and the userId being equal and return to a list. 

////REQUEST APPROVE (PURCH. REQ.)

//////REQUEST PASSED IN BODY---SET STATUS TO "APPROVED"---RETURN REQUEST
//create a variable that find the requestid
//if null, notfound
//updates status to APPROVED
//and saves changes. (await _context.SaveChangesAsync().

////REQUEST REJECT (PURCH. REQ.)

//////REQUEST PASSED IN BODY---SET STATUS TO "REJECTED"---REASONFORREJECTION WILL COME FROM FRONT END RETURN REQUEST
//reject is much the same as approve with the exception of changing the status to REJECTED and saving the reason. 
