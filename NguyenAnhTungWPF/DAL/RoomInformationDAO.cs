using Microsoft.EntityFrameworkCore;
using NguyenAnhTungWPF.Models;
using System.Collections.Generic;
using System.Linq;

namespace NguyenAnhTungWPF.DAL
{
    public class RoomInformationDAO
    {
        FuminiHotelManagementContext _context;
        public RoomInformationDAO(FuminiHotelManagementContext context)
        {
            _context = context;
        }

        public List<RoomInformation> GetAvailableRooms()
        {
            return _context.RoomInformations
                           .Include(r => r.RoomType) 
                           .Where(r => r.RoomStatus == "Active") 
                           .ToList();
        }

        // Luôn Include(r => r.RoomType) để binding trong DataGrid hoạt động
        public IEnumerable<RoomInformation> GetAll() => _context.RoomInformations.Include(r => r.RoomType).ToList();

        public IEnumerable<RoomInformation> GetByNumber(string roomNumber) => _context.RoomInformations.Include(r => r.RoomType).Where(r => r.RoomNumber.Contains(roomNumber)).ToList();

        public RoomInformation GetById(int id) => _context.RoomInformations.Include(r => r.RoomType).SingleOrDefault(r => r.RoomId == id);

        public void Add(RoomInformation room)
        {
            _context.RoomInformations.Add(room);
            _context.SaveChanges();
        }

        public void Update(RoomInformation room)
        {
            _context.RoomInformations.Update(room);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var room = _context.RoomInformations.Find(id);
            if (room != null)
            {
                if(_context.BookingDetails.Where(bd => bd.RoomId == id).Any())
                {
                    room.RoomStatus = "Deactive"; 
                    _context.RoomInformations.Update(room);
                }
                else
                {
                    _context.RoomInformations.Remove(room);
                }
                _context.SaveChanges();
            }
        }

        public IEnumerable<RoomType> GetAllRoomTypes() => _context.RoomTypes.ToList();


    }
}