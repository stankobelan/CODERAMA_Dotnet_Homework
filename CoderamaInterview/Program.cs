using CoderamaInterview.Factories;
using CoderamaInterview.Formatter;
using CoderamaInterview.Repositories;
using CoderamaInterview.Serialization;
using CoderamaInterview.Services;
using CoderamaInterview.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Register response caching services
builder.Services.AddResponseCaching();
// Add services to the container.
// Registrácia repository
builder.Services.AddSingleton<IDocumentRepository, InMemoryDocumentRepository>();
//builder.Services.AddSingleton<IDocumentRepository, FileSystemDocumentRepository>();

// Registrácia serializerov (pridajte ďalšie implementácie podľa potreby)
builder.Services.AddSingleton<IDocumentSerializer, JsonDocumentSerializer>();
builder.Services.AddSingleton<IDocumentSerializer, XmlDocumentSerializer>(); 
builder.Services.AddSingleton<IDocumentSerializer, MessagePackDocumentSerializer>();

// Registrácia factory pre serializer
builder.Services.AddSingleton<IDocumentSerializerFactory, DocumentSerializerFactory>();

// Registrácia business logiky
builder.Services.AddSingleton< IDocumentService,DocumentService>();


builder.Services.AddControllers(options =>
    {
        options.InputFormatters.Insert(0, new MessagePackInputFormatter());
        options.OutputFormatters.Insert(0, new MessagePackOutputFormatter());
    })
    .AddXmlSerializerFormatters();

builder.Services.AddValidatorsFromAssemblyContaining<DocumentValidator>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Configure logging BEFORE building the app.
if (builder.Environment.IsDevelopment())
{
    // In development mode, clear existing providers and add Console logging.
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
}



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Enable response caching middleware
app.UseResponseCaching();
app.MapControllers();

app.Run();