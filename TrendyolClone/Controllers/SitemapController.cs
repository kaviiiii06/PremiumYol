using Microsoft.AspNetCore.Mvc;
using TrendyolClone.Services;

namespace TrendyolClone.Controllers
{
    public class SitemapController : Controller
    {
        private readonly SitemapService _sitemapService;

        public SitemapController(SitemapService sitemapService)
        {
            _sitemapService = sitemapService;
        }

        [Route("sitemap.xml")]
        public async Task<IActionResult> Index()
        {
            var xml = await _sitemapService.GenerateSitemapXmlAsync();
            return Content(xml, "application/xml");
        }
    }
}
