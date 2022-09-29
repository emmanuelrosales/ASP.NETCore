using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Store.Data;
using System.Diagnostics;
using WA4.Models;
using static WA4.Models.HomeIndexViewModel;
using WA4.Extensions;

namespace WA4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NWContext _db;

        public HomeController(ILogger<HomeController> logger, NWContext db)
        {
            _logger = logger;
            this._db = db;
        }

        public IActionResult Index(HomeIndexViewModel m)
        {
            m.Groups = (List<CategoryGroupViewModel>)(from p in _db.Products.Where(p => p.ProductName.Contains(m.Filter)).Include(p => p.Category).ToList()
                                                      group p by p.Category?.CategoryName ?? "Sin Categoría" into CategoryProducts
                                                      select new CategoryGroupViewModel()
                                                      {
                                                          CategoryName = CategoryProducts.Key,
                                                          Products = CategoryProducts.ToList()
                                                      }).ToList();

            return View(m);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult IndexPartial(int? id)
        {
            var isAjax = Request.IsAjaxRequest();

            if (id != null)
            {
                return PartialView("ProductPartial", _db.Products.Where(p => p.ProductId == id).SingleOrDefault());
            }
            else
            {
                return NotFound();
            }
        }

        public IActionResult IndexViewComponent(int? id)
        {
            var isAjax = Request.IsAjaxRequest();

            if (id != null)
            {
                return ViewComponent("Product", new { id });
            }
            else
            {
                return NotFound();
            }
        }
    }
}