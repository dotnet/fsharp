// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace FSharp.Compiler.SourceCodeServices

open System
open System.Collections.Generic

open FSharp.Compiler
open FSharp.Compiler.Ast
open FSharp.Compiler.Range
open FSharp.Compiler.AbstractIL.Internal.Library 

type ShortIdent = string
type Idents = ShortIdent[]
type MaybeUnresolvedIdent = { Ident: ShortIdent; Resolved: bool }
type MaybeUnresolvedIdents = MaybeUnresolvedIdent[]
type IsAutoOpen = bool

[<AutoOpen>]
module Extensions =

    type FSharpEntity with
        member x.TryGetFullName() =
            try x.TryFullName 
            with _ -> 
                try Some(String.Join(".", x.AccessPath, x.DisplayName))
                with _ -> None

        member x.TryGetFullDisplayName() =
            let fullName = x.TryGetFullName() |> Option.map (fun fullName -> fullName.Split '.')
            let res = 
                match fullName with
                | Some fullName ->
                    match Option.attempt (fun _ -> x.DisplayName) with
                    | Some shortDisplayName when not (shortDisplayName.Contains ".") ->
                        Some (fullName |> Array.replace (fullName.Length - 1) shortDisplayName)
                    | _ -> Some fullName
                | None -> None 
                |> Option.map (fun fullDisplayName -> String.Join (".", fullDisplayName))
            //debug "GetFullDisplayName: FullName = %A, Result = %A" fullName res
            res

        member x.TryGetFullCompiledName() =
            let fullName = x.TryGetFullName() |> Option.map (fun fullName -> fullName.Split '.')
            let res = 
                match fullName with
                | Some fullName ->
                    match Option.attempt (fun _ -> x.CompiledName) with
                    | Some shortCompiledName when not (shortCompiledName.Contains ".") ->
                        Some (fullName |> Array.replace (fullName.Length - 1) shortCompiledName)
                    | _ -> Some fullName
                | None -> None 
                |> Option.map (fun fullDisplayName -> String.Join (".", fullDisplayName))
            //debug "GetFullCompiledName: FullName = %A, Result = %A" fullName res
            res

        member x.PublicNestedEntities =
            x.NestedEntities |> Seq.filter (fun entity -> entity.Accessibility.IsPublic)

        member x.TryGetMembersFunctionsAndValues = 
            try x.MembersFunctionsAndValues with _ -> [||] :> _

    let isOperator (name: string) =
        name.StartsWithOrdinal("( ") && name.EndsWithOrdinal(" )") && name.Length > 4
            && name.Substring (2, name.Length - 4) 
               |> String.forall (fun c -> c <> ' ' && not (Char.IsLetter c))

    type FSharpMemberOrFunctionOrValue with
        // FullType may raise exceptions (see https://github.com/fsharp/fsharp/issues/307). 
        member x.FullTypeSafe = Option.attempt (fun _ -> x.FullType)

        member x.TryGetFullDisplayName() =
            let fullName = Option.attempt (fun _ -> x.FullName.Split '.')
            match fullName with
            | Some fullName ->
                match Option.attempt (fun _ -> x.DisplayName) with
                | Some shortDisplayName when not (shortDisplayName.Contains ".") ->
                    Some (fullName |> Array.replace (fullName.Length - 1) shortDisplayName)
                | _ -> Some fullName
            | None -> None
            |> Option.map (fun fullDisplayName -> String.Join (".", fullDisplayName))

        member x.TryGetFullCompiledOperatorNameIdents() : Idents option =
            // For operator ++ displayName is ( ++ ) compiledName is op_PlusPlus
            if isOperator x.DisplayName && x.DisplayName <> x.CompiledName then
                x.DeclaringEntity
                |> Option.bind (fun e -> e.TryGetFullName())
                |> Option.map (fun enclosingEntityFullName -> 
                     Array.append (enclosingEntityFullName.Split '.') [| x.CompiledName |])
            else None

    type FSharpAssemblySignature with
        member x.TryGetEntities() = try x.Entities :> _ seq with _ -> Seq.empty

[<RequireQualifiedAccess>]
type LookupType =
    | Fuzzy
    | Precise

[<NoComparison; NoEquality>]
type AssemblySymbol = 
    { FullName: string
      CleanedIdents: Idents
      Namespace: Idents option
      NearestRequireQualifiedAccessParent: Idents option
      TopRequireQualifiedAccessParent: Idents option
      AutoOpenParent: Idents option
      Symbol: FSharpSymbol
      Kind: LookupType -> EntityKind
      UnresolvedSymbol: UnresolvedSymbol }
    override x.ToString() = sprintf "%A" x  

type AssemblyPath = string
type AssemblyContentType = Public | Full

