using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RestoApi.Data;
using RestoApi.Entities;
using Swashbuckle.AspNetCore.Filters;

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
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();

});
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

app.Run();

public class NoOpEmailSender : IEmailSender<ApplicationUser>
{
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink) => Task.CompletedTask;
    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) => Task.CompletedTask;
    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) => Task.CompletedTask;
}
