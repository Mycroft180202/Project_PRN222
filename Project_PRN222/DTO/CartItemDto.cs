namespace Project_PRN222.DTO
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int? UserId { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}
