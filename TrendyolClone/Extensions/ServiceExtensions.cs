using TrendyolClone.Services;

namespace TrendyolClone.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // HTTP Clients
            services.AddHttpClient<DropshippingService>();
            services.AddHttpClient<ApiService>();

            // Scoped Services
            services.AddScoped<IDropshippingService, DropshippingService>();
            services.AddScoped<ApiService>();
            services.AddScoped<UrunServisi>();
            services.AddScoped<SiparisServisi>();
            services.AddScoped<IPaymentService, IyzicoPaymentService>();
            services.AddScoped<ILoggingService, LoggingService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IUrunVaryasyonService, UrunVaryasyonService>();
            
            // YENİ: Kupon ve Kargo Servisleri
            services.AddScoped<IKuponService, KuponService>();
            services.AddScoped<IKargoService, KargoService>();
            services.AddScoped<IKayitliSepetService, KayitliSepetService>();
            
            // YENİ: Arama Servisi
            services.AddScoped<IAramaService, AramaService>();
            
            // YENİ: Gelişmiş Sipariş Servisi
            services.AddScoped<IGelismisSiparisService, GelismisSiparisService>();
            
            // YENİ: Bildirim Servisleri
            services.AddScoped<IBildirimService, BildirimService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<ISmsSender, SmsSender>();
            
            // YENİ: Admin Servisleri
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IAdminOrderService, AdminOrderService>();
            services.AddScoped<IRaporService, RaporService>();
            
            // YENİ: Satıcı Paneli Servisleri
            services.AddScoped<ISaticiService, SaticiService>();
            services.AddScoped<ISaticiUrunService, SaticiUrunService>();
            services.AddScoped<ISaticiSiparisService, SaticiSiparisService>();
            services.AddScoped<ISaticiFinansService, SaticiFinansService>();
            
            // YENİ: Yorum & Değerlendirme Servisleri
            services.AddScoped<IYorumService, YorumService>();
            
            // YENİ: SEO & Marketing Servisleri
            services.AddScoped<ISeoService, SeoService>();
            services.AddScoped<SitemapService>();
            services.AddScoped<AnalyticsService>();

            // Singleton Services
            services.AddSingleton<ICacheService, CacheService>();

            return services;
        }

        public static IServiceCollection AddCustomCaching(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            
            return services;
        }

        public static IServiceCollection AddCustomSession(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Strict;
            });

            return services;
        }
    }
}
