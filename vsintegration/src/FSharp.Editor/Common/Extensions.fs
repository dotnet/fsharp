// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
[<AutoOpen>]
/// Type and Module Extensions
module internal Microsoft.VisualStudio.FSharp.Editor.Extensions

open System
open System.IO
open System.Collections.Immutable
open System.Runtime.InteropServices

open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.TextManager.Interop

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio
open Microsoft.VisualStudio.OLE.Interop

type private FSharpGlyph = FSharp.Compiler.EditorServices.FSharpGlyph
type private FSharpRoslynGlyph = Microsoft.CodeAnalysis.ExternalAccess.FSharp.FSharpGlyph

type Path with
    static member GetFullPathSafe path =
        try Path.GetFullPath path
        with _ -> path

    static member GetFileNameSafe path =
        try Path.GetFileName path
        with _ -> path


type System.IServiceProvider with
    member x.GetService<'T>() = x.GetService(typeof<'T>) :?> 'T
    member x.GetService<'S, 'T>() = x.GetService(typeof<'S>) :?> 'T

type ProjectId with
    member this.ToFSharpProjectIdString() =
        this.Id.ToString("D").ToLowerInvariant()

type Project with
    member this.IsFSharpMiscellaneous = this.Name = FSharpConstants.FSharpMiscellaneousFilesName
    member this.IsFSharpMetadata = this.Name.StartsWith(FSharpConstants.FSharpMetadataName)
    member this.IsFSharpMiscellaneousOrMetadata = this.IsFSharpMiscellaneous || this.IsFSharpMetadata

type TextViewEventsHandler
    (
        onChangeCaretHandler: (IVsTextView * int * int -> unit) option,
        onKillFocus: (IVsTextView -> unit) option,
        onSetFocus: (IVsTextView -> unit) option
    ) =
    interface IVsTextViewEvents with
        member this.OnChangeCaretLine(view: IVsTextView, newline: int, oldline: int) =
            onChangeCaretHandler
            |> Option.iter (fun handler -> handler (view, newline, oldline))

        member this.OnChangeScrollInfo
            (
                _view: IVsTextView,
                _iBar: int,
                _iMinUnit: int,
                _iMaxUnits: int,
                _iVisibleUnits: int,
                _iFirstVisibleUnit: int
            ) =
            ()

        member this.OnKillFocus(view: IVsTextView) =
            onKillFocus |> Option.iter (fun handler -> handler (view))

        member this.OnSetBuffer(_view: IVsTextView, _buffer: IVsTextLines) = ()

        member this.OnSetFocus(view: IVsTextView) =
            onSetFocus |> Option.iter (fun handler -> handler (view))

type ConnectionPointSubscription = System.IDisposable option

// Usage example:
//  If a handler is None, to not handle that event
//  let subscription = subscribeToTextViewEvents (textView, onChangeCaretHandler, onKillFocus, onSetFocus)
//  Unsubscribe using subscription.Dispose()
let subscribeToTextViewEvents (textView: IVsTextView, onChangeCaretHandler, onKillFocus, onSetFocus) : ConnectionPointSubscription =
    let handler = TextViewEventsHandler(onChangeCaretHandler, onKillFocus, onSetFocus)

    match textView with
    | :? IConnectionPointContainer as cpContainer ->
        let riid = typeof<IVsTextViewEvents>.GUID
        let mutable cookie = 0u

        match cpContainer.FindConnectionPoint(ref riid) with
        | null -> None
        | cp ->
            Some(
                cp.Advise(handler, &cookie)

                { new IDisposable with
                    member _.Dispose() = cp.Unadvise(cookie)
                }
            )
    | _ -> None

type Document with

    member this.TryGetLanguageService<'T when 'T :> ILanguageService>() =
        match this.Project with
        | null -> None
        | project ->
            match project.LanguageServices with
            | null -> None
            | languageServices ->
                languageServices.GetService<'T>()
                |> Some

    member this.TryGetIVsTextView() : IVsTextView option =
        match ServiceProvider.GlobalProvider.GetService(typeof<SVsTextManager>) with
        | :? IVsTextManager as textManager ->
            // Grab IVsRunningDocumentTable
            match ServiceProvider.GlobalProvider.GetService(typeof<SVsRunningDocumentTable>) with
            | :? IVsRunningDocumentTable as rdt ->
                match rdt.FindAndLockDocument(uint32 _VSRDTFLAGS.RDT_NoLock, this.FilePath) with
                | hr, _, _, docData, _ when ErrorHandler.Succeeded(hr) && docData <> IntPtr.Zero ->
                    match Marshal.GetObjectForIUnknown docData with
                    | :? IVsTextBuffer as ivsTextBuffer ->
                        match textManager.GetActiveView(0, ivsTextBuffer) with
                        | hr, vsTextView when ErrorHandler.Succeeded(hr) -> Some vsTextView
                        | _ -> None
                    | _ -> None
                | _ -> None
            | _ -> None
        | _ -> None

    member this.TryGetTextViewAndCaretPos() : (IVsTextView * Position) option =
        match this.TryGetIVsTextView() with
        | Some textView ->
            match textView.GetCaretPos() with
            | hr, line, column when ErrorHandler.Succeeded(hr) -> Some(textView, Position.fromZ line column)
            | _ -> None
        | None -> None

    member this.IsFSharpScript =
        isScriptFile this.FilePath

module private SourceText =

    open System.Runtime.CompilerServices

    /// Ported from Roslyn.Utilities
    [<RequireQualifiedAccess>]
    module Hash =
        /// (From Roslyn) This is how VB Anonymous Types combine hash values for fields.
        let combine (newKey: int) (currentKey: int) = (currentKey * (int 0xA5555529)) + newKey

        let combineValues (values: seq<'T>) =
            (0, values) ||> Seq.fold (fun hash value -> combine (value.GetHashCode()) hash)

    let weakTable = ConditionalWeakTable<SourceText, ISourceText>()

    let create (sourceText: SourceText) =
        let sourceText =
            { 
                new Object() with
                    override _.GetHashCode() =
                        let checksum = sourceText.GetChecksum()
                        let contentsHash = if not checksum.IsDefault then Hash.combineValues checksum else 0
                        let encodingHash = if not (isNull sourceText.Encoding) then sourceText.Encoding.GetHashCode() else 0

                        sourceText.ChecksumAlgorithm.GetHashCode()
                        |> Hash.combine encodingHash
                        |> Hash.combine contentsHash
                        |> Hash.combine sourceText.Length

                interface ISourceText with
            
                    member _.Item with get index = sourceText.[index]

                    member _.GetLineString(lineIndex) =
                        sourceText.Lines.[lineIndex].ToString()

                    member _.GetLineCount() =
                        sourceText.Lines.Count

                    member _.GetLastCharacterPosition() =
                        if sourceText.Lines.Count > 0 then
                            (sourceText.Lines.Count, sourceText.Lines.[sourceText.Lines.Count - 1].Span.Length)
                        else
                            (0, 0)

                    member _.GetSubTextString(start, length) =
                        sourceText.GetSubText(TextSpan(start, length)).ToString()

                    member _.SubTextEquals(target, startIndex) =
                        if startIndex < 0 || startIndex >= sourceText.Length then
                            invalidArg "startIndex" "Out of range."

                        if String.IsNullOrEmpty(target) then
                            invalidArg "target" "Is null or empty."

                        let lastIndex = startIndex + target.Length
                        if lastIndex <= startIndex || lastIndex >= sourceText.Length then
                            invalidArg "target" "Too big."

                        let mutable finished = false
                        let mutable didEqual = true
                        let mutable i = 0
                        while not finished && i < target.Length do
                            if target.[i] <> sourceText.[startIndex + i] then
                                didEqual <- false
                                finished <- true // bail out early                        
                            else
                                i <- i + 1

                        didEqual

                    member _.ContentEquals(sourceText) =
                        match sourceText with
                        | :? SourceText as sourceText -> sourceText.ContentEquals(sourceText)
                        | _ -> false

                    member _.Length = sourceText.Length

                    member _.CopyTo(sourceIndex, destination, destinationIndex, count) =
                        sourceText.CopyTo(sourceIndex, destination, destinationIndex, count)
            }

        sourceText

type SourceText with

    member this.ToFSharpSourceText() =
        SourceText.weakTable.GetValue(this, Runtime.CompilerServices.ConditionalWeakTable<_,_>.CreateValueCallback(SourceText.create))

type NavigationItem with
    member x.RoslynGlyph : FSharpRoslynGlyph =
        match x.Glyph with
        | FSharpGlyph.Class
        | FSharpGlyph.Typedef
        | FSharpGlyph.Type
        | FSharpGlyph.Exception ->
            match x.Access with
            | Some SynAccess.Private -> FSharpRoslynGlyph.ClassPrivate
            | Some SynAccess.Internal -> FSharpRoslynGlyph.ClassInternal
            | _ -> FSharpRoslynGlyph.ClassPublic
        | FSharpGlyph.Constant -> 
            match x.Access with
            | Some SynAccess.Private -> FSharpRoslynGlyph.ConstantPrivate
            | Some SynAccess.Internal -> FSharpRoslynGlyph.ConstantInternal
            | _ -> FSharpRoslynGlyph.ConstantPublic
        | FSharpGlyph.Delegate -> 
            match x.Access with
            | Some SynAccess.Private -> FSharpRoslynGlyph.DelegatePrivate
            | Some SynAccess.Internal -> FSharpRoslynGlyph.DelegateInternal
            | _ -> FSharpRoslynGlyph.DelegatePublic
        | FSharpGlyph.Union
        | FSharpGlyph.Enum -> 
            match x.Access with
            | Some SynAccess.Private -> FSharpRoslynGlyph.EnumPrivate
            | Some SynAccess.Internal -> FSharpRoslynGlyph.EnumInternal
            | _ -> FSharpRoslynGlyph.EnumPublic
        | FSharpGlyph.EnumMember
        | FSharpGlyph.Variable
        | FSharpGlyph.Field -> 
            match x.Access with
            | Some SynAccess.Private -> FSharpRoslynGlyph.FieldPrivate
            | Some SynAccess.Internal -> FSharpRoslynGlyph.FieldInternal
            | _ -> FSharpRoslynGlyph.FieldPublic
        | FSharpGlyph.Event -> 
            match x.Access with
            | Some SynAccess.Private -> FSharpRoslynGlyph.EventPrivate
            | Some SynAccess.Internal -> FSharpRoslynGlyph.EventInternal
            | _ -> FSharpRoslynGlyph.EventPublic
        | FSharpGlyph.Interface -> 
            match x.Access with
            | Some SynAccess.Private -> FSharpRoslynGlyph.InterfacePrivate
            | Some SynAccess.Internal -> FSharpRoslynGlyph.InterfaceInternal
            | _ -> FSharpRoslynGlyph.InterfacePublic
        | FSharpGlyph.Method
        | FSharpGlyph.OverridenMethod -> 
            match x.Access with
            | Some SynAccess.Private -> FSharpRoslynGlyph.MethodPrivate
            | Some SynAccess.Internal -> FSharpRoslynGlyph.MethodInternal
            | _ -> FSharpRoslynGlyph.MethodPublic
        | FSharpGlyph.Module -> 
            match x.Access with
            | Some SynAccess.Private -> FSharpRoslynGlyph.ModulePrivate
            | Some SynAccess.Internal -> FSharpRoslynGlyph.ModuleInternal
            | _ -> FSharpRoslynGlyph.ModulePublic
        | FSharpGlyph.NameSpace -> FSharpRoslynGlyph.Namespace
        | FSharpGlyph.Property -> 
            match x.Access with
            | Some SynAccess.Private -> FSharpRoslynGlyph.PropertyPrivate
            | Some SynAccess.Internal -> FSharpRoslynGlyph.PropertyInternal
            | _ -> FSharpRoslynGlyph.PropertyPublic
        | FSharpGlyph.Struct -> 
            match x.Access with
            | Some SynAccess.Private -> FSharpRoslynGlyph.StructurePrivate
            | Some SynAccess.Internal -> FSharpRoslynGlyph.StructureInternal
            | _ -> FSharpRoslynGlyph.StructurePublic
        | FSharpGlyph.ExtensionMethod ->
            match x.Access with
            | Some SynAccess.Private -> FSharpRoslynGlyph.ExtensionMethodPrivate
            | Some SynAccess.Internal -> FSharpRoslynGlyph.ExtensionMethodInternal
            | _ -> FSharpRoslynGlyph.ExtensionMethodPublic
        | FSharpGlyph.Error -> FSharpRoslynGlyph.Error

[<RequireQualifiedAccess>]
module String =   

    let getLines (str: string) =
        use reader = new StringReader(str)
        [|  let mutable line = reader.ReadLine()
            while not (isNull line) do
                yield line
                line <- reader.ReadLine()
            if str.EndsWith("\n") then
            // last trailing space not returned
            // http://stackoverflow.com/questions/19365404/stringreader-omits-trailing-linebreak
                yield String.Empty
        |]


[<RequireQualifiedAccess>]
module Option =

    let guard (x: bool) : Option<unit> =
        if x then Some() else None

    let attempt (f: unit -> 'T) = try Some <| f() with _ -> None

    /// Returns 'Some list' if all elements in the list are Some, otherwise None
    let ofOptionList (xs : 'a option list) : 'a list option =

        if xs |> List.forall Option.isSome then
            xs |> List.map Option.get |> Some
        else
            None

[<RequireQualifiedAccess>]
module Seq =

    let toImmutableArray (xs: seq<'a>) : ImmutableArray<'a> = xs.ToImmutableArray()

[<RequireQualifiedAccess>]
module Array =
    let foldi (folder : 'State -> int -> 'T -> 'State) (state : 'State) (xs : 'T[]) =
        let mutable state = state
        let mutable i = 0
        for x in xs do
            state <- folder state i x
            i <- i + 1
        state

    let toImmutableArray (xs: 'T[]) = xs.ToImmutableArray()

[<RequireQualifiedAccess>]
module Exception =

    /// Returns a flattened string of the exception's message and all of its inner exception
    /// messages recursively.
    let flattenMessage (root: System.Exception) =

        let rec flattenInner (exc: System.Exception) =
            match exc with
            | null -> []
            | _ -> [exc.Message] @ (flattenInner exc.InnerException)
        
        // If an aggregate exception only has a single inner exception, use that as the root
        match root with
        | :? AggregateException as agg ->
            if agg.InnerExceptions.Count = 1
            then agg.InnerExceptions.[0]
            else agg :> exn
        | _ -> root
        |> flattenInner
        |> String.concat " ---> "
