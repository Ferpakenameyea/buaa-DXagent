using DXAgent;
using DXAgent.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Configuration
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

string apiKey = builder.Configuration["FAST_GPT_KEY"] ?? throw new InvalidOperationException("FAST_GPT_KEY environment variable is not set.");

builder.Services.AddScoped<IFastGPTAPI>(service =>
{
    return new FastGPTAPI(
        new Uri("http://113.47.2.75:3000"),
        apikey: apiKey);
});

builder.Services.AddSingleton<IPromptTemplate, PromptTemplate>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(DXAgent.Client._Imports).Assembly);

app.Run();
