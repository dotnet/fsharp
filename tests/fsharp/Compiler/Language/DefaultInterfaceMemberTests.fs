// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test
open FSharp.Test.Utilities
open FSharp.Compiler.Diagnostics

#if NETCOREAPP

[<TestFixture>]
module DefaultInterfaceMemberConsumptionTests_LanguageVersion_4_6 =

    [<Literal>]
    let targetVersion = "5.0"

    [<Test>]
    let ``IL - Errors with lang version not supported`` () =
        let ilSource =
            """
.assembly il.dll
{
}
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
namespace FSharpTest

open ILTest

type Test () =

    interface ITest
            """

        let ilCmpl =
            CompilationUtil.CreateILCompilation ilSource
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [ilCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3350, (8, 15, 8, 20), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
            (FSharpDiagnosticSeverity.Error, 366, (8, 15, 8, 20), "No implementation was given for 'ITest.DefaultMethod() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# with explicit implementation - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest1
    {
        void Method1()
        {
            Console.Write("ITest1." + nameof(Method1));
        }

        void Method2();
    }

    public interface ITest2 : ITest1
    {
        void ITest1.Method2()
        {
            Console.Write("ITest2" + nameof(Method2));
        }

        void Method3();
    }

    public interface ITest3 : ITest2
    {
        void ITest2.Method3()
        {
            Console.Write("ITest3" + nameof(Method3));
        }

        void Method4();
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type Test () =

    interface ITest3 with

        member __.Method1 () = Console.Write("FSharp-Method1")

        member __.Method2 () = Console.Write("FSharp-Method2")

        member __.Method3 () = Console.Write("FSharp-Method3")

        member __.Method4 () = Console.Write("FSharp-Method4")

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest3
    test.Method1 ()
    Console.Write("-")
    test.Method2 ()
    Console.Write("-")
    test.Method3 ()
    Console.Write("-")
    test.Method4 ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "FSharp-Method1-FSharp-Method2-FSharp-Method3-FSharp-Method4")

    [<Test>]
    let ``C# simple - Errors with lang version not supported`` () =
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

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3350, (8, 15, 8, 20), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
            (FSharpDiagnosticSeverity.Error, 366, (8, 15, 8, 20), "No implementation was given for those members: 
\t'ITest.DefaultMethod() : unit'
\t'ITest.NonDefaultMethod() : unit'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# simple - Errors with lang version not supported - 2`` () =
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
        
        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3350, (8, 15, 8, 20), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
            (FSharpDiagnosticSeverity.Error, 366, (8, 15, 8, 20), "No implementation was given for 'ITest.DefaultMethod() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# simple - Errors with lang version not supported - 3`` () =
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

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3350, (8, 15, 8, 20), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
            (FSharpDiagnosticSeverity.Error, 366, (8, 15, 8, 20), "No implementation was given for those members: 
	'ITest.Method1() : unit'
	'ITest.Method2() : unit'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# simple with internal DIM - Errors with lang version not supported`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest
    {
        internal void DefaultMethod()
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

open System
open CSharpTest

type Test () =

    interface ITest with

        member __.NonDefaultMethod () = Console.Write("NonDefaultMethod")
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3350, (9, 15, 9, 20), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
            (FSharpDiagnosticSeverity.Error, 366, (9, 15, 9, 20), "No implementation was given for 'ITest.DefaultMethod() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# simple with internal DIM - Errors with not accessible`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest
    {
        internal void DefaultMethod()
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

open System
open CSharpTest

type Test () =

    interface ITest with

        member __.DefaultMethod () = Console.Write("DefaultMethod")

        member __.NonDefaultMethod () = Console.Write("NonDefaultMethod")
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 855, (11, 19, 11, 32), "No abstract or interface member was found that corresponds to this override")
        |])

    [<Test>]
    let ``C# simple with static operator method - Errors with lang version not supported`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface I1
    {
        public static int operator +(I1 x, I1 y)
        {
            Console.Write("I1.+");
            return 1;
        }
    }
 
    public interface I2 : I1
    {}
}
            """

        let fsharpSource =
            """
module FSharpTest

open System
open CSharpTest

type Test () =

    interface I2

let f () =
    let x = Test () :> I1
    let y = Test () :> I2
    x + y
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3350, (14, 7, 14, 8), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
            (FSharpDiagnosticSeverity.Error, 3350, (14, 9, 14, 10), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
        |])

    [<Test>]
    let ``C# simple with static method - Errors with lang version not supported`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface I1
    {
        public static int StaticMethod(I1 x, I1 y)
        {
            Console.Write("I1.+");
            return 1;
        }
    }
 
    public interface I2 : I1
    {}
}
            """

        let fsharpSource =
            """
module FSharpTest

open System
open CSharpTest

type Test () =

    interface I2

let f () =
    let x = Test () :> I1
    let y = Test () :> I2
    I1.StaticMethod (x, y)
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3350, (14, 5, 14, 27), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
        |])

    [<Test>]
    let ``C# simple with static property - Errors with lang version not supported`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface I1
    {
        public static int StaticProperty { get; set; }
    }
}
            """

        let fsharpSource =
            """
module FSharpTest

open System
open CSharpTest

let f () =
    I1.StaticProperty
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3350, (8, 5, 8, 22), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
        |])

    [<Test>]
    let ``C# simple with static field - Errors with lang version not supported`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface I1
    {
        public static int StaticField;
    }
}
            """

        let fsharpSource =
            """
module FSharpTest

open System
open CSharpTest

