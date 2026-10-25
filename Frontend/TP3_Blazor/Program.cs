using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TP3_Blazor;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using TP3_Blazor.Services;
using TP3_Blazor.Providers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Registrar servicios a consumir en la aplicación Blazor WebAssembly
builder.Services.AddScoped<AuthService>();

// Configuramos el HttpClient para que apunte directamente a la API en el puerto 5126
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5126/") });

// Agregamos el soporte para LocalStorage (guardar el token JWT en el navegador)
builder.Services.AddBlazoredLocalStorage();

// Registramos los servicios de Autorización y el Proveedor de Autenticación personalizado
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<ApiAuthenticationStateProvider>(); // <-- Esta línea asegura el casting correcto en el AuthService

await builder.Build().RunAsync();