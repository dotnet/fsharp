// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
[<AutoOpen>]
/// Type and Module Extensions
module internal Microsoft.VisualStudio.FSharp.Editor.Extensions

open System
open System.IO
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host
open Microsoft.FSharp.Compiler.Text
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.SourceCodeServices


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
                    override __.GetHashCode() =
                        let checksum = sourceText.GetChecksum()
                        let contentsHash = if not checksum.IsDefault then Hash.combineValues checksum else 0
                        let encodingHash = if not (isNull sourceText.Encoding) then sourceText.Encoding.GetHashCode() else 0

                        sourceText.ChecksumAlgorithm.GetHashCode()
                        |> Hash.combine encodingHash
                        |> Hash.combine contentsHash
                        |> Hash.combine sourceText.Length

                interface ISourceText with
            
                    member __.Item with get index = sourceText.[index]

                    member __.GetLineString(lineIndex) =
                        sourceText.Lines.[lineIndex].ToString()

                    member __.GetLineCount() =
                        sourceText.Lines.Count

                    member __.GetLastCharacterPosition() =
                        if sourceText.Lines.Count > 0 then
                            (sourceText.Lines.Count, sourceText.Lines.[sourceText.Lines.Count - 1].Span.Length)
                        else
                            (0, 0)

                    member __.GetSubTextString(start, length) =
                        sourceText.GetSubText(TextSpan(start, length)).ToString()

                    member __.SubTextEquals(target, startIndex) =
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

                    member __.ContentEquals(sourceText) =
                        match sourceText with
                        | :? SourceText as sourceText -> sourceText.ContentEquals(sourceText)
                        | _ -> false

                    member __.Length = sourceText.Length

                    member __.CopyTo(sourceIndex, destination, destinationIndex, count) =
                        sourceText.CopyTo(sourceIndex, destination, destinationIndex, count)
            }

        sourceText

type SourceText with

    member this.ToFSharpSourceText() =
        SourceText.weakTable.GetValue(this, Runtime.CompilerServices.ConditionalWeakTable<_,_>.CreateValueCallback(SourceText.create))

