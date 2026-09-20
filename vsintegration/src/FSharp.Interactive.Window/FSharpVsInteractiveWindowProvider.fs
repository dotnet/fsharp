// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Interactive

open System
open System.ComponentModel.Composition
open System.Diagnostics
open System.IO
open System.Threading

open Microsoft.VisualStudio
open Microsoft.VisualStudio.InteractiveWindow.Shell
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Utilities

open Microsoft.VisualStudio.FSharp.Interactive.Session

/// Identities the window is registered and addressed by. Public so that they are declared once and
/// referenced, rather than repeated by every component that needs them.
module InteractiveWindowGuids =

    /// Visual Studio persists the window's place in the layout under this, so it must not change
    /// once shipped.
    [<Literal>]
    let ToolWindowIdString = "6B0F0D9E-1B4A-4C4E-9E2D-6F3B2A5C7D18"

    let ToolWindowId = Guid ToolWindowIdString

    let FSharpLanguageServiceId = Guids.guidFsharpLanguageService

    /// FSharp.Editor declares this too, but it sits above this project and cannot be referenced
    /// from here, so the name is repeated rather than shared.
    [<Literal>]
    let FSharpContentTypeName = "F#"

/// Reads the session settings the existing Tools, Options page writes.
module internal InteractiveHostOptionsFactory =

    /// The folder `dotnet fsi` is run from: the open solution's, whose `global.json` then picks the
    /// SDK; the user's profile when nothing is open. Read on the UI thread, where the window's
    /// evaluator calls in.
    let private startDirectory () =
        let solutionDirectory =
            try
                match ServiceProvider.GlobalProvider.GetService typeof<SVsSolution> with
                | :? IVsSolution as solution ->
                    let hr, directory, _, _ = solution.GetSolutionInfo()

                    if
                        ErrorHandler.Succeeded hr
                        && not (String.IsNullOrEmpty directory)
                        && Directory.Exists directory
                    then
                        ValueSome directory
                    else
                        ValueNone
                | _ -> ValueNone
            with _ ->
                ValueNone

        match solutionDirectory with
        | ValueSome directory -> directory
        | ValueNone -> Environment.GetFolderPath Environment.SpecialFolder.UserProfile

    let create () =
        {
            InitialWorkingDirectory = startDirectory ()
            UserArguments = SessionsProperties.fsiArgs
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

    let mutable window: IVsInteractiveWindow | null = null
    let mutable evaluator: FSharpInteractiveEvaluator voption = ValueNone

    member this.Create(instanceId: int) =
        let host = new InteractiveHostClient(Process.GetCurrentProcess().Id)
        let created = new FSharpInteractiveEvaluator(host, InteractiveHostOptionsFactory.create)
        evaluator <- ValueSome created

        let toolWindow =
            windowFactory.Create(
                InteractiveWindowGuids.ToolWindowId,
                instanceId,
                VFSIstrings.SR.fsharpInteractive (),
                created,
                __VSCREATETOOLWIN.CTW_fForceCreate
            )

        window <- toolWindow

        toolWindow.SetLanguage(
            InteractiveWindowGuids.FSharpLanguageServiceId,
            contentTypeRegistry.GetContentType InteractiveWindowGuids.FSharpContentTypeName
        )

        let interactiveWindow = toolWindow.InteractiveWindow
        interactiveWindow.TextView.Closed.Add(fun _ -> (created :> IDisposable).Dispose())
        interactiveWindow.InitializeAsync() |> ignore
        toolWindow

    member this.Open(instanceId: int, focus: bool) =
        let toolWindow =
            match window with
            | null -> this.Create instanceId
            | existing -> existing

        toolWindow.Show focus
        toolWindow

    /// Send text an editor command picked up, showing the window without taking focus from the
    /// document the user is still typing in.
    member this.SubmitFromEditor(text: string, sourcePath: string, startLine: int) =
        let toolWindow = this.Open(0, focus = false)

        evaluator
        |> ValueOption.iter _.SetNextSubmissionOrigin(sourcePath, startLine)

        toolWindow.InteractiveWindow.SubmitAsync [| text |] |> ignore

    member _.Window = window

    member _.Evaluator = evaluator
