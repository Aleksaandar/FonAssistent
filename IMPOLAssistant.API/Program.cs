using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Elastic.Transport;
using FONAssistant.API.Data;
using IMPOLAssistant.API.Models;
using IMPOLAssistant.KernelMemory;
using IMPOLAssistant.SemanticKernel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.DocumentStorage.DevTools;
using Microsoft.KernelMemory.FileSystem.DevTools;
using Microsoft.KernelMemory.MemoryStorage.DevTools;
using Microsoft.KernelMemory.Pipeline.Queue.DevTools;
using Microsoft.KernelMemory.SemanticKernel;
using Microsoft.SemanticKernel;
using System;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var openAiApiKey = builder.Configuration["OpenAI:ApiKey"];

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<FonAssistentDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped(container =>
{
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4o-mini", openAiApiKey)
    .AddOpenAITextEmbeddingGeneration("text-embedding-ada-002", openAiApiKey)
    .Build();
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    return kernel;
});


var openAIConfig = new OpenAIConfig
{
    TextModel = "gpt-4o-mini",
    TextModelMaxTokenTotal = 16000,
    EmbeddingModel = "text-embedding-ada-002",
    EmbeddingModelMaxTokenTotal = 8191,
    APIKey = openAiApiKey
};
builder.Services.AddScoped<IKernelMemory>(container =>
{
    return new KernelMemoryBuilder()
        .WithOpenAI(openAIConfig)
        .WithElasticsearchMemoryDb(new ElasticsearchConfig()
        {
            Endpoint = "https://localhost:9200/",
            UserName = "elastic",
            Password = "vnaCcGxhAEI5JIuhz6Wx"
        })
        .WithSearchClientConfig(new SearchClientConfig
        {
            AnswerTokens = 10_000,
            EmptyAnswer = "Odgvor nije pronadjen.",
            Temperature = 0.1
        })
        .Build<MemoryServerless>();
});

builder.Services.AddTransient<IKernelMemoryService, KernelMemoryService>();
builder.Services.AddScoped<ISemanticKernelService, SemanticKernelService>();
builder.Services.AddScoped<ITabelaLeguraKernelService, TabelaLeguraKernelService>();



var app = builder.Build();




if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
