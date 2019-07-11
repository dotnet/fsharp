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
using System

namespace CSharpTest
{
    public interface ITest
    {
        void NonDefaultMethod();
        int NonDefaultProperty { get; set; }

        void DefaultMethod()
        {
            Console.WriteLine("DefaultMethod");
        }

        int DefaultGetProperty
        {
            get
            {
                return 0;
            }
        }

        int DefaultSetProperty
        {
            set
            {
            }
        }

        int DefaultGetSetProperty
        {
            get
            {
                return 0;
            }
            set
            {
            }
        }
    }
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open CSharpTest

type Test () =

    interface ITest with

        member __.NonDefaultMethod() = ()

        member __.NonDefaultProperty
            with get () = 0
            and set _ = ()
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.Preview, TargetFramework.NetCoreApp30)
        CompilerAssert.PassWithCSharpCompilation c fsharpSource