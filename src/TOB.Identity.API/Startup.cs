using System.Reflection;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using TOB.Identity.Services;
using TOB.Identity.Infrastructure.Mapping;
using TOB.Identity.Infrastructure.Repositories;
using TOB.Identity.Infrastructure.Repositories.Implementations;
using TOB.Identity.Infrastructure.Validation;
using TOB.Identity.Services.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using TOB.Identity.Infrastructure.Data;
using TOB.Identity.API.Extensions;
using TOB.Identity.Domain.AppSettings;

namespace TOB.Identity.API;

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
        services.AddMvc();
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<CreateTenantRequestValidator>(includeInternalTypes: true);

        var identityContextConnString = Configuration.GetConnectionString("IdentityDBContext");

        services.AddDbContextFactory<IdentityDBContext>(options =>
            options.UseSqlServer(identityContextConnString));

        services.AddAutoMapper(
            Assembly.GetAssembly(typeof(IdentityMappingProfile)));

        services.ConfigureGraphClient(Configuration);

        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRolePermissionsRepository, RolePermissionsRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();

        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserRoleService, UserRoleService>();

        services.AddMemoryCache();

        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });

            c.SwaggerDoc("v1", new OpenApiInfo { Title = "TOB Consulting Identity API", Version = "v1" });
        });

        services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.Configure<AzureAd>(Configuration.GetSection(nameof(AzureAd)));
        services.AddSingleton<AzureAd>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddMicrosoftIdentityWebApi(
                   options =>
                   {
                       Configuration.Bind("AzureAd", options);

                       options.TokenValidationParameters.NameClaimType = "name";
                   },
                   options => { Configuration.Bind("AzureAd", options); });

        services.AddAuthorization();

        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

        var origins = Configuration["AllowedOrigins"].Split(";");

        services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
        {
            builder.WithOrigins(origins)
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials();
        }));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseCors("CorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseSwagger();

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("../swagger/v1/swagger.json", "TOB Consulting Identity API V1");
        });

    }
}
