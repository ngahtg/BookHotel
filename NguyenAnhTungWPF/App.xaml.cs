using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NguyenAnhTungWPF.DAL;
using NguyenAnhTungWPF.Repositories;
using NguyenAnhTungWPF.Services;
using NguyenAnhTungWPF.ViewModels;
using NguyenAnhTungWPF.Views;
using System.IO;
using System.Windows;

namespace NguyenAnhTungWPF
{
    public partial class App : Application
    {
        public static IHost AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    // Cấu hình để đọc appsettings.json
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Lấy chuỗi kết nối từ appsettings.json
                    string connectionString = hostContext.Configuration.GetConnectionString("DBContext");

                    // 1. Đăng ký DbContext (ĐÂY LÀ PHẦN SỬA QUAN TRỌNG)
                    // Dạy cho DI cách tạo DbContext, thay vì dùng OnConfiguring
                    services.AddDbContext<FuminiHotelManagementContext>(options =>
                    {
                        options.UseSqlServer(connectionString);
                    }, ServiceLifetime.Scoped); // Scoped là tốt nhất cho DbContext

                    // 2. Đăng ký các lớp DAO (Data Access)
                    // Chúng sẽ tự động nhận 'FuminiHotelManagementContext'
                    services.AddScoped<CustomerDAO>();
                    services.AddScoped<RoomTypeDAO>();
                    services.AddScoped<BookingReservationDAO>();
                    services.AddScoped<RoomInformationDAO>();
                    services.AddScoped<BookingDetailDAO>();
                    // (Thêm các DAO khác của bạn ở đây...)

                    // 3. Đăng ký Repositories ()
                    // Đây là lớp BLL (Business Logic Layer)
                    services.AddScoped<ICustomerRepository, CustomerRepository>();
                    services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
                    services.AddScoped<IBookingReservationRepository, BookingReservationRepository>();
                    services.AddScoped<IRoomInformationRepository, RoomInformationRepository>();
                    services.AddScoped<IBookingDetailRepository, BookingDetailRepository>();

                    // (Thêm các Repository khác của bạn ở đây...)

                    //Singleton cho UserSessionService vì nó lưu trạng thái đăng nhập
                    services.AddSingleton<IUserSessionService, UserSessionService>();

                    // 4. Đăng ký ViewModels và Views (MVVM)
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<AdminViewModel>();
                    services.AddTransient<CustomerProfileViewModel>();
                    services.AddTransient<ManageBookingViewModel>();
                    services.AddTransient<ManageCustomerViewModel>();
                    services.AddTransient<ManageRoomViewModel>();
                    services.AddTransient<ViewHistoryViewModel>();


                    services.AddTransient<LoginWindow>(provider =>
                        new LoginWindow(provider.GetRequiredService<LoginViewModel>())
                    );
                    services.AddTransient<AdminWindow>(provider =>
                        new AdminWindow(provider.GetRequiredService<AdminViewModel>())
                    );
                    services.AddTransient<CustomerProfileWindow>(provider =>
                        new CustomerProfileWindow(provider.GetRequiredService<CustomerProfileViewModel>())
                    );
                    services.AddTransient<ManageBookingWindow>(provider =>
                        new ManageBookingWindow(provider.GetRequiredService<ManageBookingViewModel>())
                    );
                    services.AddTransient<ManageCustomerWindow>(provider =>
                        new ManageCustomerWindow(provider.GetRequiredService<ManageCustomerViewModel>())
                    );
                    services.AddTransient<ManageRoomWindow>(provider =>
                        new ManageRoomWindow(provider.GetRequiredService<ManageRoomViewModel>())
                    );
                    services.AddTransient<ViewHistoryWindow>(provider =>
                        new ViewHistoryWindow(provider.GetRequiredService<ViewHistoryViewModel>())
                    );


                    // (Thêm các View/ViewModel khác...)

                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost.StartAsync();

            using (var scope = AppHost.Services.CreateScope())
            {
                var provider = scope.ServiceProvider;

                var config = provider.GetRequiredService<IConfiguration>();
                string adminEmail = config["AdminAccount:Email"];
                string adminPass = config["AdminAccount:Password"];

                var customerRepo = provider.GetRequiredService<ICustomerRepository>();
                customerRepo.CreateAdminAccount(adminEmail, adminPass);
            }
            // Khởi động cửa sổ chính (hoặc cửa sổ Login)
            var startupForm = AppHost.Services.GetRequiredService<LoginWindow>();
            startupForm.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost.StopAsync();
            base.OnExit(e);
        }
    }
}