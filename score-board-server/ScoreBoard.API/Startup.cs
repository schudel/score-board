using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ScoreBoard.API.Hubs;
using ScoreBoard.API.Models;
using ScoreBoard.Services.Helpers;
using Serilog;

namespace ScoreBoard.API
{
    public class Startup
    {
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            this.env = env;
            // Init Serilog configuration
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();

            services.AddCors();
            //services.AddControllers().AddNewtonsoftJson(options =>
            //{
            //    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            //});
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            // configure strongly typed settings objects
            IConfigurationSection appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure SmtpSettings
            services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));

            // configure jwt authentication
            AppSettings appSettings = appSettingsSection.Get<AppSettings>();
            byte[] key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                    .AddJwtBearer(options =>
                    {
                        // TODO: set RequireHttpsMetadata = true in production via config/environment
                        options.RequireHttpsMetadata = !env.IsDevelopment();
                        options.SaveToken = true;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateIssuer = true,
                            ValidIssuer = appSettings.Issuer,
                            ValidateAudience = true,
                            ValidAudience = appSettings.Audience
                        };
                    });

            // configure DI for application services
            DependencyInjectionManager.RegisterBindings(services);

            // RegisterBindings the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", 
                             new OpenApiInfo
                             {
                                 Title = "Scoreboard API",
                                 Version = "v1",
                                 Description = "Scoreboard ASP.NET Core Web API",
                                 // TODO: Add maintainer contact information
                                 Contact = new OpenApiContact
                                 {
                                     Email = "{maintainer-email}",
                                     Name = "{maintainer-name}",
                                     Url = new Uri("https://{maintainer-url}")
                                 } });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field like \"bearer {jwt_token}\". The bearer you can generate with \"authenticate\" of the playerDto controller",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new[] { "readAccess", "writeAccess" }
                    }
                });
                try
                {
                    //Locate the XML file being generated by ASP.NET...
                    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                    string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    //... and tell Swagger to use those XML comments.
                    c.IncludeXmlComments(xmlPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });

            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins("http://localhost:4200", "https://score-board.{your-domain}");
            }));
            // Add SignalR for chat
            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = false;
                //hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
            }).AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat",
                                           options =>
                                           {
                                               options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
                                           });
                endpoints.MapHub<LiveHub>("/live",
                                          options =>
                                          {
                                              options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
                                          });

            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Scoreboard API V1");
                c.DocumentTitle = "Scoreboard API";
                c.DisplayRequestDuration();
                c.EnableFilter();
                c.ShowExtensions();
                c.EnableValidator();
            });
        }
    }
}