let f () =
    I1.StaticField
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3350, (8, 5, 8, 19), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
        |])

    [<Test>]
    let ``C# simple with static method using SRTP - Errors with lang version not supported`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface I1
    {
        public static int StaticMethod(int x, int y)
        {
            return x + y;
        }
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

let inline callStatic<^T when ^T : (static member StaticMethod : int * int -> int)> x y =
    (^T : (static member StaticMethod : int * int -> int) (x, y))

let f1 () =
    callStatic<I1> 1 2
    """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3350, (9, 5, 9, 15), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
        |])

    [<Test>]
    let ``C# simple diamond inheritance - Errors with lang version not supported and should not see the error for specific implementation`` () =
        let csharpSource =
            """
namespace CSharpTest
{
    public interface IA
    {
        void M();
    }

    public interface IB : IA
    {
        void IA.M()
        {
        }
    }

    public interface IC : IA
    {
        void IA.M()
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

    interface IB
    interface IC
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 363, (8, 15, 8, 17), "The interface 'IA' is included in multiple explicitly implemented interface types. Add an explicit implementation of this interface.")
            (FSharpDiagnosticSeverity.Error, 3350, (9, 15, 9, 17), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
            (FSharpDiagnosticSeverity.Error, 366, (9, 15, 9, 17), "No implementation was given for 'IA.M() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
            (FSharpDiagnosticSeverity.Error, 3350, (8, 15, 8, 17), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
            (FSharpDiagnosticSeverity.Error, 366, (8, 15, 8, 17), "No implementation was given for 'IA.M() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# simple diamond inheritance - Errors with lang version not supported and should not see the error for specific implementation - 2`` () =
        let csharpSource =
            """
namespace CSharpTest
{
    public interface IA
    {
        void M();
    }

    public interface IB : IA
    {
        void IA.M()
        {
        }
    }

    public interface IC : IA
    {
        void IA.M()
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

    interface IB
    interface IC
    interface IA
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3350, (10, 15, 10, 17), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
            (FSharpDiagnosticSeverity.Error, 366, (10, 15, 10, 17), "No implementation was given for 'IA.M() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# simple with protected DIM - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest
    {
        protected void M1()
        {
            Console.Write(nameof(M1));
        }

        public void M2()
        {
            this.M1();
            this.M1();
        }
    }

    public interface ITest2 : ITest
    {
        void ITest.M1()
        {
        }
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type Test () =

    interface ITest with

        // protected member
        member __.M1() = 
            Console.Write("Protected")

        member __.M2() =
            Console.Write("Explicit")

type Test2 () =
    inherit Test ()

    interface ITest2 with

        // protected member
        member __.M1() =
            Console.Write("ProtectedOverride")

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest
    test.M2 ()

    Console.Write("-")

    let test2 = Test2 () :> ITest
    test2.M2 ()
    0
            """

        // Explicitly implementing a protected DIM is allowed in F# 4.6.
        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "Explicit-Explicit")

    [<Test>]
    let ``C# with overloading and generics - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface IA<T>
    {
        void M<U>(U x, T y)
        {
            Console.Write("M<U>(U, T)");
        }

        void M<U>(T x, U y);

        void M(T x);

        void M<U>(U x);

        void M(T x, string text);
    }

    public interface IB<T> : IA<T>
    {
        void IA<T>.M(T x)
        {
            Console.Write("InIB");
        }
    }
}
            """

        let fsharpSource =
            """
open CSharpTest

open System

type Test () =

    interface IA<int> with

        member __.M(_x: int) = Console.Write("InTest")

        member __.M<'Item> (x: int, y: 'Item) = 
            Console.Write(x.ToString())
            Console.Write(y.ToString ())

        member __.M<'TTT> (x: 'TTT) =
            Console.Write(x.ToString ())

        member __.M (x: int, text: string) =
            Console.Write("ABC")
            Console.Write(x.ToString())
            Console.Write(text)

        member __.M<'U> (_x: 'U, _y: int) = ()

type Test2 () =
    inherit Test ()

    interface IB<int> with

        member __.M(_x: int) = Console.Write("InTest2")

[<EntryPoint>]
let main _ =
    let test = Test () :> IA<int>
    let test2 = Test2 () :> IA<int>

    test.M 1
    test2.M 1

    test.M<int16> (123, 456s)
    test2.M<int16> (789, 111s)

    test.M<string> "STRING"
    test2.M<string> "-STRING"

    test.M (222, "FSharp")
    test2.M (333, "CSharp")

    test.M<obj> (obj (), 1)
    test2.M<obj> (obj (), 1)

    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "InTestInTest2123456789111STRING-STRINGABC222FSharpABC333CSharp")


    [<Test>]
    let ``C# with overloading and generics - Errors with lang version`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface IA<T>
    {
        void M<U>(U x, T y)
        {
            Console.Write("M<U>(U, T)");
        }

        void M<U>(T x, U y);

        void M(T x);

        void M<U>(U x);

        void M(T x, string text);
    }

    public interface IB<T> : IA<T>
    {
        void IA<T>.M(T x)
        {
            Console.Write("InIB");
        }
    }
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open CSharpTest

open System

type Test () =

    interface IA<int> with

        member __.M(_x: int) = Console.Write("InTest")

        member __.M<'Item> (x: int, y: 'Item) = 
            Console.Write(string x)
            Console.Write(y.ToString ())

        member __.M<'TTT> (x: 'TTT) =
            Console.Write(x.ToString ())

        member __.M (x: int, text: string) =
            Console.Write("ABC")
            Console.Write(string x)
            Console.Write(text)

type Test2 () =
    inherit Test ()

    interface IB<int>
            """
        
        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3350, (10, 15, 10, 22), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
            (FSharpDiagnosticSeverity.Error, 358, (10, 15, 10, 22), "The override for 'M<'U> : 'U * int -> unit' was ambiguous")
        |])

#else

[<TestFixture>]
module DefaultInterfaceMemberConsumptionTests_LanguageVersion_4_6_net472 =

    [<Literal>]
    let targetVersion = "5.0"

    [<Test>]
    let ``IL - Errors with lang version and target runtime not supported`` () =
        let ilSource =
            """
.assembly il.dll
{
}
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
namespace FSharpTest

open ILTest

type Test () =

    interface ITest
            """

        let ilCmpl =
            CompilationUtil.CreateILCompilation ilSource
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [ilCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3351, (8, 15, 8, 20), "Feature 'default interface member consumption' is not supported by target runtime.")
            (FSharpDiagnosticSeverity.Error, 3350, (8, 15, 8, 20), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
            (FSharpDiagnosticSeverity.Error, 366, (8, 15, 8, 20), "No implementation was given for 'ITest.DefaultMethod() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``IL - Errors with target runtime not supported when implemented`` () =
        let ilSource =
            """
.assembly il.dll
{
}
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
namespace FSharpTest

open ILTest

