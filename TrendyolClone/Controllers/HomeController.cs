using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendyolClone.Data;
using TrendyolClone.Models;

namespace TrendyolClone.Controllers;

public class HomeController : BaseController
{
    public HomeController(UygulamaDbContext db) : base(db) { }

    public async Task<IActionResult> Index()
    {
        try 
        {
            var urunler = await _context.Urunler
                .Include(p => p.Kategori)
                .Where(p => p.Aktif)
                .OrderByDescending(p => p.Puan)
                .Take(8)
                .ToListAsync();

            var kategoriler = await _context.Kategoriler
                .Where(c => c.Aktif)
                .Take(6)
                .ToListAsync();

            ViewBag.Categories = kategoriler;
            return View(urunler);
        }
        catch
        {
            return View(new List<Urun>());
        }
    }

    public IActionResult Privacy() => View();
    public IActionResult About() => View();
    public IActionResult Contact() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new HataModeli { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
