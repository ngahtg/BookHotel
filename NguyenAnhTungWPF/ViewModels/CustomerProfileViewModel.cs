using Microsoft.Extensions.DependencyInjection;
using NguyenAnhTungWPF.Models;
using NguyenAnhTungWPF.Repositories;
using NguyenAnhTungWPF.Services;
using NguyenAnhTungWPF.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NguyenAnhTungWPF.ViewModels
{

    public class CustomerProfileViewModel : ViewModelBase
    {
        // Services và Repositories
        private readonly IUserSessionService _sessionService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingReservationRepository _bookingRepository;
        private readonly IRoomInformationRepository _roomRepository;
        private readonly IServiceProvider _serviceProvider; // Để mở cửa sổ Login khi Logout

        public event Action RequestClose; // Để đóng cửa sổ này

        private Customer _currentCustomer;

        // --- Thuộc tính cho Tab "My Profile" ---
        private string _fullName;
        public string FullName { get => _fullName; set { _fullName = value; OnPropertyChanged(); } }

        private string _email;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }

        private string _telephone;
        public string Telephone { get => _telephone; set { _telephone = value; OnPropertyChanged(); } }

        private DateTime? _birthday;
        public DateTime? Birthday { get => _birthday; set { _birthday = value; OnPropertyChanged(); } }

        private string _password;
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }

        private string _errorMessage;
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged(); } }

        // --- Thuộc tính cho Tab "Booking History" ---
        public ObservableCollection<BookingReservation> BookingHistory { get; set; }

        // --- Thuộc tính cho Tab "New Booking" ---
        public ObservableCollection<RoomInformation> AvailableRooms { get; set; }

        private DateTime? _newBookingStartDate = DateTime.Now;
        public DateTime? NewBookingStartDate { get => _newBookingStartDate; set { _newBookingStartDate = value; OnPropertyChanged(); } }

        private DateTime? _newBookingEndDate = DateTime.Now.AddDays(1);
        public DateTime? NewBookingEndDate { get => _newBookingEndDate; set { _newBookingEndDate = value; OnPropertyChanged(); } }

        private RoomInformation _selectedAvailableRoom;
        public RoomInformation SelectedAvailableRoom { get => _selectedAvailableRoom; set { _selectedAvailableRoom = value; OnPropertyChanged(); } }

        private string _bookingErrorMessage;
        public string BookingErrorMessage { get => _bookingErrorMessage; set { _bookingErrorMessage = value; OnPropertyChanged(); } }

        // --- Commands ---
        public ICommand UpdateProfileCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand BookRoomCommand { get; }

        public CustomerProfileViewModel(
            IUserSessionService sessionService,
            ICustomerRepository customerRepository,
            IBookingReservationRepository bookingRepository,
            IRoomInformationRepository roomRepository,
            IServiceProvider serviceProvider)
        {
            _sessionService = sessionService;
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _serviceProvider = serviceProvider;

            BookingHistory = new ObservableCollection<BookingReservation>();
            AvailableRooms = new ObservableCollection<RoomInformation>();

            UpdateProfileCommand = new RelayCommand(ExecuteUpdateProfile, CanExecute);
            LogoutCommand = new RelayCommand(ExecuteLogout, CanExecute);
            BookRoomCommand = new RelayCommand(ExecuteBookRoom, CanExecuteBookRoom);

            LoadCustomerData();
            LoadBookingHistory();
            LoadAvailableRooms();
        }

        private void LoadCustomerData()
        {
            _currentCustomer = _sessionService.CurrentCustomer;
            if (_currentCustomer != null)
            {
                // Tải lại thông tin mới nhất từ DB
                _currentCustomer = _customerRepository.GetById(_currentCustomer.CustomerId);

                FullName = _currentCustomer.CustomerFullName;
                Email = _currentCustomer.EmailAddress;
                Telephone = _currentCustomer.Telephone;
                Birthday = _currentCustomer.CustomerBirthday.HasValue
                    ? _currentCustomer.CustomerBirthday.Value.ToDateTime(TimeOnly.MinValue)
                    : (DateTime?)null;
                Password = _currentCustomer.Password;
            }
        }

        private void LoadBookingHistory()
        {
            if (_currentCustomer == null) return;
            var history = _bookingRepository.GetByCustomerId(_currentCustomer.CustomerId);
            BookingHistory.Clear();
            foreach (var item in history)
            {
                BookingHistory.Add(item);
            }
        }

        private void LoadAvailableRooms()
        {
            var rooms = _roomRepository.GetAvailableRooms();
            AvailableRooms.Clear();
            foreach (var room in rooms)
            {
                AvailableRooms.Add(room);
            }
        }

        private bool CanExecute(object obj) => true;

        private void ExecuteUpdateProfile(object obj)
        {
            try
            {
                _currentCustomer.CustomerFullName = FullName;
                _currentCustomer.Telephone = Telephone;
                _currentCustomer.CustomerBirthday = Birthday.HasValue
                    ? DateOnly.FromDateTime(Birthday.Value)
                    : (DateOnly?)null;
                _currentCustomer.Password = Password;

                _customerRepository.Update(_currentCustomer);
                ErrorMessage = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error updating profile: {ex.Message}";
            }
        }

        private void ExecuteLogout(object obj)
        {
            _sessionService.Logout();

            // Mở lại cửa sổ Login
            var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
            loginWindow.Show();

            // Đóng cửa sổ này
            RequestClose?.Invoke();
        }

        private bool CanExecuteBookRoom(object obj)
        {
            return SelectedAvailableRoom != null && NewBookingStartDate.HasValue && NewBookingEndDate.HasValue;
        }

        private void ExecuteBookRoom(object obj)
        {
            try
            {
                if (NewBookingEndDate <= NewBookingStartDate)
                {
                    BookingErrorMessage = "End date must be after start date.";
                    return;
                }

                DateOnly startDate = DateOnly.FromDateTime(NewBookingStartDate.Value);
                DateOnly endDate = DateOnly.FromDateTime(NewBookingEndDate.Value);

                int totalDays = endDate.DayNumber - startDate.DayNumber;
                if (totalDays <= 0)
                {
                    BookingErrorMessage = "End date must be after start date.";
                    return;
                }

                // (Đây là logic đơn giản, bạn cần làm kỹ hơn)
                // 1. Tạo BookingReservation
                var reservation = new BookingReservation
                {
                    BookingDate = DateTime.Now,
                    TotalPrice = SelectedAvailableRoom.RoomPricePerDay * (decimal)(NewBookingEndDate.Value - NewBookingStartDate.Value).TotalDays,
                    CustomerId = _currentCustomer.CustomerId,
                    BookingStatus = "Pending",
                    BookingDetails = new List<BookingDetail>() // Khởi tạo List
                };

                // 2. Tạo BookingDetail
                var detail = new BookingDetail
                {
                    RoomId = SelectedAvailableRoom.RoomId,
                    StartDate = startDate,
                    EndDate = endDate,
                    ActualPrice = reservation.TotalPrice
                };

                reservation.BookingDetails.Add(detail);

                // 3. Lưu vào DB
                _bookingRepository.Add(reservation);

                BookingErrorMessage = "Booking successful!";
                LoadBookingHistory(); // Refresh lịch sử
            }
            catch (Exception ex)
            {
                // Giúp hiển thị lỗi chi tiết hơn (nếu có lỗi bên trong)
                string innerEx = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                BookingErrorMessage = $"Error booking room: {innerEx}";
            }
        }
    }
}