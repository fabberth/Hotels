using Hotels.AppDbContexts;
using Hotels.Models;
using Hotels.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Hotels.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext dbContext;
        public HomeController(ILogger<HomeController> logger, AppDbContext _dbContext)
        {
            _logger = logger;
            dbContext = _dbContext;
        }

        [Authorize(Policy = "CookiePolicy")]
        public IActionResult Index()
        {
            var model = new AppUserViewModel();
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        #region Configuration

        [Authorize(Policy = "CookiePolicyAdmin")]
        public IActionResult Configuration()
        {
            var model = new ConfigurationViewModel();
            model.Items = dbContext.Configurations.AsQueryable().OrderBy(x => x.Module).ToList();
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveConfiguration(ConfigurationViewModel model)
        {
            DataResponse response = new DataResponse(false, "No se encontro datos.");
            if (model.parameterItems == null || model.parameterItems.Count < 1)
            {
                return Ok(response);
            }

            foreach (var item in model.parameterItems)
            {
                var spli = item.Key.Split("-");
                string module = spli[0];
                string _Name = spli[1];
                var confi = dbContext.Configurations.FirstOrDefault(x => x.Module == module && x.Name == _Name);
                if (confi != null)
                {
                    if (confi.Value != item.Value)
                    {
                        confi.Value = item.Value;
                        dbContext.SaveChanges();
                    }
                }
            }

            TempData[AppDictionary.MensajeSuccess] = $"Configuración guardada";

            return Ok(new DataResponse(true, "Configuración guardada",
                new { uri = Url.Action("Configuration", "Home", new { area = "" }) }
                ));
        }

        #endregion

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
