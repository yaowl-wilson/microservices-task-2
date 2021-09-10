using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTicketBookingAPI
{
    public class MovieTicketBooking
    {
        public int ID { set; get; } = 0;
        public DateTime? Date { set; get; } = null;
        public String Venue { set; get; } = null;
        public int? NumberOfTickets { set; get; } = null;
        public double? Amount { set; get; } = null;
        public String Currency { set; get; } = null;
    }
}
