namespace Car_Rental_System.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId {get; set;} = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal PricePerDay { get; set; }
        public string LicensePlates { get; set; } = string.Empty;
    }
}
