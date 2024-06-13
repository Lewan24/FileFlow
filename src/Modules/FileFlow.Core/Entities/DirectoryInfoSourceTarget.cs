namespace FileFlow.Core.Entities;

public record DirectoryInfoSourceTargetExclude(DirectoryInfo Source, DirectoryInfo Target, string[]? Exclude = null);