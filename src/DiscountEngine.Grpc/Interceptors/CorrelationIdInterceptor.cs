using Grpc.Core.Interceptors;
using Grpc.Core;

namespace DiscountEngine.Grpc.Interceptors
{
    public class CorrelationIdInterceptor(ILogger<CorrelationIdInterceptor> logger) : Interceptor
    {
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            var correlationId = context.RequestHeaders
                .FirstOrDefault(h => h.Key == "x-correlation-id")?.Value;

            if (string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
                logger.LogInformation("No Correlation ID was provided. Generating a new one: {CorrelationId}", correlationId);
            }
            var responseMetadata = new Metadata
            {
                { "x-correlation-id", correlationId }
            };

            await context.WriteResponseHeadersAsync(responseMetadata);
            using (logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId
            }))
            {
                return await continuation(request, context);
            }
        }
    }
}
