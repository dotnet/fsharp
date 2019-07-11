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

        let NetStandard = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.netstandard).GetReference(display = "netstandard.dll (netstandard 2.0 ref)")

        let MscorlibRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.mscorlib).GetReference(display = "mscorlib.dll (netstandard 2.0 ref)")

        let SystemRuntimeRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.System_Runtime).GetReference(display = "System.Runtime.dll (netstandard 2.0 ref)")

        let SystemCoreRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.System_Core).GetReference(display = "System.Core.dll (netstandard 2.0 ref)")

        let SystemDynamicRuntimeRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netstandard20.System_Dynamic_Runtime).GetReference(display = "System.Dynamic.Runtime.dll (netstandard 2.0 ref)")

    [<RequireQualifiedAccess>]
    module NetCoreApp30 =

        let NetStandard = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netcoreapp30.netstandard).GetReference(display = "netstandard.dll (netcoreapp 3.0 ref)")

        let MscorlibRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netcoreapp30.mscorlib).GetReference(display = "mscorlib.dll (netcoreapp 3.0 ref)")

        let SystemRuntimeRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netcoreapp30.System_Runtime).GetReference(display = "System.Runtime.dll (netcoreapp 3.0 ref)")

        let SystemCoreRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netcoreapp30.System_Core).GetReference(display = "System.Core.dll (netcoreapp 3.0 ref)")

        let SystemDynamicRuntimeRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netcoreapp30.System_Dynamic_Runtime).GetReference(display = "System.Dynamic.Runtime.dll (netcoreapp 3.0 ref)")

        let SystemConsoleRef = lazy AssemblyMetadata.CreateFromImage(TestResources.NetFX.netcoreapp30.System_Console).GetReference(display = "System.Console.dll (netcoreapp 3.0 ref)")
        

[<RequireQualifiedAccess>]
module TargetFrameworkUtil =

    open TestReferences

    let private NetStandard20References =
        lazy ImmutableArray.Create(NetStandard20.NetStandard.Value, NetStandard20.MscorlibRef.Value, NetStandard20.SystemRuntimeRef.Value, NetStandard20.SystemCoreRef.Value, NetStandard20.SystemDynamicRuntimeRef.Value)

    let private NetCoreApp30References =
        lazy ImmutableArray.Create(NetCoreApp30.NetStandard.Value, NetCoreApp30.MscorlibRef.Value, NetCoreApp30.SystemRuntimeRef.Value, NetCoreApp30.SystemCoreRef.Value, NetCoreApp30.SystemDynamicRuntimeRef.Value, NetCoreApp30.SystemConsoleRef.Value)

    let GetReferences tf =
        match tf with
        | TargetFramework.NetStandard20 -> NetStandard20References.Value
        | TargetFramework.NetCoreApp30 -> NetCoreApp30References.Value

[<AbstractClass; Sealed>]
type CompilationUtil private () =
    
    static member CreateCSharpCompilation (source: string, lv, ?tf, ?additionalReferences) =
        let tf = defaultArg tf TargetFramework.NetStandard20
        let additionalReferences = defaultArg additionalReferences ImmutableArray.Empty
        let references = TargetFrameworkUtil.GetReferences tf
        CSharpCompilation.Create(
            Guid.NewGuid().ToString (),
            [ CSharpSyntaxTree.ParseText (source, CSharpParseOptions lv) ],
            references.As<MetadataReference>().AddRange additionalReferences,
            CSharpCompilationOptions (OutputKind.DynamicallyLinkedLibrary))

type RoslynLanguageVersion = LanguageVersion