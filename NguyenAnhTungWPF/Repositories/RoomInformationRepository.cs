using NguyenAnhTungWPF.DAL;
using NguyenAnhTungWPF.Models;
using System.Collections.Generic;

namespace NguyenAnhTungWPF.Repositories
{
    public class RoomInformationRepository : IRoomInformationRepository
    {
        private readonly RoomInformationDAO _roomDAO;
        public RoomInformationRepository(RoomInformationDAO roomDAO)
        {
            _roomDAO = roomDAO;
        }

        public List<RoomInformation> GetAvailableRooms()
        {
            return _roomDAO.GetAvailableRooms();
        }
        public IEnumerable<RoomInformation> GetAllRooms() => _roomDAO.GetAll();
        public IEnumerable<RoomInformation> GetRoomsByNumber(string roomNumber) => _roomDAO.GetByNumber(roomNumber);
        public RoomInformation GetRoomById(int id) => _roomDAO.GetById(id);
        public void AddRoom(RoomInformation room) => _roomDAO.Add(room);
        public void UpdateRoom(RoomInformation room) => _roomDAO.Update(room);
        public void DeleteRoom(int id) => _roomDAO.Delete(id);
        public IEnumerable<RoomType> GetAllRoomTypes() => _roomDAO.GetAllRoomTypes();
    }
}