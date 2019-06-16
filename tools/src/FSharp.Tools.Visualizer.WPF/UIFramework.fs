namespace FSharp.Tools.Visualizer.WPF

open System
open System.Collections
open System.Collections.Generic
open System.Collections.Concurrent
open Microsoft.CodeAnalysis.Text
open ICSharpCode.AvalonEdit
open ICSharpCode.AvalonEdit.Highlighting
open ICSharpCode.AvalonEdit.CodeCompletion

[<AutoOpen>]
module rec Virtual =

    [<RequireQualifiedAccess>]
    type DataGridColumn = 
        | Text of header: string * bindingName: string

    [<RequireQualifiedAccess>]
    type MenuItem = MenuItem of header: string * nested: MenuItem list * onClick: (unit -> unit)

    [<RequireQualifiedAccess>]
    type TreeViewItem = TreeViewItem of header: string * nested: TreeViewItem list * onClick: (unit -> unit)

    type CompletionItem = CompletionItem of text: string

    [<RequireQualifiedAccess>]
    type View =
        | Empty
        | Common of View * willExit: bool
        | DockPanel of View list
        | DataGrid of columns: DataGridColumn list * data: IEnumerable
        | Menu of MenuItem list * dockTop: bool
        | Editor of highlightSpans: HighlightSpan list * willRedraw: bool * completionItems: CompletionItem list * onTextChanged: (SourceText * int * bool -> unit) * onCompletionTriggered: (SourceText * int -> unit)
        | TreeView of TreeViewItem list

[<Sealed>]
type internal ReferenceEqualityComparer () =
    
    interface IEqualityComparer<obj> with

        member __.GetHashCode value =            
            System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode value

        member __.Equals (left, right) =
            obj.ReferenceEquals (left, right)

[<Sealed>]
type internal WpfGlobals (app: System.Windows.Application, uiThread) =
    
    member __.Application = app

    member val Events = Dictionary<obj, obj>()
    member val EventSubscriptions = Dictionary<obj, IDisposable>(ReferenceEqualityComparer())

    member __.RunOnUIThread (f: unit -> unit) = 
        if System.Threading.Thread.CurrentThread.ManagedThreadId = app.Dispatcher.Thread.ManagedThreadId then
            f ()
        else
            uiThread f

