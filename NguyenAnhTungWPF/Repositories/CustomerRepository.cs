using NguyenAnhTungWPF.DAL;
using NguyenAnhTungWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NguyenAnhTungWPF.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDAO _customerDAO;
        public CustomerRepository(CustomerDAO customerDAO)
        {
            _customerDAO=  customerDAO;
        }

        public void CreateAdminAccount(string username, string password) => _customerDAO.CreateAdminAccount(username, password);
        public Customer Login(string username, string password) => _customerDAO.Login(username, password);
        public IEnumerable<Customer> GetAllCustomers() => _customerDAO.GetAll();
        public IEnumerable<Customer> GetCustomersByName(string name) => _customerDAO.GetByName(name);
        public Customer GetCustomerById(int id) => _customerDAO.GetById(id);
        public void AddCustomer(Customer customer) => _customerDAO.Add(customer);
        public void UpdateCustomer(Customer customer) => _customerDAO.Update(customer);
        public void DeleteCustomer(int id) => _customerDAO.Delete(id);
        public Customer GetById(int customerId) => _customerDAO.GetById(customerId);
        public void Update(Customer customer) => _customerDAO.Update(customer);
    }
}
