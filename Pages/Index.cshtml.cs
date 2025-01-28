using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using WebAppPruebaTecnica.Models;
using WebAppPruebaTecnica.Service;

namespace WebAppPruebaTecnica.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly UserService _userService; // referencia al servicio para acceder a data del usuario global
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public List<CardData> Cards { get; set; }
        public IndexModel(ILogger<IndexModel> logger, UserService userService, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _userService = userService;
            _httpClientFactory = httpClientFactory;
        }

        public void OnGet()
        {
            var userData = _userService.GetUserData();
            Cards = LoadClientCards();
            FullName = $"{userData.FirstName} {userData.MiddleName} {userData.LastName1} {userData.LastName2}";
            ShortName = $"{userData.FirstName} {userData.LastName1}";
        }

        public List<CardData> LoadClientCards()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return new List<CardData>();
            }

            var client = _httpClientFactory.CreateClient();
            var url = $"https://localhost:7289/api/Clients/cards-per-client?Id_Cliente={userId}";

            try
            {
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    return JsonSerializer.Deserialize<List<CardData>>(content);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error fetching cards.");
                    return new List<CardData>();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return new List<CardData>();
            }
        }
    }
}

