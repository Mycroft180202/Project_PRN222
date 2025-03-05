namespace Project_PRN222.Models
{
    public class Vendor
    {
        public int VendorId { get; set; }

        public string VendorName { get; set; } = null!; 

        public string? ContactEmail { get; set; } 

        public string? ContactPhone { get; set; } 

        public string? Address { get; set; } 

        public DateTime? CreatedDate { get; set; } 

        public bool IsActive { get; set; } = true; 

        public int UserId { get; set; } 

        public virtual User User { get; set; } 

        public virtual ICollection<Product> Products { get; set; } = new List<Product>(); 
    }
}