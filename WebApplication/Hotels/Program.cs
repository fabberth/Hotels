using Hotels.AppDbContexts;
using Hotels.Services;
using Hotels.Utilities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var DefaultConnection = $"{Environment.GetEnvironmentVariable("DB_CONNECTION")}"; // DOCKER
if (string.IsNullOrEmpty(DefaultConnection))
{
    DefaultConnection = appsettings["DefaultConnection"];
}
if (string.IsNullOrEmpty(DefaultConnection))
{
    throw new Exception("\"DefaultConnection\" key not found");
}

//Configure the EntityFramework context SqlServer
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(DefaultConnection)
);

builder.Services.AddScoped<IHotelRepository, HotelRepository>();

builder.Services.AddScoped<IRoomRepository, RoomRepository>();
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddMvc();

#region Authentication
// Configuración de autenticación
builder.Services.AddAuthentication(AppDictionary.AuthenticationScheme)
                .AddCookie(AppDictionary.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/Account/Login";
                });
#endregion

#region Policy

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CookiePolicy", policy =>
    {
        policy.AuthenticationSchemes.Add(AppDictionary.AuthenticationScheme);
        policy.RequireRole(AppDictionary.AuthenticationCommon);
    });

    options.AddPolicy("CookiePolicyAdmin", policy =>
    {
        policy.AuthenticationSchemes.Add(AppDictionary.AuthenticationScheme);
        policy.RequireRole(AppDictionary.AuthenticationAdmin);
    });

});

#endregion

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
