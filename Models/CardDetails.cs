namespace WebAppPruebaTecnica.Models
{
    public class CardDetails
    {
        public int id_Tarjeta {  get; set; }
        public decimal saldoActual { get; set; }
        public decimal saldoDisponible { get; set; }
        public decimal limiteCredito { get; set; }
        public string numeroTarjeta { get; set; }
    }
}
