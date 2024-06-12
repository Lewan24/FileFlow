using Cocona;
using FileFlow.Shared.Interfaces;

namespace FileFlow.Cli.Commands;

internal class RepoChangesCommands(IRepoService api)
{
    private readonly IRepoChangesService _repoChanges = api;

    [Command("commit", Description = "Saves all changes made after last commit")]
    public async Task CommitAsync([Option('m', Description = "Commit message")]string message)
    {
        await _repoChanges.CommitAsync(message);
    }
    
    [Command("status", Description = "Checks what changes were made from last commit to now")]
    public async Task CheckStatusAsync()
    {
        await _repoChanges.GetChanges();
    }
}