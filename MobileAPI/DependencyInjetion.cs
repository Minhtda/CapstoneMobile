using Application.InterfaceService;
using Application.Service;
using Application.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MobileAPI.MobileSevice;
using System.Text;

namespace MobileAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMobileAPIService(this IServiceCollection services, string secretKey)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IClaimService, ClaimService>();
            services.AddScoped<ICurrentUserIp, CurrentUserIp>();
            services.AddScoped<ICurrentTime, CurrentTime>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ISendMailHelper, SendMailHelper>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUploadFile, UploadFile>();
            services.AddScoped<IExchangeConditionService, ExchangeConditionService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISubcriptionService, SubcriptionService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IVerifyUserService, VerifyUserService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<ISubscriptionHistoryService, SubscriptionHistoryService>();
            services.AddScoped<IWalletTransactionService, WalletTransactionService>();
            services.AddMemoryCache();
            services.AddScoped<IPolicyService, PolicyService>();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(12);
                options.Cookie.Name = "GoodExchange";
                options.Cookie.IsEssential = true;
            });
            /*var options = ConfigurationOptions.Parse(cacheConnectionString); // host1:port1, host2:port2, ...
            options.Password = "MinhQuan@123";
            services.AddSingleton<IDatabase>(cfg =>
            {
                IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(options);
                return multiplexer.GetDatabase();
            });*/
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = secretKey,
                      ValidAudience = secretKey,
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                      ClockSkew = TimeSpan.FromSeconds(1)
                  };
              });

            return services;
        }
    }
}
