// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open System
open NUnit.Framework

[<TestFixture>]
module DefaultInterfaceMethodInteropTests =

    [<Test>]
    let SimpleCSharpDefaultInterfaceMethodTypeChecks() =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest
    {
        void NonDefaultMethod()
        {
            Console.WriteLine(nameof(NonDefaultMethod));
        }
    }
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open CSharpTest

type Test () =

    interface ITest
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.Compile (fsharpSource, (* TODO: change this to default *) "preview", c)