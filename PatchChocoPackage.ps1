$version = (Get-Childitem env:app_version).Value
$nuspecFile = ".\choco\iforgot.nuspec"
$installPsFile = ".\choco\tools\chocolateyinstall.ps1"

(Get-Content $nuspecFile).replace('[VERSION]', $version) | Set-Content $nuspecFile
(Get-Content $installPsFile).replace('[VERSION]', $version) | Set-Content $installPsFile

