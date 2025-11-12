using NguyenAnhTungWPF.DAL;
using NguyenAnhTungWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NguyenAnhTungWPF.Repositories
{
    public class BookingReservationRepository: IBookingReservationRepository
    {
        private readonly BookingReservationDAO _bookingDAO;
        public BookingReservationRepository(BookingReservationDAO bookingDAO)
        {
            _bookingDAO = bookingDAO;
        }

        public List<BookingReservation> GetByCustomerId(int customerId)
        {
            return _bookingDAO.GetByCustomerId(customerId);
        }

        public void Add(BookingReservation reservation)
        {
            _bookingDAO.Add(reservation);
        }

        public IEnumerable<BookingReservation> GetAllBookings() => _bookingDAO.GetAll();
        public IEnumerable<BookingReservation> GetBookingsByDateRange(DateTime start, DateTime end) => _bookingDAO.GetByDateRange(start, end);

    }
}