type FSharpNavigationDeclarationItem with
    member x.RoslynGlyph : Glyph =
        match x.Glyph with
        | FSharpGlyph.Class
        | FSharpGlyph.Typedef
        | FSharpGlyph.Type
        | FSharpGlyph.Exception ->
            match x.Access with
            | Some SynAccess.Private -> Glyph.ClassPrivate
            | Some SynAccess.Internal -> Glyph.ClassInternal
            | _ -> Glyph.ClassPublic
        | FSharpGlyph.Constant -> 
            match x.Access with
            | Some SynAccess.Private -> Glyph.ConstantPrivate
            | Some SynAccess.Internal -> Glyph.ConstantInternal
            | _ -> Glyph.ConstantPublic
        | FSharpGlyph.Delegate -> 
            match x.Access with
            | Some SynAccess.Private -> Glyph.DelegatePrivate
            | Some SynAccess.Internal -> Glyph.DelegateInternal
            | _ -> Glyph.DelegatePublic
        | FSharpGlyph.Union
        | FSharpGlyph.Enum -> 
            match x.Access with
            | Some SynAccess.Private -> Glyph.EnumPrivate
            | Some SynAccess.Internal -> Glyph.EnumInternal
            | _ -> Glyph.EnumPublic
        | FSharpGlyph.EnumMember
        | FSharpGlyph.Variable
        | FSharpGlyph.Field -> 
            match x.Access with
            | Some SynAccess.Private -> Glyph.FieldPrivate
            | Some SynAccess.Internal -> Glyph.FieldInternal
            | _ -> Glyph.FieldPublic
        | FSharpGlyph.Event -> 
            match x.Access with
            | Some SynAccess.Private -> Glyph.EventPrivate
            | Some SynAccess.Internal -> Glyph.EventInternal
            | _ -> Glyph.EventPublic
        | FSharpGlyph.Interface -> 
            match x.Access with
            | Some SynAccess.Private -> Glyph.InterfacePrivate
            | Some SynAccess.Internal -> Glyph.InterfaceInternal
            | _ -> Glyph.InterfacePublic
        | FSharpGlyph.Method
        | FSharpGlyph.OverridenMethod -> 
            match x.Access with
            | Some SynAccess.Private -> Glyph.MethodPrivate
            | Some SynAccess.Internal -> Glyph.MethodInternal
            | _ -> Glyph.MethodPublic
        | FSharpGlyph.Module -> 
            match x.Access with
            | Some SynAccess.Private -> Glyph.ModulePrivate
            | Some SynAccess.Internal -> Glyph.ModuleInternal
            | _ -> Glyph.ModulePublic
        | FSharpGlyph.NameSpace -> Glyph.Namespace
        | FSharpGlyph.Property -> 
            match x.Access with
            | Some SynAccess.Private -> Glyph.PropertyPrivate
            | Some SynAccess.Internal -> Glyph.PropertyInternal
            | _ -> Glyph.PropertyPublic
        | FSharpGlyph.Struct -> 
            match x.Access with
            | Some SynAccess.Private -> Glyph.StructurePrivate
            | Some SynAccess.Internal -> Glyph.StructureInternal
            | _ -> Glyph.StructurePublic
        | FSharpGlyph.ExtensionMethod ->
            match x.Access with
            | Some SynAccess.Private -> Glyph.ExtensionMethodPrivate
            | Some SynAccess.Internal -> Glyph.ExtensionMethodInternal
            | _ -> Glyph.ExtensionMethodPublic
        | FSharpGlyph.Error -> Glyph.Error



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

    let inline ofNull value =
        if obj.ReferenceEquals(value, null) then None else Some value

    /// Gets the option if Some x, otherwise try to get another value
    let inline orTry f =
        function
        | Some x -> Some x
        | None -> f()

    /// Gets the value if Some x, otherwise try to get another value by calling a function
    let inline getOrTry f =
        function
        | Some x -> x
        | None -> f()

    /// Returns 'Some list' if all elements in the list are Some, otherwise None
    let ofOptionList (xs : 'a option list) : 'a list option =

        if xs |> List.forall Option.isSome then
            xs |> List.map Option.get |> Some
        else
            None

[<RequireQualifiedAccess>]
module Seq =
    open System.Collections.Immutable

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

    /// Optimized arrays equality. ~100x faster than `array1 = array2` on strings.
    /// ~2x faster for floats
    /// ~0.8x slower for ints
    let areEqual (xs: 'T []) (ys: 'T []) =
        match xs, ys with
        | null, null -> true
        | [||], [||] -> true
        | null, _ | _, null -> false
        | _ when xs.Length <> ys.Length -> false
        | _ ->
            let mutable stop = false
            let mutable i = 0
            let mutable result = true
            while i < xs.Length && not stop do
                if xs.[i] <> ys.[i] then 
                    stop <- true
                    result <- false
                i <- i + 1
            result
    
    /// check if subArray is found in the wholeArray starting 
    /// at the provided index
    let isSubArray (subArray: 'T []) (wholeArray:'T []) index = 
        if isNull subArray || isNull wholeArray then false
        elif subArray.Length = 0 then true
        elif subArray.Length > wholeArray.Length then false
        elif subArray.Length = wholeArray.Length then areEqual subArray wholeArray else
        let rec loop subidx idx =
            if subidx = subArray.Length then true 
            elif subArray.[subidx] = wholeArray.[idx] then loop (subidx+1) (idx+1) 
            else false
        loop 0 index
        
    /// Returns true if one array has another as its subset from index 0.
    let startsWith (prefix: _ []) (whole: _ []) =
        isSubArray prefix whole 0
        
    /// Returns true if one array has trailing elements equal to another's.
    let endsWith (suffix: _ []) (whole: _ []) =
        isSubArray suffix whole (whole.Length-suffix.Length)


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
