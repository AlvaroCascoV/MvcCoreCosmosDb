using Microsoft.Azure.Cosmos;
using MvcCoreCosmosDb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string cosmosDbString = builder.Configuration.GetValue<string>("AzureKeys:CosmosDb");
CosmosClient client = new CosmosClient(cosmosDbString);
Container container = client.GetContainer("vehiculoscosmos", "containercoches");
builder.Services.AddSingleton<CosmosClient>(x => client);
builder.Services.AddTransient<Container>(x => container);
builder.Services.AddTransient<ServiceCosmosDb>();


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