type Test () =

    interface ITest with

        member __.DefaultMethod () = ()
            """

        let ilCmpl =
            CompilationUtil.CreateILCompilation ilSource
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [ilCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3351, (8, 15, 8, 20), "Feature 'default interface member consumption' is not supported by target runtime.")
        |])

    [<Test>]
    let ``C# simple with static method - Errors with lang version and target runtime not supported`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface I1
    {
        public static int StaticMethod(I1 x, I1 y)
        {
            Console.Write("I1.+");
            return 1;
        }
    }
 
    public interface I2 : I1
    {}
}
            """

        let fsharpSource =
            """
module FSharpTest

open System
open CSharpTest

type Test () =

    interface I2

let f () =
    let x = Test () :> I1
    let y = Test () :> I2
    I1.StaticMethod (x, y)
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3351, (14, 5, 14, 27), "Feature 'default interface member consumption' is not supported by target runtime.")
            (FSharpDiagnosticSeverity.Error, 3350, (14, 5, 14, 27), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
        |])

#endif

#if NETCOREAPP

[<TestFixture>]
module DefaultInterfaceMemberConsumptionTests =

    [<Test>]
    let ``C# simple - Errors with un-implemented non-DIM`` () =
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

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 366, (8, 15, 8, 20), "No implementation was given for 'ITest.NonDefaultMethod() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# simple with static operator method - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface I1
    {
        public static int operator +(I1 x, I1 y)
        {
            Console.Write("I1.+");
            return 1;
        }
    }
 
    public interface I2 : I1
    {}
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type Test () =

    interface I2

[<EntryPoint>]
let main _ =
    let x = Test () :> I1
    let y = Test () :> I2
    let result = x + y
    Console.Write(result.ToString())
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "I1.+1")

    [<Test>]
    let ``C# simple - Runs`` () =
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

        member _.NonDefaultMethod () =
            Console.Write("NonDefaultMethod")

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest
    test.DefaultMethod ()
    Console.Write("-")
    test.NonDefaultMethod ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "DefaultMethod-NonDefaultMethod")

    [<Test>]
    let ``C# simple with protected DIM - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest
    {
        protected void M1()
        {
            Console.Write(nameof(M1));
        }

        public void M2()
        {
            this.M1();
            this.M1();
        }
    }

    public interface ITest2 : ITest
    {
        void ITest.M1()
        {
        }
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type Test () =

    interface ITest with

        // protected member
        member _.M1() = 
            Console.Write("Protected")

type Test2 () =
    inherit Test ()

    interface ITest2 with

        // protected member
        member _.M1() =
            Console.Write("ProtectedOverride")

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest
    test.M2 ()

    Console.Write("-")

    let test2 = Test2 () :> ITest
    test2.M2 ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "ProtectedProtected-ProtectedOverrideProtectedOverride")

    [<Test>]
    let ``C# simple with protected DIM using object expression - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest
    {
        protected void M1()
        {
            Console.Write(nameof(M1));
        }

        public void M2()
        {
            this.M1();
            this.M1();
        }
    }

    public interface ITest2 : ITest
    {
        void ITest.M1()
        {
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
    let test =
        { new ITest with
            member _.M1() =
                Console.Write("ObjExprProtected") }
    test.M2 ()

    Console.Write("-")

    let test2 =
        { new ITest2 with
            member _.M1() = 
                Console.Write("ObjExprProtected2") }
    test2.M2 ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "ObjExprProtectedObjExprProtected-ObjExprProtected2ObjExprProtected2")

    [<Test>]
    let ``C# simple with protected DIM - Errors due to protected level`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest
    {
        protected void M1()
        {
            Console.Write(nameof(M1));
        }

        public void M2()
        {
            this.M1();
            this.M1();
        }
    }

    public interface ITest2 : ITest
    {
        void ITest.M1()
        {
        }
    }
}
            """

        let fsharpSource =
            """
module FSharpTest

open System
open CSharpTest

type Test () =

    interface ITest with

        // protected member
        member _.M1() = 
            Console.Write("Protected")

type Test2 () =
    inherit Test ()

    interface ITest2 with

        // protected member
        member _.M1() =
            Console.Write("ProtectedOverride")

let f () =
    let test = Test () :> ITest
    test.M1 ()

    Console.Write("-")

    let test2 = Test2 () :> ITest
    test2.M1 ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 491, (26, 5, 26, 15), "The member or object constructor 'M1' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
            (FSharpDiagnosticSeverity.Error, 491, (31, 5, 31, 16), "The member or object constructor 'M1' is not accessible. Private members may only be accessed from within the declaring type. Protected members may only be accessed from an extending type and cannot be accessed from inner lambda expressions.")
        |])

    [<Test>]
    let ``C# simple with protected DIM - Errors due to protected level - 2`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest
    {
        protected void M1()
        {
            Console.Write(nameof(M1));
        }

        public void M2()
        {
            this.M1();
            this.M1();
        }
    }

    public interface ITest2 : ITest
    {
        void ITest.M1()
        {
        }
    }
}
            """

        let fsharpSource =
            """
module FSharpTest

open System
open CSharpTest

type Test () =

    member this.M() =
        (this :> ITest).M1()

    interface ITest with

        member this.M2() =
            (this :> ITest).M1()

        // protected member
        member _.M1() = 
            Console.Write("Protected")

type Test2 () =
    inherit Test ()

    member this.M() =
        (this :> ITest2).M1()

    interface ITest2 with

        member this.M2() =
            (this :> ITest).M1()

        // protected member
        member _.M1() =
            Console.Write("ProtectedOverride")
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 629, (10, 9, 10, 27), "Method 'M1' is not accessible from this code location")
            (FSharpDiagnosticSeverity.Error, 629, (15, 13, 15, 31), "Method 'M1' is not accessible from this code location")
            (FSharpDiagnosticSeverity.Error, 629, (25, 9, 25, 28), "Method 'M1' is not accessible from this code location")
            (FSharpDiagnosticSeverity.Error, 629, (30, 13, 30, 31), "Method 'M1' is not accessible from this code location")
        |])

    [<Test>]
    let ``C# simple with internal DIM - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest
    {
        internal void DefaultMethod()
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

        member _.NonDefaultMethod () =
            Console.Write("NonDefaultMethod")

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest
    test.NonDefaultMethod ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "NonDefaultMethod")

    [<Test>]
    let ``C# simple with internal DIM - Errors with missing method`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest
    {
        internal void DefaultMethod()
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

type Test () =

    interface ITest with

        member _.NonDefaultMethod () =
            Console.Write("NonDefaultMethod")

let f () =
    let test = Test () :> ITest
    test.DefaultMethod ()
    test.NonDefaultMethod ()
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 39, (16, 10, 16, 23), "The type 'ITest' does not define the field, constructor or member 'DefaultMethod'. Maybe you want one of the following:
   NonDefaultMethod")
        |])

    [<Test>]
    let ``C# simple with internal DIM - Errors with not accessible`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest
    {
        internal void DefaultMethod()
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

type Test () =

    interface ITest with

        member __.DefaultMethod () =
            Console.Write("DefaultMethod")

        member __.NonDefaultMethod () =
            Console.Write("NonDefaultMethod")
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 855, (11, 19, 11, 32), "No abstract or interface member was found that corresponds to this override")
        |])

    [<Test>]
    let ``C# simple with internal DIM but with IVT - Runs`` () =
        let csharpSource =
            """
using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Test")]

namespace CSharpTest
{
    public interface ITest
    {
        internal void DefaultMethod()
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

        member _.DefaultMethod () =
            Console.Write("IVT-")

