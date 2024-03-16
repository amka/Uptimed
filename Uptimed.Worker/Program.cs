using Hangfire;
using Hangfire.Redis.StackExchange;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();

// Add Jobs processing 
var redis = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("UptimedJobs")!);
GlobalConfiguration.Configuration.UseRedisStorage(redis);

using var server = new BackgroundJobServer();
Console.WriteLine("Hangfire Server started. Press any key to exit...");


var host = builder.Build();
host.Run();