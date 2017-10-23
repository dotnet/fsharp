// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Range

module UnusedOpens =
    open Microsoft.FSharp.Compiler.PrettyNaming

    /// Represents single open statement.
    type OpenStatement =
        { /// Full namespace or module identifier as it's presented in source code.
          Idents: Set<string>
          /// Modules.
          Modules: FSharpEntity list
          /// Range of open statement itself.
          Range: range
          /// Scope on which this open declaration is applied.
          AppliedScope: range
          /// If it's prefixed with the special "global" namespace.
          IsGlobal: bool }

        member this.AllChildSymbols =
            let rec getAllChildSymbolsInModule (modul: FSharpEntity) =
                seq {
                    for ent in modul.NestedEntities do
                        yield ent :> FSharpSymbol
                        
                        if ent.IsFSharpRecord then
                            for rf in ent.FSharpFields do
                                yield upcast rf
                        
                        if ent.IsFSharpUnion && not (hasAttribute<RequireQualifiedAccessAttribute> ent.Attributes) then
                            for unionCase in ent.UnionCases do
                                yield upcast unionCase

                        if ent.IsFSharpModule && hasAttribute<AutoOpenAttribute> ent.Attributes then
                            yield! getAllChildSymbolsInModule ent

                    for fv in modul.MembersFunctionsAndValues do 
                        yield upcast fv
                        
                    for apCase in modul.ActivePatternCases do
                        yield upcast apCase
                }

            seq { for modul in this.Modules do
                    yield! getAllChildSymbolsInModule modul
            } |> Seq.cache

    let getOpenStatements (openDeclarations: FSharpOpenDeclaration list) : OpenStatement list = 
        openDeclarations
        |> List.choose (fun openDeclaration ->
             match openDeclaration with
             | FSharpOpenDeclaration.Open ((firstId :: _) as longId, modules, appliedScope) ->
                 Some { Idents = 
                            modules 
                            |> List.choose (fun x -> x.TryFullName |> Option.map (fun fullName -> x, fullName)) 
                            |> List.collect (fun (modul, fullName) -> 
                                 [ yield fullName
                                   if modul.HasFSharpModuleSuffix then
                                     yield fullName.[..fullName.Length - 7] // "Module" length plus zero index correction
                                 ])
                            |> Set.ofList
                        Modules = modules
                        Range =
                            let lastId = List.last longId
                            mkRange appliedScope.FileName firstId.idRange.Start lastId.idRange.End
                        AppliedScope = appliedScope
                        IsGlobal = firstId.idText = MangledGlobalName  }
             | _ -> None // for now
           )

    let filterSymbolUses (getSourceLineStr: int -> string) (symbolUses: FSharpSymbolUse[]) : FSharpSymbolUse[] =
        symbolUses
        |> Array.filter (fun su -> not su.IsFromDefinition)
        |> Array.filter (fun su ->
             match su.Symbol with
             | :? FSharpMemberOrFunctionOrValue as fv when fv.IsExtensionMember -> true
             | _ -> 
                let partialName = QuickParse.GetPartialLongNameEx (getSourceLineStr su.RangeAlternate.StartLine, su.RangeAlternate.EndColumn - 1)
                partialName.PartialIdent <> "" && partialName.QualifyingIdents = [])

    let getUnusedOpens (checkFileResults: FSharpCheckFileResults, getSourceLineStr: int -> string) : Async<range list> =
        
        let filter (openStatements: OpenStatement list) (symbolUses: FSharpSymbolUse[]) : OpenStatement list =
            let rec filterInner acc (openStatements: OpenStatement list) (seenOpenStatements: OpenStatement list) = 
                
                let isUsed (openStatement: OpenStatement) =
                    if openStatement.IsGlobal then true
                    else
                        let usedSomewhere =
                            symbolUses
                            |> Array.exists (fun symbolUse -> 
                                let inScope = rangeContainsRange openStatement.AppliedScope symbolUse.RangeAlternate
                                if not inScope then false
                                //elif openStatement.Idents |> Set.intersect symbolUse.PossibleNamespaces |> Set.isEmpty then false
                                else
                                    let moduleSymbols = openStatement.AllChildSymbols |> Seq.toList
                                    moduleSymbols
                                    |> List.exists (fun x -> x.IsEffectivelySameAs symbolUse.Symbol))

                        if not usedSomewhere then false
                        else
                            let alreadySeen =
                                seenOpenStatements
                                |> List.exists (fun seenNs ->
                                    // if such open statement has already been marked as used in this or outer module, we skip it 
                                    // (that is, do not mark as used so far)
                                    rangeContainsRange seenNs.AppliedScope openStatement.AppliedScope && 
                                    not (openStatement.Idents |> Set.intersect seenNs.Idents |> Set.isEmpty))
                            not alreadySeen
                
                match openStatements with
                | os :: xs when not (isUsed os) -> 
                    filterInner (os :: acc) xs (os :: seenOpenStatements)
                | os :: xs ->
                    filterInner acc xs (os :: seenOpenStatements)
                | [] -> List.rev acc
            
            filterInner [] openStatements []

        async {
            let! symbolUses = checkFileResults.GetAllUsesOfAllSymbolsInFile()
            let symbolUses = filterSymbolUses getSourceLineStr symbolUses
            let openStatements = getOpenStatements checkFileResults.OpenDeclarations
            return filter openStatements symbolUses |> List.map (fun os -> os.Range)
        }