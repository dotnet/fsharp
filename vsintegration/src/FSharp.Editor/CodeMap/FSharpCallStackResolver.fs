// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System

open Microsoft.CodeAnalysis

open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open CancellableTasks

/// What kind of node the resolved frame should become on the map.
type internal ResolvedFrameKind =
    | ResolvedMethod
    | ResolvedProperty
    | ResolvedEvent

/// Something true of a frame that the Properties pane shows and the map's styles can filter on. A
/// frame carries the traits it genuinely has and nothing else, so there is no way to spell "it is
/// not a constructor" - and no way to leave a field of a wide record accidentally set.
type internal FrameTrait =
    | Public
    | Internal
    | Private
    | Static
    | Generic
    | Constructor
    | Operator
    | Extension
    | Abstract
    | CompilerGenerated
    | PropertyGet
    | PropertySet
    // Traits the code schema has no place for, because it was shaped for C# and VB.
    | Module
    | Union
    | Record
    | Exception
    | Measure
    | ActivePattern
    | UnionCaseTester
    | Function
    | Inline
    | Mutable
    /// The frame came from a compiler-generated closure class rather than from a name the user wrote.
    | Lifted

/// Where a stack frame was written, together with the identity the Code Map needs to fuse the
/// node with the ones its metadata provider produces.
type internal ResolvedFrame =
    {
        DeclarationRange: range
        Project: Project
        /// The enclosing namespace only – containing modules and types go into `TypeChain` instead,
        /// matching how the metadata provider identifies nested types.
        Namespace: string voption
        /// Compiled type names outermost-first, each with its generic arity; the last one declares
        /// the member.
        TypeChain: struct (string * int) array
        MemberName: string voption
        Kind: ResolvedFrameKind
        Traits: Set<FrameTrait>
        /// The source name, which for an operator or an active pattern reads very differently from
        /// the compiled one the node is identified by.
        DisplayName: string
    }

