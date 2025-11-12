using Microsoft.Extensions.DependencyInjection;
using NguyenAnhTungWPF.Models;
using NguyenAnhTungWPF.Repositories;
using NguyenAnhTungWPF.Services;
using NguyenAnhTungWPF.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NguyenAnhTungWPF.ViewModels
{
    public class LoginViewModel: ViewModelBase
    {
        private readonly IUserSessionService _sessionService;
        private readonly IServiceProvider _serviceProvider;
        public event Action RequestClose;

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(); // Thông báo cho UI cập nhật
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(); // Thông báo cho UI cập nhật
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(IUserSessionService sessionService, IServiceProvider serviceProvider)
        {
            _sessionService = sessionService;
            _serviceProvider = serviceProvider;
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
        }

        private void ExecuteLogin(object parameter)
        {
            // Lấy mật khẩu từ PasswordBox được truyền qua CommandParameter
            string password = (parameter as PasswordBox)?.Password;

            if (string.IsNullOrEmpty(password))
            {
                ErrorMessage = "Vui lòng nhập mật khẩu.";
                return;
            }

            // ViewModel gọi Repository (Lớp BLL)
            bool loginSuccess = _sessionService.Login(Email, password);

            if (loginSuccess)
            {
                // Đăng nhập thành công
                Customer customer = _sessionService.CurrentCustomer;

                ErrorMessage = "Đăng nhập thành công!";
                if(customer.CustomerStatus=="Admin")
                {
                    // Mở cửa sổ Admin
                    var adminWindow = _serviceProvider.GetRequiredService<AdminWindow>();
                    adminWindow.Show();
                    RequestClose?.Invoke();
                }
                else if(customer.CustomerStatus=="Active")
                {
                    // Mở cửa sổ CustomerProfile
                    var customerProfileWindow = _serviceProvider.GetRequiredService<CustomerProfileWindow>();
                    customerProfileWindow.Show();
                    RequestClose?.Invoke();
                }
                else
                {
                    ErrorMessage = "Tài khoản của bạn không có quyền truy cập.";
                }
            }
            else
            {
                // Đăng nhập thất bại
                ErrorMessage = "Email hoặc mật khẩu không chính xác.";
            }
        }

        private bool CanExecuteLogin(object parameter)
        {
            // Chỉ cho phép nhấn nút Login khi Email không rỗng
            return !string.IsNullOrEmpty(Email);
        }
    }
}

    

