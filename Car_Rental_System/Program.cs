using Car_Rental_System.Data;
using Car_Rental_System.Models;
using Car_Rental_System.Repositories;
using Car_Rental_System.Services;
using Car_Rental_System.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DinkToPdf;
using DinkToPdf.Contracts;
using Car_Rental_System.Helpers;


var wkHtmlToPdfPath = Path.Combine(Directory.GetCurrentDirectory(), "lib", "libwkhtmltox.dll");
new CustomAssemblyLoadContext().LoadUnmanagedLibrary(wkHtmlToPdfPath);


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

var configuration = builder.Configuration; 

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CarRentalConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => false;
    options.MinimumSameSitePolicy = SameSiteMode.None; 
});


builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None; 
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



// 🔹 Cấu hình Google & Facebook Authentication
builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        googleOptions.CallbackPath = "/signin-google"; // 🔹 Đảm bảo Redirect URI chính xác
    })
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        facebookOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
        facebookOptions.CallbackPath = "/signin-facebook";
    });

// 🔹 Đăng ký Repository
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<ICarRentalRepository, CarRentalRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
// 🔹 Đăng ký Service
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IVNPayService, VNPayService>();
builder.Services.AddSingleton<PdfService>();
builder.Services.AddSingleton<ViewRenderService>();
builder.Services.AddTransient<UnitOfWork>();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));


/// Cấu hình VNpay
builder.Services.Configure<VnPayConfigOptions>(
    builder.Configuration.GetSection("VnPay"));
// 🔹 Cấu hình MVC
builder.Services.AddControllersWithViews();

// 🔹 Bật Logging để debug lỗi dễ hơn
builder.Logging.SetMinimumLevel(LogLevel.Debug);

var app = builder.Build();

// 🔹 Xử lý lỗi trong Production
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// cấu hình để chạy api 

// 🔹 Middleware (Thứ tự quan trọng)
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCookiePolicy();  // 🔥 Phải trước Authentication
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// 🔹 Cấu hình Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.SeedRolesAndAdminUser(services, configuration);
}

// 🔹 Chạy ứng dụng
app.Run();
