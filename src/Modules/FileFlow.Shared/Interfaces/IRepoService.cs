using FileFlow.Core.Entities;
using OneOf;
using OneOf.Types;

namespace FileFlow.Shared.Interfaces;

/// <summary>
/// Global service containing all needed functions to allow service work properly
/// </summary>
public interface IRepoService : IRepoConfigService, IRepoChangesService, IRepoHistoryService;

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
    /// <returns><see cref="AddedRemovedChanges"/> with count of added items and removed items or <see cref="Error"/></returns>
    Task<OneOf<AddedRemovedChanges, Error>> GetChanges();
}

/// <summary>
/// Interface that stores function for commits history operations
/// </summary>
public interface IRepoHistoryService
{
    /// <summary>
    /// Simple method to fetch history of made commits on repo
    /// </summary>
    /// <returns><see cref="List{T}"/> with commits history or <see cref="Error"/></returns>
    Task<OneOf<List<string>, Error>> GetCommitsHistoryAsync();
}