        member _.NonDefaultMethod () =
            Console.Write("NonDefaultMethod")

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest
    test.DefaultMethod ()
    test.NonDefaultMethod ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl], name = "Test")

        CompilerAssert.ExecutionHasOutput(fsCmpl, "IVT-NonDefaultMethod")

    [<Test>]
    let ``C# simple with one DIM for F# object expression - Runs`` () =
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

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "DefaultMethod")

    [<Test>]
    let ``C# simple with one DIM and one non-DIM for F# object expression - Runs`` () =
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
    let test = { new ITest with member _.NonDefaultMethod () = Console.Write("ObjExpr") }
    test.DefaultMethod ()
    Console.Write("-")
    test.NonDefaultMethod();
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "DefaultMethod-ObjExpr")

    [<Test>]
    let ``C# simple with one DIM and one non-DIM for F# object expression - Errors with lack of implementation`` () =
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

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 366, (7, 12, 7, 25), "No implementation was given for 'ITest.NonDefaultMethod() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# simple with override - Runs`` () =
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

        member _.DefaultMethod () =
            Console.Write("OverrideDefaultMethod")

        member _.NonDefaultMethod () =
            Console.Write("NonDefaultMethod")

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest
    test.DefaultMethod ()
    Console.Write("-")
    test.NonDefaultMethod ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "OverrideDefaultMethod-NonDefaultMethod")

    [<Test>]
    let ``C# simple with override for object expression - Runs`` () =
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
            member _.DefaultMethod () =
                Console.Write("ObjExprOverrideDefaultMethod")
            member _.NonDefaultMethod () =
                Console.Write("ObjExprNonDefaultMethod") }
    test.DefaultMethod ()
    Console.Write("-")
    test.NonDefaultMethod ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "ObjExprOverrideDefaultMethod-ObjExprNonDefaultMethod")

    [<Test>]
    let ``C# from hierarchical interfaces - Runs`` () =
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

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "FromITest2-Method1-FromITest2-Method2")

    [<Test>]
    let ``C# diamond hierarchical interfaces - Errors with lack of explicit shared interface type`` () =
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

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 363, (9, 15, 9, 21), "The interface 'ITest1' is included in multiple explicitly implemented interface types. Add an explicit implementation of this interface.")
            (FSharpDiagnosticSeverity.Error, 3352, (10, 15, 10, 21), "Interface member 'ITest1.Method1() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 3352, (10, 15, 10, 21), "Interface member 'ITest1.Method2() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 366, (10, 15, 10, 21), "No implementation was given for those members: 
	'ITest1.Method1() : unit'
	'ITest1.Method2() : unit'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
            (FSharpDiagnosticSeverity.Error, 3352, (9, 15, 9, 21), "Interface member 'ITest1.Method1() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 3352, (9, 15, 9, 21), "Interface member 'ITest1.Method2() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 366, (9, 15, 9, 21), "No implementation was given for those members: 
	'ITest1.Method1() : unit'
	'ITest1.Method2() : unit'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# diamond hierarchical interfaces - Errors with no most specific implementation`` () =
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

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3352, (9, 15, 9, 21), "Interface member 'ITest1.Method1() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 3352, (9, 15, 9, 21), "Interface member 'ITest1.Method2() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 366, (9, 15, 9, 21), "No implementation was given for those members: 
	'ITest1.Method1() : unit'
	'ITest1.Method2() : unit'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# diamond hierarchical interfaces but combined in one C# interface - Errors with no most specific implementation`` () =
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

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3352, (9, 15, 9, 28), "Interface member 'ITest1.Method1() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 3352, (9, 15, 9, 28), "Interface member 'ITest1.Method2() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 366, (9, 15, 9, 28), "No implementation was given for those members: 
	'ITest1.Method1() : unit'
	'ITest1.Method2() : unit'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# diamond hierarchical interfaces but combined in one F# interface - Errors with no most specific implementation`` () =
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

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3352, (13, 15, 13, 28), "Interface member 'ITest1.Method1() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 3352, (13, 15, 13, 28), "Interface member 'ITest1.Method2() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 366, (13, 15, 13, 28), "No implementation was given for those members: 
	'ITest1.Method1() : unit'
	'ITest1.Method2() : unit'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# diamond hierarchical interfaces but re-abstracted in one and then combined in one F# interface - Errors with no most specific implementation`` () =
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

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3352, (13, 15, 13, 28), "Interface member 'ITest1.Method1() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 3352, (13, 15, 13, 28), "Interface member 'ITest1.Method2() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 366, (13, 15, 13, 28), "No implementation was given for those members: 
	'ITest1.Method1() : unit'
	'ITest1.Method2() : unit'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# diamond hierarchical interfaces but all re-abstracted and then combined in one F# interface - Errors with need to implement members`` () =
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

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3352, (13, 15, 13, 28), "Interface member 'ITest1.Method1() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 3352, (13, 15, 13, 28), "Interface member 'ITest1.Method2() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 366, (13, 15, 13, 28), "No implementation was given for those members: 
	'ITest1.Method1() : unit'
	'ITest1.Method2() : unit'
Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# diamond hierarchical interfaces then combined in one F# interface and then implemented - Runs`` () =
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

        member _.Method1 () = Console.Write("FSharpICombinedTest-Method1")

        member _.Method2 () = Console.Write("FSharpICombinedTest-Method2")

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest3
    test.Method1 ()
    Console.Write("-")
    test.Method2 ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "FSharpICombinedTest-Method1-FSharpICombinedTest-Method2")

    [<Test>]
    let ``C# diamond hierarchical interfaces but all re-abstracted and then combined in one F# interface and then implemented - Runs`` () =
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

        member _.Method1 () = Console.Write("FSharpICombinedTest-Method1")

        member _.Method2 () = Console.Write("FSharpICombinedTest-Method2")

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest2
    test.Method1 ()
    Console.Write("-")
    test.Method2 ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "FSharpICombinedTest-Method1-FSharpICombinedTest-Method2")

    [<Test>]
    let ``C# diamond hierarchical interfaces then using explicit interfaces and then implemented - Runs`` () =
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

type Test () =

    interface ITest2
    interface ITest1 with

        member _.Method1 () = Console.Write("FSharpExplicitTest-Method1")

        member _.Method2 () = Console.Write("FSharpExplicitTest-Method2")
    interface ITest3

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest2
    test.Method1 ()
    Console.Write("-")
    test.Method2 ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "FSharpExplicitTest-Method1-FSharpExplicitTest-Method2")

    [<Test>]
    let ``C# diamond hierarchical interfaces but all re-abstracted and then combined in one F# interface and then implemented one method - Errors with no most specific implementation`` () =
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
    inherit ITest1
    inherit ITest2
    inherit ITest3

