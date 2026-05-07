using SaigonRide.Data;
using SaigonRide.Repositories;
using SaigonRide.Services;
using SaigonRide.Services.Payments; // Thêm namespace chứa các file API thanh toán
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.
builder.Services.AddControllersWithViews();

// Cấu hình Session để lưu trữ trạng thái đăng nhập của sinh viên
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cấu hình kết nối Database SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Đăng ký các Repository (Lớp tương tác dữ liệu)
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<StationRepository>();
builder.Services.AddScoped<VehicleRepository>();

// 3. Đăng ký các Service (Lớp xử lý nghiệp vụ)
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<StationService>();
builder.Services.AddScoped<VehicleService>();

// --- ĐĂNG KÝ CÁC DỊCH VỤ THANH TOÁN QR ---
builder.Services.AddScoped<MomoService>();
builder.Services.AddScoped<PaypalService>();

var app = builder.Build();

// 4. Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// QUAN TRỌNG: Thứ tự Session phải TRƯỚC Authorization
app.UseSession();
app.UseAuthorization();

// Thiết lập trang đăng nhập là trang mặc định khi khởi chạy ứng dụng
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();