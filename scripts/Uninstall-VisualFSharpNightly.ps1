[CmdletBinding()]
param (
    [Parameter()] [string] $InstanceId,
    [Parameter()] [string] $Version,
    [switch] $Quiet
)

$vs = Get-VSSetupInstance
[array]$argumentList = '/u:VisualFSharp'
if(-not $InstanceId){
    $InstanceId = $vs.InstanceId
}
$argumentList += "/instanceIds:$InstanceId"
if($Quiet.IsPresent){
    $argumentList += "/quiet"
}
Start-Process (Join-Path $vs.InstallationPath 'Common7\IDE\VSIXInstaller.exe') -ArgumentList $argumentList -Wait