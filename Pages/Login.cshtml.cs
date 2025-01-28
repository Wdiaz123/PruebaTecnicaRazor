using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using WebAppPruebaTecnica.Models;

namespace WebAppPruebaTecnica.Pages
{
    public class LoginModel : PageModel
    {

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        private readonly IHttpClientFactory _httpClientFactory;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

      
        public string Message { get; set; }

        public void OnGet()
        {
          
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var client = new HttpClient();
                var url = "https://localhost:7289/api/Clients/verify-login";
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(Username), "username");
                formData.Add(new StringContent(Password), "password");

                var response = await client.PostAsync(url, formData);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var userData = JsonSerializer.Deserialize<UserData>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (userData != null)
                    {
                        // Crear los claims del usuario
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userData.Id_Cliente.ToString()),
                    new Claim(ClaimTypes.Name, userData.Username),
                    new Claim("FullName", $"{userData.FirstName} {userData.LastName1} {userData.LastName2}"),
                    new Claim("MiddleName", userData.MiddleName),
                    new Claim("FirstName", userData.FirstName),
                    new Claim("LastName1", userData.LastName1),
                    new Claim("LastName2", userData.LastName2)
                };

                        // Crear la identidad y el principal
                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        // Establecer la cookie de autenticación
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid user data received.");
                        return Page();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Login failed. Please check your credentials.");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }


       

    }
}

