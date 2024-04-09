using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
//using HotelProject.WEBAPI.Db_Access;
using System.Reflection;
using NSwag.Generation.AspNetCore;
using HotelProject.Services;
using Swashbuckle.AspNetCore.Swagger;
using System.Linq;
using NJsonSchema.Generation;
using Microsoft.AspNetCore.Builder;
using HotelProject.DAL.Db_Access;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Startup
{

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Startup(IConfiguration configuration)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        this.Configuration = configuration;
    }

    public IConfiguration Configuration { get; private set; }

    public ILifetimeScope AutofacContainer { get; private set; }

    // ConfigureServices is where you register dependencies. This gets
    // called by the runtime before the ConfigureContainer method, below.
#pragma warning disable CS8601 // Possible null reference assignment.
    static readonly Version version = typeof(Startup).Assembly.GetName().Version;
#pragma warning restore CS8601 // Possible null reference assignment.
    public void ConfigureServices(IServiceCollection services)
    {

        services.AddCors(options =>
        {
            options.AddPolicy("AllowOrigin",
                builder => builder
                    .WithOrigins("http://localhost:4200") // Specify your Angular app's URL
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });


        services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(
        Configuration.GetConnectionString("HotelConnection")));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        o.SaveToken = true;
    });
        
        services.AddMvc();

        services.AddControllers();


        services.AddOpenApiDocument(document => document.DocumentName = "a");

        const string AdminNameSpace = "HotelProject.WEBAPI.Controllers";

        services.AddSwaggerDocument(document =>
        {
            document.DocumentName = "webapi_swagger";
            document.Title = "webapi API";
            document.Description = "Interfejs webapi";
            ApplyDefaults(document);
            document.AddOperationFilter(opc => opc.ControllerType.Namespace == AdminNameSpace);
            //document.GenerateEnumMappingDescription = true;
        });

        services.AddOpenApiDocument(document =>
        {
            document.DocumentName = "webapi_openapi";
            document.Title = "webapi API";
            document.Description = "Interfejs webapi";
            ApplyDefaults(document);
            document.AddOperationFilter(opc => opc.ControllerType.Namespace == AdminNameSpace);
        });


        services.AddOptions();

        //services.AddCors(options =>
        //{
        //    options.AddPolicy("AllowOrigin",
        //        builder =>
        //        {
        //            builder.WithOrigins("http://localhost",
        //                                "http://localhost:4200",
        //                                "http://localhost:7195")
        //                .AllowAnyMethod()
        //                .AllowAnyHeader()
        //                .SetIsOriginAllowedToAllowWildcardSubdomains();
        //        });
        //});
        

    }
    private static void ApplyDefaults(AspNetCoreOpenApiDocumentGeneratorSettings document)
    {

        document.Version = version.ToString();
        
        document.GenerateEnumMappingDescription = true; //GenerateEnumMappingDescription
    }

    // ConfigureContainer is where you can register things directly
    // with Autofac. This runs after ConfigureServices so the things
    // here will override registrations made in ConfigureServices.
    // Don't build the container; that gets done for you by the factory.
    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterModule(new ServiceModule());
      
    }


    public void Configure(
      IApplicationBuilder app,
      ILoggerFactory loggerFactory)
    {

        this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();
        app.UseCors("AllowOrigin");

        app.UseOpenApi();
        app.UseSwaggerUi3();
        
        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });


    }
}
