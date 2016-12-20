// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open System
open Microsoft.FSharp.Compiler.Ast
open System.Collections.Generic
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Range

type internal ShortIdent = string
type Idents = ShortIdent[]
type IsAutoOpen = bool
type ModuleKind = { IsAutoOpen: bool; HasModuleSuffix: bool }

[<AutoOpen>]
module private Utils =
    [<RequireQualifiedAccess>]
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Option =
        let inline attempt (f: unit -> 'T) = try Some <| f() with _ -> None

    [<RequireQualifiedAccess>]
    [<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
    module Array =
        /// Returns a new array with an element replaced with a given value.
        let replace index value (array: _ []) =
            if index >= array.Length then raise (IndexOutOfRangeException "index")
            let res = Array.copy array
            res.[index] <- value
            res

    type FSharpEntity with
        member x.TryGetFullName() =
            Option. attempt (fun _ -> x.TryFullName)
            |> Option.flatten
            |> Option.orElseWith (fun _ ->
                Option.attempt (fun _ -> String.Join(".", x.AccessPath, x.DisplayName)))

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
            Option.attempt (fun _ -> x.MembersFunctionsAndValues) |> Option.defaultValue ([||] :> _)

    let isOperator (name: string) =
        name.StartsWith "( " && name.EndsWith " )" && name.Length > 4
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
                Option.attempt (fun _ -> x.EnclosingEntity)
                |> Option.bind (fun e -> e.TryGetFullName())
                |> Option.map (fun enclosingEntityFullName -> 
                     Array.append (enclosingEntityFullName.Split '.') [| x.CompiledName |])
            else None

    type FSharpAssemblySignature with
        member x.TryGetEntities() = try x.Entities :> _ seq with _ -> Seq.empty

    let isAttribute<'T> (attribute: FSharpAttribute) =
        // CompiledName throws exception on DataContractAttribute generated by SQLProvider
        match (try Some attribute.AttributeType.CompiledName with _ -> None) with
        | Some name when name = typeof<'T>.Name -> true
        | _ -> false

    let hasAttribute<'T> (attributes: seq<FSharpAttribute>) =
        attributes |> Seq.exists isAttribute<'T>

    let tryGetAttribute<'T> (attributes: seq<FSharpAttribute>) =
        attributes |> Seq.tryFind isAttribute<'T>

    let hasModuleSuffixAttribute (entity: FSharpEntity) = 
        entity.Attributes
        |> tryGetAttribute<CompilationRepresentationAttribute>
        |> Option.bind (fun a -> 
            try Some a.ConstructorArguments with _ -> None
            |> Option.bind (fun args -> args |> Seq.tryPick (fun (_, arg) ->
                let res =
                    match arg with
                    | :? int32 as arg when arg = int CompilationRepresentationFlags.ModuleSuffix -> 
                        Some() 
                    | :? CompilationRepresentationFlags as arg when arg = CompilationRepresentationFlags.ModuleSuffix -> 
                        Some() 
                    | _ -> 
                        None
                res)))
        |> Option.isSome

type EntityKind =
    | Attribute
    | Type
    | FunctionOrValue of isActivePattern:bool
    | Module of ModuleKind
    override x.ToString() = sprintf "%A" x

type RawEntity = 
    { /// Full entity name as it's seen in compiled code (raw FSharpEntity.FullName, FSharpValueOrFunction.FullName). 
      FullName: string
      /// Entity name parts with removed module suffixes (Ns.M1Module.M2Module.M3.entity -> Ns.M1.M2.M3.entity)
      /// and replaced compiled names with display names (FSharpEntity.DisplayName, FSharpValueOrFucntion.DisplayName).
      /// Note: *all* parts are cleaned, not the last one. 
      CleanedIdents: Idents
      Namespace: Idents option
      IsPublic: bool
      TopRequireQualifiedAccessParent: Idents option
      AutoOpenParent: Idents option
      Kind: EntityKind }
    override x.ToString() = sprintf "%A" x  

type AssemblyPath = string
type AssemblyContentType = Public | Full

type internal Parent = 
    { Namespace: Idents option
      RequiresQualifiedAccess: Idents option
      AutoOpen: Idents option
      WithModuleSuffix: Idents option }
    static member Empty = 
        { Namespace = None
          RequiresQualifiedAccess = None
          AutoOpen = None
          WithModuleSuffix = None }
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
            if entity.IsFSharpModule && idents.Length > 0 && hasModuleSuffixAttribute entity then
                let lastIdent = idents.[idents.Length - 1]
                if lastIdent.EndsWith "Module" then
                    idents |> Array.replace (idents.Length - 1) (lastIdent.Substring(0, lastIdent.Length - 6))
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

module internal TypedAstPatterns =
    let (|TypeWithDefinition|_|) (ty: FSharpType) =
        if ty.HasTypeDefinition then Some ty.TypeDefinition
        else None

    let (|Attribute|_|) (entity: FSharpEntity) =
        let isAttribute (entity: FSharpEntity) =
            let getBaseType (entity: FSharpEntity) =
                try 
                    match entity.BaseType with
                    | Some (TypeWithDefinition def) -> Some def
                    | _ -> None
                with _ -> None

            let rec isAttributeType (ty: FSharpEntity option) =
                match ty with
                | None -> false
                | Some ty ->
                    match ty.TryGetFullName() with
                    | None -> false
                    | Some fullName ->
                        fullName = "System.Attribute" || isAttributeType (getBaseType ty)
            isAttributeType (Some entity)
        if isAttribute entity then Some() else None

    let (|FSharpModule|_|) (entity: FSharpEntity) = if entity.IsFSharpModule then Some() else None

type internal AssemblyContentCacheEntry =
    { FileWriteTime: DateTime 
      ContentType: AssemblyContentType 
      Entities: RawEntity list }

[<NoComparison; NoEquality>]
type internal IAssemblyContentCache =
    abstract TryGet: AssemblyPath -> AssemblyContentCacheEntry option
    abstract Set: AssemblyPath -> AssemblyContentCacheEntry -> unit

module internal AssemblyContentProvider =
    open System.IO

    let private createEntity ns (parent: Parent) (entity: FSharpEntity) =
        parent.FormatEntityFullName entity
        |> Option.map (fun (fullName, cleanIdents) ->
            { FullName = fullName
              CleanedIdents = cleanIdents
              Namespace = ns
              IsPublic = entity.Accessibility.IsPublic
              TopRequireQualifiedAccessParent = parent.RequiresQualifiedAccess |> Option.map parent.FixParentModuleSuffix
              AutoOpenParent = parent.AutoOpen |> Option.map parent.FixParentModuleSuffix
              Kind = 
                match entity with
                | TypedAstPatterns.Attribute -> EntityKind.Attribute 
                | TypedAstPatterns.FSharpModule ->
                    EntityKind.Module 
                        { IsAutoOpen = hasAttribute<AutoOpenAttribute> entity.Attributes
                          HasModuleSuffix = hasModuleSuffixAttribute entity }
                | _ -> EntityKind.Type })

    let private traverseMemberFunctionAndValues ns (parent: Parent) (membersFunctionsAndValues: seq<FSharpMemberOrFunctionOrValue>) =
        membersFunctionsAndValues
        |> Seq.collect (fun func ->
            let processIdents fullName idents = 
                { FullName = fullName
                  CleanedIdents = parent.FixParentModuleSuffix idents
                  Namespace = ns
                  IsPublic = func.Accessibility.IsPublic
                  TopRequireQualifiedAccessParent = 
                        parent.RequiresQualifiedAccess |> Option.map parent.FixParentModuleSuffix
                  AutoOpenParent = parent.AutoOpen |> Option.map parent.FixParentModuleSuffix
                  Kind = EntityKind.FunctionOrValue func.IsActivePattern }

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

    let rec private traverseEntity contentType (parent: Parent) (entity: FSharpEntity) = 

        seq { if not entity.IsProvided then
                match contentType, entity.Accessibility.IsPublic with
                | Full, _ | Public, true ->
                    let ns = entity.Namespace |> Option.map (fun x -> x.Split '.') |> Option.orElse parent.Namespace
                    let currentEntity = createEntity ns parent entity

                    match currentEntity with
                    | Some x -> yield x
                    | None -> ()

                    let currentParent =
                        { RequiresQualifiedAccess =
                            parent.RequiresQualifiedAccess
                            |> Option.orElse (
                                if hasAttribute<RequireQualifiedAccessAttribute> entity.Attributes then 
                                    parent.FormatEntityFullName entity |> Option.map snd
                                else None)
                          AutoOpen =
                            let isAutoOpen = entity.IsFSharpModule && hasAttribute<AutoOpenAttribute> entity.Attributes
                            match isAutoOpen, parent.AutoOpen with
                            // if parent is also AutoOpen, then keep the parent
                            | true, Some parent -> Some parent 
                            // if parent is not AutoOpen, but current entity is, peek the latter as a new AutoOpen module
                            | true, None -> parent.FormatEntityFullName entity |> Option.map snd
                            // if current entity is not AutoOpen, we discard whatever parent was
                            | false, _ -> None 

                          WithModuleSuffix = 
                            if entity.IsFSharpModule && hasModuleSuffixAttribute entity then 
                                currentEntity |> Option.map (fun e -> e.CleanedIdents) 
                            else parent.WithModuleSuffix
                          Namespace = ns }

                    if entity.IsFSharpModule then
                        match entity.TryGetMembersFunctionsAndValues with
                        | xs when xs.Count > 0 ->
                            yield! traverseMemberFunctionAndValues ns currentParent xs
                        | _ -> ()

                    for e in (try entity.NestedEntities :> _ seq with _ -> Seq.empty) do
                        yield! traverseEntity contentType currentParent e 
                | _ -> () }

    let getAssemblySignatureContent contentType (signature: FSharpAssemblySignature) =
            signature.TryGetEntities()
            |> Seq.collect (traverseEntity contentType Parent.Empty)
            |> Seq.distinct

    let private getAssemblySignaturesContent contentType (assemblies: FSharpAssembly list) = 
        assemblies 
        |> Seq.collect (fun asm -> getAssemblySignatureContent contentType asm.Contents)
        |> Seq.toList

    let getAssemblyContent (withCache: (IAssemblyContentCache -> _) -> _) 
                           contentType (fileName: string option) (assemblies: FSharpAssembly list) =
        match assemblies |> List.filter (fun x -> not x.IsProviderGenerated), fileName with
        | [], _ -> []
        | assemblies, Some fileName ->
            let fileWriteTime = FileInfo(fileName).LastWriteTime 
            withCache <| fun cache ->
                match contentType, cache.TryGet fileName with 
                | _, Some entry
                | Public, Some entry when entry.FileWriteTime = fileWriteTime -> entry.Entities
                | _ ->
                    let entities = getAssemblySignaturesContent contentType assemblies
                    cache.Set fileName { FileWriteTime = fileWriteTime; ContentType = contentType; Entities = entities }
                    entities
        | assemblies, None -> 
            getAssemblySignaturesContent contentType assemblies
        |> List.filter (fun entity -> 
            match contentType, entity.IsPublic with
            | Full, _ | Public, true -> true
            | _ -> false)

type internal EntityCache() =
    let dic = Dictionary<AssemblyPath, AssemblyContentCacheEntry>()
    interface IAssemblyContentCache with
        member __.TryGet assembly =
            match dic.TryGetValue assembly with
            | true, entry -> Some entry
            | _ -> None
        member __.Set assembly entry = dic.[assembly] <- entry

    member __.Clear() = dic.Clear()
    member x.Locking f = lock dic <| fun _ -> f (x :> IAssemblyContentCache)

module internal ParsedInput =
    open Microsoft.FSharp.Compiler
    open Microsoft.FSharp.Compiler.Ast

    type private EndLine = int

    /// An recursive pattern that collect all sequential expressions to avoid StackOverflowException
    let rec (|Sequentials|_|) = function
        | SynExpr.Sequential(_, _, e, Sequentials es, _) ->
            Some(e::es)
        | SynExpr.Sequential(_, _, e1, e2, _) ->
            Some [e1; e2]
        | _ -> None

    let getEntityKind (input: ParsedInput) (pos: Range.pos) : EntityKind option =
        let (|ConstructorPats|) = function
            | Pats ps -> ps
            | NamePatPairs(xs, _) -> List.map snd xs

        let isPosInRange range = Range.rangeContainsPos range pos

        let ifPosInRange range f =
            if isPosInRange range then f()
            else None

        let rec walkImplFileInput (ParsedImplFileInput(_, _, _, _, _, moduleOrNamespaceList, _)) = 
            List.tryPick (walkSynModuleOrNamespace true) moduleOrNamespaceList

        and walkSynModuleOrNamespace isTopLevel (SynModuleOrNamespace(_, _, isModule, decls, _, attrs, _, r)) =
            if isModule && isTopLevel then None else List.tryPick walkAttribute attrs
            |> Option.orElse (ifPosInRange r (fun _ -> List.tryPick (walkSynModuleDecl isTopLevel) decls))

        and walkAttribute (attr: SynAttribute) = 
            if isPosInRange attr.Range then Some EntityKind.Attribute else None
            |> Option.orElse (walkExprWithKind (Some EntityKind.Type) attr.ArgExpr)

        and walkTypar (Typar (ident, _, _)) = ifPosInRange ident.idRange (fun _ -> Some EntityKind.Type)

        and walkTyparDecl (SynTyparDecl.TyparDecl (attrs, typar)) = 
            List.tryPick walkAttribute attrs
            |> Option.orElse (walkTypar typar)
            
        and walkTypeConstraint = function
            | SynTypeConstraint.WhereTyparDefaultsToType (t1, t2, _) -> walkTypar t1 |> Option.orElse (walkType t2)
            | SynTypeConstraint.WhereTyparIsValueType(t, _) -> walkTypar t
            | SynTypeConstraint.WhereTyparIsReferenceType(t, _) -> walkTypar t
            | SynTypeConstraint.WhereTyparIsUnmanaged(t, _) -> walkTypar t
            | SynTypeConstraint.WhereTyparSupportsNull (t, _) -> walkTypar t
            | SynTypeConstraint.WhereTyparIsComparable(t, _) -> walkTypar t
            | SynTypeConstraint.WhereTyparIsEquatable(t, _) -> walkTypar t
            | SynTypeConstraint.WhereTyparSubtypeOfType(t, ty, _) -> walkTypar t |> Option.orElse (walkType ty)
            | SynTypeConstraint.WhereTyparSupportsMember(ts, sign, _) -> 
                List.tryPick walkType ts |> Option.orElse (walkMemberSig sign)
            | SynTypeConstraint.WhereTyparIsEnum(t, ts, _) -> walkTypar t |> Option.orElse (List.tryPick walkType ts)
            | SynTypeConstraint.WhereTyparIsDelegate(t, ts, _) -> walkTypar t |> Option.orElse (List.tryPick walkType ts)

        and walkPatWithKind (kind: EntityKind option) = function
            | SynPat.Ands (pats, _) -> List.tryPick walkPat pats
            | SynPat.Named(SynPat.Wild nameRange as pat, _, _, _, _) -> 
                if isPosInRange nameRange then None
                else walkPat pat
            | SynPat.Typed(pat, t, _) -> walkPat pat |> Option.orElse (walkType t)
            | SynPat.Attrib(pat, attrs, _) -> walkPat pat |> Option.orElse (List.tryPick walkAttribute attrs)
            | SynPat.Or(pat1, pat2, _) -> List.tryPick walkPat [pat1; pat2]
            | SynPat.LongIdent(_, _, typars, ConstructorPats pats, _, r) -> 
                ifPosInRange r (fun _ -> kind)
                |> Option.orElse (
                    typars 
                    |> Option.bind (fun (SynValTyparDecls (typars, _, constraints)) -> 
                        List.tryPick walkTyparDecl typars
                        |> Option.orElse (List.tryPick walkTypeConstraint constraints)))
                |> Option.orElse (List.tryPick walkPat pats)
            | SynPat.Tuple(pats, _) -> List.tryPick walkPat pats
            | SynPat.Paren(pat, _) -> walkPat pat
            | SynPat.ArrayOrList(_, pats, _) -> List.tryPick walkPat pats
            | SynPat.IsInst(t, _) -> walkType t
            | SynPat.QuoteExpr(e, _) -> walkExpr e
            | _ -> None

        and walkPat = walkPatWithKind None

        and walkBinding (SynBinding.Binding(_, _, _, _, attrs, _, _, pat, returnInfo, e, _, _)) =
            List.tryPick walkAttribute attrs
            |> Option.orElse (walkPat pat)
            |> Option.orElse (walkExpr e)
            |> Option.orElse (
                match returnInfo with
                | Some (SynBindingReturnInfo (t, _, _)) -> walkType t
                | None -> None)

        and walkInterfaceImpl (InterfaceImpl(_, bindings, _)) =
            List.tryPick walkBinding bindings

        and walkIndexerArg = function
            | SynIndexerArg.One e -> walkExpr e
            | SynIndexerArg.Two(e1, e2) -> List.tryPick walkExpr [e1; e2]

        and walkType = function
            | SynType.LongIdent ident -> ifPosInRange ident.Range (fun _ -> Some EntityKind.Type)
            | SynType.App(ty, _, types, _, _, _, _) -> 
                walkType ty |> Option.orElse (List.tryPick walkType types)
            | SynType.LongIdentApp(_, _, _, types, _, _, _) -> List.tryPick walkType types
            | SynType.Tuple(ts, _) -> ts |> List.tryPick (fun (_, t) -> walkType t)
            | SynType.Array(_, t, _) -> walkType t
            | SynType.Fun(t1, t2, _) -> walkType t1 |> Option.orElse (walkType t2)
            | SynType.WithGlobalConstraints(t, _, _) -> walkType t
            | SynType.HashConstraint(t, _) -> walkType t
            | SynType.MeasureDivide(t1, t2, _) -> walkType t1 |> Option.orElse (walkType t2)
            | SynType.MeasurePower(t, _, _) -> walkType t
            | _ -> None

        and walkClause (Clause(pat, e1, e2, _, _)) =
            walkPatWithKind (Some EntityKind.Type) pat 
            |> Option.orElse (walkExpr e2)
            |> Option.orElse (Option.bind walkExpr e1)

        and walkExprWithKind (parentKind: EntityKind option) = function
            | SynExpr.LongIdent (_, LongIdentWithDots(_, dotRanges), _, r) ->
                match dotRanges with
                | [] when isPosInRange r -> parentKind |> Option.orElse (Some (EntityKind.FunctionOrValue false)) 
                | firstDotRange :: _  ->
                    let firstPartRange = 
                        Range.mkRange "" r.Start (Range.mkPos firstDotRange.StartLine (firstDotRange.StartColumn - 1))
                    if isPosInRange firstPartRange then
                        parentKind |> Option.orElse (Some (EntityKind.FunctionOrValue false))
                    else None
                | _ -> None
            | SynExpr.Paren (e, _, _, _) -> walkExprWithKind parentKind e
            | SynExpr.Quote(_, _, e, _, _) -> walkExprWithKind parentKind e
            | SynExpr.Typed(e, _, _) -> walkExprWithKind parentKind e
            | SynExpr.Tuple(es, _, _) -> List.tryPick (walkExprWithKind parentKind) es
            | SynExpr.ArrayOrList(_, es, _) -> List.tryPick (walkExprWithKind parentKind) es
            | SynExpr.Record(_, _, fields, r) -> 
                ifPosInRange r (fun _ ->
                    fields |> List.tryPick (fun (_, e, _) -> e |> Option.bind (walkExprWithKind parentKind)))
            | SynExpr.New(_, t, e, _) -> walkExprWithKind parentKind e |> Option.orElse (walkType t)
            | SynExpr.ObjExpr(ty, _, bindings, ifaces, _, _) -> 
                walkType ty
                |> Option.orElse (List.tryPick walkBinding bindings)
                |> Option.orElse (List.tryPick walkInterfaceImpl ifaces)
            | SynExpr.While(_, e1, e2, _) -> List.tryPick (walkExprWithKind parentKind) [e1; e2]
            | SynExpr.For(_, _, e1, _, e2, e3, _) -> List.tryPick (walkExprWithKind parentKind) [e1; e2; e3]
            | SynExpr.ForEach(_, _, _, _, e1, e2, _) -> List.tryPick (walkExprWithKind parentKind) [e1; e2]
            | SynExpr.ArrayOrListOfSeqExpr(_, e, _) -> walkExprWithKind parentKind e
            | SynExpr.CompExpr(_, _, e, _) -> walkExprWithKind parentKind e
            | SynExpr.Lambda(_, _, _, e, _) -> walkExprWithKind parentKind e
            | SynExpr.MatchLambda(_, _, synMatchClauseList, _, _) -> 
                List.tryPick walkClause synMatchClauseList
            | SynExpr.Match(_, e, synMatchClauseList, _, _) -> 
                walkExprWithKind parentKind e |> Option.orElse (List.tryPick walkClause synMatchClauseList)
            | SynExpr.Do(e, _) -> walkExprWithKind parentKind e
            | SynExpr.Assert(e, _) -> walkExprWithKind parentKind e
            | SynExpr.App(_, _, e1, e2, _) -> List.tryPick (walkExprWithKind parentKind) [e1; e2]
            | SynExpr.TypeApp(e, _, tys, _, _, _, _) -> 
                walkExprWithKind (Some EntityKind.Type) e |> Option.orElse (List.tryPick walkType tys)
            | SynExpr.LetOrUse(_, _, bindings, e, _) -> List.tryPick walkBinding bindings |> Option.orElse (walkExprWithKind parentKind e)
            | SynExpr.TryWith(e, _, clauses, _, _, _, _) -> walkExprWithKind parentKind e |> Option.orElse (List.tryPick walkClause clauses)
            | SynExpr.TryFinally(e1, e2, _, _, _) -> List.tryPick (walkExprWithKind parentKind) [e1; e2]
            | SynExpr.Lazy(e, _) -> walkExprWithKind parentKind e
            | Sequentials es -> List.tryPick (walkExprWithKind parentKind) es
            | SynExpr.IfThenElse(e1, e2, e3, _, _, _, _) -> 
                List.tryPick (walkExprWithKind parentKind) [e1; e2] |> Option.orElse (match e3 with None -> None | Some e -> walkExprWithKind parentKind e)
            | SynExpr.Ident ident -> ifPosInRange ident.idRange (fun _ -> Some (EntityKind.FunctionOrValue false))
            | SynExpr.LongIdentSet(_, e, _) -> walkExprWithKind parentKind e
            | SynExpr.DotGet(e, _, _, _) -> walkExprWithKind parentKind e
            | SynExpr.DotSet(e, _, _, _) -> walkExprWithKind parentKind e
            | SynExpr.DotIndexedGet(e, args, _, _) -> walkExprWithKind parentKind e |> Option.orElse (List.tryPick walkIndexerArg args)
            | SynExpr.DotIndexedSet(e, args, _, _, _, _) -> walkExprWithKind parentKind e |> Option.orElse (List.tryPick walkIndexerArg args)
            | SynExpr.NamedIndexedPropertySet(_, e1, e2, _) -> List.tryPick (walkExprWithKind parentKind) [e1; e2]
            | SynExpr.DotNamedIndexedPropertySet(e1, _, e2, e3, _) -> List.tryPick (walkExprWithKind parentKind) [e1; e2; e3]
            | SynExpr.TypeTest(e, t, _) -> walkExprWithKind parentKind e |> Option.orElse (walkType t)
            | SynExpr.Upcast(e, t, _) -> walkExprWithKind parentKind e |> Option.orElse (walkType t)
            | SynExpr.Downcast(e, t, _) -> walkExprWithKind parentKind e |> Option.orElse (walkType t)
            | SynExpr.InferredUpcast(e, _) -> walkExprWithKind parentKind e
            | SynExpr.InferredDowncast(e, _) -> walkExprWithKind parentKind e
            | SynExpr.AddressOf(_, e, _, _) -> walkExprWithKind parentKind e
            | SynExpr.JoinIn(e1, _, e2, _) -> List.tryPick (walkExprWithKind parentKind) [e1; e2]
            | SynExpr.YieldOrReturn(_, e, _) -> walkExprWithKind parentKind e
            | SynExpr.YieldOrReturnFrom(_, e, _) -> walkExprWithKind parentKind e
            | SynExpr.LetOrUseBang(_, _, _, _, e1, e2, _) -> List.tryPick (walkExprWithKind parentKind) [e1; e2]
            | SynExpr.DoBang(e, _) -> walkExprWithKind parentKind e
            | SynExpr.TraitCall (ts, sign, e, _) ->
                List.tryPick walkTypar ts 
                |> Option.orElse (walkMemberSig sign)
                |> Option.orElse (walkExprWithKind parentKind e)
            | _ -> None

        and walkExpr = walkExprWithKind None

        and walkSimplePat = function
            | SynSimplePat.Attrib (pat, attrs, _) ->
                walkSimplePat pat |> Option.orElse (List.tryPick walkAttribute attrs)
            | SynSimplePat.Typed(pat, t, _) -> walkSimplePat pat |> Option.orElse (walkType t)
            | _ -> None

        and walkField (SynField.Field(attrs, _, _, t, _, _, _, _)) =
            List.tryPick walkAttribute attrs |> Option.orElse (walkType t)

        and walkValSig (SynValSig.ValSpfn(attrs, _, _, t, _, _, _, _, _, _, _)) =
            List.tryPick walkAttribute attrs |> Option.orElse (walkType t)

        and walkMemberSig = function
            | SynMemberSig.Inherit (t, _) -> walkType t
            | SynMemberSig.Member(vs, _, _) -> walkValSig vs
            | SynMemberSig.Interface(t, _) -> walkType t
            | SynMemberSig.ValField(f, _) -> walkField f
            | SynMemberSig.NestedType(SynTypeDefnSig.TypeDefnSig (info, repr, memberSigs, _), _) -> 
                walkComponentInfo false info
                |> Option.orElse (walkTypeDefnSigRepr repr)
                |> Option.orElse (List.tryPick walkMemberSig memberSigs)

        and walkMember = function
            | SynMemberDefn.AbstractSlot (valSig, _, _) -> walkValSig valSig
            | SynMemberDefn.Member(binding, _) -> walkBinding binding
            | SynMemberDefn.ImplicitCtor(_, attrs, pats, _, _) -> 
                List.tryPick walkAttribute attrs |> Option.orElse (List.tryPick walkSimplePat pats)
            | SynMemberDefn.ImplicitInherit(t, e, _, _) -> walkType t |> Option.orElse (walkExpr e)
            | SynMemberDefn.LetBindings(bindings, _, _, _) -> List.tryPick walkBinding bindings
            | SynMemberDefn.Interface(t, members, _) -> 
                walkType t 
                |> Option.orElse (members |> Option.bind (List.tryPick walkMember))
            | SynMemberDefn.Inherit(t, _, _) -> walkType t
            | SynMemberDefn.ValField(field, _) -> walkField field
            | SynMemberDefn.NestedType(tdef, _, _) -> walkTypeDefn tdef
            | SynMemberDefn.AutoProperty(attrs, _, _, t, _, _, _, _, e, _, _) -> 
                List.tryPick walkAttribute attrs
                |> Option.orElse (Option.bind walkType t)
                |> Option.orElse (walkExpr e)
            | _ -> None

        and walkEnumCase (EnumCase(attrs, _, _, _, _)) = List.tryPick walkAttribute attrs

        and walkUnionCaseType = function
            | SynUnionCaseType.UnionCaseFields fields -> List.tryPick walkField fields
            | SynUnionCaseType.UnionCaseFullType(t, _) -> walkType t

        and walkUnionCase (UnionCase(attrs, _, t, _, _, _)) = 
            List.tryPick walkAttribute attrs |> Option.orElse (walkUnionCaseType t)

        and walkTypeDefnSimple = function
            | SynTypeDefnSimpleRepr.Enum (cases, _) -> List.tryPick walkEnumCase cases
            | SynTypeDefnSimpleRepr.Union(_, cases, _) -> List.tryPick walkUnionCase cases
            | SynTypeDefnSimpleRepr.Record(_, fields, _) -> List.tryPick walkField fields
            | SynTypeDefnSimpleRepr.TypeAbbrev(_, t, _) -> walkType t
            | _ -> None

        and walkComponentInfo isModule (ComponentInfo(attrs, typars, constraints, _, _, _, _, r)) =
            if isModule then None else ifPosInRange r (fun _ -> Some EntityKind.Type)
            |> Option.orElse (
                List.tryPick walkAttribute attrs
                |> Option.orElse (List.tryPick walkTyparDecl typars)
                |> Option.orElse (List.tryPick walkTypeConstraint constraints))

        and walkTypeDefnRepr = function
            | SynTypeDefnRepr.ObjectModel (_, defns, _) -> List.tryPick walkMember defns
            | SynTypeDefnRepr.Simple(defn, _) -> walkTypeDefnSimple defn
            | SynTypeDefnRepr.Exception(_) -> None

        and walkTypeDefnSigRepr = function
            | SynTypeDefnSigRepr.ObjectModel (_, defns, _) -> List.tryPick walkMemberSig defns
            | SynTypeDefnSigRepr.Simple(defn, _) -> walkTypeDefnSimple defn
            | SynTypeDefnSigRepr.Exception(_) -> None

        and walkTypeDefn (TypeDefn (info, repr, members, _)) =
            walkComponentInfo false info
            |> Option.orElse (walkTypeDefnRepr repr)
            |> Option.orElse (List.tryPick walkMember members)

        and walkSynModuleDecl isTopLevel (decl: SynModuleDecl) =
            match decl with
            | SynModuleDecl.NamespaceFragment fragment -> walkSynModuleOrNamespace isTopLevel fragment
            | SynModuleDecl.NestedModule(info, _, modules, _, range) ->
                walkComponentInfo true info
                |> Option.orElse (ifPosInRange range (fun _ -> List.tryPick (walkSynModuleDecl false) modules))
            | SynModuleDecl.Open _ -> None
            | SynModuleDecl.Let (_, bindings, _) -> List.tryPick walkBinding bindings
            | SynModuleDecl.DoExpr (_, expr, _) -> walkExpr expr
            | SynModuleDecl.Types (types, _) -> List.tryPick walkTypeDefn types
            | _ -> None

        let res = 
            match input with 
            | ParsedInput.SigFile _ -> None
            | ParsedInput.ImplFile input -> walkImplFileInput input
        //debug "%A" ast
        res
