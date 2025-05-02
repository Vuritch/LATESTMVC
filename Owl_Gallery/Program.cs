using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Owl_Gallery.Data;
using Owl_Gallery.Models;
using Owl_Gallery.Services; // 👈 You need this for EmailSender

var builder = WebApplication.CreateBuilder(args);

// 1) Database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) Email Service for Newsletter
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

// 3) Authentication setup
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Login/Login";
    options.LogoutPath = "/Login/Logout";
    options.Cookie.Name = "OwlGalleryAuth";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    // ✅ Force Google to always ask user which account to use
    options.Events.OnRedirectToAuthorizationEndpoint = context =>
    {
        context.Response.Redirect(context.RedirectUri + "&prompt=select_account");
        return Task.CompletedTask;
    };
});

// 4) MVC and Views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 5) Middleware pipeline
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
