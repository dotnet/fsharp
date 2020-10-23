// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System.Collections.Immutable
open NUnit.Framework
open FSharp.Test.Utilities
open FSharp.Test.Utilities.Utilities
open FSharp.Compiler.SourceCodeServices
open Microsoft.CodeAnalysis

[<TestFixture>]
module OptionalInteropTests =

    [<Test>]
    let ``C# method with an optional parameter and called with an option type should compile`` () =
        let csSrc =
            """
using Microsoft.FSharp.Core;

namespace CSharpTest
{
    public static class Test
    {
        public static void M(FSharpOption<int> x = null) { }
    }
}
            """

        let fsSrc =
            """
open System
open CSharpTest

Test.M(x = Some 1)
            """

        let fsharpCoreAssembly =
            typeof<_ list>.Assembly.Location
            |> MetadataReference.CreateFromFile

        let cs =
            CompilationUtil.CreateCSharpCompilation(csSrc, CSharpLanguageVersion.CSharp8, TargetFramework.NetStandard20, additionalReferences = ImmutableArray.CreateRange [fsharpCoreAssembly])
            |> CompilationReference.Create

        let fs = Compilation.Create(fsSrc, SourceKind.Fsx, CompileOutput.Exe, options = [|"--langversion:5.0"|], cmplRefs = [cs])
        CompilerAssert.Compile fs
