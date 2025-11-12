using Microsoft.Extensions.DependencyInjection; // Cần để mở cửa sổ Add/Update
using NguyenAnhTungWPF.Models;
using NguyenAnhTungWPF.Repositories;
using NguyenAnhTungWPF.Services;
using NguyenAnhTungWPF.Views; // Cần để mở cửa sổ Add/Update
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NguyenAnhTungWPF.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        private readonly IUserSessionService _sessionService;
        // Repositories
        private readonly ICustomerRepository _customerRepo;
        private readonly IRoomInformationRepository _roomRepo;
        private readonly IBookingReservationRepository _bookingRepo;
        private readonly IServiceProvider _serviceProvider;

        public event Action RequestClose;

        // Tab 1: Customer Management
        private ObservableCollection<Customer> _customers = new();
        public ObservableCollection<Customer> Customers { get => _customers; set { _customers = value; OnPropertyChanged(); } }
        private Customer? _selectedCustomer;
        public Customer? SelectedCustomer { get => _selectedCustomer; set { _selectedCustomer = value; OnPropertyChanged(); } }
        private string _customerSearchText = "";
        public string CustomerSearchText { get => _customerSearchText; set { _customerSearchText = value; OnPropertyChanged(); } }
        public ICommand SearchCustomerCommand { get; }
        public ICommand AddCustomerCommand { get; }
        public ICommand UpdateCustomerCommand { get; }
        public ICommand DeleteCustomerCommand { get; }

        // Tab 2: Room Management
        private ObservableCollection<RoomInformation> _rooms = new();
        public ObservableCollection<RoomInformation> Rooms { get => _rooms; set { _rooms = value; OnPropertyChanged(); } }
        private RoomInformation? _selectedRoom;
        public RoomInformation? SelectedRoom { get => _selectedRoom; set { _selectedRoom = value; OnPropertyChanged(); } }
        private string _roomSearchText = "";
        public string RoomSearchText { get => _roomSearchText; set { _roomSearchText = value; OnPropertyChanged(); } }
        public ICommand SearchRoomCommand { get; }
        public ICommand AddRoomCommand { get; }
        public ICommand UpdateRoomCommand { get; }
        public ICommand DeleteRoomCommand { get; }

        // Tab 3: Booking Management
        private ObservableCollection<BookingReservation> _bookings = new();
        public ObservableCollection<BookingReservation> Bookings { get => _bookings; set { _bookings = value; OnPropertyChanged(); } }
        private BookingReservation? _selectedBooking;
        public BookingReservation? SelectedBooking { get => _selectedBooking; set { _selectedBooking = value; OnPropertyChanged(); } }

        // Tab 4: Statistics & Reports
        private DateTime _reportStartDate = DateTime.Now.AddMonths(-1);
        public DateTime ReportStartDate { get => _reportStartDate; set { _reportStartDate = value; OnPropertyChanged(); } }
        private DateTime _reportEndDate = DateTime.Now;
        public DateTime ReportEndDate { get => _reportEndDate; set { _reportEndDate = value; OnPropertyChanged(); } }
        private ObservableCollection<BookingReservation> _reportResults = new();
        public ObservableCollection<BookingReservation> ReportResults { get => _reportResults; set { _reportResults = value; OnPropertyChanged(); } }
        public ICommand GenerateReportCommand { get; }
        public ICommand LogoutCommand { get; }
        public AdminViewModel(IUserSessionService sessionService, ICustomerRepository customerRepo, IRoomInformationRepository roomRepo,
                              IBookingReservationRepository bookingRepo, IServiceProvider serviceProvider)
        {
            _sessionService = sessionService;

            _customerRepo = customerRepo;
            _roomRepo = roomRepo;
            _bookingRepo = bookingRepo;
            _serviceProvider = serviceProvider;

            // Customer Commands
            SearchCustomerCommand = new RelayCommand(LoadCustomers);
            AddCustomerCommand = new RelayCommand(AddCustomer);
            UpdateCustomerCommand = new RelayCommand(UpdateCustomer, CanExecuteCustomerAction);
            DeleteCustomerCommand = new RelayCommand(DeleteCustomer, CanExecuteCustomerAction);

            // Room Commands
            SearchRoomCommand = new RelayCommand(LoadRooms);
            AddRoomCommand = new RelayCommand(AddRoom);
            UpdateRoomCommand = new RelayCommand(UpdateRoom, CanExecuteRoomAction);
            DeleteRoomCommand = new RelayCommand(DeleteRoom, CanExecuteRoomAction);

            // Report Commands
            GenerateReportCommand = new RelayCommand(GenerateReport);
            LogoutCommand = new RelayCommand(ExecuteLogout);
            // Load initial data
            LoadCustomers(null);
            LoadRooms(null);
            LoadBookings();
        }

        // --- Customer Methods ---
        private void LoadCustomers(object? param)
        {
            var customers = string.IsNullOrEmpty(CustomerSearchText)
                ? _customerRepo.GetAllCustomers()
                : _customerRepo.GetCustomersByName(CustomerSearchText);
            Customers = new ObservableCollection<Customer>(customers);
        }

        private void AddCustomer(object? param)
        {
            var manageCustomerWindow = _serviceProvider.GetRequiredService<ManageCustomerWindow>();
            var viewModel = manageCustomerWindow.DataContext as ManageCustomerViewModel;
            if (viewModel != null)
            {
                viewModel.Initilize(null);
            }
            manageCustomerWindow.ShowDialog();
            LoadCustomers(null);
        }

        private void UpdateCustomer(object? param)
        {
            if (SelectedCustomer == null) return;

            var manageCustomerWindow = _serviceProvider.GetRequiredService<ManageCustomerWindow>();

            var viewModel = manageCustomerWindow.DataContext as ManageCustomerViewModel;
            if (viewModel != null)
            {
                viewModel.Initilize(SelectedCustomer.CustomerId);
            }
            manageCustomerWindow.ShowDialog();

            LoadCustomers(null);
        }

        private void DeleteCustomer(object? param)
        {
            if (SelectedCustomer == null) return;

            // Yêu cầu: "Delete action always combines with confirmation."
            if (MessageBox.Show($"Are you sure you want to delete {SelectedCustomer.CustomerFullName}?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _customerRepo.DeleteCustomer(SelectedCustomer.CustomerId);
                    LoadCustomers(null); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not delete customer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanExecuteCustomerAction(object? param) => SelectedCustomer != null;

        // --- Room Methods ---
        private void LoadRooms(object? param)
        {
            var rooms = string.IsNullOrEmpty(RoomSearchText)
                ? _roomRepo.GetAllRooms()
                : _roomRepo.GetRoomsByNumber(RoomSearchText);
            Rooms = new ObservableCollection<RoomInformation>(rooms);
        }

        private void AddRoom(object? param)
        {
            var manageRoomWindow = _serviceProvider.GetRequiredService<ManageRoomWindow>();
            var viewModel = manageRoomWindow.DataContext as ManageRoomViewModel;
            if (viewModel != null)
            {
                viewModel.Initialize(null);
            }
            manageRoomWindow.ShowDialog();
            LoadRooms(null);
        }

        private void UpdateRoom(object? param)
        {
            if (SelectedRoom == null) return;

            var manageRoomWindow = _serviceProvider.GetRequiredService<ManageRoomWindow>();
            var viewModel = manageRoomWindow.DataContext as ManageRoomViewModel;
            if (viewModel != null)
            {
                viewModel.Initialize(SelectedRoom.RoomId);
            }
            manageRoomWindow.ShowDialog();
            LoadRooms(null);
        }

        private void DeleteRoom(object? param)
        {
            if (SelectedRoom == null) return;

            if (MessageBox.Show($"Are you sure you want to delete {SelectedRoom.RoomNumber}?",
                      "Confirm Delete",
                      MessageBoxButton.YesNo,
                      MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                     _roomRepo.DeleteRoom(SelectedRoom.RoomId);
                    LoadRooms(null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not delete room: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private bool CanExecuteRoomAction(object? param) => SelectedRoom != null;

        // --- Booking Methods ---
        private void LoadBookings()
        {
            Bookings = new ObservableCollection<BookingReservation>(_bookingRepo.GetAllBookings());
        }

        // --- Report Methods ---
        private void GenerateReport(object? param)
        {
            var results = _bookingRepo.GetBookingsByDateRange(ReportStartDate, ReportEndDate);
            ReportResults = new ObservableCollection<BookingReservation>(results);
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
    }
}