type Test () =

    interface ICombinedTest with

        member _.Method2 () = Console.Write("FSharpICombinedTest-Method2")
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3352, (14, 15, 14, 28), "Interface member 'ITest1.Method1() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 366, (14, 15, 14, 28), "No implementation was given for 'ITest1.Method1() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# diamond hierarchical interfaces then combined in one C# interface and then implemented - Runs`` () =
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

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "CSharpICombinedTest-Method1-CSharpICombinedTest-Method2")

    [<Test>]
    let ``C# diamond complex hierarchical interfaces then combined in one C# interface and then implemented - Runs`` () =
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

    public interface ICombinedTest1 : ITest3, ITest2
    {
        void ITest1.Method1()
        {
            Console.Write("CSharpICombinedTest1-" + nameof(Method1));
        }

        void ITest1.Method2()
        {
            Console.Write("CSharpICombinedTest1-" + nameof(Method2));
        }
    }

    public interface ICombinedTest2 : ITest3, ITest2
    {
        void ITest1.Method1()
        {
            Console.Write("CSharpICombinedTest2-" + nameof(Method1));
        }

        void ITest1.Method2()
        {
            Console.Write("CSharpICombinedTest2-" + nameof(Method2));
        }
    }

    public interface ICombinedSideTest : ICombinedTest2
    {
        void ITest1.Method1()
        {
            Console.Write("CSharpICombinedSideTest-" + nameof(Method1));
        }

        void ITest1.Method2()
        {
            Console.Write("CSharpICombinedSideTest-" + nameof(Method2));
        }
    }

    public interface IFinalCombinedTest : ICombinedTest1, ICombinedSideTest
    {
        void ITest1.Method1()
        {
            Console.Write("CSharpIFinalCombinedTest-" + nameof(Method1));
        }

        void ITest1.Method2()
        {
            Console.Write("CSharpIFinalCombinedTest-" + nameof(Method2));
        }
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type Test () =

    interface IFinalCombinedTest

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest1
    test.Method1 ()
    Console.Write("-")
    test.Method2 ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "CSharpIFinalCombinedTest-Method1-CSharpIFinalCombinedTest-Method2")

    [<Test>]
    let ``C# diamond complex hierarchical interfaces then combined in one C# interface and then implemented - Runs - 2`` () =
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
        void ITest1.Method2()
        {
            Console.Write("FromITest3-" + nameof(Method2));
        }
    }

    public interface ICombinedTest1 : ITest3, ITest2
    {
        void ITest1.Method1()
        {
            Console.Write("CSharpICombinedTest1-" + nameof(Method1));
        }

        void ITest1.Method2()
        {
            Console.Write("CSharpICombinedTest1-" + nameof(Method2));
        }
    }

    public interface ICombinedTest2 : ITest3, ITest2
    {
        void ITest1.Method1()
        {
            Console.Write("CSharpICombinedTest2-" + nameof(Method1));
        }
    }

    public interface ICombinedSideTest : ICombinedTest1
    {
        void ITest1.Method2()
        {
            Console.Write("CSharpICombinedSideTest-" + nameof(Method2));
        }
    }

    public interface IFinalCombinedTest : ICombinedTest2, ICombinedSideTest
    {
        void ITest1.Method1()
        {
            Console.Write("CSharpIFinalCombinedTest-" + nameof(Method1));
        }

        abstract void ITest1.Method2();
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type Test () =

    interface ICombinedSideTest with

        member _.Method2 () = ()

    interface IFinalCombinedTest

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest1
    test.Method1 ()
    Console.Write("-")
    test.Method2 ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "CSharpIFinalCombinedTest-Method1-")

    [<Test>]
    let ``C# diamond complex hierarchical interfaces then combined in one C# interface and then implemented - Runs - 3`` () =
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
        void ITest1.Method2()
        {
            Console.Write("FromITest3-" + nameof(Method2));
        }
    }

    public interface ICombinedTest1 : ITest3, ITest2
    {
        void ITest1.Method1()
        {
            Console.Write("CSharpICombinedTest1-" + nameof(Method1));
        }

        void ITest1.Method2()
        {
            Console.Write("CSharpICombinedTest1-" + nameof(Method2));
        }
    }

    public interface ICombinedTest2 : ITest3, ITest2
    {
        void ITest1.Method1()
        {
            Console.Write("CSharpICombinedTest2-" + nameof(Method1));
        }
    }

    public interface ICombinedSideTest : ICombinedTest1
    {
        void ITest1.Method2()
        {
            Console.Write("CSharpICombinedSideTest-" + nameof(Method2));
        }
    }

    public interface IFinalCombinedTest : ICombinedTest2, ICombinedSideTest
    {
        void ITest1.Method1()
        {
            Console.Write("CSharpIFinalCombinedTest-" + nameof(Method1));
        }

        void ITest1.Method2()
        {
            Console.Write("CSharpIFinalCombinedTest-" + nameof(Method2));
        }
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type Test () =

    interface ICombinedSideTest
    interface IFinalCombinedTest
    interface ITest1

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest1
    test.Method1 ()
    Console.Write("-")
    test.Method2 ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "CSharpIFinalCombinedTest-Method1-CSharpIFinalCombinedTest-Method2")

    [<Test>]
    let ``C# multi-diamond complex hierarchical interfaces with hiding methods then explicitly implemented - Runs`` () =
        let csharpSource =
            """
namespace CSharpTest
{
    public interface IBase
    {
        void Method()
        {
        }
    }

    public interface IA1 : IBase
    {
        void IBase.Method()
        {
        }

        new void Method();
    }

    public interface IB1 : IBase
    {
        void IBase.Method()
        {
        }

        new void Method();
    }

    public interface IC1 : IBase
    {
        void IBase.Method()
        {
        }

        new void Method();
    }

    public interface ID1 : IBase
    {
        void IBase.Method()
        {
        }
    }

    public interface IDiamond1 : IA1, IB1
    {
    }

    public interface IDiamond2 : IC1, ID1
    {
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type Test () =

    interface IBase with

         member _.Method () = Console.Write("IBase")

    interface IA1 with

        member _.Method () = Console.Write("IA1")

    interface IB1 with

        member _.Method () = Console.Write("IB1")

    interface IC1 with

        member _.Method () = Console.Write("IC1")

    interface IDiamond1
    interface IDiamond2

[<EntryPoint>]
let main _ =
    let test = Test () :> ID1
    test.Method ()
    Console.Write("-") // IBase

    //let test = Test () :> IDiamond1
    //test.Method ()
    //Console.Write("-") // IA1

    let test = Test () :> IB1
    test.Method ()
    Console.Write("-") // IB1

    let test = Test () :> IA1
    test.Method ()
    Console.Write("-") // IA1

    let test = Test () :> IDiamond2
    test.Method ()
    Console.Write("-") // IC1
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "IBase-IB1-IA1-IC1-")

    [<Test>]
    let ``C# multi-diamond complex hierarchical interfaces then explicitly implemented - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface IBase
    {
        void Method()
        {
        }
    }

    public interface IA1 : IBase
    {
        abstract void IBase.Method();
    }

    public interface IB1 : IBase
    {
    }

    public interface IC1 : IBase
    {
        void CMethod()
        {
            Console.Write("ABC");
        }
    }

    public interface IC2 : IC1
    {
        abstract void IC1.CMethod();
    }

    public interface IC3 : IC2
    {
        void IC1.CMethod()
        {
            Console.Write("XYZ");
        }
    }

    public interface ID1 : IBase
    {
    }

    public interface IDiamond1 : IA1, IB1
    {
    }

    public interface IDiamond2 : IC2, ID1
    {
    }

    public interface IDiamond3 : IC3, IA1
    {
    }

    public interface IMultiDiamond1 : IDiamond1, IDiamond3
    {
    }

    class CSharpClass : IBase, IDiamond2, IMultiDiamond1
    {
        void IBase.Method()
        {
        }
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type Test () =

    interface IBase with

        member _.Method () = Console.Write "123"

    interface IC2
    interface IDiamond2
    interface IMultiDiamond1

[<EntryPoint>]
let main _ =
    let test = Test () :> IMultiDiamond1
    test.Method ()
    test.CMethod ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "123XYZ")

    [<Test>]
    let ``C# diamond complex hierarchical interfaces then explicitly implemented - Runs`` () =
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
        void ITest1.Method2()
        {
            Console.Write("FromITest3-" + nameof(Method2));
        }
    }

    public interface ICombinedTest1 : ITest2
    {
        void ITest1.Method1()
        {
            Console.Write("CSharpICombinedTest1-" + nameof(Method1));
        }

        void ITest1.Method2()
        {
            Console.Write("CSharpICombinedTest1-" + nameof(Method2));
        }
    }

    public interface ICombinedTest2 : ITest3, ITest2
    {
        void ITest1.Method1()
        {
            Console.Write("CSharpICombinedTest2-" + nameof(Method1));
        }
    }

    public interface ICombinedSideTest : ICombinedTest1
    {
        void ITest1.Method2()
        {
            Console.Write("CSharpICombinedSideTest-" + nameof(Method2));
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
    interface ICombinedSideTest
    interface ITest1 with

        member _.Method1 () = ()

        member _.Method2 () = ()

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest1
    test.Method1 ()
    Console.Write("-")
    test.Method2 ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "-")

    [<Test>]
    let ``C# diamond complex hierarchical interfaces then combined in one C# interface and then implemented - Errors with no impl`` () =
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
        void ITest1.Method2()
        {
            Console.Write("FromITest3-" + nameof(Method2));
        }
    }

    public interface ICombinedTest1 : ITest3, ITest2
    {
        abstract void ITest1.Method1();

        abstract void ITest1.Method2();
    }

    public interface ICombinedTest2 : ITest3, ITest2
    {
        void ITest1.Method1()
        {
            Console.Write("CSharpICombinedTest2-" + nameof(Method1));
        }
    }

    public interface ICombinedSideTest : ICombinedTest1
    {
        void ITest1.Method2()
        {
            Console.Write("CSharpICombinedSideTest-" + nameof(Method2));
        }
    }

    public interface IFinalCombinedTest : ICombinedTest2, ICombinedSideTest
    {
        void ITest1.Method1()
        {
            Console.Write("CSharpIFinalCombinedTest-" + nameof(Method1));
        }

        abstract void ITest1.Method2();
    }
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open System
open CSharpTest

type Test () =

    interface IFinalCombinedTest
    interface ICombinedSideTest

type Test2 () =
    inherit Test ()
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 366, (10, 15, 10, 32), "No implementation was given for 'ITest1.Method2() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# simple with property - Runs`` () =
        let csharpSource =
            """
