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
    public class RoomsController : Controller
    {
        private readonly AppDbContext dbContext;
        private readonly IHotelRepository hotelRepository;
        private readonly IRoomRepository roomRepository;
        public RoomsController(AppDbContext _dbContext, IHotelRepository _hotelRepository, IRoomRepository _roomRepository)
        {
            dbContext = _dbContext;
            hotelRepository = _hotelRepository;
            roomRepository = _roomRepository;
        }

        public IActionResult Show(string code)
        {
            var room = roomRepository.Get(code);
            if (room == null)
            {
                throw new Exception($"No se encontro datos.");
            }

            room.Hotel = room.Hotel ?? hotelRepository.Get(room.IdHotel.ToString());

            var model = new RoomViewModel { Item = room };
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        public IActionResult New(string IdHotel)
        {
            var hotel = hotelRepository.Get(IdHotel);
            if (hotel == null)
            {
                throw new Exception($"No se encontro Hotel, para asignar habitacion");
            }

            if (!hotel.IsEnabled)
            {
                TempData[AppDictionary.MensajeDanger] = $"Hotel {hotel.Code} deshabilitado";

                return RedirectToActionPreserveMethod("List");
            }

            var model = new RoomViewModel();
            model.Hotel = hotel;
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(RoomViewModel model)
        {
            DataResponse response = new DataResponse(false, "No se encontro datos.");
            if (model.Item == null)
            {
                return Ok(response);
            }

            try
            {
                model.Item.ValidateRoom();

                var room = roomRepository.Get(model.Item.Code);
                if (room != null)
                {
                    return Ok(new DataResponse(false, $"El Codigo {model.Item.Code} no esta disponible"));
                }

                var hotel = hotelRepository.Get(model.Item.IdHotel.ToString());

                if (hotel == null)
                {
                    return Ok(new DataResponse(false, $"Hotel no encontrado {model.Item.IdHotel}"));
                }

                model.Item.Hotel = hotel;

            }
            catch (Exception e)
            {
                return Ok(new DataResponse(false, e.Message));
            }

            roomRepository.Add(model.Item);

            TempData[AppDictionary.MensajeSuccess] = $"Se creo Registro";

            return Ok(new DataResponse(true, "Se creo Registro",
                        new { uri = Url.Action("Show", "Rooms", new { code = model.Item.Code, area = "" }) }
                    ));
        }

        public IActionResult Edit(string code)
        {
            var room = roomRepository.Get(code);
            if (room == null)
            {
                throw new Exception($"No se encontro datos.");
            }
            room.Hotel = room.Hotel ?? hotelRepository.Get(room.IdHotel.ToString());
            RoomViewModel model = new RoomViewModel { Item = room };
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        [HttpPost]
        public IActionResult Update(RoomViewModel model)
        {
            DataResponse response = new DataResponse(false, "No se encontro datos.");
            if (model.Item == null)
            {
                return Ok(response);
            }

            try
            {
                model.Item.ValidateRoom();

                var room = roomRepository.All().FirstOrDefault(x => x.IdBD == model.Item.IdBD);
                if (room == null)
                {
                    return Ok(new DataResponse(false, $"No se encontro habitacion disponible"));
                }

                if (room.Code != model.Item.Code)
                {
                    if (roomRepository.Get(model.Item.Code) != null)
                    {
                        return Ok(new DataResponse(false, $"El Codigo {model.Item.Code} no esta disponible"));
                    }
                }

                room.Code = model.Item.Code;
                room.Name = model.Item.Name;
                room.IsEnabled = model.Item.IsEnabled;
                room.NumberOfPeople = model.Item.NumberOfPeople;
                room.BaseCost = model.Item.BaseCost;
                room.ImposedCost = model.Item.ImposedCost;
                room.TypeOfRoom = model.Item.TypeOfRoom;
                room.Location = model.Item.Location;
                room.Observation = model.Item.Observation;

                roomRepository.Update(room);

            }
            catch (Exception e)
            {
                return Ok(new DataResponse(false, e.Message));
            }

            TempData[AppDictionary.MensajeSuccess] = $"Registro actualizado";

            return Ok(new DataResponse(true, "Registro actualizado",
                        new { uri = Url.Action("Show", "Rooms", new { code = model.Item.Code, area = "" }) }
                    ));
        }

        public IActionResult List()
        {
            var model = new RoomViewModel();
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        [HttpGet]
        public IActionResult GetElements(bool IsReporteCsv, string Code, string Name, string Location, string CodeHotel)
        {
            var Rooms = roomRepository.All();

            if (!string.IsNullOrEmpty(Code))
            {
                Rooms = Rooms.Where(x => x.Code.Contains(Code));
            }

            if (!string.IsNullOrEmpty(Name))
            {
                Rooms = Rooms.Where(x => x.Name.Contains(Name));
            }

            if (!string.IsNullOrEmpty(Location))
            {
                Rooms = Rooms.Where(x => x.Location.Contains(Location));
            }

            if (!string.IsNullOrEmpty(CodeHotel))
            {
                var hotel = hotelRepository.Get(CodeHotel);
                Rooms = Rooms.Where(x => x.IdHotel == hotel.IdHotel);
            }

            if (IsReporteCsv)
            {
                byte[] csvBytes = new CsvHelperUtil().GenerateCsvFile(Rooms);

                return File(csvBytes, "text/csv", "Room.csv");
            }

            return Ok(JsonConvert.SerializeObject(Rooms.Select(x => new { x.Code, x.Name, Hotel = x.Hotel.Code, x.IsEnabled , x.Location })));
        }

        public IActionResult Delete(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Ok(new DataResponse(false, "Parametro no valido."));
            }

            var room = roomRepository.Get(code);
            if (room == null)
            {
                throw new Exception($"No se encontro datos.");
            }

            roomRepository.Delete(room);

            TempData[AppDictionary.MensajeInfo] = $"Elemento {room.Code} eliminado";

            return RedirectToActionPreserveMethod("List");
        }

    }
}
