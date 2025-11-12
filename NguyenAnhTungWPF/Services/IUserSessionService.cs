using NguyenAnhTungWPF.Models;

namespace NguyenAnhTungWPF.Services
{
    public interface IUserSessionService
    {
        Customer? CurrentCustomer { get; }
        bool Login(string email, string password);
        void Logout();
    }
}