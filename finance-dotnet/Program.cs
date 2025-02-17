using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Minio;
using finance_dotnet.Backend.Repositories;
using System;
using Minio.DataModel.Tags;
using System.Reflection.PortableExecutable;
using finance_dotnet.Backend.Auths;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using finance_dotnet.Backend.Kakfa;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    // Look for static files in webroot
    //WebRootPath = "webroot"
});

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
var otlpLogEndpoint = builder.Configuration.GetValue<string>("OpenTelemetry:ExporterOtlpLogEndpoint");
builder.Logging.ClearProviders().AddConsole().AddOpenTelemetry(options =>
{
    options.IncludeScopes = true;
    options.IncludeFormattedMessage = true;
    //options.ParseStateValues = true;
    options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName: "dotnet-app").AddAttributes(new Dictionary<string, object>
    {
        ["environment.name"] = "dev",
    }));
    //options.AddConsoleExporter();
    options.AddOtlpExporter(otlpOptions =>
    {
        //otlpOptions.BatchExportProcessorOptions.ScheduledDelayMilliseconds = 2000;
        //otlpOptions.BatchExportProcessorOptions.MaxExportBatchSize = 5000;
        otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
        otlpOptions.Endpoint = new Uri(otlpLogEndpoint!);
    });
});

builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

var minioSection = builder.Configuration.GetSection("Minio");
builder.Services.AddSingleton(new MinioClient()
    .WithEndpoint(builder.Configuration.GetConnectionString("Minio"))
    .WithCredentials(minioSection["AccessKey"], minioSection["SecretKey"])
    .Build()
);

//builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true; // Enforce lowercase URLs
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "finance dotnet API", Version = "v1" });
});

builder.Services.AddHttpClient();


builder.Services.AddScoped(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("Postgres");
    return new NpgsqlConnection(connectionString);
});


// repo
builder.Services.AddScoped<IMinioRepository, MinioRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// service
//builder.Services.AddScoped<IOrderService, OrderService>();


//builder.Services.AddOpenIddict().AddCore(options =>
//{
//    options.SetDefaultApplicationEntity<UserRepository>();
//}).AddServer(options =>
//{
//    // AuthorizationEndpointUri = Authorization Endpoint when login for Authorization Code
//    // TokenEndpointUri = Exchange Authorization Code to get JWT Access Token
//    options.SetAuthorizationEndpointUris("/connect/authorize")
//        .SetTokenEndpointUris("/connect/token");

//    // enable Authorization Code Flow for third party login
//    // RequireProofKeyForCodeExchange = Proof Key for Code Exchange (PKCE) to prevent Code Injection
//    options.AllowAuthorizationCodeFlow()
//               .RequireProofKeyForCodeExchange();

//    //OAuth Scopes = user email and user profile
//    options.RegisterScopes("email", "profile");

//    // UseAspNetCore = OpenIddict handle HTTP request
//    // EnableAuthorizationEndpointPassthrough = http requests to Controller AuthorizationEndpointUri
//    // EnableTokenEndpointPassthrough = http requests to Controller TokenEndpointUri
//    options.UseAspNetCore()
//               .EnableAuthorizationEndpointPassthrough()
//               .EnableTokenEndpointPassthrough();

//}).AddValidation(options =>
//{
//    // Self-Validation
//    options.UseLocalServer();
//    // enable to read HTTP Authorization Header Bearer Token
//    options.UseAspNetCore();
//});


//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;
//})
//.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
//.AddOAuth("GitHub", options =>
//{
//    options.ClientId = "你的 GitHub Client ID";
//    options.ClientSecret = "你的 GitHub Client Secret";
//    options.CallbackPath = new PathString("/signin-github");
//    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
//    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
//    options.UserInformationEndpoint = "https://api.github.com/user";
//    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
//    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");

//    options.SaveTokens = true;
//});


//builder.Services.AddScoped<IAuthorizationHandler, ABACAuthorizationHandler>();
//builder.Services.AddAuthorizationBuilder()
//    .AddPolicy("SearchCatalog", policy => policy.Requirements.Add(new ABACRequirement("Search", "Catalog")))
//    .AddPolicy("AddToCart", policy => policy.Requirements.Add(new ABACRequirement("Add", "Cart")))
//    .AddPolicy("PlaceOrder", policy => policy.Requirements.Add(new ABACRequirement("PlaceOrder", "Order")))
//    .AddPolicy("ManageUsers", policy => policy.Requirements.Add(new ABACRequirement("ManageUsers", "Admin")));

// Add services to the container.
var app = builder.Build();


// init db

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true,
    DefaultContentType = "application/octet-stream",
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Frontend")),
    RequestPath = "/frontend"
});

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();

// redirect
app.MapGet("/", context =>
{
    context.Response.Redirect("frontend/index.html", permanent: true);
    return Task.CompletedTask;
});

app.Run();
