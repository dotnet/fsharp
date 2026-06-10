#nowarn "57"

namespace FSharp.Compiler.Service.Tests.HotReload

open System
open System.IO
open System.Reflection
open System.Reflection.Metadata
open System.Reflection.Metadata.Ecma335
open System.Reflection.PortableExecutable
open System.Runtime.Loader
open Xunit

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.Workspace
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Text
open FSharp.Test
open FSharp.Test.Utilities

open FSharp.Compiler.Service.Tests.Common

[<Collection(nameof NotThreadSafeResourceCollection)>]
module HotReloadCheckerTests =

    let private baselineSource =
        """
namespace Sample

type Type =
    static member GetValue() = 1
"""

    let private updatedSource =
        """
namespace Sample

type Type =
    static member GetValue() = 2
"""

    let private createChecker () =
        FSharpChecker.Create(
            keepAssemblyContents = true,
            keepAllBackgroundResolutions = false,
            keepAllBackgroundSymbolUses = false,
            enableBackgroundItemKeyStoreAndSemanticClassification = false,
            enablePartialTypeChecking = false,
            captureIdentifiersWhenParsing = false,
            useTransparentCompiler = CompilerAssertHelpers.UseTransparentCompiler
        )

    let private prepareProjectOptions
        (checker: FSharpChecker)
        (fsPath: string)
        (dllPath: string)
        (source: string)
        =
        let projectOptions, _ =
            checker.GetProjectOptionsFromScript(
                fsPath,
                SourceText.ofString source,
                assumeDotNetFramework = false,
                useSdkRefs = true,
                useFsiAuxLib = false
            )
            |> Async.RunImmediate

        { projectOptions with
            SourceFiles = [| fsPath |]
            OtherOptions =
                projectOptions.OtherOptions
                |> Array.append
                    [| "--target:library"
                       "--langversion:preview"
                       "--optimize-"
                       "--debug:portable"
                       "--deterministic"
                       "--enable:hotreloaddeltas"
                       $"--out:{dllPath}" |] }

    let private compileProject
        (checker: FSharpChecker)
        (projectOptions: FSharpProjectOptions)
        (includeHotReloadCapture: bool)
        =
        let options =
            if includeHotReloadCapture then
                projectOptions.OtherOptions
            else
                projectOptions.OtherOptions
                |> Array.filter (fun opt -> not (opt.StartsWith("--enable:hotreloaddeltas", StringComparison.OrdinalIgnoreCase)))

        let argv =
            Array.concat [ [| "fsc.exe" |]; options; projectOptions.SourceFiles ]

        let diagnostics, exOpt =
            checker.Compile(argv)
            |> Async.RunImmediate

        let errors =
            diagnostics
            |> Array.filter (fun diagnostic -> diagnostic.Severity = FSharpDiagnosticSeverity.Error)

        match errors, exOpt with
        | [||], None -> ()
        | errs, _ ->
            failwithf "Compilation failed: %A" (errs |> Array.map (fun d -> d.Message))

    let private withShortOutputOption (projectOptions: FSharpProjectOptions) (dllPath: string) =
        { projectOptions with
            OtherOptions =
                projectOptions.OtherOptions
                |> Array.filter (fun opt ->
                    not (opt.StartsWith("--out:", StringComparison.OrdinalIgnoreCase) ||
                         opt.StartsWith("-o:", StringComparison.OrdinalIgnoreCase) ||
                         String.Equals(opt, "-o", StringComparison.OrdinalIgnoreCase)))
                |> Array.append [| $"-o:{dllPath}" |] }


    let private withSplitOutputOption (projectOptions: FSharpProjectOptions) (outputSwitch: string) (dllPath: string) =
        { projectOptions with
            OtherOptions =
                projectOptions.OtherOptions
                |> Array.filter (fun opt ->
                    not (opt.StartsWith("--out:", StringComparison.OrdinalIgnoreCase) ||
                         opt.StartsWith("-o:", StringComparison.OrdinalIgnoreCase) ||
                         String.Equals(opt, "-o", StringComparison.OrdinalIgnoreCase) ||
                         String.Equals(opt, "--out", StringComparison.OrdinalIgnoreCase)))
                |> Array.append [| outputSwitch; dllPath |] }

    let private withExecutableTarget (projectOptions: FSharpProjectOptions) =
        { projectOptions with
            OtherOptions =
                projectOptions.OtherOptions
                |> Array.map (fun opt ->
                    if String.Equals(opt, "--target:library", StringComparison.OrdinalIgnoreCase) then
                        "--target:exe"
                    else
                        opt) }

    let private withTrackedResourceInput (projectOptions: FSharpProjectOptions) (resourcePath: string) =
        { projectOptions with
            OtherOptions =
                projectOptions.OtherOptions
                |> Array.append [| $"--resource:{resourcePath},HotReloadPayload" |] }

    let private withReferences (referencePaths: string list) (projectOptions: FSharpProjectOptions) =
        let referenceArgs =
            referencePaths
            |> List.map (fun path -> "-r:" + path)
            |> List.toArray

        { projectOptions with
            OtherOptions =
                projectOptions.OtherOptions
                |> Array.append referenceArgs }

    let private getTypeProviderReferencePaths () =
#if DEBUG
        let csharpAnalysisPath =
            Path.Combine(__SOURCE_DIRECTORY__, "../../../artifacts/bin/TestTP/Debug/netstandard2.0/CSharp_Analysis.dll")
            |> Path.GetFullPath

        let testTypeProviderPath =
            Path.Combine(__SOURCE_DIRECTORY__, "../../../artifacts/bin/TestTP/Debug/netstandard2.0/TestTP.dll")
            |> Path.GetFullPath
#else
        let csharpAnalysisPath =
            Path.Combine(__SOURCE_DIRECTORY__, "../../../artifacts/bin/TestTP/Release/netstandard2.0/CSharp_Analysis.dll")
            |> Path.GetFullPath

        let testTypeProviderPath =
            Path.Combine(__SOURCE_DIRECTORY__, "../../../artifacts/bin/TestTP/Release/netstandard2.0/TestTP.dll")
            |> Path.GetFullPath
