﻿using System;
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
        public async Task<ActionResult<IEnumerable<Request>>> GetRequest() {
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


        /*

        * `RejectionReason` must be provided when the request is rejected
        * The `UserId` is automatically set to the Id of the logged in user.
        * Neither `Status` nor `Total` may be set by the user.These are set by the application only.
        * The `Total` is auto calculated by adding up all the lines currently on the request
        * There should be a virtual `User` instance in the Request to hold the FK
        instance when reading a Request
        * There should be a virtual collection of `RequestLine` instances in the Request to
        hold the collection of lines related to this Request.

        ### Methods: (other than generated methods)

        * Review(request) - Sets the status of the request for the id provided to "REVIEW"
        * Approve(request) - Sets the status of the request for the id provided to "APPROVED"
        * Reject(request) - Sets the status of the request for the id provided to "REJECTED"
        * GetReviews(userId) - Gets requests in review status and now owned by userId
        * */
        [HttpPut("Review")]                          //Set status to REVIEW
        public async Task<ActionResult<Request>>
            SetToReview(Request request) {
            request.Status = "REVIEW";
            await _context.SaveChangesAsync();
            return request;
        }

        [HttpPut("Approved")]                          //Set status to APPROVED
        public async Task<ActionResult<Request>>
            SetToApproved(Request request) {
            request.Status = "APPROVED";
            await _context.SaveChangesAsync();
            return request;
        }

        [HttpPut("Rejected")]                          //Set status to REJECTED
        public async Task<ActionResult<Request>>
            SetToRejected(Request request) {
            request.Status = "REJECTED";
            request.RejectionReason = "Just Because";
            await _context.SaveChangesAsync();
            return request;
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
