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
type MaybeUnresolvedIdent = { Ident: ShortIdent; Resolved: bool }
type MaybeUnresolvedIdents = MaybeUnresolvedIdent[]
type IsAutoOpen = bool

[<AutoOpen>]
module internal Extensions =
    [<RequireQualifiedAccess>]
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module Option =
        let inline attempt (f: unit -> 'T) = try Some (f()) with _ -> None        
        let inline orElse v = function Some x -> Some x | None -> v

    [<RequireQualifiedAccess>]
    [<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
    module Array =
        /// Returns a new array with an element replaced with a given value.
        let replace index value (array: _ []) =
            if index >= array.Length then raise (IndexOutOfRangeException "index")
            let res = Array.copy array
            res.[index] <- value
            res

        /// Optimized arrays equality. ~100x faster than `array1 = array2` on strings.
        /// ~2x faster for floats
        /// ~0.8x slower for ints
        let inline areEqual (xs: 'T []) (ys: 'T []) =
            match xs, ys with
            | null, null -> true
            | [||], [||] -> true
            | null, _ | _, null -> false
            | _ when xs.Length <> ys.Length -> false
            | _ ->
                let mutable break' = false
                let mutable i = 0
                let mutable result = true
                while i < xs.Length && not break' do
                    if xs.[i] <> ys.[i] then 
                        break' <- true
                        result <- false
                    i <- i + 1
                result

        /// Returns all heads of a given array.
        /// For [|1;2;3|] it returns [|[|1; 2; 3|]; [|1; 2|]; [|1|]|]
        let heads (array: 'T []) =
            let res = Array.zeroCreate<'T[]> array.Length
            for i = array.Length - 1 downto 0 do
                res.[i] <- array.[0..i]
            res

        /// check if subArray is found in the wholeArray starting 
        /// at the provided index
        let inline isSubArray (subArray: 'T []) (wholeArray:'T []) index = 
            if isNull subArray || isNull wholeArray then false
            elif subArray.Length = 0 then true
            elif subArray.Length > wholeArray.Length then false
            elif subArray.Length = wholeArray.Length then areEqual subArray wholeArray else
            let rec loop subidx idx =
                if subidx = subArray.Length then true 
                elif subArray.[subidx] = wholeArray.[idx] then loop (subidx+1) (idx+1) 
                else false
            loop 0 index
        
        /// Returns true if one array has another as its subset from index 0.
        let startsWith (prefix: _ []) (whole: _ []) =
            isSubArray prefix whole 0
        
        /// Returns true if one array has trailing elements equal to another's.
        let endsWith (suffix: _ []) (whole: _ []) =
            isSubArray suffix whole (whole.Length-suffix.Length)
        
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

[<AutoOpen>]
module internal Utils =
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

[<RequireQualifiedAccess>]
type internal LookupType =
    | Fuzzy
    | Precise

[<NoComparison; NoEquality>]
type internal RawEntity = 
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
      Kind: LookupType -> EntityKind }
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
            try entity.IsAttributeType with _ -> false
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
              Kind = fun lookupType ->
                match entity, lookupType with                
                | TypedAstPatterns.FSharpModule, _ ->
                    EntityKind.Module 
                        { IsAutoOpen = hasAttribute<AutoOpenAttribute> entity.Attributes
                          HasModuleSuffix = hasModuleSuffixAttribute entity }
                | _, LookupType.Fuzzy ->
                    EntityKind.Type
                | _, LookupType.Precise ->
                    match entity with
                    | TypedAstPatterns.Attribute -> EntityKind.Attribute 
                    | _ -> EntityKind.Type 
            })

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
                  Kind = fun _ -> EntityKind.FunctionOrValue func.IsActivePattern }

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
                                if entity.IsFSharp && hasAttribute<RequireQualifiedAccessAttribute> entity.Attributes then 
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
            |> Seq.distinctBy (fun {FullName = fullName; CleanedIdents = cleanIdents} -> (fullName, cleanIdents))

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

type internal Entity =
    { FullRelativeName: string
      Qualifier: string
      Namespace: string option
      Name: string }
    override x.ToString() = sprintf "%A" x

[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module internal Entity =
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
            // the last part must be unresolved, otherwise we show false positive suggestions like
            // "open System" for `let _ = System.DateTime.Naaaw`. Here only "Naaw" is unresolved.
            |> Array.filter (fun x -> not (x.[x.Length - 1].Resolved))
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
                            Name = match restIdents with [|_|] -> "" | _ -> String.concat "." restIdents }) 

