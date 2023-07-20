// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System.Collections.Immutable
open NUnit.Framework
open FSharp.Test
open FSharp.Test.Utilities
open Microsoft.CodeAnalysis

[<TestFixture>]
module StaticNameResolutionTests =

    [<Test>]
    let ``C# nested type is accessible even when extension method is present with same name``() =
        let csSrc =
            """
namespace CSharpLib
{
    public static class Extensions
    {
        // Note instance extension member has same name as nested type
        static public T cuda<T>(this T x) { return x; }
    }

    public class torch
    {
        // Note instance extension member has same name as nested type
        public class cuda
        {
            public static bool isAvailable() { return true; }
            public bool isAvailable2() { return true; }
        }
    }
}
           """

        let fsSrc =
            """
open CSharpLib

let res1 : bool = torch.cuda.isAvailable()
let res2 : bool = torch.cuda().isAvailable2()
let res3 : bool = torch.cuda().cuda().isAvailable2()
            """

        let fsharpCoreAssembly =
            typeof<_ list>.Assembly.Location
            |> MetadataReference.CreateFromFile

        let cs =
            CompilationUtil.CreateCSharpCompilation(csSrc, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31, additionalReferences = ImmutableArray.CreateRange [fsharpCoreAssembly])
            |> CompilationReference.Create

        let fs = Compilation.Create(fsSrc, CompileOutput.Exe, options = [| |], cmplRefs = [cs])
        CompilerAssert.Compile fs

