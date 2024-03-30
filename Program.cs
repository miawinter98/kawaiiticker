using Kawaiiticker.Components;
using Kawaiiticker.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddHealthChecks();
builder.Services.AddDbContextFactory<AppRepository>();

#endregion

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.UseHealthChecks("/health");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

{
	await using var scope = app.Services.CreateAsyncScope();
	var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppRepository>>();
	await using var context = await factory.CreateDbContextAsync();
	await context.Database.MigrateAsync();
}

app.Run();
