using DirectoryService.Infrastructure;
using DirectoryService.Presentation.EndpointResults;
using DirectoryService.Web;
using Microsoft.OpenApi.Models;
using Shared;
using Shared.Errors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProgramDependencies();

builder.Services.AddScoped<DirectoryServiceDbContext>(_ =>
    new DirectoryServiceDbContext(builder.Configuration.GetConnectionString("DirectoryServiceDb")!,  builder.Environment.IsDevelopment()));

builder.Services.AddOpenApi(
    options =>
    {
        options.AddSchemaTransformer(
            (schema, context, _) =>
            {
                if (context.JsonTypeInfo.Type == typeof(Envelope<Errors>))
                {
                    if (schema.Properties.TryGetValue("errors", out var errorsProp))
                    {
                        errorsProp.Items.Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.Schema,
                            Id = "Error",
                        };
                    }
                }

                return Task.CompletedTask;
            });
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "DirectoryService v1"));
}

app.MapControllers();

app.Run();