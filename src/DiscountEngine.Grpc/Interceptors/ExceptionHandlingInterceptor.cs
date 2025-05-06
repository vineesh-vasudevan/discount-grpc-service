using DiscountEngine.Domain.Exceptions;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace DiscountEngine.Grpc.Interceptors
{
    public class ExceptionHandlingInterceptor(ILogger<ExceptionHandlingInterceptor> logger) : Interceptor
    {
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (NotFoundException ex)
            {
                logger.LogWarning(ex, "Resource not found");
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (ConflictException ex)
            {
                logger.LogWarning(ex, "Resource already present");
                throw new RpcException(new Status(StatusCode.AlreadyExists, ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                logger.LogWarning(ex, "Invalid operation");
                throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception");
                throw new RpcException(new Status(StatusCode.Internal, "An unexpected error occurred."));
            }
        }
    }
}
