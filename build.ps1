. "scripts\UpdateAssemblyInfo.ps1"
Add-Type -assembly "system.io.compression.filesystem"

# Set variables
$env:PATH = "${env:PATH}:;C:\Program Files (x86)\MSBuild\14.0\Bin;C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE;C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow;C:\Program Files (x86)\NSIS\Bin"
$releaseBinDir = Join-Path -Path $PSScriptRoot "IForgot\bin\Release"
$solutionDir = Join-Path -Path $PSScriptRoot "IForgot"
$nsiDir = Join-Path -Path $PSScriptRoot "installer"
$nsiPath = Join-Path -Path $nsiDir "installer.nsi"
$nsiExePath = Join-Path -Path $nsiDir "installer.exe"
$nugetLocation = Join-Path -Path $PSScriptRoot "nuget.exe"
$metaPath = Join-Path -Path $PSScriptRoot "meta.xml"
[xml]$meta = Get-Content $metaPath
$version = [version]$meta.Settings.Version
$buildOutputDir = Join-Path -Path $PSScriptRoot "BuildOutput"

# Remove bin and obj folders
Get-ChildItem .\ -include bin,obj -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }

# Create folders
if(!(Test-Path $buildOutputDir))
{
    Write-Host "Creating $buildOutputDir"
    New-Item -ItemType directory -Path $buildOutputDir
}

# Patch version
$buildNumber = (Get-Date).ToString("yyMMdd")
$versionString = "{0}.{1}.{2}.{3}" -f $version.Major, $version.Minor, $version.Build, ($version.Revision + 1)
$meta.Settings.Version = $versionString
$meta.Save($metaPath)
Write-Host "Updating version numbers to $versionString"
Update-AllAssemblyInfoFiles $solutionDir $versionString

# Create sub folders
$buildOutputSubDir = Join-Path -Path $buildOutputDir "IForgot_$versionString"
$buildOutputBinDir = Join-Path -Path $buildOutputSubDir "bin"
$buildOutputZipPath = Join-Path -Path $buildOutputSubDir "app.zip"
if(!(Test-Path $buildOutputSubDir))
{
    Write-Host "Creating $buildOutputSubDir"
    New-Item -ItemType directory -Path $buildOutputSubDir
}
if(!(Test-Path $buildOutputBinDir))
{
    Write-Host "Creating $buildOutputBinDir"
    New-Item -ItemType directory -Path $buildOutputBinDir
}

# NuGet restore
& $nugetLocation restore

# Build solution
& msbuild IForgot.sln /p:Configuration=Release

# Copy build output
Write-Host "Copying build output from $releaseBinDir to $buildOutputBinDir"
Get-Childitem $releaseBinDir -Recurse -Filter "*.exe" | Copy-Item -Destination $buildOutputBinDir
Get-Childitem $releaseBinDir -Recurse -Filter "*.dll" | Copy-Item -Destination $buildOutputBinDir

# Rename .exe file
Write-Host "Renaming .exe file"
Rename-Item (Join-Path -Path $buildOutputBinDir "IForgot.exe") "iforgot.exe"

# Build installer
$env:VersionMajor = $version.Major
$env:VersionMinor = $version.Minor
$env:VersionBuild = $version.Build
$env:BuildOutputDirectory = $buildOutputBinDir

Write-Host "Building installer $nsiPath"
& makensis $nsiPath

# Moving installer file
Write-Host "Moving installer file to $buildOutputSubDir"
Move-Item $nsiExePath $buildOutputSubDir

# Zip built files
Write-Host "Zipping $buildOutputBinDir to $buildOutputZipPath"
[io.compression.zipfile]::CreateFromDirectory($buildOutputBinDir, $buildOutputZipPath)
Write-Host "Deleting $buildOutputBinDir"
Remove-Item $buildOutputBinDir -Force -Recurse