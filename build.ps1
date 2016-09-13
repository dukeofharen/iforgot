# Set variables
$env:PATH = "${env:PATH}:;C:\Program Files (x86)\MSBuild\14.0\Bin;C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE;C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow;C:\Program Files (x86)\NSIS\Bin"
$releaseBinDir = Join-Path -Path $PSScriptRoot "IForgot\bin\Release"
$buildOutputDir = Join-Path -Path $PSScriptRoot "BuildOutput"
$buildOutputBinDir = Join-Path -Path $buildOutputDir "bin"
$nsiDir = Join-Path -Path $PSScriptRoot "installer"
$nsiPath = Join-Path -Path $nsiDir "installer.nsi"
$nsiExePath = Join-Path -Path $nsiDir "installer.exe"

# Remove bin and obj folders
Get-ChildItem .\ -include bin,obj -Recurse | foreach ($_) { remove-item $_.fullname -Force -Recurse }

# Remove BuildOutputDir
if(Test-Path $buildOutputDir)
{
    Write-Host "Deleting $buildOutputDir"
    Remove-Item $buildOutputDir -Force -Recurse
}

# Create folders
if(!(Test-Path $buildOutputDir))
{
    Write-Host "Creating $buildOutputDir"
    New-Item -ItemType directory -Path $buildOutputDir
}
if(!(Test-Path $buildOutputBinDir))
{
    Write-Host "Creating $buildOutputBinDir"
    New-Item -ItemType directory -Path $buildOutputBinDir
}
# TODO Patch version

# Build solution
& msbuild IForgot.sln /p:Configuration=Release

# Copy build output
Write-Host "Copying build output from $releaseBinDir to $buildOutputBinDir"
Get-Childitem $releaseBinDir -Recurse -Filter "*.exe" | Copy-Item -Destination $buildOutputBinDir
Get-Childitem $releaseBinDir -Recurse -Filter "*.dll" | Copy-Item -Destination $buildOutputBinDir

# Rename .exe file
Write-Host "Renaming .exe file"
Rename-Item (Join-Path -Path $buildOutputBinDir "IForgot.exe") "iforgot.exe"

# TODO Zip built files

# Build installer
Write-Host "Building installer $nsiPath"
& makensis $nsiPath

# Moving installer file
Write-Host "Moving installer file to $buildOutputDir"
Move-Item $nsiExePath $buildOutputDir