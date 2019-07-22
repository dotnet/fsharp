// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

#if NETCOREAPP

[<TestFixture>]
module DefaultInterfaceMethodInteropTests =

    [<Test>]
    let ``C# DIM Consumption - Errors with un-implemented non-DIM`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest
    {
        void DefaultMethod()
        {
            Console.Write(nameof(DefaultMethod));
        }

        void NonDefaultMethod();
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
        CompilerAssert.HasTypeCheckErrors (fsharpSource, c, [
            {
                Number = 366
                StartLine = 8
                StartColumn = 14
                EndLine = 8
                EndColumn = 19
                Message = "No implementation was given for 'ITest.NonDefaultMethod() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'."
            }
        ])

    [<Test>]
    let ``C# DIM Consumption - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest
    {
        void DefaultMethod()
        {
            Console.Write(nameof(DefaultMethod));
        }

        void NonDefaultMethod();
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type Test () =

    interface ITest with

        member __.NonDefaultMethod () =
            Console.Write("NonDefaultMethod")

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest
    test.DefaultMethod ()
    Console.Write("-")
    test.NonDefaultMethod ()
    0
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.CompileExeAndRun (fsharpSource, c, "DefaultMethod-NonDefaultMethod")

    [<Test>]
    let ``C# DIM Consumption with override - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest
    {
        void DefaultMethod()
        {
            Console.Write(nameof(DefaultMethod));
        }

        void NonDefaultMethod();
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type Test () =

    interface ITest with

        member __.DefaultMethod () =
            Console.Write("OverrideDefaultMethod")

        member __.NonDefaultMethod () =
            Console.Write("NonDefaultMethod")

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest
    test.DefaultMethod ()
    Console.Write("-")
    test.NonDefaultMethod ()
    0
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.CompileExeAndRun (fsharpSource, c, "OverrideDefaultMethod-NonDefaultMethod")

    [<Test>]
    let ``C# Complex DIM Consumption with override - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest1
    {
        void Method1()
        {
            Console.Write(nameof(Method1));
        }

        void Method2();
    }

    public interface ITest2 : ITest1
    {
        void ITest1.Method1()
        {
            Console.Write("FromITest2-" + nameof(Method1));
        }

        void ITest1.Method2()
        {
            Console.Write("FromITest2-" + nameof(Method2));
        }
    }

    public class CSharpTestClass : ITest2
    {
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type Test () =

    interface ITest2

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest1
    test.Method1 ()
    Console.Write("-")
    test.Method2 ()
    0
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.CompileExeAndRun (fsharpSource, c, "FromITest2-Method1-FromITest2-Method2")

#endif