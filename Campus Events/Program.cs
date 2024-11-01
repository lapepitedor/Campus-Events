
using Campus_Events;

var builder = new HostBuilder();
builder.UseContentRoot(Directory.GetCurrentDirectory());

// Configure the Generic Host here...
//configure Logging
builder.ConfigureLogging(logging =>
{
    logging.AddConsole();
});

//configure app settings
builder.ConfigureAppConfiguration((hostingContext, config) =>
{
    var env = hostingContext.HostingEnvironment;
    config.AddJsonFile("appsettings.json", optional: false);
    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
    config.AddEnvironmentVariables();
    config.AddCommandLine(args);
});

builder.ConfigureWebHostDefaults(webBuilder =>
{
    // Configure the WebHost here...
    webBuilder.UseStartup<StartUp>();

});

var host = builder.Build();
host.Run();