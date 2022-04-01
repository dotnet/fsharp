// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace FSharp.Compiler.EditorServices

open System
open System.Collections.Generic
open Internal.Utilities.Library 
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.IO
open FSharp.Compiler.Symbols

type IsAutoOpen = bool

[<RequireQualifiedAccess>]
type LookupType =
    | Fuzzy
    | Precise

[<NoComparison; NoEquality>]
type AssemblySymbol = 
    { FullName: string
      CleanedIdents: ShortIdents
      Namespace: ShortIdents option
      NearestRequireQualifiedAccessParent: ShortIdents option
      TopRequireQualifiedAccessParent: ShortIdents option
      AutoOpenParent: ShortIdents option
      Symbol: FSharpSymbol
      Kind: LookupType -> EntityKind
      UnresolvedSymbol: UnresolvedSymbol }

    override x.ToString() = sprintf "%A" x  

type AssemblyPath = string
type AssemblyContentType = Public | Full

type Parent = 
    { Namespace: ShortIdents option
      ThisRequiresQualifiedAccess: (* isForMemberOrValue *) bool -> ShortIdents option
      TopRequiresQualifiedAccess: (* isForMemberOrValue *) bool -> ShortIdents option
      AutoOpen: ShortIdents option
      WithModuleSuffix: ShortIdents option 
      IsModule: bool }

    static member Empty = 
        { Namespace = None
          ThisRequiresQualifiedAccess = fun _ -> None
          TopRequiresQualifiedAccess = fun _ -> None
          AutoOpen = None
          WithModuleSuffix = None 
          IsModule = true }

    static member RewriteParentIdents (parentIdents: ShortIdents option) (idents: ShortIdents) =
        match parentIdents with
        | Some p when p.Length <= idents.Length -> 
            for i in 0..p.Length - 1 do
                idents.[i] <- p.[i]
        | _ -> ()
        idents
    
    member x.FixParentModuleSuffix (idents: ShortIdents) =
        Parent.RewriteParentIdents x.WithModuleSuffix idents

    member _.FormatEntityFullName (entity: FSharpEntity) =
        // remove number of arguments from generic types
        // e.g. System.Collections.Generic.Dictionary`2 -> System.Collections.Generic.Dictionary
        // and System.Data.Listeners`1.Func -> System.Data.Listeners.Func
        let removeGenericParamsCount (idents: ShortIdents) =
            idents 
            |> Array.map (fun ident ->
                if ident.Length > 0 && Char.IsDigit ident.[ident.Length - 1] then
                    let lastBacktickIndex = ident.LastIndexOf '`' 
                    if lastBacktickIndex <> -1 then
                        ident.Substring(0, lastBacktickIndex)
                    else ident
                else ident)

        let removeModuleSuffix (idents: ShortIdents) =
            if entity.IsFSharpModule && idents.Length > 0 then
                let lastIdent = idents.[idents.Length - 1]
                if lastIdent <> entity.DisplayName then
                    idents |> Array.replace (idents.Length - 1) entity.DisplayName
                else idents
            else idents

        entity.TryGetFullName()
        |> Option.bind (fun fullName -> 
            entity.TryGetFullDisplayName()
            |> Option.map (fun fullDisplayName ->
                fullName,
                fullDisplayName.Split '.'
                |> removeGenericParamsCount 
                |> removeModuleSuffix))

type AssemblyContentCacheEntry =
    { FileWriteTime: DateTime 
      ContentType: AssemblyContentType 
      Symbols: AssemblySymbol list }

[<NoComparison; NoEquality>]
type IAssemblyContentCache =
    abstract TryGet: AssemblyPath -> AssemblyContentCacheEntry option
    abstract Set: AssemblyPath -> AssemblyContentCacheEntry -> unit

