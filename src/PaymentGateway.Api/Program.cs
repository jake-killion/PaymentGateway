using PaymentGateway.Api.Models.Settings;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Repositories.Interfaces;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Services.Interfaces;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AcquiringBankSettings>(
    builder.Configuration.GetSection("AcquiringBank"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
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
