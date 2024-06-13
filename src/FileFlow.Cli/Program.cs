using System.Reflection;
using Cocona;
using FileFlow.Api;
using FileFlow.Cli.Commands;
using Microsoft.Extensions.Logging;

var builder = CoconaApp.CreateBuilder(args, options =>
{
    options.TreatPublicMethodsAsCommands = false;
});

builder.Services.AddFileFlowApi();

var app = builder.Build();

app.AddCommand("testrun", (ILogger<Program> logger) =>
{
    logger.LogInformation("Application has started successfully. Test Completed");
}).WithDescription("also: 'test' // Test if the application runs properly")
.WithAliases("test");

app.AddCommand("version", () =>
{
    var asciiLogo = @"
  _____ _ _      _____ _               
 |  ___(_) | ___|  ___| | _____      __
 | |_  | | |/ _ \ |_  | |/ _ \ \ /\ / /
 |  _| | | |  __/  _| | | (_) \ V  V / 
 |_|   |_|_|\___|_|   |_|\___/ \_/\_/  
                                       
";

    Console.ForegroundColor = ConsoleColor.DarkMagenta;
    Console.WriteLine(asciiLogo);

    var currentAssembly = Assembly.GetEntryAssembly();
    var versionString = currentAssembly?
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
        .InformationalVersion;
    var company = currentAssembly?.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
    var copyright = currentAssembly?.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
    
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"FileFlow v{versionString}");
    
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nCompany: {company}\nCopyright: {copyright}\n");
    
    Console.ResetColor();
}).WithDescription("also: 'v' // Check the version of the application")
    .WithAliases("v");

app.AddCommands<RepoConfigurationCommands>();
app.AddCommands<RepoChangesCommands>();
app.AddCommands<RepoHistoryCommands>();

app.Run();