using System.Reflection;
using APISupport;
using CommonServices;
using DatabaseContext;

using IdentityServerAPI;

using log4net;
using log4net.Config;

using Microsoft.EntityFrameworkCore;

using SecurityUtilityTypes;

using LoggingLibrary;
using AuthenticationRepository;

using Microsoft.OpenApi.Models;

var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
Console.WriteLine("Starting");

var applicationLifeCycle = new ApplicationLifeCycle.ApplicationLifeCycle("KioskHost");
applicationLifeCycle.Initialize();

await applicationLifeCycle.StartRequestAsync();

var loggerFactory = applicationLifeCycle.Resolve<LoggingLibrary.ILoggerFactory>();
var logger = loggerFactory.Create("IdentityServer");

logger.Debug(LogClass.General, "Starting up system.");

var builder = WebApplication.CreateBuilder(args);
var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: myAllowSpecificOrigins,
            policy =>
                {
                    policy.WithOrigins("http://localhost:7050").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
                });
    });

ConfigurationManager configurationManager = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
        {
            options.OperationFilter<JwtTokenHeaderFilter>();
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Identity Server API",
                    Description = "An identity server for authenticating users and supply JWT Tokens and Refresh Tokens.  Additionally, api endpoints exist for updating/creating new users that can be authenticated."
                });

            // using System.Reflection;
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

// Dependencies
builder.Services.AddSingleton(applicationLifeCycle.Resolve<IDateTimeService>());
builder.Services.AddSingleton(applicationLifeCycle.Resolve<LoggingLibrary.ILogger>());
builder.Services.AddSingleton(_ => applicationLifeCycle.Resolve<ISecretKeyRetrieval>());
builder.Services.AddSingleton(applicationLifeCycle.Resolve<IJwtValidate>());

var connectionString = configurationManager.GetConnectionString("IdentityServer");
Console.WriteLine($"ConnectionString: {connectionString}");
builder.Services.AddDbContext<DataContext>((s, options) => options.UseNpgsql(connectionString));

// Repositories
builder.Services.AddTransient(_ => applicationLifeCycle.Resolve<IAuthenticationRepository>());

var app = builder.Build();
await app.MigrateDatabaseAsync();

//// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseRequestResponseLogging();
    app.UseSwagger();
    app.UseSwaggerUI();


}
else
{
    app.UseRequestResponseLogging();
    app.UseSwagger();
    app.UseSwaggerUI();


    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(origin => true).AllowCredentials());
app.UseValidateAPICall();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
