using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// ��������� ��������� CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()  // ��������� ����� ��������
              .AllowAnyMethod()  // ��������� ����� ������ (GET, POST, PUT, DELETE)
              .AllowAnyHeader(); // ��������� ����� ���������
    });
});

var app = builder.Build();

// ���������� CORS � ����������
app.UseCors("AllowAllOrigins");

// ����������� ��� �������� ���������� � ������
List<SmartPhone> repo = new List<SmartPhone>();

// ��������� ������ ��� �����������
repo.Add(new SmartPhone("Samsung Galaxy S23", 3.2, 8, 256, "Android", 169));
repo.Add(new SmartPhone("iPhone 15", 3.4, 6, 128, "iOS", 174));

app.MapGet("/api/smartphones", () => Results.Ok(repo));
app.MapGet("/api/smartphones/{id}", (int id) => Results.Ok(repo.FirstOrDefault(s => s.Model.GetHashCode() == id)));
app.MapPost("/api/smartphones", (SmartPhone smartphone) => {
    repo.Add(smartphone);
    return Results.Created($"/api/smartphones/{smartphone.Model}", smartphone);
});
app.MapPut("/api/smartphones/{id}", (int id, SmartPhone smartphone) => {
    var existingSmartphone = repo.FirstOrDefault(s => s.Model.GetHashCode() == id);
    if (existingSmartphone == null)
        return Results.NotFound();
    existingSmartphone.Model = smartphone.Model;
    existingSmartphone.ProcessorSpeed = smartphone.ProcessorSpeed;
    existingSmartphone.Ram = smartphone.Ram;
    existingSmartphone.Storage = smartphone.Storage;
    existingSmartphone.Os = smartphone.Os;
    existingSmartphone.Weight = smartphone.Weight;
    return Results.NoContent();
});
app.MapDelete("/api/smartphones/{id}", (int id) => {
    var smartphone = repo.FirstOrDefault(s => s.Model.GetHashCode() == id);
    if (smartphone != null)
    {
        repo.Remove(smartphone);
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.Run();

// ����� SmartPhone
public class SmartPhone
{
    public string Model { get; set; }
    public double ProcessorSpeed { get; set; }
    public int Ram { get; set; }
    public int Storage { get; set; }
    public string Os { get; set; }
    public double Weight { get; set; }

    public SmartPhone(string model, double processorSpeed, int ram, int storage, string os, double weight)
    {
        Model = model;
        ProcessorSpeed = processorSpeed;
        Ram = ram;
        Storage = storage;
        Os = os;
        Weight = weight;
    }
}
