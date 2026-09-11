// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Navigation
open Microsoft.VisualStudio.LanguageServices

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open CancellableTasks

[<RequireQualifiedAccess>]
type internal SymbolMemberType =
    | Event
    | Property
    | Method
    | Constructor
    | Other

type internal SymbolPath =
    {
        EntityPath: string list
        MemberOrValName: string
        GenericParameters: int
    }

[<RequireQualifiedAccess>]
type internal DocCommentId =
    | Member of SymbolPath * SymbolMemberType: SymbolMemberType
    | Field of SymbolPath
    | Type of EntityPath: string list
    | None

/// Where a declaration is, for a feature that shows it in place rather than navigating to it.
[<Struct>]
type internal FileLocation =
    {
        FilePath: string
        /// 0-based, the way Roslyn counts lines.
        Position: Microsoft.CodeAnalysis.Text.LinePosition
    }

type FSharpNavigableLocation(metadataAsSource: FSharpMetadataAsSourceService, symbolRange: range, project: Project) =
    interface IFSharpNavigableLocation with
        member _.NavigateToAsync(_options: FSharpNavigationOptions2, cancellationToken: CancellationToken) : Task<bool> =
            cancellableTask {
                let targetPath = symbolRange.FileName

                let! cancellationToken = CancellableTask.getCancellationToken ()

                let targetDoc =
                    project.Solution.TryGetDocumentFromFSharpRange(symbolRange, project.Id)

                match targetDoc with
                | None -> return false
                | Some targetDoc ->
                    let! targetSource = targetDoc.GetTextAsync(cancellationToken)
                    let gtd = GoToDefinition(metadataAsSource)

                    let (|Signature|Implementation|) filepath =
                        if isSignatureFile filepath then
                            Signature
                        else
                            Implementation

                    match targetPath with
                    | Signature -> return! gtd.NavigateToSymbolDefinitionAsync(targetDoc, targetSource, symbolRange)
                    | Implementation -> return! gtd.NavigateToSymbolDeclarationAsync(targetDoc, targetSource, symbolRange)
            }
            |> CancellableTask.start cancellationToken

