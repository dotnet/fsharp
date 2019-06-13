namespace FSharp.Tools.Visualizer.WPF

open System
open System.Collections
open System.Collections.Generic
open System.Collections.Concurrent
open Microsoft.CodeAnalysis.Text

[<AutoOpen>]
module rec Virtual =

    [<RequireQualifiedAccess>]
    type Command =
        | Nop
        | Update of (unit -> View)

    [<RequireQualifiedAccess>]
    type DataGridColumn = 
        | Text of header: string * bindingName: string

    [<RequireQualifiedAccess>]
    type MenuItem = MenuItem of header: string * nested: MenuItem list * onClick: (unit -> unit)

    [<RequireQualifiedAccess>]
    type View =
        | Empty
        | Common of View * willExit: bool
        | DockPanel of View list
        | DataGrid of columns: DataGridColumn list * data: IEnumerable
        | Menu of MenuItem list * dockTop: bool
        | TextBox of acceptsReturn: bool * onTextChanged: (SourceText -> unit)

[<Sealed>]
type internal ReferenceEqualityComparer () =
    
    interface IEqualityComparer<obj> with

        member __.GetHashCode value =            
            System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode value

        member __.Equals (left, right) =
            obj.ReferenceEquals (left, right)

[<Sealed>]
type internal WpfGlobals (app: System.Windows.Application, uiThread) =

    let dispatchCommandsCalledEvent = Event<Command list>()
    
    member __.Application = app

    member val Events = Dictionary<obj, obj>()
    member val EventSubscriptions = Dictionary<obj, IDisposable>(ReferenceEqualityComparer())

    member __.DispatchCommands (commands: Command list) = 
        uiThread (fun () -> dispatchCommandsCalledEvent.Trigger commands)

    member __.DispatchCommandsCalled = dispatchCommandsCalledEvent.Publish

