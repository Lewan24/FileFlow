namespace FileFlow.Application.CodeExtensions;

internal static class DirectoryExtented
{
    public static void Copy(string sourceDirectory, string targetDirectory)
    {
        DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
        DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

        CopyAll(diSource, diTarget);
    }

    private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
    {
        Directory.CreateDirectory(target.FullName);

        CopyEachFileIntoNewDirectory(source, target);
        CopyEachSubDirectoryUsingRecursion(source, target);
    }

    private static void CopyEachFileIntoNewDirectory(DirectoryInfo source, DirectoryInfo target)
    {
        foreach (FileInfo fi in source.GetFiles())
        {
            if (fi.Name == ".FileFlow")
                continue;
            
            //Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
            fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
        }
    }

    private static void CopyEachSubDirectoryUsingRecursion(DirectoryInfo source, DirectoryInfo target)
    {
        foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
        {
            if (diSourceSubDir.Name == ".FileFlow")
                continue;

            DirectoryInfo nextTargetSubDir =
                target.CreateSubdirectory(diSourceSubDir.Name);
            CopyAll(diSourceSubDir, nextTargetSubDir);
        }
    }
}