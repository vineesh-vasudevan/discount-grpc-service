namespace DiscountEngine.Domain.Entities
{
    public class Discount
    {
        public int Id { get; set; }
        public string Code { get; set; } = default!;
        public string ProductCode { get; set; } = default!;
        public double Amount { get; set; }
        public string Description { get; set; } = default!;

    }
}
