﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test.Utilities

open System
open System.IO
open System.Reflection
open System.Collections.Immutable
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open System.Diagnostics
open FSharp.Test.Utilities

// This file mimics how Roslyn handles their compilation references for compilation testing

module Utilities =

    [<RequireQualifiedAccess>]
    type TargetFramework =
        | NetStandard20
        | NetCoreApp31

    let private getResourceStream name =
        let assembly = typeof<TargetFramework>.GetTypeInfo().Assembly

        let stream = assembly.GetManifestResourceStream(name);

        match stream with
        | null -> failwith (sprintf "Resource '%s' not found in %s." name assembly.FullName)
        | _ -> stream

    let private getResourceBlob name =
        use stream = getResourceStream name
        let (bytes: byte[]) = Array.zeroCreate (int stream.Length)
        use memoryStream = new MemoryStream (bytes)
        stream.CopyTo(memoryStream)
        bytes

    let private getOrCreateResource (resource: byref<byte[]>) (name: string) =
        match resource with
        | null -> getResourceBlob name
        | _ -> resource

    module private TestReferences =
        [<RequireQualifiedAccess>]
        module NetStandard20 =
            let netStandard = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.netstandard).GetReference(display = "netstandard.dll (netstandard 2.0 ref)")
            let mscorlibRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.mscorlib).GetReference(display = "mscorlib.dll (netstandard 2.0 ref)")
            let systemRuntimeRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.System_Runtime).GetReference(display = "System.Runtime.dll (netstandard 2.0 ref)")
            let systemCoreRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.System_Core).GetReference(display = "System.Core.dll (netstandard 2.0 ref)")
            let systemDynamicRuntimeRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.System_Dynamic_Runtime).GetReference(display = "System.Dynamic.Runtime.dll (netstandard 2.0 ref)")


        module private NetCoreApp31Refs =
            let mutable (_mscorlib: byte[]) = Unchecked.defaultof<byte[]>
            let mutable (_netstandard: byte[]) = Unchecked.defaultof<byte[]>
            let mutable (_System_Console: byte[]) = Unchecked.defaultof<byte[]>
            let mutable (_System_Core: byte[]) = Unchecked.defaultof<byte[]>
            let mutable (_System_Dynamic_Runtime: byte[]) = Unchecked.defaultof<byte[]>
            let mutable (_System_Runtime: byte[]) = Unchecked.defaultof<byte[]>
            let mscorlib () = getOrCreateResource &_mscorlib "mscorlib.dll"
            let netstandard () = getOrCreateResource &_netstandard "netstandard.dll"
            let System_Core () = getOrCreateResource &_System_Core "System.Core.dll"
            let System_Console () = getOrCreateResource &_System_Console "System.Console.dll"
            let System_Runtime () = getOrCreateResource &_System_Runtime "System.Runtime.dll"
            let System_Dynamic_Runtime () = getOrCreateResource &_System_Dynamic_Runtime "System.Dynamic.Runtime.dll"

        [<RequireQualifiedAccess>]
        module NetCoreApp31 =
            let netStandard = lazy AssemblyMetadata.CreateFromImage(NetCoreApp31Refs.netstandard ()).GetReference(display = "netstandard.dll (netcoreapp 3.1 ref)")
            let mscorlibRef = lazy AssemblyMetadata.CreateFromImage(NetCoreApp31Refs.mscorlib ()).GetReference(display = "mscorlib.dll (netcoreapp 3.1 ref)")
            let systemRuntimeRef = lazy AssemblyMetadata.CreateFromImage(NetCoreApp31Refs.System_Runtime ()).GetReference(display = "System.Runtime.dll (netcoreapp 3.1 ref)")
            let systemCoreRef = lazy AssemblyMetadata.CreateFromImage(NetCoreApp31Refs.System_Core ()).GetReference(display = "System.Core.dll (netcoreapp 3.1 ref)")
            let systemDynamicRuntimeRef = lazy AssemblyMetadata.CreateFromImage(NetCoreApp31Refs.System_Dynamic_Runtime ()).GetReference(display = "System.Dynamic.Runtime.dll (netcoreapp 3.1 ref)")
            let systemConsoleRef = lazy AssemblyMetadata.CreateFromImage(NetCoreApp31Refs.System_Console ()).GetReference(display = "System.Console.dll (netcoreapp 3.1 ref)")

    [<RequireQualifiedAccess>]
    module TargetFrameworkUtil =

        open TestReferences

        let private netStandard20References =
            lazy ImmutableArray.Create(NetStandard20.netStandard.Value, NetStandard20.mscorlibRef.Value, NetStandard20.systemRuntimeRef.Value, NetStandard20.systemCoreRef.Value, NetStandard20.systemDynamicRuntimeRef.Value)
        let private netCoreApp31References =
            lazy ImmutableArray.Create(NetCoreApp31.netStandard.Value, NetCoreApp31.mscorlibRef.Value, NetCoreApp31.systemRuntimeRef.Value, NetCoreApp31.systemCoreRef.Value, NetCoreApp31.systemDynamicRuntimeRef.Value, NetCoreApp31.systemConsoleRef.Value)

        let getReferences tf =
            match tf with
                | TargetFramework.NetStandard20 -> netStandard20References.Value
                | TargetFramework.NetCoreApp31 -> netCoreApp31References.Value

    type RoslynLanguageVersion = LanguageVersion

    [<Flags>]
    type CSharpCompilationFlags =
        | None = 0x0
        | InternalsVisibleTo = 0x1

    [<RequireQualifiedAccess>]
    type TestCompilation =
        | CSharp of CSharpCompilation
        | IL of ilSource: string * result: Lazy<string * byte []>

        member this.AssertNoErrorsOrWarnings () =
            match this with
                | TestCompilation.CSharp c ->
                    let diagnostics = c.GetDiagnostics ()

                    if not diagnostics.IsEmpty then
                        NUnit.Framework.Assert.Fail ("CSharp source diagnostics:\n" + (diagnostics |> Seq.map (fun x -> x.GetMessage () + "\n") |> Seq.reduce (+)))

                | TestCompilation.IL (_, result) ->
                    let errors, _ = result.Value
                    if errors.Length > 0 then
                        NUnit.Framework.Assert.Fail ("IL source errors: " + errors)

        member this.EmitAsFile (outputPath: string) =
            match this with
                | TestCompilation.CSharp c ->
                    let c = c.WithAssemblyName(Path.GetFileNameWithoutExtension outputPath)
                    let _x = c.References
                    let emitResult = c.Emit outputPath
                    if not emitResult.Success then
                        failwithf "Unable to emit C# compilation.\n%A" emitResult.Diagnostics

                | TestCompilation.IL (_, result) ->
                    let (_, data) = result.Value
                    File.WriteAllBytes (outputPath, data)

    type CSharpLanguageVersion =
        | CSharp8 = 0

    [<AbstractClass; Sealed>]
    type CompilationUtil private () =

        static member CreateCSharpCompilation (source: string, lv: CSharpLanguageVersion, ?tf, ?additionalReferences, ?name) =
            let lv =
                match lv with
                    | CSharpLanguageVersion.CSharp8 -> LanguageVersion.CSharp8
                    | _ -> LanguageVersion.Default

            let tf = defaultArg tf TargetFramework.NetStandard20
            let n = defaultArg name (Guid.NewGuid().ToString ())
            let additionalReferences = defaultArg additionalReferences ImmutableArray.Empty
            let references = TargetFrameworkUtil.getReferences tf
            let c =
                CSharpCompilation.Create(
                    n,
                    [ CSharpSyntaxTree.ParseText (source, CSharpParseOptions lv) ],
                    references.As<MetadataReference>().AddRange additionalReferences,
                    CSharpCompilationOptions (OutputKind.DynamicallyLinkedLibrary))
            TestCompilation.CSharp c

        static member CreateILCompilation (source: string) =
            let compute =
                lazy
                    let ilFilePath = Path.GetTempFileName ()
                    let tmp = Path.GetTempFileName()
                    let dllFilePath = Path.ChangeExtension (tmp, ".dll")
                    try
                        File.WriteAllText (ilFilePath, source)
                        let errors = ILChecker.reassembleIL ilFilePath dllFilePath
                        try
                           (errors, File.ReadAllBytes dllFilePath)
                        with
                            | _ -> (errors, [||])
                    finally
                        try File.Delete ilFilePath with | _ -> ()
                        try File.Delete tmp with | _ -> ()
                        try File.Delete dllFilePath with | _ -> ()
            TestCompilation.IL (source, compute)

    /// Disposable type to implement a simple resolve handler that searches the currently loaded assemblies to see if the requested assembly is already loaded.
    type AlreadyLoadedAppDomainResolver () =
        let resolveHandler =
            ResolveEventHandler(fun _ args ->
                let assemblies = AppDomain.CurrentDomain.GetAssemblies()
                let assembly = assemblies |> Array.tryFind(fun a -> String.Compare(a.FullName, args.Name,StringComparison.OrdinalIgnoreCase) = 0)
                assembly |> Option.defaultValue Unchecked.defaultof<Assembly>
                )
        do AppDomain.CurrentDomain.add_AssemblyResolve(resolveHandler)

        interface IDisposable with
            member this.Dispose() = AppDomain.CurrentDomain.remove_AssemblyResolve(resolveHandler)


