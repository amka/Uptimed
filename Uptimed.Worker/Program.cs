using Hangfire;
using Hangfire.Redis.StackExchange;
using Octonica.ClickHouseClient;
using StackExchange.Redis;
using Uptimed.Shared;
using Uptimed.Worker.Jobs;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add Jobs processing 
// Init ClickHouse connection

// Init Redis connection
builder.Services.AddScoped<ICallMonitoringJob, CallMonitoringJob>(sp => new CallMonitoringJob(
    new HttpClient(),
    new ClickHouseConnection(builder.Configuration.GetConnectionString("UptimedLogs")!)
));

// Init Hangfire
builder.Services.AddHangfire(configuration =>
{
    var redis = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("UptimedJobs")!);
    configuration.UseRedisStorage(redis);
});
builder.Services.AddHangfireServer();


var host = builder.Build();
await host.RunAsync();