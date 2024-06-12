namespace FileFlow.Shared.Interfaces;

/// <summary>
/// Global service containing all needed functions to allow service work properly
/// </summary>
public interface IRepoService : IRepoConfigService, IRepoChangesService;

/// <summary>
/// Basic functions like initialization
/// </summary>
public interface IRepoConfigService
{
    /// <summary>
    /// Initialize repository
    /// </summary>
    /// <param name="name"><see cref="string"/> name of new repository</param>
    /// <returns></returns>
    Task InitRepoAsync(string name);
}

/// <summary>
/// Needed functions for commiting and checking changes in repo
/// </summary>
public interface IRepoChangesService
{
    /// <summary>
    /// Saves changes in repository
    /// </summary>
    /// <param name="message"><see cref="string"/> message that will be stored in history of commits</param>
    /// <returns></returns>
    Task CommitAsync(string message);
    /// <summary>
    /// Get actual status and show all changes made from last commit
    /// </summary>
    /// <returns></returns>
    Task GetChanges();
}