using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Middleware;
using TrendyolClone.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Preserve property names as-is (don't convert to camelCase)
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Entity Framework bağlantısı - PostgreSQL veya SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UygulamaDbContext>(options =>
{
    if (connectionString?.Contains("postgresql") == true || connectionString?.Contains("postgres") == true)
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        options.UseSqlite(connectionString ?? "Data Source=trendyol.db");
    }
});

// Custom Extensions
builder.Services.AddCustomCaching();
builder.Services.AddCustomSession();
builder.Services.AddApplicationServices();

// Logging configuration
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<UygulamaDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Global error handling
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// CSP ayarları
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Content-Security-Policy", 
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net https://code.jquery.com https://cdnjs.cloudflare.com; " +
        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
        "font-src 'self' https://cdnjs.cloudflare.com; " +
        "img-src 'self' data: https: http:;");
    
    // Security Headers
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    
    await next();
});

// Session ÖNCE başlatılmalı
app.UseSession();
app.UseAuthorization();

// Rate Limiting Middleware - Session'dan SONRA
if (builder.Configuration.GetValue<bool>("RateLimiting:EnableRateLimiting", true))
{
    app.UseMiddleware<RateLimitingMiddleware>();
}

// Health check endpoint
app.MapHealthChecks("/health");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Veritabanını oluştur ve örnek verileri ekle
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UygulamaDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // Migrations uygula (PostgreSQL için)
        logger.LogInformation("Veritabanı migrations uygulanıyor...");
        context.Database.Migrate();
        logger.LogInformation("✓ Veritabanı migrations başarıyla uygulandı.");
        
        // Sadece veritabanı boşsa seed data ekle
        if (!context.Kullanicilar.Any() || !context.Yoneticiler.Any())
        {
            logger.LogInformation("Seed data ekleniyor...");
            TrendyolClone.Data.VeriEkleyici.VeriEkle(context);
            logger.LogInformation("✓ Seed data başarıyla eklendi.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Veritabanı başlatma sırasında hata oluştu.");
        // Production'da hata fırlat
        if (!app.Environment.IsDevelopment())
        {
            throw;
        }
    }
}

app.Run();
