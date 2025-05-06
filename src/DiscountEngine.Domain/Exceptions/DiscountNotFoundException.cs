namespace DiscountEngine.Domain.Exceptions
{
    public class DiscountNotFoundException : NotFoundException
    {
        public DiscountNotFoundException(string code)
            : base($"Discount with code '{code}' was not found.") { }

        public DiscountNotFoundException(int id)
            : base($"Discount with ID '{id}' was not found.") { }
    }
}
