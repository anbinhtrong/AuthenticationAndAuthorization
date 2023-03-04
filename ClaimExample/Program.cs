using ClaimExample.Data;
using ClaimExample.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
        options => {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequiredLength = 4;
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            //Other options go here
        }
        )
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(config =>
{
    config.Cookie.Name = "NgocTho";
    config.LoginPath = "/Home/Login";
    config.AccessDeniedPath = "/Home/UserAccessDenied";
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var services = builder.Services;
services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();
//add custom data
await SeedData(app);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


//app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


static async Task SeedData(WebApplication app)
{
    using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
        var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
        var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
        if (userManager == null || roleManager == null) return;
        var hasRole = roleManager.Roles.Any();
        if (!hasRole)
        {
            await ContextSeed.SeedRolesAsync(userManager, roleManager);
        }
        var hasUser = userManager.Users.Any();
        if (!hasUser)
        {
            await ContextSeed.SeedSuperAdminAsync(userManager, roleManager);
        }
    } 
}