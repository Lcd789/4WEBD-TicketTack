using BlazorWeb.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Récupération de l'URL de l'API Gateway
var apiGatewayUrl = builder.Configuration["ApiGateway:Url"] ?? "http://api-gateway/";

// Ajout des services pour Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Ajout du HttpClient avec IHttpClientFactory
builder.Services.AddHttpClient("ApiGatewayClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:YOUR_APIGATEWAY_PORT/"); // Remplacez par le bon port
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Ajout du HttpClient en service Scoped (utilisation simple dans les composants Blazor)
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiGatewayClient"));

var app = builder.Build();

// Configuration du pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
