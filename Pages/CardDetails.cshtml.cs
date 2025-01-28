using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Security.Claims;
using WebAppPruebaTecnica.Service;
using System.Text.Json;
using WebAppPruebaTecnica.Models;
using System.Text;

namespace WebAppPruebaTecnica.Pages
{
    public class CardDetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly UserService _userService; // referencia al servicio para acceder a data del usuario global
        [BindProperty]
        public int Id_Tarjeta { get; set; }
        public CardDetails CardDetail { get; set; }
        public CardMonthlyPurchases CardMonthlyPurchases { get; set; }
        public List<CardPurchases> CardPurchases { get; set; }
        public List<TransactionHistory> Transactions { get; set; } = new();


        public CardDetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        public IActionResult OnGet(string id)
        {

            CardDetail = LoadCardDetails(id);
            CardPurchases = LoadMonthPurchases(id);
            CardMonthlyPurchases = LoadMonthlyPurchases(id);
            Transactions = LoadTransactionHistory(id);
            return CardDetail != null ? Page() : NotFound();
            
        }


        public CardDetails LoadCardDetails(string cardId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return null;

            var client = _httpClientFactory.CreateClient();
            var url = $"https://localhost:7289/api/Clients/details-per-card?Id_Tarjeta={cardId}";

            try
            {
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var cardDetails = JsonSerializer.Deserialize<List<CardDetails>>(content);
                    return cardDetails?.FirstOrDefault();
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public List<CardPurchases> LoadMonthPurchases(string cardId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return new List<CardPurchases>();
            }

            var client = _httpClientFactory.CreateClient();
            var url = $"https://localhost:7289/api/Clients/purchases-per-card?Id_Tarjeta={cardId}";

            try
            {
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    return JsonSerializer.Deserialize<List<CardPurchases>>(content);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error fetching cards.");
                    return new List<CardPurchases>();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return new List<CardPurchases>();
            }
        }


        public CardMonthlyPurchases LoadMonthlyPurchases(string cardId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return null;

            var client = _httpClientFactory.CreateClient();
            var url = $"https://localhost:7289/api/Clients/monthly-purchases-per-card?Id_Tarjeta={cardId}";

            try
            {
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().Result;
                    var cardMonthlyPurchases = JsonSerializer.Deserialize<CardMonthlyPurchases>(content);
                    return cardMonthlyPurchases;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<IActionResult> OnPostProcessPaymentAsync()
        {
        
            var idTarjeta = Request.Form["idTarjeta"];
            var montoPagar = Request.Form["montoPagar"];
            var fechaPago = Request.Form["fechaPago"];

            // Validate and parse data
            if (!int.TryParse(idTarjeta, out var tarjetaId) ||
                !decimal.TryParse(montoPagar, out var monto) ||
                !DateTime.TryParse(fechaPago, out var fecha))
            {
                ModelState.AddModelError(string.Empty, "Invalid input data.");
                return Page();
            }

          
            var payment = new CardPayment
            {
                id_Tarjeta = tarjetaId,
                montoPago = monto,
                fechaPago = fecha
            };

           
            using var client = new HttpClient();
            var url = "https://localhost:7289/api/Payment/card-process-payment";

            try
            {
                var jsonContent = JsonSerializer.Serialize(payment);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Payment processed successfully.";
                    return RedirectToPage("/Index"); 
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Payment failed: {errorDetails}";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return Page();
            }
        }


        public async Task<IActionResult> OnPostProcessPurchaseAsync()
        {

            var idTarjeta = Request.Form["idTarjeta"];
            var montoCompra = Request.Form["montoComprar"];
            var fechaCompra = Request.Form["fechaCompra"];
            var descripcionCompra = Request.Form["descripcionCompra"];

            if (!int.TryParse(idTarjeta, out var tarjetaId) ||
            !decimal.TryParse(montoCompra, out var monto) ||
            !DateTime.TryParse(fechaCompra, out var fecha) ||
            string.IsNullOrWhiteSpace(descripcionCompra))
            {
                TempData["ErrorMessage"] = "Invalid input data. Please check your entries.";
                return Page();
            }

            var payment = new CardPurchases
            {
                id_Tarjeta = tarjetaId,
                montoCompra = monto,
                fechaCompra = fecha,
                descripcionCompra = descripcionCompra // Assign descripcionCompra
            };


            using var client = new HttpClient();
            var url = "https://localhost:7289/api/Payment/card-process-purchase-payment";

            try
            {
                var jsonContent = JsonSerializer.Serialize(payment);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Purchase processed successfully.";
                    return RedirectToPage("/Index");
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Purchase failed: {errorDetails}";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return Page();
            }
        }


        public List<TransactionHistory> LoadTransactionHistory(string cardId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return new List<TransactionHistory>();
            }

            var client = _httpClientFactory.CreateClient();
            var url = $"https://localhost:7289/api/Payment/card-transaction-history?Id_Tarjeta={cardId}";

            try
            {
                var response = client.GetAsync(url).GetAwaiter().GetResult();  // Use .GetResult() to block synchronously

                if (response.IsSuccessStatusCode)
                {
                    var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();  // Synchronously read the content
                    return JsonSerializer.Deserialize<List<TransactionHistory>>(content);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error fetching transaction history.");
                    return new List<TransactionHistory>();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return new List<TransactionHistory>();
            }
        }
    }

    
}
