namespace IpoApp.Models.Entities
{
    public class Ipo
    {
        public int Id { get; set; }
        public string? CompanyName { get; set; }
        public decimal Price { get; set; }
        public DateTime OpenDate { get; set; }
        public string TenantId { get; set; }
    }
}