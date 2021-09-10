using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieTicketBookingAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Json;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MovieTicketBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieTicketBookingController : ControllerBase
    {
        public MovieTicketBookingContext _context;

        public MovieTicketBookingController(MovieTicketBookingContext context) 
        {
            _context = context;

            if (_context.MovieTicketBookingItems.Count() == 0)
            {
                var MovieTicketBookingList = new List<MovieTicketBooking>
                {
                    new MovieTicketBooking {
                        ID = 1,
                        Date = DateTime.Now,
                        Venue = "Plaza Singapura GV",
                        NumberOfTickets = 2,
                        Amount = 50.0,
                        Currency = "sgd"
                    },
                    new MovieTicketBooking {
                        ID = 2,
                        Date = DateTime.Now,
                        Venue = "Yishun GV",
                        NumberOfTickets = 2,
                        Amount = 50.0,
                        Currency = "usd"
                    },
                    new MovieTicketBooking {
                        ID = 3,
                        Date = DateTime.Now,
                        Venue = "Jurong Point GV",
                        NumberOfTickets = 2,
                        Amount = 50.0,
                        Currency = "aud"
                    }
                };

                _context.AddRange(MovieTicketBookingList);
                _context.SaveChanges();
            }
        }
        private async Task<double> CurrencyConverterBasedOnSGD(String currencyType)
        {
            double results = -1L;
            HttpClient client = null;
            HttpResponseMessage response = null;

            try
            {
                string apiCallURL = 
                    String.Format(@"https://cdn.jsdelivr.net/gh/fawazahmed0/currency-api@1/latest/currencies/{0}/sgd.json", currencyType);

                client = new HttpClient();
                response = (HttpResponseMessage)await client.GetAsync(apiCallURL);

                var json_results = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(results);

                dynamic json = JObject.Parse(json_results);
                Debug.WriteLine((String)json.sgd);

                results = Convert.ToDouble((String)json.sgd);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return results;
        }


        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieTicketBooking>>> GetMovieTicketBookingItems()
        {
            var movieTicketBooking = await _context.MovieTicketBookingItems.ToListAsync();

            foreach (var item in movieTicketBooking.Where(w => w.Currency != "sgd"))
            {
                var value = await CurrencyConverterBasedOnSGD(item.Currency);
                item.Amount = item.Amount * value;
            }

            return movieTicketBooking;

        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieTicketBooking>> GetMovieTicketBooking(int id)
        {
            var movieTicketBooking = await _context.MovieTicketBookingItems.FindAsync(id);

            if (movieTicketBooking == null)
            {
                return NotFound();
            }
            else
            {
                var value = await CurrencyConverterBasedOnSGD(movieTicketBooking.Currency);
                movieTicketBooking.Amount = movieTicketBooking.Amount / value;
            }

            return movieTicketBooking;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovieTicketBooking(int id, MovieTicketBooking movieTicketBooking)
        {
            if (id != movieTicketBooking.ID)
            {
                return BadRequest();
            }

            try
            {
                var local =
                    _context.Set<MovieTicketBooking>()
                    .Local
                    .FirstOrDefault(entry =>
                        entry.ID.Equals(id));

                if (local != null)
                {
                    var value = await CurrencyConverterBasedOnSGD(movieTicketBooking.Currency);
                    local.Amount = local.Amount * value;

                    // detach
                    _context.Entry(local).State = EntityState.Detached;
                }
                _context.Entry(movieTicketBooking).State = EntityState.Modified;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieTicketBookingExists(id))
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

        // POST: api/Employees
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<MovieTicketBooking>> PostMovieTicketBooking(MovieTicketBooking movieTicketBooking)
        {
            var value = await CurrencyConverterBasedOnSGD(movieTicketBooking.Currency);
            movieTicketBooking.Amount = movieTicketBooking.Amount * value;

            _context.MovieTicketBookingItems.Add(movieTicketBooking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovieTicketBooking", new { id = movieTicketBooking.ID }, movieTicketBooking);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MovieTicketBooking>> DeleteMovieTicketBooking(int id)
        {
            var employee = await _context.MovieTicketBookingItems.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.MovieTicketBookingItems.Remove(employee);
            await _context.SaveChangesAsync();

            return employee;
        }

        private bool MovieTicketBookingExists(int id)
        {
            return _context.MovieTicketBookingItems.Any(e => e.ID == id);
        }
    }
}
