using FashionShopMVC.Repositories;
using FashionShopMVC.Data;
using FashionShopMVC.Models.Domain;
using Microsoft.AspNetCore.Identity;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using AspNetCoreHero.ToastNotification;
using sportMVC.Models.Seed;
using FashionShopMVC.Repositories.@interface;
using FashionShop.Repositories;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using FashionShop.Service.Service;
using FashionShop.Service.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddTransient<FashionShopMVC.Services.IEmailSender, FashionShopMVC.Services.EmailSender>();
builder.Services.AddControllersWithViews();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();


// Register Database
builder.Services.AddDbContext<FashionShopDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});



builder.Services.AddIdentity<User,  IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<FashionShopDBContext>()
    .AddDefaultTokenProviders();


// config cookie authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Admin/Authen/Login";
    options.AccessDeniedPath = "/Admin/Authen/AccessDenied";
    options.LogoutPath = "/Admin/Authen/Logout";
    options.Cookie.Name = "FashionShopAuthCookie";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

// add authentication service




// Register Controller
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();


builder.Services.AddScoped<IFavoriteProductRepository, FavoriteProductRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
//builder.Services.AddScoped<IProvinceRepository, ProvinceRepository>();
//builder.Services.AddScoped<IDistrictRepository, DistrictRepository>();
//builder.Services.AddScoped<IWardRepository, WardRepository>();
builder.Services.AddScoped<IStatisticRepository, StatisticRepository>();

// adding email service

var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfig>();
builder.Services.AddSingleton(emailConfig);
builder.Services.AddScoped<IEmailAuthService, EmailAuthService>();

builder.Services.AddNotyf(
    config => {
        config.DurationInSeconds = 10;
        config.IsDismissable = true;
        config.Position = NotyfPosition.TopRight;
    });
// config Vnpay servic -PhuThinh
builder.Services.AddSingleton<IVnpayRespository, VnpayRepository>();



// config service authentication - lam duy

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireCustomerRole", policy => policy.RequireRole("Customer"));
});

// config sessions


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization();
//Register service authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]
            ))
    };
});

// Config identity user
//builder.Services.AddIdentityCore<User>()
//    .AddRoles<IdentityRole>()
//    .AddTokenProvider<DataProtectorTokenProvider<User>>("FashionShop")
//    .AddEntityFrameworkStores<FashionShopDBContext>()
//    .AddDefaultTokenProviders();


// Config password register
//builder.Services.Configure<IdentityOptions>(option =>
//{
//    // Thiết lập về Password
//    option.Password.RequireDigit = false; // Không bắt phải có số
//    option.Password.RequireLowercase = false; // Không bắt phải có chữ thường
//    option.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
//    option.Password.RequireUppercase = false; // Không bắt buộc chữ in
//    option.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
//    option.Password.RequiredUniqueChars = 0; // Số ký tự riêng biệt

//});
builder.Services.AddHttpContextAccessor();
    builder.Services.AddSession();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }



//app.UseNotyf();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "assets")), // Sử dụng WebRootPath để trỏ tới wwwroot
    RequestPath = "/assets" // Đường dẫn để truy cập tài nguyên tĩnh
});

    //app.UseStaticFiles(new StaticFileOptions
    //{
    //    FileProvider = new PhysicalFileProvider(
    //        Path.Combine(builder.Environment.ContentRootPath, "app")), // Sử dụng ContentRootPath
    //    RequestPath = "/app" // Đường dẫn để truy cập tài nguyên tĩnh
    //});


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "UploadFiles")),
    RequestPath = "/UploadFiles"
});


    //app.Use(async (context, next) =>
    //{
    //    if (context.Request.Path.StartsWithSegments("/Admin") && !context.User.Identity.IsAuthenticated)
    //    {
    //        context.Response.Redirect("/Admin/User/Login");
    //        return;
    //    }
    //    await next();
    //});



using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SampleData.Initialize(services);

}

app.UseSession();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "Admin",
    areaName: "Admin",
    pattern: "Admin/{controller=AdminHome}/{action=Index}/{id?}");






app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
