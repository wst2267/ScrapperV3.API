using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using ScrapperV3.API.Repository;
using ScrapperV3.API.Utility.Configuration;

try
{
    var builder = WebApplication.CreateBuilder();

    //set config
    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

    var configuration = config.Build();
    builder.Services.Configure<MongoDbConfig>(configuration.GetSection("MongoDbConfig"));
    builder.Services.Configure<LibraryConfig>(configuration.GetSection("LibraryConfig"));

    builder.Services.AddScoped<IScrapperRepository, ScrapperRepository>();

    builder.Services.AddHttpClient(Options.DefaultName, c =>
    {
    }).ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, certChain, policyErrors) => true
        };
    });

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddMvc().AddNewtonsoftJson();
    builder.Services.AddOptions();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Scrapper V3", Version = "v3" });
    });

    var app = builder.Build();
    app.UseHttpsRedirection();
    app.UseSwagger();
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "Scrapper V3");
        //s.RoutePrefix = "/swagger/index.html";
    });
    app.UseCors(builder => builder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()
    );
    app.UseDeveloperExceptionPage();

    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    // log
}