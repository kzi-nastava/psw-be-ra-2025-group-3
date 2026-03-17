using Explorer.API.Middleware;
using Explorer.API.Notifications;
using Explorer.API.Services;
using Explorer.API.Startup;
using Explorer.Blog.Infrastructure;
using Explorer.Payments.Infrastructure;
using Explorer.Stakeholders.Infrastructure;
using Explorer.Tours.API.Public;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.IO;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// =======================
// ===== SERVICES ========
// =======================

builder.Services.AddControllers();
builder.Services.ConfigureSwagger(builder.Configuration);
builder.Services.AddSignalR();
// 🔥 DODATO: EKSPLICITNI CORS ZA SIGNALR
var allowedOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS")?.Split(',')
    ?? new[] { "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("_corsPolicy", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.ConfigureAuth();

builder.Services.ConfigureStakeholdersModule();
builder.Services.ConfigureBlogModule();
builder.Services.ConfigurePaymentsModule();
builder.Services.AddScoped<INotificationPublisher, SignalRNotificationPublisher>();


builder.Services.RegisterModules();
builder.Services.AddScoped<IAuthorProfileQueryService, AuthorProfileQueryService>();
builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

// =======================
// ===== APP PIPELINE ====
// =======================

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

var uploadsPath = Path.Combine(builder.Environment.WebRootPath, "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

// 🔥 CORS MORA BITI IZMEĐU UseRouting i Auth
app.UseCors("_corsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseWebSockets();
app.MapHub<Explorer.API.Hubs.NotificationHub>("/hubs/notifications");

app.Run();

namespace Explorer.API
{
    public partial class Program { }
}
