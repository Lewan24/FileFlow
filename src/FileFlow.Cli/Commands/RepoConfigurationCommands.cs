using Cocona;
using FileFlow.Shared.Interfaces;

namespace FileFlow.Cli.Commands;

internal class RepoConfigurationCommands(IRepoService api)
{
    private readonly IRepoConfigService _repoConfiguration = api;

    [Command("init", Description = "Creates new repository")]
    public async Task InitAsync([Option("name", shortNames: ['n', 'N'], Description = "Repository name")]string repoName = "Default_Init_Repo")
    {
        await _repoConfiguration.InitRepoAsync(repoName);
    }
}