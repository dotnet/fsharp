// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Linq
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Navigation
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.LanguageServices

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

[<Export(typeof<IFSharpCrossLanguageSymbolNavigationService>)>]
[<Export(typeof<FSharpCrossLanguageSymbolNavigationService>)>]
type FSharpCrossLanguageSymbolNavigationService() =
    let componentModel =
        Package.GetGlobalService(typeof<ComponentModelHost.SComponentModel>) :?> ComponentModelHost.IComponentModel

    let workspace = componentModel.GetService<VisualStudioWorkspace>()

    let metadataAsSource =
        componentModel.DefaultExportProvider.GetExport<FSharpMetadataAsSourceService>().Value

    let tryFindFieldByName (name: string) (e: FSharpEntity) =
        let fields =
            e.FSharpFields
            |> Seq.filter (fun x -> x.DisplayName = name && not x.IsCompilerGenerated)
            |> Seq.map (fun e -> e.DeclarationLocation)

        if fields.Count() <= 0 && (e.IsFSharpUnion || e.IsFSharpRecord) then
            Seq.singleton e.DeclarationLocation
        else
            fields

    let tryFindValByNameAndType
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

    let tryFindVal
        (name: string)
        (documentCommentId: string)
        (symbolMemberType: SymbolMemberType)
        (genericParametersCount: int)
        (e: FSharpEntity)
        =
        let entities = e.TryGetMembersFunctionsAndValues()

        // First, try and find entity by exact xml signature, return if found,
        // otherwise, just try and match by parsed name and number of arguments.

        let entitiesByXmlSig =
            entities
            |> Seq.filter (fun e -> e.XmlDocSig = documentCommentId)
            |> Seq.map (fun e -> e.DeclarationLocation)

        if Seq.isEmpty entitiesByXmlSig then
            tryFindValByNameAndType name symbolMemberType genericParametersCount e entities
        else
            entitiesByXmlSig

    /// Convert a documentation comment ID to a navigation path.
    /// Uses the shared XmlDocSigParser from FSharp.Compiler.Symbols.
    static member internal DocCommentIdToPath(docId: string) =
        // Use the shared parser from FSharp.Compiler.Symbols
        match XmlDocSigParser.parseDocCommentId docId with
        | ParsedDocCommentId.Type path -> DocCommentId.Type path

        | ParsedDocCommentId.Member(typePath, memberName, genericArity, kind) ->
            // Convert constructor name format (.ctor in parser, ``.ctor`` needed for F# lookup)
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

    interface IFSharpCrossLanguageSymbolNavigationService with
        member _.TryGetNavigableLocationAsync
            (assemblyName: string, documentationCommentId: string, cancellationToken: CancellationToken)
            : Task<IFSharpNavigableLocation> =
            let path =
                FSharpCrossLanguageSymbolNavigationService.DocCommentIdToPath documentationCommentId

            cancellableTask {
                let projects =
                    workspace.CurrentSolution.Projects
                    |> Seq.filter (fun p -> p.IsFSharp && p.AssemblyName = assemblyName)

                let mutable locations = Seq.empty

                for project in projects do
                    let! checker, _, _, options = project.GetFSharpCompilationOptionsAsync()
                    let! result = checker.ParseAndCheckProject(options)

                    match path with
                    | DocCommentId.Member({
                                              EntityPath = entityPath
                                              MemberOrValName = memberOrVal
                                              GenericParameters = genericParametersCount
                                          },
                                          memberType) ->
                        let entity = result.AssemblySignature.FindEntityByPath(entityPath)

                        entity
                        |> Option.iter (fun e ->
                            locations <-
                                e
                                |> tryFindVal memberOrVal documentationCommentId memberType genericParametersCount
                                |> Seq.map (fun m -> (m, project))
                                |> Seq.append locations)
                    | DocCommentId.Field {
                                             EntityPath = entityPath
                                             MemberOrValName = memberOrVal
                                         } ->
                        let entity = result.AssemblySignature.FindEntityByPath(entityPath)

                        entity
                        |> Option.iter (fun e ->
                            locations <-
                                e
                                |> tryFindFieldByName memberOrVal
                                |> Seq.map (fun m -> (m, project))
                                |> Seq.append locations)
                    | DocCommentId.Type entityPath ->
                        let entity = result.AssemblySignature.FindEntityByPath(entityPath)

                        entity
                        |> Option.iter (fun e -> locations <- Seq.append locations [ e.DeclarationLocation, project ])
                    | DocCommentId.None -> ()

                // TODO: Figure out the way of giving the user choice where to navigate, if there are more than one result
                // For now, we only take 1st one, since it's usually going to be only one result (given we process names correctly).
                // More results can theoretically be returned in case of method overloads, or when we have both signature and implementation files.
                if locations.Count() >= 1 then
                    let (location, project) = locations.First()
                    return FSharpNavigableLocation(metadataAsSource, location, project) :> IFSharpNavigableLocation
                else
                    return Unchecked.defaultof<_> // returning null here, so Roslyn can fallback to default source-as-metadata implementation.
            }
            |> CancellableTask.start cancellationToken
