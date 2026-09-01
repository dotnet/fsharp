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
        EntityPath: string list
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
    let private entityPathCandidates (path: FramePathSegment list) (memberName: string voption) =
        seq {
            for split in List.length path .. -1 .. 1 do
                let entityNames = path |> List.truncate split |> List.map _.Name
                let trailing = path |> List.skip split |> List.map _.Name

                let qualifiedMember =
                    match memberName, trailing with
                    | ValueSome name, trailing -> ValueSome(String.Join(".", [ yield! trailing; yield name ]))
                    | ValueNone, [] -> ValueNone
                    | ValueNone, trailing -> ValueSome(String.Join(".", trailing))

                yield struct (entityNames, qualifiedMember)

                let unsuffixed =
                    entityNames
                    |> List.map (fun name -> stripModuleSuffix name |> ValueOption.defaultValue name)

                if unsuffixed <> entityNames then
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

    let private tryResolveInProject (frame: ParsedFrame) (signature: FSharpAssemblySignature) =
        let requested = memberName frame.Member

        entityPathCandidates frame.Path requested
        |> Seq.tryPickV (fun struct (entityPath, qualifiedMember) ->
            match signature.FindEntityByPath entityPath with
            | None -> ValueNone
            | Some entity ->
                match frame.Member, qualifiedMember with
                | FrameStartupCode, _ -> ValueSome(entity.DeclarationLocation, entityPath, ValueNone)
                | FrameClosureBody origin, _ ->
                    let enclosing =
                        match qualifiedMember with
                        | ValueSome name ->
                            tryFindMember frame name entity
                            |> ValueOption.orElseWith (fun () -> tryFindEnclosingDeclaration origin entity)
                        | ValueNone -> tryFindEnclosingDeclaration origin entity

                    enclosing
                    |> ValueOption.map (fun m -> m.DeclarationLocation, entityPath, ValueSome m.CompiledName)
                | _, ValueSome name ->
                    tryFindMember frame name entity
                    |> ValueOption.map (fun m -> m.DeclarationLocation, entityPath, ValueSome m.CompiledName)
                | _, ValueNone -> ValueNone)

    let private projectsProducing (workspace: Workspace) (assemblyName: string) =
        workspace.CurrentSolution.Projects
        |> Seq.filter (fun p ->
            p.IsFSharp
            && String.Equals(p.AssemblyName, assemblyName, StringComparison.OrdinalIgnoreCase))

    /// Maps parsed frames back to the source they were written in, searching only the projects that
    /// produced the module the debugger reported. Frames are resolved as a batch so a project is
    /// checked once for the whole stack rather than once per frame.
    let tryResolveMany (workspace: Workspace) (assemblyName: string) (frames: ParsedFrame list) =
        cancellableTask {
            let resolved = Array.create (List.length frames) ValueNone

            for project in projectsProducing workspace assemblyName do
                if resolved |> Array.exists _.IsNone then
                    let! checker, _, _, options = project.GetFSharpCompilationOptionsAsync()
                    let! results = checker.ParseAndCheckProject(options)

                    frames
                    |> List.iteri (fun i frame ->
                        if resolved.[i].IsNone then
                            resolved.[i] <-
                                tryResolveInProject frame results.AssemblySignature
                                |> ValueOption.map (fun (declarationRange, entityPath, resolvedMember) ->
                                    {
                                        DeclarationRange = declarationRange
                                        Project = project
                                        EntityPath = entityPath
                                        MemberName = resolvedMember
                                    }))

            return List.ofArray resolved
        }

    let tryResolve (workspace: Workspace) (assemblyName: string) (frame: ParsedFrame) =
        cancellableTask {
            let! resolved = tryResolveMany workspace assemblyName [ frame ]
            return List.head resolved
        }
