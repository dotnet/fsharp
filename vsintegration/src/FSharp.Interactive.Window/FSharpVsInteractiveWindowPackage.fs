// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.Runtime.InteropServices

open Microsoft.VisualStudio
open Microsoft.VisualStudio.ComponentModelHost
open Microsoft.VisualStudio.InteractiveWindow.Shell
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

module internal InteractiveWindowPackageGuids =

    [<Literal>]
    let PackageIdString = "F5C1B3D2-8E47-4A96-9C0B-1D7E4A2F6B39"

    [<Literal>]
    let OutputWindowIdString = "34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3"

[<Guid(InteractiveWindowPackageGuids.PackageIdString)>]
[<PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)>]
[<ProvideInteractiveWindow(InteractiveWindowGuids.ToolWindowIdString,
                           Orientation = ToolWindowOrientation.Bottom,
                           Style = VsDockStyle.Tabbed,
                           Window = InteractiveWindowPackageGuids.OutputWindowIdString)>]
type internal FSharpVsInteractiveWindowPackage() =
    inherit AsyncPackage()

    interface IVsToolWindowFactory with

        member this.CreateToolWindow(toolWindowType: byref<Guid>, id: uint32) =
            if toolWindowType <> InteractiveWindowGuids.ToolWindowId then
                VSConstants.E_FAIL
            else
                match this.GetService typeof<SComponentModel> with
                | :? IComponentModel as components ->
                    components.DefaultExportProvider.GetExportedValue<FSharpVsInteractiveWindowProvider>().Create(int id)
                    |> ignore

                    VSConstants.S_OK
                | _ -> VSConstants.E_FAIL
