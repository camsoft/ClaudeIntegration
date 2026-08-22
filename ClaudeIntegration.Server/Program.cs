using System.Diagnostics;
using Anthropic;
using Anthropic.Core;
using ClaudeIntegration.Server.Services;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// The API key should be stored via `dotnet user-secrets set Anthropic:ApiKey "<key>"`
// (this project already has a UserSecretsId configured) or the ANTHROPIC_API_KEY
// environment variable - never commit it to source control.
builder.Services.AddSingleton(_ =>
{
    var apiKey = builder.Configuration["Anthropic:ApiKey"]
        ?? Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY")
        ?? throw new InvalidOperationException(
            "Anthropic API key not configured. Set it via 'dotnet user-secrets set Anthropic:ApiKey \"<key>\"' or the ANTHROPIC_API_KEY environment variable.");

    return new AnthropicClient(new ClientOptions { ApiKey = apiKey });
});
builder.Services.AddScoped<IClaudeService, ClaudeService>();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    // Automatically open a Swagger UI browser tab once the app has started,
    // in addition to the SPA tab that Visual Studio opens via launchBrowser.
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var addresses = app.Services.GetRequiredService<IServer>()
            .Features.Get<IServerAddressesFeature>()?.Addresses;
        var address = addresses?.FirstOrDefault(a => a.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            ?? addresses?.FirstOrDefault();

        if (address is null)
        {
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo($"{address}/swagger") { UseShellExecute = true });
        }
        catch
        {
            // Ignore failures (e.g., no default browser configured in the environment).
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
