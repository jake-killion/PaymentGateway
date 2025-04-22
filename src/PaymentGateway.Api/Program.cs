using PaymentGateway.Api.Models.Settings;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Repositories.Interfaces;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Services.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Gathers configuration settings from appsettings.json and environment variables
builder.Services.Configure<AcquiringBankSettings>(
    builder.Configuration.GetSection("AcquiringBank"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

// Registering services and repositories with the dependency injection container
// This allows us to inject them into our controllers and other services
// We use AddSingleton for the repository to ensure that we have a single instance of it
// We use AddTransient for the services to ensure that we have a new instance of them each time they are requested
builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddTransient<IPaymentsService, PaymentsService>();
builder.Services.AddTransient<IPaymentsValidationService, PaymentsValidationService>();
builder.Services.AddTransient<IAcquiringBankService, AcquiringBankService>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
