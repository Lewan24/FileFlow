namespace FileFlow.Core.Entities;

public record DirectorySourceTargetExclude(string SourcePath, string TargetPath, string[]? Exclude = null);
