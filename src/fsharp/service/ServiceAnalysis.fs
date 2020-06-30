// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open FSharp.Compiler
open FSharp.Compiler.Range
open FSharp.Compiler.PrettyNaming
open System.Collections.Generic
open System.Runtime.CompilerServices
open FSharp.Compiler.AbstractIL.Internal.Library

module UnusedOpens =

    let rec isOpenEntityUsedByEntity first (openEntity: FSharpEntity) (entity: FSharpEntity) =      
        let isEqual = openEntity.Equals entity
        if not isEqual && (first || Symbol.hasAttribute<AutoOpenAttribute> entity.Attributes) then
            match entity.DeclaringEntity with
            | Some entity -> isOpenEntityUsedByEntity false openEntity entity
            | _ -> false
        else
            isEqual

    let isOpenEntityUsedBySymbol (openEntity: FSharpEntity) (symbol: FSharpSymbol) =
        match symbol with
        | :? FSharpMemberOrFunctionOrValue as f ->
            match f.DeclaringEntity with
            | Some entity -> isOpenEntityUsedByEntity true openEntity entity
            | _ -> false
        | :? FSharpEntity as entity ->
            match entity.DeclaringEntity with
            | Some entity -> isOpenEntityUsedByEntity false openEntity entity
            | _ -> false
        | _ -> false

    let isOpenDeclarationUsed (openDeclaration: FSharpOpenDeclaration) (symbolUses: FSharpSymbolUse []) =
        symbolUses
        |> Array.exists (fun symbolUse ->
            if not symbolUse.IsFromOpenStatement && Range.rangeContainsRange openDeclaration.AppliedScope symbolUse.RangeAlternate then
                openDeclaration.Modules |> List.exists (fun x -> isOpenEntityUsedBySymbol x symbolUse.Symbol) ||
                openDeclaration.Types |> List.exists (fun x -> x.HasTypeDefinition && isOpenEntityUsedBySymbol x.TypeDefinition symbolUse.Symbol)
            else
                false)

    let filterOpenStatements (symbolUses: FSharpSymbolUse []) (openDeclarations: FSharpOpenDeclaration []) =
        async {
            return 
                [ for x in openDeclarations do
                    match x.Range with
                    | None -> ()
                    | Some r ->
                        if not (isOpenDeclarationUsed x symbolUses) then
                            r ]
        }

    /// Get the open statements whose contents are not referred to anywhere in the symbol uses.
    /// Async to allow cancellation.
    let getUnusedOpens (checkFileResults: FSharpCheckFileResults, _getSourceLineStr: int -> string) : Async<range list> =
        async {
            let! symbolUses = checkFileResults.GetAllUsesOfAllSymbolsInFile()
            return! filterOpenStatements symbolUses checkFileResults.OpenDeclarations
        }
 
