using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketBookingAPI.Models
{
    public class MovieTicketBookingContext : DbContext
    {
        public MovieTicketBookingContext() { }
        public MovieTicketBookingContext(DbContextOptions<MovieTicketBookingContext> options)
            : base(options)
        {
        }

        public DbSet<MovieTicketBooking> MovieTicketBookingItems { get; set; }
    }
}
