using FileFlow.Core.Entities;

namespace FileFlow.Application.CodeExtensions;

internal static class DirectoryExtended
{
    public static void CopyRecursivelyWithExclude(DirectorySourceTargetExclude dirSrcTarget)
    {
        DirectoryInfo diSource = new DirectoryInfo(dirSrcTarget.SourcePath);
        DirectoryInfo diTarget = new DirectoryInfo(dirSrcTarget.TargetPath);

        CopyAll(new DirectoryInfoSourceTargetExclude(diSource, diTarget, dirSrcTarget.Exclude));
    }

    private static void CopyAll(DirectoryInfoSourceTargetExclude dirISrcTarget)
    {
        Directory.CreateDirectory(dirISrcTarget.Target.FullName);

        CopyEachFileIntoNewDirectory(dirISrcTarget);
        CopyEachSubDirectoryUsingRecursion(dirISrcTarget);
    }

    private static void CopyEachFileIntoNewDirectory(DirectoryInfoSourceTargetExclude dirISrcTarget)
    {
        foreach (FileInfo fi in dirISrcTarget.Source.GetFiles())
        {
            if (dirISrcTarget.Exclude is not null && dirISrcTarget.Exclude.Contains(fi.Name))
                continue;
            
            //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
            fi.CopyTo(Path.Combine(dirISrcTarget.Target.FullName, fi.Name), true);
        }
    }

    private static void CopyEachSubDirectoryUsingRecursion(DirectoryInfoSourceTargetExclude dirISrcTarget)
    {
        foreach (DirectoryInfo diSourceSubDir in dirISrcTarget.Source.GetDirectories())
        {
            if (dirISrcTarget.Exclude is not null && dirISrcTarget.Exclude.Contains(diSourceSubDir.Name))
                continue;

            DirectoryInfo nextTargetSubDir =
                dirISrcTarget.Target.CreateSubdirectory(diSourceSubDir.Name);
            
            CopyAll(dirISrcTarget);
        }
    }
}