using FileFlow.Shared.Interfaces;
using Microsoft.Extensions.Logging;

namespace FileFlow.Application.Services;

internal class RepoService(ILogger<RepoService> logger) : IRepoService
{
    public async Task InitRepoAsync(string name)
    {
        logger.LogInformation("Initializing repository {RepoName}...", name);

        await Task.Delay(1000);
    
        logger.LogInformation("Successfully initialized repository with name: '{RepoName}'", name);
    }

    public async Task CommitAsync(string message)
    {
        logger.LogInformation("Saving changes...");

        await Task.Delay(2000);
    
        logger.LogInformation("Successfully saved made changes with message: '{CommitMessage}'", message);
    }

    public Task GetChanges()
    {
        throw new NotImplementedException();
    }
}