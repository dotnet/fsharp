// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

#if NETCOREAPP

[<TestFixture>]
module DefaultInterfaceMethodTests =

    [<Test>]
    let ``C# simple consumption - Errors with lang version not supported`` () =
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
                Number = 3302
                StartLine = 8
                StartColumn = 14
                EndLine = 8
                EndColumn = 19
                Message = "Feature 'default interface methods interop' is not available in F# 4.6. Please use language version 4.7 or greater."
            }
            {
                Number = 366;
                StartLine = 8;
                StartColumn = 14;
                EndLine = 8;
                EndColumn = 19;
                Message = "No implementation was given for those members: 
	'ITest.DefaultMethod() : unit'
	'ITest.NonDefaultMethod() : unit'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'."
            }
        ], fsharpLanguageVersion = "4.6")

    [<Test>]
    let ``C# simple consumption - Errors with lang version not supported - 2`` () =
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

    interface ITest with

        member __.NonDefaultMethod () = ()
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.HasTypeCheckErrors (fsharpSource, c, [
            {
                Number = 3302
                StartLine = 8
                StartColumn = 14
                EndLine = 8
                EndColumn = 19
                Message = "Feature 'default interface methods interop' is not available in F# 4.6. Please use language version 4.7 or greater."
            }
            {
                Number = 366;
                StartLine = 8;
                StartColumn = 14;
                EndLine = 8;
                EndColumn = 19;
                Message = "No implementation was given for 'ITest.DefaultMethod() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'."
            }
        ], fsharpLanguageVersion = "4.6")

    [<Test>]
    let ``C# simple consumption - Errors with lang version not supported - 3`` () =
        let csharpSource =
            """
namespace CSharpTest
{
    public interface ITest
    {
        void Method1()
        {
        }

        void Method2()
        {
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
        CompilerAssert.HasTypeCheckErrors (fsharpSource, c, [
            {
                Number = 3302
                StartLine = 8
                StartColumn = 14
                EndLine = 8
                EndColumn = 19
                Message = "Feature 'default interface methods interop' is not available in F# 4.6. Please use language version 4.7 or greater."
            }
            {
                Number = 3302
                StartLine = 8
                StartColumn = 14
                EndLine = 8
                EndColumn = 19
                Message = "Feature 'default interface methods interop' is not available in F# 4.6. Please use language version 4.7 or greater."
            }
            {
                Number = 366;
                StartLine = 8;
                StartColumn = 14;
                EndLine = 8;
                EndColumn = 19;
                Message = "No implementation was given for those members: 
	'ITest.Method1() : unit'
	'ITest.Method2() : unit'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'."
            }
        ], fsharpLanguageVersion = "4.6")

    [<Test>]
    let ``C# simple consumption - Errors with un-implemented non-DIM`` () =
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
    let ``C# simple consumption - Runs`` () =
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
    let ``C# simple consumption with one DIM for F# object expression - Runs`` () =
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
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

[<EntryPoint>]
let main _ =
    let test = { new ITest }
    test.DefaultMethod ()
    0
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.CompileExeAndRun (fsharpSource, c, "DefaultMethod")

    [<Test>]
    let ``C# simple consumption with one DIM and one non-DIM for F# object expression - Runs`` () =
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

[<EntryPoint>]
let main _ =
    let test = { new ITest with member __.NonDefaultMethod () = Console.Write("ObjExpr") }
    test.DefaultMethod ()
    Console.Write("-")
    test.NonDefaultMethod();
    0
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.CompileExeAndRun (fsharpSource, c, "DefaultMethod-ObjExpr")

    [<Test>]
    let ``C# simple consumption with one DIM and one non-DIM for F# object expression - Errors with lack of implementation`` () =
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
module FSharpTest

open System
open CSharpTest

let test = { new ITest }
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.HasTypeCheckErrors (fsharpSource, c, [
            {
                Number = 366
                StartLine = 7
                StartColumn = 11
                EndLine = 7
                EndColumn = 24
                Message = "No implementation was given for 'ITest.NonDefaultMethod() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'."
            }
        ])

    [<Test>]
    let ``C# simple consumption with override - Runs`` () =
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
    let ``C# simple consumption with override for object expression - Runs`` () =
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

[<EntryPoint>]
let main _ =
    let test =
        { new ITest with
            member __.DefaultMethod () =
                Console.Write("ObjExprOverrideDefaultMethod")
            member __.NonDefaultMethod () =
                Console.Write("ObjExprNonDefaultMethod") }
    test.DefaultMethod ()
    Console.Write("-")
    test.NonDefaultMethod ()
    0
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.CompileExeAndRun (fsharpSource, c, "ObjExprOverrideDefaultMethod-ObjExprNonDefaultMethod")

    [<Test>]
    let ``C# consumption from hierarchical interfaces - Runs`` () =
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

    [<Test>]
    let ``C# consumption from two separate hierarchical interfaces - Errors with lack of explicit shared interface type`` () =
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

    public interface ITest3 : ITest1
    {
        void ITest1.Method1()
        {
            Console.Write("FromITest3-" + nameof(Method1));
        }

        void ITest1.Method2()
        {
            Console.Write("FromITest3-" + nameof(Method2));
        }
    }
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open System
open CSharpTest

type Test () =

    interface ITest2
    interface ITest3
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.HasTypeCheckErrors (fsharpSource, c, [
            {
                Number = 363
                StartLine = 9
                StartColumn = 14
                EndLine = 9
                EndColumn = 20
                Message = "The interface 'ITest1' is included in multiple explicitly implemented interface types. Add an explicit implementation of this interface."
            }
            {
                Number = 3304
                StartLine = 10
                StartColumn = 14
                EndLine = 10
                EndColumn = 20
                Message = "Interface member 'ITest1.Method1() : unit' does not have a most specific implementation."
            }
            {
                Number = 3304
                StartLine = 10
                StartColumn = 14
                EndLine = 10
                EndColumn = 20
                Message = "Interface member 'ITest1.Method2() : unit' does not have a most specific implementation."
            }
            {
                Number = 3304
                StartLine = 9
                StartColumn = 14
                EndLine = 9
                EndColumn = 20
                Message = "Interface member 'ITest1.Method1() : unit' does not have a most specific implementation."
            }
            {
                Number = 3304
                StartLine = 9
                StartColumn = 14
                EndLine = 9
                EndColumn = 20
                Message = "Interface member 'ITest1.Method2() : unit' does not have a most specific implementation."
            }
        ])

    [<Test>]
    let ``C# consumption from two separate hierarchical interfaces - Errors with no most specific implementation`` () =
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

    public interface ITest3 : ITest1
    {
        void ITest1.Method1()
        {
            Console.Write("FromITest3-" + nameof(Method1));
        }

        void ITest1.Method2()
        {
            Console.Write("FromITest3-" + nameof(Method2));
        }
    }
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open System
open CSharpTest

type Test () =

    interface ITest1
    interface ITest2
    interface ITest3
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.HasTypeCheckErrors (fsharpSource, c, [
            {
                Number = 3304
                StartLine = 9
                StartColumn = 14
                EndLine = 9
                EndColumn = 20
                Message = "Interface member 'ITest1.Method1() : unit' does not have a most specific implementation."
            }
            {
                Number = 3304
                StartLine = 9
                StartColumn = 14
                EndLine = 9
                EndColumn = 20
                Message = "Interface member 'ITest1.Method2() : unit' does not have a most specific implementation."
            }
        ])

    [<Test>]
    let ``C# consumption from two separate hierarchical interfaces but combined in one C# interface - Errors with no most specific implementation`` () =
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

    public interface ITest3 : ITest1
    {
        void ITest1.Method1()
        {
            Console.Write("FromITest3-" + nameof(Method1));
        }

        void ITest1.Method2()
        {
            Console.Write("FromITest3-" + nameof(Method2));
        }
    }

    public interface ICombinedTest : ITest2, ITest3
    {
    }
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open System
open CSharpTest

type Test () =

    interface ICombinedTest
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.HasTypeCheckErrors (fsharpSource, c, [
            {
                Number = 3304
                StartLine = 9
                StartColumn = 14
                EndLine = 9
                EndColumn = 27
                Message = "Interface member 'ITest1.Method1() : unit' does not have a most specific implementation."
            }
            {
                Number = 3304
                StartLine = 9
                StartColumn = 14
                EndLine = 9
                EndColumn = 27
                Message = "Interface member 'ITest1.Method2() : unit' does not have a most specific implementation."
            }
        ])

    [<Test>]
    let ``C# consumption from two separate hierarchical interfaces but combined in one F# interface - Errors with no most specific implementation`` () =
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

    public interface ITest3 : ITest1
    {
        void ITest1.Method1()
        {
            Console.Write("FromITest3-" + nameof(Method1));
        }

        void ITest1.Method2()
        {
            Console.Write("FromITest3-" + nameof(Method2));
        }
    }
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open System
open CSharpTest

type ICombinedTest =
    inherit ITest2
    inherit ITest3

type Test () =

    interface ICombinedTest
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.HasTypeCheckErrors (fsharpSource, c, [
            {
                Number = 3304
                StartLine = 13
                StartColumn = 14
                EndLine = 13
                EndColumn = 27
                Message = "Interface member 'ITest1.Method1() : unit' does not have a most specific implementation."
            }
            {
                Number = 3304
                StartLine = 13
                StartColumn = 14
                EndLine = 13
                EndColumn = 27
                Message = "Interface member 'ITest1.Method2() : unit' does not have a most specific implementation."
            }
        ])


    [<Test>]
    let ``C# consumption from two separate hierarchical interfaces but re-abstracted in one and then combined in one F# interface - Errors with no most specific implementation`` () =
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

    public interface ITest3 : ITest1
    {
        abstract void ITest1.Method1();

        abstract void ITest1.Method2();
    }
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open System
open CSharpTest

type ICombinedTest =
    inherit ITest2
    inherit ITest3

type Test () =

    interface ICombinedTest
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.HasTypeCheckErrors (fsharpSource, c, [
            {
                Number = 3304
                StartLine = 13
                StartColumn = 14
                EndLine = 13
                EndColumn = 27
                Message = "Interface member 'ITest1.Method1() : unit' does not have a most specific implementation."
            }
            {
                Number = 3304
                StartLine = 13
                StartColumn = 14
                EndLine = 13
                EndColumn = 27
                Message = "Interface member 'ITest1.Method2() : unit' does not have a most specific implementation."
            }
        ])

    [<Test>]
    let ``C# consumption from two separate hierarchical interfaces but all re-abstracted and then combined in one F# interface - Errors with need to implement members`` () =
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
        abstract void ITest1.Method1();

        abstract void ITest1.Method2();
    }

    public interface ITest3 : ITest1
    {
        abstract void ITest1.Method1();

        abstract void ITest1.Method2();
    }
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open System
open CSharpTest

type ICombinedTest =
    inherit ITest2
    inherit ITest3

type Test () =

    interface ICombinedTest
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.HasTypeCheckErrors (fsharpSource, c, [
            {
                Number = 366
                StartLine = 13
                StartColumn = 14
                EndLine = 13
                EndColumn = 27
                Message = "No implementation was given for those members: 
	'ITest1.Method1() : unit'
	'ITest1.Method2() : unit'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'."
            }
        ])

    [<Test>]
    let ``C# consumption from two separate hierarchical interfaces then combined in one F# interface and then implemented - Runs`` () =
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

    public interface ITest3 : ITest1
    {
        void ITest1.Method1()
        {
            Console.Write("FromITest3-" + nameof(Method1));
        }

        void ITest1.Method2()
        {
            Console.Write("FromITest3-" + nameof(Method2));
        }
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type ICombinedTest =
    inherit ITest1
    inherit ITest2
    inherit ITest3

type Test () =

    interface ICombinedTest with

        member __.Method1 () = Console.Write("FSharpICombinedTest-Method1")

        member __.Method2 () = Console.Write("FSharpICombinedTest-Method2")

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest3
    test.Method1 ()
    Console.Write("-")
    test.Method2 ()
    0
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.CompileExeAndRun (fsharpSource, c, "FSharpICombinedTest-Method1-FSharpICombinedTest-Method2")

    [<Test>]
    let ``C# consumption from two separate hierarchical interfaces but all re-abstracted and then combined in one F# interface and then implemented - Runs`` () =
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
        abstract void ITest1.Method1();

        abstract void ITest1.Method2();
    }

    public interface ITest3 : ITest1
    {
        abstract void ITest1.Method1();

        abstract void ITest1.Method2();
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type ICombinedTest =
    inherit ITest1
    inherit ITest2
    inherit ITest3

type Test () =

    interface ICombinedTest with

        member __.Method1 () = Console.Write("FSharpICombinedTest-Method1")

        member __.Method2 () = Console.Write("FSharpICombinedTest-Method2")

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest2
    test.Method1 ()
    Console.Write("-")
    test.Method2 ()
    0
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.CompileExeAndRun (fsharpSource, c, "FSharpICombinedTest-Method1-FSharpICombinedTest-Method2")

    [<Test>]
    let ``C# consumption from two separate hierarchical interfaces then combined in one C# interface and then implemented - Runs`` () =
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

    public interface ITest3 : ITest1
    {
        void ITest1.Method1()
        {
            Console.Write("FromITest3-" + nameof(Method1));
        }

        void ITest1.Method2()
        {
            Console.Write("FromITest3-" + nameof(Method2));
        }
    }

    public interface ICombinedTest : ITest3, ITest2
    {
        void ITest1.Method1()
        {
            Console.Write("CSharpICombinedTest-" + nameof(Method1));
        }

        void ITest1.Method2()
        {
            Console.Write("CSharpICombinedTest-" + nameof(Method2));
        }
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type Test () =

    interface ICombinedTest

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest1
    test.Method1 ()
    Console.Write("-")
    test.Method2 ()
    0
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.CompileExeAndRun (fsharpSource, c, "CSharpICombinedTest-Method1-CSharpICombinedTest-Method2")

#else

[<TestFixture>]
module DefaultInterfaceMethodTests =

    [<Test>]
    let ``C# simple consumption - Errors with lang version and target runtime not supported`` () =
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
                Number = 3302
                StartLine = 8
                StartColumn = 14
                EndLine = 8
                EndColumn = 19
                Message = "Feature 'default interface methods interop' is not available in F# 4.6. Please use language version 4.7 or greater."
            }
            {
                Number = 3303
                StartLine = 8
                StartColumn = 14
                EndLine = 8
                EndColumn = 19
                Message = "Feature 'default interface methods interop' is not supported by target runtime."
            }
            {
                Number = 366;
                StartLine = 8;
                StartColumn = 14;
                EndLine = 8;
                EndColumn = 19;
                Message = "No implementation was given for those members: 
	'ITest.DefaultMethod() : unit'
	'ITest.NonDefaultMethod() : unit'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'."
            }
        ], fsharpLanguageVersion = "4.6")

    [<Test>]
    let ``C# simple consumption - Errors with lang version and target runtime not supported - 2`` () =
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

    interface ITest with

        member __.NonDefaultMethod () = ()
            """

        let c = CompilationUtil.CreateCSharpCompilation (csharpSource, RoslynLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
        CompilerAssert.HasTypeCheckErrors (fsharpSource, c, [
            {
                Number = 3302
                StartLine = 8
                StartColumn = 14
                EndLine = 8
                EndColumn = 19
                Message = "Feature 'default interface methods interop' is not available in F# 4.6. Please use language version 4.7 or greater."
            }
            {
                Number = 3303
                StartLine = 8
                StartColumn = 14
                EndLine = 8
                EndColumn = 19
                Message = "Feature 'default interface methods interop' is not supported by target runtime."
            }
            {
                Number = 366;
                StartLine = 8;
                StartColumn = 14;
                EndLine = 8;
                EndColumn = 19;
                Message = "No implementation was given for 'ITest.DefaultMethod() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'."
            }
        ], fsharpLanguageVersion = "4.6")

    [<Test>]
    let ``C# simple consumption - Errors with lang version and target runtime not supported - 3`` () =
        let csharpSource =
            """
namespace CSharpTest
{
    public interface ITest
    {
        void Method1()
        {
        }

        void Method2()
        {
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
        CompilerAssert.HasTypeCheckErrors (fsharpSource, c, [
            {
                Number = 3302
                StartLine = 8
                StartColumn = 14
                EndLine = 8
                EndColumn = 19
                Message = "Feature 'default interface methods interop' is not available in F# 4.6. Please use language version 4.7 or greater."
            }
            {
                Number = 3303
                StartLine = 8
                StartColumn = 14
                EndLine = 8
                EndColumn = 19
                Message = "Feature 'default interface methods interop' is not supported by target runtime."
            }
            {
                Number = 3302
                StartLine = 8
                StartColumn = 14
                EndLine = 8
                EndColumn = 19
                Message = "Feature 'default interface methods interop' is not available in F# 4.6. Please use language version 4.7 or greater."
            }
            {
                Number = 3303
                StartLine = 8
                StartColumn = 14
                EndLine = 8
                EndColumn = 19
                Message = "Feature 'default interface methods interop' is not supported by target runtime."
            }
            {
                Number = 366;
                StartLine = 8;
                StartColumn = 14;
                EndLine = 8;
                EndColumn = 19;
                Message = "No implementation was given for those members: 
	'ITest.Method1() : unit'
	'ITest.Method2() : unit'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'."
            }
        ], fsharpLanguageVersion = "4.6")

    [<Test>]
    let ``C# simple consumption - Errors with target runtime not supported`` () =
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
                Number = 3303
                StartLine = 8
                StartColumn = 14
                EndLine = 8
                EndColumn = 19
                Message = "Feature 'default interface methods interop' is not supported by target runtime."
            }
            {
                Number = 366;
                StartLine = 8;
                StartColumn = 14;
                EndLine = 8;
                EndColumn = 19;
                Message = "No implementation was given for 'ITest.DefaultMethod() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'."
            }
        ])

    [<Test>]
    let ``IL simple consumption - not valid target runtime or lang version but has explicit implementation - Runs`` () =
        let ilSource =
            """
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )
  .ver 4:2:1:0
}
.assembly ClassLibrary1
{
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                                   63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .custom instance void [System.Runtime]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = ( 01 00 18 2E 4E 45 54 43 6F 72 65 41 70 70 2C 56   // ....NETCoreApp,V
                                                                                                              65 72 73 69 6F 6E 3D 76 33 2E 30 01 00 54 0E 14   // ersion=v3.0..T..
                                                                                                              46 72 61 6D 65 77 6F 72 6B 44 69 73 70 6C 61 79   // FrameworkDisplay
                                                                                                              4E 61 6D 65 00 )                                  // Name.
  .custom instance void [System.Runtime]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 0D 43 6C 61 73 73 4C 69 62 72 61 72 79 31   // ...ClassLibrary1
                                                                                                      00 00 ) 
  .custom instance void [System.Runtime]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 07 52 65 6C 65 61 73 65 00 00 )             // ...Release..
  .custom instance void [System.Runtime]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )             // ...1.0.0.0..
  .custom instance void [System.Runtime]System.Reflection.AssemblyInformationalVersionAttribute::.ctor(string) = ( 01 00 05 31 2E 30 2E 30 00 00 )                   // ...1.0.0..
  .custom instance void [System.Runtime]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 0D 43 6C 61 73 73 4C 69 62 72 61 72 79 31   // ...ClassLibrary1
                                                                                                      00 00 ) 
  .custom instance void [System.Runtime]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 0D 43 6C 61 73 73 4C 69 62 72 61 72 79 31   // ...ClassLibrary1
                                                                                                    00 00 ) 
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module ClassLibrary1.dll
// MVID: {42EF172F-72A5-4FCA-96A6-5454244E5433}
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x04EC0000

.class interface public abstract auto ansi ILTest.ITest
{
  .method public hidebysig newslot virtual instance void  DefaultMethod() cil managed
  {
    .maxstack  8
    IL_0000:  ret
  }

}
            """

        let fsharpSource =
            """
open CSharpTest

type Test () =

    interface ITest with

        member __.DefaultMethod () = Console.Write("ExplicitFSharp")

[<EntryPoint>]
let main _ =
   // let test = Test () :> ITest
   // test.DefaultMethod ()
    0
            """

        let c = CompilationUtil.CreateILCompilation ilSource
        CompilerAssert.CompileExeAndRun (fsharpSource, c, "ExplicitFSharp", fsharpLanguageVersion = "4.6")

#endif