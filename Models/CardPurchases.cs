namespace WebAppPruebaTecnica.Models
{
    public class CardPurchases
    {
        public int id_Compra { get; set; }
        public int id_Tarjeta { get; set; }
        public decimal montoCompra { get; set; }
        public string descripcionCompra { get; set; }
        public DateTime fechaCompra { get; set; }
    }

    public class CardPayment
    {
        public int id_Tarjeta { get; set; }
        public decimal montoPago { get; set; }
        public DateTime fechaPago { get; set; }

    }

    public class TransactionHistory
    {
        public int id { get; set; }
        public decimal monto { get; set; }
        public DateTime fecha { get; set; }
        public string tipo { get; set; }
    }
}
