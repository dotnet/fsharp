// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test.Utilities

open System
open System.IO
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
        | NetCoreApp30

    module private TestReferences =

        [<RequireQualifiedAccess>]
        module NetStandard20 =
            let netStandard = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.netstandard).GetReference(display = "netstandard.dll (netstandard 2.0 ref)")
            let mscorlibRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.mscorlib).GetReference(display = "mscorlib.dll (netstandard 2.0 ref)")
            let systemRuntimeRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.System_Runtime).GetReference(display = "System.Runtime.dll (netstandard 2.0 ref)")
            let systemCoreRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.System_Core).GetReference(display = "System.Core.dll (netstandard 2.0 ref)")
            let systemDynamicRuntimeRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.System_Dynamic_Runtime).GetReference(display = "System.Dynamic.Runtime.dll (netstandard 2.0 ref)")

        [<RequireQualifiedAccess>]
        module NetCoreApp30 =
            let netStandard = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netcoreapp30.netstandard).GetReference(display = "netstandard.dll (netcoreapp 3.0 ref)")
            let mscorlibRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netcoreapp30.mscorlib).GetReference(display = "mscorlib.dll (netcoreapp 3.0 ref)")
            let systemRuntimeRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netcoreapp30.System_Runtime).GetReference(display = "System.Runtime.dll (netcoreapp 3.0 ref)")
            let systemCoreRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netcoreapp30.System_Core).GetReference(display = "System.Core.dll (netcoreapp 3.0 ref)")
            let systemDynamicRuntimeRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netcoreapp30.System_Dynamic_Runtime).GetReference(display = "System.Dynamic.Runtime.dll (netcoreapp 3.0 ref)")
            let systemConsoleRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netcoreapp30.System_Console).GetReference(display = "System.Console.dll (netcoreapp 3.0 ref)")

    [<RequireQualifiedAccess>]
    module private TargetFrameworkUtil =

        open TestReferences

        let private netStandard20References =
            lazy ImmutableArray.Create(NetStandard20.netStandard.Value, NetStandard20.mscorlibRef.Value, NetStandard20.systemRuntimeRef.Value, NetStandard20.systemCoreRef.Value, NetStandard20.systemDynamicRuntimeRef.Value)
        let private netCoreApp30References =
            lazy ImmutableArray.Create(NetCoreApp30.netStandard.Value, NetCoreApp30.mscorlibRef.Value, NetCoreApp30.systemRuntimeRef.Value, NetCoreApp30.systemCoreRef.Value, NetCoreApp30.systemDynamicRuntimeRef.Value, NetCoreApp30.systemConsoleRef.Value)

        let internal getReferences tf =
            match tf with
                | TargetFramework.NetStandard20 -> netStandard20References.Value
                | TargetFramework.NetCoreApp30 -> netCoreApp30References.Value

    type RoslynLanguageVersion = LanguageVersion

    [<Flags>]
    type CSharpCompilationFlags =
        | None = 0x0
        | InternalsVisibleTo = 0x1

    // TODO: this and Compilation.Compile needs to be merged for sake of consistency.
    // TODO: After merging, add new type of FSharp compilation.
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