/// What one resolution strategy found. `tryResolveMany` is what turns it into a `ResolvedFrame`,
/// because only it knows which project the signature came from.
[<NoEquality; NoComparison>]
type private Resolution =
    {
        Range: range
        Entity: FSharpEntity
        MemberName: string voption
        Kind: ResolvedFrameKind
        Traits: Set<FrameTrait>
        DisplayName: string
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

    /// The signature exposes a property through its accessor methods, so `get_Computed` is what a
    /// `Computed.get` frame has to match - `IsProperty` alone finds nothing.
    let private matchesAccessor prefix (name: string) (m: FSharpMemberOrFunctionOrValue) =
        matchesName name m || matchesName (prefix + name) m

    let private tryFindMember (frame: ParsedFrame) (name: string) (entity: FSharpEntity) =
        let members = entity.TryGetMembersFunctionsAndValues()

        let byKind =
            match frame.Member with
            | FrameConstructor
            | FrameStaticConstructor -> members |> Seq.filter _.IsConstructor
            | FramePropertyGetter _ ->
                members
                |> Seq.filter (fun m -> (m.IsProperty || m.IsPropertyGetterMethod) && matchesAccessor "get_" name m)
            | FramePropertySetter _ ->
                members
                |> Seq.filter (fun m -> (m.IsProperty || m.IsPropertySetterMethod) && matchesAccessor "set_" name m)
            | _ -> members |> Seq.filter (matchesName name)

        byKind
        |> Seq.filter (fun m ->
            m.GenericParameters.Count = frame.MethodGenericArity
            || frame.MethodGenericArity = 0)
        |> Seq.tryHeadV

    /// The name the compiler gave the closure class, which is unique within its declaring entity and
    /// so keeps sibling and nested closures apart on the map.
    let private closureIdentity (origin: ClosureOrigin) =
        match origin.Ordinal with
        | ValueSome ordinal -> $"{origin.EnclosingName}@{origin.Line}-{ordinal}"
        | ValueNone -> $"{origin.EnclosingName}@{origin.Line}"

    /// A closure is compiled to its own class, so nothing in the assembly signature carries its name.
    /// The enclosing declaration is what locates it: the nearest one starting at or above the line.
    let private tryFindDeclarationAbove line (entity: FSharpEntity) =
        entity.TryGetMembersFunctionsAndValues()
        |> Seq.filter (fun m -> m.DeclarationLocation.StartLine <= line)
        |> Seq.sortByDescending _.DeclarationLocation.StartLine
        |> Seq.tryHeadV

    /// The line a closure class is named after, except for the ones a state machine lifts out of a
    /// `task` body: the compiler numbers those from line 1, where no declaration can be found. There
    /// the line the debugger reports for the frame is the honest one.
    let private closureLine (frame: ParsedFrame) (origin: ClosureOrigin) (entity: FSharpEntity) =
        match tryFindDeclarationAbove origin.Line entity with
        | ValueSome _ -> origin.Line
        | ValueNone ->
            match frame.SourcePosition with
            | ValueSome position -> position.Line
            | ValueNone -> origin.Line

    let private stripArity (compiledName: string) =
        match compiledName.IndexOf '`' with
        | -1 -> compiledName
        | i -> compiledName.Substring(0, i)

    /// FCS records the namespace on the outermost type or module only - a nested one reports `None` -
    /// so finding it means walking the declaring chain the same way the type chain does.
    let rec private namespaceOf (entity: FSharpEntity) =
        match entity.DeclaringEntity with
        | Some parent when not parent.IsNamespace -> namespaceOf parent
        | _ ->
            match entity.Namespace with
            | Some ns when not (String.IsNullOrEmpty ns) -> ValueSome ns
            | _ -> ValueNone

    /// Compiled type names from the outermost type or module down to `entity`, with generic arities.
    let private typeChainOf (entity: FSharpEntity) =
        let rec walk (entity: FSharpEntity) chain =
            let chain =
                struct (stripArity entity.CompiledName, entity.GenericParameters.Count) :: chain

            match entity.DeclaringEntity with
            | Some parent when not parent.IsNamespace -> walk parent chain
            | _ -> chain

        walk entity [] |> Array.ofList

    let private kindOf (m: FSharpMemberOrFunctionOrValue) =
        if m.IsProperty || m.IsPropertyGetterMethod || m.IsPropertySetterMethod then
            ResolvedProperty
        elif m.IsEventAddMethod || m.IsEventRemoveMethod then
            ResolvedEvent
        else
            ResolvedMethod

    let private declaringKind (m: FSharpMemberOrFunctionOrValue) =
        match m.DeclaringEntity with
        | Some entity -> entity.IsFSharpUnion, entity.IsFSharpRecord, entity.IsFSharpExceptionDeclaration
        | None -> false, false, false

    let private isInline (m: FSharpMemberOrFunctionOrValue) =
        match m.InlineAnnotation with
        | FSharpInlineAnnotation.AlwaysInline
        | FSharpInlineAnnotation.AggressiveInline -> true
        | _ -> false

    let private traitsOf (m: FSharpMemberOrFunctionOrValue) =
        let isUnion, isRecord, isException = declaringKind m

        set
            [
                if m.Accessibility.IsPublic then
                    Public
                if m.Accessibility.IsInternal then
                    Internal
                if m.Accessibility.IsPrivate then
                    Private
                // A module-level function compiles to a static method even though F# has no `static`
                // keyword for it, so instance membership is the honest test.
                if not m.IsInstanceMember then
                    Static
                if m.GenericParameters.Count > 0 then
                    Generic
                if m.IsConstructor then
                    Constructor
                if PrettyNaming.IsLogicalOpName m.CompiledName then
                    Operator
                if m.IsExtensionMember then
                    Extension
                if m.IsDispatchSlot then
                    Abstract
                if m.IsCompilerGenerated then
                    CompilerGenerated
                if m.IsPropertyGetterMethod then
                    PropertyGet
                if m.IsPropertySetterMethod then
                    PropertySet
                if isUnion then
                    Union
                if isRecord then
                    Record
                if isException then
                    Exception
                if m.IsActivePattern then
                    ActivePattern
                if m.IsUnionCaseTester then
                    UnionCaseTester
                if m.IsFunction then
                    Function
                if isInline m then
                    Inline
                if m.IsMutable then
                    Mutable
            ]

    let private entityTraits (entity: FSharpEntity) =
        set
            [
                if entity.Accessibility.IsPublic then
                    Public
                if entity.Accessibility.IsInternal then
                    Internal
                if entity.Accessibility.IsPrivate then
                    Private
                if entity.IsFSharpModule then
                    Static
                if entity.GenericParameters.Count > 0 then
                    Generic
                if entity.IsFSharpModule then
                    Module
                if entity.IsFSharpUnion then
                    Union
                if entity.IsFSharpRecord then
                    Record
                if entity.IsFSharpExceptionDeclaration then
                    Exception
                if entity.IsMeasure then
                    Measure
            ]

    /// A `<StartupCode$…>.$Demo.$Demo` frame names the file, not the construct: its path is useless,
    /// but the debugger reports the source line the initializer is running. Resolve by that position -
    /// the public binding written on the line (`initialized`), else the type or module whose static
    /// initializer owns it (`Initialized`); private `static let` bindings are absent from the public
    /// signature, so the enclosing entity is the closest honest name.
    let private tryResolveByPosition (position: SourcePosition) (signature: FSharpAssemblySignature) =
        let sameFile (r: range) =
            String.Equals(r.FileName, position.File, StringComparison.OrdinalIgnoreCase)

        let rec flatten (entity: FSharpEntity) =
            seq {
                entity

                for nested in entity.NestedEntities do
                    yield! flatten nested
            }

        let entities = signature.Entities |> Seq.collect flatten |> Seq.toArray

        let bindingOnLine =
            entities
            |> Seq.collect (fun entity ->
                entity.TryGetMembersFunctionsAndValues()
                |> Seq.map (fun m -> struct (entity, m)))
            |> Seq.filter (fun struct (_, m) ->
                sameFile m.DeclarationLocation
                && m.DeclarationLocation.StartLine = position.Line)
            |> Seq.tryHeadV

        match bindingOnLine with
        | ValueSome(struct (entity, m)) ->
            ValueSome
                {
                    Range = m.DeclarationLocation
                    Entity = entity
                    MemberName = ValueSome m.CompiledName
                    Kind = kindOf m
                    Traits = traitsOf m
                    DisplayName = m.DisplayName
                }
        | ValueNone ->
            entities
            |> Seq.filter (fun entity ->
                sameFile entity.DeclarationLocation
                && entity.DeclarationLocation.StartLine <= position.Line)
            |> Seq.sortByDescending (fun entity -> entity.DeclarationLocation.StartLine)
            |> Seq.tryHeadV
            |> ValueOption.map (fun entity ->
                // A private `static let` runs in the type's static constructor; naming that as the
                // enclosing type is honest, and marking it a static ctor gives it the ctor glyph
                // instead of a class one and keeps it distinct from the instance ctor's `.ctor` node.
                {
                    Range = entity.DeclarationLocation
                    Entity = entity
                    MemberName = ValueSome ".cctor"
                    Kind = ResolvedMethod
                    Traits = entityTraits entity |> Set.add Static |> Set.add Constructor
                    DisplayName = entity.DisplayName
                })

    /// A closure has no entity of its own, so it is placed by the declaration that contains it, and
    /// identified by the compiler's own `name@line` - which keeps nested and sibling closures apart.
    let private resolveClosure (frame: ParsedFrame) origin (entity: FSharpEntity) qualifiedMember =
        let line = closureLine frame origin entity

        let enclosing =
            match qualifiedMember with
            | ValueSome name ->
                tryFindMember frame name entity
                |> ValueOption.orElseWith (fun () -> tryFindDeclarationAbove line entity)
            | ValueNone -> tryFindDeclarationAbove line entity

        // A `task` builder's resumption thunk is numbered from line 1 and carries no sequence points,
        // so neither its name nor the debugger places it. It is still the user's binding by name, and
        // leaving it unresolved would put a bare `Invoke` on the map, so it falls back to the
        // declaration that contains it.
        let struct (host, hostName, traits, anchor) =
            match enclosing with
            | ValueSome m -> struct (m.DeclarationLocation, m.DisplayName, traitsOf m |> Set.add Lifted, line)
            | ValueNone ->
                let host = entity.DeclarationLocation

                struct (host, entity.DisplayName, entityTraits entity |> Set.add Lifted, host.StartLine)

        // `outer`, `inner` and `work` name a real binding and read well on their own. A synthesized
        // name - `Pipe #1 input at line 97` for a computation expression body or a pipeline stage -
        // names nothing, and the function the body belongs to may be absent from the stack entirely:
        // an `async` block runs from FSharp.Core, so nothing else on the map says `asyncBody`. Taking
        // the enclosing declaration alone would collide with that function's own node where it *is*
        // on the stack, as a pipeline's is, so the line comes with it.
        let displayName =
            if origin.EnclosingName.IndexOf ' ' >= 0 then
                $"%s{hostName}@%d{anchor}"
            else
                origin.EnclosingName

        ValueSome
            {
                Range = Range.mkRange host.FileName (Position.mkPos anchor 0) (Position.mkPos anchor 0)
                Entity = entity
                MemberName = ValueSome(closureIdentity origin)
                Kind = ResolvedMethod
                Traits = traits
                DisplayName = displayName
            }

    let private resolveMember (frame: ParsedFrame) (entity: FSharpEntity) name =
        tryFindMember frame name entity
        |> ValueOption.map (fun m ->
            {
                Range = m.DeclarationLocation
                Entity = entity
                MemberName = ValueSome m.CompiledName
                Kind = kindOf m
                Traits = traitsOf m
                DisplayName = m.DisplayName
            })

    /// A frame name flattens the declaring path, so the entity it names is found by trying every
    /// split of that path; within the entity it is the frame's own shape that says what to look for.
    let private tryResolveByName (frame: ParsedFrame) (signature: FSharpAssemblySignature) =
        entityPathCandidates frame.Path (memberName frame.Member)
        |> Seq.tryPickV (fun struct (entityPath, qualifiedMember) ->
            match signature.FindEntityByPath(List.ofArray entityPath) with
            | None -> ValueNone
            | Some entity ->
                match frame.Member, qualifiedMember with
                | FrameClosureBody origin, _ -> resolveClosure frame origin entity qualifiedMember
                | _, ValueSome name -> resolveMember frame entity name
                | _, ValueNone -> ValueNone)

    /// Module initialization names the file rather than a construct, so only the debugger's position
    /// places it; everything else is found by name.
    let private tryResolveInProject (frame: ParsedFrame) (signature: FSharpAssemblySignature) =
        match frame.Member with
        | FrameStartupCode ->
            frame.SourcePosition
            |> ValueOption.bind (fun position -> tryResolveByPosition position signature)
        | _ -> tryResolveByName frame signature

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
                                |> ValueOption.map (fun resolution ->
                                    {
                                        DeclarationRange = resolution.Range
                                        Project = project
                                        Namespace = namespaceOf resolution.Entity
                                        TypeChain = typeChainOf resolution.Entity
                                        MemberName = resolution.MemberName
                                        Kind = resolution.Kind
                                        Traits = resolution.Traits
                                        DisplayName = resolution.DisplayName
                                    }))

            return resolved
        }

    let tryResolve (workspace: Workspace) (assemblyName: string) (frame: ParsedFrame) =
        cancellableTask {
            let! resolved = tryResolveMany workspace assemblyName [| frame |]
            return Array.head resolved
        }