/// Locates the F# declaration Roslyn names by assembly and documentation comment id when C# or
/// Visual Basic navigates into an F# project. Kept apart from the MEF service so it can be exercised
/// without a Visual Studio workspace.
module internal CrossLanguageSymbolNavigation =

    [<Literal>]
    let private UserOpName = "CrossLanguageSymbolNavigation"

    let docCommentIdToPath (docId: string) =
        match XmlDocSigParser.parseDocCommentId docId with
        | ParsedDocCommentId.Type path -> DocCommentId.Type path

        | ParsedDocCommentId.Member(typePath, memberName, genericArity, kind) ->
            // The parser reports constructors as .ctor; the F# lookup needs the backticked form.
            let memberOrValName = if memberName = ".ctor" then "``.ctor``" else memberName

            let symbolMemberType =
                match kind with
                | DocCommentIdKind.Method ->
                    if memberName = ".ctor" then
                        SymbolMemberType.Constructor
                    else
                        SymbolMemberType.Method
                | DocCommentIdKind.Property -> SymbolMemberType.Property
                | DocCommentIdKind.Event -> SymbolMemberType.Event
                | _ -> SymbolMemberType.Other

            DocCommentId.Member(
                {
                    EntityPath = typePath
                    MemberOrValName = memberOrValName
                    GenericParameters = genericArity
                },
                symbolMemberType
            )

        | ParsedDocCommentId.Field(typePath, fieldName) ->
            DocCommentId.Field
                {
                    EntityPath = typePath
                    MemberOrValName = fieldName
                    GenericParameters = 0
                }

        | ParsedDocCommentId.None -> DocCommentId.None

    /// The fields of the entity, and the literals of a module, which compile to fields.
    let private tryFindFieldByName (name: string) (e: FSharpEntity) =
        let fields =
            e.FSharpFields
            |> Seq.filter (fun x -> x.DisplayName = name && not x.IsCompilerGenerated)
            |> Seq.map _.DeclarationLocation

        let literals =
            e.TryGetMembersFunctionsAndValues()
            |> Seq.filter (fun v -> v.LiteralValue.IsSome && (v.CompiledName = name || v.DisplayName = name))
            |> Seq.map _.DeclarationLocation

        if
            Seq.isEmpty fields
            && Seq.isEmpty literals
            && (e.IsFSharpUnion || e.IsFSharpRecord)
        then
            Seq.singleton e.DeclarationLocation
        else
            Seq.append fields literals

    let private tryFindValByNameAndType
        (name: string)
        (symbolMemberType: SymbolMemberType)
        (genericParametersCount: int)
        (e: FSharpEntity)
        (entities: FSharpMemberOrFunctionOrValue seq)
        =

        let defaultFilter (e: FSharpMemberOrFunctionOrValue) =
            (e.DisplayName = name || e.CompiledName = name)
            && e.GenericParameters.Count = genericParametersCount

        let isProperty (e: FSharpMemberOrFunctionOrValue) = defaultFilter e && e.IsProperty
        let isConstructor (e: FSharpMemberOrFunctionOrValue) = defaultFilter e && e.IsConstructor

        let getLocation (e: FSharpMemberOrFunctionOrValue) = e.DeclarationLocation

        let filteredEntities: range seq =
            match symbolMemberType with
            | SymbolMemberType.Other
            | SymbolMemberType.Method -> entities |> Seq.filter defaultFilter |> Seq.map getLocation
            // F# record-specific logic, if navigating to the record's ctor, then navigate to record declaration.
            // If we navigating to F# record property, we first check if it's "custom" property, if it's one of the record fields, we search for it in the fields.
            | SymbolMemberType.Constructor when e.IsFSharpRecord -> Seq.singleton e.DeclarationLocation
            | SymbolMemberType.Property when e.IsFSharpRecord ->
                let properties = entities |> Seq.filter isProperty |> Seq.map getLocation
                let fields = tryFindFieldByName name e
                Seq.append properties fields
            | SymbolMemberType.Constructor -> entities |> Seq.filter isConstructor |> Seq.map getLocation
            // When navigating to property for the record, it will be in members bag for custom ones, but will be in the fields in fields.
            | SymbolMemberType.Event // Events are just properties`
            | SymbolMemberType.Property -> entities |> Seq.filter isProperty |> Seq.map getLocation

        filteredEntities

    /// The union case behind its compiled members: the `NewCase` factory, the `IsCase` tester and
    /// the `Case` property of a nullary case.
    let private unionCaseLocations (name: string) (entity: FSharpEntity) =
        if entity.IsFSharpUnion then
            entity.UnionCases
            |> Seq.filter (fun unionCase ->
                let compiled = unionCase.CompiledName
                name = compiled || name = $"New{compiled}" || name = $"Is{compiled}")
            |> Seq.map _.DeclarationLocation
        else
            Seq.empty

    /// The members of the entity the id names: those whose compiled id matches exactly and, when
    /// `byShape`, those whose name, kind and arity fit when no id matched.
    let private memberLocations
        (byShape: bool)
        (documentationCommentId: string)
        (symbolPath: SymbolPath)
        (memberType: SymbolMemberType)
        (entity: FSharpEntity)
        =
        let members = entity.TryGetMembersFunctionsAndValues()

        let exact =
            seq {
                yield!
                    members
                    |> Seq.filter (fun m -> m.XmlDocSig = documentationCommentId)
                    |> Seq.map _.DeclarationLocation

                yield! unionCaseLocations symbolPath.MemberOrValName entity
            }

        if byShape && Seq.isEmpty exact then
            tryFindValByNameAndType symbolPath.MemberOrValName memberType symbolPath.GenericParameters entity members
        else
            exact

    let private declarationsIn (byShape: bool) (signature: FSharpAssemblySignature) (documentationCommentId: string) (path: DocCommentId) =
        let inEntity entityPath (locationsOf: FSharpEntity -> range seq) =
            signature.FindEntityByPath entityPath
            |> Option.map locationsOf
            |> Option.defaultValue Seq.empty

        match path with
        | DocCommentId.Member(symbolPath, memberType) ->
            inEntity symbolPath.EntityPath (memberLocations byShape documentationCommentId symbolPath memberType)
        | DocCommentId.Field symbolPath -> inEntity symbolPath.EntityPath (tryFindFieldByName symbolPath.MemberOrValName)
        | DocCommentId.Type entityPath -> inEntity entityPath (fun entity -> Seq.singleton entity.DeclarationLocation)
        | DocCommentId.None -> Seq.empty

    let private entityPathOf (path: DocCommentId) =
        match path with
        | DocCommentId.Member(symbolPath, _)
        | DocCommentId.Field symbolPath -> symbolPath.EntityPath
        | DocCommentId.Type entityPath -> entityPath
        | DocCommentId.None -> []

    /// A compiled segment of a doc id against a source segment: the generic arity suffix and the
    /// `Module` suffix of `CompilationRepresentation(ModuleSuffix)` exist only in compiled names.
    let private segmentMatches (compiled: ReadOnlySpan<char>) (source: ReadOnlySpan<char>) =
        let compiled =
            match compiled.IndexOf '`' with
            | -1 -> compiled
            | arity -> compiled.Slice(0, arity)

        compiled.Equals(source, StringComparison.Ordinal)
        || (compiled.Length = source.Length + "Module".Length
            && compiled.Slice(0, source.Length).Equals(source, StringComparison.Ordinal)
            && compiled.Slice(source.Length).Equals("Module".AsSpan(), StringComparison.Ordinal))

    let rec private pathMatches (entityPath: string list) (source: ReadOnlySpan<char>) =
        match entityPath with
        | [] -> source.IsEmpty
        | [ last ] -> source.IndexOf '.' = -1 && segmentMatches (last.AsSpan()) source
        | segment :: rest ->
            match source.IndexOf '.' with
            | -1 -> false
            | dot ->
                segmentMatches (segment.AsSpan()) (source.Slice(0, dot))
                && pathMatches rest (source.Slice(dot + 1))

    /// Whether the parsed item declares the entity the doc id names.
    let declaresEntity (entityPath: string list) (item: NavigableItem) =
        match item.Kind with
        | NavigableItemKind.Module
        | NavigableItemKind.Type
        | NavigableItemKind.Exception ->
            match item.Container.FullName with
            | "" -> pathMatches entityPath (item.Name.AsSpan())
            | container -> pathMatches entityPath ($"{container}.{item.Name}".AsSpan())
        | _ -> false

    /// The project's documents whose parse tree declares the entity, in compile order.
    let candidateDocuments (entityPath: string list) (project: Project) =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            let! _, _, _, options = project.GetFSharpCompilationOptionsAsync()

            let compileOrder = Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)

            options.SourceFiles
            |> Array.iteri (fun index path -> compileOrder[path] <- index)

            let declaresIn (document: Document) =
                cancellableTask {
                    ct.ThrowIfCancellationRequested()
                    let! parseResults = document.GetFSharpParseResultsAsync UserOpName

                    if
                        NavigateTo.GetNavigableItems parseResults.ParseTree
                        |> Array.exists (declaresEntity entityPath)
                    then
                        return ValueSome document
                    else
                        return ValueNone
                }

            let! candidates =
                project.Documents
                |> Seq.filter (fun document -> isFSharpSourceFile document.FilePath)
                |> Seq.map declaresIn
                // Throttle to avoid launching a parse per document in the project all at once.
                |> CancellableTask.whenAllThrottled (max 1 Environment.ProcessorCount)

            return
                candidates
                |> Array.chooseV id
                |> Array.sortBy (fun document ->
                    match compileOrder.TryGetValue document.FilePath with
                    | true, index -> index
                    | _ -> Int32.MaxValue)
                |> List.ofArray
        }

    let private tryLocateInDocument (byShape: bool) (documentationCommentId: string) (path: DocCommentId) (document: Document) =
        cancellableTask {
            let! _, checkResults = document.GetFSharpParseAndCheckResultsAsync UserOpName

            return
                declarationsIn byShape checkResults.PartialAssemblySignature documentationCommentId path
                |> Seq.tryHeadV
        }

    /// Checks only the documents that declare the entity. An exact id match in any of them wins;
    /// the name-and-shape heuristics run only on the last one, whose partial signature holds every
    /// member the entity gets from the files that declare it.
    let tryLocateViaNavigableItems (documentationCommentId: string) (path: DocCommentId) (project: Project) =
        cancellableTask {
            let! candidates = candidateDocuments (entityPathOf path) project

            match!
                candidates
                |> CancellableTask.tryPick (tryLocateInDocument false documentationCommentId path)
            with
            | ValueSome range -> return ValueSome range
            | ValueNone ->
                match List.tryLast candidates with
                | Some last -> return! tryLocateInDocument true documentationCommentId path last
                | None -> return ValueNone
        }

    /// Checks the whole project.
    let tryLocateInProject (documentationCommentId: string) (path: DocCommentId) (project: Project) =
        cancellableTask {
            let! checker, _, _, options = project.GetFSharpCompilationOptionsAsync()
            let! result = checker.ParseAndCheckProject(options)

            return
                declarationsIn true result.AssemblySignature documentationCommentId path
                |> Seq.tryHeadV
        }

    /// The declaration's range and the project holding it, for the assembly name and doc id Roslyn passes.
    let tryFindDeclaration (solution: Solution) (assemblyName: string) (documentationCommentId: string) =
        match docCommentIdToPath documentationCommentId with
        | DocCommentId.None -> CancellableTask.singleton ValueNone
        | path ->
            cancellableTask {
                // The target frameworks of one project declare the same entities in the same files apart
                // from conditional compilation, so one instance per project file goes first.
                let instances =
                    solution.Projects
                    |> Seq.filter (fun p -> p.IsFSharp && p.AssemblyName = assemblyName)
                    |> Seq.groupBy _.FilePath
                    |> Seq.map (snd >> List.ofSeq)
                    |> List.ofSeq

                let ordered =
                    [
                        for instance in instances -> instance.Head
                        for instance in instances do
                            yield! instance.Tail
                    ]

                let located (locate: Project -> CancellableTask<range voption>) (project: Project) =
                    locate project
                    |> CancellableTask.map (ValueOption.map (fun range -> struct (range, project)))

                match!
                    ordered
                    |> CancellableTask.tryPick (located (tryLocateViaNavigableItems documentationCommentId path))
                with
                | ValueSome found -> return ValueSome found
                | ValueNone ->
                    return!
                        ordered
                        |> CancellableTask.tryPick (located (tryLocateInProject documentationCommentId path))
            }

    /// The file and position of the declaration, for a feature that shows it in place rather than navigating to it.
    /// Peek opens the path it is handed, and a range names its file the way the compiler recorded it - relative,
    /// under a path map - so the path handed back is the document's.
    let tryFindFileLocation (solution: Solution) (assemblyName: string) (documentationCommentId: string) =
        cancellableTask {
            match! tryFindDeclaration solution assemblyName documentationCommentId with
            | ValueNone -> return ValueNone
            | ValueSome(struct (range, project)) ->
                match solution.TryGetDocumentFromFSharpRange(range, project.Id) with
                | None -> return ValueNone
                | Some document ->
                    return
                        ValueSome
                            {
                                FilePath = document.FilePath
                                // FCS lines are 1-based; columns are 0-based in both.
                                Position = Microsoft.CodeAnalysis.Text.LinePosition(range.StartLine - 1, range.StartColumn)
                            }
        }

    /// The file location in the shape the Roslyn contract carries it.
    let toContract (location: FileLocation voption) : Nullable<struct (string * Microsoft.CodeAnalysis.Text.LinePosition)> =
        match location with
        | ValueSome location -> Nullable(struct (location.FilePath, location.Position))
        | ValueNone -> Nullable()

