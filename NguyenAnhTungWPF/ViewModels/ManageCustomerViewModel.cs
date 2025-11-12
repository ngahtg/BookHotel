using NguyenAnhTungWPF.Models;
using NguyenAnhTungWPF.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NguyenAnhTungWPF.ViewModels
{
    public class ManageCustomerViewModel : ViewModelBase
    {
        private readonly ICustomerRepository _customerRepository;
        private Customer _currentCustomer=new();
        public event Action RequestClose;

        //Bìnding
        public Customer CurrentCustomer { get => _currentCustomer;set { _currentCustomer = value;OnPropertyChanged(); }  }
        public string WindowTitle { get; set; }
        public string SaveButtonText { get; set; }
        public List<string> StatusOptions { get; } = new List<string> { "Active", "Deactive" };

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

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        

        public ManageCustomerViewModel(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
            SaveCommand=new RelayCommand(SaveChanges, CanSave);
            CancelCommand = new RelayCommand(Cancel);

        }
        internal void Initilize(int? customerId)
        {
            if (customerId == null)
            {
                WindowTitle = "Add New Customer";
                _currentCustomer = new Customer { CustomerStatus = "Active" };
            }
            else
            {
                WindowTitle = "Update Customer";
                _currentCustomer = _customerRepository.GetCustomerById(customerId.Value);
            }
        }
        private bool CanSave(object? param)
        {
            return !string.IsNullOrWhiteSpace(FullName) &&
                   !string.IsNullOrWhiteSpace(Email) &&
                   !string.IsNullOrWhiteSpace(Telephone) &&
                   Birthday != null &&
                   !string.IsNullOrWhiteSpace(Password) ;
        }
        private void SaveChanges(object? param)
        {
            try
            {
                if(WindowTitle=="Add New Customer")
                {
                    _customerRepository.AddCustomer(_currentCustomer);
                }
                else
                {
                    _customerRepository.UpdateCustomer(_currentCustomer);
                }
                RequestClose?.Invoke();
            } catch (Exception ex)
            {
                ErrorMessage = "Error" + ex.Message;
            }
        }
        private void Cancel(object? param)
        {
            RequestClose?.Invoke();
        }
    }
}