module AssemblyContent =

    let UnresolvedSymbol (topRequireQualifiedAccessParent: ShortIdents option) (cleanedIdents: ShortIdents) (fullName: string) =
        let getNamespace (idents: ShortIdents) = 
            if idents.Length > 1 then Some idents.[..idents.Length - 2] else None

        let ns = 
            topRequireQualifiedAccessParent 
            |> Option.bind getNamespace 
            |> Option.orElseWith (fun () -> getNamespace cleanedIdents)
            |> Option.defaultValue [||]

        let displayName = 
            let nameIdents = if cleanedIdents.Length > ns.Length then cleanedIdents |> Array.skip ns.Length else cleanedIdents
            nameIdents |> String.concat "."
                
        { FullName = fullName
          DisplayName = displayName
          Namespace = ns }

    let createEntity ns (parent: Parent) (entity: FSharpEntity) =
        parent.FormatEntityFullName entity
        |> Option.map (fun (fullName, cleanIdents) ->
            let topRequireQualifiedAccessParent = parent.TopRequiresQualifiedAccess false |> Option.map parent.FixParentModuleSuffix
            { FullName = fullName
              CleanedIdents = cleanIdents
              Namespace = ns
              NearestRequireQualifiedAccessParent = parent.ThisRequiresQualifiedAccess false |> Option.map parent.FixParentModuleSuffix
              TopRequireQualifiedAccessParent = topRequireQualifiedAccessParent
              AutoOpenParent = parent.AutoOpen |> Option.map parent.FixParentModuleSuffix
              Symbol = entity
              Kind = fun lookupType ->
                match entity, lookupType with                
                | FSharpSymbolPatterns.FSharpModule, _ ->
                    EntityKind.Module 
                        { IsAutoOpen = entity.HasAttribute<AutoOpenAttribute>()
                          HasModuleSuffix = FSharpSymbolPatterns.hasModuleSuffixAttribute entity }
                | _, LookupType.Fuzzy ->
                    EntityKind.Type
                | _, LookupType.Precise ->
                    match entity with
                    | FSharpSymbolPatterns.Attribute -> EntityKind.Attribute 
                    | _ -> EntityKind.Type
              UnresolvedSymbol = UnresolvedSymbol topRequireQualifiedAccessParent cleanIdents fullName
            })

    let traverseMemberFunctionAndValues ns (parent: Parent) (membersFunctionsAndValues: seq<FSharpMemberOrFunctionOrValue>) =
        let topRequireQualifiedAccessParent = parent.TopRequiresQualifiedAccess false |> Option.map parent.FixParentModuleSuffix
        let autoOpenParent = parent.AutoOpen |> Option.map parent.FixParentModuleSuffix
        membersFunctionsAndValues
        |> Seq.filter (fun x -> not x.IsInstanceMember && not x.IsPropertyGetterMethod && not x.IsPropertySetterMethod)
        |> Seq.collect (fun func ->
            let processIdents fullName idents = 
                let cleanedIdents = parent.FixParentModuleSuffix idents
                { FullName = fullName
                  CleanedIdents = cleanedIdents
                  Namespace = ns
                  NearestRequireQualifiedAccessParent = parent.ThisRequiresQualifiedAccess true |> Option.map parent.FixParentModuleSuffix
                  TopRequireQualifiedAccessParent = topRequireQualifiedAccessParent
                  AutoOpenParent = autoOpenParent
                  Symbol = func
                  Kind = fun _ -> EntityKind.FunctionOrValue func.IsActivePattern
                  UnresolvedSymbol = UnresolvedSymbol topRequireQualifiedAccessParent cleanedIdents fullName }

            [ yield! func.TryGetFullDisplayName() 
                     |> Option.map (fun fullDisplayName -> processIdents func.FullName (fullDisplayName.Split '.'))
                     |> Option.toList
              (* for 
                 [<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
                 module M =
                     let (++) x y = ()
                 open M
                 let _ = 1 ++ 2
                
                 we should return additional RawEntity { FullName = MModule.op_PlusPlus; CleanedIdents = [|"M"; "op_PlusPlus"|] ... }
              *)
              yield! func.TryGetFullCompiledOperatorNameIdents() 
                     |> Option.map (fun fullCompiledIdents ->
                          processIdents (fullCompiledIdents |> String.concat ".") fullCompiledIdents)
                     |> Option.toList ])

    let rec traverseEntity contentType (parent: Parent) (entity: FSharpEntity) = 

        seq { 
#if !NO_TYPEPROVIDERS 
              if not entity.IsProvided then
#endif
                match contentType, entity.Accessibility.IsPublic with
                | Full, _ | Public, true ->
                    let ns = entity.Namespace |> Option.map (fun x -> x.Split '.') |> Option.orElse parent.Namespace
                    let currentEntity = createEntity ns parent entity

                    match currentEntity with
                    | Some x -> yield x
                    | None -> ()

                    let rqa = parent.FormatEntityFullName entity |> Option.map snd
                    let rqaForType = if entity.IsFSharp && entity.HasAttribute<RequireQualifiedAccessAttribute>() then rqa else None
                    let thisRequiresQualifierAccess (isForMethodOrValue: bool) = if isForMethodOrValue then rqa else rqaForType

                    let currentParent =
                        { ThisRequiresQualifiedAccess = thisRequiresQualifierAccess >> Option.orElse (parent.ThisRequiresQualifiedAccess false)
                          TopRequiresQualifiedAccess = fun forMV -> (parent.TopRequiresQualifiedAccess false) |> Option.orElse (thisRequiresQualifierAccess forMV)
                          
                          AutoOpen =
                            let isAutoOpen = entity.IsFSharpModule && entity.HasAttribute<AutoOpenAttribute>()
                            match isAutoOpen, parent.AutoOpen with
                            // if parent is also AutoOpen, then keep the parent
                            | true, Some parent -> Some parent 
                            // if parent is not AutoOpen, but current entity is, peek the latter as a new AutoOpen module
                            | true, None -> parent.FormatEntityFullName entity |> Option.map snd
                            // if current entity is not AutoOpen, we discard whatever parent was
                            | false, _ -> None 

                          WithModuleSuffix = 
                            if entity.IsFSharpModule && (FSharpSymbolPatterns.hasModuleSuffixAttribute entity || entity.CompiledName <> entity.DisplayName) then 
                                currentEntity |> Option.map (fun e -> e.CleanedIdents) 
                            else parent.WithModuleSuffix

                          Namespace = ns
                          IsModule = entity.IsFSharpModule }

                    match entity.TryGetMembersFunctionsAndValues() with
                    | xs when xs.Count > 0 ->
                        yield! traverseMemberFunctionAndValues ns currentParent xs
                    | _ -> ()

                    for e in (try entity.NestedEntities :> _ seq with _ -> Seq.empty) do
                        yield! traverseEntity contentType currentParent e 
                | _ -> () }


    let GetAssemblySignatureContent contentType (signature: FSharpAssemblySignature) =

        // We ignore all diagnostics during this operation
        //
        // CLEANUP: this function is run on the API user's calling thread.  It potentially accesses TAST data structures 
        // concurrently with other threads.  On an initial review this is not a problem since type provider computations
        // are not triggered (see "if not entity.IsProvided") and the other data accessed is immutable or computed safely 
        // on-demand.  However a more compete review may be warranted.

        use _ignoreAllDiagnostics = new ErrorScope()  

        signature.TryGetEntities()
        |> Seq.collect (traverseEntity contentType Parent.Empty)
        |> Seq.distinctBy (fun {FullName = fullName; CleanedIdents = cleanIdents} -> (fullName, cleanIdents))
        |> Seq.toList

    let getAssemblySignaturesContent contentType (assemblies: FSharpAssembly list) = 
        assemblies |> List.collect (fun asm -> GetAssemblySignatureContent contentType asm.Contents)

    let GetAssemblyContent (withCache: (IAssemblyContentCache -> _) -> _) contentType (fileName: string option) (assemblies: FSharpAssembly list) =

        // We ignore all diagnostics during this operation
        //
        // CLEANUP: this function is run on the API user's calling thread.  It potentially accesses TAST data structures 
        // concurrently with other threads.  On an initial review this is not a problem since type provider computations
        // are not triggered (see "if not entity.IsProvided") and the other data accessed is immutable or computed safely 
        // on-demand.  However a more compete review may be warranted.
        use _ignoreAllDiagnostics = new ErrorScope()  

#if !NO_TYPEPROVIDERS 
        match assemblies |> List.filter (fun x -> not x.IsProviderGenerated), fileName with
#else
        match assemblies, fileName with
#endif
        | [], _ -> []
        | assemblies, Some fileName ->
            let fileWriteTime = FileSystem.GetLastWriteTimeShim(fileName) 
            withCache <| fun cache ->
                match contentType, cache.TryGet fileName with 
                | _, Some entry
                | Public, Some entry when entry.FileWriteTime = fileWriteTime -> entry.Symbols
                | _ ->
                    let symbols = getAssemblySignaturesContent contentType assemblies
                    cache.Set fileName { FileWriteTime = fileWriteTime; ContentType = contentType; Symbols = symbols }
                    symbols
        | assemblies, None -> 
            getAssemblySignaturesContent contentType assemblies
        |> List.filter (fun entity -> 
            match contentType with
            | Full -> true
            | Public -> entity.Symbol.Accessibility.IsPublic)

type EntityCache() =
    let dic = Dictionary<AssemblyPath, AssemblyContentCacheEntry>()
    interface IAssemblyContentCache with
        member _.TryGet assembly =
            match dic.TryGetValue assembly with
            | true, entry -> Some entry
            | _ -> None
        member _.Set assembly entry = dic.[assembly] <- entry

    member _.Clear() = dic.Clear()
    member x.Locking f = lock dic <| fun _ -> f (x :> IAssemblyContentCache)

