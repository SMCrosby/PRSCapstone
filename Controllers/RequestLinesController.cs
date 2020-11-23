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
    public class RequestLinesController : ControllerBase {
        private readonly PRSCapstoneContext _context;




        public RequestLinesController(PRSCapstoneContext context) {
            _context = context;
        }

        // GET: api/RequestLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequestLine>>> GetRequestLines() {
            return await _context.RequestLines.ToListAsync();
        }

        // GET: api/RequestLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RequestLine>> GetRequestLine(int id) {
            var requestLine = await _context.RequestLines.FindAsync(id);

            if (requestLine == null)
            {
                return NotFound();
            }

            return requestLine;
        }


        [HttpGet("Request/{requestId}")]                       //Pulls up RequestsLines with RequestId
        public async Task<ActionResult<IEnumerable<RequestLine>>>
            GetRequestsLines(int requestId) {
            return await _context.RequestLines.Where(rl => rl.RequestId == requestId)
                                            .ToListAsync();

        }



        //[HttpPut("RequestTotal")]                      //Updating The Total Price
        private async Task<IActionResult>
          RecalculateRequestTotal(int id, Request request) {
            var reqTotal = (from rl in await _context.RequestLines.ToListAsync()
                            join pr in await _context.Products.ToListAsync()
                            on rl.ProductId equals pr.Id

                            join req in await _context.Requests.ToListAsync()
                            on rl.RequestId equals req.Id
                            where rl.RequestId == id                //Only RequestLines who's foreign key(RequestId) matches our Id
                            select new {
                                RequestTotal = rl.Quantity * pr.Price       //Creating new Column called RequestTotal
                            }).Sum(t => t.RequestTotal);
            request.Total = reqTotal;
            await _context.SaveChangesAsync();
            return (IActionResult)request;
            //return await PutRequest(id, request);
        }

   
        // PUT: api/RequestLines/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequestLine(int id, RequestLine requestLine)
        {
            if (id != requestLine.Id){
                return BadRequest();
            }

            _context.Entry(requestLine).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!RequestLineExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RequestLines
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<RequestLine>> PostRequestLine(RequestLine requestLine)
        {
            _context.RequestLines.Add(requestLine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequestLine", new { id = requestLine.Id }, requestLine);
        }

        // DELETE: api/RequestLines/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RequestLine>> DeleteRequestLine(int id)
        {
            var requestLine = await _context.RequestLines.FindAsync(id);
            if (requestLine == null)
            {
                return NotFound();
            }

            _context.RequestLines.Remove(requestLine);
            await _context.SaveChangesAsync();

            return requestLine;
        }

        private bool RequestLineExists(int id)
        {
            return _context.RequestLines.Any(e => e.Id == id);
        }
    }
}