type Parent = 
    { Namespace: Idents option
      ThisRequiresQualifiedAccess: (* isForMemberOrValue *) bool -> Idents option
      TopRequiresQualifiedAccess: (* isForMemberOrValue *) bool -> Idents option
      AutoOpen: Idents option
      WithModuleSuffix: Idents option 
      IsModule: bool }
    static member Empty = 
        { Namespace = None
          ThisRequiresQualifiedAccess = fun _ -> None
          TopRequiresQualifiedAccess = fun _ -> None
          AutoOpen = None
          WithModuleSuffix = None 
          IsModule = true }
    static member RewriteParentIdents (parentIdents: Idents option) (idents: Idents) =
        match parentIdents with
        | Some p when p.Length <= idents.Length -> 
            for i in 0..p.Length - 1 do
                idents.[i] <- p.[i]
        | _ -> ()
        idents
    
    member x.FixParentModuleSuffix (idents: Idents) =
        Parent.RewriteParentIdents x.WithModuleSuffix idents

    member __.FormatEntityFullName (entity: FSharpEntity) =
        // remove number of arguments from generic types
        // e.g. System.Collections.Generic.Dictionary`2 -> System.Collections.Generic.Dictionary
        // and System.Data.Listeners`1.Func -> System.Data.Listeners.Func
        let removeGenericParamsCount (idents: Idents) =
            idents 
            |> Array.map (fun ident ->
                if ident.Length > 0 && Char.IsDigit ident.[ident.Length - 1] then
                    let lastBacktickIndex = ident.LastIndexOf '`' 
                    if lastBacktickIndex <> -1 then
                        ident.Substring(0, lastBacktickIndex)
                    else ident
                else ident)

        let removeModuleSuffix (idents: Idents) =
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

module AssemblyContentProvider =
    open System.IO

    let unresolvedSymbol (topRequireQualifiedAccessParent: Idents option) (cleanedIdents: Idents) (fullName: string) =
        let getNamespace (idents: Idents) = 
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
                | Symbol.FSharpModule, _ ->
                    EntityKind.Module 
                        { IsAutoOpen = Symbol.hasAttribute<AutoOpenAttribute> entity.Attributes
                          HasModuleSuffix = Symbol.hasModuleSuffixAttribute entity }
                | _, LookupType.Fuzzy ->
                    EntityKind.Type
                | _, LookupType.Precise ->
                    match entity with
                    | Symbol.Attribute -> EntityKind.Attribute 
                    | _ -> EntityKind.Type
              UnresolvedSymbol = unresolvedSymbol topRequireQualifiedAccessParent cleanIdents fullName
            })

    let traverseMemberFunctionAndValues ns (parent: Parent) (membersFunctionsAndValues: seq<FSharpMemberOrFunctionOrValue>) =
        let topRequireQualifiedAccessParent = parent.TopRequiresQualifiedAccess true |> Option.map parent.FixParentModuleSuffix
        let autoOpenParent = parent.AutoOpen |> Option.map parent.FixParentModuleSuffix
        membersFunctionsAndValues
        |> Seq.filter (fun x -> not x.IsInstanceMember && not x.IsPropertyGetterMethod && not x.IsPropertySetterMethod)
        |> Seq.collect (fun func ->
            let processIdents fullName idents = 
                let cleanedIdentes = parent.FixParentModuleSuffix idents
                { FullName = fullName
                  CleanedIdents = cleanedIdentes
                  Namespace = ns
                  NearestRequireQualifiedAccessParent = parent.ThisRequiresQualifiedAccess true |> Option.map parent.FixParentModuleSuffix
                  TopRequireQualifiedAccessParent = topRequireQualifiedAccessParent
                  AutoOpenParent = autoOpenParent
                  Symbol = func
                  Kind = fun _ -> EntityKind.FunctionOrValue func.IsActivePattern
                  UnresolvedSymbol = unresolvedSymbol topRequireQualifiedAccessParent cleanedIdentes fullName }

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
#if !NO_EXTENSIONTYPING 
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
                    let rqaForType = if entity.IsFSharp && Symbol.hasAttribute<RequireQualifiedAccessAttribute> entity.Attributes then rqa else None
                    let thisRequiresQualifierAccess (isForMethodOrValue: bool) = if isForMethodOrValue then rqa else rqaForType

                    let currentParent =
                        { ThisRequiresQualifiedAccess = thisRequiresQualifierAccess >> Option.orElse (parent.ThisRequiresQualifiedAccess false)
                          TopRequiresQualifiedAccess = fun forMV -> (parent.TopRequiresQualifiedAccess false) |> Option.orElse (thisRequiresQualifierAccess forMV)
                          
                          AutoOpen =
                            let isAutoOpen = entity.IsFSharpModule && Symbol.hasAttribute<AutoOpenAttribute> entity.Attributes
                            match isAutoOpen, parent.AutoOpen with
                            // if parent is also AutoOpen, then keep the parent
                            | true, Some parent -> Some parent 
                            // if parent is not AutoOpen, but current entity is, peek the latter as a new AutoOpen module
                            | true, None -> parent.FormatEntityFullName entity |> Option.map snd
                            // if current entity is not AutoOpen, we discard whatever parent was
                            | false, _ -> None 

                          WithModuleSuffix = 
                            if entity.IsFSharpModule && (Symbol.hasModuleSuffixAttribute entity || entity.CompiledName <> entity.DisplayName) then 
                                currentEntity |> Option.map (fun e -> e.CleanedIdents) 
                            else parent.WithModuleSuffix

                          Namespace = ns
                          IsModule = entity.IsFSharpModule }

                    match entity.TryGetMembersFunctionsAndValues with
                    | xs when xs.Count > 0 ->
                        yield! traverseMemberFunctionAndValues ns currentParent xs
                    | _ -> ()

                    for e in (try entity.NestedEntities :> _ seq with _ -> Seq.empty) do
                        yield! traverseEntity contentType currentParent e 
                | _ -> () }


    let getAssemblySignatureContent contentType (signature: FSharpAssemblySignature) =

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
        assemblies |> List.collect (fun asm -> getAssemblySignatureContent contentType asm.Contents)

    let getAssemblyContent (withCache: (IAssemblyContentCache -> _) -> _) contentType (fileName: string option) (assemblies: FSharpAssembly list) =

        // We ignore all diagnostics during this operation
        //
        // CLEANUP: this function is run on the API user's calling thread.  It potentially accesses TAST data structures 
        // concurrently with other threads.  On an initial review this is not a problem since type provider computations
        // are not triggered (see "if not entity.IsProvided") and the other data accessed is immutable or computed safely 
        // on-demand.  However a more compete review may be warranted.
        use _ignoreAllDiagnostics = new ErrorScope()  