type internal ScopeKind =
    | Namespace
    | TopModule
    | NestedModule
    | OpenDeclaration
    | HashDirective
    override x.ToString() = sprintf "%A" x

[<Measure>] type internal FCS

type internal Point<[<Measure>]'t> = { Line : int; Column : int }

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal Point =
    let make line column : Point<'t> = { Line = line; Column = column }

type internal InsertContext =
    { ScopeKind: ScopeKind
      Pos: Point<FCS> }

module internal ParsedInput =
    type Col = int
    type private EndLine = int

    type Scope =
        { Idents: Idents
          Kind: ScopeKind }

    let tryFindInsertionContext (currentLine: int) (ast: ParsedInput) = 
        let result: (Scope * Point<FCS>) option ref = ref None
        let ns: string[] option ref = ref None
        let modules = ResizeArray<Idents * EndLine * Col>()  

        let inline longIdentToIdents ident = ident |> Seq.map (fun x -> string x) |> Seq.toArray
        
        let addModule (longIdent: LongIdent) endLine col =
            modules.Add(longIdent |> List.map string |> List.toArray, endLine, col)

        let doRange kind (scope: LongIdent) line col =
            if line <= currentLine then
                match !result with
                | None -> 
                    result := Some ({ Idents = longIdentToIdents scope; Kind = kind }, Point.make line col)
                | Some (oldScope, oldPos) ->
                    match kind, oldScope.Kind with
                    | (Namespace | NestedModule | TopModule), OpenDeclaration
                    | _ when oldPos.Line <= line ->
                        result := 
                            Some ({ Idents = 
                                        match scope with 
                                        | [] -> oldScope.Idents 
                                        | _ -> longIdentToIdents scope
                                    Kind = kind },
                                  Point.make line col)
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


        let rec walkImplFileInput (ParsedImplFileInput(_, _, _, _, _, moduleOrNamespaceList, _)) = 
            List.iter (walkSynModuleOrNamespace []) moduleOrNamespaceList

        and walkSynModuleOrNamespace (parent: LongIdent) (SynModuleOrNamespace(ident, _, isModule, decls, _, _, _, range)) =
            if range.EndLine >= currentLine then
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
                addModule fullIdent range.EndLine range.StartColumn
                List.iter (walkSynModuleDecl fullIdent) decls

        and walkSynModuleDecl (parent: LongIdent) (decl: SynModuleDecl) =
            match decl with
            | SynModuleDecl.NamespaceFragment fragment -> walkSynModuleOrNamespace parent fragment
            | SynModuleDecl.NestedModule(ComponentInfo(_, _, _, ident, _, _, _, _), _, decls, _, range) ->
                let fullIdent = parent @ ident
                addModule fullIdent range.EndLine range.StartColumn
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
            |> Option.map (fun (scope, pos) ->
                let ns = !ns |> Option.map longIdentToIdents
                scope, ns, { pos with Line = pos.Line + 1 })
        
        let modules = 
            modules 
            |> Seq.filter (fun (_, endLine, _) -> endLine < currentLine)
            |> Seq.sortBy (fun (m, _, _) -> -m.Length)
            |> Seq.toList

        fun (partiallyQualifiedName: MaybeUnresolvedIdents) 
            (requiresQualifiedAccessParent: Idents option, autoOpenParent: Idents option, entityNamespace: Idents option, entity: Idents) ->
            match res with
            | None -> [||]
            | Some (scope, ns, pos) -> 
                Entity.tryCreate(ns, scope.Idents, partiallyQualifiedName, requiresQualifiedAccessParent, autoOpenParent, entityNamespace, entity)
                |> Array.map (fun e ->
                    e,
                    match modules |> List.filter (fun (m, _, _) -> entity |> Array.startsWith m ) with
                    | [] -> { ScopeKind = scope.Kind; Pos = pos }
                    | (_, endLine, startCol) :: _ ->
                        //printfn "All modules: %A, Win module: %A" modules m
                        let scopeKind =
                            match scope.Kind with
                            | TopModule -> NestedModule
                            | x -> x
                        { ScopeKind = scopeKind; Pos = Point.make (endLine + 1) startCol })