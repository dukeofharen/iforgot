$configuration = (Get-Childitem env:CONFIGURATION).Value
Rename-Item "IForgot\bin\$configuration\IForgot.exe" iforgot.exe
Rename-Item "IForgot\bin\$configuration\IForgot.exe.config" App.config

Get-Childitem . -recurse -include *.xml -force | Remove-Item
Get-Childitem . -recurse -include *.pdb -force | Remove-Item