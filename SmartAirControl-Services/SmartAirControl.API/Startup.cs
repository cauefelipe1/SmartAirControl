using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartAirControl.API.Core.Jwt;
using SmartAirControl.API.Core.Settings;
using SmartAirControl.API.Database;
using SmartAirControl.API.RepositoryInjection;
using SmartAirControl.Models.Authentication;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartAirControl.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = InjectAppSettings(services, Configuration);

            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
            services.AddControllers();
            AddAuthentication(services, appSettings);
            AddSwaggerConfuguration(services);
            AddDatabaseDependencies(services);
            services.AddRepositories(appSettings);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartAirControl.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }

        private void AddSwaggerConfuguration(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SmartAirControl.API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT authorization. \n Example: 'Bearer 12345abcdef'",
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                               Type = ReferenceType.SecurityScheme,
                               Id = "Bearer"
                             }
                        },
                        new string[] {}
                    }
                });

                // Including XML Docs
                string path = AppContext.BaseDirectory;
                string modelsXml = Path.Combine(path, "SmartAirControl.Models.xml");

                if (File.Exists(modelsXml))
                    c.IncludeXmlComments(modelsXml);

                string apiXml = Path.Combine(path, $"{ Assembly.GetEntryAssembly().GetName().Name }.xml");
                if (File.Exists(apiXml))
                    c.IncludeXmlComments(apiXml);
            });

        }

        private void AddAuthentication(IServiceCollection services, AppSettingsData appSettings)
        {
            services.AddSingleton<IJwtService, JwtIdentityModelService>();

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(jwtConfig =>
            {
                jwtConfig.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = appSettings.Jwt.Issuer,
                    ValidAudience = appSettings.Jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Jwt.Secret))
                };
                jwtConfig.Events = new JwtBearerEvents
                {
                    OnTokenValidated = jwtContext => Task.Run(() =>
                    {
                        if (jwtContext.SecurityToken is JwtSecurityToken jwtToken)
                        {
                            var props = typeof(IdentityClaimsModel).GetProperties();

                            foreach (var prop in props)
                            {
                                string val = jwtToken.Claims.FirstOrDefault(x => string.Equals(x.Type, prop.Name))?.Value;

                                if (val is not null)
                                    jwtContext.HttpContext.Items[prop.Name] = val;
                            }
                        }
                    })
                };
            });
        }

        private AppSettingsData InjectAppSettings(IServiceCollection services, IConfiguration configuration)
        {
            var settings = new AppSettingsData(configuration);
            services.AddSingleton(settings);

            return settings;
        }

        private void AddDatabaseDependencies(IServiceCollection services)
        {
            services.AddSingleton<DapperContext>();

            services.AddTransient<SqLiteDatabaseInitializer>();
            services.AddTransient<SqlServerDatabaseInitializer>();
            services.AddTransient<PostgreSqlDatabaseInitializer>();
        }
    }
}
