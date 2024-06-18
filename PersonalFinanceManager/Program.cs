using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using PersonalFinanceManager.DatabaseContext;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default")
    );
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PersonalFinanceManagerAPI",
        Version = "1.0",
        Description = "API para gerenciamento financeiro pessoal.",
        Contact = new OpenApiContact
        {
            Name = "Henrique dos Santos Silva",
            Email = "hss.henrique.silva@gmail.com",
            Url = new Uri("https://github.com/henrique-santos-silva/PersonalFinanceManager"),
        },
       
    });

    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header, 
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            new string[] {}
        }
    });

    c.EnableAnnotations();

});

var app = builder.Build();



app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PersonalFinanceManagerAPI V1");
    c.RoutePrefix = string.Empty;
    c.DocumentTitle = "Doc: Personal Finance Manager API"; 
});


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
