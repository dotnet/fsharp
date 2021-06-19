namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
open System.IO
open System.Reflection
open Microsoft.VisualStudio.Composition
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Host
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor

type TestHostWorkspaceServices(hostServices: HostServices, workspace: Workspace) =
    inherit HostWorkspaceServices()

    let resolver = Resolver.DefaultInstance
    let catalog = 
        let asms = AppDomain.CurrentDomain.GetAssemblies()
        let partDiscovery = PartDiscovery.Combine(new AttributedPartDiscoveryV1(resolver), new AttributedPartDiscovery(resolver, isNonPublicSupported = true));
        let parts = partDiscovery.CreatePartsAsync(asms).Result
        let catalog = ComposableCatalog.Create(resolver)
        catalog.AddParts(parts)

    let configuration = CompositionConfiguration.Create(catalog.WithCompositionService())
    let runtimeComposition = RuntimeComposition.CreateRuntimeComposition(configuration)
    let exportProviderFactory = runtimeComposition.CreateExportProviderFactory()
    let exportProvider = exportProviderFactory.CreateExportProvider().AsExportProvider()

    override _.Workspace = workspace

    override _.GetService<'T when 'T :> IWorkspaceService>() =
        exportProvider.GetExport<'T>().Value

    override _.FindLanguageServices(filter) = Seq.empty

    override _.HostServices = hostServices

type TestHostServices() =
    inherit HostServices()

    static let instance = TestHostServices()
    static member Instance = instance

    override this.CreateWorkspaceServices(workspace) =
        TestHostWorkspaceServices(this, workspace) :> HostWorkspaceServices

[<AbstractClass;Sealed>]
type RoslynTestHelpers private () =

    static member CreateDocument (filePath, text: SourceText) =
        let isScript = String.Equals(Path.GetExtension(filePath), ".fsx", StringComparison.OrdinalIgnoreCase)
        let asms = AppDomain.CurrentDomain.GetAssemblies()
        let workspace = new AdhocWorkspace(Host.Mef.MefHostServices.Create(asms))

        let projId = ProjectId.CreateNewId()
        let docId = DocumentId.CreateNewId(projId)

        let docInfo =
            DocumentInfo.Create(
                docId,
                filePath,
                loader=TextLoader.From(text.Container, VersionStamp.Create(DateTime.UtcNow)),
                filePath=filePath,
                sourceCodeKind= if isScript then SourceCodeKind.Script else SourceCodeKind.Regular)

        let projInfo =
            ProjectInfo.Create(
                projId,
                VersionStamp.Create(DateTime.UtcNow),
                "test.fsproj", 
                "test.dll", 
                LanguageNames.CSharp, // We cannot use LanguageNames.FSharp as Roslyn doesn't support creating adhoc projects with F# language name.
                documents = [docInfo]
            )

        let solutionInfo = SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Create(DateTime.UtcNow), "test.sln", [projInfo])

        let solution = workspace.AddSolution(solutionInfo)
        solution.GetProject(projId).GetDocument(docId)

    static member CreateDocument (filePath, code: string) =
        let text = SourceText.From(code)
        RoslynTestHelpers.CreateDocument(filePath, text), text