#if !NO_EXTENSIONTYPING 
        match assemblies |> List.filter (fun x -> not x.IsProviderGenerated), fileName with
#else
        match assemblies, fileName with
#endif
        | [], _ -> []
        | assemblies, Some fileName ->
            let fileWriteTime = FileInfo(fileName).LastWriteTime 
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
            match contentType, FSharpSymbol.GetAccessibility(entity.Symbol) with
            | Full, _ -> true
            | Public, access ->
                match access with
                | None -> true
                | Some x when x.IsPublic -> true
                | _ -> false)

type EntityCache() =
    let dic = Dictionary<AssemblyPath, AssemblyContentCacheEntry>()
    interface IAssemblyContentCache with
        member __.TryGet assembly =
            match dic.TryGetValue assembly with
            | true, entry -> Some entry
            | _ -> None
        member __.Set assembly entry = dic.[assembly] <- entry

    member __.Clear() = dic.Clear()
    member x.Locking f = lock dic <| fun _ -> f (x :> IAssemblyContentCache)

type StringLongIdent = string

type Entity =
    { FullRelativeName: StringLongIdent
      Qualifier: StringLongIdent
      Namespace: StringLongIdent option
      Name: StringLongIdent
      LastIdent: string }
    override x.ToString() = sprintf "%A" x

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Entity =
    let getRelativeNamespace (targetNs: Idents) (sourceNs: Idents) =
        let rec loop index =
            if index > targetNs.Length - 1 then sourceNs.[index..]
            // target namespace is not a full parent of source namespace, keep the source ns as is
            elif index > sourceNs.Length - 1 then sourceNs
            elif targetNs.[index] = sourceNs.[index] then loop (index + 1)
            else sourceNs.[index..]
        if sourceNs.Length = 0 || targetNs.Length = 0 then sourceNs
        else loop 0

    let cutAutoOpenModules (autoOpenParent: Idents option) (candidateNs: Idents) =
        let nsCount = 
            match autoOpenParent with
            | Some parent when parent.Length > 0 -> 
                min (parent.Length - 1) candidateNs.Length
            | _ -> candidateNs.Length
        candidateNs.[0..nsCount - 1]

    let tryCreate (targetNamespace: Idents option, targetScope: Idents, partiallyQualifiedName: MaybeUnresolvedIdents, 
                   requiresQualifiedAccessParent: Idents option, autoOpenParent: Idents option, candidateNamespace: Idents option, candidate: Idents) =
        match candidate with
        | [||] -> [||]
        | _ ->
            partiallyQualifiedName
            |> Array.heads
            // long ident must contain an unresolved part, otherwise we show false positive suggestions like
            // "open System" for `let _ = System.DateTime.Naaaw`. Here only "Naaw" is unresolved.
            |> Array.filter (fun x -> x |> Array.exists (fun x -> not x.Resolved))
            |> Array.choose (fun parts ->
                let parts = parts |> Array.map (fun x -> x.Ident)
                if not (candidate |> Array.endsWith parts) then None
                else 
                  let identCount = parts.Length
                  let fullOpenableNs, restIdents = 
                      let openableNsCount =
                          match requiresQualifiedAccessParent with
                          | Some parent -> min parent.Length candidate.Length
                          | None -> candidate.Length
                      candidate.[0..openableNsCount - 2], candidate.[openableNsCount - 1..]
              
                  let openableNs = cutAutoOpenModules autoOpenParent fullOpenableNs
                   
                  let getRelativeNs ns =
                      match targetNamespace, candidateNamespace with
                      | Some targetNs, Some candidateNs when candidateNs = targetNs ->
                          getRelativeNamespace targetScope ns
                      | None, _ -> getRelativeNamespace targetScope ns
                      | _ -> ns
              
                  let relativeNs = getRelativeNs openableNs
              
                  match relativeNs, restIdents with
                  | [||], [||] -> None
                  | [||], [|_|] -> None
                  | _ ->
                      let fullRelativeName = Array.append (getRelativeNs fullOpenableNs) restIdents
                      let ns = 
                          match relativeNs with 
                          | [||] -> None 
                          | _ when identCount > 1 && relativeNs.Length >= identCount -> 
                              Some (relativeNs.[0..relativeNs.Length - identCount] |> String.concat ".")
                          | _ -> Some (relativeNs |> String.concat ".")
                      let qualifier = 
                          if fullRelativeName.Length > 1 && fullRelativeName.Length >= identCount then
                              fullRelativeName.[0..fullRelativeName.Length - identCount]  
                          else fullRelativeName
                      Some 
                          { FullRelativeName = String.concat "." fullRelativeName //.[0..fullRelativeName.Length - identCount - 1]
                            Qualifier = String.concat "." qualifier
                            Namespace = ns
                            Name = match restIdents with [|_|] -> "" | _ -> String.concat "." restIdents 
                            LastIdent = Array.tryLast restIdents |> Option.defaultValue "" }) 

type ScopeKind =
    | Namespace
    | TopModule
    | NestedModule
    | OpenDeclaration
    | HashDirective
    override x.ToString() = sprintf "%A" x

