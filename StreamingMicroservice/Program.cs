using Microsoft.EntityFrameworkCore;
using StreamingMicroservice.Data;
using StreamingMicroservice.Services.Blob;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var cs = builder.Configuration.GetConnectionString("StreamingDb");

var blobSection = builder.Configuration.GetSection("BlobSettings");
builder.Services.Configure<BlobSettings>(blobSection);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(cs));

builder.Services.AddScoped<IBlobService, BlobService>();

builder.Services.AddControllers().AddJsonOptions(
    x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
    options.AllowAnyHeader();
});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
