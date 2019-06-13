// Learn more about F# at http://fsharp.org

open System
open FSharp.Tools.Visualizer.WPF

// will be important: https://www.codeproject.com/Articles/161871/Fast-Colored-TextBox-for-syntax-highlighting-2
[<AutoOpen>]
module rec App =

    type Model =
        {
            FileMenuHeader: string
            WillExit: bool
        }

        static member Default =
            {
                FileMenuHeader = "_File"
                WillExit = false
            }

    type Msg =
        | Exit

    let exitMenuItemView dispatch = MenuItem.MenuItem ("_Exit", [], fun _ -> dispatch Exit)

    let update msg model =
        match msg with
        | Exit ->
            { model with
                WillExit = true
            }

    let view model dispatch =
        View.Common (
            View.DockPanel 
                [
                    View.Menu ([ MenuItem.MenuItem ("_File", [ exitMenuItemView dispatch ], fun _ -> ()) ], dockTop = true)
                    View.TextBox (true, fun _ -> ())
                    View.TreeView
                ],
            model.WillExit
        )


[<EntryPoint;STAThread>]
let main argv =
    let app = FSharpVisualizerApplication()
    app.Run(FrameworkWindow(app, Model.Default, update, view))
