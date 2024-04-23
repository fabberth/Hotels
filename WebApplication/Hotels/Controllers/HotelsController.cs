using Hotels.AppDbContexts;
using Hotels.Domain;
using Hotels.Models;
using Hotels.Services;
using Hotels.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Hotels.Controllers
{
    [Authorize(Policy = "CookiePolicy")]
    public class HotelsController : Controller
    {
        private readonly AppDbContext dbContext;
        private readonly IHotelRepository hotelRepository;
        public HotelsController(AppDbContext _dbContext, IHotelRepository _hotelRepository)
        {
            dbContext = _dbContext;
            hotelRepository = _hotelRepository;
        }

        public IActionResult Show(string code)
        {
            var hotel = hotelRepository.Get(code);
            if (hotel == null)
            {
                throw new Exception($"No se encontro datos.");
            }

            HotelViewModel model = new HotelViewModel { Item = hotel };
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        public IActionResult New()
        {
            var model = new HotelViewModel();
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(HotelViewModel model)
        {
            DataResponse response = new DataResponse(false, "No se encontro datos.");
            if (model.Item == null)
            {
                return Ok(response);
            }

            try
            {
                model.Item.ValidateHotel();

                var hotel = hotelRepository.Get(model.Item.Code);
                if (hotel != null)
                {
                    return Ok(new DataResponse(false, $"El Codigo {model.Item.Code} no esta disponible"));
                }

            }
            catch (Exception e)
            {
                return Ok(new DataResponse(false, e.Message));
            }

            hotelRepository.Add(model.Item);

            TempData[AppDictionary.MensajeSuccess] = $"Se creo Hotel";

            return Ok(new DataResponse(true, "Se creo Hotel",
                        new { uri = Url.Action("Show", "Hotels", new { code = model.Item.Code, area = "" }) }
                    ));
        }

        public IActionResult Edit(string code)
        {
            var hotel = hotelRepository.Get(code);
            if (hotel == null)
            {
                throw new Exception($"No se encontro datos.");
            }

            HotelViewModel model = new HotelViewModel { Item = hotel };
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        [HttpPost]
        public IActionResult Update(HotelViewModel model)
        {
            DataResponse response = new DataResponse(false, "No se encontro datos.");
            if (model.Item == null)
            {
                return Ok(response);
            }

            try
            {
                model.Item.ValidateHotel();

                var hotel = hotelRepository.All().FirstOrDefault(x=> x.IdHotel == model.Item.IdHotel);
                if (hotel == null)
                {
                    return Ok(new DataResponse(false, $"No se encontro hotel disponible"));
                }

                if(hotel.Code != model.Item.Code)
                {
                    if (hotelRepository.Get(model.Item.Code) != null)
                    {
                        return Ok(new DataResponse(false, $"El Codigo {model.Item.Code} no esta disponible"));
                    }
                }

                hotel.Code = model.Item.Code;
                hotel.Name = model.Item.Name;
                hotel.IsEnabled = model.Item.IsEnabled;

                hotelRepository.Update(hotel);

            }
            catch (Exception e)
            {
                return Ok(new DataResponse(false, e.Message));
            }

            TempData[AppDictionary.MensajeSuccess] = $"Registro actualizado";

            return Ok(new DataResponse(true, "Registro actualizado",
                        new { uri = Url.Action("Show", "Hotels", new { code = model.Item.Code, area = "" }) }
                    ));
        }

        public IActionResult List()
        {
            var model = new HotelViewModel();
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        [HttpGet]
        public IActionResult GetElements(bool IsReporteCsv, string Code, string Name, string IsEnabled)
        {
            var hotels = hotelRepository.All();

            if (!string.IsNullOrEmpty(Code))
            {
                hotels = hotels.Where(x => x.Code.Contains(Code));
            }

            if (!string.IsNullOrEmpty(Name))
            {
                hotels = hotels.Where(x => x.Name.Contains(Name));
            }

            if (!string.IsNullOrEmpty(IsEnabled) && bool.TryParse(IsEnabled, out bool rIsEnabled))
            {
                hotels = hotels.Where(x => x.IsEnabled == rIsEnabled);
            }

            if (IsReporteCsv)
            {
                byte[] csvBytes = new CsvHelperUtil().GenerateCsvFile(hotels);

                return File(csvBytes, "text/csv", "Hotels.csv");
            }

            return Ok(JsonConvert.SerializeObject(hotels.ToList()));
        }

        public IActionResult Delete(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Ok(new DataResponse(false, "Parametro no valido."));
            }

            var hotel = hotelRepository.Get(code);
            if (hotel == null)
            {
                throw new Exception($"No se encontro datos.");
            }

            hotelRepository.Delete(hotel);

            TempData[AppDictionary.MensajeInfo] = $"Hotel {hotel.Code} eliminado";

            return RedirectToActionPreserveMethod("List");
        }

    }
}
