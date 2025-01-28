using static WebAppPruebaTecnica.Pages.LoginModel;
using System.Security.Claims;
using WebAppPruebaTecnica.Models;

namespace WebAppPruebaTecnica.Service
{
    public class UserService // Servicio para acceder globalmente a la informacion del cliente para utilizarse en el resto de paginas.
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserData GetUserData()
        {
            var user = _httpContextAccessor.HttpContext.User;
            return new UserData
            {
                Id_Cliente = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                Username = user.FindFirst(ClaimTypes.Name)?.Value,
                FirstName = user.FindFirst("FirstName")?.Value,
                MiddleName = user.FindFirst("MiddleName")?.Value,
                LastName1 = user.FindFirst("LastName1")?.Value,
                LastName2 = user.FindFirst("LastName2")?.Value
            };
        }
    }
}
