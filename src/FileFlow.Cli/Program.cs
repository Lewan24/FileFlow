﻿using Cocona;
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
}).WithDescription("Test if the application runs properly");

app.AddCommands<RepoConfigurationCommands>();
app.AddCommands<RepoChangesCommands>();
app.AddCommands<RepoHistoryCommands>();

app.Run();