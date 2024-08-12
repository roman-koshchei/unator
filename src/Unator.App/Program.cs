using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Unator.App.Data;
using Unator.App.Handlers;
using Unator.App.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UnatorDb>(options => options.UseSqlite("Data Source=./unator.db"));
builder.Services
    .AddIdentity<DbUser, DbRole>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 8;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireDigit = true;
    })
    .AddEntityFrameworkStores<UnatorDb>()
    .AddDefaultTokenProviders();

builder.Services
    .AddAuthentication()
    .AddCookie();

builder.Services.AddAuthorization();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);

    options.LoginPath = new PathString(AuthHandlers.AuthRoute.Url());
    options.AccessDeniedPath = new PathString(AuthHandlers.AuthRoute.Url());
    options.LogoutPath = new PathString(AuthHandlers.LogoutRoute.Url());
    options.ReturnUrlParameter = "comeback";

    options.SlidingExpiration = true;
});

// Add services to the container.

builder.Services.AddHostedService<EmailWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

EmailHandlers.Map(app);

app.Run();