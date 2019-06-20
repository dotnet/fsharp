// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open FSharp.Tools.Visualizer.WPF
open FSharp.Compiler.Compilation

[<AutoOpen>]
module Helpers =

    let createCompilation (text: SourceText) =
        let sources =
            [
                ("test1.fs", text)
            ]
        let workspace = new AdhocWorkspace ()
        let temporaryStorage = workspace.Services.TemporaryStorage

        let sourceSnapshots =
            sources
            |> List.map (fun (filePath, sourceText) -> temporaryStorage.CreateFSharpSourceSnapshot (filePath, sourceText, CancellationToken.None))
            |> ImmutableArray.CreateRange

        let currentReferencedAssemblies =
            let asmLocations =
                AppDomain.CurrentDomain.GetAssemblies()
                |> Array.choose (fun asm -> 
                    if not asm.IsDynamic then
                        Some asm.Location
                    else
                        None
                )
            HashSet(asmLocations, StringComparer.OrdinalIgnoreCase)

        let metadataReferences =
            Directory.EnumerateFiles(Path.GetDirectoryName typeof<System.Object>.Assembly.Location)
            |> Seq.choose (fun filePath ->
                try
                    System.Reflection.AssemblyName.GetAssemblyName filePath |> ignore
                    if String.Equals (Path.GetExtension filePath, ".dll", StringComparison.OrdinalIgnoreCase) then
                        Some (PortableExecutableReference.CreateFromFile filePath)
                    else
                        None
                with
                | _ -> None
            )
            |> Seq.map (fun peReference -> FSharpMetadataReference.PortableExecutable peReference)
            |> ImmutableArray.CreateRange

        let fsharpCoreMetadataReference =
            PortableExecutableReference.CreateFromFile typeof<int list>.Assembly.Location
            |> FSharpMetadataReference.PortableExecutable

        let compilation = FSharpCompilation.Create ("""C:\test.dll""", """C:\""", sourceSnapshots, metadataReferences.Add fsharpCoreMetadataReference)
        let options =
            { compilation.Options with CommandLineArgs = ["--targetprofile:netcore"] }
        compilation.SetOptions options

[<AutoOpen>]
module rec App =

    type Model =
        {
            FileMenuHeader: string
            WillExit: bool
            Compilation: FSharpCompilation
            Text: SourceText
            CancellationTokenSource: CancellationTokenSource
            RootNode: FSharpSyntaxNode option
            Highlights: HighlightSpan list
            NodeHighlight: FSharpSyntaxNode option
            CompletionItems: CompletionItem list
            WillRedraw: bool
        }

        static member Default =
            let text = SourceText.From("")
            {
                FileMenuHeader = "_File"
                WillExit = false
                Compilation = createCompilation text
                Text = text
                CancellationTokenSource = new CancellationTokenSource ()
                RootNode = None
                Highlights = []
                NodeHighlight = None
                CompletionItems = []
                WillRedraw = true
            }

    type Msg =
        | Exit
        | ForceGC
        | UpdateText of SourceText * (Model -> unit)
        | UpdateLexicalAnalysis of lexicalHighlights: HighlightSpan list
        | UpdateSyntacticAnalysis
        | UpdateVisualizers of highlights: HighlightSpan list * CompletionItem list * rootNode: FSharpSyntaxNode
        | UpdateNodeHighlight of FSharpSyntaxNode

    let KeywordColor = Drawing.Color.FromArgb (86, 156, 214)
    let StringColor = Drawing.Color.FromArgb (214, 157, 133)

    let getLexicalHighlights (syntaxTree: FSharpSyntaxTree) ct =
        syntaxTree.GetTokens (ct = ct)
        |> Seq.choose (fun x ->
            if x.IsKeyword then
                Some (HighlightSpan (x.Span, KeywordColor, HighlightSpanKind.Foreground))
            elif x.IsString then
                Some (HighlightSpan (x.Span, StringColor, HighlightSpanKind.Foreground))
            else
                None
        )
        |> List.ofSeq

    let getLexicalAnalysis (model: Model) ct =
        let stopwatch = System.Diagnostics.Stopwatch.StartNew ()

        let syntaxTree = model.Compilation.GetSyntaxTree "test1.fs"
        let highlights = getLexicalHighlights syntaxTree ct

        stopwatch.Stop ()
        printfn "lexical analysis: %A ms" stopwatch.Elapsed.TotalMilliseconds
        highlights

    let getSyntacticAnalysis (model: Model) ct =
        let stopwatch = System.Diagnostics.Stopwatch.StartNew ()

        let syntaxTree = model.Compilation.GetSyntaxTree "test1.fs"
        let _rootNode = syntaxTree.GetRootNode ct

        stopwatch.Stop ()
        printfn "syntactic analysis: %A ms" stopwatch.Elapsed.TotalMilliseconds

    let getSemanticAnalysis (model: Model) didCompletionTrigger caretOffset ct =
        let stopwatch = System.Diagnostics.Stopwatch.StartNew ()

        let semanticModel = model.Compilation.GetSemanticModel "test1.fs"
        let diagnostics = semanticModel.Compilation.GetDiagnostics ct

        printfn "%A" diagnostics

        let highlights =
            diagnostics
            |> Seq.map (fun x ->
                let span = x.Location.SourceSpan
                let color =
                    match x.Severity with
                    | DiagnosticSeverity.Error -> Drawing.Color.Red
                    | _ -> Drawing.Color.LightGreen
                let kind = HighlightSpanKind.Underline
                HighlightSpan (span, color, kind)
            )
            |> List.ofSeq

        let completionItems =
            if didCompletionTrigger then
                semanticModel.LookupSymbols (caretOffset, ct)
                |> Seq.map (fun symbol ->
                    CompletionItem (symbol.Name)
                )
                |> List.ofSeq
            else
                []

        stopwatch.Stop ()
        printfn "semantic analysis: %A ms" stopwatch.Elapsed.TotalMilliseconds

        (highlights, completionItems, semanticModel.SyntaxTree.GetRootNode ct)

    let update msg model =
        match msg with
        | Exit ->
            { model with
                WillExit = true
                WillRedraw = false
            }
        | ForceGC ->
            GC.Collect ()
            model
        | UpdateText (text, callback) ->
            let textSnapshot = FSharpSourceSnapshot.FromText ("test1.fs", text)
            model.CancellationTokenSource.Cancel ()
            model.CancellationTokenSource.Dispose ()

            let compilation = model.Compilation.ReplaceSourceSnapshot textSnapshot

            let updatedModel =
                { model with
                    Text = text
                    Compilation = compilation
                    CancellationTokenSource = new CancellationTokenSource ()
                    NodeHighlight = None
                    Highlights = []
                    CompletionItems = []
                    WillRedraw = false
                }
            callback updatedModel
            updatedModel

        | UpdateSyntacticAnalysis ->
            model

        | UpdateLexicalAnalysis lexicalHighlights ->
            { model with
                Highlights = lexicalHighlights
                WillRedraw = true
            }           

        | UpdateVisualizers (highlights, completionItems, rootNode) ->  
            { model with 
                RootNode = Some rootNode
                Highlights = model.Highlights @ highlights
                CompletionItems = completionItems
                WillRedraw = true
            }

        | UpdateNodeHighlight node ->
            { model with NodeHighlight = Some node; WillRedraw = true }

    let ForceGCMenuItemView dispatch = MenuItem.MenuItem ("_Force GC", [], fun _ -> dispatch ForceGC)
    let exitMenuItemView dispatch = MenuItem.MenuItem ("_Exit", [], fun _ -> dispatch Exit)

    let view model dispatch =
        let treeItems = []

        let otherHighlights =
            match model.NodeHighlight with
            | Some node ->
                [ HighlightSpan(node.Span, Drawing.Color.Gray, HighlightSpanKind.Background) ]
            | _ -> []

        let onTextChanged =
            fun (text, caretOffset, didCompletionTrigger) -> 
                dispatch (UpdateText (text, fun updatedModel ->
                    let computation =
                        async {
                            try
                                use! _do = Async.OnCancel (fun () -> printfn "cancelled")
                                let! ct = Async.CancellationToken

                                do! Async.Sleep 50
                                let s = System.Diagnostics.Stopwatch.StartNew ()
                                let lexicalAnalysis = getLexicalAnalysis updatedModel ct
                                s.Stop ()
                                let sleep = 50 - int s.ElapsedMilliseconds
                                if sleep > 0 then
                                    do! Async.Sleep sleep

                                dispatch (UpdateLexicalAnalysis lexicalAnalysis)

                                getSyntacticAnalysis updatedModel ct
                                dispatch (UpdateSyntacticAnalysis)

                                let highlights, completionItems, rootNode = getSemanticAnalysis updatedModel didCompletionTrigger caretOffset ct
                                dispatch (UpdateVisualizers (highlights, completionItems, rootNode))
                            with
                            | :? OperationCanceledException -> ()
                            | ex ->
                                printfn "%A" ex.Message
                        }
                    Async.Start (computation, cancellationToken = updatedModel.CancellationTokenSource.Token)
                ))

        let onCompletionTriggered =
            fun (text, caretOffset) -> ()

        View.Common (
            View.DockPanel 
                [
                    View.Menu ([ MenuItem.MenuItem ("_File", [ ForceGCMenuItemView dispatch; exitMenuItemView dispatch ], fun _ -> ()) ], dockTop = true)
                    View.Editor (model.Highlights, model.WillRedraw, model.CompletionItems, onTextChanged, onCompletionTriggered)
                    View.TreeView (treeItems)
                ],
            model.WillExit
        )


[<EntryPoint;STAThread>]
let main argv =
    let app = FSharpVisualizerApplication()
    app.Run(FrameworkWindow(app, Model.Default, update, view))
