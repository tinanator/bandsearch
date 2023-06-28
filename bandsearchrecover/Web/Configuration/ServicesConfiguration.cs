using System.Text;
using BandSearch.Database;
using BandSearch.Models;
using BandSearch.Web.Database;
using BandSearch.Web.DTOs;
using BandSearch.Web.Mappers;
using BandSearch.Web.Models;
using BandSearch.Web.Services;
using BandSearch.Web.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;

namespace BandSearch.Web.Configuration
{
    public static class ServicesConfiguration
    {
        public static void ConfigureUserServices(WebApplicationBuilder builder) 
        {
            builder.Services.AddScoped<IValidator<UpdateUserDTO>, UserValidator>();
            builder.Services.AddScoped<Repository<User>>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<UserDetailsMapper>();
            builder.Services.AddScoped<UserMapper>();
            builder.Services.AddScoped<IUserService, UserService>();
        }
        public static void ConfigureBandServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IValidator<string>, BandNameValidator>();
            builder.Services.AddScoped<IRepository<Band>, Repository<Band>>();
            builder.Services.AddScoped<BandMapper>();
            builder.Services.AddScoped<BandOpenPositionMapper>();
            builder.Services.AddScoped<IBandRepository, BandRepository>();
            builder.Services.AddScoped<IBandService, BandService>();
        }

        public static void ConfigureInstrumentLevelServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IRepository<InstrumentLevel>, Repository<InstrumentLevel>>();
            builder.Services.AddScoped<IValidator<InstrumentLevelDTO>, InstrumentLevelValidator>();
        }

        public static void ConfigureOpenPositionServices(WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IValidator<BandOpenPositionDTO>, BandOpenPositionValidator>();
            builder.Services.AddScoped<IRepository<BandOpenPosition>, Repository<BandOpenPosition>>();
        }

        public static void ConfigureLoggingService(WebApplicationBuilder builder) 
        {
            Log.Logger = new LoggerConfiguration()
                     .WriteTo.Seq("http://localhost:5985/")
                     .CreateLogger();

            builder.Services.AddSingleton(Log.Logger);

            builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Seq("http://localhost:5985")
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithCorrelationIdHeader("my-custom-correlation-id"));
        }

        public static void ConfigureAuthServices(WebApplicationBuilder builder) 
        {
            builder.Services.AddScoped<UserRegisterDataDTOMapper>();
            builder.Services.AddScoped<IValidator<UserRegisterDataDTO>, UserRegisterDataValidation>();
            builder.Services.AddScoped<IValidator<UserCredentials>, UserCredentialsValidator>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
                };
            });
        }
    }
}
