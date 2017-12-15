// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Range

module UnusedOpens =
    open Microsoft.FSharp.Compiler.PrettyNaming
    open System.Runtime.CompilerServices

    type Module =
        { Entity: FSharpEntity
          IsNestedAutoOpen: bool }

        member this.ChildSymbols =
            seq { for ent in this.Entity.NestedEntities do
                      yield ent :> FSharpSymbol
                      
                      if ent.IsFSharpRecord then
                          for rf in ent.FSharpFields do
                              yield upcast rf
                      
                      if ent.IsFSharpUnion && not (Symbol.hasAttribute<RequireQualifiedAccessAttribute> ent.Attributes) then
                          for unionCase in ent.UnionCases do
                              yield upcast unionCase

                      if Symbol.hasAttribute<ExtensionAttribute> ent.Attributes then
                          for fv in ent.MembersFunctionsAndValues do
                              // fv.IsExtensionMember is always false for C# extension methods returning by `MembersFunctionsAndValues`,
                              // so we have to check Extension attribute instead. 
                              // (note: fv.IsExtensionMember has proper value for symbols returning by GetAllUsesOfAllSymbolsInFile though)
                              if Symbol.hasAttribute<ExtensionAttribute> fv.Attributes then
                                  yield upcast fv
                      
                  for apCase in this.Entity.ActivePatternCases do
                      yield upcast apCase                    

                  for fv in this.Entity.MembersFunctionsAndValues do 
                      yield upcast fv
            } |> Seq.cache

    type ModuleGroup = 
        { Modules: Module list }
        
        static member Create (modul: FSharpEntity) =
            let rec getModuleAndItsAutoOpens (isNestedAutoOpen: bool) (modul: FSharpEntity) =
                [ yield { Entity = modul; IsNestedAutoOpen = isNestedAutoOpen }
                  for ent in modul.NestedEntities do
                    if ent.IsFSharpModule && Symbol.hasAttribute<AutoOpenAttribute> ent.Attributes then
                      yield! getModuleAndItsAutoOpens true ent ]
            { Modules = getModuleAndItsAutoOpens false modul }

    /// Represents single open statement.
    type OpenStatement =
        { /// All modules which this open declaration effectively opens, _not_ including auto open ones.
          Modules: ModuleGroup list
          /// Range of open statement itself.
          Range: range
          /// Scope on which this open declaration is applied.
          AppliedScope: range }

    let getOpenStatements (openDeclarations: FSharpOpenDeclaration list) : OpenStatement list = 
        openDeclarations
        |> List.filter (fun x -> not x.IsOwnNamespace)
        |> List.choose (fun openDecl ->
             match openDecl.LongId, openDecl.Range with
             | firstId :: _, Some range ->
                 if firstId.idText = MangledGlobalName then 
                     None
                 else
                     Some { Modules = openDecl.Modules |> List.map ModuleGroup.Create
                            Range = range
                            AppliedScope = openDecl.AppliedScope }
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

    type UsedModule =
        { Module: FSharpEntity
          AppliedScope: range }

    let getUnusedOpens (checkFileResults: FSharpCheckFileResults, getSourceLineStr: int -> string) : Async<range list> =
        
        let filterOpenStatements (openStatements: OpenStatement list) (symbolUses: FSharpSymbolUse[]) : OpenStatement list =
            
            let rec filterInner acc (openStatements: OpenStatement list) (usedModules: UsedModule list) = 
            
                let getUsedModules (openStatement: OpenStatement) =
                    let notAlreadyUsedModuleGroups =
                        openStatement.Modules
                        |> List.choose (fun x ->
                             let notUsedModules =
                                x.Modules
                                |> List.filter (fun x ->
                                      not (usedModules
                                           |> List.exists (fun used ->
                                                rangeContainsRange used.AppliedScope openStatement.AppliedScope &&
                                                used.Module.IsEffectivelySameAs x.Entity)))
                             
                             match notUsedModules with
                             | [] -> None
                             | _ when notUsedModules |> List.exists (fun x -> not x.IsNestedAutoOpen) -> 
                                Some { Modules = notUsedModules }
                             | _ -> None)

                    match notAlreadyUsedModuleGroups with
                    | [] -> []
                    | _ ->
                        let symbolUsesInScope = symbolUses |> Array.filter (fun symbolUse -> rangeContainsRange openStatement.AppliedScope symbolUse.RangeAlternate)
                        notAlreadyUsedModuleGroups
                        |> List.filter (fun modulGroup ->
                             modulGroup.Modules
                             |> List.exists (fun modul ->
                                  symbolUsesInScope
                                  |> Array.exists (fun symbolUse ->
                                       let usedByEnclosingEntity =
                                           match symbolUse.Symbol with
                                           | :? FSharpMemberOrFunctionOrValue as f ->
                                                match f.EnclosingEntity with
                                                | Some ent when ent.IsNamespace || ent.IsFSharpModule ->
                                                    Some (ent.IsEffectivelySameAs modul.Entity)
                                                | _ -> None
                                           | _ -> None
                                       match usedByEnclosingEntity with
                                       | Some x -> x
                                       | None -> modul.ChildSymbols |> Seq.exists (fun x -> x.IsEffectivelySameAs symbolUse.Symbol)
                                     )))
                        |> List.collect (fun mg -> 
                            mg.Modules |> List.map (fun x -> { Module = x.Entity; AppliedScope = openStatement.AppliedScope }))
                                          
                match openStatements with
                | os :: xs ->
                    match getUsedModules os with
                    | [] -> filterInner (os :: acc) xs usedModules
                    | um -> filterInner acc xs (um @ usedModules)
                | [] -> List.rev acc
            
            filterInner [] openStatements []

        async {
            let! symbolUses = checkFileResults.GetAllUsesOfAllSymbolsInFile()
            let symbolUses = filterSymbolUses getSourceLineStr symbolUses
            let openStatements = getOpenStatements checkFileResults.OpenDeclarations
            return filterOpenStatements openStatements symbolUses |> List.map (fun os -> os.Range)
        }