[<AutoOpen>]
module internal Helpers =

    type System.Windows.Documents.FlowDocument with
    
        member this.GetCharIndexAtOffset (offset: int) =
            let mutable nextPointer = this.ContentStart
            let mutable count = 0

            while nextPointer <> null && this.ContentStart.GetOffsetToPosition(nextPointer) < offset do
                if nextPointer.CompareTo(this.ContentEnd) = 0 then
                    nextPointer <- null
                else
                    let context = nextPointer.GetPointerContext(Windows.Documents.LogicalDirection.Forward)
                    if context = Windows.Documents.TextPointerContext.Text || context = Windows.Documents.TextPointerContext.None  then
                        nextPointer <- nextPointer.GetNextInsertionPosition(Windows.Documents.LogicalDirection.Forward)
                        count <- count + 1
                    else
                        nextPointer <- nextPointer.GetNextInsertionPosition(Windows.Documents.LogicalDirection.Forward)
            count

    let diffSeq (state: 'T seq) (newState: 'T seq) create update (removeAt: int -> (unit -> unit)) clear =
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

        let removes = ResizeArray ()
        if lastIndex <> -1 && stateCount - 1 > lastIndex then
            for i = lastIndex + 1 to stateCount - 1 do
                removes.Add (removeAt i)

        for remove in removes do
            remove ()

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
                (fun i -> fun () -> wpfDockPanel.Children.Remove (wpfDockPanel.Children.[i]))
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
                (fun i -> fun () -> wpfDataGrid.Columns.Remove (wpfDataGrid.Columns.[i]) |> ignore)
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
                        fun () -> wpfMainMenuItem.Items.Remove (wpfMenuItem)
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
                    fun () -> wpfMenu.Items.Remove wpfMenuItem
                )
                (fun () ->
                    for i = 0 to wpfMenu.Items.Count - 1 do
                        let wpfMenuItem = wpfMenu.Items.[i] :?> System.Windows.Controls.MenuItem
                        g.EventSubscriptions.[wpfMenuItem].Dispose ()
                        g.EventSubscriptions.Remove(wpfMenuItem) |> ignore
                    wpfMenu.Items.Clear ()
                )

            wpfMenu :> System.Windows.UIElement

        | View.Editor (highlightSpans, willRedraw, completionItems, onTextChanged, onCompletionTriggered) ->
            let wpfTextBox =
                match view with
                | View.Editor (_) -> (wpfUIElement :?> FSharpTextEditor)
                | _ -> 
                    FSharpTextEditor ()

            wpfTextBox.WordWrap <- true
            wpfTextBox.Width <- 1080.

            if not (g.EventSubscriptions.ContainsKey wpfTextBox) then
                g.Events.[wpfTextBox] <- (wpfTextBox.SourceTextChanged, wpfTextBox.CompletionTriggered)
                g.EventSubscriptions.[wpfTextBox] <- 
                    let (textChanged, completionTriggered) = (g.Events.[wpfTextBox] :?> (IEvent<SourceText * int * bool> * IEvent<SourceText * int>))
                    let e1 = textChanged.Subscribe onTextChanged
                    let e2 = completionTriggered.Subscribe onCompletionTriggered
                    { new IDisposable with

                        member __.Dispose () =
                            e1.Dispose ()
                            e2.Dispose ()
                    }
            else
                g.EventSubscriptions.[wpfTextBox].Dispose ()
                g.EventSubscriptions.[wpfTextBox] <- 
                    let (textChanged, completionTriggered) = (g.Events.[wpfTextBox] :?> (IEvent<SourceText * int * bool> * IEvent<SourceText * int>))
                    let e1 = textChanged.Subscribe onTextChanged
                    let e2 = completionTriggered.Subscribe onCompletionTriggered
                    { new IDisposable with

                        member __.Dispose () =
                            e1.Dispose ()
                            e2.Dispose ()
                    }

            

            if willRedraw then
                wpfTextBox.ClearAllTextSpanColors ()
                highlightSpans
                |> List.iter (fun (HighlightSpan (span, color, kind)) ->
                    if wpfTextBox.SourceText.Length >= span.End && not (span.Length = 0) then
                        let linePosSpan = wpfTextBox.SourceText.Lines.GetLinePositionSpan span
                        let startLine = linePosSpan.Start.Line
                        let endLine = linePosSpan.End.Line

                        for lineNumber = startLine to endLine do

                            let line = wpfTextBox.SourceText.Lines.[lineNumber]

                            let start =
                                if startLine = lineNumber then
                                    span.Start
                                else
                                    line.Start

                            let length =
                                if endLine = lineNumber then
                                    if startLine = lineNumber then
                                        span.Length
                                    else
                                        span.End - line.Start
                                else
                                    line.End - line.Start

                            let textSpanColors = wpfTextBox.GetTextSpanColors (lineNumber + 1)
                            textSpanColors.Add (HighlightSpan (TextSpan(start, length), color, kind))
                )
                wpfTextBox.Redraw ()

            let completionData =
                completionItems
                |> List.map (fun (CompletionItem (text)) ->
                    FSharpCompletionData text
                )

            wpfTextBox.ShowCompletions completionData

            wpfTextBox :> System.Windows.UIElement

        | View.TreeView (newMenuItems) ->
            let wpfTreeView, oldMenuItems =
                match view with
                | View.TreeView (oldMenuItems) -> (wpfUIElement :?> System.Windows.Controls.TreeView, oldMenuItems)
                | _ -> System.Windows.Controls.TreeView (), []

            let rec updateTreeViewItems (treeViewItems: TreeViewItem list) acc =
                match treeViewItems with
                | [] -> List.rev acc
                | (TreeViewItem.TreeViewItem (header, nested, onClick)) :: tail ->
                    let wpfTreeViewItem = System.Windows.Controls.TreeViewItem()
                    wpfTreeViewItem.Header <- header
                    wpfTreeViewItem.Selected.Add (fun _ -> if wpfTreeViewItem.IsSelected then onClick ())
                    wpfTreeViewItem.IsExpanded <- true
                    updateTreeViewItems nested []
                    |> List.iter (fun x ->
                        wpfTreeViewItem.Items.Add x |> ignore
                    )

                    updateTreeViewItems tail (wpfTreeViewItem :: acc)

            wpfTreeView.Items.Clear ()
            updateTreeViewItems newMenuItems []
            |> List.iter (fun x ->
                wpfTreeView.Items.Add x |> ignore
            )

            wpfTreeView :> System.Windows.UIElement
               
type FrameworkWindow<'Model, 'Msg> (app: System.Windows.Application, init: 'Model, update: 'Msg -> 'Model -> 'Model, view: 'Model -> ('Msg -> unit) -> View) as this =
    inherit System.Windows.Window ()

    let g = WpfGlobals (app, this.Dispatcher.Invoke)

    let mutable currentState = (init, View.Empty)

    let updateUI oldViewState viewState =
        let content = updateUIElement g (this.Content :?> System.Windows.UIElement) oldViewState viewState
        if not (obj.ReferenceEquals (this.Content, content)) then
            this.Content <- content

    do
        this.Closed.Add (fun _ ->
            g.EventSubscriptions
            |> Seq.iter (fun pair -> pair.Value.Dispose())
        )

        let mutable dispatch = Unchecked.defaultof<_>

        let computeState msg =
            let oldModel, _ = currentState
            let model = update msg oldModel
            let viewState = view model dispatch
            (model, viewState)

        let updateState (model, viewState) =
            let _, oldViewState = currentState
            currentState <- (model, viewState)
            (oldViewState, viewState)

        let update msg =
            let state = computeState msg
            let oldViewState, viewState = updateState state
            updateUI oldViewState viewState

        dispatch <- (fun msg ->
            if System.Threading.Thread.CurrentThread.ManagedThreadId = g.Application.Dispatcher.Thread.ManagedThreadId then
                update msg
            else
                g.RunOnUIThread (fun () -> update msg)
        )


        let oldViewState, viewState = updateState (init, view init dispatch)
        updateUI oldViewState viewState