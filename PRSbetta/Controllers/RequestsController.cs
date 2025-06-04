using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PRSbetta.Data;
using PRSbetta.DTO;
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
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {
            return Ok(await _context.Requests.ToListAsync());
        }

        // GET: api/Requests/5


        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequestById(int id)
        {
            var request = await _context.Requests.FindAsync(id);

            if (request == null)
                return NotFound();

            return request;
        }

        // POST: api/Requests

        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //generate ReqNum (provided from prs-requirement-req-num-techspecs.doc)

        private string getNextRequestNumber()
        {
            // requestNumber format: R2409230011 
            // 11 chars, 'R' + YYMMDD + 4 digit # w/ leading zeros 
            string requestNbr = "R";
            // add YYMMDD string 
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            requestNbr += today.ToString("yyMMdd");
            // get maximum request number from db 
            string maxReqNbr = _context.Requests.Max(r => r.RequestNumber);
            String reqNbr = "";
            if (maxReqNbr != null)
            {
                // get last 4 characters, convert to number 
                String tempNbr = maxReqNbr.Substring(7);
                int nbr = Int32.Parse(tempNbr);
                nbr++;
                // pad w/ leading zeros 
                reqNbr += nbr;
                reqNbr = reqNbr.PadLeft(4, '0');
            }
            else
            {
                reqNbr = "0001";
            }
            requestNbr += reqNbr;
            return requestNbr;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpPost]
        public async Task<IActionResult> CreateRequest(RequestDTO requestDTO)
        {
            // Create a new Request object from the DTO
            var user = await _context.Users.FindAsync(requestDTO.UserId);
            if (user == null)
            {
                return NotFound($"User not found for id: {requestDTO.UserId}");
            }

            var request = new Request
            {
                UserId = requestDTO.UserId,
                Description = requestDTO.Description,
                Justification = requestDTO.Justification,
                DateNeeded = requestDTO.DateNeeded,
                DeliveryMode = requestDTO.DeliveryMode,
                Status = "NEW",
                RequestNumber = getNextRequestNumber(),
                Total = 0,
                SubmittedDate = DateTime.Now
            };
            _context.Requests.Add(request);            // Save request
            await _context.SaveChangesAsync();

            return StatusCode(201, request);
        }

        //public async Task<IActionResult> PostRequest(Request newRequest)
        //{
        //    if (newRequest == null)
        //        return BadRequest();

        //    string todayPrefix = "R" + DateTime.UtcNow.ToString("yyMMdd");

        //    int countToday = await _context.Requests
        //        .CountAsync(r => r.RequestNumber.StartsWith(todayPrefix));

        //    string sequence = (countToday + 1).ToString("D4");
        //    newRequest.RequestNumber = todayPrefix + sequence;

        //    // Add and save the new request
        //    _context.Requests.Add(newRequest);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetRequestById), new { id = newRequest.Id }, newRequest);
        //    // Client gets a 201, Header is created /api/requests/123 
        //}



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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id, Request request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestExists(id))
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

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }




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

            request.Status = (request.Total > 50 ? "REVIEW" : "APPROVE");
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
        public async Task<ActionResult> SetAsRejected(int id, [FromBody] string reason)
        {
            var request = _context.Requests.Find(id);
            if (request == null)
            {
                return NotFound();
            }
            else
            {

                request.Status = "REJECTED";
                request.ReasonForRejection = reason;
                await _context.SaveChangesAsync();
                return NoContent();
            }

        }
    }
}