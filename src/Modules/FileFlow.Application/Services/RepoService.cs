using System.Text;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using FileFlow.Application.CodeExtensions;
using FileFlow.Core.Entities;
using FileFlow.Shared.Interfaces;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;

namespace FileFlow.Application.Services;

internal class RepoService(ILogger<RepoService> logger) : IRepoService
{
    private readonly string _hiddenFileFlowFilesDirName = ".FileFlow";
    private readonly string _stateFileName = "state.data";
    private readonly string _configFileName = ".FileFlow.cfg";
    private readonly string _commitHistoryFileName = ".commits.history";
    //private readonly string _fileFlowIgnore = ".FileFlowIgnore";
    
    private async Task SaveCurrentStateAsync(string repoPath)
    {
        var statePath = Path.Combine(repoPath, _hiddenFileFlowFilesDirName, _stateFileName);

        if (!File.Exists(statePath))
        {
            var file = File.Create(statePath);
            file.Close();
        }

        var files = Directory.GetFiles(repoPath, "*", SearchOption.AllDirectories)
            .Where(f => f.Contains(repoPath) && !f.Contains(".FileFlow"))
            .Select(f => f.Replace(repoPath, "").TrimStart(Path.DirectorySeparatorChar))
            .ToList();

        var directories = Directory.GetDirectories(repoPath, "*", SearchOption.AllDirectories)
            .Where(d => d.Contains(repoPath) && !d.Contains(".FileFlow"))
            .Select(d => d.Replace(repoPath, "").TrimStart(Path.DirectorySeparatorChar))
            .ToList();

        await File.WriteAllLinesAsync(statePath, files.Concat(directories));

        await SaveNewestCommitToHiddenPrevDir(repoPath);
    }

    private Task SaveNewestCommitToHiddenPrevDir(string repoPath)
    {
        var prevDirPath = Path.Combine(repoPath, _hiddenFileFlowFilesDirName, "prev");
        if (Directory.Exists(prevDirPath))
            Directory.Delete(prevDirPath, true);
        
        DirectoryExtended.CopyRecursivelyWithExclude(new DirectorySourceTargetExclude(repoPath, prevDirPath, [".FileFlow"]));

        return Task.CompletedTask;
    }
    
    private async Task CreateInitConfigFileAsync(string repoName, string repoPath)
    {
        logger.LogInformation("Checking and preparing FileFlow required directories...");
        var hiddenFileFlowDirPath = Path.Combine(repoPath, _hiddenFileFlowFilesDirName);
        if (!Directory.Exists(hiddenFileFlowDirPath))
            Directory.CreateDirectory(hiddenFileFlowDirPath);
            
        logger.LogInformation("Creating init config file...");
        var config = new RepoConfig(repoName, repoPath);
        var configJson = JsonConvert.SerializeObject(config, Formatting.Indented);
        
        logger.LogInformation("Saving config...");
        var configFilePath = Path.Combine(hiddenFileFlowDirPath, _configFileName);
        await File.WriteAllTextAsync(configFilePath, configJson);
    }

    // private Task CreateInitIgnoreFileAsync()
    // {
    //     // TODO: To Implement
    //     return Task.CompletedTask;
    // }
    
    private async Task<RepoConfig?> GetConfigAsync()
    {
        var configFilePath = Path.Combine(Directory.GetCurrentDirectory(), _hiddenFileFlowFilesDirName, _configFileName);
        var configFileContent = await File.ReadAllTextAsync(configFilePath);
        var config = JsonConvert.DeserializeObject<RepoConfig>(configFileContent);

        return config;
    }
    
    private (List<string> Added, List<string> Removed) CheckAndGetChangesCounts(string statePath, RepoConfig config)
    {
        var previousState = File.ReadAllLines(statePath).ToHashSet();
        var currentState = Directory.GetFiles(Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories)
            .Where(f => f.Contains(config.RepoPathBase)  && !f.Contains(".FileFlow"))
            .Select(f => f.Replace(config.RepoPathBase, "").TrimStart(Path.DirectorySeparatorChar))
            .Concat(Directory.GetDirectories(config.RepoPathBase, "*", SearchOption.AllDirectories)
                .Where(d => d.Contains(config.RepoPathBase)  && !d.Contains(".FileFlow"))
                .Select(d => d.Replace(config.RepoPathBase, "").TrimStart(Path.DirectorySeparatorChar)))
            .ToHashSet();

        var added = currentState.Except(previousState).ToList();
        var removed = previousState.Except(currentState).ToList();

        return (added, removed);
    }

    private async Task AreFilesValidAsync()
    {
        var hiddenFileFlowFilesPath = Path.Combine(Directory.GetCurrentDirectory(), _hiddenFileFlowFilesDirName);
        if (!Directory.Exists(hiddenFileFlowFilesPath))
            throw new Exception("Can't find '.FileFlow' directory. Initialize repo using command 'init'.");
        
        var config = await GetConfigAsync();
        if (config is null)
            throw new Exception("Can't read configuration file. Init repository or change working directory.");
    }
    
    public async Task InitRepoAsync(string name)
    {
        logger.LogInformation("Trying to initialize repository {RepoName}...", name);

        var repoPath = Path.Combine(Directory.GetCurrentDirectory(), name);

        if (Directory.Exists(repoPath))
        {
            logger.LogWarning("Repository already exists.");
            return;
        }
        
        try
        {
            logger.LogInformation("Creating repository...");
            
            Directory.CreateDirectory(repoPath);
            await CreateInitConfigFileAsync(name, repoPath);
            
            await SaveCurrentStateAsync(repoPath);
        
            logger.LogInformation("Successfully initialized empty repository in: '{RepoPath}'", repoPath);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurs while creating directory: '{RepoName}'", name);
        }
    }

