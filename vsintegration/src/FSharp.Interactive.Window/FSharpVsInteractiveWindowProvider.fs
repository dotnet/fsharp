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

module InteractiveWindowGuids =

    /// Persisted in users' layouts: never change.
    [<Literal>]
    let ToolWindowIdString = "6B0F0D9E-1B4A-4C4E-9E2D-6F3B2A5C7D18"

    let ToolWindowId = Guid ToolWindowIdString

    let FSharpLanguageServiceId = Guids.guidFsharpLanguageService

    /// Must equal FSharpConstants.FSharpContentTypeName (FSharp.Editor).
    [<Literal>]
    let FSharpContentTypeName = "F#"

module internal InteractiveHostOptionsFactory =

    let private solutionDirectory () =
        ThreadHelper.ThrowIfNotOnUIThread()

        try
            match ServiceProvider.GlobalProvider.GetService typeof<SVsSolution> with
            | :? IVsSolution as solution ->
                match solution.GetSolutionInfo() with
                | hr, directory, _, _ when ErrorHandler.Succeeded hr && Directory.Exists directory -> ValueSome directory
                | _ -> ValueNone
            | _ -> ValueNone
        with _ ->
            ValueNone

    let create () =
        {
            InitialWorkingDirectory =
                solutionDirectory ()
                |> ValueOption.defaultWith (fun () -> Environment.GetFolderPath Environment.SpecialFolder.UserProfile)
            UserArguments = SessionsProperties.fsiArgs
            DebugMode = SessionsProperties.fsiDebugMode
            LanguageVersionPreview = SessionsProperties.fsiPreview
            UICultureLcid = Thread.CurrentThread.CurrentUICulture.LCID
        }

[<Export(typeof<FSharpVsInteractiveWindowProvider>)>]
[<Sealed>]
type internal FSharpVsInteractiveWindowProvider
    [<ImportingConstructor>]
    (windowFactory: IVsInteractiveWindowFactory, contentTypeRegistry: IContentTypeRegistryService, scanners: ILexicalScannerFactory) =

    let mutable window: IVsInteractiveWindow | null = null
    let mutable evaluator: FSharpInteractiveEvaluator voption = ValueNone

    member _.Create(instanceId: int) =
        let host = new InteractiveHostClient(Process.GetCurrentProcess().Id)

        let created =
            new FSharpInteractiveEvaluator(host, InteractiveHostOptionsFactory.create, scanners)

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

        interactiveWindow.TextView.Closed.Add(fun _ ->
            (created :> IDisposable).Dispose()

            if obj.ReferenceEquals(window, toolWindow) then
                window <- null

            match evaluator with
            | ValueSome current when obj.ReferenceEquals(current, created) -> evaluator <- ValueNone
            | _ -> ())

        interactiveWindow.InitializeAsync() |> ignore
        toolWindow

    member this.Open(instanceId: int, focus: bool) =
        let toolWindow =
            match window with
            | null -> this.Create instanceId
            | existing -> existing

        toolWindow.Show focus
        toolWindow

    member this.SubmitFromEditor(text: string, sourcePath: string, startLine: int) =
        let toolWindow = this.Open(0, focus = false)

        evaluator |> ValueOption.iter _.AddSubmissionOrigin(text, sourcePath, startLine)

        toolWindow.InteractiveWindow.SubmitAsync [| text |] |> ignore
