using FluentValidation;

using PaymentGateway.Api.Configuration;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services;
using PaymentGateway.Api.Services.Validators;

using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register interfaces and their implementations
builder.Services.AddSingleton<IPaymentsRepository, PaymentsRepository>();
builder.Services.AddScoped<IPaymentsProcessor, PaymentProcessor>();
builder.Services.AddSingleton<IValidator<MerchantPaymentRequest>, MerchantPaymentRequestValidator>();

// Bind configuration
var acquiringBankApiOptions = builder.Configuration.GetSection("AcquiringBankApi").Get<AcquiringBankApiOptions>() ?? new AcquiringBankApiOptions();
// Register the Refit client and get BaseUri from appsettings.json
builder.Services.AddRefitClient<IAcquiringBankApi>()
        .ConfigureHttpClient(c => c.BaseAddress = new Uri(acquiringBankApiOptions.BaseUri));

var app = builder.Build();

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