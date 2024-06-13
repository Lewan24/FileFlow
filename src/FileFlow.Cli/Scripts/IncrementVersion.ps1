param (
    [string]$AssemblyInfoPath
)

$assemblyInfoContent = Get-Content $AssemblyInfoPath

# Find the version attributes and increment their version numbers
$regex = [regex]::Escape('[assembly: AssemblyInformationalVersion("') + '([0-9]+)\.([0-9]+)\.([0-9]+)\.([0-9]+)"' + [regex]::Escape(')]')
$matches = [regex]::Matches($assemblyInfoContent, $regex)

if ($matches.Count -eq 1) {
    $major = [int]$matches[0].Groups[1].Value
    $minor = [int]$matches[0].Groups[2].Value
    $build = [int]$matches[0].Groups[3].Value
    $revision = [int]$matches[0].Groups[4].Value

    # Increment the revision number
    $revision += 1

    if ($revision -ge 10) {
        $revision = 0
        $build += 1
    }

    $newVersion = "$major.$minor.$build.$revision"
    $newAssemblyInfoContent = [regex]::Replace($assemblyInfoContent, $regex, "[assembly: AssemblyInformationalVersion(`"$newVersion`")]")

    # Update AssemblyFileVersion and AssemblyVersion as well
    $newAssemblyInfoContent = $newAssemblyInfoContent -replace '(?<=AssemblyFileVersion\(")\d+(\.\d+){3}', $newVersion
    $newAssemblyInfoContent = $newAssemblyInfoContent -replace '(?<=AssemblyVersion\(")\d+(\.\d+){3}', $newVersion

    Set-Content $AssemblyInfoPath $newAssemblyInfoContent
}

# Set the version for dotnet pack
Write-Host "##vso[build.updatebuildnumber]$newVersion"
