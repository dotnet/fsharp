// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Threading

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
        [
            for split in List.length path .. -1 .. 1 do
                let entityNames = path |> List.truncate split |> List.map _.Name
                let trailing = path |> List.skip split |> List.map _.Name

                let qualifiedMember =
                    match memberName, trailing with
                    | ValueSome name, trailing -> ValueSome(String.Join(".", [ yield! trailing; yield name ]))
                    | ValueNone, [] -> ValueNone
                    | ValueNone, trailing -> ValueSome(String.Join(".", trailing))

                yield entityNames, qualifiedMember

                let unsuffixed =
                    entityNames
                    |> List.map (fun name -> stripModuleSuffix name |> ValueOption.defaultValue name)

                if unsuffixed <> entityNames then
                    yield unsuffixed, qualifiedMember
        ]

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
        |> Seq.tryHead

    /// A closure is compiled to its own class, so nothing in the assembly signature carries its name.
    /// Report the enclosing declaration instead - the nearest one starting at or above the line the
    /// closure was written on, which is what C# and VB do for lambdas.
    let private tryFindEnclosingDeclaration (origin: ClosureOrigin) (entity: FSharpEntity) =
        entity.TryGetMembersFunctionsAndValues()
        |> Seq.filter (fun m -> m.DeclarationLocation.StartLine <= origin.Line)
        |> Seq.sortByDescending _.DeclarationLocation.StartLine
        |> Seq.tryHead

    let private tryResolveInProject (frame: ParsedFrame) (signature: FSharpAssemblySignature) =
        let requested = memberName frame.Member

        entityPathCandidates frame.Path requested
        |> Seq.tryPick (fun (entityPath, qualifiedMember) ->
            match signature.FindEntityByPath entityPath with
            | None -> None
            | Some entity ->
                match frame.Member, qualifiedMember with
                | FrameStartupCode, _ -> Some(entity.DeclarationLocation, entityPath, ValueNone)
                | FrameClosureBody origin, _ ->
                    let enclosing =
                        match qualifiedMember with
                        | ValueSome name ->
                            tryFindMember frame name entity
                            |> Option.orElseWith (fun () -> tryFindEnclosingDeclaration origin entity)
                        | ValueNone -> tryFindEnclosingDeclaration origin entity

                    enclosing
                    |> Option.map (fun m -> m.DeclarationLocation, entityPath, ValueSome m.CompiledName)
                | _, ValueSome name ->
                    tryFindMember frame name entity
                    |> Option.map (fun m -> m.DeclarationLocation, entityPath, ValueSome m.CompiledName)
                | _, ValueNone -> None)

    /// Maps a parsed frame back to the source it was written in, searching only the projects that
    /// produced the module the debugger reported.
    let tryResolve (workspace: Workspace) (assemblyName: string) (frame: ParsedFrame) =
        cancellableTask {
            let projects =
                workspace.CurrentSolution.Projects
                |> Seq.filter (fun p ->
                    p.IsFSharp
                    && String.Equals(p.AssemblyName, assemblyName, StringComparison.OrdinalIgnoreCase))

            let mutable resolved = ValueNone

            for project in projects do
                if resolved.IsNone then
                    let! checker, _, _, options = project.GetFSharpCompilationOptionsAsync()
                    let! results = checker.ParseAndCheckProject(options)

                    match tryResolveInProject frame results.AssemblySignature with
                    | Some(declarationRange, entityPath, resolvedMember) ->
                        resolved <-
                            ValueSome
                                {
                                    DeclarationRange = declarationRange
                                    Project = project
                                    EntityPath = entityPath
                                    MemberName = resolvedMember
                                }
                    | None -> ()

            return resolved
        }

    let tryResolveFrameName (workspace: Workspace) (assemblyName: string) (frameName: string) (cancellationToken: CancellationToken) =
        match FSharpStackFrameNameParser.parse frameName with
        | ValueNone -> System.Threading.Tasks.Task.FromResult ValueNone
        | ValueSome frame ->
            tryResolve workspace assemblyName frame
            |> CancellableTask.start cancellationToken
