﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests.Helpers

open System
open System.IO
open System.Reflection
open System.Linq
open System.Collections.Generic
open System.Collections.Immutable
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.Composition
open Microsoft.CodeAnalysis.Host
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.CodeAnalysis.Host.Mef
open FSharp.Compiler.CodeAnalysis
open FSharp.Test.ProjectGeneration

[<AutoOpen>]
module MefHelpers =

    let getAssemblies () =
        let self = Assembly.GetExecutingAssembly()
        let here = AppContext.BaseDirectory

        let imports =
            [|
                "Microsoft.CodeAnalysis.Workspaces.dll"
                "Microsoft.VisualStudio.Shell.15.0.dll"
                "Microsoft.VisualStudio.Platform.VSEditor.dll"
                "FSharp.Editor.dll"
            |]

        let resolvedImports = imports.Select(fun name -> Path.Combine(here, name)).ToList()

        let missingDlls =
            resolvedImports.Where(fun path -> not (File.Exists(path))).ToList()

        if (missingDlls.Any()) then
            failwith "Missing imports"

        let loadedImports = resolvedImports.Select(fun p -> Assembly.LoadFrom(p)).ToList()

        let result =
            loadedImports.ToDictionary(fun k -> Path.GetFileNameWithoutExtension(k.Location))

        result.Values
        |> Seq.append [| self |]
        |> Seq.append MefHostServices.DefaultAssemblies
        |> Array.ofSeq

    let exportProviderFactory =
        let resolver = Resolver.DefaultInstance

        let catalog =
            let asms = getAssemblies ()

            let partDiscovery =
                PartDiscovery.Combine(
                    new AttributedPartDiscoveryV1(resolver),
                    new AttributedPartDiscovery(resolver, isNonPublicSupported = true)
                )

            let parts = partDiscovery.CreatePartsAsync(asms).Result
            ComposableCatalog.Create(resolver).AddParts(parts).WithCompositionService()

        let configuration = CompositionConfiguration.Create(catalog)

        RuntimeComposition
            .CreateRuntimeComposition(configuration)
            .CreateExportProviderFactory()

    let createExportProvider () =
        exportProviderFactory.CreateExportProvider()

type TestWorkspaceServiceMetadata(serviceType: string, layer: string) =

    member _.ServiceType = serviceType
    member _.Layer = layer

    new(data: IDictionary<string, obj>) =
        let serviceType =
            match data.TryGetValue("ServiceType") with
            | true, result -> result :?> string
            | _ -> Unchecked.defaultof<_>

        let layer =
            match data.TryGetValue("Layer") with
            | true, result -> result :?> string
            | _ -> Unchecked.defaultof<_>

        TestWorkspaceServiceMetadata(serviceType, layer)

    new(serviceType: Type, layer: string) = TestWorkspaceServiceMetadata(serviceType.AssemblyQualifiedName, layer)

type TestLanguageServiceMetadata(language: string, serviceType: string, layer: string, data: IDictionary<string, obj>) =

    member _.Language = language
    member _.ServiceType = serviceType
    member _.Layer = layer
    member _.Data = data

    new(data: IDictionary<string, obj>) =
        let language =
            match data.TryGetValue("Language") with
            | true, result -> result :?> string
            | _ -> Unchecked.defaultof<_>

        let serviceType =
            match data.TryGetValue("ServiceType") with
            | true, result -> result :?> string
            | _ -> Unchecked.defaultof<_>

        let layer =
            match data.TryGetValue("Layer") with
            | true, result -> result :?> string
            | _ -> Unchecked.defaultof<_>

        TestLanguageServiceMetadata(language, serviceType, layer, data)

