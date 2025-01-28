
namespace WebAppPruebaTecnica.Models
{
    public class CardMonthlyPurchases
    {
        public decimal totalMesActual {  get; set; }
        public decimal totalMesAnterior { get; set; }

        internal CardMonthlyPurchases FirstOrDefault()
        {
            throw new NotImplementedException();
        }
    }
}