#endif

        if not (File.Exists(csharpAnalysisPath)) then
            failwithf "Expected type-provider dependency assembly to exist at '%s'." csharpAnalysisPath

        if not (File.Exists(testTypeProviderPath)) then
            failwithf "Expected type-provider assembly to exist at '%s'." testTypeProviderPath

        csharpAnalysisPath, testTypeProviderPath

    let private toWorkspaceCompilerArgs (projectOptions: FSharpProjectOptions) =
        Array.append projectOptions.OtherOptions projectOptions.SourceFiles

    let private createProjectSnapshot (projectOptions: FSharpProjectOptions) =
        FSharpProjectSnapshot.FromOptions(projectOptions, DocumentSource.FileSystem)
        |> Async.RunImmediate

    let private getMethodTokenInfos (dllPath: string) =
        use stream = File.OpenRead(dllPath)
        use peReader = new PEReader(stream)
        let metadataReader = peReader.GetMetadataReader()

        metadataReader.MethodDefinitions
        |> Seq.map (fun handle ->
            let methodDef = metadataReader.GetMethodDefinition(handle)
            let declaringType = metadataReader.GetTypeDefinition(methodDef.GetDeclaringType())
            let declaringTypeName = metadataReader.GetString(declaringType.Name)
            let methodName = metadataReader.GetString(methodDef.Name)
            let token = MetadataTokens.GetToken(EntityHandle.op_Implicit handle)
            declaringTypeName, methodName, token)
        |> Seq.toList

    let private getMethodToken (dllPath: string) (declaringType: string) (methodName: string) =
        getMethodTokenInfos dllPath
        |> List.tryFind (fun (typeName, name, _) -> typeName = declaringType && name = methodName)
        |> Option.map (fun (_, _, token) -> token)
        |> Option.defaultWith (fun () ->
            let available =
                getMethodTokenInfos dllPath
                |> List.map (fun (typeName, name, token) -> sprintf "%s::%s (0x%08X)" typeName name token)
                |> String.concat "; "

            failwithf
                "Failed to find method token for %s::%s in '%s'. Available methods: %s"
                declaringType
                methodName
                dllPath
                available)

    let private getMethodTokenByParameterCount (dllPath: string) (declaringType: string) (methodName: string) (parameterCount: int) =
        use stream = File.OpenRead(dllPath)
        use peReader = new PEReader(stream)
        let metadataReader = peReader.GetMetadataReader()

        let tryReadParameterCount (methodDef: MethodDefinition) =
            try
                let blobReader = metadataReader.GetBlobReader(methodDef.Signature)
                let header = blobReader.ReadByte()
                let hasGenericArity = (header &&& 0x10uy) <> 0uy

                if hasGenericArity then
                    ignore (blobReader.ReadCompressedInteger())

                blobReader.ReadCompressedInteger()
            with _ ->
                -1

        metadataReader.MethodDefinitions
        |> Seq.choose (fun handle ->
            let methodDef = metadataReader.GetMethodDefinition(handle)
            let typeDef = metadataReader.GetTypeDefinition(methodDef.GetDeclaringType())
            let typeName = metadataReader.GetString(typeDef.Name)
            let name = metadataReader.GetString(methodDef.Name)

            if typeName = declaringType && name = methodName then
                let token = MetadataTokens.GetToken(EntityHandle.op_Implicit handle)
                let count = tryReadParameterCount methodDef
                Some(count, token)
            else
                None)
        |> Seq.tryFind (fun (count, _) -> count = parameterCount)
        |> Option.map snd
        |> Option.defaultWith (fun () ->
            let available =
                metadataReader.MethodDefinitions
                |> Seq.choose (fun handle ->
                    let methodDef = metadataReader.GetMethodDefinition(handle)
                    let typeDef = metadataReader.GetTypeDefinition(methodDef.GetDeclaringType())
                    let typeName = metadataReader.GetString(typeDef.Name)
                    let name = metadataReader.GetString(methodDef.Name)
                    if typeName = declaringType && name = methodName then
                        let token = MetadataTokens.GetToken(EntityHandle.op_Implicit handle)
                        let count = tryReadParameterCount methodDef
                        Some(sprintf "%s::%s/%d (0x%08X)" typeName name count token)
                    else
                        None)
                |> String.concat "; "

            failwithf
                "Failed to find method token for %s::%s/%d in '%s'. Available overloads: %s"
                declaringType
                methodName
                parameterCount
                dllPath
                available)

    let private getMethodDisplayByToken (dllPath: string) (token: int) =
        getMethodTokenInfos dllPath
        |> List.tryFind (fun (_, _, methodToken) -> methodToken = token)
        |> Option.map (fun (typeName, methodName, _) -> $"{typeName}::{methodName}")
        |> Option.defaultWith (fun () -> $"<unknown:0x{token:X8}>")

    let private getMethodTokenByParameterTypes
        (dllPath: string)
        (declaringType: string)
        (methodName: string)
        (parameterTypeNames: string list)
        =
        let contextId = Guid.NewGuid().ToString("N")
        let loadContext = new AssemblyLoadContext($"fcs-hotreload-{contextId}", isCollectible = true)

        try
            let assembly = loadContext.LoadFromAssemblyPath(Path.GetFullPath(dllPath))

            let declaringTypeInfo =
                assembly.GetTypes()
                |> Array.tryFind (fun typeInfo -> typeInfo.Name = declaringType)
                |> Option.defaultWith (fun () ->
                    let availableTypes =
                        assembly.GetTypes()
                        |> Array.map (fun typeInfo -> typeInfo.FullName)
                        |> String.concat "; "

                    failwithf
                        "Failed to find type '%s' in '%s'. Available types: %s"
                        declaringType
                        dllPath
                        availableTypes)

            let matchingMethod =
                declaringTypeInfo.GetMethods(BindingFlags.Instance ||| BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
                |> Array.filter (fun methodInfo -> methodInfo.Name = methodName)
                |> Array.tryFind (fun methodInfo ->
                    let methodParameterTypes =
                        methodInfo.GetParameters()
                        |> Array.map (fun parameter -> parameter.ParameterType.FullName)
                        |> Array.toList

                    methodParameterTypes = parameterTypeNames)

            match matchingMethod with
            | Some methodInfo -> methodInfo.MetadataToken
            | None ->
                let availableOverloads =
                    declaringTypeInfo.GetMethods(BindingFlags.Instance ||| BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
                    |> Array.filter (fun methodInfo -> methodInfo.Name = methodName)
                    |> Array.map (fun methodInfo ->
                        let methodParameterTypes =
                            methodInfo.GetParameters()
                            |> Array.map (fun parameter -> parameter.ParameterType.FullName)
                            |> String.concat ", "

                        $"{methodInfo.Name}({methodParameterTypes})")
                    |> String.concat "; "

                failwithf
                    "Failed to find method token for %s::%s(%s) in '%s'. Available overloads: %s"
                    declaringType
                    methodName
                    (String.concat ", " parameterTypeNames)
                    dllPath
                    availableOverloads
        finally
            loadContext.Unload()

    let private getMethodTokenBySignature
        (dllPath: string)
        (declaringType: string)
        (methodName: string)
        (genericArity: int)
        (parameterTypeNames: string list)
        =
        let contextId = Guid.NewGuid().ToString("N")
        let loadContext = new AssemblyLoadContext($"fcs-hotreload-sig-{contextId}", isCollectible = true)

        try
            let assembly = loadContext.LoadFromAssemblyPath(Path.GetFullPath(dllPath))

            let declaringTypeInfo =
                assembly.GetTypes()
                |> Array.tryFind (fun typeInfo -> typeInfo.Name = declaringType)
                |> Option.defaultWith (fun () ->
                    let availableTypes =
                        assembly.GetTypes()
                        |> Array.map (fun typeInfo -> typeInfo.FullName)
                        |> String.concat "; "

                    failwithf
                        "Failed to find type '%s' in '%s'. Available types: %s"
                        declaringType
                        dllPath
                        availableTypes)

            let matchingMethod =
                declaringTypeInfo.GetMethods(BindingFlags.Instance ||| BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
                |> Array.filter (fun methodInfo -> methodInfo.Name = methodName)
                |> Array.tryFind (fun methodInfo ->
                    let methodGenericArity = methodInfo.GetGenericArguments().Length

                    let methodParameterTypes =
                        methodInfo.GetParameters()
                        |> Array.map (fun parameter -> parameter.ParameterType.FullName)
                        |> Array.toList

                    methodGenericArity = genericArity && methodParameterTypes = parameterTypeNames)

            match matchingMethod with
            | Some methodInfo -> methodInfo.MetadataToken
            | None ->
                let availableOverloads =
                    declaringTypeInfo.GetMethods(BindingFlags.Instance ||| BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
                    |> Array.filter (fun methodInfo -> methodInfo.Name = methodName)
                    |> Array.map (fun methodInfo ->
                        let methodGenericArity = methodInfo.GetGenericArguments().Length

                        let methodParameterTypes =
                            methodInfo.GetParameters()
                            |> Array.map (fun parameter -> parameter.ParameterType.FullName)
                            |> String.concat ", "

                        $"{methodInfo.Name}`{methodGenericArity}({methodParameterTypes})")
                    |> String.concat "; "

                failwithf
                    "Failed to find method token for %s::%s`%d(%s) in '%s'. Available overloads: %s"
                    declaringType
                    methodName
                    genericArity
                    (String.concat ", " parameterTypeNames)
                    dllPath
                    availableOverloads
        finally
            loadContext.Unload()

    [<Fact>]
    let ``HotReloadCapabilities expose supported flags`` () =
        let checker = createChecker ()
        let capabilities = checker.HotReloadCapabilities

        Assert.True(capabilities.SupportsIl, "Expected IL support flag to be set")
        Assert.True(capabilities.SupportsMetadata, "Expected metadata support flag to be set")
        Assert.True(capabilities.SupportsPortablePdb, "Expected portable PDB support flag to be set")
        Assert.True(capabilities.SupportsMultipleGenerations, "Expected multi-generation flag to be set")
        Assert.False(capabilities.SupportsRuntimeApply, "Runtime apply capability should require explicit opt-in")

    [<Fact>]
    let ``Compiler outputs stay byte-identical when hot reload capture flag is toggled`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-flag-parity", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")
        let pdbPath = Path.ChangeExtension(dllPath, ".pdb")

        File.WriteAllText(fsPath, baselineSource)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath baselineSource

        checker.InvalidateAll()
        compileProject checker projectOptions false
        let baselineDllBytes = File.ReadAllBytes(dllPath)
        let baselinePdbBytes = File.ReadAllBytes(pdbPath)

        compileProject checker projectOptions true
        let hotReloadDllBytes = File.ReadAllBytes(dllPath)
        let hotReloadPdbBytes = File.ReadAllBytes(pdbPath)

        Assert.Equal<byte>(baselineDllBytes, hotReloadDllBytes)
        Assert.Equal<byte>(baselinePdbBytes, hotReloadPdbBytes)

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``StartHotReloadSession and EmitHotReloadDelta produce delta`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-checker", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        File.WriteAllText(fsPath, baselineSource)

        let checker = createChecker ()

        let projectOptions = prepareProjectOptions checker fsPath dllPath baselineSource

        // Build the baseline assembly that StartHotReloadSession will use.
        checker.InvalidateAll()
        compileProject checker projectOptions true

        let startResult =
            checker.StartHotReloadSession(projectOptions)
            |> Async.RunImmediate

        match startResult with
        | Error error -> failwithf "Failed to start hot reload session: %A" error
        | Ok () -> ()

        // Update source, rebuild without triggering another baseline capture, and emit a delta.
        File.WriteAllText(fsPath, updatedSource)
        checker.NotifyFileChanged(fsPath, projectOptions)
        |> Async.RunImmediate
        compileProject checker projectOptions false

        let emitResult =
            checker.EmitHotReloadDelta(projectOptions)
            |> Async.RunImmediate

        match emitResult with
        | Error error -> failwithf "EmitHotReloadDelta failed: %A" error
        | Ok delta ->
            Assert.NotEmpty(delta.Metadata)
            Assert.NotEmpty(delta.IL)
            Assert.NotEmpty(delta.UpdatedMethods)

        checker.EndHotReloadSession()
        Assert.False(checker.HotReloadSessionActive)

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``StartHotReloadSession replaces existing process-wide session`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-checker-single-session", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath1 = Path.Combine(projectDir, "LibraryOne.fs")
        let fsPath2 = Path.Combine(projectDir, "LibraryTwo.fs")
        let dllPath1 = Path.Combine(projectDir, "LibraryOne.dll")
        let dllPath2 = Path.Combine(projectDir, "LibraryTwo.dll")

        let sourceOne =
            """
namespace SessionOne

type Type =
    static member GetValue() = 1
"""

        let sourceTwo =
            """
namespace SessionTwo

type Type =
    static member GetValue() = 2
"""

        File.WriteAllText(fsPath1, sourceOne)
        File.WriteAllText(fsPath2, sourceTwo)

        let checker = createChecker ()
        let projectOptions1 = prepareProjectOptions checker fsPath1 dllPath1 sourceOne
        let projectOptions2 = prepareProjectOptions checker fsPath2 dllPath2 sourceTwo

        checker.InvalidateAll()

        compileProject checker projectOptions1 true

        match checker.StartHotReloadSession(projectOptions1) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start first hot reload session: %A" error
        | Ok () -> ()

        let firstSession =
            match FSharp.Compiler.HotReloadState.tryGetSession () with
            | ValueSome session -> session
            | ValueNone -> failwith "Expected the first hot reload session to be active."

        compileProject checker projectOptions2 true

        match checker.StartHotReloadSession(projectOptions2) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start replacement hot reload session: %A" error
        | Ok () -> ()

        let secondSession =
            match FSharp.Compiler.HotReloadState.tryGetSession () with
            | ValueSome session -> session
            | ValueNone -> failwith "Expected replacement hot reload session to be active."

        Assert.True(checker.HotReloadSessionActive)
        Assert.NotEqual<Guid>(firstSession.Baseline.ModuleId, secondSession.Baseline.ModuleId)

        checker.EndHotReloadSession()
        Assert.False(checker.HotReloadSessionActive)

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``StartHotReloadSession replacement clears pending update from prior session`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-checker-session-replacement", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath1 = Path.Combine(projectDir, "LibraryOne.fs")
        let fsPath2 = Path.Combine(projectDir, "LibraryTwo.fs")
        let dllPath1 = Path.Combine(projectDir, "LibraryOne.dll")
        let dllPath2 = Path.Combine(projectDir, "LibraryTwo.dll")

        let sourceOne =
            """
namespace SessionOne

type Type =
    static member GetValue() = 1
"""

        let sourceTwo =
            """
namespace SessionTwo

type Type =
    static member GetValue() = 2
"""

        File.WriteAllText(fsPath1, sourceOne)
        File.WriteAllText(fsPath2, sourceTwo)

        let checker = createChecker ()
        let projectOptions1 = prepareProjectOptions checker fsPath1 dllPath1 sourceOne
        let projectOptions2 = prepareProjectOptions checker fsPath2 dllPath2 sourceTwo

        checker.InvalidateAll()

        compileProject checker projectOptions1 true

        match checker.StartHotReloadSession(projectOptions1) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start first hot reload session: %A" error
        | Ok () -> ()

        let firstSession =
            match FSharp.Compiler.HotReloadState.tryGetSession () with
            | ValueSome session -> session
            | ValueNone -> failwith "Expected first hot reload session to be active."

        let stagedPendingBaseline =
            { firstSession.Baseline with
                EncId = Guid.NewGuid()
                NextGeneration = firstSession.Baseline.NextGeneration + 1 }

        FSharp.Compiler.HotReloadState.updateBaseline stagedPendingBaseline

        let firstSessionWithPending =
            match FSharp.Compiler.HotReloadState.tryGetSession () with
            | ValueSome session -> session
            | ValueNone -> failwith "Expected first hot reload session to remain active after staging pending update."

        Assert.True(firstSessionWithPending.PendingUpdate.IsSome)

        compileProject checker projectOptions2 true

        match checker.StartHotReloadSession(projectOptions2) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start replacement hot reload session: %A" error
        | Ok () -> ()

        let replacementSession =
            match FSharp.Compiler.HotReloadState.tryGetSession () with
            | ValueSome session -> session
            | ValueNone -> failwith "Expected replacement hot reload session to be active."

        Assert.True(checker.HotReloadSessionActive)
        Assert.NotEqual<Guid>(firstSessionWithPending.Baseline.ModuleId, replacementSession.Baseline.ModuleId)
        Assert.True(replacementSession.PendingUpdate.IsNone)

        checker.EndHotReloadSession()
        Assert.False(checker.HotReloadSessionActive)

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``StartHotReloadSession and EmitHotReloadDelta accept project snapshots`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-checker-snapshot", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        File.WriteAllText(fsPath, baselineSource)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath baselineSource
        let baselineSnapshot = createProjectSnapshot projectOptions

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(baselineSnapshot) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start hot reload session from snapshot: %A" error
        | Ok () -> ()

        File.WriteAllText(fsPath, updatedSource)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        let updatedSnapshot = createProjectSnapshot projectOptions

        match checker.EmitHotReloadDelta(updatedSnapshot) |> Async.RunImmediate with
        | Error error -> failwithf "EmitHotReloadDelta failed for snapshot input: %A" error
        | Ok delta ->
            Assert.NotEmpty(delta.Metadata)
            Assert.NotEmpty(delta.IL)
            Assert.NotEmpty(delta.UpdatedMethods)

        checker.EndHotReloadSession()
        Assert.False(checker.HotReloadSessionActive)

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``Workspace project snapshots drive hot reload session lifecycle`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-checker-workspace", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")
        let projectPath = Path.Combine(projectDir, "Library.fsproj")
        File.WriteAllText(projectPath, "<Project Sdk=\"Microsoft.NET.Sdk\"></Project>")
        File.WriteAllText(fsPath, baselineSource)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath baselineSource
        let workspace = FSharpWorkspace(checker)
        let fileUri = Uri(fsPath)

        checker.InvalidateAll()
        compileProject checker projectOptions true

        let projectIdentifier =
            workspace.Projects.AddOrUpdate(projectPath, dllPath, toWorkspaceCompilerArgs projectOptions)

        let baselineSnapshot =
            workspace.Query.GetProjectSnapshot(projectIdentifier)
            |> Option.defaultWith (fun () -> failwith "Expected workspace baseline snapshot.")

        match checker.StartHotReloadSession(baselineSnapshot) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start hot reload session from workspace snapshot: %A" error
        | Ok () -> ()

        File.WriteAllText(fsPath, updatedSource)
        workspace.Files.Close(fileUri)
        compileProject checker projectOptions false

        workspace.Projects.AddOrUpdate(projectPath, dllPath, toWorkspaceCompilerArgs projectOptions)
        |> ignore

        let updatedSnapshot =
            workspace.Query.GetProjectSnapshot(projectIdentifier)
            |> Option.defaultWith (fun () -> failwith "Expected workspace updated snapshot.")

        match checker.EmitHotReloadDelta(updatedSnapshot) |> Async.RunImmediate with
        | Error error -> failwithf "EmitHotReloadDelta failed for workspace snapshot input: %A" error
        | Ok delta ->
            Assert.NotEmpty(delta.Metadata)
            Assert.NotEmpty(delta.IL)
            Assert.NotEmpty(delta.UpdatedMethods)

        checker.EndHotReloadSession()
        Assert.False(checker.HotReloadSessionActive)

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``Workspace snapshot config version changes when tracked dependency input changes`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-workspace-tracked-inputs", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")
        let projectPath = Path.Combine(projectDir, "Library.fsproj")
        let resourcePath = Path.Combine(projectDir, "payload.xaml")

        File.WriteAllText(projectPath, "<Project Sdk=\"Microsoft.NET.Sdk\"></Project>")
        File.WriteAllText(fsPath, baselineSource)
        File.WriteAllText(resourcePath, "<Page><TextBlock Text=\"v1\" /></Page>")

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath baselineSource |> withTrackedResourceInput <| resourcePath
        let workspace = FSharpWorkspace(checker)

        let projectIdentifier =
            workspace.Projects.AddOrUpdate(projectPath, dllPath, toWorkspaceCompilerArgs projectOptions)

        let baselineSnapshot =
            workspace.Query.GetProjectSnapshot(projectIdentifier)
            |> Option.defaultWith (fun () -> failwith "Expected workspace baseline snapshot.")

        let baselineVersion = Convert.ToHexString(baselineSnapshot.ProjectConfig.Version)
        let baselineTrackedInput =
            baselineSnapshot.ProjectConfig.TrackedInputsOnDisk
            |> List.tryFind (fun reference ->
                String.Equals(Path.GetFullPath(reference.Path), Path.GetFullPath(resourcePath), StringComparison.Ordinal))
            |> Option.defaultWith (fun () -> failwith "Expected tracked dependency input to be present in workspace config.")

        File.WriteAllText(resourcePath, "<Page><TextBlock Text=\"v2\" /></Page>")
        File.SetLastWriteTime(resourcePath, baselineTrackedInput.LastModified.AddSeconds(2.0))
        workspace.Files.Close(Uri(resourcePath))
        workspace.Projects.AddOrUpdate(projectPath, dllPath, toWorkspaceCompilerArgs projectOptions) |> ignore

        let updatedSnapshot =
            workspace.Query.GetProjectSnapshot(projectIdentifier)
            |> Option.defaultWith (fun () -> failwith "Expected workspace updated snapshot.")

        let updatedVersion = Convert.ToHexString(updatedSnapshot.ProjectConfig.Version)
        let updatedTrackedInput =
            updatedSnapshot.ProjectConfig.TrackedInputsOnDisk
            |> List.tryFind (fun reference ->
                String.Equals(Path.GetFullPath(reference.Path), Path.GetFullPath(resourcePath), StringComparison.Ordinal))
            |> Option.defaultWith (fun () -> failwith "Expected tracked dependency input to remain in workspace config.")

        Assert.True(
            not (String.Equals(baselineVersion, updatedVersion, StringComparison.Ordinal)),
            "Expected workspace project config version to change after tracked dependency input update.")
        Assert.True(
            updatedTrackedInput.LastModified > baselineTrackedInput.LastModified,
            $"Expected tracked input timestamp to advance. Before={baselineTrackedInput.LastModified:o}, After={updatedTrackedInput.LastModified:o}")

        try
            Directory.Delete(projectDir, true)
        with _ -> ()


    [<Theory>]
    [<InlineData("-o")>]
    [<InlineData("--out")>]
    let ``Workspace snapshot ignores split output option paths when hashing tracked inputs`` (outputSwitch: string) =
        let projectDir =
            Path.Combine(Path.GetTempPath(), "fcs-hotreload-split-output-tracking", Guid.NewGuid().ToString("N"))

        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")
        let projectPath = Path.Combine(projectDir, "Library.fsproj")
        let resourcePath = Path.Combine(projectDir, "payload.xaml")

        File.WriteAllText(projectPath, "<Project Sdk=\"Microsoft.NET.Sdk\"></Project>")
        File.WriteAllText(fsPath, baselineSource)
        File.WriteAllText(resourcePath, "<Page><TextBlock Text=\"v1\" /></Page>")
        File.WriteAllBytes(dllPath, [| 0uy |])

        try
            let checker = createChecker ()

            let baselineOptions =
                prepareProjectOptions checker fsPath dllPath baselineSource
                |> withTrackedResourceInput <| resourcePath

            let projectOptions = withSplitOutputOption baselineOptions outputSwitch dllPath

            let baselineSnapshot = createProjectSnapshot projectOptions
            let baselineVersion = Convert.ToHexString(baselineSnapshot.ProjectConfig.Version)

            let pathEquals left right =
                String.Equals(Path.GetFullPath(left), Path.GetFullPath(right), StringComparison.Ordinal)

            let baselineTrackedInputs = baselineSnapshot.ProjectConfig.TrackedInputsOnDisk

            let trackedResource =
                baselineTrackedInputs
                |> List.tryFind (fun reference -> pathEquals reference.Path resourcePath)
                |> Option.defaultWith (fun () -> failwith "Expected tracked resource input to be present.")

            Assert.True(
                baselineTrackedInputs
                |> List.forall (fun reference -> not (pathEquals reference.Path dllPath)),
                $"Output path '{dllPath}' should not be tracked for split option '{outputSwitch}'.")

            File.SetLastWriteTime(dllPath, trackedResource.LastModified.AddSeconds(1.0))

            let outputTouchedSnapshot = createProjectSnapshot projectOptions
            let outputTouchedVersion = Convert.ToHexString(outputTouchedSnapshot.ProjectConfig.Version)

            Assert.Equal(
                baselineVersion,
                outputTouchedVersion)

            File.WriteAllText(resourcePath, "<Page><TextBlock Text=\"v2\" /></Page>")
            File.SetLastWriteTime(resourcePath, trackedResource.LastModified.AddSeconds(2.0))

            let resourceTouchedSnapshot = createProjectSnapshot projectOptions
            let resourceTouchedVersion = Convert.ToHexString(resourceTouchedSnapshot.ProjectConfig.Version)

            Assert.True(
                not (String.Equals(outputTouchedVersion, resourceTouchedVersion, StringComparison.Ordinal)),
                "Expected tracked resource changes to update project config version.")
        finally
            try
                Directory.Delete(projectDir, true)
            with _ -> ()

    [<Fact>]
    let ``StartHotReloadSession accepts short output option`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-short-output", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        File.WriteAllText(fsPath, baselineSource)

        let checker = createChecker ()
        let baselineOptions = prepareProjectOptions checker fsPath dllPath baselineSource
        let projectOptions = withShortOutputOption baselineOptions dllPath

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start hot reload session with -o: output option: %A" error
        | Ok () -> ()

        File.WriteAllText(fsPath, updatedSource)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
        | Ok delta ->
            Assert.NotEmpty(delta.Metadata)
            Assert.NotEmpty(delta.IL)
        | Error FSharpHotReloadError.MissingOutputPath ->
            failwith "Expected -o: output option to resolve to a valid output path."
        | Error error ->
            failwithf "EmitHotReloadDelta failed for -o: output option: %A" error

        checker.EndHotReloadSession()

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``EmitHotReloadDelta rejects stale output assembly`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-stale-output", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        File.WriteAllText(fsPath, baselineSource)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath baselineSource

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        File.WriteAllText(fsPath, updatedSource)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate

        // Intentionally skip recompilation so the output assembly stays stale.
        match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
        | Error (FSharpHotReloadError.DeltaEmissionFailed message) ->
            Assert.Contains("stale build output", message, StringComparison.OrdinalIgnoreCase)
        | Error other -> failwithf "Expected DeltaEmissionFailed for stale output, got %A" other
        | Ok _ -> failwith "Expected stale output detection to reject delta emission."

        checker.EndHotReloadSession()

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``Tracked dependency invalidation keeps subsequent source edit hot-reloadable`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-dependency-invalidation", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")
        let resourcePath = Path.Combine(projectDir, "payload.xaml")

        File.WriteAllText(fsPath, baselineSource)
        File.WriteAllText(resourcePath, "<Page><TextBlock Text=\"v1\" /></Page>")

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath baselineSource |> withTrackedResourceInput <| resourcePath

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        let getValueToken = getMethodToken dllPath "Type" "GetValue"

        File.WriteAllText(resourcePath, "<Page><TextBlock Text=\"v2\" /></Page>")
        checker.InvalidateConfiguration(projectOptions)
        compileProject checker projectOptions false

        File.WriteAllText(fsPath, updatedSource)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "EmitHotReloadDelta failed after dependency invalidation: %A" error
        | Ok delta -> Assert.Contains(getValueToken, delta.UpdatedMethods)

        checker.EndHotReloadSession()
        Assert.False(checker.HotReloadSessionActive)

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``Method body edit on module function updates message token and not main`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-module-loop", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        let baseline =
            """
module LoopDemo

let message () = "generation 0"

[<EntryPoint>]
let main _ =
    while true do
        printfn "%s" (message ())
        System.Threading.Thread.Sleep(2000)

    0
"""

        let updated =
            """
module LoopDemo

let message () = "generation 1"

[<EntryPoint>]
let main _ =
    while true do
        printfn "%s" (message ())
        System.Threading.Thread.Sleep(2000)

    0
"""

        File.WriteAllText(fsPath, baseline)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath baseline |> withExecutableTarget

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        let messageToken = getMethodToken dllPath "LoopDemo" "message"
        File.WriteAllText(fsPath, updated)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "EmitHotReloadDelta failed for module loop method edit: %A" error
        | Ok delta ->
            Assert.Contains(messageToken, delta.UpdatedMethods)
            let mainToken = getMethodToken dllPath "LoopDemo" "main"
            Assert.DoesNotContain(mainToken, delta.UpdatedMethods)

        checker.EndHotReloadSession()

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``Property getter edit updates Greeter get_Message token and not main`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-property-loop", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        let baseline =
            """
module LoopProperties

type Greeter() =
    member _.Message = "generation 0"

let greeter = Greeter()

[<EntryPoint>]
let main _ =
    while true do
        printfn "%s" greeter.Message
        System.Threading.Thread.Sleep(2000)

    0
"""

        let updated =
            """
module LoopProperties

type Greeter() =
    member _.Message = "generation 1"

let greeter = Greeter()

[<EntryPoint>]
let main _ =
    while true do
        printfn "%s" greeter.Message
        System.Threading.Thread.Sleep(2000)

    0
"""

        File.WriteAllText(fsPath, baseline)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath baseline |> withExecutableTarget

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        let getterToken = getMethodToken dllPath "Greeter" "get_Message"
        File.WriteAllText(fsPath, updated)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "EmitHotReloadDelta failed for property loop edit: %A" error
        | Ok delta ->
            Assert.Contains(getterToken, delta.UpdatedMethods)
            let mainToken = getMethodToken dllPath "LoopProperties" "main"
            Assert.DoesNotContain(mainToken, delta.UpdatedMethods)

        checker.EndHotReloadSession()

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``Overloaded method-body edit updates matching overload token`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-overload-edit", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        let baseline =
            """
module OverloadDemo

type Calculator() =
    member _.Compute(value: int) = value + 1
    member _.Compute(value: int, extra: int) = value + extra + 1
"""

        let updated =
            """
module OverloadDemo

type Calculator() =
    member _.Compute(value: int) = value + 1
    member _.Compute(value: int, extra: int) = value + extra + 2
"""

        File.WriteAllText(fsPath, baseline)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath baseline

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        let oneArgToken = getMethodTokenByParameterCount dllPath "Calculator" "Compute" 1
        let twoArgToken = getMethodTokenByParameterCount dllPath "Calculator" "Compute" 2
        File.WriteAllText(fsPath, updated)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "EmitHotReloadDelta failed for overload edit: %A" error
        | Ok delta ->
            Assert.Contains(twoArgToken, delta.UpdatedMethods)
            Assert.DoesNotContain(oneArgToken, delta.UpdatedMethods)

        checker.EndHotReloadSession()

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``Same-arity overloaded method-body edit updates matching overload token`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-overload-type-edit", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        let baseline =
            """
module OverloadTypeDemo

type Calculator() =
    member _.Compute(value: int) = value + 1
    member _.Compute(value: string) = value.Length + 1
"""

        let updated =
            """
module OverloadTypeDemo

type Calculator() =
    member _.Compute(value: int) = value + 1
    member _.Compute(value: string) = value.Length + 2
"""

        File.WriteAllText(fsPath, baseline)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath baseline

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        let intOverloadToken = getMethodTokenByParameterTypes dllPath "Calculator" "Compute" [ "System.Int32" ]
        let stringOverloadToken = getMethodTokenByParameterTypes dllPath "Calculator" "Compute" [ "System.String" ]
        File.WriteAllText(fsPath, updated)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "EmitHotReloadDelta failed for same-arity overload edit: %A" error
        | Ok delta ->
            Assert.Contains(stringOverloadToken, delta.UpdatedMethods)
            Assert.DoesNotContain(intOverloadToken, delta.UpdatedMethods)

        checker.EndHotReloadSession()

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``Same-arity overload with typar parameter edits generic overload token`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-overload-typar-edit", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        let baseline =
            """
module GenericOverloadDemo

type Calculator<'T>() =
    member _.Compute(value: 'T) = sprintf "generic:%A" value
    member _.Compute(value: string) = "string:" + value
"""

        let updated =
            """
module GenericOverloadDemo

type Calculator<'T>() =
    member _.Compute(value: 'T) = sprintf "updated:%A" value
    member _.Compute(value: string) = "string:" + value
"""

        File.WriteAllText(fsPath, baseline)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath baseline

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        let genericOverloadToken = getMethodTokenByParameterTypes dllPath "Calculator`1" "Compute" [ null ]
        let stringOverloadToken = getMethodTokenByParameterTypes dllPath "Calculator`1" "Compute" [ "System.String" ]
        File.WriteAllText(fsPath, updated)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "EmitHotReloadDelta failed for typar overload edit: %A" error
        | Ok delta ->
            Assert.Contains(genericOverloadToken, delta.UpdatedMethods)
            Assert.DoesNotContain(stringOverloadToken, delta.UpdatedMethods)

        checker.EndHotReloadSession()

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``Same-arity overload with generic arity difference edits generic overload token`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-overload-generic-arity-edit", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        let baseline =
            """
module GenericArityOverloadDemo

type Calculator() =
    member _.Compute(value: int) = value + 1
    member _.Compute<'T>(value: int) = value + typeof<'T>.Name.Length + 2
"""

        let updated =
            """
module GenericArityOverloadDemo

type Calculator() =
    member _.Compute(value: int) = value + 1
    member _.Compute<'T>(value: int) = value + typeof<'T>.Name.Length + 3
"""

        File.WriteAllText(fsPath, baseline)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath baseline

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        let nonGenericOverloadToken =
            getMethodTokenBySignature dllPath "Calculator" "Compute" 0 [ "System.Int32" ]

        let genericOverloadToken =
            getMethodTokenBySignature dllPath "Calculator" "Compute" 1 [ "System.Int32" ]

        File.WriteAllText(fsPath, updated)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "EmitHotReloadDelta failed for generic arity overload edit: %A" error
        | Ok delta ->
            Assert.Contains(genericOverloadToken, delta.UpdatedMethods)
            Assert.DoesNotContain(nonGenericOverloadToken, delta.UpdatedMethods)

        checker.EndHotReloadSession()

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``Async method-body edit keeps updated methods user-authored`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-async-methods", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        let baseline =
            """
namespace AsyncMethods

module Demo =
    let GetMessage () =
        async {
            do! Async.Sleep 1
            return "generation 0"
        }
"""

        let updated =
            """
namespace AsyncMethods

module Demo =
    let GetMessage () =
        async {
            do! Async.Sleep 1
            let suffix = "1"
            return "generation " + suffix
        }
"""

        File.WriteAllText(fsPath, baseline)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath baseline

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        let getMessageToken = getMethodToken dllPath "Demo" "GetMessage"
        File.WriteAllText(fsPath, updated)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "EmitHotReloadDelta failed for async method edit: %A" error
        | Ok delta ->
            Assert.Contains(getMessageToken, delta.UpdatedMethods)

            let updatedMethodDisplays =
                delta.UpdatedMethods
                |> List.map (getMethodDisplayByToken dllPath)
            let updatedMethodDisplayText = String.concat ", " updatedMethodDisplays

            Assert.True(
                updatedMethodDisplays |> List.exists (fun methodDisplay -> methodDisplay.Contains("@hotreload")),
                $"Expected synthesized helper method update for async edit. Updated methods: {updatedMethodDisplayText}")

        checker.EndHotReloadSession()

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``Computation-expression usage edit updates user-authored view method token`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-ce-usage", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        let baseline =
            """
module UiDslDemo

type HtmlBuilder() =
    member _.Yield(text: string) = text
    member _.Combine(a: string, b: string) = a + b
    member _.Delay(f: unit -> string) = f()
    member _.Zero() = ""

let html = HtmlBuilder()

let view name =
    html {
        "Hello, "
        name
    }
"""

        let updated =
            """
module UiDslDemo

type HtmlBuilder() =
    member _.Yield(text: string) = text
    member _.Combine(a: string, b: string) = a + b
    member _.Delay(f: unit -> string) = f()
    member _.Zero() = ""

let html = HtmlBuilder()

let view name =
    html {
        "Welcome, "
        name
    }
"""

        File.WriteAllText(fsPath, baseline)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath baseline

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        let viewToken = getMethodToken dllPath "UiDslDemo" "view"
        File.WriteAllText(fsPath, updated)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "EmitHotReloadDelta failed for computation-expression usage edit: %A" error
        | Ok delta -> Assert.Contains(viewToken, delta.UpdatedMethods)

        checker.EndHotReloadSession()

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``Computation-expression usage edit with local lambda still targets user-authored method token`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-ce-transformed-helpers", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        let baseline =
            """
module UiDslDemo

type HtmlBuilder() =
    member _.Yield(text: string) = text
    member _.Combine(a: string, b: string) = a + b
    member _.Delay(f: unit -> string) = f()
    member _.Zero() = ""

let html = HtmlBuilder()

let view name =
    let prefixFactory = fun () -> "Hello, "
    html {
        prefixFactory ()
        name
    }
"""

        let updated =
            """
module UiDslDemo

type HtmlBuilder() =
    member _.Yield(text: string) = text
    member _.Combine(a: string, b: string) = a + b
    member _.Delay(f: unit -> string) = f()
    member _.Zero() = ""

let html = HtmlBuilder()

let view name =
    let prefixFactory = fun () -> "Welcome, "
    html {
        prefixFactory ()
        name
    }
"""

        File.WriteAllText(fsPath, baseline)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath baseline

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        let viewToken = getMethodToken dllPath "UiDslDemo" "view"
        File.WriteAllText(fsPath, updated)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
        | Error error ->
            failwithf
                "EmitHotReloadDelta failed for computation-expression transformed-helper edit: %A"
                error
        | Ok delta ->
            Assert.Contains(viewToken, delta.UpdatedMethods)

            let updatedMethodDisplays =
                delta.UpdatedMethods
                |> List.map (getMethodDisplayByToken dllPath)
            let updatedMethodDisplayText = String.concat ", " updatedMethodDisplays

            Assert.True(
                updatedMethodDisplays |> List.exists (fun methodDisplay -> methodDisplay.Contains("@hotreload")),
                $"Expected synthesized helper method update for CE local-lambda edit. Updated methods: {updatedMethodDisplayText}")

        checker.EndHotReloadSession()

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``Type-provider erased usage edit updates user-authored method token`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-typeprovider-erased", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")
        let csharpAnalysisPath, testTypeProviderPath = getTypeProviderReferencePaths ()

        let baseline =
            """
namespace ProviderHotReload

type Provided = ErasedWithConstructor.Provided.MyType

module Demo =
    let render () =
        let provided = Provided()
        provided.DoNothing()
        "generation 0"
"""

        let updated =
            """
namespace ProviderHotReload

type Provided = ErasedWithConstructor.Provided.MyType

module Demo =
    let render () =
        let provided = Provided()
        provided.DoNothingOneArg()
        "generation 1"
"""

        File.WriteAllText(fsPath, baseline)

        let checker = createChecker ()

        let projectOptions =
            prepareProjectOptions checker fsPath dllPath baseline
            |> withReferences [ csharpAnalysisPath; testTypeProviderPath ]

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        let renderToken = getMethodToken dllPath "Demo" "render"
        File.WriteAllText(fsPath, updated)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "EmitHotReloadDelta failed for type-provider erased usage edit: %A" error
        | Ok delta -> Assert.Contains(renderToken, delta.UpdatedMethods)

        checker.EndHotReloadSession()
        Assert.False(checker.HotReloadSessionActive)

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``Type-provider generative static-argument change with unchanged usage yields no semantic delta`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-typeprovider-generative", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")
        let csharpAnalysisPath, testTypeProviderPath = getTypeProviderReferencePaths ()

        let baseline =
            """
namespace ProviderHotReload

type Generated = GeneratedWithConstructor.Provided.GenerativeProvider<3>

module Demo =
    let create () =
        let value = Generated()
        value.ToString()
"""

        let updated =
            """
namespace ProviderHotReload

type Generated = GeneratedWithConstructor.Provided.GenerativeProvider<4>

module Demo =
    let create () =
        let value = Generated()
        value.ToString()
"""

        File.WriteAllText(fsPath, baseline)

        let checker = createChecker ()

        let projectOptions =
            prepareProjectOptions checker fsPath dllPath baseline
            |> withReferences [ csharpAnalysisPath; testTypeProviderPath ]

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        File.WriteAllText(fsPath, updated)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
        | Ok _ ->
            failwith "Expected no semantic delta when only generative static argument changes and consumed IL is unchanged."
        | Error FSharpHotReloadError.NoChanges -> ()
        | Error error ->
            failwithf "Expected NoChanges for generative static-argument update, got: %A" error

        checker.EndHotReloadSession()
        Assert.False(checker.HotReloadSessionActive)

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``Type-provider generative usage edit updates user-authored method token`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-typeprovider-generative-usage", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")
        let csharpAnalysisPath, testTypeProviderPath = getTypeProviderReferencePaths ()

        let baseline =
            """
namespace ProviderHotReload

type Generated = GeneratedWithConstructor.Provided.GenerativeProvider<3>

module Demo =
    let create () =
        let value = Generated()
        value.ToString()
"""

        let updated =
            """
namespace ProviderHotReload

type Generated = GeneratedWithConstructor.Provided.GenerativeProvider<3>

module Demo =
    let create () =
        let value = Generated()
        value.ToString() + "!"
"""

        File.WriteAllText(fsPath, baseline)

        let checker = createChecker ()

        let projectOptions =
            prepareProjectOptions checker fsPath dllPath baseline
            |> withReferences [ csharpAnalysisPath; testTypeProviderPath ]

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        let createToken = getMethodToken dllPath "Demo" "create"
        File.WriteAllText(fsPath, updated)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "EmitHotReloadDelta failed for type-provider generative usage edit: %A" error
        | Ok delta -> Assert.Contains(createToken, delta.UpdatedMethods)

        checker.EndHotReloadSession()
        Assert.False(checker.HotReloadSessionActive)

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    [<Fact>]
    let ``Type-provider dependency timestamp invalidation keeps subsequent source edit hot-reloadable`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-typeprovider-dependency", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")
        let csharpAnalysisSourcePath, testTypeProviderSourcePath = getTypeProviderReferencePaths ()

        let tpDir = Path.Combine(projectDir, "tp")
        Directory.CreateDirectory(tpDir) |> ignore

        let csharpAnalysisPath = Path.Combine(tpDir, "CSharp_Analysis.dll")
        let testTypeProviderPath = Path.Combine(tpDir, "TestTP.dll")

        File.Copy(csharpAnalysisSourcePath, csharpAnalysisPath, overwrite = true)
        File.Copy(testTypeProviderSourcePath, testTypeProviderPath, overwrite = true)

        let baseline =
            """
namespace ProviderHotReload

type Provided = ErasedWithConstructor.Provided.MyType

module Demo =
    let render () =
        let provided = Provided()
        provided.DoNothing()
        "generation 0"
"""

        let updated =
            """
namespace ProviderHotReload

type Provided = ErasedWithConstructor.Provided.MyType

module Demo =
    let render () =
        let provided = Provided()
        provided.DoNothingOneArg()
        "generation 1"
"""

        File.WriteAllText(fsPath, baseline)

        let checker = createChecker ()

        let projectOptions =
            prepareProjectOptions checker fsPath dllPath baseline
            |> withReferences [ csharpAnalysisPath; testTypeProviderPath ]

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        let renderToken = getMethodToken dllPath "Demo" "render"

        // Simulate provider dependency refresh and force configuration invalidation before the source edit.
        let dependencyTimestamp = File.GetLastWriteTime(csharpAnalysisPath).AddSeconds(2.0)
        File.SetLastWriteTime(csharpAnalysisPath, dependencyTimestamp)
        checker.InvalidateConfiguration(projectOptions)
        compileProject checker projectOptions false

        File.WriteAllText(fsPath, updated)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        match checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate with
        | Error error ->
            failwithf
                "EmitHotReloadDelta failed after type-provider dependency invalidation: %A"
                error
        | Ok delta -> Assert.Contains(renderToken, delta.UpdatedMethods)

        checker.EndHotReloadSession()
        Assert.False(checker.HotReloadSessionActive)

        try
            Directory.Delete(projectDir, true)
        with _ -> ()

    // -------------------------------------------------------------------------
    // Rude Edit Rejection Tests
    // -------------------------------------------------------------------------
    // These tests verify that disallowed edits are properly rejected at the
    // FSharpChecker API level, returning UnsupportedEdit errors.

    let private signatureChangeBaseline =
        """
namespace Sample

type Type =
    static member GetValue(x: int) = x + 1
"""

    let private signatureChangeUpdated =
        """
namespace Sample

type Type =
    static member GetValue(x: string) = x.Length
"""

    [<Fact>]
    let ``EmitHotReloadDelta rejects signature change`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-sig-change", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        File.WriteAllText(fsPath, signatureChangeBaseline)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath signatureChangeBaseline

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        // Change the method signature (int -> string parameter)
        File.WriteAllText(fsPath, signatureChangeUpdated)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        let emitResult = checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate

        match emitResult with
        | Ok _ -> failwith "Expected signature change to be rejected"
        | Error (FSharpHotReloadError.UnsupportedEdit msg) ->
            Assert.Contains("Rude edits", msg, StringComparison.OrdinalIgnoreCase)
        | Error other -> failwithf "Expected UnsupportedEdit error, got: %A" other

        checker.EndHotReloadSession()
        try Directory.Delete(projectDir, true) with _ -> ()

    let private recordBaseline =
        """
namespace Sample

type Person = { Name: string }

module Helpers =
    let greet (p: Person) = $"Hello, {p.Name}"
"""

    let private recordWithNewField =
        """
namespace Sample

type Person = { Name: string; Age: int }

module Helpers =
    let greet (p: Person) = $"Hello, {p.Name}, age {p.Age}"
"""

    [<Fact>]
    let ``EmitHotReloadDelta rejects record field addition`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-record-field", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        File.WriteAllText(fsPath, recordBaseline)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath recordBaseline

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        // Add a new field to the record (type layout change)
        File.WriteAllText(fsPath, recordWithNewField)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        let emitResult = checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate

        match emitResult with
        | Ok _ -> failwith "Expected record field addition to be rejected"
        | Error (FSharpHotReloadError.UnsupportedEdit msg) ->
            // Should mention rude edits or structural edits
            Assert.True(
                msg.Contains("Rude", StringComparison.OrdinalIgnoreCase) ||
                msg.Contains("Structural", StringComparison.OrdinalIgnoreCase),
                $"Expected rude/structural edit message, got: {msg}")
        | Error other -> failwithf "Expected UnsupportedEdit error, got: %A" other

        checker.EndHotReloadSession()
        try Directory.Delete(projectDir, true) with _ -> ()

    let private moduleBaseline =
        """
namespace Sample

module Helpers =
    let getValue () = 42
"""

    let private moduleWithNewFunction =
        """
namespace Sample

module Helpers =
    let getValue () = 42
    let getOther () = 99
"""

    [<Fact>]
    let ``EmitHotReloadDelta rejects new function addition`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-func-add", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        File.WriteAllText(fsPath, moduleBaseline)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath moduleBaseline

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        // Add a new function (declaration added)
        File.WriteAllText(fsPath, moduleWithNewFunction)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        let emitResult = checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate

        match emitResult with
        | Ok _ -> failwith "Expected new function addition to be rejected"
        | Error (FSharpHotReloadError.UnsupportedEdit msg) ->
            // Should mention rude edits or structural edits
            Assert.True(
                msg.Contains("Rude", StringComparison.OrdinalIgnoreCase) ||
                msg.Contains("Structural", StringComparison.OrdinalIgnoreCase),
                $"Expected rude/structural edit message, got: {msg}")
        | Error other -> failwithf "Expected UnsupportedEdit error, got: %A" other

        checker.EndHotReloadSession()
        try Directory.Delete(projectDir, true) with _ -> ()

    let private unionBaseline =
        """
namespace Sample

type Shape =
    | Circle of radius: float
    | Square of side: float

module Shapes =
    let area shape =
        match shape with
        | Circle r -> System.Math.PI * r * r
        | Square s -> s * s
"""

    let private unionWithNewCase =
        """
namespace Sample

type Shape =
    | Circle of radius: float
    | Square of side: float
    | Triangle of base': float * height: float

module Shapes =
    let area shape =
        match shape with
        | Circle r -> System.Math.PI * r * r
        | Square s -> s * s
        | Triangle (b, h) -> 0.5 * b * h
"""

    [<Fact>]
    let ``EmitHotReloadDelta rejects union case addition`` () =
        let projectDir = Path.Combine(Path.GetTempPath(), "fcs-hotreload-union-case", Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(projectDir) |> ignore

        let fsPath = Path.Combine(projectDir, "Library.fs")
        let dllPath = Path.Combine(projectDir, "Library.dll")

        File.WriteAllText(fsPath, unionBaseline)

        let checker = createChecker ()
        let projectOptions = prepareProjectOptions checker fsPath dllPath unionBaseline

        checker.InvalidateAll()
        compileProject checker projectOptions true

        match checker.StartHotReloadSession(projectOptions) |> Async.RunImmediate with
        | Error error -> failwithf "Failed to start session: %A" error
        | Ok () -> ()

        // Add a new union case (type layout change)
        File.WriteAllText(fsPath, unionWithNewCase)
        checker.NotifyFileChanged(fsPath, projectOptions) |> Async.RunImmediate
        compileProject checker projectOptions false

        let emitResult = checker.EmitHotReloadDelta(projectOptions) |> Async.RunImmediate

        match emitResult with
        | Ok _ -> failwith "Expected union case addition to be rejected"
        | Error (FSharpHotReloadError.UnsupportedEdit msg) ->
            // Should mention rude edits or structural edits
            Assert.True(
                msg.Contains("Rude", StringComparison.OrdinalIgnoreCase) ||
                msg.Contains("Structural", StringComparison.OrdinalIgnoreCase),
                $"Expected rude/structural edit message, got: {msg}")
        | Error other -> failwithf "Expected UnsupportedEdit error, got: %A" other

        checker.EndHotReloadSession()
        try Directory.Delete(projectDir, true) with _ -> ()
