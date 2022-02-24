using System.Net.Mime;
using covidtracking.Settings;
using covidtracking.Database;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using covidtracking.Entities;
using covidtracking.Dtos;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

//Custom classes mapper
BsonClassMap.RegisterClassMap<Patient>(cm =>
{
    cm.MapProperty(c => c.govtId);
    cm.MapProperty(c => c.firstName);
    cm.MapProperty(c => c.lastName);
    cm.MapProperty(c => c.birthDate);
    cm.MapProperty(c => c.phoneNumber);
    cm.MapProperty(c => c.email);
    cm.MapProperty(c => c.address);
    cm.MapProperty(c => c.isCovidPositive);
    cm.MapProperty(c => c.houseResidentsAmount);
    cm.MapProperty(c => c.infectedByPatientID);
});
BsonClassMap.RegisterClassMap<PotentialPatient>(cm =>
{
    cm.MapProperty(c => c.key);
    cm.MapProperty(c => c.firstName);
    cm.MapProperty(c => c.lastName);
    cm.MapProperty(c => c.phoneNumber);
});
BsonClassMap.RegisterClassMap<PatientEncounter>(cm =>
{
    cm.MapProperty(c => c.id);
    cm.MapProperty(c => c.potentialPatientsEncountered);
});
BsonClassMap.RegisterClassMap<PatientRoute>(cm =>
{
    cm.MapProperty(c => c.id);
    cm.MapProperty(c => c.route);
});
BsonClassMap.RegisterClassMap<Address>(cm =>
{
    cm.MapProperty(c => c.city);
    cm.MapProperty(c => c.street);
    cm.MapProperty(c => c.houseNumber);
    cm.MapProperty(c => c.appartmentNumber);
});
BsonClassMap.RegisterClassMap<Visit>(cm =>
{
    cm.MapProperty(c => c.dateOfVisit);
    cm.MapProperty(c => c.siteName);
    cm.MapProperty(c => c.siteAddress);
});
BsonClassMap.RegisterClassMap<LabTests>(cm =>
{//FIXED
    cm.MapProperty(c => c.id);
    cm.MapProperty(c => c.numberOfNegativeTests);
    cm.MapProperty(c => c.labTestResults);
});
BsonClassMap.RegisterClassMap<LabTestResult>(cm =>
{
    cm.MapProperty(c => c.labId);
    cm.MapProperty(c => c.testId);
    cm.MapProperty(c => c.patientId);
    cm.MapProperty(c => c.testDate);
    cm.MapProperty(c => c.isCovidPositive);
});
BsonClassMap.RegisterClassMap<Isolated>(cm =>
{
    cm.MapProperty(c => c.id);
    cm.MapProperty(c => c.encounteredId);
});
BsonClassMap.RegisterClassMap<Statistics>(cm =>
{
    cm.MapProperty(c => c.infected);
    cm.MapProperty(c => c.healed);
    cm.MapProperty(c => c.isolated);
    cm.MapProperty(c => c.cityStatistics);
});
BsonClassMap.RegisterClassMap<CityStatistics>(cm =>
{
    cm.MapProperty(c => c.city);
    cm.MapProperty(c => c.infected);
});
BsonClassMap.RegisterClassMap<Infected>(cm =>
{
    cm.MapProperty(c => c.id);
    cm.MapProperty(c => c.infectedDateTime);
});
BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
//BsonSerializer.RegisterSerializer(new BsonDateTimeSerializer());      IS NEEDED??

var mongoDbsettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

builder.Services.AddSingleton<IMongoClient>(ServiceProvider =>
{
    return new MongoClient(mongoDbsettings.ConnectionString);
});

// Dependency Injection
builder.Services.AddSingleton<IIsolatedDB, MongoDBIsolatedModel>();
builder.Services.AddSingleton<IPatientRoutesDB, MongoDBPatientsRoutesModel>();
builder.Services.AddSingleton<IPotentialPatientsDB, MongoDBPotentialPatientsModel>();
builder.Services.AddSingleton<IPatientsDB, MongoDBPatientsModel>();
builder.Services.AddSingleton<IPatientsEncountersDB, MongoDBPatientsEncountersModel>();
builder.Services.AddSingleton<ILabTestsDB, MongoDBLabTestsModel>();
builder.Services.AddSingleton<IStatisticsDb, MongoDBStatisticsModel>();
builder.Services.AddSingleton<IInfectedDB, MongoDBInfectedModel>();

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

builder.Services.AddHealthChecks()
.AddMongoDb(
    mongoDbsettings.ConnectionString,
    name: "covidtracking",
    timeout: TimeSpan.FromSeconds(3),
    tags: new[] { "ready" }
    );
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}


app.UseAuthorization();

//HealthChecks
app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
{
    Predicate = (check) => check.Tags.Contains("ready"),
    ResponseWriter = async (context, report) =>
    {
        var result = JsonSerializer.Serialize(
            new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    exception = entry.Value.Exception != null ? entry.Value.Exception.Message : "none",
                    duration = entry.Value.Duration.ToString()
                })
            }
        );
        context.Response.ContentType = MediaTypeNames.Application.Json;
        await context.Response.WriteAsync(result);
    }
});
app.MapHealthChecks("/healthz/live", new HealthCheckOptions
{
    Predicate = (_) => false
});

app.MapControllers();

app.Run();
