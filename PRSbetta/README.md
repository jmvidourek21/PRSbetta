All entities DELETE	
*Need to use path variable, rather than posting instance in body, due to restriction in Angular Http				

Request POST	
*Set status to 'New' and submittedDate to current date				




////////here's the stuff AI gave you - try to grok and rewrite////////

((((((((((((For the numbering requirement))))))))))))
//// ""Request Service"" ////

using Microsoft.AspNetCore.Mvc;  <--mvc is wrong XXXXX
using PRSbetta.Models;

[ApiController]
[Route("api/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly RequestService _requestService;

    public RequestsController(RequestService requestService)
    {
        _requestService = requestService;
    }

    [HttpPost]
    public async Task<ActionResult<Request>> Create([FromQuery] int userId)
    {
        if (userId <= 0)
            return BadRequest("UserId is required.");

        var request = await _requestService.CreateNewRequestAsync(userId);
        return Ok(request);
    }
}


//// ""Request Controller"" ////
public class RequestService
{
    private readonly PrsbettaContext _context;

    public RequestService(PrsbettaContext context)
    {
        _context = context;
    }

    public async Task<Request> CreateNewRequestAsync(int userId)
    {
        var today = DateTime.UtcNow.Date;

        // Count how many requests have already been created today
        int dailyCount = await _context.Requests
            .Where(r => r.SubmittedDate.HasValue && r.SubmittedDate.Value.Date == today)
            .CountAsync();

        int sequence = dailyCount + 1;
        string sequenceStr = sequence.ToString("D4");
        string dateSegment = today.ToString("yyMMdd");
        string requestNumber = $"R{dateSegment}{sequenceStr}";

        var newRequest = new Request
        {
            UserId = userId,
            RequestNumber = requestNumber,
            SubmittedDate = DateTime.UtcNow,
            Status = "NEW", // Optional default
        };

        _context.Requests.Add(newRequest);
        await _context.SaveChangesAsync();

        return newRequest;
    }
}


(((((((((( Create Request with Limited Fields (TR3), Set Status to "New" and SubmittedDate (TR11)))))))))))
[ApiController]
[Route("api/[controller]")]
public class RequestController : ControllerBase
{
    private readonly PrsbettaContext _context;

    public RequestController(PrsbettaContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Request>> CreateRequest([FromBody] RequestDto dto)
    {
        var today = DateTime.UtcNow.Date;

        var countToday = await _context.Requests
            .Where(r => r.SubmittedDate.HasValue && r.SubmittedDate.Value.Date == today)
            .CountAsync();

        string sequence = (countToday + 1).ToString("D4");
        string dateStr = today.ToString("yyMMdd");
        string requestNumber = $"R{dateStr}{sequence}";

        var request = new Request
        {
            UserId = dto.UserId,
            RequestNumber = requestNumber,
            Description = dto.Description,
            Justification = dto.Justification,
            DateNeeded = dto.DateNeeded,
            DeliveryMode = dto.DeliveryMode,
            SubmittedDate = DateTime.UtcNow,
            Status = "NEW",
        };

        _context.Requests.Add(request);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Request>> GetById(int id)
    {
        var request = await _context.Requests.FindAsync(id);
        return request == null ? NotFound() : Ok(request);
    }

    [HttpPost("submit")]
    public async Task<ActionResult<Request>> SubmitForReview([FromBody] int requestId)
    {
        var request = await _context.Requests.FindAsync(requestId);
        if (request == null) return NotFound();

        request.Status = "REVIEW";
        await _context.SaveChangesAsync();
        return Ok(request);
    }

    [HttpGet("review/{userId}")]
    public async Task<ActionResult<IEnumerable<Request>>> GetReviewQueue(int userId)
    {
        var requests = await _context.Requests
            .Where(r => r.Status == "REVIEW" && r.UserId != userId)
            .ToListAsync();

        return Ok(requests);
    }

    [HttpPost("approve")]
    public async Task<ActionResult<Request>> ApproveRequest([FromBody] Request req)
    {
        var existing = await _context.Requests.FindAsync(req.Id);
        if (existing == null) return NotFound();

        existing.Status = "APPROVED";
        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpPost("reject")]
    public async Task<ActionResult<Request>> RejectRequest([FromBody] Request req)
    {
        var existing = await _context.Requests.FindAsync(req.Id);
        if (existing == null) return NotFound();

        existing.Status = "REJECTED";
        existing.ReasonForRejection = req.ReasonForRejection;
        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRequest(int id)
    {
        var request = await _context.Requests.FindAsync(id);
        if (request == null) return NotFound();

        _context.Requests.Remove(request);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public class RequestDto
{
    public int UserId { get; set; }
    public string? Description { get; set; }
    public string? Justification { get; set; }
    public DateOnly? DateNeeded { get; set; }
    public string? DeliveryMode { get; set; }
}




((((((((((((((Line Item Controller (TR4, TR5)))))))))))))))

[ApiController]
[Route("api/[controller]")]
public class LineItemController : ControllerBase
{
    private readonly PrsbettaContext _context;

    public LineItemController(PrsbettaContext context)
    {
        _context = context;
    }

    [HttpGet("request/{requestId}")]
    public async Task<ActionResult<IEnumerable<Lineitem>>> GetLineItems(int requestId)
    {
        var items = await _context.Lineitems
            .Where(li => li.RequestId == requestId)
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<Lineitem>> AddLineItem([FromBody] Lineitem item)
    {
        _context.Lineitems.Add(item);
        await _context.SaveChangesAsync();
        return Ok(item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLineItem(int id, [FromBody] Lineitem item)
    {
        if (id != item.Id) return BadRequest();

        _context.Entry(item).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLineItem(int id)
    {
        var item = await _context.Lineitems.FindAsync(id);
        if (item == null) return NotFound();

        _context.Lineitems.Remove(item);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}



(((((((((recalcTotal)))))))))

private async Task RecalculateRequestTotal(int requestId)
{
    var request = await _context.Requests.Include(r => r.Lineitem).FirstOrDefaultAsync(r => r.Id == requestId);
    if (request == null) return;

    request.Total = request.Lineitem?.Sum(li => li.Quantity * li.Product?.Price) ?? 0;
    await _context.SaveChangesAsync();
}
