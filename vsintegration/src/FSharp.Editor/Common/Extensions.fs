﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
[<AutoOpen>]
/// Type and Module Extensions
module internal Microsoft.VisualStudio.FSharp.Editor.Extensions

open System
open System.IO
open System.Collections.Immutable

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host

open FSharp.Compiler.Text
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.SyntaxTree

type private FSharpGlyph = FSharp.Compiler.SourceCodeServices.FSharpGlyph
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
