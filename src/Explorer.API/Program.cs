using System.IO;
using Explorer.API.Middleware;
using Explorer.API.Notifications;
using Explorer.API.Startup;
using Explorer.Blog.Infrastructure;
using Explorer.Tours.API.Public;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// =======================
// ===== SERVICES ========
// =======================

builder.Services.AddControllers();
builder.Services.ConfigureSwagger(builder.Configuration);
builder.Services.AddSignalR();

// 🔥 DODATO: EKSPLICITNI CORS ZA SIGNALR
builder.Services.AddCors(options =>
{
    options.AddPolicy("_corsPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // 🔥 OBAVEZNO ZA SIGNALR
    });
});

builder.Services.ConfigureAuth();

builder.Services.ConfigureBlogModule();
builder.Services.AddScoped<INotificationPublisher, SignalRNotificationPublisher>();

builder.Services.RegisterModules();

builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

// =======================
// ===== APP PIPELINE ====
// =======================

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

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

app.UseHttpsRedirection();
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
