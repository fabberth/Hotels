using Hotels.AppDbContexts;
using Hotels.Domain;
using Hotels.Models;
using Hotels.Services;
using Hotels.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Hotels.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext dbContext;
        private readonly IHotelRepository hotelRepository;
        public AccountController(AppDbContext _dbContext, IHotelRepository _hotelRepository)
        {
            dbContext = _dbContext;
            hotelRepository = _hotelRepository;
        }

        #region UsersController

        [Authorize(Policy = "CookiePolicyAdmin")]
        public IActionResult ShowUser(string identificacion)
        {
            var usuario = dbContext.Users.FirstOrDefault(x => x.Id == identificacion || x.UserName == identificacion);
            if (usuario == null)
            {
                throw new Exception($"No se encontro usuario con la identificacion {identificacion}");
            }

            AppUserViewModel model = new AppUserViewModel { Item = usuario };
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        [Authorize(Policy = "CookiePolicyAdmin")]
        public IActionResult NewUser()
        {
            var model = new BaseModel();
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        [Authorize(Policy = "CookiePolicyAdmin")]
        public IActionResult EditUser(string identificacion)
        {
            var usuario = dbContext.Users.FirstOrDefault(x => x.Id == identificacion);
            if (usuario == null)
            {
                throw new Exception($"No se encontro usuario con la identificacion {identificacion}");
            }

            AppUserViewModel model = new AppUserViewModel { Item = usuario };
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        [Authorize(Policy = "CookiePolicyAdmin")]
        public IActionResult DeleteUser(string identificacion)
        {
            if (string.IsNullOrEmpty(identificacion))
            {
                return Ok(new DataResponse(false, "Parametro no valido."));
            }

            var usuario = dbContext.Users.FirstOrDefault(x => x.Id == identificacion);

            if (usuario == null)
            {
                return Ok(new DataResponse(false, $"No se encontro usuario con la identificacion {identificacion}"));
            }

            dbContext.Users.Remove(usuario);
            dbContext.SaveChanges();

            TempData[AppDictionary.MensajeInfo] = $"Usuario {identificacion} eliminado";

            return RedirectToActionPreserveMethod("ListUsers", "Account");
        }

        [Authorize(Policy = "CookiePolicyAdmin")]
        [HttpPost]
        public IActionResult UpdateUser(AppUserViewModel model)
        {
            DataResponse response = new DataResponse(false, "No se encontro datos.");
            if (model.Item == null)
            {
                return Ok(response);
            }

            try
            {
                model.Item.ValidateUser();
                if (string.IsNullOrEmpty(model.Item.Password))
                {
                    response.Msg = "password no es valido";
                    return Ok(response);
                }

            }
            catch (Exception e)
            {
                return Ok(new DataResponse(false, e.Message));
            }

            var usuario = dbContext.Users.FirstOrDefault(x => x.Id == model.Item.Id);

            if (usuario == null)
            {
                return Ok(new DataResponse(false, $"No se encontro usuario con la identificacion {model.Item.Id}"));
            }

            if (usuario.UserName != model.Item.UserName && dbContext.Users.FirstOrDefault(x => x.UserName == model.Item.UserName) != null)
            {
                return Ok(new DataResponse(false, $"UserName NO disponible {model.Item.UserName}"));
            }

            usuario.UserName = model.Item.UserName;
            usuario.Names = model.Item.Names;
            usuario.LastName = model.Item.LastName;
            usuario.Password = model.Item.Password;
            usuario.Email = model.Item.Email;
            dbContext.SaveChanges();

            TempData[AppDictionary.MensajeSuccess] = $"Se actualizo Usuario";

            return Ok(new DataResponse(true, "Se actualizo Usuario",
                new { uri = Url.Action("ShowUser", "Account", new { identificacion = usuario.Id, area = "" }) }
                ));
        }

        [Authorize(Policy = "CookiePolicyAdmin")]
        [HttpPost]
        public IActionResult CreateUser(AppUserViewModel model)
        {
            DataResponse response = new DataResponse(false, "No se encontro datos.");
            if (model.Item == null)
            {
                return Ok(response);
            }

            try
            {
                model.Item.ValidateUser();

                var usuario = dbContext.Users.FirstOrDefault(x => x.Id == model.Item.Id);
                if (usuario != null)
                {
                    return Ok(new DataResponse(false, $"La identificacion {model.Item.Id} no esta disponible"));
                }

                usuario = dbContext.Users.FirstOrDefault(x => x.UserName == model.Item.UserName);
                if (usuario != null)
                {
                    return Ok(new DataResponse(false, $"Username {model.Item.UserName} no esta disponible"));
                }

                if (string.IsNullOrEmpty(model.Item.Password))
                {
                    response.Msg = "password no es valido";
                    return Ok(response);
                }
            }
            catch (Exception e)
            {
                return Ok(new DataResponse(false, e.Message));
            }

            dbContext.Users.Add(model.Item);
            dbContext.SaveChanges();

            TempData[AppDictionary.MensajeSuccess] = $"Se creo Usuario";

            return Ok(new DataResponse(true, "Se creo Usuario",
                        new { uri = Url.Action("ShowUser", "Account", new { identificacion = model.Item.Id, area = "" }) }
                    ));

        }

        [Authorize(Policy = "CookiePolicyAdmin")]
        public IActionResult ListUsers()
        {
            var model = new BaseModel();
            model.GetActionsBanner(HttpContext.User.Identity);
            return View(model);
        }

        [Authorize(Policy = "CookiePolicyAdmin")]
        [HttpGet]
        public IActionResult GetUsers(bool IsReporteCsv, string identificacion, string nombres, string username)
        {
            var users = dbContext.Users.AsQueryable();

            if (!string.IsNullOrEmpty(identificacion))
            {
                users = users.Where(x => x.Id == identificacion);
            }

            if (!string.IsNullOrEmpty(nombres))
            {
                users = users.Where(x => x.Names.Contains(nombres));
            }

            if (!string.IsNullOrEmpty(username))
            {
                users = users.Where(x => x.UserName.Contains(username));
            }

            if (IsReporteCsv)
            {
                byte[] csvBytes = new CsvHelperUtil().GenerateCsvFile<AppUser>(users);

                return File(csvBytes, "text/csv", "Usuarios.csv");
            }

            return Ok(JsonConvert.SerializeObject(users.ToList()));
        }

        #endregion

        public IActionResult AccessDenied()
        {
            TempData[AppDictionary.MensajeInfo] = $"No autorizado.";
            return Redirect("/");
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(AppDictionary.AuthenticationScheme);

            return Redirect("/");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ValidateCredential(string identificacion, string password)
        {
            var usuario = dbContext.Users.FirstOrDefault(x => x.Id == identificacion || x.UserName == identificacion);

            DataResponse response = new DataResponse(false, $"No se encontro usuario con identificacion {identificacion}");
            if (usuario == null)
            {
                return Ok(response);
            }

            if (password != usuario.Password)
            {
                response.Msg = "Credenciales incorrectas.";
                return Ok(response);
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, usuario.UserName),
                                           new Claim(ClaimTypes.NameIdentifier, usuario.Id),
                                           new Claim(ClaimTypes.Role, AppDictionary.AuthenticationCommon)
            };

            var AdminUsers = dbContext.Configurations.AsQueryable().FirstOrDefault(x => x.Name == "AdminUsers");

            if (AdminUsers == null)
            {
                BuildSession(claims);
                return Ok(new DataResponse(true, ""));
            }

            var users = AdminUsers.Value.Split(",");
            if (users.FirstOrDefault(x => x == usuario.UserName) != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, AppDictionary.AuthenticationAdmin));
            }

            BuildSession(claims);

            return Ok(new DataResponse(true, ""));
        }

        private void BuildSession(IEnumerable<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims, AppDictionary.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            HttpContext.SignInAsync(AppDictionary.AuthenticationScheme, principal);
        }
    }
}
