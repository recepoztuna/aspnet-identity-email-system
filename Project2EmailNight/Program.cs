using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Project2EmailNight.Context;
using Project2EmailNight.Entities;
using Project2EmailNight.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<EmailContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity Yapýlandýrmasý
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
	// Email doðrulama (þimdilik kapalý)
	options.SignIn.RequireConfirmedEmail = false;

	// Þifre gereksinimleri
	options.Password.RequireDigit = true;
	options.Password.RequiredLength = 6;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireLowercase = false;

	// Kullanýcý ayarlarý
	options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<EmailContext>()
.AddErrorDescriber<CustomIdentityValidator>()
.AddDefaultTokenProviders();  // ? BUNU EKLE (þifre sýfýrlama için)


// Cookie Ayarlarý - BUNU EKLE
builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Login/UserLogin";
	options.LogoutPath = "/Login/Logout";
	options.AccessDeniedPath = "/Home/Index";
	options.ExpireTimeSpan = TimeSpan.FromDays(30);
	options.SlidingExpiration = true;
});


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