module SimplifyNames = 
    type SimplifiableRange =
        {
          Range: range
          RelativeName: string
        }

    let getPlidLength (plid: string list) = (plid |> List.sumBy String.length) + plid.Length    

    let getSimplifiableNames (checkFileResults: FSharpCheckFileResults, getSourceLineStr: int -> string) : Async<SimplifiableRange list> =
        async {
            let result = ResizeArray()
            let! symbolUses = checkFileResults.GetAllUsesOfAllSymbolsInFile()
            let symbolUses =
                symbolUses
                |> Array.filter (fun symbolUse -> not symbolUse.IsFromOpenStatement)
                |> Array.Parallel.map (fun symbolUse ->
                    let lineStr = getSourceLineStr symbolUse.RangeAlternate.StartLine
                    // for `System.DateTime.Now` it returns ([|"System"; "DateTime"|], "Now")
                    let partialName = QuickParse.GetPartialLongNameEx(lineStr, symbolUse.RangeAlternate.EndColumn - 1)
                    // `symbolUse.RangeAlternate.Start` does not point to the start of plid, it points to start of `name`,
                    // so we have to calculate plid's start ourselves.
                    let plidStartCol = symbolUse.RangeAlternate.EndColumn - partialName.PartialIdent.Length - (getPlidLength partialName.QualifyingIdents)
                    symbolUse, partialName.QualifyingIdents, plidStartCol, partialName.PartialIdent)
                |> Array.filter (fun (_, plid, _, name) -> name <> "" && not (List.isEmpty plid))
                |> Array.groupBy (fun (symbolUse, _, plidStartCol, _) -> symbolUse.RangeAlternate.StartLine, plidStartCol)
                |> Array.map (fun (_, xs) -> xs |> Array.maxBy (fun (symbolUse, _, _, _) -> symbolUse.RangeAlternate.EndColumn))

            for symbolUse, plid, plidStartCol, name in symbolUses do
                if not symbolUse.IsFromDefinition then
                    let posAtStartOfName =
                        let r = symbolUse.RangeAlternate
                        if r.StartLine = r.EndLine then Range.mkPos r.StartLine (r.EndColumn - name.Length)
                        else r.Start   

                    let getNecessaryPlid (plid: string list) : Async<string list> =
                        let rec loop (rest: string list) (current: string list) =
                            async {
                                match rest with
                                | [] -> return current
                                | headIdent :: restPlid ->
                                    let! res = checkFileResults.IsRelativeNameResolvableFromSymbol(posAtStartOfName, current, symbolUse.Symbol)
                                    if res then return current
                                    else return! loop restPlid (headIdent :: current)
                            }
                        loop (List.rev plid) []
                    
                    let! necessaryPlid = getNecessaryPlid plid 
                    
                    match necessaryPlid with
                    | necessaryPlid when necessaryPlid = plid -> ()
                    | necessaryPlid ->
                        let r = symbolUse.RangeAlternate
                        let necessaryPlidStartCol = r.EndColumn - name.Length - (getPlidLength necessaryPlid)
                    
                        let unnecessaryRange = 
                            Range.mkRange r.FileName (Range.mkPos r.StartLine plidStartCol) (Range.mkPos r.EndLine necessaryPlidStartCol)
                    
                        let relativeName = (String.concat "." plid) + "." + name
                        result.Add({Range = unnecessaryRange; RelativeName = relativeName})

            return List.ofSeq result
        }

module UnusedDeclarations = 
    let isPotentiallyUnusedDeclaration (symbol: FSharpSymbol) : bool =

        match symbol with

        // Determining that a record, DU or module is used anywhere requires inspecting all their enclosed entities (fields, cases and func / vals)
        // for usages, which is too expensive to do. Hence we never gray them out.
        | :? FSharpEntity as e when e.IsFSharpRecord || e.IsFSharpUnion || e.IsInterface || e.IsFSharpModule || e.IsClass || e.IsNamespace -> false

        // FCS returns inconsistent results for override members; we're skipping these symbols.
        | :? FSharpMemberOrFunctionOrValue as f when 
                f.IsOverrideOrExplicitInterfaceImplementation ||
                f.IsBaseValue ||
                f.IsConstructor -> false

        // Usage of DU case parameters does not give any meaningful feedback; we never gray them out.
        | :? FSharpParameter -> false
        | _ -> true

    let getUnusedDeclarationRanges (symbolsUses: FSharpSymbolUse[]) (isScript: bool) =
        let definitions =
            symbolsUses
            |> Array.filter (fun su -> 
                su.IsFromDefinition && 
                su.Symbol.DeclarationLocation.IsSome && 
                (isScript || su.IsPrivateToFile) && 
                not (su.Symbol.DisplayName.StartsWith "_") &&
                isPotentiallyUnusedDeclaration su.Symbol)

        let usages =
            let usages = 
                symbolsUses
                |> Array.filter (fun su -> not su.IsFromDefinition)
                |> Array.choose (fun su -> su.Symbol.DeclarationLocation)
            HashSet(usages)

        let unusedRanges =
            definitions
            |> Array.map (fun defSu -> defSu, usages.Contains defSu.Symbol.DeclarationLocation.Value)
            |> Array.groupBy (fun (defSu, _) -> defSu.RangeAlternate)
            |> Array.filter (fun (_, defSus) -> defSus |> Array.forall (fun (_, isUsed) -> not isUsed))
            |> Array.map (fun (m, _) -> m)

        Array.toList unusedRanges
    
    let getUnusedDeclarations(checkFileResults: FSharpCheckFileResults, isScriptFile: bool) : Async<range list> = 
        async {
            let! allSymbolUsesInFile = checkFileResults.GetAllUsesOfAllSymbolsInFile()
            let unusedRanges = getUnusedDeclarationRanges allSymbolUsesInFile isScriptFile
            return unusedRanges
        }