type TestHostLanguageServices(workspaceServices: HostWorkspaceServices, language: string, exportProvider: ExportProvider) as this =
    inherit HostLanguageServices()

    let services1 =
        exportProvider.GetExports<ILanguageService, TestLanguageServiceMetadata>()
        |> Seq.filter (fun x -> x.Metadata.Language = language)

    let factories1 =
        exportProvider.GetExports<ILanguageServiceFactory, TestLanguageServiceMetadata>()
        |> Seq.filter (fun x -> x.Metadata.Language = language)
        |> Seq.map (fun x -> Lazy<_, _>((fun () -> x.Value.CreateLanguageService(this)), x.Metadata))

    let otherServices1 = Seq.append factories1 services1

    let otherServicesMap1 =
        otherServices1
        |> Seq.map (fun x -> KeyValuePair(x.Metadata.ServiceType, x))
        |> Seq.distinctBy (fun x -> x.Key)
        |> System.Collections.Concurrent.ConcurrentDictionary

    override this.WorkspaceServices = workspaceServices

    override this.Language = language

    override this.GetService<'T when 'T :> ILanguageService>() : 'T =
        match otherServicesMap1.TryGetValue(typeof<'T>.AssemblyQualifiedName) with
        | true, otherService -> otherService.Value :?> 'T
        | _ ->
            try
                exportProvider.GetExport<'T>().Value
            with _ ->
                Unchecked.defaultof<'T>

type TestHostWorkspaceServices(hostServices: HostServices, workspace: Workspace) as this =
    inherit HostWorkspaceServices()

    let exportProvider = createExportProvider ()

    let services1 =
        exportProvider.GetExports<IWorkspaceService, TestWorkspaceServiceMetadata>()

    let factories1 =
        exportProvider.GetExports<IWorkspaceServiceFactory, TestWorkspaceServiceMetadata>()
        |> Seq.map (fun x -> Lazy<_, _>((fun () -> x.Value.CreateService(this)), x.Metadata))

    let otherServices1 = Seq.append factories1 services1

    let otherServicesMap1 =
        otherServices1
        |> Seq.map (fun x -> KeyValuePair(x.Metadata.ServiceType, x))
        |> Seq.distinctBy (fun x -> x.Key)
        |> System.Collections.Concurrent.ConcurrentDictionary

    let langServices =
        TestHostLanguageServices(this, LanguageNames.FSharp, exportProvider)

    override _.Workspace = workspace

    override this.GetService<'T when 'T :> IWorkspaceService>() : 'T =
        let ty = typeof<'T>

        match otherServicesMap1.TryGetValue(ty.AssemblyQualifiedName) with
        | true, otherService -> otherService.Value :?> 'T
        | _ ->
            try
                exportProvider.GetExport<'T>().Value
            with _ ->
                Unchecked.defaultof<'T>

    override _.FindLanguageServices(filter) = Seq.empty

    override _.GetLanguageServices(languageName) =
        match languageName with
        | LanguageNames.FSharp -> langServices :> HostLanguageServices
        | _ -> raise (NotSupportedException(sprintf "Language '%s' not supported in FSharp VS tests." languageName))

    override _.HostServices = hostServices

type TestHostServices() =
    inherit HostServices()

    override this.CreateWorkspaceServices(workspace) =
        TestHostWorkspaceServices(this, workspace)

[<AbstractClass; Sealed>]
type RoslynTestHelpers private () =

    static member DefaultProjectOptions: FSharpProjectOptions =
        {
            ProjectFileName = "C:\\test.fsproj"
            ProjectId = None
            SourceFiles = [| "C:\\test.fs" |]
            ReferencedProjects = [||]
            OtherOptions = [||]
            IsIncompleteTypeCheckEnvironment = true
            UseScriptResolutionRules = false
            LoadTime = DateTime.MaxValue
            UnresolvedReferences = None
            OriginalLoadReferences = []
            Stamp = None
        }

    static member private GetSourceCodeKind filePath =
        let extension = Path.GetExtension(filePath)

        match extension with
        | ".fsx" -> SourceCodeKind.Script
        | ".fsi" -> SourceCodeKind.Regular
        | ".fs" -> SourceCodeKind.Regular
        | _ -> failwith "not supported"

    static member CreateSolution projects =
        let workspace = new AdhocWorkspace(TestHostServices())
        let id = SolutionId.CreateNewId()
        let versionStamp = VersionStamp.Create(DateTime.UtcNow)
        let slnPath = "test.sln"

        let solutionInfo = SolutionInfo.Create(id, versionStamp, slnPath, projects)
        let solution = workspace.AddSolution(solutionInfo)
        solution

    static member CreateDocumentInfo projId filePath (code: string) =
        DocumentInfo.Create(
            DocumentId.CreateNewId(projId),
            filePath,
            loader = TextLoader.From(SourceText.From(code).Container, VersionStamp.Create(DateTime.UtcNow)),
            filePath = filePath,
            sourceCodeKind = RoslynTestHelpers.GetSourceCodeKind filePath
        )

    static member CreateProjectInfo id filePath documents =
        ProjectInfo.Create(
            id,
            VersionStamp.Create(DateTime.UtcNow),
            filePath,
            "test.dll",
            LanguageNames.FSharp,
            documents = documents,
            filePath = filePath
        )

    static member SetProjectOptions projId (solution: Solution) (options: FSharpProjectOptions) =
        solution
            .Workspace
            .Services
            .GetService<IFSharpWorkspaceService>()
            .FSharpProjectOptionsManager
            .SetCommandLineOptions(
                projId,
                options.SourceFiles,
                options.OtherOptions |> ImmutableArray.CreateRange
            )

    static member SetEditorOptions (solution: Solution) options =
        solution.Workspace.Services.GetService<EditorOptions>().With(options)

    static member CreateSolution(source, ?options: FSharpProjectOptions, ?editorOptions) =
        let projId = ProjectId.CreateNewId()

        let docInfo = RoslynTestHelpers.CreateDocumentInfo projId "C:\\test.fs" source

        let projFilePath = "C:\\test.fsproj"
        let projInfo = RoslynTestHelpers.CreateProjectInfo projId projFilePath [ docInfo ]
        let solution = RoslynTestHelpers.CreateSolution [ projInfo ]

        options
        |> Option.defaultValue RoslynTestHelpers.DefaultProjectOptions
        |> RoslynTestHelpers.SetProjectOptions projId solution

        if editorOptions.IsSome then
            RoslynTestHelpers.SetEditorOptions solution editorOptions.Value

        solution

    static member GetSingleDocument(solution: Solution) =
        let project = solution.Projects |> Seq.exactlyOne
        let document = project.Documents |> Seq.exactlyOne
        document

    static member CreateSolution(syntheticProject: SyntheticProject) =

        let checker = syntheticProject.SaveAndCheck()

        assert (syntheticProject.DependsOn = []) // multi-project not supported yet

        let projId = ProjectId.CreateNewId()

        let docInfos =
            [
                for project, file in syntheticProject.GetAllFiles() do
                    let filePath = getFilePath project file
                    RoslynTestHelpers.CreateDocumentInfo projId filePath (File.ReadAllText filePath)

                    if file.HasSignatureFile then
                        let sigFilePath = getSignatureFilePath project file
                        RoslynTestHelpers.CreateDocumentInfo projId sigFilePath (File.ReadAllText sigFilePath)
            ]

        let projInfo =
            RoslynTestHelpers.CreateProjectInfo projId syntheticProject.ProjectFileName docInfos

        let options = syntheticProject.GetProjectOptions checker

        let metadataReferences =
            options.OtherOptions
            |> Seq.filter (fun x -> x.StartsWith("-r:"))
            |> Seq.map (fun x -> x.Substring(3) |> MetadataReference.CreateFromFile :> MetadataReference)

        let projInfo = projInfo.WithMetadataReferences metadataReferences

        let solution = RoslynTestHelpers.CreateSolution [ projInfo ]

        options |> RoslynTestHelpers.SetProjectOptions projId solution

        solution, checker

    static member GetFsDocument(code, ?customProjectOption: string, ?customEditorOptions) =
        let customProjectOptions =
            customProjectOption
            |> Option.map (fun o -> [| o |])
            |> Option.defaultValue (Array.empty)

        let options =
            { RoslynTestHelpers.DefaultProjectOptions with
                OtherOptions =
                    [|
                        "--targetprofile:netcore" // without this lib some symbols are not loaded
                        "--nowarn:3384" // The .NET SDK for this script could not be determined
                    |]
                    |> Array.append customProjectOptions
            }

        let solution =
            match customEditorOptions with
            | Some o -> RoslynTestHelpers.CreateSolution(code, options, o)
            | None -> RoslynTestHelpers.CreateSolution(code, options)

        solution |> RoslynTestHelpers.GetSingleDocument

    static member GetFsiAndFsDocuments (fsiCode: string) (fsCode: string) =
        let projInfo =
            { SyntheticProject.Create(
                  { sourceFile "test" [] with
                      SignatureFile = Custom fsiCode
                      Source = fsCode
                  }
              ) with

                AutoAddModules = false
                SkipInitialCheck = true
                OtherOptions =
                    [
                        "--targetprofile:netcore" // without this lib some symbols are not loaded
                        "--nowarn:3384" // The .NET SDK for this script could not be determined
                    ]
            }

        let solution, _ = RoslynTestHelpers.CreateSolution projInfo
        let project = solution.Projects |> Seq.exactlyOne

        project.Documents
        |> Seq.sortWith (fun d1 _ -> if d1.IsFSharpSignatureFile then -1 else 1)
