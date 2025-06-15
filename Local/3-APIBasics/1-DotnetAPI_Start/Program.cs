// Learn more about web apis here: https://learn.microsoft.com/en-us/aspnet/web-api/

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors((options) => // https://learn.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-9.0
    {
        options.AddPolicy("DevCors", (corsBuilder) =>
            {
                corsBuilder.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8000")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        options.AddPolicy("ProdCors", (corsBuilder) =>
            {
                corsBuilder.WithOrigins("https://myProductionSite.com")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
    });

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
// learn more about swashbucle (swagger): https://github.com/domaindrivendev/Swashbuckle.AspNetCore/tree/master?tab=readme-ov-file#swashbuckleaspnetcore
builder.Services.AddSwaggerGen(swaggerGenOptions =>
{
    swaggerGenOptions.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth Sample", Version = "v1" });
    swaggerGenOptions.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
            },
            Array.Empty<string>()
        }
    });
});
// Test how to add a logger service
// Research the meaning of System.InvalidOperationException: Unable to resolve service for type 'Microsoft.Extensions.Logging.ILogger' while attempting to activate 'DotnetAPI.Controllers.WeatherForecastController'.
// see also: https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line#create-an-iloggerfactory-with-di
builder.Services.AddLogging(builder =>
{
    builder.AddDebug();
    builder.AddConsole();
});

// This service is used for httpLogging
builder.Services.AddHttpLogging(config =>
{
    config.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestPath
        | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestMethod
        | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponseStatusCode
        | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.Duration;
});

// builder.Services.AddW3CLogging(options =>
// {
//     options.LoggingFields = Microsoft.AspNetCore.HttpLogging.W3CLoggingFields.All;
// });

///////////// To generate a signing key for JWT ///////////////
// 1. Get a random string (this is from the config)
string? tokenKeyString = builder.Configuration.GetSection("AppSettings:TokenKey").Value;

// 2. Generate a Symetric security key, using the random string as parameter.
// The random string must be converted to byte array. see: https://learn.microsoft.com/en-us/dotnet/api/microsoft.identitymodel.tokens.symmetricsecuritykey.-ctor?view=msal-web-dotnet-latest#microsoft-identitymodel-tokens-symmetricsecuritykey-ctor(system-byte())
// JWT authentication can use symetric security keys
SymmetricSecurityKey tokenKey = new(
        // converts the string to a byte array
        Encoding.UTF8.GetBytes(
            tokenKeyString ?? ""
        )
    );

// read more about authentication here: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-9.0
// using bearer tokens: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-jwt-bearer-authentication?view=aspnetcore-9.0#implementing-jwt-bearer-token-authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
// see: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.jwtbearerextensions.addjwtbearer?view=aspnetcore-9.0
    .AddJwtBearer(options =>
    {
        // see: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.jwtbearer.jwtbeareroptions?view=aspnetcore-9.0
        options.TokenValidationParameters = new() // OR options.TokenValidationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey = tokenKey,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseCors("ProdCors");
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// Enable http logging middleware after adding the service
app.UseHttpLogging();

// Enable logging for all requests using W3C format
// app.UseW3CLogging();

app.Run();