type InsertContext =
    { ScopeKind: ScopeKind
      Pos: pos }

type Module =
    { Idents: Idents
      Range: range }

type OpenStatementInsertionPoint =
    | TopLevel
    | Nearest

module ParsedInput =
    open FSharp.Compiler
    open FSharp.Compiler.Ast

    /// An recursive pattern that collect all sequential expressions to avoid StackOverflowException
    let rec (|Sequentials|_|) = function
        | SynExpr.Sequential (_, _, e, Sequentials es, _) ->
            Some(e :: es)
        | SynExpr.Sequential (_, _, e1, e2, _) ->
            Some [e1; e2]
        | _ -> None

    let (|ConstructorPats|) = function
        | SynConstructorArgs.Pats ps -> ps
        | SynConstructorArgs.NamePatPairs(xs, _) -> List.map snd xs

    /// Returns all `Ident`s and `LongIdent`s found in an untyped AST.
    let getLongIdents (input: ParsedInput option) : IDictionary<Range.pos, LongIdent> =
        let identsByEndPos = Dictionary<Range.pos, LongIdent>()
    
        let addLongIdent (longIdent: LongIdent) =
            for ident in longIdent do
                identsByEndPos.[ident.idRange.End] <- longIdent
    
        let addLongIdentWithDots (LongIdentWithDots (longIdent, lids) as value) =
            match longIdent with
            | [] -> ()
            | [_] as idents -> identsByEndPos.[value.Range.End] <- idents
            | idents ->
                for dotRange in lids do
                    identsByEndPos.[Range.mkPos dotRange.EndLine (dotRange.EndColumn - 1)] <- idents
                identsByEndPos.[value.Range.End] <- idents
    
        let addIdent (ident: Ident) =
            identsByEndPos.[ident.idRange.End] <- [ident]
    
        let rec walkImplFileInput (ParsedImplFileInput (modules = moduleOrNamespaceList)) =
            List.iter walkSynModuleOrNamespace moduleOrNamespaceList
    
        and walkSynModuleOrNamespace (SynModuleOrNamespace(_, _, _, decls, _, attrs, _, _)) =
            List.iter walkAttribute attrs
            List.iter walkSynModuleDecl decls
    
        and walkAttribute (attr: SynAttribute) =
            addLongIdentWithDots attr.TypeName
            walkExpr attr.ArgExpr
    
        and walkTyparDecl (SynTyparDecl.TyparDecl (attrs, typar)) =
            List.iter walkAttribute attrs
            walkTypar typar
    
        and walkTypeConstraint = function
            | SynTypeConstraint.WhereTyparIsValueType (t, _)
            | SynTypeConstraint.WhereTyparIsReferenceType (t, _)
            | SynTypeConstraint.WhereTyparIsUnmanaged (t, _)
            | SynTypeConstraint.WhereTyparSupportsNull (t, _)
            | SynTypeConstraint.WhereTyparIsComparable (t, _)
            | SynTypeConstraint.WhereTyparIsEquatable (t, _) -> walkTypar t
            | SynTypeConstraint.WhereTyparDefaultsToType (t, ty, _)
            | SynTypeConstraint.WhereTyparSubtypeOfType (t, ty, _) -> walkTypar t; walkType ty
            | SynTypeConstraint.WhereTyparIsEnum (t, ts, _)
            | SynTypeConstraint.WhereTyparIsDelegate (t, ts, _) -> walkTypar t; List.iter walkType ts
            | SynTypeConstraint.WhereTyparSupportsMember (ts, sign, _) -> List.iter walkType ts; walkMemberSig sign
    
        and walkPat = function
            | SynPat.Tuple (_,pats, _)
            | SynPat.ArrayOrList (_, pats, _)
            | SynPat.Ands (pats, _) -> List.iter walkPat pats
            | SynPat.Named (pat, ident, _, _, _) ->
                walkPat pat
                addIdent ident
            | SynPat.Typed (pat, t, _) ->
                walkPat pat
                walkType t
            | SynPat.Attrib (pat, attrs, _) ->
                walkPat pat
                List.iter walkAttribute attrs
            | SynPat.Or (pat1, pat2, _) -> List.iter walkPat [pat1; pat2]
            | SynPat.LongIdent (ident, _, typars, ConstructorPats pats, _, _) ->
                addLongIdentWithDots ident
                typars
                |> Option.iter (fun (SynValTyparDecls (typars, _, constraints)) ->
                     List.iter walkTyparDecl typars
                     List.iter walkTypeConstraint constraints)
                List.iter walkPat pats
            | SynPat.Paren (pat, _) -> walkPat pat
            | SynPat.IsInst (t, _) -> walkType t
            | SynPat.QuoteExpr(e, _) -> walkExpr e
            | _ -> ()
    
        and walkTypar (Typar (_, _, _)) = ()
    
        and walkBinding (SynBinding.Binding (_, _, _, _, attrs, _, _, pat, returnInfo, e, _, _)) =
            List.iter walkAttribute attrs
            walkPat pat
            walkExpr e
            returnInfo |> Option.iter (fun (SynBindingReturnInfo (t, _, _)) -> walkType t)
    
        and walkInterfaceImpl (InterfaceImpl(_, bindings, _)) = List.iter walkBinding bindings
    
        and walkIndexerArg = function
            | SynIndexerArg.One e -> walkExpr e
            | SynIndexerArg.Two (e1, e2) -> List.iter walkExpr [e1; e2]
    
        and walkType = function
            | SynType.Array (_, t, _)
            | SynType.HashConstraint (t, _)
            | SynType.MeasurePower (t, _, _) -> walkType t
            | SynType.Fun (t1, t2, _)
            | SynType.MeasureDivide (t1, t2, _) -> walkType t1; walkType t2
            | SynType.LongIdent ident -> addLongIdentWithDots ident
            | SynType.App (ty, _, types, _, _, _, _) -> walkType ty; List.iter walkType types
            | SynType.LongIdentApp (_, _, _, types, _, _, _) -> List.iter walkType types
            | SynType.Tuple (_, ts, _) -> ts |> List.iter (fun (_, t) -> walkType t)
            | SynType.WithGlobalConstraints (t, typeConstraints, _) ->
                walkType t; List.iter walkTypeConstraint typeConstraints
            | _ -> ()
    
        and walkClause (Clause (pat, e1, e2, _, _)) =
            walkPat pat
            walkExpr e2
            e1 |> Option.iter walkExpr
    
        and walkSimplePats = function
            | SynSimplePats.SimplePats (pats, _) -> List.iter walkSimplePat pats
            | SynSimplePats.Typed (pats, ty, _) -> 
                walkSimplePats pats
                walkType ty
    
        and walkExpr = function
            | SynExpr.Paren (e, _, _, _)
            | SynExpr.Quote (_, _, e, _, _)
            | SynExpr.Typed (e, _, _)
            | SynExpr.InferredUpcast (e, _)
            | SynExpr.InferredDowncast (e, _)
            | SynExpr.AddressOf (_, e, _, _)
            | SynExpr.DoBang (e, _)
            | SynExpr.YieldOrReturn (_, e, _)
            | SynExpr.ArrayOrListOfSeqExpr (_, e, _)
            | SynExpr.CompExpr (_, _, e, _)
            | SynExpr.Do (e, _)
            | SynExpr.Assert (e, _)
            | SynExpr.Lazy (e, _)
            | SynExpr.YieldOrReturnFrom (_, e, _) -> walkExpr e
            | SynExpr.Lambda (_, _, pats, e, _) ->
                walkSimplePats pats
                walkExpr e
            | SynExpr.New (_, t, e, _)
            | SynExpr.TypeTest (e, t, _)
            | SynExpr.Upcast (e, t, _)
            | SynExpr.Downcast (e, t, _) -> walkExpr e; walkType t
            | SynExpr.Tuple (_, es, _, _)
            | Sequentials es
            | SynExpr.ArrayOrList (_, es, _) -> List.iter walkExpr es
            | SynExpr.App (_, _, e1, e2, _)
            | SynExpr.TryFinally (e1, e2, _, _, _)
            | SynExpr.While (_, e1, e2, _) -> List.iter walkExpr [e1; e2]
            | SynExpr.Record (_, _, fields, _) ->
                fields |> List.iter (fun ((ident, _), e, _) ->
                            addLongIdentWithDots ident
                            e |> Option.iter walkExpr)
            | SynExpr.Ident ident -> addIdent ident
            | SynExpr.ObjExpr (ty, argOpt, bindings, ifaces, _, _) ->
                argOpt |> Option.iter (fun (e, ident) ->
                    walkExpr e
                    ident |> Option.iter addIdent)
                walkType ty
                List.iter walkBinding bindings
                List.iter walkInterfaceImpl ifaces
            | SynExpr.LongIdent (_, ident, _, _) -> addLongIdentWithDots ident
            | SynExpr.For (_, ident, e1, _, e2, e3, _) ->
                addIdent ident
                List.iter walkExpr [e1; e2; e3]
            | SynExpr.ForEach (_, _, _, pat, e1, e2, _) ->
                walkPat pat
                List.iter walkExpr [e1; e2]
            | SynExpr.MatchLambda (_, _, synMatchClauseList, _, _) ->
                List.iter walkClause synMatchClauseList
            | SynExpr.Match (_, e, synMatchClauseList, _) ->
                walkExpr e
                List.iter walkClause synMatchClauseList
            | SynExpr.TypeApp (e, _, tys, _, _, _, _) ->
                List.iter walkType tys; walkExpr e
            | SynExpr.LetOrUse (_, _, bindings, e, _) ->
                List.iter walkBinding bindings; walkExpr e
            | SynExpr.TryWith (e, _, clauses, _, _, _, _) ->
                List.iter walkClause clauses;  walkExpr e
            | SynExpr.IfThenElse (e1, e2, e3, _, _, _, _) ->
                List.iter walkExpr [e1; e2]
                e3 |> Option.iter walkExpr
            | SynExpr.LongIdentSet (ident, e, _)
            | SynExpr.DotGet (e, _, ident, _) ->
                addLongIdentWithDots ident
                walkExpr e
            | SynExpr.DotSet (e1, idents, e2, _) ->
                walkExpr e1
                addLongIdentWithDots idents
                walkExpr e2
            | SynExpr.Set (e1, e2, _) ->
                walkExpr e1
                walkExpr e2
            | SynExpr.DotIndexedGet (e, args, _, _) ->
                walkExpr e
                List.iter walkIndexerArg args
            | SynExpr.DotIndexedSet (e1, args, e2, _, _, _) ->
                walkExpr e1
                List.iter walkIndexerArg args
                walkExpr e2
            | SynExpr.NamedIndexedPropertySet (ident, e1, e2, _) ->
                addLongIdentWithDots ident
                List.iter walkExpr [e1; e2]
            | SynExpr.DotNamedIndexedPropertySet (e1, ident, e2, e3, _) ->
                addLongIdentWithDots ident
                List.iter walkExpr [e1; e2; e3]
            | SynExpr.JoinIn (e1, _, e2, _) -> List.iter walkExpr [e1; e2]
            | SynExpr.LetOrUseBang (_, _, _, pat, e1, e2, _) ->
                walkPat pat
                List.iter walkExpr [e1; e2]
            | SynExpr.TraitCall (ts, sign, e, _) ->
                List.iter walkTypar ts
                walkMemberSig sign
                walkExpr e
            | SynExpr.Const (SynConst.Measure(_, m), _) -> walkMeasure m
            | _ -> ()
    
        and walkMeasure = function
            | SynMeasure.Product (m1, m2, _)
            | SynMeasure.Divide (m1, m2, _) -> walkMeasure m1; walkMeasure m2
            | SynMeasure.Named (longIdent, _) -> addLongIdent longIdent
            | SynMeasure.Seq (ms, _) -> List.iter walkMeasure ms
            | SynMeasure.Power (m, _, _) -> walkMeasure m
            | SynMeasure.Var (ty, _) -> walkTypar ty
            | SynMeasure.One
            | SynMeasure.Anon _ -> ()
    
        and walkSimplePat = function
            | SynSimplePat.Attrib (pat, attrs, _) ->
                walkSimplePat pat
                List.iter walkAttribute attrs
            | SynSimplePat.Typed(pat, t, _) ->
                walkSimplePat pat
                walkType t
            | _ -> ()
    
        and walkField (SynField.Field(attrs, _, _, t, _, _, _, _)) =
            List.iter walkAttribute attrs
            walkType t
    
        and walkValSig (SynValSig.ValSpfn(attrs, _, _, t, SynValInfo(argInfos, argInfo), _, _, _, _, _, _)) =
            List.iter walkAttribute attrs
            walkType t
            argInfo :: (argInfos |> List.concat)
            |> List.map (fun (SynArgInfo(attrs, _, _)) -> attrs)
            |> List.concat
            |> List.iter walkAttribute
    
        and walkMemberSig = function
            | SynMemberSig.Inherit (t, _)
            | SynMemberSig.Interface(t, _) -> walkType t
            | SynMemberSig.Member(vs, _, _) -> walkValSig vs
            | SynMemberSig.ValField(f, _) -> walkField f
            | SynMemberSig.NestedType(SynTypeDefnSig.TypeDefnSig (info, repr, memberSigs, _), _) ->
                let isTypeExtensionOrAlias =
                    match repr with
                    | SynTypeDefnSigRepr.Simple(SynTypeDefnSimpleRepr.TypeAbbrev _, _)
                    | SynTypeDefnSigRepr.ObjectModel(SynTypeDefnKind.TyconAbbrev, _, _)
                    | SynTypeDefnSigRepr.ObjectModel(SynTypeDefnKind.TyconAugmentation, _, _) -> true
                    | _ -> false
                walkComponentInfo isTypeExtensionOrAlias info
                walkTypeDefnSigRepr repr
                List.iter walkMemberSig memberSigs
    
        and walkMember = function
            | SynMemberDefn.AbstractSlot (valSig, _, _) -> walkValSig valSig
            | SynMemberDefn.Member (binding, _) -> walkBinding binding
            | SynMemberDefn.ImplicitCtor (_, attrs, pats, _, _) ->
                List.iter walkAttribute attrs
                List.iter walkSimplePat pats
            | SynMemberDefn.ImplicitInherit (t, e, _, _) -> walkType t; walkExpr e
            | SynMemberDefn.LetBindings (bindings, _, _, _) -> List.iter walkBinding bindings
            | SynMemberDefn.Interface (t, members, _) ->
                walkType t
                members |> Option.iter (List.iter walkMember)
            | SynMemberDefn.Inherit (t, _, _) -> walkType t
            | SynMemberDefn.ValField (field, _) -> walkField field
            | SynMemberDefn.NestedType (tdef, _, _) -> walkTypeDefn tdef
            | SynMemberDefn.AutoProperty (attrs, _, _, t, _, _, _, _, e, _, _) ->
                List.iter walkAttribute attrs
                Option.iter walkType t
                walkExpr e
            | _ -> ()
    
        and walkEnumCase (EnumCase(attrs, _, _, _, _)) = List.iter walkAttribute attrs
    
        and walkUnionCaseType = function
            | SynUnionCaseType.UnionCaseFields fields -> List.iter walkField fields
            | SynUnionCaseType.UnionCaseFullType (t, _) -> walkType t
    
        and walkUnionCase (SynUnionCase.UnionCase (attrs, _, t, _, _, _)) =
            List.iter walkAttribute attrs
            walkUnionCaseType t
    
        and walkTypeDefnSimple = function
            | SynTypeDefnSimpleRepr.Enum (cases, _) -> List.iter walkEnumCase cases
            | SynTypeDefnSimpleRepr.Union (_, cases, _) -> List.iter walkUnionCase cases
            | SynTypeDefnSimpleRepr.Record (_, fields, _) -> List.iter walkField fields
            | SynTypeDefnSimpleRepr.TypeAbbrev (_, t, _) -> walkType t
            | _ -> ()
    
        and walkComponentInfo isTypeExtensionOrAlias (ComponentInfo(attrs, typars, constraints, longIdent, _, _, _, _)) =
            List.iter walkAttribute attrs
            List.iter walkTyparDecl typars
            List.iter walkTypeConstraint constraints
            if isTypeExtensionOrAlias then
                addLongIdent longIdent
    
        and walkTypeDefnRepr = function
            | SynTypeDefnRepr.ObjectModel (_, defns, _) -> List.iter walkMember defns
            | SynTypeDefnRepr.Simple(defn, _) -> walkTypeDefnSimple defn
            | SynTypeDefnRepr.Exception _ -> ()
    
        and walkTypeDefnSigRepr = function
            | SynTypeDefnSigRepr.ObjectModel (_, defns, _) -> List.iter walkMemberSig defns
            | SynTypeDefnSigRepr.Simple(defn, _) -> walkTypeDefnSimple defn
            | SynTypeDefnSigRepr.Exception _ -> ()
    
        and walkTypeDefn (TypeDefn (info, repr, members, _)) =
            let isTypeExtensionOrAlias =
                match repr with
                | SynTypeDefnRepr.ObjectModel (SynTypeDefnKind.TyconAugmentation, _, _)
                | SynTypeDefnRepr.ObjectModel (SynTypeDefnKind.TyconAbbrev, _, _)
                | SynTypeDefnRepr.Simple (SynTypeDefnSimpleRepr.TypeAbbrev _, _) -> true
                | _ -> false
            walkComponentInfo isTypeExtensionOrAlias info
            walkTypeDefnRepr repr
            List.iter walkMember members
    
        and walkSynModuleDecl (decl: SynModuleDecl) =
            match decl with
            | SynModuleDecl.NamespaceFragment fragment -> walkSynModuleOrNamespace fragment
            | SynModuleDecl.NestedModule (info, _, modules, _, _) ->
                walkComponentInfo false info
                List.iter walkSynModuleDecl modules
            | SynModuleDecl.Let (_, bindings, _) -> List.iter walkBinding bindings
            | SynModuleDecl.DoExpr (_, expr, _) -> walkExpr expr
            | SynModuleDecl.Types (types, _) -> List.iter walkTypeDefn types
            | SynModuleDecl.Attributes (attrs, _) -> List.iter walkAttribute attrs
            | _ -> ()
    
        match input with
        | Some (ParsedInput.ImplFile input) ->
             walkImplFileInput input
        | _ -> ()
        //debug "%A" idents
        upcast identsByEndPos
    
    let getLongIdentAt ast pos =
        let idents = getLongIdents (Some ast)
        match idents.TryGetValue pos with
        | true, idents -> Some idents
        | _ -> None

    type Scope =
        { Idents: Idents
          Kind: ScopeKind }

    let tryFindNearestPointAndModules (currentLine: int) (ast: ParsedInput) (insertionPoint: OpenStatementInsertionPoint) = 
        // We ignore all diagnostics during this operation
        //
        // Based on an initial review, no diagnostics should be generated.  However the code should be checked more closely.
        use _ignoreAllDiagnostics = new ErrorScope()  

        let result: (Scope * pos * (* finished *) bool) option ref = ref None
        let ns: string[] option ref = ref None
        let modules = ResizeArray<Module>()  

        let inline longIdentToIdents ident = ident |> Seq.map (fun x -> string x) |> Seq.toArray
        
        let addModule (longIdent: LongIdent, range: range) =
            modules.Add 
                { Idents = longIdent |> List.map string |> List.toArray 
                  Range = range }

        let doRange kind (scope: LongIdent) line col =
            if line <= currentLine then
                match !result, insertionPoint with
                | None, _ -> 
                    result := Some ({ Idents = longIdentToIdents scope; Kind = kind }, mkPos line col, false)
                | Some (_, _, true), _ -> ()
                | Some (oldScope, oldPos, false), OpenStatementInsertionPoint.TopLevel when kind <> OpenDeclaration ->
                    result := Some (oldScope, oldPos, true)
                | Some (oldScope, oldPos, _), _ ->
                    match kind, oldScope.Kind with
                    | (Namespace | NestedModule | TopModule), OpenDeclaration
                    | _ when oldPos.Line <= line ->
                        result := 
                            Some ({ Idents = 
                                        match scope with 
                                        | [] -> oldScope.Idents 
                                        | _ -> longIdentToIdents scope
                                    Kind = kind },
                                  mkPos line col,
                                  false)
                    | _ -> ()

        let getMinColumn (decls: SynModuleDecls) =
            match decls with
            | [] -> None
            | firstDecl :: _ -> 
                match firstDecl with
                | SynModuleDecl.NestedModule (_, _, _, _, r)
                | SynModuleDecl.Let (_, _, r)
                | SynModuleDecl.DoExpr (_, _, r)
                | SynModuleDecl.Types (_, r)
                | SynModuleDecl.Exception (_, r)
                | SynModuleDecl.Open (_, r)
                | SynModuleDecl.HashDirective (_, r) -> Some r
                | _ -> None
                |> Option.map (fun r -> r.StartColumn)


        let rec walkImplFileInput (ParsedImplFileInput (modules = moduleOrNamespaceList)) = 
            List.iter (walkSynModuleOrNamespace []) moduleOrNamespaceList

        and walkSynModuleOrNamespace (parent: LongIdent) (SynModuleOrNamespace(ident, _, kind, decls, _, _, _, range)) =
            if range.EndLine >= currentLine then
                let isModule = kind.IsModule
                match isModule, parent, ident with
                | false, _, _ -> ns := Some (longIdentToIdents ident)
                // top level module with "inlined" namespace like Ns1.Ns2.TopModule
                | true, [], _f :: _s :: _ -> 
                    let ident = longIdentToIdents ident
                    ns := Some (ident.[0..ident.Length - 2])
                | _ -> ()
                
                let fullIdent = parent @ ident

                let startLine =
                    if isModule then range.StartLine
                    else range.StartLine - 1

                let scopeKind =
                    match isModule, parent with
                    | true, [] -> TopModule
                    | true, _ -> NestedModule
                    | _ -> Namespace

                doRange scopeKind fullIdent startLine range.StartColumn
                addModule (fullIdent, range)
                List.iter (walkSynModuleDecl fullIdent) decls

        and walkSynModuleDecl (parent: LongIdent) (decl: SynModuleDecl) =
            match decl with
            | SynModuleDecl.NamespaceFragment fragment -> walkSynModuleOrNamespace parent fragment
            | SynModuleDecl.NestedModule(ComponentInfo(_, _, _, ident, _, _, _, _), _, decls, _, range) ->
                let fullIdent = parent @ ident
                addModule (fullIdent, range)
                if range.EndLine >= currentLine then
                    let moduleBodyIdentation = getMinColumn decls |> Option.defaultValue (range.StartColumn + 4)
                    doRange NestedModule fullIdent range.StartLine moduleBodyIdentation
                    List.iter (walkSynModuleDecl fullIdent) decls
            | SynModuleDecl.Open (_, range) -> doRange OpenDeclaration [] range.EndLine (range.StartColumn - 5)
            | SynModuleDecl.HashDirective (_, range) -> doRange HashDirective [] range.EndLine range.StartColumn
            | _ -> ()

        match ast with 
        | ParsedInput.SigFile _ -> ()
        | ParsedInput.ImplFile input -> walkImplFileInput input

        let res =
            !result
            |> Option.map (fun (scope, pos, _) ->
                let ns = !ns |> Option.map longIdentToIdents
                scope, ns, mkPos (pos.Line + 1) pos.Column)
        
        let modules = 
            modules 
            |> Seq.filter (fun x -> x.Range.EndLine < currentLine)
            |> Seq.sortBy (fun x -> -x.Idents.Length)
            |> Seq.toList

        res, modules

    let findBestPositionToInsertOpenDeclaration (modules: Module list) scope pos (entity: Idents) =
        match modules |> List.filter (fun x -> entity |> Array.startsWith x.Idents) with
        | [] -> { ScopeKind = scope.Kind; Pos = pos }
        | m :: _ ->
            //printfn "All modules: %A, Win module: %A" modules m
            let scopeKind =
                match scope.Kind with
                | TopModule -> NestedModule
                | x -> x
            { ScopeKind = scopeKind
              Pos = mkPos (Line.fromZ m.Range.EndLine) m.Range.StartColumn }

    let tryFindInsertionContext (currentLine: int) (ast: ParsedInput) (partiallyQualifiedName: MaybeUnresolvedIdents) (insertionPoint: OpenStatementInsertionPoint) = 
        let res, modules = tryFindNearestPointAndModules currentLine ast insertionPoint
        // CLEANUP: does this really need to be a partial application with pre-computation?  Can this be made more explicit?
        fun (requiresQualifiedAccessParent: Idents option, autoOpenParent: Idents option, entityNamespace: Idents option, entity: Idents) ->

            // We ignore all diagnostics during this operation
            //
            // Based on an initial review, no diagnostics should be generated.  However the code should be checked more closely.
            use _ignoreAllDiagnostics = new ErrorScope()  
            match res with
            | None -> [||]
            | Some (scope, ns, pos) -> 
                Entity.tryCreate(ns, scope.Idents, partiallyQualifiedName, requiresQualifiedAccessParent, autoOpenParent, entityNamespace, entity)
                |> Array.map (fun e -> e, findBestPositionToInsertOpenDeclaration modules scope pos entity)

    /// Corrects insertion line number based on kind of scope and text surrounding the insertion point.
    let adjustInsertionPoint (getLineStr: int -> string) ctx  =
        let line =
            match ctx.ScopeKind with
            | ScopeKind.TopModule ->
                if ctx.Pos.Line > 1 then
                    // it's an implicit module without any open declarations    
                    let line = getLineStr (ctx.Pos.Line - 2)
                    let isImpliciteTopLevelModule =
                        not (line.StartsWithOrdinal("module") && not (line.EndsWithOrdinal("=")))
                    if isImpliciteTopLevelModule then 1 else ctx.Pos.Line
                else 1
            | ScopeKind.Namespace ->
                // for namespaces the start line is start line of the first nested entity
                if ctx.Pos.Line > 1 then
                    [0..ctx.Pos.Line - 1]
                    |> List.mapi (fun i line -> i, getLineStr line)
                    |> List.tryPick (fun (i, lineStr) -> 
                        if lineStr.StartsWithOrdinal("namespace") then Some i
                        else None)
                    |> function
                        // move to the next line below "namespace" and convert it to F# 1-based line number
                        | Some line -> line + 2 
                        | None -> ctx.Pos.Line
                else 1  
            | _ -> ctx.Pos.Line

        mkPos line ctx.Pos.Column
    
    let findNearestPointToInsertOpenDeclaration (currentLine: int) (ast: ParsedInput) (entity: Idents) (insertionPoint: OpenStatementInsertionPoint) =
        match tryFindNearestPointAndModules currentLine ast insertionPoint with
        | Some (scope, _, point), modules -> 
            findBestPositionToInsertOpenDeclaration modules scope point entity
        | _ ->
            // we failed to find insertion point because ast is empty for some reason, return top left point in this case  
            { ScopeKind = ScopeKind.TopModule
              Pos = mkPos 1 0 }
