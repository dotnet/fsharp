// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Range

module UnusedOpens =
    open Microsoft.FSharp.Compiler.PrettyNaming

    /// Represents single open statement.
    type OpenStatement =
        { /// All modules which this open declaration effectively opens, including all auto open ones, recursively.
          Modules: FSharpEntity list
          /// Range of open statement itself.
          Range: range
          /// Scope on which this open declaration is applied.
          AppliedScope: range
          /// If it's prefixed with the special "global" namespace.
          IsGlobal: bool }

        member this.AllChildSymbols =
            seq { for modul in this.Modules do
                    for ent in modul.NestedEntities do
                        yield ent :> FSharpSymbol
                        
                        if ent.IsFSharpRecord then
                            for rf in ent.FSharpFields do
                                yield upcast rf
                        
                        if ent.IsFSharpUnion && not (Symbol.hasAttribute<RequireQualifiedAccessAttribute> ent.Attributes) then
                            for unionCase in ent.UnionCases do
                                yield upcast unionCase
                    
                    for fv in modul.MembersFunctionsAndValues do 
                        yield upcast fv
                        
                    for apCase in modul.ActivePatternCases do
                        yield upcast apCase                    
            } |> Seq.cache

    let rec getModuleAndItsAutoOpens (modul: FSharpEntity) =
        [ yield modul
          for ent in modul.NestedEntities do
            if ent.IsFSharpModule && Symbol.hasAttribute<AutoOpenAttribute> ent.Attributes then
              yield! getModuleAndItsAutoOpens ent ]

    let getOpenStatements (openDeclarations: FSharpOpenDeclaration list) : OpenStatement list = 
        openDeclarations
        |> List.filter (fun x -> not x.IsOwnNamespace)
        |> List.choose (fun openDecl ->
             match openDecl.LongId, openDecl.Range with
             | firstId :: _, Some range ->
                 Some { Modules = openDecl.Modules |> List.collect getModuleAndItsAutoOpens
                        Range = range
                        AppliedScope = openDecl.AppliedScope
                        IsGlobal = firstId.idText = MangledGlobalName  }
             | _ -> None)

    let filterSymbolUses (getSourceLineStr: int -> string) (symbolUses: FSharpSymbolUse[]) : FSharpSymbolUse[] =
        symbolUses
        |> Array.filter (fun su ->
             match su.Symbol with
             | :? FSharpMemberOrFunctionOrValue as fv when fv.IsExtensionMember -> 
                // extension members should be taken into account even though they have a prefix (as they do most of the time)
                true
             | _ -> 
                let partialName = QuickParse.GetPartialLongNameEx (getSourceLineStr su.RangeAlternate.StartLine, su.RangeAlternate.EndColumn - 1)
                // for the rest of symbols we pick only those which are the first part of a long idend, because it's they which are
                // conteined in opened namespaces / modules. For example, we pick `IO` from long ident `IO.File.OpenWrite` because
                // it's `open System` which really brings it into scope.
                partialName.QualifyingIdents = [])

    let getUnusedOpens (checkFileResults: FSharpCheckFileResults, getSourceLineStr: int -> string) : Async<range list> =
        
        let filterOpenStatements (openStatements: OpenStatement list) (symbolUses: FSharpSymbolUse[]) : OpenStatement list =
            let rec filterInner acc (openStatements: OpenStatement list) (seenOpenStatements: OpenStatement list) = 
                
                let isUsed (openStatement: OpenStatement) =
                    if openStatement.IsGlobal then true
                    else
                        let usedSomewhere =
                            symbolUses
                            |> Array.exists (fun symbolUse -> 
                                let inScope = rangeContainsRange openStatement.AppliedScope symbolUse.RangeAlternate
                                if not inScope then false
                                else
                                    openStatement.AllChildSymbols
                                    |> Seq.exists (fun x -> x.IsEffectivelySameAs symbolUse.Symbol))

                        if not usedSomewhere then false
                        else
                            let alreadySeen =
                                seenOpenStatements
                                |> List.exists (fun seenNs ->
                                    // if such open statement has already been marked as used in this or outer module, we skip it 
                                    // (that is, do not mark as used so far)
                                    rangeContainsRange seenNs.AppliedScope openStatement.AppliedScope && 
                                    openStatement.Modules |> List.exists (fun x -> seenNs.Modules |> List.exists (fun s -> s.IsEffectivelySameAs x)))
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
            return filterOpenStatements openStatements symbolUses |> List.map (fun os -> os.Range)
        }