[<Export(typeof<IFSharpCrossLanguageSymbolNavigationService>)>]
[<Export(typeof<FSharpCrossLanguageSymbolNavigationService>)>]
type internal FSharpCrossLanguageSymbolNavigationService
    [<ImportingConstructor>]
    (metadataAsSource: FSharpMetadataAsSourceService, [<Import(AllowDefault = true)>] workspace: VisualStudioWorkspace) =

    static member internal DocCommentIdToPath(docId: string) =
        CrossLanguageSymbolNavigation.docCommentIdToPath docId

    interface IFSharpCrossLanguageSymbolNavigationService with
        member _.TryGetNavigableLocationAsync
            (assemblyName: string, documentationCommentId: string, cancellationToken: CancellationToken)
            : Task<IFSharpNavigableLocation> =
            cancellableTask {
                match workspace with
                | null -> return null
                | workspace ->
                    match!
                        CrossLanguageSymbolNavigation.tryFindDeclaration workspace.CurrentSolution assemblyName documentationCommentId
                    with
                    | ValueSome(struct (range, project)) ->
                        return FSharpNavigableLocation(metadataAsSource, range, project) :> IFSharpNavigableLocation
                    | ValueNone ->
                        // Roslyn falls back to its own metadata-as-source when no location comes back.
                        return null
            }
            |> CancellableTask.start cancellationToken

    interface IFSharpCrossLanguageSymbolNavigationService2 with
        member _.TryGetNavigableFileLocationAsync
            (assemblyName: string, documentationCommentId: string, cancellationToken: CancellationToken)
            : Task<Nullable<struct (string * Microsoft.CodeAnalysis.Text.LinePosition)>> =
            cancellableTask {
                match workspace with
                | null -> return CrossLanguageSymbolNavigation.toContract ValueNone
                | workspace ->
                    let! location =
                        CrossLanguageSymbolNavigation.tryFindFileLocation workspace.CurrentSolution assemblyName documentationCommentId

                    return CrossLanguageSymbolNavigation.toContract location
            }
            |> CancellableTask.start cancellationToken
