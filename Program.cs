using SaigonRide.Data;
using SaigonRide.Repositories;
using SaigonRide.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add services to the container.
builder.Services.AddControllersWithViews();

// QUAN TRỌNG: Đảm bảo Session được cấu hình để lưu UserId khi User đăng nhập
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session tồn tại trong 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Đăng ký các Repository
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<StationRepository>();
builder.Services.AddScoped<VehicleRepository>();

// 3. Đăng ký các Service
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<StationService>();
builder.Services.AddScoped<VehicleService>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();