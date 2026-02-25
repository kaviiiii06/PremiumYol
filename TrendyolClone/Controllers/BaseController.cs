using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TrendyolClone.Data;

namespace TrendyolClone.Controllers
{
    public class BaseController : Controller
    {
        protected readonly UygulamaDbContext _context;

        public BaseController(UygulamaDbContext db)
        {
            _context = db;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var ayarlar = _context.SiteAyarlari.FirstOrDefault();
                ViewBag.SiteSettings = ayarlar;
            }
            catch
            {
                ViewBag.SiteSettings = null;
            }
            
            base.OnActionExecuting(context);
        }
    }
}