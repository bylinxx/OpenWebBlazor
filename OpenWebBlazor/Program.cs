using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using OpenWebBlazor.Components;
using OpenWebBlazor.Components.Auth;
using OpenWebBlazor.Models;
using OpenWebBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Secrets/appsettings.json");

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication("WebAuth")
    .AddCookie("WebAuth", options =>
    {
        options.Cookie.Name = "WebAuth";
        options.LoginPath = "/Account/Login";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<IAuthorizationHandler, WebAuthorizationHandler>();

// blazor 不会创建与所需生存期一致的服务范围，不能使用AddDbContext
builder.Services.AddDbContext<WebDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<RoleService>();
builder.Services.AddTransient<MenuService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAntDesign();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();