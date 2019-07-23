// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

#if NETCOREAPP

[<TestFixture>]
module DefaultInterfaceMethodTests =

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
                Message = "Interface member 'Method1 : unit -> unit' does not have a most specific implementation."
            }
            {
                Number = 3304
                StartLine = 9
                StartColumn = 14
                EndLine = 9
                EndColumn = 20
                Message = "Interface member 'Method2 : unit -> unit' does not have a most specific implementation."
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
                Message = "Interface member 'Method1 : unit -> unit' does not have a most specific implementation."
            }
            {
                Number = 3304
                StartLine = 9
                StartColumn = 14
                EndLine = 9
                EndColumn = 27
                Message = "Interface member 'Method2 : unit -> unit' does not have a most specific implementation."
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
                Message = "Interface member 'Method1 : unit -> unit' does not have a most specific implementation."
            }
            {
                Number = 3304
                StartLine = 13
                StartColumn = 14
                EndLine = 13
                EndColumn = 27
                Message = "Interface member 'Method2 : unit -> unit' does not have a most specific implementation."
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
                Message = "Interface member 'Method1 : unit -> unit' does not have a most specific implementation."
            }
            {
                Number = 3304
                StartLine = 13
                StartColumn = 14
                EndLine = 13
                EndColumn = 27
                Message = "Interface member 'Method2 : unit -> unit' does not have a most specific implementation."
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

#endif