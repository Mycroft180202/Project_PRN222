namespace Project_PRN222.DTO
{
    public class CheckoutRequest
    {
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public int ShipmentMethodId { get; set; }
        public string PaymentMethod { get; set; }
    }
}
