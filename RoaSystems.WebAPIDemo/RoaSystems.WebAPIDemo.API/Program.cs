using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RoaSystems.Libraries.Repositories;
using RoaSystems.Libraries.Services;
using RoaSystems.Libraries.Services.Middleware;
using RoaSystems.Libraries.Model;
using RoaSystems.Libraries.Model.Security;
using System.Text;

namespace RoaSystems.WebAPIDemo.API
{
    public class Program
    {
        public class Environment
        {
            public string Name { get; set; }
            public string ConnectionString { get; set; }
        }
        private string GetConnectionStringForEnvironment(IConfiguration configuration, string environmentMode)
        {
            var environments = configuration.GetSection("Environments").Get<List<Environment>>();
            var environment = environments?.FirstOrDefault(x => x.Name == environmentMode);

            if (environment == null || string.IsNullOrEmpty(environment.ConnectionString))
            {
                throw new Exception($"Connection string for environment '{environmentMode}' not found.");
            }

            return configuration.GetConnectionString(environment.ConnectionString);
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var jwtKey = builder.Configuration["Jwt:SecretKey"];
            var issuer = builder.Configuration["Jwt:Issuer"];

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Configure MySQL and get the connection string for the environment based on the environment mode
            var environmentMode = builder.Configuration["Environment"] ?? "Development";
            var program = new Program();
            var connStr = program.GetConnectionStringForEnvironment(builder.Configuration, environmentMode);
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connStr,
                   new MySqlServerVersion(new Version(5, 7, 38))), ServiceLifetime.Scoped);
            // Registering AuthenticationDbContext
            builder.Services.AddDbContext<AuthenticationDbContext>(options => options.UseMySql(connStr,
                new MySqlServerVersion(new Version(5, 7, 38))));
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthenticationDbContext>()
                .AddDefaultTokenProviders();


            // JWT setup
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<TokenService>(); // If directly used by other services
            // Register UserTokenService
            builder.Services.AddScoped<IUserTokenService, UserTokenService>();
            builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
            builder.Services.AddScoped<IResourceTypeRepository, ResourceTypeRepository>();
            builder.Services.AddScoped<IUserTokenRepository, UserTokenRepository>();
            builder.Services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
            builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            builder.Services.AddScoped<IUserClaimRepository, UserClaimRepository>();
            builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            builder.Services.AddScoped<IUserTokenRepository, UserTokenRepository>();
            builder.Services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
            builder.Services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
            builder.Services.AddScoped<ISubscriptionTypeRepository, SubscriptionTypeRepository>();
            builder.Services.AddScoped<ISubscriptionBenefitTypeRepository, SubscriptionBenefitTypeRepository>();
            builder.Services.AddScoped<ISubscriptionTypeBenefitRepository, SubscriptionTypeBenefitRepository>();
            builder.Services.AddScoped<IUserLoginRepository, UserLoginRepository>();
            // Services
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRoleService, UserRoleService>();
            builder.Services.AddScoped<ILanguagesService, LanguagesService>();
            builder.Services.AddScoped<IApplicationService, ApplicationService>();
            builder.Services.AddScoped<IResourceTypeService, ResourceTypeService>();
            builder.Services.AddScoped<IUserPermissionService, UserPermissionService>();
            builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
            builder.Services.AddScoped<IUserClaimService, UserClaimService>();
            builder.Services.AddScoped<IUserTokenService, UserTokenService>();
            builder.Services.AddScoped<IUserProfileService, UserProfileService>();
            builder.Services.AddScoped<IUserLoginService, UserLoginService>();
            builder.Services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
            builder.Services.AddScoped<IVerificationCodeService, VerificationCodeService>();
            builder.Services.AddScoped<ISecurityService, SecurityService>();
            builder.Services.AddScoped<ISubscriptionTypeService, SubscriptionTypeService>();
            builder.Services.AddScoped<ISubscriptionBenefitTypeService, SubscriptionBenefitTypeService>();
            builder.Services.AddScoped<ISubscriptionTypeBenefitService, SubscriptionTypeBenefitService>();
            builder.Services.AddScoped<IErrorLogsService, ErrorLogsService>();
            // Emailer and SMS Sender Services...
            builder.Services.AddScoped<IEmailerService, EmailerService>();
            builder.Services.AddHttpClient<ISMSSenderService, SMSSenderService>();
            // Register authentication with JWT Bearer
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        // ValidAudience = "tuAudience",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"Token inválido: {context.Exception.Message}");
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            // Evita redirecciones, fuerza 401
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsync("{\"error\": \"Unauthorized\"}");
                        }
                    };
                });

            // Swagger configuration...
            builder.Services.AddSwaggerGen(c =>
            {
                // Add JWT Bearer token support to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });

                // Other configurations for Swagger (Optional)
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Roa Systems API", Version = "v1" });
            });



            var app = builder.Build();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
            // Specify the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Roa Systems API v1");
                c.RoutePrefix = string.Empty; // Make Swagger UI available at the root
            });

            // Register error handling middleware
            app.UseMiddleware<ErrorHandlingMiddleware>();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }


            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("AllowAllOrigins");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
