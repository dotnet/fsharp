// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.PrettyNaming
open System.Collections.Generic
open System.Runtime.CompilerServices

module UnusedOpens =

    let symbolHash = HashIdentity.FromFunctions (fun (x: FSharpSymbol) -> x.GetEffectivelySameAsHash()) (fun x y -> x.IsEffectivelySameAs(y))

    /// Represents one namespace or module opened by an 'open' statement
    type OpenedModule(entity: FSharpEntity, isNestedAutoOpen: bool) = 

        /// Compute an indexed table of the set of symbols revealed by 'open', on-demand
        let revealedSymbols : Lazy<HashSet<FSharpSymbol>> =
           lazy
            let symbols = 
               [| for ent in entity.NestedEntities do
                      yield ent :> FSharpSymbol
                      
                      if ent.IsFSharpRecord then
                          for rf in ent.FSharpFields do
                              yield rf  :> FSharpSymbol
                      
                      if ent.IsFSharpUnion && not (Symbol.hasAttribute<RequireQualifiedAccessAttribute> ent.Attributes) then
                          for unionCase in ent.UnionCases do
                              yield unionCase :> FSharpSymbol

                      if Symbol.hasAttribute<ExtensionAttribute> ent.Attributes then
                          for fv in ent.MembersFunctionsAndValues do
                              // fv.IsExtensionMember is always false for C# extension methods returning by `MembersFunctionsAndValues`,
                              // so we have to check Extension attribute instead. 
                              // (note: fv.IsExtensionMember has proper value for symbols returning by GetAllUsesOfAllSymbolsInFile though)
                              if Symbol.hasAttribute<ExtensionAttribute> fv.Attributes then
                                  yield fv :> FSharpSymbol
                      
                  for apCase in entity.ActivePatternCases do
                      yield apCase :> FSharpSymbol

                  // The IsNamespace and IsFSharpModule cases are handled by looking at DeclaringEntity below
                  if not entity.IsNamespace && not entity.IsFSharpModule then
                      for fv in entity.MembersFunctionsAndValues do 
                          yield fv :> FSharpSymbol |]

            HashSet<_>(symbols, symbolHash)

        member __.Entity = entity
        member __.IsNestedAutoOpen = isNestedAutoOpen
        member __.RevealedSymbolsContains(symbol) = revealedSymbols.Force().Contains symbol

    type OpenedModuleGroup = 
        { OpenedModules: OpenedModule list }
        
        static member Create (modul: FSharpEntity) =
            let rec getModuleAndItsAutoOpens (isNestedAutoOpen: bool) (modul: FSharpEntity) =
                [ yield OpenedModule (modul, isNestedAutoOpen)
                  for ent in modul.NestedEntities do
                    if ent.IsFSharpModule && Symbol.hasAttribute<AutoOpenAttribute> ent.Attributes then
                      yield! getModuleAndItsAutoOpens true ent ]
            { OpenedModules = getModuleAndItsAutoOpens false modul }

    /// Represents single open statement.
    type OpenStatement =
        { /// All namespaces and modules which this open declaration effectively opens, including the AutoOpen ones
          OpenedGroups: OpenedModuleGroup list

          /// The range of open statement itself
          Range: range

          /// The scope on which this open declaration is applied
          AppliedScope: range }

    /// Gets the open statements, their scopes and their resolutions
    let getOpenStatements (openDeclarations: FSharpOpenDeclaration[]) : OpenStatement[] = 
        openDeclarations
        |> Array.filter (fun x -> not x.IsOwnNamespace)
        |> Array.choose (fun openDecl ->
             match openDecl.LongId, openDecl.Range with
             | firstId :: _, Some range ->
                 if firstId.idText = MangledGlobalName then 
                     None
                 else
                     Some { OpenedGroups = openDecl.Modules |> List.map OpenedModuleGroup.Create
                            Range = range
                            AppliedScope = openDecl.AppliedScope }
             | _ -> None)

    /// Only consider symbol uses which are the first part of a long ident, i.e. with no qualifying identifiers
    let filterSymbolUses (getSourceLineStr: int -> string) (symbolUses: FSharpSymbolUse[]) : FSharpSymbolUse[] =
        symbolUses
        |> Array.filter (fun su ->
             match su.Symbol with
             | :? FSharpMemberOrFunctionOrValue as fv when fv.IsExtensionMember -> 
                // Extension members should be taken into account even though they have a prefix (as they do most of the time)
                true
             | :? FSharpMemberOrFunctionOrValue as fv when not fv.IsModuleValueOrMember -> 
                // Local values can be ignored
                false
             | :? FSharpGenericParameter -> 
                // Generic parameters can be ignored, they never come into scope via 'open'
                false
             | _ -> 
                // For the rest of symbols we pick only those which are the first part of a long ident, because it's they which are
                // contained in opened namespaces / modules. For example, we pick `IO` from long ident `IO.File.OpenWrite` because
                // it's `open System` which really brings it into scope.
                let partialName = QuickParse.GetPartialLongNameEx (getSourceLineStr su.RangeAlternate.StartLine, su.RangeAlternate.EndColumn - 1)
                List.isEmpty partialName.QualifyingIdents)

    /// Split symbol uses into cases that are easy to handle (via DeclaringEntity) and those that don't have a good DeclaringEntity
    let splitSymbolUses (symbolUses: FSharpSymbolUse[]) : FSharpSymbolUse[] * FSharpSymbolUse[] =
        symbolUses |> Array.partition (fun symbolUse ->
            let symbol = symbolUse.Symbol
            match symbol with
            | :? FSharpMemberOrFunctionOrValue as f ->
                match f.DeclaringEntity with
                | Some ent when ent.IsNamespace || ent.IsFSharpModule -> true
                | _ -> false
            | _ -> false)

    /// Represents intermediate tracking data used to track the modules which are known to have been used so far
    type UsedModule =
        { Module: FSharpEntity
          AppliedScope: range }

    /// Given an 'open' statement, find fresh modules/namespaces referred to by that statement where there is some use of a revealed symbol
    /// in the scope of the 'open' is from that module.
    ///
    /// Performance will be roughly NumberOfOpenStatements x NumberOfSymbolUses
    let getUsedModules (symbolUses1: FSharpSymbolUse[], symbolUses2: FSharpSymbolUse[]) (usedModules: UsedModule list) (openStatement: OpenStatement) =

        // Don't re-check modules whose symbols are already known to have been used
        let openedGroupsToExamine =
            openStatement.OpenedGroups |> List.choose (fun openedGroup ->
                let openedEntitiesToExamine =
                    openedGroup.OpenedModules 
                    |> List.filter (fun openedEntity ->
                        not (usedModules
                            |> List.exists (fun used ->
                                rangeContainsRange used.AppliedScope openStatement.AppliedScope &&
                                used.Module.IsEffectivelySameAs openedEntity.Entity)))
                             
                match openedEntitiesToExamine with
                | [] -> None
                | _ when openedEntitiesToExamine |> List.exists (fun x -> not x.IsNestedAutoOpen) -> Some { OpenedModules = openedEntitiesToExamine }
                | _ -> None)

        // Find the opened groups that are used by some symbol use
        let newlyUsedOpenedGroups = 
            openedGroupsToExamine |> List.filter (fun openedGroup ->
                openedGroup.OpenedModules |> List.exists (fun openedEntity ->

                    symbolUses1 |> Array.exists (fun symbolUse ->
                        rangeContainsRange openStatement.AppliedScope symbolUse.RangeAlternate &&
                        match symbolUse.Symbol with
                        | :? FSharpMemberOrFunctionOrValue as f ->
                            match f.DeclaringEntity with
                            | Some ent when ent.IsNamespace || ent.IsFSharpModule -> ent.IsEffectivelySameAs openedEntity.Entity
                            | _ -> false
                        | _ -> false) || 

                    symbolUses2 |> Array.exists (fun symbolUse ->
                        rangeContainsRange openStatement.AppliedScope symbolUse.RangeAlternate &&
                        openedEntity.RevealedSymbolsContains symbolUse.Symbol)))

        // Return them as interim used entities
        newlyUsedOpenedGroups |> List.collect (fun openedGroup -> 
                openedGroup.OpenedModules |> List.map (fun x -> { Module = x.Entity; AppliedScope = openStatement.AppliedScope }))
                                          
    /// Incrementally filter out the open statements one by one. Filter those whose contents are referred to somewhere in the symbol uses.
    /// Async to allow cancellation.
    let rec filterOpenStatementsIncremental symbolUses (openStatements: OpenStatement list) (usedModules: UsedModule list) acc = 
        async { 
            match openStatements with
            | openStatement :: rest ->
                match getUsedModules symbolUses usedModules openStatement with
                | [] -> 
                    // The open statement has not been used, include it in the results
                    return! filterOpenStatementsIncremental symbolUses rest usedModules (openStatement :: acc)
                | moreUsedModules -> 
                    // The open statement has been used, add the modules which are already known to be used to the list of things we don't need to re-check
                    return! filterOpenStatementsIncremental symbolUses rest (moreUsedModules @ usedModules) acc
            | [] -> return List.rev acc
        }

    /// Filter out the open statements whose contents are referred to somewhere in the symbol uses.
    /// Async to allow cancellation.
    let filterOpenStatements symbolUses openStatements =
        async {
            let! results = filterOpenStatementsIncremental symbolUses (List.ofArray openStatements) [] []
            return results |> List.map (fun os -> os.Range)
        }

    /// Get the open statements whose contents are not referred to anywhere in the symbol uses.
    /// Async to allow cancellation.
    let getUnusedOpens (checkFileResults: FSharpCheckFileResults, getSourceLineStr: int -> string) : Async<range list> =
        async {
            let! symbolUses = checkFileResults.GetAllUsesOfAllSymbolsInFile()
            let symbolUses = filterSymbolUses getSourceLineStr symbolUses
            let symbolUses = splitSymbolUses symbolUses
            let openStatements = getOpenStatements checkFileResults.OpenDeclarations
            return! filterOpenStatements symbolUses openStatements
        }