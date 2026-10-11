// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic

open Microsoft.CodeAnalysis

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open CancellableTasks

/// What kind of node the resolved frame should become on the map.
type internal ResolvedFrameKind =
    | ResolvedMethod
    | ResolvedProperty
    | ResolvedEvent

/// Something true of a frame that the Properties pane shows and the map's styles can filter on.
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

    /// `FindEntityByPath` looks entities up by their mangled names, so a generic segment must keep
    /// its arity suffix (`Box`1`), which the frame parser split off.
    let private mangledName (segment: FramePathSegment) =
        if segment.GenericArity = 0 then
            segment.Name
        else
            $"{segment.Name}`{segment.GenericArity}"

    /// A frame name flattens namespaces, modules and nested types into one dotted path, and an
    /// explicit interface implementation puts the interface name inside the member name. Both are
    /// undone by trying every split of the path, longest entity path first.
    let private entityPathCandidates (path: FramePathSegment array) (memberName: string voption) =
        seq {
            for split in path.Length .. -1 .. 1 do
                let entityNames = path |> Seq.truncate split |> Seq.map mangledName |> Seq.toArray

                let memberNames =
                    [|
                        yield! path |> Seq.skip split |> Seq.map _.Name
                        yield! ValueOption.toArray memberName
                    |]

                let qualifiedMember =
                    if memberNames.Length = 0 then
                        ValueNone
                    else
                        ValueSome(String.Join(".", memberNames))

                yield struct (entityNames, qualifiedMember)

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
        // The signature carries an `[<CLIEvent>]` twice, as the event and as the getter behind it,
        // and both answer to the same compiled name. The event is the one that says what it is.
        |> Seq.sortByDescending _.IsEvent
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
    /// the line the debugger reports for the frame is used.
    let private closureLine (frame: ParsedFrame) (origin: ClosureOrigin) (entity: FSharpEntity) =
        match tryFindDeclarationAbove origin.Line entity, frame.SourcePosition with
        | ValueNone, ValueSome position -> position.Line
        | _ -> origin.Line

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

    /// An `[<CLIEvent>]` is a property as well - the getter behind it is how the event is read - so
    /// the event has to be asked about first or every event answers as a property.
    let private kindOf (m: FSharpMemberOrFunctionOrValue) =
        if m.IsEvent || m.IsEventAddMethod || m.IsEventRemoveMethod then
            ResolvedEvent
        elif m.IsProperty || m.IsPropertyGetterMethod || m.IsPropertySetterMethod then
            ResolvedProperty
        else
            ResolvedMethod

    let private traitsWhere (traits: (bool * FrameTrait) list) =
        set
            [
                for holds, frameTrait in traits do
                    if holds then
                        frameTrait
            ]

    let private traitsOf (m: FSharpMemberOrFunctionOrValue) =
        let isDeclaredIn (kind: FSharpEntity -> bool) = m.DeclaringEntity |> Option.exists kind

        let isInline =
            match m.InlineAnnotation with
            | FSharpInlineAnnotation.AlwaysInline
            | FSharpInlineAnnotation.AggressiveInline -> true
            | _ -> false

        traitsWhere
            [
                m.Accessibility.IsPublic, Public
                m.Accessibility.IsInternal, Internal
                m.Accessibility.IsPrivate, Private
                // A module-level function compiles to a static method though F# has no `static`
                // keyword for it, so instance membership is the test.
                not m.IsInstanceMember, Static
                m.GenericParameters.Count > 0, Generic
                m.IsConstructor, Constructor
                PrettyNaming.IsLogicalOpName m.CompiledName, Operator
                m.IsExtensionMember, Extension
                m.IsDispatchSlot, Abstract
                m.IsCompilerGenerated, CompilerGenerated
                m.IsPropertyGetterMethod, PropertyGet
                m.IsPropertySetterMethod, PropertySet
                isDeclaredIn _.IsFSharpUnion, Union
                isDeclaredIn _.IsFSharpRecord, Record
                isDeclaredIn _.IsFSharpExceptionDeclaration, Exception
                m.IsActivePattern, ActivePattern
                m.IsUnionCaseTester, UnionCaseTester
                m.IsFunction, Function
                isInline, Inline
                m.IsMutable, Mutable
            ]

    let private entityTraits (entity: FSharpEntity) =
        traitsWhere
            [
                entity.Accessibility.IsPublic, Public
                entity.Accessibility.IsInternal, Internal
                entity.Accessibility.IsPrivate, Private
                entity.IsFSharpModule, Static
                entity.GenericParameters.Count > 0, Generic
                entity.IsFSharpModule, Module
                entity.IsFSharpUnion, Union
                entity.IsFSharpRecord, Record
                entity.IsFSharpExceptionDeclaration, Exception
                entity.IsMeasure, Measure
            ]

    let private memberResolution (entity: FSharpEntity) (m: FSharpMemberOrFunctionOrValue) =
        {
            Range = m.DeclarationLocation
            Entity = entity
            MemberName = ValueSome m.CompiledName
            Kind = kindOf m
            Traits = traitsOf m
            DisplayName = m.DisplayName
        }

    /// The range of the type or module declaration that owns a source line. An entity's own range
    /// covers nothing but its name, so the parse tree answers: the innermost declaration containing
    /// the line, and for the trivia between two declarations, contained by none, the one below it.
    let private declarationOwning (shape: NavigationItems) line =
        let declarations =
            shape.Declarations
            |> Array.map _.Declaration
            // A namespace is no entity of its own and its range spans the whole file.
            |> Array.filter (fun d -> d.Kind <> NavigationItemKind.Namespace)

        let containing =
            declarations
            |> Array.filter (fun d -> d.Range.StartLine <= line && line <= d.Range.EndLine)
            // The innermost one: a module's range spans every type written inside it.
            |> Array.sortByDescending _.Range.StartLine
            |> Array.tryHeadV

        let below () =
            declarations
            |> Array.filter (fun d -> d.Range.StartLine > line)
            |> Array.sortBy _.Range.StartLine
            |> Array.tryHeadV

        containing |> ValueOption.orElseWith below |> ValueOption.map _.Range

    /// A `<StartupCode$…>.$Demo.$Demo` frame names the file, not the construct, so it is resolved by
    /// the line the debugger reports: the public binding written on it, else the type or module
    /// whose initializer owns it - a private `static let` is absent from the signature.
    let private tryResolveByPosition (position: SourcePosition) (declaration: range) (signature: FSharpAssemblySignature) =
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
            |> Seq.tryFindV (fun struct (_, m) ->
                sameFile m.DeclarationLocation
                && m.DeclarationLocation.StartLine = position.Line)

        match bindingOnLine with
        | ValueSome(struct (entity, m)) -> ValueSome(memberResolution entity m)
        | ValueNone ->
            // The declaration's own entity: the outermost one written inside its range, since
            // anything nested starts further down.
            entities
            |> Seq.filter (fun entity ->
                sameFile entity.DeclarationLocation
                && declaration.StartLine <= entity.DeclarationLocation.StartLine
                && entity.DeclarationLocation.StartLine <= declaration.EndLine)
            |> Seq.sortBy _.DeclarationLocation.StartLine
            |> Seq.tryHeadV
            |> ValueOption.map (fun entity ->
                // A private `static let` runs in the type's static constructor, which also gives the
                // node a constructor glyph and keeps it apart from the instance constructor's `.ctor`.
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
            qualifiedMember
            |> ValueOption.bind (fun name -> tryFindMember frame name entity)
            |> ValueOption.orElseWith (fun () -> tryFindDeclarationAbove line entity)

        // A `task` builder's resumption thunk is numbered from line 1 and carries no sequence points,
        // so neither its name nor the debugger places it: it falls back to the declaring entity
        // rather than reach the map as a bare `Invoke`.
        let struct (host, hostName, traits, anchor) =
            match enclosing with
            | ValueSome m -> struct (m.DeclarationLocation, m.DisplayName, traitsOf m |> Set.add Lifted, line)
            | ValueNone ->
                let host = entity.DeclarationLocation

                struct (host, entity.DisplayName, entityTraits entity |> Set.add Lifted, host.StartLine)

        // A synthesized name - `Pipe #1 input at line 97` for a computation expression body or a
        // pipeline stage - names nothing, and the function it belongs to may not be on the stack:
        // an `async` block runs from FSharp.Core. The line keeps it apart from that function's own
        // node where it is on the stack, as a pipeline's is.
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

    /// Within the entity it is the frame's own shape that says what to look for.
    let private tryResolveByName (frame: ParsedFrame) (signature: FSharpAssemblySignature) =
        entityPathCandidates frame.Path (memberName frame.Member)
        |> Seq.tryPickV (fun struct (entityPath, qualifiedMember) ->
            match signature.FindEntityByPath(List.ofArray entityPath) with
            | None -> ValueNone
            | Some entity ->
                match frame.Member, qualifiedMember with
                | FrameClosureBody origin, _ -> resolveClosure frame origin entity qualifiedMember
                | _, ValueSome name -> tryFindMember frame name entity |> ValueOption.map (memberResolution entity)
                | _, ValueNone -> ValueNone)

    /// Module initialization names the file rather than a construct, so only the debugger's position
    /// places it; everything else is found by name.
    let private tryResolveInProject (frame: ParsedFrame) shapeOf (signature: FSharpAssemblySignature) =
        match frame.Member with
        | FrameStartupCode ->
            frame.SourcePosition
            |> ValueOption.bind (fun position ->
                shapeOf position.File
                |> ValueOption.bind (fun shape -> declarationOwning shape position.Line)
                |> ValueOption.bind (fun declaration -> tryResolveByPosition position declaration signature))
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

                    // Only a startup frame is placed by its line, and only that needs the shape of
                    // the file it landed in.
                    let shapes = Dictionary<string, NavigationItems>(StringComparer.OrdinalIgnoreCase)

                    let startupFiles =
                        frames
                        |> Seq.chooseV (fun frame ->
                            match frame.Member, frame.SourcePosition with
                            | FrameStartupCode, ValueSome position -> ValueSome position.File
                            | _ -> ValueNone)
                        |> Seq.distinct

                    for file in startupFiles do
                        match
                            project.Documents
                            |> Seq.tryFindV (fun d -> String.Equals(d.FilePath, file, StringComparison.OrdinalIgnoreCase))
                        with
                        | ValueNone -> ()
                        | ValueSome document ->
                            let! parseResults = document.GetFSharpParseResultsAsync "FSharpCallStackResolver"
                            shapes.[file] <- Navigation.getNavigation parseResults.ParseTree

                    let shapeOf file =
                        match shapes.TryGetValue file with
                        | true, shape -> ValueSome shape
                        | _ -> ValueNone

                    frames
                    |> Array.iteri (fun i frame ->
                        if resolved.[i].IsNone then
                            resolved.[i] <-
                                tryResolveInProject frame shapeOf results.AssemblySignature
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
