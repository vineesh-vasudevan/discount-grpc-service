using DiscountEngine.Domain.Entities;
using DiscountEngine.Domain.Services;
using DiscountEngine.Grpc.Protos;
using Grpc.Core;

namespace DiscountEngine.Grpc.Services
{
    public class DiscountGrpcService(IDiscountService discountService) : DiscountProtoService.DiscountProtoServiceBase
    {
        public async override Task<DiscountResponse> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var discount = await discountService.GetDiscount(request.Code, context.CancellationToken);
            return CreateDiscountResponse(discount);
        }

        public override async Task<DiscountResponse> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var discount = new Discount
            {
                Code = request.Code,
                ProductCode = request.ProductCode,
                Amount = request.Amount,
                Description = request.Description
            };

            var result = await discountService.CreateAsync(discount, context.CancellationToken);
            return CreateDiscountResponse(result);
        }

        public override async Task<DiscountResponse> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var discount = new Discount
            {
                Id = request.Id,
                Code = request.Code,
                ProductCode = request.ProductCode,
                Amount = request.Amount,
                Description = request.Description
            };

            var result = await discountService.UpdateAsync(discount, context.CancellationToken);
            return CreateDiscountResponse(result);
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var result = await discountService.DeleteByCodeAsync(request.Code, context.CancellationToken);
            return new DeleteDiscountResponse
            {
                Success = result
            };
        }

        private static DiscountResponse CreateDiscountResponse(Discount discount)
        {
            return new DiscountResponse
            {
                Id = discount.Id,
                Code = discount.Code,
                ProductCode = discount.ProductCode,
                Amount = discount.Amount,
                Description = discount.Description
            };
        }
    }
}