[<AutoOpen>]
module internal Helpers =

    type System.Windows.Documents.FlowDocument with
    
        member this.GetCharIndexAtOffset (offset: int) =
            let pointerToStop = this.ContentStart.GetPositionAtOffset offset
            let mutable nextPointer = this.ContentStart
            let mutable count = 0
            while nextPointer <> null && this.ContentStart.GetOffsetToPosition(nextPointer) < offset do
                if nextPointer.CompareTo(this.ContentEnd) = 0 then
                    nextPointer <- null
                else
                    if nextPointer.GetPointerContext(Windows.Documents.LogicalDirection.Forward) = Windows.Documents.TextPointerContext.Text then
                        nextPointer <- nextPointer.GetNextInsertionPosition(Windows.Documents.LogicalDirection.Forward)
                        count <- count + 1
                    else
                        nextPointer <- nextPointer.GetNextInsertionPosition(Windows.Documents.LogicalDirection.Forward)
            count

    let diffSeq (state: 'T seq) (newState: 'T seq) create update removeAt clear =
        let mutable lastIndex = -1
        (state, newState)
        ||> Seq.iteri2 (fun i item newItem ->
            update i item newItem
            lastIndex <- i
        )

        let stateCount = Seq.length state
        let newStateCount = Seq.length newState

        if lastIndex = -1 && stateCount <> 0 then
            clear ()

        if lastIndex <> -1 && stateCount - 1 > lastIndex then
            for i = lastIndex + 1 to stateCount - 1 do
                removeAt i

        if newStateCount - 1 > lastIndex then
            newState
            |> Seq.skip (lastIndex + 1)
            |> Seq.iter (fun newItem -> create newItem)

    let rec updateUIElement (g: WpfGlobals) (wpfUIElement: System.Windows.UIElement) (view: View) (newView: View) : System.Windows.UIElement =
        match newView with
        | View.Empty -> null

        | View.Common (newCommonView, willExit) ->
            if willExit then
                g.Application.Shutdown ()

            match view with
            | View.Common (commonView, _) ->
                updateUIElement g wpfUIElement commonView newCommonView
            | _ ->
                updateUIElement g wpfUIElement view newCommonView

        | View.DockPanel newChildViews ->
            let wpfDockPanel, oldChildViews =
                match view with
                | View.DockPanel oldChildViews -> (wpfUIElement :?> System.Windows.Controls.DockPanel, oldChildViews)
                | _ ->  System.Windows.Controls.DockPanel (), []
                
            diffSeq oldChildViews newChildViews
                (fun newChild -> 
                    wpfDockPanel.Children.Add (updateUIElement g null View.Empty newChild) |> ignore
                )
                (fun i oldChild newChild -> 
                    let wpfChild = wpfDockPanel.Children.[i]
                    let updatedWpfChild = updateUIElement g wpfChild oldChild newChild
                    if not (obj.ReferenceEquals (wpfChild, updatedWpfChild)) then
                        wpfDockPanel.Children.RemoveAt i
                        wpfDockPanel.Children.Insert (i, updatedWpfChild)
                )
                wpfDockPanel.Children.RemoveAt
                wpfDockPanel.Children.Clear

            wpfDockPanel :> System.Windows.UIElement

        | View.DataGrid (newColumns, newData) ->
            let wpfDataGrid, oldColumns, oldData =
                match view with
                | View.DataGrid (oldColumns, oldData) -> (wpfUIElement :?> System.Windows.Controls.DataGrid, oldColumns, oldData)
                | _ -> System.Windows.Controls.DataGrid (AutoGenerateColumns = false, IsReadOnly = true), [], [] :> IEnumerable

            diffSeq oldColumns newColumns
                (fun newColumn ->
                    match newColumn with
                    | DataGridColumn.Text (header, bindingName) ->
                        System.Windows.Controls.DataGridTextColumn(Header = header, Binding = System.Windows.Data.Binding (bindingName))
                        |> wpfDataGrid.Columns.Add
                )
                (fun i oldColumn newColumn ->
                    let wpfColumn = wpfDataGrid.Columns.[i]
                    match oldColumn, newColumn, wpfColumn with
                    | DataGridColumn.Text (oldHeader, oldBindingName), DataGridColumn.Text (newHeader, newBindingName), (:? System.Windows.Controls.DataGridTextColumn as wpfColumn) ->
                        if oldHeader <> newHeader then
                            wpfColumn.Header <- newHeader

                        if oldBindingName <> newBindingName then
                            wpfColumn.Binding <- System.Windows.Data.Binding (newBindingName)
                    | _ -> failwith "not supported yet"
                )
                wpfDataGrid.Columns.RemoveAt
                wpfDataGrid.Columns.Clear

            if not (obj.ReferenceEquals (oldData, newData)) then
                wpfDataGrid.ItemsSource <- newData

            wpfDataGrid :> System.Windows.UIElement

        | View.Menu (newMenuItems, newDockTop) ->
            let wpfMenu, oldMenuItems, oldDockTop =
                match view with
                | View.Menu (oldMenuItems, oldDockTop) -> (wpfUIElement :?> System.Windows.Controls.Menu, oldMenuItems, oldDockTop)
                | _ -> (System.Windows.Controls.Menu (), [], false)

            if oldDockTop <> newDockTop then
                System.Windows.Controls.DockPanel.SetDock(wpfMenu, Windows.Controls.Dock.Top)

            let rec diffNestedMenuItems (wpfMainMenuItem: System.Windows.Controls.MenuItem) oldMenuItems newMenuItems =
                diffSeq oldMenuItems newMenuItems
                    (fun menuItem ->
                        match menuItem with
                        | MenuItem.MenuItem (header, nested, onClick) ->
                            let wpfMenuItem = System.Windows.Controls.MenuItem(Header = header)

                            diffNestedMenuItems wpfMenuItem [] nested

                            let subscription = wpfMenuItem.Click.Subscribe (fun _ -> onClick ())
                            g.EventSubscriptions.Add(wpfMenuItem, subscription)
                            wpfMainMenuItem.Items.Add wpfMenuItem |> ignore
                    )
                    (fun i oldMenuItem newMenuItem ->
                        match oldMenuItem, newMenuItem with
                        | MenuItem.MenuItem (oldHeader, oldNested, _), MenuItem.MenuItem (newHeader, newNested, onClick) ->
                            let wpfMenuItem = wpfMainMenuItem.Items.[i] :?> System.Windows.Controls.MenuItem

                            diffNestedMenuItems wpfMenuItem oldNested newNested

                            if oldHeader <> newHeader then
                                wpfMenuItem.Header <- newHeader

                            g.EventSubscriptions.[wpfMenuItem].Dispose()
                            g.EventSubscriptions.[wpfMenuItem] <- wpfMenuItem.Click.Subscribe (fun _ -> onClick ())
                    )
                    (fun i ->
                        let wpfMenuItem = wpfMainMenuItem.Items.[i] :?> System.Windows.Controls.MenuItem
                        g.EventSubscriptions.[wpfMenuItem].Dispose ()
                        g.EventSubscriptions.Remove(wpfMenuItem) |> ignore
                        wpfMenu.Items.RemoveAt i
                    )
                    (fun () ->
                        for i = 0 to wpfMainMenuItem.Items.Count - 1 do
                            let wpfMenuItem = wpfMainMenuItem.Items.[i] :?> System.Windows.Controls.MenuItem
                            g.EventSubscriptions.[wpfMenuItem].Dispose ()
                            g.EventSubscriptions.Remove(wpfMenuItem) |> ignore
                        wpfMainMenuItem.Items.Clear ()
                    )

            diffSeq oldMenuItems newMenuItems
                (fun menuItem ->
                    match menuItem with
                    | MenuItem.MenuItem (header, nested, onClick) ->
                        let wpfMenuItem = System.Windows.Controls.MenuItem(Header = header)
                        let subscription = wpfMenuItem.Click.Subscribe (fun _ -> onClick ())

                        diffNestedMenuItems wpfMenuItem [] nested

                        g.EventSubscriptions.Add(wpfMenuItem, subscription)
                        wpfMenu.Items.Add wpfMenuItem |> ignore
                )
                (fun i oldMenuItem newMenuItem ->
                    match oldMenuItem, newMenuItem with
                    | MenuItem.MenuItem (oldHeader, oldNested, _), MenuItem.MenuItem (newHeader, newNested, onClick) ->
                        let wpfMenuItem = wpfMenu.Items.[i] :?> System.Windows.Controls.MenuItem

                        diffNestedMenuItems wpfMenuItem oldNested newNested

                        if oldHeader <> newHeader then
                            wpfMenuItem.Header <- newHeader

                        g.EventSubscriptions.[wpfMenuItem].Dispose()
                        g.EventSubscriptions.[wpfMenuItem] <- wpfMenuItem.Click.Subscribe (fun _ -> onClick ())
                )
                (fun i ->
                    let wpfMenuItem = wpfMenu.Items.[i] :?> System.Windows.Controls.MenuItem
                    g.EventSubscriptions.[wpfMenuItem].Dispose ()
                    g.EventSubscriptions.Remove(wpfMenuItem) |> ignore
                    wpfMenu.Items.RemoveAt i
                )
                (fun () ->
                    for i = 0 to wpfMenu.Items.Count - 1 do
                        let wpfMenuItem = wpfMenu.Items.[i] :?> System.Windows.Controls.MenuItem
                        g.EventSubscriptions.[wpfMenuItem].Dispose ()
                        g.EventSubscriptions.Remove(wpfMenuItem) |> ignore
                    wpfMenu.Items.Clear ()
                )

            wpfMenu :> System.Windows.UIElement

        | View.TextBox (acceptsReturn, onTextChanged) ->
            let wpfTextBox, oldAcceptsReturn =
                match view with
                | View.TextBox (oldAcceptsReturn, _) -> (wpfUIElement :?> System.Windows.Controls.RichTextBox, oldAcceptsReturn)
                | _ -> 
                    let wpfTextBox = System.Windows.Controls.RichTextBox ()
                    wpfTextBox.AcceptsTab <- false
                    wpfTextBox, false

            if oldAcceptsReturn <> acceptsReturn then
                wpfTextBox.AcceptsReturn <- acceptsReturn

            if not (g.EventSubscriptions.ContainsKey wpfTextBox) then
                let mutable text = SourceText.From (String.Empty)
                let wpfDocument = wpfTextBox.Document
                g.Events.[wpfTextBox] <- 
                    wpfTextBox.TextChanged 
                    |> Event.map (fun args ->
                        let changes = args.Changes |> Array.ofSeq
                        let textChanges =
                            changes
                            |> Array.map (fun x ->
                                let textRange = System.Windows.Documents.TextRange (wpfDocument.ContentStart.GetPositionAtOffset(x.Offset), wpfTextBox.Document.ContentStart.GetPositionAtOffset(x.Offset + x.AddedLength))
                                let start = wpfDocument.GetCharIndexAtOffset(x.Offset)
                                TextChange (TextSpan(start, x.RemovedLength), textRange.Text)
                            )
                        text <- text.WithChanges textChanges
                        printfn "%A" (text.ToString ())
                        text
                    )
                g.EventSubscriptions.[wpfTextBox] <- 
                    (g.Events.[wpfTextBox] :?> IEvent<SourceText>).Subscribe onTextChanged
            else
                g.EventSubscriptions.[wpfTextBox].Dispose ()
                g.EventSubscriptions.[wpfTextBox] <- 
                    (g.Events.[wpfTextBox] :?> IEvent<SourceText>).Subscribe onTextChanged

            wpfTextBox :> System.Windows.UIElement
               
