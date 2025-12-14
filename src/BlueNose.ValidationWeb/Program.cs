using BlueNose.Core.Extensions;
using BlueNose.ValidationWeb.Tools;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "BlueNose FHIR Validation API",
        Version = "v1",
        Description = "RESTful API for FHIR resource validation with SSE-based MCP transport support"
    });
});

// Add BlueNose core services
builder.Services.AddBlueNoseCore(builder.Configuration);

// Configure MCP server with SSE transport for web
builder.Services
    .AddMcpServer(options =>
    {
        options.ServerInfo = new()
        {
            Name = "BlueNose FHIR Validation Web",
            Version = "1.0.0"
        };
    })
    .WithHttpTransport()
    .WithToolsFromAssembly();

// Add CORS for MCP SSE clients
builder.Services.AddCors(options =>
{
    options.AddPolicy("McpPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BlueNose FHIR Validation API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("McpPolicy");
app.UseAuthorization();

app.MapControllers();

// Map MCP SSE endpoint
app.MapMcp("/mcp");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Timestamp = DateTimeOffset.UtcNow,
    FhirVersion = builder.Configuration["Fhir:Version"]
}));

app.Run();

// Make Program class accessible for integration testing
public partial class Program { }
