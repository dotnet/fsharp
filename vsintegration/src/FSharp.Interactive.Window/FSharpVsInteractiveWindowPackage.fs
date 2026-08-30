// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.Runtime.InteropServices
open System.Threading
open System.Threading.Tasks

open Microsoft.VisualStudio
open Microsoft.VisualStudio.ComponentModelHost
open Microsoft.VisualStudio.InteractiveWindow.Shell
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

module internal InteractiveWindowPackageGuids =

    [<Literal>]
    let PackageIdString = "F5C1B3D2-8E47-4A96-9C0B-1D7E4A2F6B39"

    /// Docks the window with the Output window, where the existing one appears.
    [<Literal>]
    let OutputWindowIdString = "34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3"

[<Guid(InteractiveWindowPackageGuids.PackageIdString)>]
[<PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)>]
[<ProvideInteractiveWindow(InteractiveWindowGuids.ToolWindowIdString,
                           Orientation = ToolWindowOrientation.Bottom,
                           Style = VsDockStyle.Tabbed,
                           Window = InteractiveWindowPackageGuids.OutputWindowIdString)>]
type internal FSharpVsInteractiveWindowPackage() as this =
    inherit AsyncPackage()

    let mutable provider: FSharpVsInteractiveWindowProvider voption = ValueNone

    let getProvider () =
        match provider with
        | ValueSome provider -> ValueSome provider
        | ValueNone ->
            match this.GetService(typeof<SComponentModel>) with
            | :? IComponentModel as components ->
                let resolved =
                    components.DefaultExportProvider.GetExportedValue<FSharpVsInteractiveWindowProvider>()

                provider <- ValueSome resolved
                ValueSome resolved
            | _ -> ValueNone

    member _.Provider = getProvider ()

    override _.InitializeAsync(cancellationToken: CancellationToken, progress: IProgress<ServiceProgressData>) =
        base.InitializeAsync(cancellationToken, progress)

    interface IVsToolWindowFactory with

        /// Called when Visual Studio restores the window from a persisted layout.
        member _.CreateToolWindow(toolWindowType: byref<Guid>, id: uint32) =
            if toolWindowType = InteractiveWindowGuids.ToolWindowId then
                match getProvider () with
                | ValueSome provider ->
                    provider.Create(int id) |> ignore
                    VSConstants.S_OK
                | ValueNone -> VSConstants.E_FAIL
            else
                VSConstants.E_FAIL
