using Itishnik.Infrastructure.Data;
using Itishnik.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment() && !Environment.CurrentDirectory.Contains("nswag"))
{
    try
    {
        await app.InitialiseDatabaseAsync();
    }
    catch (Exception)
    {
        // ignored
    }
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

app.MapGroup("api/account").MapIdentityApi<ApplicationUser>();
// app.MapIdentityApi<ApplicationUser>();
app.MapRazorPages();
app.MapFallbackToFile("index.html");
app.UseExceptionHandler(options => { });
app.MapEndpoints();

app.Run();

public partial class Program
{
}
