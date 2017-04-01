# run this in PowerShell as Administrator
# it will launch Visual Studio Installer and prompt you to add
# all artifacts required to build Visual F#

if (-not (Get-Module -ListAvailable -Name VSSetup)) {
    Install-Module VSSetup -Scope CurrentUser
}
$vs = Get-VSSetupInstance

# Microsoft.Net.Component.4.6.2.SDK: .NET Framework 4.6.2 SDK
# Microsoft.Net.Core.Component.SDK: .NET Core 1.0 - 1.1 development tools
# Microsoft.Component.NetFX.Core.Runtime: .NET Core runtime
# Microsoft.VisualStudio.Component.FSharp: F# language support
# Microsoft.VisualStudio.Component.VC.ATL: Visual C++ ATL support
# Microsoft.VisualStudio.Component.VSSDK: Visual Studio SDK
# Microsoft.Component.MSBuild: MSBuild
# Microsoft.VisualStudio.Component.NuGet: NuGet package manager
# Microsoft.VisualStudio.Component.VC.Tools.x86.x64: VC++ 2017 v141 toolset (x86,x64)
# Microsoft.VisualStudio.Component.Windows10SDK.14393: Windows 10 SDK (10.0.14393.0)
# Microsoft.VisualStudio.Component.Windows81SDK: Windows 8.1 SDK
# Microsoft.Net.Component.4.6.TargetingPack: .NET Framework 4.6 targeting pack
# Microsoft.Net.Component.4.5.1.TargetingPack: .NET Framework 4.5.1 targeting pack
# Microsoft.Net.Component.4.6.2.TargetingPack: .NET Framework 4.6.2 targeting pack

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
    --add Microsoft.Net.Component.4.6.TargetingPack `
    --add Microsoft.Net.Component.4.5.1.TargetingPack `
    --add Microsoft.Net.Component.4.6.2.TargetingPack