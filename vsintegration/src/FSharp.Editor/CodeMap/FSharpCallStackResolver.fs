// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System

open Microsoft.CodeAnalysis

open FSharp.Compiler.Symbols
open FSharp.Compiler.Text

open CancellableTasks

/// Where a stack frame was written, together with the identity the Code Map needs to fuse the
/// node with the ones its metadata provider produces.
type internal ResolvedFrame =
    {
        DeclarationRange: range
        Project: Project
        EntityPath: string array
        /// The enclosing namespace only – containing modules and types go into `TypeChain` instead,
        /// matching how the metadata provider identifies nested types.
        Namespace: string voption
        /// Compiled type names outermost-first, each with its generic arity; the last one declares
        /// the member.
        TypeChain: struct (string * int) array
        MemberName: string voption
    }

[<RequireQualifiedAccess>]
module internal FSharpCallStackResolver =

    /// The `Module` suffix is added when a module would otherwise collide with a type of the same name.
    [<Literal>]
    let private ModuleSuffix = "Module"

    let private stripModuleSuffix (name: string) =
        if name.EndsWith(ModuleSuffix, StringComparison.Ordinal) then
            ValueSome(name.Substring(0, name.Length - ModuleSuffix.Length))
        else
            ValueNone

    /// A frame name flattens namespaces, modules and nested types into one dotted path, and an
    /// explicit interface implementation puts the interface name inside the member name. Both are
    /// undone by trying every split of the path, longest entity path first.
    /// `FindEntityByPath` looks entities up by their mangled names, so a generic segment must keep
    /// its arity suffix (`Box`1`), which the frame parser split off.
    let private mangledName (segment: FramePathSegment) =
        if segment.GenericArity = 0 then
            segment.Name
        else
            $"{segment.Name}`{segment.GenericArity}"

    let private entityPathCandidates (path: FramePathSegment array) (memberName: string voption) =
        seq {
            for split in path.Length .. -1 .. 1 do
                let entityNames = path |> Seq.truncate split |> Seq.map mangledName |> Seq.toArray
                let trailing = path |> Seq.skip split |> Seq.map _.Name |> Seq.toArray

                let qualifiedMember =
                    match memberName with
                    | ValueSome name -> ValueSome(String.Join(".", [| yield! trailing; yield name |]))
                    | ValueNone when trailing.Length = 0 -> ValueNone
                    | ValueNone -> ValueSome(String.Join(".", trailing))

                yield struct (entityNames, qualifiedMember)

                // Only worth a second candidate when a segment actually carries the suffix.
                if entityNames |> Array.exists (stripModuleSuffix >> ValueOption.isSome) then
                    let unsuffixed =
                        entityNames
                        |> Array.map (fun name -> stripModuleSuffix name |> ValueOption.defaultValue name)

                    yield struct (unsuffixed, qualifiedMember)
        }

    let private memberName frameMember =
        match frameMember with
        | FrameMethod name -> ValueSome name
        | FrameConstructor -> ValueSome ".ctor"
        | FrameStaticConstructor -> ValueSome ".cctor"
        | FramePropertyGetter name
        | FramePropertySetter name -> ValueSome name
        | FrameActivePattern cases -> ValueSome($"""|{String.Join("|", cases)}|""")
        | FrameClosureBody origin -> ValueSome origin.EnclosingName
        | FrameStartupCode -> ValueNone

    let private matchesName (name: string) (m: FSharpMemberOrFunctionOrValue) =
        String.Equals(m.CompiledName, name, StringComparison.Ordinal)
        || String.Equals(m.DisplayName, name, StringComparison.Ordinal)
        || String.Equals(m.LogicalName, name, StringComparison.Ordinal)

    let private tryFindMember (frame: ParsedFrame) (name: string) (entity: FSharpEntity) =
        let members = entity.TryGetMembersFunctionsAndValues()

        let byKind =
            match frame.Member with
            | FrameConstructor
            | FrameStaticConstructor -> members |> Seq.filter _.IsConstructor
            | FramePropertyGetter _
            | FramePropertySetter _ -> members |> Seq.filter (fun m -> m.IsProperty && matchesName name m)
            | _ -> members |> Seq.filter (matchesName name)

        byKind
        |> Seq.filter (fun m ->
            m.GenericParameters.Count = frame.MethodGenericArity
            || frame.MethodGenericArity = 0)
        |> Seq.tryHeadV

    /// A closure is compiled to its own class, so nothing in the assembly signature carries its name.
    /// Report the enclosing declaration instead - the nearest one starting at or above the line the
    /// closure was written on, which is what C# and VB do for lambdas.
    let private tryFindEnclosingDeclaration (origin: ClosureOrigin) (entity: FSharpEntity) =
        entity.TryGetMembersFunctionsAndValues()
        |> Seq.filter (fun m -> m.DeclarationLocation.StartLine <= origin.Line)
        |> Seq.sortByDescending _.DeclarationLocation.StartLine
        |> Seq.tryHeadV

    let private stripArity (compiledName: string) =
        match compiledName.IndexOf '`' with
        | -1 -> compiledName
        | i -> compiledName.Substring(0, i)

    /// Compiled type names from the outermost type or module down to `entity`, with generic arities.
    let private typeChainOf (entity: FSharpEntity) =
        let rec walk (entity: FSharpEntity) chain =
            let chain =
                struct (stripArity entity.CompiledName, entity.GenericParameters.Count) :: chain

            match entity.DeclaringEntity with
            | Some parent when not parent.IsNamespace -> walk parent chain
            | _ -> chain

        walk entity [] |> Array.ofList

    let private tryResolveInProject (frame: ParsedFrame) (signature: FSharpAssemblySignature) =
        let requested = memberName frame.Member

        entityPathCandidates frame.Path requested
        |> Seq.tryPickV (fun struct (entityPath, qualifiedMember) ->
            match signature.FindEntityByPath(List.ofArray entityPath) with
            | None -> ValueNone
            | Some entity ->
                let resolved (location: range) memberName =
                    ValueSome(location, entityPath, entity, memberName)

                match frame.Member, qualifiedMember with
                | FrameStartupCode, _ -> resolved entity.DeclarationLocation ValueNone
                | FrameClosureBody origin, _ ->
                    let enclosing =
                        match qualifiedMember with
                        | ValueSome name ->
                            tryFindMember frame name entity
                            |> ValueOption.orElseWith (fun () -> tryFindEnclosingDeclaration origin entity)
                        | ValueNone -> tryFindEnclosingDeclaration origin entity

                    enclosing
                    |> ValueOption.bind (fun m -> resolved m.DeclarationLocation (ValueSome m.CompiledName))
                | _, ValueSome name ->
                    tryFindMember frame name entity
                    |> ValueOption.bind (fun m -> resolved m.DeclarationLocation (ValueSome m.CompiledName))
                | _, ValueNone -> ValueNone)

    let private projectsProducing (workspace: Workspace) (assemblyName: string) =
        workspace.CurrentSolution.Projects
        |> Seq.filter (fun p ->
            p.IsFSharp
            && String.Equals(p.AssemblyName, assemblyName, StringComparison.OrdinalIgnoreCase))

    /// Maps parsed frames back to the source they were written in, searching only the projects that
    /// produced the module the debugger reported. Frames are resolved as a batch so a project is
    /// checked once for the whole stack rather than once per frame.
    let tryResolveMany (workspace: Workspace) (assemblyName: string) (frames: ParsedFrame array) =
        cancellableTask {
            let resolved = Array.create frames.Length ValueNone

            for project in projectsProducing workspace assemblyName do
                if resolved |> Array.exists _.IsNone then
                    let! checker, _, _, options = project.GetFSharpCompilationOptionsAsync()
                    let! results = checker.ParseAndCheckProject(options)

                    frames
                    |> Array.iteri (fun i frame ->
                        if resolved.[i].IsNone then
                            resolved.[i] <-
                                tryResolveInProject frame results.AssemblySignature
                                |> ValueOption.map (fun (declarationRange, entityPath, entity, resolvedMember) ->
                                    {
                                        DeclarationRange = declarationRange
                                        Project = project
                                        EntityPath = entityPath
                                        Namespace =
                                            match entity.Namespace with
                                            | Some ns when not (String.IsNullOrEmpty ns) -> ValueSome ns
                                            | _ -> ValueNone
                                        TypeChain = typeChainOf entity
                                        MemberName = resolvedMember
                                    }))

            return resolved
        }

    let tryResolve (workspace: Workspace) (assemblyName: string) (frame: ParsedFrame) =
        cancellableTask {
            let! resolved = tryResolveMany workspace assemblyName [| frame |]
            return Array.head resolved
        }
