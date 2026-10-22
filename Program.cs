using AutoMapper;
using Microsoft.Data.SqlClient;
using Restaurant.Configuration;
using Restaurant.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<EmailService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//builder.Services.AddDbContext<RestaurantContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("LocalDBConnection")));
builder.Services.AddScoped<RestaurantContext>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var localDB = configuration.GetConnectionString("LocalDBConnection");
    var dockerDB = configuration.GetConnectionString("DockerDBConnection");

    string activeConnection = localDB;

    try
    {
        var connectionBuilder = new SqlConnectionStringBuilder(localDB);
        connectionBuilder.ConnectTimeout = 3;

        using var connection = new SqlConnection(connectionBuilder.ConnectionString);
        connection.Open();
    }
    catch (Exception)
    {
        activeConnection = dockerDB;
    }

    var optionsBuilder = new DbContextOptionsBuilder<RestaurantContext>();
    optionsBuilder.UseSqlServer(activeConnection);

    return new RestaurantContext(optionsBuilder.Options);
});


builder.Services.AddIdentity<CustomUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<RestaurantContext>()
    .AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Gebruiker/Login"; // Pad naar de inlogpagina
    options.AccessDeniedPath = "/Home/Index"; // Pad naar de homepagina
});


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
