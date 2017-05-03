# run this in PowerShell as Administrator
# it will launch Visual Studio Installer and prompt you to add
# all artifacts required to build Visual F#

if (-not (Get-Module -ListAvailable -Name VSSetup)) {
    Install-Module VSSetup -Scope CurrentUser
}
$vs = Get-VSSetupInstance

& "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vs_installershell.exe" modify `
    --installPath $vs.InstallationPath `
    --add Microsoft.Net.Component.4.6.2.SDK `
    --add Microsoft.Net.Core.Component.SDK `
    --add Microsoft.Component.NetFX.Core.Runtime `
    --add Microsoft.VisualStudio.Component.FSharp `
    --add Microsoft.VisualStudio.Component.VC.ATL `
    --add Microsoft.VisualStudio.Component.VSSDK `
    --add Microsoft.Component.MSBuild `
    --add Microsoft.VisualStudio.Component.NuGet `
    --add Microsoft.VisualStudio.Component.VC.Tools.x86.x64 `
    --add Microsoft.VisualStudio.Component.Windows10SDK.14393 `
    --add Microsoft.VisualStudio.Component.Windows81SDK `
    --add Microsoft.VisualStudio.Component.PortableLibrary `
    --add Microsoft.Net.Component.4.TargetingPack `
    --add Microsoft.Net.Component.4.5.TargetingPack `
    --add Microsoft.Net.Component.4.5.1.TargetingPack `
    --add Microsoft.Net.Component.4.6.TargetingPack `
    --add Microsoft.Net.Component.4.6.1.TargetingPack `
    --add Microsoft.Net.Component.4.6.2.TargetingPack `
