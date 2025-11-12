using NguyenAnhTungWPF.Models;
using NguyenAnhTungWPF.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace NguyenAnhTungWPF.Services
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IServiceProvider _serviceProvider;
        public Customer? CurrentCustomer { get; private set; }

        public UserSessionService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool Login(string email, string password)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var customerRepo = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
                var customer = customerRepo.Login(email, password);

                if (customer != null)
                {
                    CurrentCustomer = customer;
                    return true;
                }
                return false;
            }
        }

        public void Logout()
        {
            CurrentCustomer = null;

        }
    }
}