    public async Task CommitAsync(string message)
    {
        try
        {
            await AreFilesValidAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurs while validating files.");
            return;
        }
        
        var config = await GetConfigAsync();
        var statePath = Path.Combine(config!.RepoPathBase, _hiddenFileFlowFilesDirName, _stateFileName);
        var changes = CheckAndGetChangesCounts(statePath, config);
        
        if (changes.Added.Count == 0 && changes.Removed.Count == 0)
        {
            logger.LogInformation("Can't run commit. No changes detected.");
            return;
        }
        
        logger.LogInformation("Saving changes...");

        StringBuilder commitInfoBuilder = new StringBuilder();
        commitInfoBuilder.Append($"Commit from {DateTime.Now:dddd, dd MMMM yyyy HH:mm:ss} - '{message}' - {changes.Added.Count} insertions, {changes.Removed.Count} deletions.");
        commitInfoBuilder.AppendLine();
        
        var commitsHistoryFilePath = Path.Combine(config.RepoPathBase, _hiddenFileFlowFilesDirName, _commitHistoryFileName);
        await File.AppendAllTextAsync(commitsHistoryFilePath, commitInfoBuilder.ToString());

        await SaveCurrentStateAsync(config.RepoPathBase);
        
        logger.LogInformation("Successfully saved changes.");
    }

    public async Task<OneOf<AddedRemovedChanges, Error>> GetChanges()
    {
        try
        {
            await AreFilesValidAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurs while validating files.");
            return new Error();
        }
        
        var config = await GetConfigAsync();
        
        var statePath = Path.Combine(config!.RepoPathBase, _hiddenFileFlowFilesDirName, _stateFileName);
        if (!File.Exists(statePath))
        {
            logger.LogError("No state found. Please initialize one using 'init'.");
            return new Error();
        }

        await CompareCommitsAsync(Path.Combine(config.RepoPathBase, _hiddenFileFlowFilesDirName, "prev"), config.RepoPathBase);
        
        var changes = CheckAndGetChangesCounts(statePath, config);

        if (changes.Added.Count == 0 && changes.Removed.Count == 0)
        {
            logger.LogInformation("No structure changes detected.");
            return new AddedRemovedChanges(0, 0);
        }
        
        var tempForegroundColor = Console.ForegroundColor;
            
        if (changes.Added.Count > 0)
        {
            logger.LogInformation("New files/dirs:");
            Console.ForegroundColor = ConsoleColor.Green; 
            changes.Added.ForEach(Console.WriteLine);
            Console.ForegroundColor = tempForegroundColor;
        }

        if (changes.Removed.Count > 0)
        {
            logger.LogInformation("Removed files/dirs:");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            changes.Removed.ForEach(Console.WriteLine);
            Console.ForegroundColor = tempForegroundColor;
        }
        
        return new AddedRemovedChanges(changes.Added.Count, changes.Removed.Count);
    }

    private async Task CompareCommitsAsync(string oldCommitPath, string newCommitPath)
    {
        await CompareFilesAsync(oldCommitPath, newCommitPath);
    }
    
    private async Task CompareFilesAsync(string oldCommitPath, string newCommitPath)
    {
        var differ = new Differ();
        var inlineDiffBuilder = new InlineDiffBuilder(differ);

        var oldFiles = Directory.GetFiles(oldCommitPath, "*", SearchOption.AllDirectories)
            .Where(f => f.Contains(oldCommitPath)).ToList();
        var newFiles = Directory.GetFiles(newCommitPath, "*", SearchOption.AllDirectories)
            .Where(f => f.Contains(newCommitPath) && !f.Contains(".FileFlow")).ToList();
        
        foreach (var newFile in newFiles)
        {
            var relativePath = newFile.Replace(newCommitPath, "").TrimStart(Path.DirectorySeparatorChar);
            var oldFile = oldFiles.FirstOrDefault(f => f.EndsWith(relativePath));

            if (oldFile == null)
                continue;
            
            var oldText = await File.ReadAllTextAsync(oldFile);
            var newText = await File.ReadAllTextAsync(newFile);
                
            var diff = inlineDiffBuilder.BuildDiffModel(oldText, newText);

            if (diff.Lines.Count == 0)
                continue;
                
            Console.WriteLine($"Changes in {relativePath}:");

            foreach (var line in diff.Lines)
            {
                switch (line.Type)
                {
                    case ChangeType.Inserted:
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"+ {line.Text}");
                        break;
                    case ChangeType.Deleted:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"- {line.Text}");
                        break;
                    case ChangeType.Modified:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"<  {line.Text}");
                        break;
                }
            }

            Console.ResetColor();
        }
    }
    
    public async Task<OneOf<List<string>, Error>> GetCommitsHistoryAsync()
    {
        try
        {
            await AreFilesValidAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error occurs while validating files");
            return new Error();
        }

        var config = await GetConfigAsync();

        var commitsHistoryFilePath =
            Path.Combine(config!.RepoPathBase, _hiddenFileFlowFilesDirName, _commitHistoryFileName);
        if (!File.Exists(commitsHistoryFilePath))
        {
            logger.LogWarning("Can't read history. Make first commit and try again!");
            return new Error();
        }

        var historyContent = (await File.ReadAllLinesAsync(commitsHistoryFilePath)).ToList();
        historyContent.ForEach(Console.WriteLine);

        return historyContent;
    }
}
