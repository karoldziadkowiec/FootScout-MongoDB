using FootScout_MongoDB.WebAPI.DbManager;
using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.Constants;
using FootScout_MongoDB.WebAPI.Repositories.Classes;
using FootScout_MongoDB.WebAPI.Repositories.Interfaces;
using FootScout_MongoDB.WebAPI.Services.Classes;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace FootScout_MongoDB.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;

            // Database connection
            builder.Services.Configure<MongoDBSettings>(
                configuration.GetSection(nameof(MongoDBSettings)));
            builder.Services.AddSingleton<MongoDBContext>();

            // Default authentication scheme
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // JWT Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                };
            });

            // Authorization policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminRights", policy =>
                    policy.RequireRole(RoleName.Admin));
                options.AddPolicy("UserRights", policy =>
                    policy.RequireRole(RoleName.User));
                options.AddPolicy("AdminOrUserRights", policy =>
                    policy.RequireRole(RoleName.Admin, RoleName.User));
            });

            // Services
            builder.Services.AddScoped<INewIdGeneratorService, NewIdGeneratorService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IPasswordService, PasswordService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICookieService, CookieService>();

            // Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IClubHistoryRepository, ClubHistoryRepository>();
            builder.Services.AddScoped<IAchievementsRepository, AchievementsRepository>();
            builder.Services.AddScoped<IPlayerPositionRepository, PlayerPositionRepository>();
            builder.Services.AddScoped<IPlayerFootRepository, PlayerFootRepository>();
            builder.Services.AddScoped<IOfferStatusRepository, OfferStatusRepository>();
            builder.Services.AddScoped<ISalaryRangeRepository, SalaryRangeRepository>();
            builder.Services.AddScoped<IClubAdvertisementRepository, ClubAdvertisementRepository>();
            builder.Services.AddScoped<IClubOfferRepository, ClubOfferRepository>();


            // AutoMapper service
            builder.Services.AddAutoMapper(typeof(Program));

            // Password hasher
            builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

            // Accessing HttpContext property (cookies)
            builder.Services.AddHttpContextAccessor();

            // Real time chat (SignalR)
            builder.Services.AddSignalR();

            // Controller handler
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Swagger authentication
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FootScout API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            // CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactDevClient",
                    b =>
                    {
                        b.WithOrigins("http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowed(origin => true)
                            .AllowCredentials();
                    });
            });

            var app = builder.Build();

            // HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Using CORS policy
            app.UseCors("AllowReactDevClient");
            app.UseHttpsRedirection();

            // Auth middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Endpoints
            app.MapControllers();
            //app.MapHub<ChatHub>("/chathub");

            // Seeders
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                AppSeeder.Seed(services);
            }

            app.Run();
        }
    }
}