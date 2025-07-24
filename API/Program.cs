using BL.RabbitMQ;
using BL.RabbitMQ.Publisher;

var builder = WebApplication.CreateBuilder(args);

// Logging Services
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

// Database setup

// Supabase file Storage setup

// RabbitMQ setup
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton(typeof(RabbitMqExchanges));
builder.Services.AddSingleton(typeof(RabbitMqQueues));
builder.Services.AddSingleton(typeof(IRabbitMqPublisher<>), typeof(RabbitMqPublisher<>));

// Repositories

// Services

// OpenAPI services
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Allowed cors for the application
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy.WithOrigins("http://localhost:5176")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
    options.AddPolicy("FrontendProd", policy =>
    {
        policy.WithOrigins("https://skalrahub.com")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseCors("FrontendDev");
}
else
{
    app.UseCors("FrontendProd");
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();