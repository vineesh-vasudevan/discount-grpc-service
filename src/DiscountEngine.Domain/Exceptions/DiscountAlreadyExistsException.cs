namespace DiscountEngine.Domain.Exceptions
{
    public class DiscountAlreadyExistsException(string code)
        : ConflictException($"A discount with code '{code}' already exists.")
    {
    }
}
