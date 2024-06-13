using Cocona;
using FileFlow.Shared.Interfaces;

namespace FileFlow.Cli.Commands;

internal class RepoHistoryCommands(IRepoService api)
{
    private readonly IRepoHistoryService _repoHistory = api;

    [Command("history", Description = "Check history of commits")]
    public async Task GetHistoryAsync()
    {
        await _repoHistory.GetCommitsHistoryAsync();
    }
}