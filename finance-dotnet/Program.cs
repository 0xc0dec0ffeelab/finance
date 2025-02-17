using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    // Look for static files in webroot
    //WebRootPath = "webroot"
});

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

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

//builder.Services.AddScoped(sp =>
//{
//    var configuration = sp.GetRequiredService<IConfiguration>();
//    var connectionString = configuration.GetConnectionString("PgCluster");
//    return new NpgsqlConnection(connectionString);
//});


// Add services to the container.
var app = builder.Build();

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