type FrameworkWindow<'Model, 'Msg> (app: System.Windows.Application, init: 'Model, update: 'Msg -> 'Model -> 'Model, view: 'Model -> ('Msg -> unit) -> View) as this =
    inherit System.Windows.Window ()

    let g = WpfGlobals (app, this.Dispatcher.Invoke)

    let mutable currentModel = init
    let mutable currentView = View.Empty

    do
        g.DispatchCommandsCalled.Add (fun commands ->
            commands
            |> List.iter (function
                | Command.Nop -> ()
                | Command.Update view ->
                    let updatedView = view ()
                    let content = updateUIElement g (this.Content :?> System.Windows.UIElement) currentView updatedView
                    if not (obj.ReferenceEquals (this.Content, content)) then
                        this.Content <- content
                    currentView <- updatedView
            )
        )

        this.Closed.Add (fun _ ->
            g.EventSubscriptions
            |> Seq.iter (fun pair -> pair.Value.Dispose())
        )

        let mutable dispatch = Unchecked.defaultof<_>
        dispatch <- (fun msg -> 
                                g.DispatchCommands [ Command.Update (fun () -> 
                                                                                currentModel <- (update msg currentModel)
                                                                                view currentModel dispatch) ])

        g.DispatchCommands [ Command.Update (fun () -> view currentModel dispatch) ]

