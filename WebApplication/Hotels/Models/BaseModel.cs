using Hotels.Utilities;
using System.Security.Claims;
using System.Security.Principal;

namespace Hotels.Models
{
    public class BaseModel : BaseModel<object>
    {

    }
    public class BaseModel<T>
    {
        public string Id { get; set; }
        public T Item { get; set; }
        public List<AppAction> AppActionsBanner { get; set; }

        public void GetActionsBanner(IIdentity Identity)
        {
            List<AppAction> actions = new List<AppAction> {
                        new AppAction(1, "Reservar", new AppUriAction("List", "Room"))
            };

            if (this.AppActionsBanner == null)
                this.AppActionsBanner = actions;
            else
                this.AppActionsBanner.AddRange(actions);

            if (Identity != null && Identity.IsAuthenticated)
            {
                var identity = Identity as ClaimsIdentity;
                if (identity == null)
                    return;

                var Claim = identity.Claims.FirstOrDefault(x => x.Value == AppDictionary.AuthenticationAdmin);

                if (Claim != null)
                {
                    this.AppActionsBanner.Add(new AppAction(2, "Usuarios", new AppUriAction("ListUsers", "Account", new { area = "" })));
                    this.AppActionsBanner.Add(new AppAction(3, "Configuracion", new AppUriAction("Configuration", "Home", new { area = "" })));
                }
            }

            this.AppActionsBanner = this.AppActionsBanner.OrderBy(x => x.Position).ToList();
        }
    }
}
