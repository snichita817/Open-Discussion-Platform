using ForumApp.Data;
using ForumApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)     // face ca autentificarea sa lucreze, UI, cookies
    .AddRoles<IdentityRole>()                                                                                      // adauga serviciul de management al rolurilor (!!INAINTE DE BAZA DE DATE)!!!)
    .AddEntityFrameworkStores<ApplicationDbContext>();                                                             // realizeaza conexiunea cu baza de date, stringul de conexiune aflandu-se in appsetings.json

builder.Services.AddControllersWithViews();

var app = builder.Build();

// CreateScope ofera acces la instanta curenta a aplicatiei
// var scope are un service provider = folosit pentru a injecta dependente in bd
// dependente -> bd, cookie, sesiune, autentificare
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);          // dupa pasul asta vedem baza de date populata
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
app.MapRazorPages();

app.MapControllerRoute(
    name: "sortareForum",
    pattern: "{controller=Sections}/{action=Show}/{id}/{showOrder?}");

app.MapControllerRoute(
    name: "sortareSubforum",
    pattern: "{controller=Forums}/{action=Show}/{id}/{showOrder?}");

app.Run();
