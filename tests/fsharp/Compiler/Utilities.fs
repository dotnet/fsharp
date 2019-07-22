// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System
open System.Collections.Immutable
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp

// This file mimics how Roslyn handles their compilation references for compilation testing

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

    let getReferences tf =
        match tf with
        | TargetFramework.NetStandard20 -> netStandard20References.Value
        | TargetFramework.NetCoreApp30 -> netCoreApp30References.Value

type RoslynLanguageVersion = LanguageVersion

[<AbstractClass; Sealed>]
type CompilationUtil private () =
    
    static member CreateCSharpCompilation (source: string, lv: RoslynLanguageVersion, ?tf, ?additionalReferences) =
        let tf = defaultArg tf TargetFramework.NetStandard20
        let additionalReferences = defaultArg additionalReferences ImmutableArray.Empty
        let references = TargetFrameworkUtil.getReferences tf
        CSharpCompilation.Create(
            Guid.NewGuid().ToString (),
            [ CSharpSyntaxTree.ParseText (source, CSharpParseOptions lv) ],
            references.As<MetadataReference>().AddRange additionalReferences,
            CSharpCompilationOptions (OutputKind.DynamicallyLinkedLibrary))