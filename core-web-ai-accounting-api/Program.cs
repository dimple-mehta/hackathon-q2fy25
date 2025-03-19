using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost",
        policyBuilder => policyBuilder.WithOrigins(
                "https://localhost:3000",
                "https://app.sandbox.sbc.sage.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var endpoint = "https://hackfy25q2-start.openai.azure.com/";
var apiKey = "";

if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
{
    throw new ArgumentException("Please provide valid OpenAI endpoint and API key");
}

// Add the OpenAI chat completion service as a singleton
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Services.AddAzureOpenAIChatCompletion(
    deploymentName: "megabat_client-usage_gpt_4o",
    endpoint: endpoint,
    apiKey: apiKey,
    null,
    null
);
#pragma warning restore SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

// Create singletons of your plugins
// builder.Services.AddSingleton<GoProposal_Plugins>();

// Create the plugin collection (using the KernelPluginFactory to create plugins from objects)
builder.Services.AddSingleton<KernelPluginCollection>();


// Finally, create the Kernel service with the service provider and plugin collection
builder.Services.AddTransient((serviceProvider) => {
    KernelPluginCollection pluginCollection = serviceProvider.GetRequiredService<KernelPluginCollection>();
    return new Kernel(serviceProvider, pluginCollection);
});

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
