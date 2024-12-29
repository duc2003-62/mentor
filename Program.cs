using mentor.Models;
using mentor.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Session;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(connection));
builder.Services.AddAuthorization();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn phiên làm việc
    options.Cookie.HttpOnly = true; // Chỉ cho phép truy cập cookie thông qua HTTP (không qua JavaScript)
    options.Cookie.IsEssential = true; // Đảm bảo cookie sẽ luôn được gửi trong phiên
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.InitializeAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Đã xảy ra lỗi khi khởi tạo dữ liệu.");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
