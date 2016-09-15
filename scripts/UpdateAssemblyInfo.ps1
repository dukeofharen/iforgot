# http://www.luisrocha.net/2009/11/setting-assembly-version-with-windows.html
# http://blogs.msdn.com/b/dotnetinterop/archive/2008/04/21/powershell-script-to-batch-update-assemblyinfo-cs-with-new-version.aspx
# http://jake.murzy.com/post/3099699807/how-to-update-assembly-version-numbers-with-teamcity
# https://github.com/ferventcoder/this.Log/blob/master/build.ps1#L6-L19
# http://stackoverflow.com/questions/36200323/get-and-replace-assemblyversion-from-assemblyinfo-cs

function Update-SourceVersion($version)
{
    foreach ($o in $input) 
    {
        Write-Host "Updating  '$($o.FullName)' -> $Version"
        $assemblyPattern = '\[assembly: AssemblyVersion\("(.*)"\)\]'
        $filePattern = '\[assembly: AssemblyFileVersion\("(.*)"\)\]'
        (Get-Content $o.FullName) | ForEach-Object{
            if($_ -match $assemblyPattern)
            {
                '[assembly: AssemblyVersion("{0}")]' -f $version
            }
            elseif($_ -match $filePattern)
            {
                '[assembly: AssemblyFileVersion("{0}")]' -f $version
            }
            else
            {
                $_
            }
        } | Out-File $o.FullName -encoding UTF8 -force
    }
}

function Update-AllAssemblyInfoFiles ($path, $version)
{
    Write-Host "Searching '$path'"
   foreach ($file in "AssemblyInfo.cs", "AssemblyInfo.vb" ) 
   {
        get-childitem $path -recurse |? {$_.Name -eq $file} | Update-SourceVersion $version;
   }
}