using Cocona;
using FileFlow.Api;
using FileFlow.Cli.Commands;

var builder = CoconaApp.CreateBuilder(args, options =>
{
    options.TreatPublicMethodsAsCommands = false;
});

builder.Services.AddFileFlowApi();

var app = builder.Build();

app.AddCommands<RepoConfigurationCommands>();
app.AddCommands<RepoChangesCommands>();

app.Run();