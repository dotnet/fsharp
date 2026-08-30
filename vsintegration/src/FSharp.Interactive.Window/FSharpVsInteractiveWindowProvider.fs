// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.ComponentModel.Composition
open System.Diagnostics
open System.IO
open System.Runtime.InteropServices
open System.Threading

open Microsoft.VisualStudio
open Microsoft.VisualStudio.InteractiveWindow.Shell
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Utilities

open Microsoft.VisualStudio.FSharp.Interactive.Session

module internal InteractiveWindowGuids =

    /// Visual Studio persists the window's place in the layout under this, so it must not change
    /// once shipped.
    [<Literal>]
    let ToolWindowIdString = "6B0F0D9E-1B4A-4C4E-9E2D-6F3B2A5C7D18"

    let ToolWindowId = Guid ToolWindowIdString

    let FSharpLanguageServiceId = Guid "BC6DD5A5-D4D6-4dab-A00D-A51242DBAF1B"

    [<Literal>]
    let FSharpContentTypeName = "F#"

/// Reads the session settings the existing Tools, Options page writes.
module internal InteractiveHostOptionsFactory =

    let private hostDirectory () =
        match Path.GetDirectoryName(typeof<InteractiveHostClient>.Assembly.Location) with
        | null -> Environment.CurrentDirectory
        | directory -> directory

    let currentPlatform () =
        if SessionsProperties.fsiUseNetCore then NetCore
        elif RuntimeInformation.ProcessArchitecture = Architecture.Arm64 then NetFrameworkArm64
        elif SessionsProperties.useAnyCpuVersion then NetFramework64
        else NetFramework32

    let create platform =
        {
            Platform = platform
            HostDirectory = hostDirectory ()
            InitialWorkingDirectory = Environment.GetFolderPath Environment.SpecialFolder.UserProfile
            UserArguments = SessionsProperties.fsiArgs
            ShadowCopyReferences = SessionsProperties.fsiShadowCopy
            DebugMode = SessionsProperties.fsiDebugMode
            LanguageVersionPreview = SessionsProperties.fsiPreview
            UICultureLcid = Thread.CurrentThread.CurrentUICulture.LCID
        }

/// Creates and owns the F# Interactive tool window.
[<Export(typeof<FSharpVsInteractiveWindowProvider>)>]
[<Sealed>]
type internal FSharpVsInteractiveWindowProvider
    [<ImportingConstructor>]
    (windowFactory: IVsInteractiveWindowFactory, contentTypeRegistry: IContentTypeRegistryService) =

    let mutable window: IVsInteractiveWindow = null
    let mutable evaluator: FSharpInteractiveEvaluator option = None

    let captionFor (platform: InteractiveHostPlatform) =
        sprintf "%s (%s)" (VFSIstrings.SR.fsharpInteractive ()) platform.Description

    let setCaption platform =
        match box window with
        | :? ToolWindowPane as pane -> pane.Caption <- captionFor platform
        | _ -> ()

    let currentOptions () =
        InteractiveHostOptionsFactory.create (InteractiveHostOptionsFactory.currentPlatform ())

    member this.Create(instanceId: int) =
        let host = new InteractiveHostClient(Process.GetCurrentProcess().Id)
        let created = new FSharpInteractiveEvaluator(host, currentOptions, setCaption)
        evaluator <- Some created

        window <-
            windowFactory.Create(
                InteractiveWindowGuids.ToolWindowId,
                instanceId,
                captionFor (InteractiveHostOptionsFactory.currentPlatform ()),
                created,
                __VSCREATETOOLWIN.CTW_fForceCreate
            )

        window.SetLanguage(
            InteractiveWindowGuids.FSharpLanguageServiceId,
            contentTypeRegistry.GetContentType InteractiveWindowGuids.FSharpContentTypeName
        )

        let interactiveWindow = window.InteractiveWindow
        interactiveWindow.TextView.Closed.Add(fun _ -> (created :> IDisposable).Dispose())
        interactiveWindow.InitializeAsync() |> ignore
        window

    member this.Open(instanceId: int, focus: bool) =
        if isNull (box window) then
            this.Create instanceId |> ignore

        window.Show focus
        window

    member _.Window = window

    member _.Evaluator = evaluator
