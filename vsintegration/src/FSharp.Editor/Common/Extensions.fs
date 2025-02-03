// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
[<AutoOpen>]
/// Type and Module Extensions
module internal Microsoft.VisualStudio.FSharp.Editor.Extensions

open System
open System.IO
open System.Collections.Immutable
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open Microsoft.VisualStudio.FSharp.Editor

type private FSharpGlyph = FSharp.Compiler.EditorServices.FSharpGlyph
type private FSharpRoslynGlyph = Microsoft.CodeAnalysis.ExternalAccess.FSharp.FSharpGlyph

type Path with

    static member GetFullPathSafe path =
        try
            Path.GetFullPath path
        with _ ->
            path

    static member GetFileNameSafe path =
        try
            Path.GetFileName path
        with _ ->
            path

type System.IServiceProvider with

    member x.GetService<'T>() = x.GetService(typeof<'T>) :?> 'T
    member x.GetService<'S, 'T>() = x.GetService(typeof<'S>) :?> 'T

type ProjectId with

    member this.ToFSharpProjectIdString() =
        this.Id.ToString("D").ToLowerInvariant()

type Project with

    member this.IsFSharpMiscellaneous =
        this.Name = FSharpConstants.FSharpMiscellaneousFilesName

    member this.IsFSharpMetadata = this.Name.StartsWith(FSharpConstants.FSharpMetadataName)

    member this.IsFSharpMiscellaneousOrMetadata =
        this.IsFSharpMiscellaneous || this.IsFSharpMetadata

    member this.IsFSharp = this.Language = LanguageNames.FSharp

type Document with

    member this.TryGetLanguageService<'T when 'T :> ILanguageService>() =
        match this.Project with
        | null -> None
        | project ->
            match project.LanguageServices with
            | null -> None
            | languageServices -> languageServices.GetService<'T>() |> Some

    member this.IsFSharpScript = isScriptFile this.FilePath

    member this.IsFSharpSignatureFile = isSignatureFile this.FilePath

module private SourceText =

    open System.Runtime.CompilerServices

    /// Ported from Roslyn.Utilities
    [<RequireQualifiedAccess>]
    module Hash =
        /// (From Roslyn) This is how VB Anonymous Types combine hash values for fields.
        let combine (newKey: int) (currentKey: int) =
            (currentKey * (int 0xA5555529)) + newKey

        let combineValues (values: seq<'T>) =
            (0, values) ||> Seq.fold (fun hash value -> combine (value.GetHashCode()) hash)

    let weakTable = ConditionalWeakTable<SourceText, ISourceTextNew>()

    let create (sourceText: SourceText) =
        let sourceText =
            { new Object() with
                override _.GetHashCode() =
                    let checksum = sourceText.GetChecksum()

                    let contentsHash =
                        if not checksum.IsDefault then
                            Hash.combineValues checksum
                        else
                            0

                    let encodingHash =
                        if not (isNull sourceText.Encoding) then
                            sourceText.Encoding.GetHashCode()
                        else
                            0

                    sourceText.ChecksumAlgorithm.GetHashCode()
                    |> Hash.combine encodingHash
                    |> Hash.combine contentsHash
                    |> Hash.combine sourceText.Length

                override _.ToString() = sourceText.ToString()
              interface ISourceTextNew with

                  member _.Item
                      with get index = sourceText.[index]

                  member _.GetLineString(lineIndex) = sourceText.Lines.[lineIndex].ToString()

                  member _.GetLineCount() = sourceText.Lines.Count

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

                  member this.GetSubTextFromRange range =
                      let totalAmountOfLines = sourceText.Lines.Count

                      if
                          range.StartLine = 0
                          && range.StartColumn = 0
                          && range.EndLine = 0
                          && range.EndColumn = 0
                      then
                          String.Empty
                      elif
                          range.StartLine < 1
                          || (range.StartLine - 1) > totalAmountOfLines
                          || range.EndLine < 1
                          || (range.EndLine - 1) > totalAmountOfLines
                      then
                          invalidArg (nameof range) "The range is outside the file boundaries"
                      else
                          let startLine = range.StartLine - 1
                          let line = this.GetLineString startLine

                          if range.StartLine = range.EndLine then
                              let length = range.EndColumn - range.StartColumn
                              line.Substring(range.StartColumn, length)
                          else
                              let firstLineContent = line.Substring(range.StartColumn)
                              let sb = System.Text.StringBuilder().AppendLine(firstLineContent)

                              for lineNumber in range.StartLine .. range.EndLine - 2 do
                                  sb.AppendLine(this.GetLineString lineNumber) |> ignore

                              let lastLine = this.GetLineString(range.EndLine - 1)
                              sb.Append(lastLine.Substring(0, range.EndColumn)).ToString()

                  member _.GetChecksum() = sourceText.GetChecksum()
            }

        sourceText

type SourceText with

    member this.ToFSharpSourceText() =
        SourceText.weakTable.GetValue(
            this,
            Runtime.CompilerServices.ConditionalWeakTable<_, _>
                .CreateValueCallback(SourceText.create)
        )

type NavigationItem with

    member x.RoslynGlyph: FSharpRoslynGlyph =
        match x.Glyph with
        | FSharpGlyph.Class
        | FSharpGlyph.Typedef
        | FSharpGlyph.Type
        | FSharpGlyph.Exception ->
            match x.Access with
            | Some(SynAccess.Private _) -> FSharpRoslynGlyph.ClassPrivate
            | Some(SynAccess.Internal _) -> FSharpRoslynGlyph.ClassInternal
            | _ -> FSharpRoslynGlyph.ClassPublic
        | FSharpGlyph.Constant ->
            match x.Access with
            | Some(SynAccess.Private _) -> FSharpRoslynGlyph.ConstantPrivate
            | Some(SynAccess.Internal _) -> FSharpRoslynGlyph.ConstantInternal
            | _ -> FSharpRoslynGlyph.ConstantPublic
        | FSharpGlyph.Delegate ->
            match x.Access with
            | Some(SynAccess.Private _) -> FSharpRoslynGlyph.DelegatePrivate
            | Some(SynAccess.Internal _) -> FSharpRoslynGlyph.DelegateInternal
            | _ -> FSharpRoslynGlyph.DelegatePublic
        | FSharpGlyph.Union
        | FSharpGlyph.Enum ->
            match x.Access with
            | Some(SynAccess.Private _) -> FSharpRoslynGlyph.EnumPrivate
            | Some(SynAccess.Internal _) -> FSharpRoslynGlyph.EnumInternal
            | _ -> FSharpRoslynGlyph.EnumPublic
        | FSharpGlyph.EnumMember
        | FSharpGlyph.Variable
        | FSharpGlyph.Field ->
            match x.Access with
            | Some(SynAccess.Private _) -> FSharpRoslynGlyph.FieldPrivate
            | Some(SynAccess.Internal _) -> FSharpRoslynGlyph.FieldInternal
            | _ -> FSharpRoslynGlyph.FieldPublic
        | FSharpGlyph.Event ->
            match x.Access with
            | Some(SynAccess.Private _) -> FSharpRoslynGlyph.EventPrivate
            | Some(SynAccess.Internal _) -> FSharpRoslynGlyph.EventInternal
            | _ -> FSharpRoslynGlyph.EventPublic
        | FSharpGlyph.Interface ->
            match x.Access with
            | Some(SynAccess.Private _) -> FSharpRoslynGlyph.InterfacePrivate
            | Some(SynAccess.Internal _) -> FSharpRoslynGlyph.InterfaceInternal
            | _ -> FSharpRoslynGlyph.InterfacePublic
        | FSharpGlyph.Method
        | FSharpGlyph.OverridenMethod ->
            match x.Access with
            | Some(SynAccess.Private _) -> FSharpRoslynGlyph.MethodPrivate
            | Some(SynAccess.Internal _) -> FSharpRoslynGlyph.MethodInternal
            | _ -> FSharpRoslynGlyph.MethodPublic
        | FSharpGlyph.Module ->
            match x.Access with
            | Some(SynAccess.Private _) -> FSharpRoslynGlyph.ModulePrivate
            | Some(SynAccess.Internal _) -> FSharpRoslynGlyph.ModuleInternal
            | _ -> FSharpRoslynGlyph.ModulePublic
        | FSharpGlyph.NameSpace -> FSharpRoslynGlyph.Namespace
        | FSharpGlyph.Property ->
            match x.Access with
            | Some(SynAccess.Private _) -> FSharpRoslynGlyph.PropertyPrivate
            | Some(SynAccess.Internal _) -> FSharpRoslynGlyph.PropertyInternal
            | _ -> FSharpRoslynGlyph.PropertyPublic
        | FSharpGlyph.Struct ->
            match x.Access with
            | Some(SynAccess.Private _) -> FSharpRoslynGlyph.StructurePrivate
            | Some(SynAccess.Internal _) -> FSharpRoslynGlyph.StructureInternal
            | _ -> FSharpRoslynGlyph.StructurePublic
        | FSharpGlyph.ExtensionMethod ->
            match x.Access with
            | Some(SynAccess.Private _) -> FSharpRoslynGlyph.ExtensionMethodPrivate
            | Some(SynAccess.Internal _) -> FSharpRoslynGlyph.ExtensionMethodInternal
            | _ -> FSharpRoslynGlyph.ExtensionMethodPublic
        | FSharpGlyph.Error -> FSharpRoslynGlyph.Error
        | FSharpGlyph.TypeParameter -> FSharpRoslynGlyph.TypeParameter

[<RequireQualifiedAccess>]
module String =

    let getLines (str: string) =
        use reader = new StringReader(str)

        [|
            let mutable line = reader.ReadLine()

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

    let guard (x: bool) : ValueOption<unit> = if x then ValueSome() else ValueNone

    let attempt (f: unit -> 'T) =
        try
            Some <| f ()
        with _ ->
            None

    /// Returns 'Some list' if all elements in the list are Some, otherwise None
    let ofOptionList (xs: 'a option list) : 'a list option =

        if xs |> List.forall Option.isSome then
            xs |> List.map Option.get |> Some
        else
            None

[<RequireQualifiedAccess>]
module IEnumerator =
    let chooseV f (e: IEnumerator<'T>) =
        let mutable started = false
        let mutable curr = ValueNone

        let get () =
            if not started then
                raise (InvalidOperationException("Not started"))

            match curr with
            | ValueNone -> raise (InvalidOperationException("Already finished"))
            | ValueSome x -> x

        { new IEnumerator<'U> with
            member _.Current = get ()
          interface System.Collections.IEnumerator with
              member _.Current = box (get ())

              member _.MoveNext() =
                  if not started then
                      started <- true

                  curr <- ValueNone

                  while (curr.IsNone && e.MoveNext()) do
                      curr <- f e.Current

                  ValueOption.isSome curr

              member _.Reset() =
                  raise (NotSupportedException("Reset is not supported"))
          interface System.IDisposable with
              member _.Dispose() = e.Dispose()
        }

[<RequireQualifiedAccess>]
module Seq =

    let mkSeq f =
        { new IEnumerable<'U> with
            member _.GetEnumerator() = f ()
          interface System.Collections.IEnumerable with
              member _.GetEnumerator() =
                  (f () :> System.Collections.IEnumerator)
        }

    let inline revamp f (ie: seq<_>) =
        mkSeq (fun () -> f (ie.GetEnumerator()))

    let toImmutableArray (xs: seq<'a>) : ImmutableArray<'a> = xs.ToImmutableArray()

    let inline tryHeadV (source: seq<_>) =
        use e = source.GetEnumerator()

        if (e.MoveNext()) then ValueSome e.Current else ValueNone

    let inline tryFindV ([<InlineIfLambda>] predicate) (source: seq<'T>) =
        use e = source.GetEnumerator()
        let mutable res = ValueNone

        while (ValueOption.isNone res && e.MoveNext()) do
            let c = e.Current

            if predicate c then
                res <- ValueSome c

        res

    let inline tryFindIndexV ([<InlineIfLambda>] predicate) (source: seq<_>) =
        use ie = source.GetEnumerator()

        let rec loop i =
            if ie.MoveNext() then
                if predicate ie.Current then ValueSome i else loop (i + 1)
            else
                ValueNone

        loop 0

    let inline tryPickV ([<InlineIfLambda>] chooser) (source: seq<'T>) =
        use e = source.GetEnumerator()
        let mutable res = ValueNone

        while (ValueOption.isNone res && e.MoveNext()) do
            res <- chooser e.Current

        res

    let chooseV (chooser: 'a -> 'b voption) source =
        revamp (IEnumerator.chooseV chooser) source

[<RequireQualifiedAccess>]
module Array =
    let inline foldi ([<InlineIfLambda>] folder: 'State -> int -> 'T -> 'State) (state: 'State) (xs: 'T[]) =
        let mutable state = state
        let mutable i = 0

        for x in xs do
            state <- folder state i x
            i <- i + 1

        state

    let toImmutableArray (xs: 'T[]) = xs.ToImmutableArray()

    let inline tryHeadV (array: _[]) =
        if array.Length = 0 then ValueNone else ValueSome array[0]

    let inline tryFindV ([<InlineIfLambda>] predicate) (array: _[]) =

        let rec loop i =
            if i >= array.Length then ValueNone
            else if predicate array.[i] then ValueSome array[i]
            else loop (i + 1)

        loop 0

    let inline chooseV ([<InlineIfLambda>] chooser: 'T -> 'U voption) (array: 'T[]) =

        let mutable i = 0
        let mutable first = Unchecked.defaultof<'U>
        let mutable found = false

        while i < array.Length && not found do
            let element = array.[i]

            match chooser element with
            | ValueNone -> i <- i + 1
            | ValueSome b ->
                first <- b
                found <- true

        if i <> array.Length then

            let chunk1: 'U[] = Array.zeroCreate ((array.Length >>> 2) + 1)

            chunk1.[0] <- first
            let mutable count = 1
            i <- i + 1

            while count < chunk1.Length && i < array.Length do
                let element = array.[i]

                match chooser element with
                | ValueNone -> ()
                | ValueSome b ->
                    chunk1.[count] <- b
                    count <- count + 1

                i <- i + 1

            if i < array.Length then
                let chunk2: 'U[] = Array.zeroCreate (array.Length - i)

                count <- 0

                while i < array.Length do
                    let element = array.[i]

                    match chooser element with
                    | ValueNone -> ()
                    | ValueSome b ->
                        chunk2.[count] <- b
                        count <- count + 1

                    i <- i + 1

                let res: 'U[] = Array.zeroCreate (chunk1.Length + count)

                Array.Copy(chunk1, res, chunk1.Length)
                Array.Copy(chunk2, 0, res, chunk1.Length, count)
                res
            else
                Array.sub chunk1 0 count
        else
            Array.empty

[<RequireQualifiedAccess>]
module ImmutableArray =
    let inline tryHeadV (xs: ImmutableArray<'T>) : 'T voption =
        if xs.Length = 0 then ValueNone else ValueSome xs[0]

    let inline empty<'T> = ImmutableArray<'T>.Empty

    let inline create<'T> (x: 'T) = ImmutableArray.Create<'T>(x)

[<RequireQualifiedAccess>]
module List =
    let rec tryFindV predicate list =
        match list with
        | [] -> ValueNone
        | h :: t -> if predicate h then ValueSome h else tryFindV predicate t

[<RequireQualifiedAccess>]
module Exception =

    /// Returns a flattened string of the exception's message and all of its inner exception
    /// messages recursively.
    let flattenMessage (root: System.Exception) =

        let rec flattenInner (exc: System.Exception) =
            match exc with
            | null -> []
            | _ -> [ exc.Message ] @ (flattenInner exc.InnerException)

        // If an aggregate exception only has a single inner exception, use that as the root
        match root with
        | :? AggregateException as agg ->
            if agg.InnerExceptions.Count = 1 then
                agg.InnerExceptions.[0]
            else
                agg :> exn
        | _ -> root
        |> flattenInner
        |> String.concat " ---> "

[<RequireQualifiedAccess>]
module TextSpan =
    let empty = TextSpan()

type Async with

    static member RunImmediateExceptOnUI(computation: Async<'T>, ?cancellationToken) =
        match SynchronizationContext.Current with
        | null ->
            let cancellationToken = defaultArg cancellationToken Async.DefaultCancellationToken
            let ts = TaskCompletionSource<'T>()
            let task = ts.Task

            Async.StartWithContinuations(
                computation,
                (fun k -> ts.SetResult k),
                (fun exn -> ts.SetException exn),
                (fun _ -> ts.SetCanceled()),
                cancellationToken
            )

            task.Result
        | _ -> Async.RunSynchronously(computation, ?cancellationToken = cancellationToken)

#if !NET7_0_OR_GREATER
open System.Runtime.CompilerServices

[<Sealed; AbstractClass; Extension>]
type ReadOnlySpanExtensions =
    [<Extension>]
    static member IndexOfAnyExcept(span: ReadOnlySpan<char>, value: char) =
        let mutable i = 0
        let mutable found = false

        while not found && i < span.Length do
            if span[i] <> value then found <- true else i <- i + 1

        if found then i else -1

    [<Extension>]
    static member IndexOfAnyExcept(span: ReadOnlySpan<char>, value0: char, value1: char) =
        let mutable i = 0
        let mutable found = false

        while not found && i < span.Length do
            let c = span[i]

            if c <> value0 && c <> value1 then
                found <- true
            else
                i <- i + 1

        if found then i else -1

    [<Extension>]
    static member IndexOfAnyExcept(span: ReadOnlySpan<char>, values: ReadOnlySpan<char>) =
        let mutable i = 0
        let mutable found = false

        while not found && i < span.Length do
            if values.IndexOf span[i] < 0 then
                found <- true
            else
                i <- i + 1

        if found then i else -1

    [<Extension>]
    static member LastIndexOfAnyExcept(span: ReadOnlySpan<char>, value0: char, value1: char) =
        let mutable i = span.Length - 1
        let mutable found = false

        while not found && i >= 0 do
            let c = span[i]

            if c <> value0 && c <> value1 then
                found <- true
            else
                i <- i - 1

        if found then i else -1
#endif