namespace CSharpTest
{
    public interface ITest
    {
        string A { get { return "A"; } }

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

        member _.NonDefaultMethod () =
            Console.Write("NonDefaultMethod")

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest
    Console.Write(test.A)
    Console.Write("-")
    test.NonDefaultMethod ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "A-NonDefaultMethod")

    [<Test>]
    let ``C# simple with property and override - Runs`` () =
        let csharpSource =
            """
namespace CSharpTest
{
    public interface ITest
    {
        string A { get { return "A"; } }

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

        member _.A with get () = "OverrideA"

        member _.NonDefaultMethod () =
            Console.Write("NonDefaultMethod")

[<EntryPoint>]
let main _ =
    let test = Test () :> ITest
    Console.Write(test.A)
    Console.Write("-")
    test.NonDefaultMethod ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "OverrideA-NonDefaultMethod")

    [<Test>]
    let ``C# with same methods names that hide with overloading - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest1<T>
    {
        void Method1(int x)
        {
            Console.Write(x);
        }
    }

    public interface ITest2<T> : ITest1<T>
    {
        void Method1(int x)
        {
            Console.Write(x);
        }
    }

    public interface ITest3 : ITest2<int>
    {
        void ITest1<int>.Method1(int x)
        {
            Console.Write(x);
        }

        void ITest2<int>.Method1(int x)
        {
            Console.Write(x);
        }
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type Test () =

    interface ITest2<int>
    interface ITest3

[<EntryPoint>]
let main _ =
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "")

    [<Test>]
    let ``C# with mutliple separate interfaces - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface IA
    {
        void MA()
        {
            Console.Write("IA.MA");
        }
    }

    public interface IB
    {
        void MB()
        {
            Console.Write("IB.MB");
        }
    }

    public interface IB1 : IB
    {
        void IB.MB()
        {
            Console.Write("IB1.IB.MB");
        }
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type Test () =

    interface IA
    interface IB1

[<EntryPoint>]
let main _ =
    let test = Test ()
    let testIA = test :> IA
    let testIB = test :> IB
    let testIB1 = test :> IB1
    testIA.MA ()
    Console.Write("-")
    testIB.MB ()
    Console.Write("-")
    testIB1.MB ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "IA.MA-IB1.IB.MB-IB1.IB.MB")

    [<Test>]
    let ``C# simple diamond inheritance - Errors with no specific implementation`` () =
        let csharpSource =
            """
namespace CSharpTest
{
    public interface IA
    {
        void M();
    }

    public interface IB : IA
    {
        void IA.M()
        {
        }
    }

    public interface IC : IA
    {
        void IA.M()
        {
        }
    }
}
            """

        let fsharpSource =
            """
module FSharpTest

open CSharpTest

type Test () =

    interface IB
    interface IC
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 363, (8, 15, 8, 17), "The interface 'IA' is included in multiple explicitly implemented interface types. Add an explicit implementation of this interface.")
            (FSharpDiagnosticSeverity.Error, 3352, (9, 15, 9, 17), "Interface member 'IA.M() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 366, (9, 15, 9, 17), "No implementation was given for 'IA.M() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
            (FSharpDiagnosticSeverity.Error, 3352, (8, 15, 8, 17), "Interface member 'IA.M() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 366, (8, 15, 8, 17), "No implementation was given for 'IA.M() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# simple diamond inheritance - Errors with no specific implementation - 2`` () =
        let csharpSource =
            """
namespace CSharpTest
{
    public interface IA
    {
        void M();
    }

    public interface IB : IA
    {
        void IA.M()
        {
        }
    }

    public interface IC : IA
    {
        void IA.M()
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

    interface IB
    interface IC
    interface IA
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3352, (10, 15, 10, 17), "Interface member 'IA.M() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 366, (10, 15, 10, 17), "No implementation was given for 'IA.M() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# simple diamond inheritance - Runs`` () =
        let csharpSource =
            """
namespace CSharpTest
{
    public interface IA
    {
        void M();
    }

    public interface IB : IA
    {
        void IA.M()
        {
        }
    }

    public interface IC : IA
    {
        void IA.M()
        {
        }
    }
}
            """

        let fsharpSource =
            """
open CSharpTest

open System

type Test () =

    interface IB
    interface IC
    interface IA with

        member _.M () = Console.Write("M")

[<EntryPoint>]
let main _ =
    let test = Test ()
    let testIA = test :> IA
    let testIB = test :> IB
    let testIC = test :> IC
    testIA.M ()
    testIB.M ()
    testIC.M ()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "MMM")

    [<Test>]
    let ``C# simple diamond inheritance with overloading - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface IA
    {
        void M(int x, float y)
        {
        }

        void M();

        void M(int x);

        void M(float x);

        void M(int x, int y)
        {
        }
    }

    public interface IB : IA
    {
        void IA.M()
        {
        }

        void IA.M(int x)
        {
            Console.Write(x);
        }
    }

    public interface IC : IA
    {
        void IA.M()
        {
        }

        void IA.M(int x, float y)
        {
            Console.Write(x);
            Console.Write("float");
        }
    }
}
            """

        let fsharpSource =
            """
