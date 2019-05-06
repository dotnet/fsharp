namespace FSharp.Compiler.Compilation

open System
open System.IO
open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable
open System.Collections.Generic
open System.Collections.Concurrent
open Internal.Utilities.Collections
open FSharp.Compiler
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Ast
open FSharp.Compiler.Tastops
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Range
open FSharp.Compiler.Driver
open FSharp.Compiler.Tast
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.CompileOptions
open FSharp.Compiler.TypeChecker
open FSharp.Compiler.NameResolution
open Internal.Utilities
open FSharp.Compiler.Compilation.Utilities

type LinesStorage () =

    // TODO: Yes a dictionary is bad, but quick for now.
    let lines = Dictionary<int, ResizeArray<TcSymbolUseData>> ()
    let linesMethodGroup = Dictionary<int, ResizeArray<TcSymbolUseData>> ()

    let getTableList (dic: Dictionary<_, ResizeArray<_>>) key =
        match dic.TryGetValue key with
        | true, child -> child
        | _ ->
            let child = ResizeArray ()
            dic.[key] <- child
            child

    member __.Add (cnr: CapturedNameResolution, m: range) =
        let symbols = getTableList lines m.EndLine
        symbols.Add { Item = cnr.Item; ItemOccurence = cnr.ItemOccurence; DisplayEnv = cnr.DisplayEnv; Range = cnr.Range }

    member __.AddMethodGroup (cnr: CapturedNameResolution, m: range) =
        let symbols = getTableList linesMethodGroup m.EndLine
        symbols.Add  { Item = cnr.Item; ItemOccurence = cnr.ItemOccurence; DisplayEnv = cnr.DisplayEnv; Range = cnr.Range }

    member __.RemoveAll (m: range) =
        for pair in lines do
            pair.Value.RemoveAll (fun cnr -> Range.equals cnr.Range m) |> ignore

    member __.RemoveAllMethodGroup (m: range) =
        for pair in linesMethodGroup do
            pair.Value.RemoveAll (fun cnr -> Range.equals cnr.Range m) |> ignore

    //member __.ForEach (m: range, f) =
    //    match lines.TryGetValue line with
    //    | false, _ -> None
    //    | true, symbols ->
    //        let mutable result = None
    //        let mutable i = 0
    //        while i < symbols.Count && result.IsNone do
    //            let symbol = symbols.[i]
    //            if Range.rangeContainsPos symbol.Range (mkPos line column) then
    //                result <- Some symbol
    //            i <- i + 1
    //        result

[<Sealed>]
type CheckerSink (g: TcGlobals) =
    let capturedEnvs = ResizeArray<_>()
    let capturedExprTypings = ResizeArray<_>()
    let capturedOpenDeclarations = ResizeArray<OpenDeclaration>()
    let capturedFormatSpecifierLocations = ResizeArray<_>()

    let allowedRange (m: Range.range) = not m.IsSynthetic

    let capturedNameResolutionIdentifiers =
        new System.Collections.Generic.HashSet<pos * string>
            ( { new IEqualityComparer<_> with
                    member __.GetHashCode((p: pos, i)) = p.Line + 101 * p.Column + hash i
                    member __.Equals((p1, i1), (p2, i2)) = posEq p1 p2 && i1 =  i2 } )

    let capturedModulesAndNamespaces =
        new System.Collections.Generic.HashSet<range * Item>
            ( { new IEqualityComparer<range * Item> with
                    member __.GetHashCode ((m, _)) = hash m
                    member __.Equals ((m1, item1), (m2, item2)) = Range.equals m1 m2 && ItemsAreEffectivelyEqual g item1 item2 } )

    let linesStorage = LinesStorage ()

    interface ITypecheckResultsSink with
        member sink.NotifyEnvWithScope(m, nenv, ad) =
            if allowedRange m then
                capturedEnvs.Add((m, nenv, ad))

        member sink.NotifyExprHasType(endPos, ty, denv, nenv, ad, m) =
            if allowedRange m then
                capturedExprTypings.Add((endPos, ty, denv, nenv, ad, m))

        member sink.NotifyNameResolution(endPos, item, itemMethodGroup, tpinst, occurenceType, denv, nenv, ad, m, replace) =
            // Desugaring some F# constructs (notably computation expressions with custom operators)
            // results in duplication of textual variables. So we ensure we never record two name resolutions
            // for the same identifier at the same location.
            if allowedRange m then
                if replace then 
                    linesStorage.RemoveAll m
                    linesStorage.RemoveAllMethodGroup m
                else
                    let alreadyDone =
                        match item with
                        | Item.ModuleOrNamespaces _ ->
                            not (capturedModulesAndNamespaces.Add (m, item))
                        | _ ->
                            let keyOpt =
                                match item with
                                | Item.Value vref -> Some (endPos, vref.DisplayName)
                                | Item.ArgName (id, _, _) -> Some (endPos, id.idText)
                                | _ -> None

                            match keyOpt with
                            | Some key -> not (capturedNameResolutionIdentifiers.Add key)
                            | _ -> false

                    if not alreadyDone then
                        let cnr = CapturedNameResolution(endPos, item, tpinst, occurenceType, denv, nenv, ad, m)
                        let cnrMethodGroup = CapturedNameResolution(endPos, itemMethodGroup, [], occurenceType, denv, nenv, ad, m)
                        linesStorage.Add (cnr, m)
                        linesStorage.Add (cnrMethodGroup, m)


        member sink.NotifyFormatSpecifierLocation(m, numArgs) =
            capturedFormatSpecifierLocations.Add((m, numArgs))

        member sink.NotifyOpenDeclaration openDeclaration =
            capturedOpenDeclarations.Add openDeclaration

        member sink.CurrentSourceText = None

        member sink.FormatStringCheckContext = None