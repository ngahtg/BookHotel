using Microsoft.EntityFrameworkCore;
using NguyenAnhTungWPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NguyenAnhTungWPF.DAL
{
    public class CustomerDAO
    {
        FuminiHotelManagementContext con;
        
        public CustomerDAO(FuminiHotelManagementContext con)
        {
            this.con = con;
        }

        public Customer Login(string username, string password)
        {
            var customer = con.Customers.SingleOrDefault(c => c.EmailAddress == username && c.Password == password);
            return customer;
        }

        public void CreateAdminAccount(string username, string password)
        {
            if (!con.Customers.Any(c => c.EmailAddress == username))
            {
                var admin = new Customer
                {
                    EmailAddress = username,
                    Password = password,
                    CustomerStatus = "Admin"
                };
                con.Customers.Add(admin);
                con.SaveChanges();
            }

        }
        public IEnumerable<Customer> GetAll() => con.Customers.Where(c => c.CustomerStatus != "Admin").ToList();
        public IEnumerable<Customer> GetByName(string name) => con.Customers.Where(c => c.CustomerFullName.Contains(name)).ToList();
        public Customer GetById(int id) => con.Customers.Find(id);

        public void Add(Customer customer)
        {
            con.Customers.Add(customer);
            con.SaveChanges();
        }

        public void Update(Customer customer)
        {
            con.Customers.Update(customer);
            con.SaveChanges();
        }

        public void Delete(int id)
        {
            var customer = con.Customers.Find(id);
            if (customer != null)
            {
                if(con.BookingReservations.Where(bd => bd.CustomerId == id).Any())
                {
                    customer.CustomerStatus = "Deactive";
                    con.Customers.Update(customer);
                }
                else
                {
                    con.Customers.Remove(customer);
                }
                con.SaveChanges();
            }
        }
    }
}