open CSharpTest

open System

type Test () =

    interface IB
    interface IC
    interface IA with

        member _.M () = Console.Write("M")

        member _.M (_x: single) = Console.Write("fs_single")

[<EntryPoint>]
let main _ =
    let test = Test ()
    let testIA = test :> IA
    let testIB = test :> IB
    let testIC = test :> IC
    testIA.M ()
    testIC.M 123
    testIB.M (456, 1.f)
    testIC.M 1.f
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "M123456floatfs_single")

    [<Test>]
    let ``C# simple diamond inheritance with overloading - Errors with missing overload method`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface IA
    {
        void M(int x, float y)
        {
        }

        void M();

        void M(int x);

        void M(float x);

        void M(int x, int y)
        {
        }
    }

    public interface IB : IA
    {
        void IA.M()
        {
        }

        void IA.M(int x)
        {
            Console.Write(x);
        }
    }

    public interface IC : IA
    {
        void IA.M()
        {
        }

        void IA.M(int x, float y)
        {
            Console.Write(x);
            Console.Write("float");
        }
    }
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open CSharpTest

open System

type Test () =

    interface IB
    interface IC
    interface IA with

        member _.M () = Console.Write("M")
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 366, (12, 15, 12, 17), "No implementation was given for 'IA.M(x: float32) : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# with overloading and generics - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface IA<T>
    {
        void M<U>(U x, T y)
        {
            Console.Write("M<U>(U, T)");
        }

        void M<U>(T x, U y);

        void M(T x);

        void M<U>(U x);

        void M(T x, string text);
    }

    public interface IB<T> : IA<T>
    {
        void IA<T>.M(T x)
        {
            Console.Write("InIB");
        }
    }
}
            """

        let fsharpSource =
            """
open CSharpTest

open System

type Test () =

    interface IA<int> with

        member _.M(_x: int) = Console.Write("InTest")

        member _.M<'Item> (x: int, y: 'Item) = 
            Console.Write(x.ToString())
            Console.Write(y.ToString ())

        member _.M<'TTT> (x: 'TTT) =
            Console.Write(x.ToString ())

        member _.M (x: int, text: string) =
            Console.Write("ABC")
            Console.Write(x.ToString())
            Console.Write(text)

type Test2 () =
    inherit Test ()

    interface IB<int>

[<EntryPoint>]
let main _ =
    let test = Test () :> IA<int>
    let test2 = Test2 () :> IB<int>

    test.M 1
    test2.M 1

    test.M<int16> (123, 456s)
    test2.M<int16> (789, 111s)

    test.M<string> "STRING"
    test2.M<string> "-STRING"

    test.M (222, "FSharp")
    test2.M (333, "CSharp")

    test.M<obj> (obj (), 1)
    test2.M<obj> (obj (), 1)

    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "InTestInTest123456789111STRING-STRINGABC222FSharpABC333CSharpM<U>(U, T)M<U>(U, T)")

    [<Test>]
    let ``C# diamond inheritance with overloading and generics and properties - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface IA<T>
    {
        T Prop1 { get { return default(T); } }

        T Prop2 { set; }

        T Prop3 { get { return default(T); } set { } }

        void M(T x);

        void M(int x)
        {
        }
    }

    public interface IB<T> : IA<T>
    {
        void IA<T>.M(T x)
        {
            Console.Write("InIB");
        }

        T IA<T>.Prop2
        {
            set
            {
                Console.Write("IB<T>.Prop2Set");
            }
        }
    }

    public interface IC : IA<string>
    {
        void IA<string>.M(string x)
        {
            Console.Write("InIC");
        }

        string IA<string>.Prop2
        {
            set
            {
                Console.Write("IC.Prop2Set");
            }
        }

        void M_C()
        {
        }
    }
}
            """

        let fsharpSource =
            """
open CSharpTest

open System

type Test () = 

    interface IB<string>
    interface IC
    interface IA<string> with

        member _.M(_x: string) = Console.Write("Test.String")

        member _.Prop2 with set _ = Console.Write("Test.Prop2")

[<EntryPoint>]
let main _ =
    let test = Test () :> IC
    test.M("")
    Console.Write("-")
    test.Prop2 <- ""
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "Test.String-Test.Prop2")

    [<Test>]
    let ``C# diamond inheritance with overloading and generics and properties - Errors with no specific implementation`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface IA<T>
    {
        T Prop1 { get { return default(T); } }

        T Prop2 { set; }

        T Prop3 { get { return default(T); } set { } }

        void M(T x);

        void M(int x)
        {
        }
    }

    public interface IB<T> : IA<T>
    {
        void IA<T>.M(T x)
        {
            Console.Write("InIB");
        }

        T IA<T>.Prop2
        {
            set
            {
                Console.Write("IB<T>.Prop2Set");
            }
        }
    }

    public interface IC : IA<string>
    {
        void IA<string>.M(string x)
        {
            Console.Write("InIC");
        }

        string IA<string>.Prop2
        {
            set
            {
                Console.Write("IC.Prop2Set");
            }
        }

        void M_C()
        {
        }
    }
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open CSharpTest

open System

