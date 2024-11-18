using Microsoft.Extensions.FileProviders;
using sportMVC.Models.Seed;
using FashionShopMVC.Extensions;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();



// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

builder.Services.ConfigureDatabase(configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureAuthentication(configuration);
builder.Services.ConfigureRepositories();
builder.Services.ConfigureEmailService(configuration);
builder.Services.ConfigureNotyf();
builder.Services.ConfigureSession();











builder.Services.AddHttpContextAccessor();
//builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// static files
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "assets")), // Sử dụng WebRootPath để trỏ tới wwwroot
    RequestPath = "/assets" // Đường dẫn để truy cập tài nguyên tĩnh
});

    

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "UploadFiles")),
    RequestPath = "/UploadFiles"
});


// adding sample data into database when first run
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
    pattern: "Admin/{controller=Statistics}/{action=Index}/{id?}");





app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
