using NguyenAnhTungWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NguyenAnhTungWPF.Repositories
{
    public interface ICustomerRepository
    {
        public Customer Login(string username, string password);

        public void CreateAdminAccount(string username, string password);

        Customer GetById(int customerId); 
        void Update(Customer customer);
        IEnumerable<Customer> GetAllCustomers();
        IEnumerable<Customer> GetCustomersByName(string name);
        Customer GetCustomerById(int id);
        void AddCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        void DeleteCustomer(int id);
    }
}
