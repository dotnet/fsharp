# installs a nightly VisualFSharp build
# the latest version is installed if not specified

# https://blogs.msdn.microsoft.com/dotnet/2017/03/14/announcing-nightly-releases-for-the-visual-f-tools/
# https://dotnet.myget.org/feed/fsharp/package/vsix/VisualFSharp

# requies Visual Studio Setup PowerShell Module
# https://github.com/Microsoft/vssetup.powershell
# Install-Module VSSetup -Scope CurrentUser

[CmdletBinding()]
param (
    [Parameter()] [string] $InstanceId,
    [Parameter()] [string] $Version,
    [switch] $Quiet
)

$feed = 'https://dotnet.myget.org/F/fsharp/vsix'
$wc = New-Object Net.Webclient
if(-not $Version){
    $Version = ($wc.DownloadString($feed) -as [xml]).feed.entry.Vsix.Version
}
$file = 'VisualFSharp-' + $Version + '.vsix'
$tempfile = [IO.Path]::Combine([IO.Path]::GetTempPath(), $file)
$logFile = [IO.Path]::ChangeExtension($tempfile, '.txt')
$wc.DownloadFile($feed + '/' + $file, $tempfile)
$vs = Get-VSSetupInstance
[array]$argumentList = "/logFile:`"$logFile`""
$argumentList += "/appidinstallpath:`"$($vs.InstallationPath)\Common7\IDE\devenv.exe`""
$skuName = $vs.Product.Id.Replace("Microsoft.VisualStudio.Product.","")
$argumentList += "/skuName:$skuName"
$argumentList += "/skuVersion:`"$($vs.Product.Version)`""
$argumentList += "/appidname:`"Microsoft $($vs.DisplayName)`""
if(-not $InstanceId){
    $InstanceId = $vs.InstanceId
}
$argumentList += "/instanceIds:$InstanceId"
if($Quiet.IsPresent){
    $argumentList += "/quiet"
}
$argumentList += $tempfile
Start-Process (Join-Path $vs.InstallationPath 'Common7\IDE\VSIXInstaller.exe') -Wait -ArgumentList $argumentList
Remove-Item $tempfile