using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestoApi.Data;
using RestoApi.Entities;

var builder = WebApplication.CreateBuilder(args);
// Change (localdb)\\mssqllocaldb to your SQLEXPRESS instance
var connectionString = "Server=DESKTOP-CIC0C4Q\\SQLEXPRESS;Database=BistroStackDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";

// Add this to your service registrations


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddSingleton(TimeProvider.System);



builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IEmailSender<ApplicationUser>>(
    _ => new NoOpEmailSender());

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// 1. Setup Identity with the specific User type and DbContext
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<AppDbContext>();

// 2. Add Authorization (the tool we added last time)
builder.Services.AddAuthorization();

var app = builder.Build();

app.MapGroup("/auth").MapIdentityApi<ApplicationUser>();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class NoOpEmailSender : IEmailSender<ApplicationUser>
{
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) => Task.CompletedTask;
    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) => Task.CompletedTask;
    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) => Task.CompletedTask;
}
