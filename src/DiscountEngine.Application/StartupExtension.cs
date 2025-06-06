﻿using DiscountEngine.Application.Services;
using DiscountEngine.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DiscountEngine.Application
{
    public static class StartupExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IDiscountService, DiscountServiceManager>();
            return services;
        }
    }
}
