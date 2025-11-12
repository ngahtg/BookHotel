using NguyenAnhTungWPF.Models;
using System.Collections.Generic;

namespace NguyenAnhTungWPF.Repositories
{
    public interface IRoomInformationRepository
    {
        List<RoomInformation> GetAvailableRooms();
        IEnumerable<RoomInformation> GetAllRooms();
        IEnumerable<RoomInformation> GetRoomsByNumber(string roomNumber);
        RoomInformation GetRoomById(int id);
        void AddRoom(RoomInformation room);
        void UpdateRoom(RoomInformation room);
        void DeleteRoom(int id);
        IEnumerable<RoomType> GetAllRoomTypes(); 
    }
}