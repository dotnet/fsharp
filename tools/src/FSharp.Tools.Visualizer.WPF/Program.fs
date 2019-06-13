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

    let createTestModules name amount =
        [
            for i = 1 to amount do
                yield
                    sprintf
                    """
module TestModule%i =

    type %s () =

                    member val X = 1

                    member val Y = 2

                    member val Z = 3
                    
    let testFunction (x: %s) =
                    x.X + x.Y + x.Z
                    """ i name name
        ]
        |> List.reduce (+)

    let createSource name amount =
        (sprintf """namespace Test.%s""" name) + createTestModules name amount

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
                if String.Equals (Path.GetExtension filePath, ".dll", StringComparison.OrdinalIgnoreCase) && currentReferencedAssemblies.Contains filePath then
                    Some (PortableExecutableReference.CreateFromFile filePath)
                else
                    None
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

// will be important: https://www.codeproject.com/Articles/161871/Fast-Colored-TextBox-for-syntax-highlighting-2
[<AutoOpen>]
module rec App =

    type Model =
        {
            FileMenuHeader: string
            WillExit: bool
            Compilation: FSharpCompilation
            Text: SourceText
            CancellationTokenSource: CancellationTokenSource
        }

        static member Default =
            let text = SourceText.From("")
            {
                FileMenuHeader = "_File"
                WillExit = false
                Compilation = createCompilation text
                Text = text
                CancellationTokenSource = new CancellationTokenSource ()
            }

    type Msg =
        | Exit
        | UpdateText of SourceText * (Model -> unit)
        | UpdateSyntaxVisualizer of FSharpSyntaxTree

    let exitMenuItemView dispatch = MenuItem.MenuItem ("_Exit", [], fun _ -> dispatch Exit)

    let update msg model =
        match msg with
        | Exit ->
            { model with
                WillExit = true
            }
        | UpdateText (text, callback) ->
            let textSnapshot = FSharpSourceSnapshot.FromText ("test1.fs", text)
            model.CancellationTokenSource.Cancel ()
            model.CancellationTokenSource.Dispose ()

            let updatedModel =
                { model with
                    Text = text
                    Compilation = model.Compilation.ReplaceSourceSnapshot textSnapshot
                    CancellationTokenSource = new CancellationTokenSource ()
                }
            callback updatedModel
            updatedModel
        | UpdateSyntaxVisualizer syntaxTree ->
            printfn "%A" (syntaxTree.GetDiagnostics ())
            model

    let view model dispatch =
        View.Common (
            View.DockPanel 
                [
                    View.Menu ([ MenuItem.MenuItem ("_File", [ exitMenuItemView dispatch ], fun _ -> ()) ], dockTop = true)
                    View.TextBox (true, fun text -> 
                        dispatch (UpdateText (text, fun updatedModel ->
                            let computation =
                                async {
                                    try
                                        use! _do = Async.OnCancel (fun () -> printfn "cancelled")
                                        do! Async.Sleep 150
                                        let stopwatch = System.Diagnostics.Stopwatch.StartNew ()
                                        let semanticModel = updatedModel.Compilation.GetSemanticModel "test1.fs"
                                        semanticModel.TryGetEnclosingSymbol (0, updatedModel.CancellationTokenSource.Token) |> ignore
                                        stopwatch.Stop ()
                                        printfn "type check: %A ms" stopwatch.Elapsed.TotalMilliseconds
                                        dispatch (UpdateSyntaxVisualizer (semanticModel.SyntaxTree))
                                    with
                                    | ex -> ()
                                }
                            Async.Start (computation, cancellationToken = updatedModel.CancellationTokenSource.Token)
                        ))
                    )
                    View.TreeView (
                        [
                            TreeViewItem.TreeViewItem("Level1", 
                                [
                                    TreeViewItem.TreeViewItem ("Level2", [], fun _ -> ())
                                ], fun _ -> ())
                        ]
                    )
                ],
            model.WillExit
        )


[<EntryPoint;STAThread>]
let main argv =
    let app = FSharpVisualizerApplication()
    app.Run(FrameworkWindow(app, Model.Default, update, view))