type Test () = 

    interface IB<string>
    interface IC
    interface IA<string> with

        member _.M(_x: string) = Console.Write("Test.String")
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3352, (12, 15, 12, 25), "Interface member 'IA.set_Prop2(value: string) : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 366, (12, 15, 12, 25), "No implementation was given for 'IA.set_Prop2(value: string) : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# simple diamond inheritance using object expression - Errors with no specific implementation`` () =
        let csharpSource =
            """
namespace CSharpTest
{
    public interface IA
    {
        void M();
    }

    public interface IB : IA
    {
        void IA.M()
        {
        }
    }

    public interface IC : IA
    {
        void IA.M()
        {
        }
    }
}
            """

        let fsharpSource =
            """
module FSharpTest

open CSharpTest

let test =
    { new IB
      interface IC }
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 363, (8, 7, 8, 21), "The interface 'IA' is included in multiple explicitly implemented interface types. Add an explicit implementation of this interface.")
            (FSharpDiagnosticSeverity.Error, 3352, (7, 5, 8, 21), "Interface member 'IA.M() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 366, (7, 5, 8, 21), "No implementation was given for 'IA.M() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
            (FSharpDiagnosticSeverity.Error, 3352, (8, 7, 8, 21), "Interface member 'IA.M() : unit' does not have a most specific implementation.")
            (FSharpDiagnosticSeverity.Error, 366, (8, 7, 8, 21), "No implementation was given for 'IA.M() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# diamond inheritance with no most specific problem - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface IA
    {
        void M() 
        {
        }
    }

    public interface IB : IA
    {
        void IA.M()
        {
            Console.Write("IB.IA.M");
        }
    }

    public interface IC : IA
    {
    }
}
            """

        let fsharpSource =
            """
open CSharpTest

open System

type Test () = 

    interface IA
    interface IC
    interface IB

[<EntryPoint>]
let main _ =
    let test = Test () :> IC
    test.M();
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "IB.IA.M")

    [<Test>]
    let ``C# diamond inheritance with no most specific problem - Runs - 2`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface IA
    {
        void M() 
        {
        }
    }

    public interface IB : IA
    {
        void IA.M()
        {
            Console.Write("IB.IA.M");
        }
    }

    public interface IC : IA
    {
    }
}
            """

        let fsharpSource =
            """
open CSharpTest

open System

type Test () = 

    interface IC
    interface IB

[<EntryPoint>]
let main _ =
    let test = Test () :> IC
    test.M();
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "IB.IA.M")

    [<Test>]
    let ``C# with interface statics - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface I1
    {
        public static int StaticMethod(int x, int y)
        {
            return x + y;
        }

        public static int StaticProperty { get; set; }

        public static int StaticField;
    }
 
    public interface I2 : I1
    {}
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type I3 =
    inherit I2

let f () =
    I1.StaticMethod (1, 2) |> ignore

    // Even though I1 declares statics, it can be called by qualifying I2 or I3(declared in F#).
    I2.StaticMethod (1, 2) |> ignore
    I2.StaticProperty <- 5
    let _value = I2.StaticProperty
    I2.StaticField <- 5
    let _value = I2.StaticField
    ()
    I3.StaticMethod (1, 2) |> ignore
    I3.StaticProperty <- 5 + I2.StaticProperty
    let _value = I3.StaticProperty
    I3.StaticField <- 6 + I2.StaticField
    let _value = I3.StaticField
    
    Console.Write I2.StaticProperty
    Console.Write I3.StaticField

f ()
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "1011")

    [<Test>]
    let ``C# interface statics and F# SRTP (statically resolved type parameters) - Runs`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface I1
    {
        public static int StaticMethod(int x, int y)
        {
            return x + y;
        }

        public static int StaticProperty { get; set; }

        public static int StaticField;
    }
 
    public interface I2 : I1
    {}
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type I3 =
    inherit I2

let inline callStatic<^T when ^T : (static member StaticMethod : int * int -> int)> x y =
    let value = (^T : (static member StaticMethod : int * int -> int) (x, y))
    Console.Write value

let f1 () =
    callStatic<I1> 1 2

let f2 () =
    callStatic<I2> 2 3

let f3 () =
    callStatic<I3> 4 5

f1 ()
f2 ()
f3 ()
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "359")

    [<Test>]
    let ``C# interface statics - Errors with method not defined on C# class`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface I1
    {
        public static int StaticMethod(int x, int y)
        {
            return x + y;
        }

        public static int StaticProperty { get; set; }

        public static int StaticField;
    }
 
    public interface I2 : I1
    {}

    public class CSharpClass : I2 { }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

let f () =
    CSharpClass.StaticMethod(1, 2)
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 39, (6, 17, 6, 29), "The type 'CSharpClass' does not define the field, constructor or member 'StaticMethod'.")
        |])

    [<Test>]
    let ``C# interface statics - Errors with method not defined on F# class`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface I1
    {
        public static int StaticMethod(int x, int y)
        {
            return x + y;
        }

        public static int StaticProperty { get; set; }

        public static int StaticField;
    }
 
    public interface I2 : I1
    {}
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type FSharpClass() =

    interface I1
    interface I2

let f () =
    FSharpClass.StaticMethod(1, 2)
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 39, (11, 17, 11, 29), "The type 'FSharpClass' does not define the field, constructor or member 'StaticMethod'.")
        |])

    [<Test>]
    let ``C# interface statics - Errors with method not defined on C# class using SRTP`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface I1
    {
        public static int StaticMethod(int x, int y)
        {
            return x + y;
        }

        public static int StaticProperty { get; set; }

        public static int StaticField;
    }
 
    public interface I2 : I1
    {}

    public class CSharpClass : I2 { }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

let inline callStatic<^T when ^T : (static member StaticMethod : int * int -> int)> x y =
    (^T : (static member StaticMethod : int * int -> int) (x, y))

let f () =
    callStatic<CSharpClass> 1 2
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 1, (9, 5, 9, 15), "The type 'CSharpClass' does not support the operator 'StaticMethod'")
        |])

    [<Test>]
    let ``C# interface statics - Errors with method not defined on F# class using SRTP`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface I1
    {
        public static int StaticMethod(int x, int y)
        {
            return x + y;
        }

        public static int StaticProperty { get; set; }

        public static int StaticField;
    }
 
    public interface I2 : I1
    {}
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

type FSharpClass() =

    interface I1
    interface I2

let inline callStatic<^T when ^T : (static member StaticMethod : int * int -> int)> x y =
    (^T : (static member StaticMethod : int * int -> int) (x, y))

let f () =
    callStatic<FSharpClass> 1 2
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 1, (14, 5, 14, 15), "The type 'FSharpClass' does not support the operator 'StaticMethod'")
        |])

#else

[<TestFixture>]
module DefaultInterfaceMemberConsumptionTests_net472 =

    [<Literal>]
    let targetVersion = "5.0"

    [<Test>]
    let ``IL - Errors with target runtime not supported`` () =
        let ilSource =
            """
.assembly il.dll
{
}
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
namespace FSharpTest

open ILTest

type Test () =

    interface ITest
            """

        let ilCmpl =
            CompilationUtil.CreateILCompilation ilSource
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [ilCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3351, (8, 15, 8, 20), "Feature 'default interface member consumption' is not supported by target runtime.")
            (FSharpDiagnosticSeverity.Error, 3350, (8, 15, 8, 20), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
            (FSharpDiagnosticSeverity.Error, 366, (8, 15, 8, 20), "No implementation was given for 'ITest.DefaultMethod() : unit'. Note that all interface members must be implemented and listed under an appropriate 'interface' declaration, e.g. 'interface ... with member ...'.")
        |])

    [<Test>]
    let ``C# simple with static method - Errors with target runtime not supported`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface I1
    {
        public static int StaticMethod(I1 x, I1 y)
        {
            Console.Write("I1.+");
            return 1;
        }
    }
 
    public interface I2 : I1
    {}
}
            """

        let fsharpSource =
            """
module FSharpTest

open System
open CSharpTest

type Test () =

    interface I2

let f () =
    let x = Test () :> I1
    let y = Test () :> I2
    I1.StaticMethod (x, y)
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Library, options = [|"--langversion:4.6"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 3351, (14, 5, 14, 27), "Feature 'default interface member consumption' is not supported by target runtime.")
            (FSharpDiagnosticSeverity.Error, 3350, (14, 5, 14, 27), "Feature 'default interface member consumption' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
        |])

#endif
