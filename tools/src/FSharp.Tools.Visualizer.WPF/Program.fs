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
            OldText: SourceText
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
                OldText = text
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
        | UpdateText of SourceText * (Model -> unit)
        | UpdateLexicalAnalysis of lexicalHighlights: HighlightSpan list
        | UpdateVisualizers of didCompletionTrigger: bool * caretOffset: int * CancellationToken
        | UpdateNodeHighlight of FSharpSyntaxNode

    let KeywordColor = Drawing.Color.FromArgb (86, 156, 214)

    let getLexicalHighlights (syntaxTree: FSharpSyntaxTree) ct =
        syntaxTree.GetTokens (ct = ct)
        |> Seq.choose (fun x ->
            if x.IsKeyword then
                Some (HighlightSpan (x.Span, KeywordColor, HighlightSpanKind.Foreground))
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

    let update msg model =
        match msg with
        | Exit ->
            { model with
                WillExit = true
                WillRedraw = false
            }
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
                    WillRedraw = false
                }
            callback updatedModel
            updatedModel

        | UpdateLexicalAnalysis lexicalHighlights ->
            { model with
                Highlights = lexicalHighlights
                WillRedraw = true
            }           

        | UpdateVisualizers (didCompletionTrigger, caretOffset, ct) ->  
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

            let symbolInfoOpt =
                let textChanges = model.Text.GetTextChanges model.OldText
                if textChanges.Count = 1 then
                    match semanticModel.SyntaxTree.GetRootNode(ct).TryFindNode(textChanges.[0].Span) with
                    | Some node ->
                        match node.GetAncestorsAndSelf () |> Seq.tryFind (fun x -> match x.Kind with FSharpSyntaxNodeKind.Expr _ -> true | _ -> false) with
                        | Some node -> Some node
                        | _ -> None
                    | _ ->
                        None
                else
                    None
                |> Option.map (fun node ->
                    [ HighlightSpan (node.Span, Drawing.Color.DarkGreen, HighlightSpanKind.Background) ]
                )
                |> Option.defaultValue []


            let completionItems =
                if didCompletionTrigger then
                    semanticModel.LookupSymbols (caretOffset, ct)
                    |> Seq.map (fun symbol ->
                        CompletionItem (symbol.Name)
                    )
                    |> List.ofSeq
                else
                    []

            { model with 
                OldText = model.Text
                RootNode = Some (semanticModel.SyntaxTree.GetRootNode CancellationToken.None)
                Highlights = model.Highlights @ highlights
                CompletionItems = completionItems
                WillRedraw = true
            }

        | UpdateNodeHighlight node ->
            { model with NodeHighlight = Some node; WillRedraw = true }

    let exitMenuItemView dispatch = MenuItem.MenuItem ("_Exit", [], fun _ -> dispatch Exit)

    let view model dispatch =
        let treeItems = []
            //match model.RootNode with
            //| None -> []
            //| Some rootNode ->
            //    let rec getTreeItem (node: FSharpSyntaxNode) =
            //        let nested =
            //            node.GetChildren ()
            //            |> Seq.map (fun childNode ->
            //                getTreeItem childNode
            //            )
            //            |> List.ofSeq
            //        TreeViewItem.TreeViewItem(string node, nested, fun () -> dispatch (UpdateNodeHighlight node))
            //    [ getTreeItem rootNode ]

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
                                let lexicalAnalysis = getLexicalAnalysis updatedModel ct
                                dispatch (UpdateLexicalAnalysis lexicalAnalysis)
                              //  dispatch (UpdateVisualizers (didCompletionTrigger, caretOffset, ct))
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
                    View.Menu ([ MenuItem.MenuItem ("_File", [ exitMenuItemView dispatch ], fun _ -> ()) ], dockTop = true)
                    View.Editor (model.Highlights @ otherHighlights, model.WillRedraw, model.CompletionItems, onTextChanged, onCompletionTriggered)
                    View.TreeView (treeItems)
                ],
            model.WillExit
        )


[<EntryPoint;STAThread>]
let main argv =
    let app = FSharpVisualizerApplication()
    app.Run(FrameworkWindow(app, Model.Default, update, view))
