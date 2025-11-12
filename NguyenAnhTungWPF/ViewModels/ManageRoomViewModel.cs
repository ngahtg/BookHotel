using NguyenAnhTungWPF.Models;
using NguyenAnhTungWPF.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NguyenAnhTungWPF.ViewModels
{
    public class ManageRoomViewModel : ViewModelBase
    {
        private readonly IRoomInformationRepository _roomRepo;
        public event Action RequestClose;

        private RoomInformation _currentRoom = new();
        public RoomInformation CurrentRoom { get => _currentRoom; set { _currentRoom = value; OnPropertyChanged(); } }

        private ObservableCollection<RoomType> _roomTypes = new();
        public ObservableCollection<RoomType> RoomTypes { get => _roomTypes; set { _roomTypes = value; OnPropertyChanged(); } }

        private string _windowTitle = "";
        public string WindowTitle { get => _windowTitle; set { _windowTitle = value; OnPropertyChanged(); } }

        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ManageRoomViewModel(IRoomInformationRepository roomRepo)
        {
            _roomRepo = roomRepo;
            SaveCommand = new RelayCommand(SaveChanges, CanSave);
            CancelCommand = new RelayCommand(Cancel);
            LoadRoomTypes();
        }

        private void LoadRoomTypes()
        {
            RoomTypes = new ObservableCollection<RoomType>(_roomRepo.GetAllRoomTypes());
        }

        public void Initialize(int? roomId)
        {
            if (roomId == null)
            {
                // Add
                WindowTitle = "Add New Room";
                CurrentRoom = new RoomInformation { RoomStatus = "Active" }; 
            }
            else
            {
                // Update
                WindowTitle = "Update Room";
                CurrentRoom = _roomRepo.GetRoomById(roomId.Value);
            }
        }

        private void SaveChanges(object? param)
        {
            try
            {
                if (WindowTitle=="Add New Room")
                {
                    _roomRepo.AddRoom(CurrentRoom);
                }
                else
                {
                    _roomRepo.UpdateRoom(CurrentRoom);
                }
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error: " + ex.Message;
            }
        }

        private bool CanSave(object? param)
        {
            return !string.IsNullOrEmpty(CurrentRoom.RoomNumber) &&
                   CurrentRoom.RoomMaxCapacity > 0 &&
                   CurrentRoom.RoomPricePerDay > 0 &&
                   CurrentRoom.RoomTypeId > 0;
        }

        private void Cancel(object? param)
        {
            RequestClose?.Invoke();
        }

    }
}