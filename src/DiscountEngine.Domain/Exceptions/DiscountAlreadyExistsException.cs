using DiscountEngine.Domain.Exceptions;

namespace DiscountService.Domain.Exceptions
{
    public class DiscountAlreadyExistsException : ConflictException
    {
        public DiscountAlreadyExistsException(string code)
            : base($"A discount with code '{code}' already exists.") { }
    }
}
