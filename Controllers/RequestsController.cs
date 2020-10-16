using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRSCapstone.Data;
using PRSCapstone.Models;

namespace PRSCapstone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase {
        private readonly PRSCapstoneContext _context;

        public RequestsController(PRSCapstoneContext context) {
            _context = context;
        }

        // GET: api/Requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequest() {        //can use .Include for Users/ etc?
            return await _context.Requests.ToListAsync();
        }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id) {
            var request = await _context.Requests.FindAsync(id);

            if (request == null) {
                return NotFound();
            }

            return request;
        }


        [HttpGet("RequestsInReview")]                       //Pulls up Requests that have status set to review
        public async Task<ActionResult<IEnumerable<Request>>> 
            GetRequestsInReview() {
            return await _context.Requests.Where(r => r.Status == "REVIEW").ToListAsync();

        }

        


        [HttpPut("RequestTotal")]                      //Updating The Total Price
        public async Task<IActionResult>
          RecalculateRequestTotal(int id, Request request) {
            var reqTotal = (from rl in await _context.RequestLines.ToListAsync()
                            join pr in await _context.Products.ToListAsync()
                            on rl.ProductId equals pr.Id
                            where rl.RequestId == id                //Only RequestLines who's foreign key(RequestId) matches our Id
                            select new {
                                RequestTotal = rl.Quantity * pr.Price       //Creating new Column called RequestTotal
                            }).Sum(t => t.RequestTotal);
            request.Total = reqTotal;
            return await PutRequest(id, request);
        }


        [HttpPut("Review/{id}")]                     //Set status to APPROVED if <=50; Otherwise sets to REVIEW
        public async Task<IActionResult>
           ReviewRequest(int id, Request request) {
            request.Status = request.Total <= 50 ? "APPROVED" : "REVIEW";
            return await PutRequest(id, request);
        }


        [HttpPut("Approve/{id}")]                          //Set status to APPROVED
        public async Task<IActionResult>
            SetToApproved(int id, Request request) {
            request.Status = "APPROVED";
            return await PutRequest(id, request);
        }

        [HttpPut("Reject/{id}")]                          //Set status to REJECTED
        public async Task<IActionResult>
            SetToRejected(int id, Request request,string rejectionReason) {
            request.Status = "REJECTED";
            request.RejectionReason = rejectionReason;
            //await _context.SaveChangesAsync();
            return await PutRequest(id, request);
        }




        // PUT: api/Requests/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id, Request request) {
            if (id != request.Id) {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!RequestExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Requests
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Request>> PostRequest(Request request) {
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequest", new { id = request.Id }, request);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Request>> DeleteRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null) {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return request;
        }

        private bool RequestExists(int id) {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}
