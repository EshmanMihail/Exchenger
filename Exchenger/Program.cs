using Exchenger.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });
builder.Services.AddAuthorization();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ExchengerDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );

//CreateTableRoutes();

app.Run();


void CreateTableRoutes()
{
    string[] controllers = { "Currency", "Currency", "Customer", "Customer", "Operation" };

    foreach (var controller in controllers)
    {
        app.MapControllerRoute(
            name: controller,
            pattern: $"{{controller={controller}}}/{{action=ShowTable}}");
    }
}
