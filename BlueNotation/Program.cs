using BlueNotation;
using BlueNotation.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<MidiService>();
builder.Services.AddSingleton<DataService>();
builder.Services.AddSingleton<LocalStorageService>();
builder.Services.AddSingleton<ConductorService>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();
