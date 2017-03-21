Install-Module VSSetup -Scope CurrentUser -Force
$vs = Get-VSSetupInstance
$vs | fl *