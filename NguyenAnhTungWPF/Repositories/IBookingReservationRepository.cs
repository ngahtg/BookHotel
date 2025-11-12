using NguyenAnhTungWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NguyenAnhTungWPF.Repositories
{
    public interface IBookingReservationRepository
    {
        List<BookingReservation> GetByCustomerId(int customerId);
        void Add(BookingReservation reservation);
        IEnumerable<BookingReservation> GetAllBookings();
        IEnumerable<BookingReservation> GetBookingsByDateRange(DateTime start, DateTime end);
    }
}
