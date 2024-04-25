using Hotels.AppDbContexts;
using Hotels.Domain;
using Hotels.Models;
using Hotels.Services;
using Hotels.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;

namespace Hotels.Controllers
{
    [Authorize(Policy = "CookiePolicy")]
    public class ReservesController : Controller
    {
        private readonly IHotelRepository hotelRepository;
        private readonly IRoomRepository roomRepository;
        private readonly AppDbContext dbContext;
        public ReservesController(AppDbContext _dbContext, IHotelRepository _hotelRepository, IRoomRepository _roomRepository)
        {
            dbContext = _dbContext;
            hotelRepository = _hotelRepository;
            roomRepository = _roomRepository;

        }

        public IActionResult List()
        {
            var model = new ReserveViewModel();
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        [HttpGet]
        public IActionResult GetElements(bool IsReporteCsv, string Code, string DocumentNumber, string Room, string Hotel)
        {
            var reserves = roomRepository.GetAllReservations();

            if (!string.IsNullOrEmpty(Code))
            {
                reserves = reserves.Where(x => x.Code.Contains(Code));
            }

            if (!string.IsNullOrEmpty(DocumentNumber))
            {
                reserves = reserves.Where(x => x.DocumentNumber.Contains(DocumentNumber));
            }

            if (!string.IsNullOrEmpty(Hotel))
            {
                var hotel = hotelRepository.Get(Hotel);
                if (hotel != null)
                {
                    var listId = roomRepository.GetAllByHotel(hotel).Select(x=> x.IdBD);

                    reserves = reserves.Where(x => listId.Contains(x.RoomIdBD));
                }
            }

            if (!string.IsNullOrEmpty(Room))
            {
                var room = roomRepository.Get(Room);
                if (room != null)
                {
                    reserves = reserves.Where(x => x.RoomIdBD == room.IdBD);
                }
            }

            if (IsReporteCsv)
            {
                byte[] csvBytes = new CsvHelperUtil().GenerateCsvFile(reserves);

                return File(csvBytes, "text/csv", "Reserve.csv");
            }

            return Ok(JsonConvert.SerializeObject(reserves.Select(x => new { x.IdReserve, Room = x.Room == null? "" : x.Room.Code, 
                x.DocumentType, x.DocumentNumber, x.DateOfAdmission, x.DateOfExit, x.DateOfCreation})));
        }

        public IActionResult Show(string code)
        {
            var reserve = roomRepository.GetReservation(code);
            if (reserve == null)
            {
                throw new Exception($"No se encontro datos.");
            }

            reserve.Room = reserve.Room ?? roomRepository.Get(reserve.RoomIdBD.ToString());
            //Notify(reserve);
            var model = new ReserveViewModel { Item = reserve };
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        public IActionResult Booking(string DateOfAdmission, string DateOfExit, int NumberOfPeople, string Location)
        {

            var model = new ReserveViewModel();
            var idRooBusy = new List<int>();
            var dateNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);

            if (DateTime.TryParse(DateOfAdmission, out var rDateOfAdmission))
            {
                if(rDateOfAdmission < dateNow)
                {
                    rDateOfAdmission = dateNow;
                }

                var idRoo = roomRepository.GetAllReservations().Where(x => 
                
                    x.DateOfAdmission <= rDateOfAdmission && x.DateOfExit >= rDateOfAdmission

                ).Select(x => x.RoomIdBD);
                model.DateOfAdmission = rDateOfAdmission.ToString("yyyy-MM-dd");
                idRooBusy.AddRange(idRoo);
            }

            if (DateTime.TryParse(DateOfExit, out var rDateOfExit))
            {
                if (rDateOfExit < dateNow)
                {
                    rDateOfExit = dateNow;
                }

                if (string.IsNullOrEmpty(model.DateOfAdmission))
                {
                    rDateOfAdmission = dateNow;
                    model.DateOfAdmission = rDateOfAdmission.ToString("yyyy-MM-dd");
                }

                var idRoo = roomRepository.GetAllReservations().Where(x =>

                    x.DateOfAdmission <= rDateOfExit && x.DateOfExit >= rDateOfAdmission

                ).Select(x => x.RoomIdBD);
                model.DateOfExit = rDateOfExit.ToString("yyyy-MM-dd");
                idRooBusy.AddRange(idRoo);
            }

            var rooms = roomRepository.All();

            if (idRooBusy.Any())
            {
                rooms = rooms.Where(x => !idRooBusy.Contains(x.IdBD));
            }

            if(NumberOfPeople > 0)
            {
                model.NumberOfPeople = NumberOfPeople;
                rooms = rooms.Where(x => x.NumberOfPeople >= NumberOfPeople);
            }

            if (!string.IsNullOrEmpty(Location))
            {
                model.Location = Location;
                rooms = rooms.Where(x => x.Location.Contains(Location));
            }

            model.Rooms = rooms.ToList();
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        public IActionResult New(string RoomIdBD, string DateOfAdmission, string DateOfExit)
        {
            var room = roomRepository.Get(RoomIdBD.ToString());
            if (room == null)
            {
                throw new Exception($"No se encontro habitacion, para reservar");
            }

            if (!room.IsEnabled)
            {
                TempData[AppDictionary.MensajeDanger] = $"Habitacion {room.Code} deshabilitada";

                return RedirectToActionPreserveMethod("Booking");
            }

            var model = new ReserveViewModel();
            model.Room = room;
            model.DateOfAdmission = DateOfAdmission;
            model.DateOfExit = DateOfExit;
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(ReserveViewModel model)
        {
            DataResponse response = new DataResponse(false, "No se encontro datos.");
            if (model.Item == null)
            {
                return Ok(response);
            }

            try
            {
                model.Item.ValidateReserve();

                var room = roomRepository.Get(model.Item.RoomIdBD.ToString());
                if (room == null)
                {
                    return Ok(new DataResponse(false, $"No se encontro habitacion"));
                }

                if (!room.IsEnabled)
                {
                    return Ok(new DataResponse(false, $"Habitacion {room.Code} deshabilitada"));
                }
                var dateNow = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
                if (model.Item.DateOfAdmission < dateNow)
                {
                    return Ok(new DataResponse(false, $"Fecha de ingreso no valida {model.Item.DateOfAdmission}"));
                }
                model.Item.DateOfExit = model.Item.DateOfExit.AddHours(23);
                if (model.Item.DateOfAdmission > model.Item.DateOfExit)
                {
                    return Ok(new DataResponse(false, $"Fecha de ingreso no valida {model.Item.DateOfAdmission}"));
                }

                var nReserver = roomRepository.GetRoomsByReservationDates(model.Item.DateOfAdmission, model.Item.DateOfExit);

                if (nReserver.Where(x=> x.IdBD == room.IdBD).Count() > 0)
                {
                    return Ok(new DataResponse(false, $"Habitacion NO disponible a la fecha {model.Item.DateOfAdmission} a {model.Item.DateOfExit}"));
                }

                model.Item.Room = room;
                model.Item.DateOfCreation = DateTime.Now.ToUniversalTime();
            }
            catch (Exception e)
            {
                return Ok(new DataResponse(false, e.Message));
            }

            roomRepository.AddReservation(model.Item);
            Notify(model.Item);
            TempData[AppDictionary.MensajeSuccess] = $"Se creo Registro";

            return Ok(new DataResponse(true, "Se creo Registro",
                        new { uri = Url.Action("Show", "Reserves", new { code = model.Item.IdReserve, area = "" }) }
                    ));
        }

        private void Notify(Reserve reserve)
        {
            var SmtpCorreo = dbContext.Configurations.FirstOrDefault(x => x.Name == "SmtpCorreo");
            if (SmtpCorreo == null || string.IsNullOrEmpty(SmtpCorreo.Value))
            {
                Console.WriteLine("Credenciales de correo NO configurada");
                return;
            }

            var SmtpPass = dbContext.Configurations.FirstOrDefault(x => x.Name == "SmtpPass");
            if (SmtpPass == null || string.IsNullOrEmpty(SmtpPass.Value))
            {
                Console.WriteLine("Credenciales de contrasena NO configurada");
                return;
            }

            string smtpHost = "smtp.hostinger.com";
            int smtpPort = 587;
            string emailFrom = SmtpCorreo.Value;
            string password = SmtpPass.Value;
            string emailTo = reserve.Email;

            MailMessage mensaje = new MailMessage(emailFrom, emailTo);
            mensaje.Subject = "Reservación Exitosa!";
            mensaje.Body = $@"Estimado {reserve.FullName},
Nos complace informarte que tu reserva de habitación ha sido confirmada con éxito. A continuación, te proporcionamos los detalles de tu reserva:
- Fecha de ingreso: {reserve.DateOfAdmission}
- Fecha de salida: {reserve.DateOfExit}
- Tipo de Habitación: {reserve.Room.TypeOfRoom}
- Codigo de Habitación: {reserve.Room.Code}

Si tienes alguna pregunta o necesitas asistencia adicional, no dudes en contactarnos. Estamos aquí para ayudarte y asegurarnos de que tengas una estancia placentera.

¡Esperamos con ansias darte la bienvenida pronto!

Atentamente, Hotels";

            SmtpClient clienteSmtp = new SmtpClient(smtpHost, smtpPort);
            clienteSmtp.Credentials = new NetworkCredential(emailFrom, password);
            clienteSmtp.EnableSsl = true;

            try
            {
                clienteSmtp.Send(mensaje);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar el correo electrónico: " + ex.Message);
            }
        }
    }
}
