using LoanGateway.Services;
using LoanGeteway.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMongoHelper, MongoHelper>();
builder.Services.AddScoped<ILoanService, LoanService>();


var mongoUri = builder.Configuration.GetConnectionString("mongo");


var CorsPolicy = "CorsPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, builder => {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();


IMongoClient client;
IMongoCollection<object> collection;
try
{
    client = new MongoClient(mongoUri);
    var dbName = "loan-gateway";
    var collectionName = "eligibility";

    collection = client.GetDatabase(dbName)
       .GetCollection<object>(collectionName);
}
catch (Exception e)
{
    Console.WriteLine("There was a problem connecting to your " +
        "Atlas cluster. Check that the URI includes a valid " +
        "username and password, and that your IP address is " +
        $"in the Access List. Message: {e.Message}");
    Console.WriteLine(e);
    Console.WriteLine();
    return;
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
