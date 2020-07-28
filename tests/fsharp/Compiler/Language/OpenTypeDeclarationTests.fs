// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open FSharp.Compiler.SourceCodeServices
open NUnit.Framework
open FSharp.Test.Utilities
open FSharp.Test.Utilities.Utilities
open FSharp.Test.Utilities.Compiler
open FSharp.Tests

[<TestFixture>]
module OpenTypeDeclarationTests =

    [<Literal>]
    let targetVersion = "'preview'"

    let baseModule = """
module Core_OpenStaticClasses

[<AbstractClass; Sealed>]
type MyMath() =
    static member Min(a: double, b: double) = System.Math.Min(a, b)
    static member Min(a: int, b: int) = System.Math.Min(a, b)

[<AbstractClass; Sealed; AutoOpen>]
type AutoOpenMyMath() =
    static member AutoMin(a: double, b: double) = System.Math.Min(a, b)
    static member AutoMin(a: int, b: int) = System.Math.Min(a, b)

[<AbstractClass; Sealed; RequireQualifiedAccess>]
type NotAllowedToOpen() =
    static member QualifiedMin(a: double, b: double) = System.Math.Min(a, b)
    static member QualifiedMin(a: int, b: int) = System.Math.Min(a, b)

"""

    [<Test>]
    let ``OpenSystemMathOnce - langversion:v4_6`` () =
        Fsx (baseModule + """
module OpenSystemMathOnce =

               open type System.Math
               let x = Min(1.0, 2.0)""")
        |> withOptions ["--langversion:4.6"]
        |> typecheck
        |> withDiagnostics
            [
                (Error 3350, Line 22, Col 16, Line 22, Col 37, "Feature 'open type declaration' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
                (Error 39, Line 23, Col 24, Line 23, Col 27, "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
            ]
        |> ignore

    [<Test>]
    let ``OpenSystemMathOnce - langversion:preview`` () =
        Fsx (baseModule + """
module OpenSystemMathOnce =

                       open type System.Math
                       let x = Min(1.0, 2.0)""")
         |> withOptions ["--langversion:preview"]
         |> typecheck
         |> shouldSucceed
         |> ignore

    [<Test>]
    let ``OpenSystemMathTwice - langversion:v4_6`` () =
        Fsx (baseModule + """
module OpenSystemMathTwice = 

    open type System.Math
    let x = Min(1.0, 2.0)

    open type System.Math
    let x2 = Min(2.0, 1.0)""")
        |> withOptions ["--langversion:4.6"]
        |> typecheck
        |> withDiagnostics
            [
                (Error 3350, Line 22, Col 5, Line 22, Col 26, "Feature 'open type declaration' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
                (Error 39, Line 23, Col 13, Line 23, Col 16, "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
                (Error 3350, Line 25, Col 5, Line 25, Col 26, "Feature 'open type declaration' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
                (Error 39, Line 26, Col 14, Line 26, Col 17, "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
            ]
        |> ignore

    [<Test>]
    let ``OpenSystemMathTwice - langversion:preview`` () =
        Fsx (baseModule + """
module OpenSystemMathOnce =

                   open type System.Math
                   let x = Min(1.0, 2.0)""")
        |> withOptions ["--langversion:preview"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``OpenMyMathOnce - langversion:v4_6`` () =
        Fsx (baseModule + """
module OpenMyMathOnce = 

    open type MyMath
    let x = Min(1.0, 2.0)
    let x2 = Min(1, 2)""")
        |> withOptions ["--langversion:4.6"]
        |> typecheck
        |> withDiagnostics
            [
                (Error 3350, Line 22, Col 5, Line 22, Col 21, "Feature 'open type declaration' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
                (Error 39, Line 23, Col 13, Line 23, Col 16, "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
                (Error 39, Line 24, Col 14, Line 24, Col 17, "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
            ]
        |> ignore

    [<Test>]
    let ``OpenMyMathOnce - langversion:preview`` () =
        Fsx (baseModule + """
module OpenMyMathOnce = 

    open type MyMath
    let x = Min(1.0, 2.0)
    let x2 = Min(1, 2)""")
        |> withOptions ["--langversion:preview"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``DontOpenAutoMath - langversion:v4_6`` () =
        Fsx (baseModule + """
module DontOpenAutoMath = 

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)""")
        |> withOptions ["--langversion:4.6"]
        |> typecheck
        |> withDiagnostics
            [
                (Error 39, Line 22, Col 13, Line 22, Col 20, "The value or constructor 'AutoMin' is not defined.")
                (Error 39, Line 23, Col 14, Line 23, Col 21, "The value or constructor 'AutoMin' is not defined.")
            ]
        |> ignore

    [<Test>]
    let ``DontOpenAutoMath - langversion:preview`` () =
        Fsx (baseModule + """
module DontOpenAutoMath = 

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)""")
        |> withOptions ["--langversion:preview"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``OpenAutoMath - langversion:v4_6`` () =
        Fsx (baseModule + """
module OpenAutoMath = 
    open type AutoOpenMyMath
    //open type NotAllowedToOpen

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)""")
        |> withOptions ["--langversion:4.6"]
        |> typecheck
        |> withDiagnostics
            [
                (Error 3350, Line 21, Col 5, Line 21, Col 29, "Feature 'open type declaration' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
                (Error 39, Line 24, Col 13, Line 24, Col 20, "The value or constructor 'AutoMin' is not defined.")
                (Error 39, Line 25, Col 14, Line 25, Col 21, "The value or constructor 'AutoMin' is not defined.")
            ]
        |> ignore

    [<Test>]
    let ``OpenAutoMath - langversion:preview`` () =
        Fsx (baseModule + """
module OpenAutoMath = 
    open type AutoOpenMyMath
    //open type NotAllowedToOpen

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)""")
        |> withOptions ["--langversion:preview"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``OpenAccessibleFields - langversion:preview`` () =
        Fsx (baseModule + """
module OpenAFieldFromMath =
    open type System.Math
    
    let pi = PI""")
        |> withOptions ["--langversion:preview"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Open type and use nested types as unqualified`` () =
        let csharp =
            CSharp """
using System;

namespace CSharpTest
{
    public static class Test
    {
        public class NestedTest
        {
            public void A()
            {
            }
        }

        public class NestedTest<T>
        {
            public void B()
            {
            }
        }
    }
}"""

        FSharp """
namespace FSharpTest

open type CSharpTest.Test

module Test =
    let x = NestedTest()
    let y = NestedTest<int>()
    let a = x.A()
    let b = y.B()"""
        |> withOptions ["--langversion:preview"]
        |> withReferences [csharp]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Open a type where the type declaration uses a type abbreviation as a qualifier to a real nested type`` () =
        let csharp =
            CSharp """
using System;

namespace CSharpTest
{
    public static class Test
    {
        public class NestedTest
        {
            public void A()
            {
            }
        }

        public class NestedTest<T>
        {
            public void B()
            {
            }
        }
    }
}"""

        FSharp """
namespace FSharpTest

open System
type Abbrev = CSharpTest.Test
open type Abbrev.NestedTest"""
        |> withOptions ["--langversion:preview"]
        |> withReferences [csharp]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Open a type where the type declaration uses a type abbreviation`` () =
        let csharp =
            CSharp """
using System;

namespace CSharpTest
{
    public static class Test
    {
        public class NestedTest
        {
            public void A()
            {
            }
        }

        public class NestedTest<T>
        {
            public void B()
            {
            }
        }
    }
}"""

        FSharp """
namespace FSharpTest

open System
type Abbrev = CSharpTest.Test
open type Abbrev"""
        |> withOptions ["--langversion:preview"]
        |> withReferences [csharp]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Open a nested type as qualified`` () =
        let csharp =
            CSharp """
using System;

namespace CSharpTest
{
    public static class Test
    {
        public class NestedTest
        {
            public static void A()
            {
            }
        }
    }
}"""

        FSharp """
namespace FSharpTest

open System
open type CSharpTest.Test.NestedTest

module Test =
    let x = A()"""
        |> withOptions ["--langversion:preview"]
        |> withReferences [csharp]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Open generic type and use nested types as unqualified`` () =
        let csharp =
            CSharp """
namespace CSharpTest
{
    public class Test<T>
    {
        public class NestedTest
        {
            public T B()
            {
                return default(T);
            }
        }

        public class NestedTest<U>
        {
            public T A()
            {
                return default(T);
            }
        }
    }

    public class Test
    {
    }
}"""

        FSharp """
namespace FSharpTest

open System

module Test =

    let x : CSharpTest.Test<byte>.NestedTest = CSharpTest.Test<byte>.NestedTest()
    let y : CSharpTest.Test<byte>.NestedTest<float> = CSharpTest.Test<byte>.NestedTest<float>()

    let t1 = CSharpTest.Test()

    let t2 = CSharpTest.Test<int>()

open type CSharpTest.Test<byte>

module Test2 =

    let x = NestedTest()
    let xb : byte = x.B()

    let y = NestedTest<int>()
    let ya : byte = y.A()

    let x1 = new NestedTest()
    let x1b : byte = x1.B()

    let y1 = new NestedTest<int>()
    let y1a : byte = y1.A()

    let x2 : NestedTest = new NestedTest()
    let x2b : byte = x2.B()

    let y2 : NestedTest<int> = new NestedTest<int>()
    let y2a : byte = y2.A()"""
        |> withOptions ["--langversion:preview"]
        |> withReferences [csharp]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Open generic type and use nested types as unqualified 2`` () =
         FSharp """
namespace FSharpTest

open type System.Collections.Generic.List<int>

module Test =
    let e2 = new Enumerator()"""
        |> withOptions ["--langversion:preview"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Open generic type and use nested types as unqualified 3`` () =
        let csharp =
            CSharp """
namespace CSharpTest
{
    public class Test<T>
    {
        public class NestedTest
        {
            public class NestedNestedTest
            {
                public T A()
                {
                    return default(T);
                }
            }

            public class NestedNestedTest<U>
            {
                public U B()
                {
                    return default(U);
                }
            }
        }

        public class NestedTest<U>
        {
            public class NestedNestedTest
            {
                public U C()
                {
                    return default(U);
                }
            }

            public class NestedNestedTest<R>
            {
                public R D()
                {
                    return default(R);
                }
            }
        }
    }
}"""

        FSharp """
namespace FSharpTest

open System

module Test =

    let a : CSharpTest.Test<byte>.NestedTest = CSharpTest.Test<byte>.NestedTest()
    let b : CSharpTest.Test<byte>.NestedTest<float> = CSharpTest.Test<byte>.NestedTest<float>()

    let c : CSharpTest.Test<byte>.NestedTest.NestedNestedTest = CSharpTest.Test<byte>.NestedTest.NestedNestedTest()
    let d : CSharpTest.Test<byte>.NestedTest.NestedNestedTest<float> = CSharpTest.Test<byte>.NestedTest.NestedNestedTest<float>()

    let e : CSharpTest.Test<byte>.NestedTest<float>.NestedNestedTest = CSharpTest.Test<byte>.NestedTest<float>.NestedNestedTest()
    let f : CSharpTest.Test<byte>.NestedTest<float>.NestedNestedTest<int> = CSharpTest.Test<byte>.NestedTest<float>.NestedNestedTest<int>()

open type CSharpTest.Test<byte>

module Test2 =

    let a : NestedTest.NestedNestedTest = NestedTest.NestedNestedTest()
    let aa : byte = a.A()

    let b : NestedTest.NestedNestedTest<float> = NestedTest.NestedNestedTest<float>()
    let bb : float = b.B()

    let c : NestedTest<float>.NestedNestedTest = NestedTest<float>.NestedNestedTest()
    let cc : float = c.C()

    let d : NestedTest<float>.NestedNestedTest<int> = NestedTest<float>.NestedNestedTest<int>()
    let dd : int = d.D()

open type NestedTest

module Test3 =

    let a : NestedNestedTest = NestedNestedTest()
    let aa : byte = a.A()

    let b : NestedNestedTest<float> = NestedNestedTest<float>()
    let bb : float = b.B()

open type NestedTest<float>

module Test4 =

    let c : NestedNestedTest = NestedNestedTest()
    let cc : float = c.C()

    let d : NestedNestedTest<int> = NestedNestedTest<int>()
    let dd : int = d.D()"""
        |> withOptions ["--langversion:preview"]
        |> withReferences [csharp]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Using the 'open' declaration on a possible type identifier - Error`` () =
        let csharp =
            CSharp """
using System;

namespace CSharpTest
{
    public static class Test
    {
        public static void A()
        {
        }
    }
}
            """

        FSharp """
namespace FSharpTest

open System
open CSharpTest.Test

module Test =
    let x = A()"""
        |> withOptions ["--langversion:preview"]
        |> withReferences [csharp]
        |> compile
        |> withDiagnostics
            [
                (Error 39, Line 5, Col 17, Line 5, Col 21, "The namespace 'Test' is not defined.")
                (Error 39, Line 8, Col 13, Line 8, Col 14, "The value or constructor 'A' is not defined.")
            ]
        |> ignore

    [<Test>]
    let ``Open type declaration on a namespace - Error`` () =
        FSharp """
namespace FSharpTest

open type System"""
        |> withOptions ["--langversion:preview"]
        |> compile
        |> withDiagnostics
            [
                (Error 39, Line 4, Col 11, Line 4, Col 17, "The type 'System' is not defined.")
            ]
        |> ignore

    [<Test>]
    let ``Open type declaration on a module - Error`` () =
        FSharp """
namespace FSharpTest

open type FSharp.Core.Option"""
        |> withOptions ["--langversion:preview"]
        |> compile
        |> withDiagnostics
            [
                (Error 33, Line 4, Col 11, Line 4, Col 29, "The type 'Microsoft.FSharp.Core.Option<_>' expects 1 type argument(s) but is given 0")
            ]
        |> ignore

    [<Test>]
    let ``Open type declaration on a byref - Error`` () =
        FSharp """
namespace FSharpTest

open type byref<int>
open type inref<int>
open type outref<int>"""
        |> withOptions ["--langversion:preview"]
        |> compile
        |> withDiagnostics
            [
                (Error 3252, Line 4, Col 11, Line 4, Col 21, "Byref types are not allowed in an open type declaration.")
                (Error 3252, Line 5, Col 11, Line 5, Col 21, "Byref types are not allowed in an open type declaration.")
                (Error 3252, Line 6, Col 11, Line 6, Col 22, "Byref types are not allowed in an open type declaration.")
            ]
        |> ignore

    [<Test>]
    let ``Type extensions with static members are able to be accessed in an unqualified manner`` () =
        let fsharpSource =
            """
open System

type A () =

    static member M() = Console.Write "M"

    static member P = Console.Write "P"

[<AutoOpen>]
module AExtensions =

    type A with

        static member M2() = Console.Write "M2Ext"

        static member P2 = Console.Write "P2Ext"

open type A

[<EntryPoint>]
let main _ =
    M()
    P
    M2()
    P2
    0
            """

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Exe, options = [|"--langversion:preview"|])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "MPM2ExtP2Ext")

    [<Test>]
    let ``Type extensions with static members are able to be accessed in an unqualified manner with no shadowing on identical names`` () =
        let fsharpSource =
            """
open System

type A () =

    static member M() = Console.Write "M"

    static member P = Console.Write "P"

[<AutoOpen>]
module AExtensions =

    type A with

        static member M() = Console.Write "MExt"

        static member P = Console.Write "PExt"

open type A

[<EntryPoint>]
let main _ =
    M()
    P
    0
            """

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Exe, options = [|"--langversion:preview"|])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "MP")

    [<Test>]
    let ``Type extensions with static members are able to be accessed in an unqualified manner with the nuance of favoring extension properties over extension methods of identical names`` () =
        let fsharpSource =
            """
open System

type A () =

    static member P = Console.Write "P"

[<AutoOpen>]
module AExtensions =

    type A with

        static member M = Console.Write "MExt"

[<AutoOpen>]
module AExtensions2 =

    type A with

        static member M() = Console.Write "M"

open type A

[<EntryPoint>]
let main _ =
    M
    P
    0
            """

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Exe, options = [|"--langversion:preview"|])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "MExtP")

    [<Test>]
    let ``Type extensions with static members are able to be accessed in an unqualified manner with no shadowing on identical method/property names`` () =
        let fsharpSource =
            """
open System

type A () =

    static member M() = Console.Write "M"

[<AutoOpen>]
module AExtensions =

    type A with

        static member M = Console.Write "MExt"

open type A

[<EntryPoint>]
let main _ =
    M()
    0
            """

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Exe, options = [|"--langversion:preview"|])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "M")

    [<Test>]
    let ``Opened types do no allow unqualified access to their inherited type's members - Error`` () =
        Fsx """
open type System.Math

let x = Equals(2.0, 3.0)
            """
        |> withOptions ["--langversion:preview"]
        |> compile
        |> withDiagnostics
            [
               ( Error 39, Line 4, Col 9, Line 4, Col 15, "The value or constructor 'Equals' is not defined.")
            ]
        |> ignore

    [<Test>]
    let ``Opened types do no allow unqualified access to C#-style extension methods - Error`` () =
        FSharp """
open System.Runtime.CompilerServices

module TestExtensions =
    [<Extension>]
    type IntExtensions =

        [<Extension>]
        static member Test(_: int) = ()

open type TestExtensions.IntExtensions

[<EntryPoint>]
let main _ =
    Test(1)
    0"""
        |> withOptions ["--langversion:preview"]
        |> asExe
        |> compile
        |> withDiagnostics
            [
                (Error 39, Line 15, Col 5, Line 15, Col 9,
                    "The value or constructor 'Test' is not defined. Maybe you want one of the following:
   Text
   TestExtensions")
            ]
        |> ignore

    [<Test>]
    let ``Opened types do allow unqualified access to C#-style extension methods if type has no [<Extension>] attribute`` () =
        FSharp """
open System.Runtime.CompilerServices

module TestExtensions =
    type IntExtensions =

        [<Extension>]
        static member Test(_: int) = ()

open type TestExtensions.IntExtensions

[<EntryPoint>]
let main _ =
    Test(1)
    0"""
        |> withOptions ["--langversion:preview"]
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Opened types do allow unqualified access to members with no [<Extension>] attribute`` () =
        FSharp """
open System.Runtime.CompilerServices

module TestExtensions =
    [<Extension>]
    type IntExtensions =

        static member Test(_: int) = ()

open type TestExtensions.IntExtensions

[<EntryPoint>]
let main _ =
    Test(1)
    0"""
        |> withOptions ["--langversion:preview"]
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Opened types with C# style extension members are available for normal extension method lookup`` () =
        FSharp """
open System.Runtime.CompilerServices

module TestExtensions =
    [<Extension>]
    type IntExtensions =

        [<Extension>]
        static member Test(_: int) = ()

open type TestExtensions.IntExtensions

[<EntryPoint>]
let main _ =
    let x = 1
    x.Test()
    0"""
        |> withOptions ["--langversion:preview"]
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``An assembly with an event and field with the same name, favor the field`` () =
        let ilSource =
            """
.assembly il.dll
{
}
.class public auto ansi abstract sealed beforefieldinit ILTest.C
    extends [netstandard]System.Object
{
    .field public static class [netstandard]System.EventHandler E
    .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )

    .method public hidebysig specialname static 
        void add_E (
            class [netstandard]System.EventHandler 'value'
        ) cil managed 
    {
        .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )

        .maxstack 3
        .locals init (
            [0] class [netstandard]System.EventHandler,
            [1] class [netstandard]System.EventHandler,
            [2] class [netstandard]System.EventHandler
        )

        IL_0000: ldsfld class [netstandard]System.EventHandler ILTest.C::E
        IL_0005: stloc.0
        IL_0006: ldloc.0
        IL_0007: stloc.1
        IL_0008: ldloc.1
        IL_0009: ldarg.0
        IL_000a: call class [netstandard]System.Delegate [netstandard]System.Delegate::Combine(class [netstandard]System.Delegate, class [netstandard]System.Delegate)
        IL_000f: castclass [netstandard]System.EventHandler
        IL_0014: stloc.2
        IL_0015: ldsflda class [netstandard]System.EventHandler ILTest.C::E
        IL_001a: ldloc.2
        IL_001b: ldloc.1
        IL_001c: call !!0 [netstandard]System.Threading.Interlocked::CompareExchange<class [netstandard]System.EventHandler>(!!0&, !!0, !!0)
        IL_0021: stloc.0
        IL_0022: ldloc.0
        IL_0023: ldloc.1
        IL_0024: bne.un.s IL_0006
        IL_0026: ret
    }

    .method public hidebysig specialname static 
        void remove_E (
            class [netstandard]System.EventHandler 'value'
        ) cil managed 
    {
        .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )

        .maxstack 3
        .locals init (
            [0] class [netstandard]System.EventHandler,
            [1] class [netstandard]System.EventHandler,
            [2] class [netstandard]System.EventHandler
        )

        IL_0000: ldsfld class [netstandard]System.EventHandler ILTest.C::E
        IL_0005: stloc.0
        IL_0006: ldloc.0
        IL_0007: stloc.1
        IL_0008: ldloc.1
        IL_0009: ldarg.0
        IL_000a: call class [netstandard]System.Delegate [netstandard]System.Delegate::Remove(class [netstandard]System.Delegate, class [netstandard]System.Delegate)
        IL_000f: castclass [netstandard]System.EventHandler
        IL_0014: stloc.2
        IL_0015: ldsflda class [netstandard]System.EventHandler ILTest.C::E
        IL_001a: ldloc.2
        IL_001b: ldloc.1
        IL_001c: call !!0 [netstandard]System.Threading.Interlocked::CompareExchange<class [netstandard]System.EventHandler>(!!0&, !!0, !!0)
        IL_0021: stloc.0
        IL_0022: ldloc.0
        IL_0023: ldloc.1
        IL_0024: bne.un.s IL_0006
        IL_0026: ret
    }

    .event [netstandard]System.EventHandler X
    {
        .addon void ILTest.C::add_E(class [netstandard]System.EventHandler)
        .removeon void ILTest.C::remove_E(class [netstandard]System.EventHandler)
    }

    .field public static int32 X
}
            """

        let fsharpSource =
            """
module FSharpTest

open ILTest

type C with

    member _.X = obj ()

type C with

    member _.X() = obj ()

let x1: int = C.X

open type C

let x2: int = X
            """

        let ilCmpl =
            CompilationUtil.CreateILCompilation ilSource
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:preview"|], cmplRefs = [ilCmpl])

        CompilerAssert.Compile(fsCmpl)

    [<Test>]
    let ``An assembly with an event and field with the same name, favor the field - reversed`` () =
        let ilSource =
            """
.assembly il.dll
{
}
.class public auto ansi abstract sealed beforefieldinit ILTest.C
    extends [netstandard]System.Object
{
    .field public static int32 X
    .field public static class [netstandard]System.EventHandler E
    .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )

    .method public hidebysig specialname static 
        void add_E (
            class [netstandard]System.EventHandler 'value'
        ) cil managed 
    {
        .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )

        .maxstack 3
        .locals init (
            [0] class [netstandard]System.EventHandler,
            [1] class [netstandard]System.EventHandler,
            [2] class [netstandard]System.EventHandler
        )

        IL_0000: ldsfld class [netstandard]System.EventHandler ILTest.C::E
        IL_0005: stloc.0
        IL_0006: ldloc.0
        IL_0007: stloc.1
        IL_0008: ldloc.1
        IL_0009: ldarg.0
        IL_000a: call class [netstandard]System.Delegate [netstandard]System.Delegate::Combine(class [netstandard]System.Delegate, class [netstandard]System.Delegate)
        IL_000f: castclass [netstandard]System.EventHandler
        IL_0014: stloc.2
        IL_0015: ldsflda class [netstandard]System.EventHandler ILTest.C::E
        IL_001a: ldloc.2
        IL_001b: ldloc.1
        IL_001c: call !!0 [netstandard]System.Threading.Interlocked::CompareExchange<class [netstandard]System.EventHandler>(!!0&, !!0, !!0)
        IL_0021: stloc.0
        IL_0022: ldloc.0
        IL_0023: ldloc.1
        IL_0024: bne.un.s IL_0006
        IL_0026: ret
    }

    .method public hidebysig specialname static 
        void remove_E (
            class [netstandard]System.EventHandler 'value'
        ) cil managed 
    {
        .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )

        .maxstack 3
        .locals init (
            [0] class [netstandard]System.EventHandler,
            [1] class [netstandard]System.EventHandler,
            [2] class [netstandard]System.EventHandler
        )

        IL_0000: ldsfld class [netstandard]System.EventHandler ILTest.C::E
        IL_0005: stloc.0
        IL_0006: ldloc.0
        IL_0007: stloc.1
        IL_0008: ldloc.1
        IL_0009: ldarg.0
        IL_000a: call class [netstandard]System.Delegate [netstandard]System.Delegate::Remove(class [netstandard]System.Delegate, class [netstandard]System.Delegate)
        IL_000f: castclass [netstandard]System.EventHandler
        IL_0014: stloc.2
        IL_0015: ldsflda class [netstandard]System.EventHandler ILTest.C::E
        IL_001a: ldloc.2
        IL_001b: ldloc.1
        IL_001c: call !!0 [netstandard]System.Threading.Interlocked::CompareExchange<class [netstandard]System.EventHandler>(!!0&, !!0, !!0)
        IL_0021: stloc.0
        IL_0022: ldloc.0
        IL_0023: ldloc.1
        IL_0024: bne.un.s IL_0006
        IL_0026: ret
    }

    .event [netstandard]System.EventHandler X
    {
        .addon void ILTest.C::add_E(class [netstandard]System.EventHandler)
        .removeon void ILTest.C::remove_E(class [netstandard]System.EventHandler)
    }
}
            """

        let fsharpSource =
            """
module FSharpTest

open ILTest

type C with

    member _.X = obj ()

type C with

    member _.X() = obj ()

let x1: int = C.X

open type C

let x2: int = X
            """

        let ilCmpl =
            CompilationUtil.CreateILCompilation ilSource
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:preview"|], cmplRefs = [ilCmpl])

        CompilerAssert.Compile(fsCmpl)

    [<Test>]
    let ``An assembly with a property, event, and field with the same name`` () =
        let ilSource =
            """
.assembly il.dll
{
}
.class public auto ansi abstract sealed beforefieldinit ILTest.C
    extends [netstandard]System.Object
{
    .field public static class [netstandard]System.EventHandler E
    .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )

    .method public hidebysig specialname static 
        void add_E (
            class [netstandard]System.EventHandler 'value'
        ) cil managed 
    {
        .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )

        .maxstack 3
        .locals init (
            [0] class [netstandard]System.EventHandler,
            [1] class [netstandard]System.EventHandler,
            [2] class [netstandard]System.EventHandler
        )

        IL_0000: ldsfld class [netstandard]System.EventHandler ILTest.C::E
        IL_0005: stloc.0
        IL_0006: ldloc.0
        IL_0007: stloc.1
        IL_0008: ldloc.1
        IL_0009: ldarg.0
        IL_000a: call class [netstandard]System.Delegate [netstandard]System.Delegate::Combine(class [netstandard]System.Delegate, class [netstandard]System.Delegate)
        IL_000f: castclass [netstandard]System.EventHandler
        IL_0014: stloc.2
        IL_0015: ldsflda class [netstandard]System.EventHandler ILTest.C::E
        IL_001a: ldloc.2
        IL_001b: ldloc.1
        IL_001c: call !!0 [netstandard]System.Threading.Interlocked::CompareExchange<class [netstandard]System.EventHandler>(!!0&, !!0, !!0)
        IL_0021: stloc.0
        IL_0022: ldloc.0
        IL_0023: ldloc.1
        IL_0024: bne.un.s IL_0006
        IL_0026: ret
    }

    .method public hidebysig specialname static 
        void remove_E (
            class [netstandard]System.EventHandler 'value'
        ) cil managed 
    {
        .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )

        .maxstack 3
        .locals init (
            [0] class [netstandard]System.EventHandler,
            [1] class [netstandard]System.EventHandler,
            [2] class [netstandard]System.EventHandler
        )

        IL_0000: ldsfld class [netstandard]System.EventHandler ILTest.C::E
        IL_0005: stloc.0
        IL_0006: ldloc.0
        IL_0007: stloc.1
        IL_0008: ldloc.1
        IL_0009: ldarg.0
        IL_000a: call class [netstandard]System.Delegate [netstandard]System.Delegate::Remove(class [netstandard]System.Delegate, class [netstandard]System.Delegate)
        IL_000f: castclass [netstandard]System.EventHandler
        IL_0014: stloc.2
        IL_0015: ldsflda class [netstandard]System.EventHandler ILTest.C::E
        IL_001a: ldloc.2
        IL_001b: ldloc.1
        IL_001c: call !!0 [netstandard]System.Threading.Interlocked::CompareExchange<class [netstandard]System.EventHandler>(!!0&, !!0, !!0)
        IL_0021: stloc.0
        IL_0022: ldloc.0
        IL_0023: ldloc.1
        IL_0024: bne.un.s IL_0006
        IL_0026: ret
    }

    .event [netstandard]System.EventHandler X
    {
        .addon void ILTest.C::add_E(class [netstandard]System.EventHandler)
        .removeon void ILTest.C::remove_E(class [netstandard]System.EventHandler)
    }

    .field public static int32 X

    .field private static initonly string '<Y>k__BackingField'
    .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )

    .method public hidebysig specialname static 
        string get_Y () cil managed 
    {
        .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )

        .maxstack 8

        IL_0000: ldsfld string ILTest.C::'<Y>k__BackingField'
        IL_0005: ret
    }

    .property string X()
    {
        .get string ILTest.C::get_Y()
    }
}
            """

        let fsharpSource =
            """
module FSharpTest

open ILTest

type C with

    member _.X = obj ()

type C with

    member _.X() = obj ()

let x1: string = C.X

open type C

let x2: string = X
            """

        let ilCmpl =
            CompilationUtil.CreateILCompilation ilSource
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:preview"|], cmplRefs = [ilCmpl])

        CompilerAssert.Compile(fsCmpl)

    [<Test>]
    let ``An assembly with a method, property, event, and field with the same name`` () =
        let ilSource =
            """
.assembly il.dll
{
}
.class public auto ansi abstract sealed beforefieldinit ILTest.C
    extends [netstandard]System.Object
{
    .field public static class [netstandard]System.EventHandler E
    .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )

    .method public hidebysig specialname static 
        void add_E (
            class [netstandard]System.EventHandler 'value'
        ) cil managed 
    {
        .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )

        .maxstack 3
        .locals init (
            [0] class [netstandard]System.EventHandler,
            [1] class [netstandard]System.EventHandler,
            [2] class [netstandard]System.EventHandler
        )

        IL_0000: ldsfld class [netstandard]System.EventHandler ILTest.C::E
        IL_0005: stloc.0
        IL_0006: ldloc.0
        IL_0007: stloc.1
        IL_0008: ldloc.1
        IL_0009: ldarg.0
        IL_000a: call class [netstandard]System.Delegate [netstandard]System.Delegate::Combine(class [netstandard]System.Delegate, class [netstandard]System.Delegate)
        IL_000f: castclass [netstandard]System.EventHandler
        IL_0014: stloc.2
        IL_0015: ldsflda class [netstandard]System.EventHandler ILTest.C::E
        IL_001a: ldloc.2
        IL_001b: ldloc.1
        IL_001c: call !!0 [netstandard]System.Threading.Interlocked::CompareExchange<class [netstandard]System.EventHandler>(!!0&, !!0, !!0)
        IL_0021: stloc.0
        IL_0022: ldloc.0
        IL_0023: ldloc.1
        IL_0024: bne.un.s IL_0006
        IL_0026: ret
    }

    .method public hidebysig specialname static 
        void remove_E (
            class [netstandard]System.EventHandler 'value'
        ) cil managed 
    {
        .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )

        .maxstack 3
        .locals init (
            [0] class [netstandard]System.EventHandler,
            [1] class [netstandard]System.EventHandler,
            [2] class [netstandard]System.EventHandler
        )

        IL_0000: ldsfld class [netstandard]System.EventHandler ILTest.C::E
        IL_0005: stloc.0
        IL_0006: ldloc.0
        IL_0007: stloc.1
        IL_0008: ldloc.1
        IL_0009: ldarg.0
        IL_000a: call class [netstandard]System.Delegate [netstandard]System.Delegate::Remove(class [netstandard]System.Delegate, class [netstandard]System.Delegate)
        IL_000f: castclass [netstandard]System.EventHandler
        IL_0014: stloc.2
        IL_0015: ldsflda class [netstandard]System.EventHandler ILTest.C::E
        IL_001a: ldloc.2
        IL_001b: ldloc.1
        IL_001c: call !!0 [netstandard]System.Threading.Interlocked::CompareExchange<class [netstandard]System.EventHandler>(!!0&, !!0, !!0)
        IL_0021: stloc.0
        IL_0022: ldloc.0
        IL_0023: ldloc.1
        IL_0024: bne.un.s IL_0006
        IL_0026: ret
    }

    .event [netstandard]System.EventHandler X
    {
        .addon void ILTest.C::add_E(class [netstandard]System.EventHandler)
        .removeon void ILTest.C::remove_E(class [netstandard]System.EventHandler)
    }

    .field public static int32 X

    .field private static initonly string '<Y>k__BackingField'
    .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
        01 00 00 00
    )

    .method public hidebysig specialname static 
        string get_Y () cil managed 
    {
        .custom instance void [netstandard]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
            01 00 00 00
        )

        .maxstack 8

        IL_0000: ldsfld string ILTest.C::'<Y>k__BackingField'
        IL_0005: ret
    }

    .property string X()
    {
        .get string ILTest.C::get_Y()
    }

    .method public hidebysig static 
        float32 X () cil managed 
    {
        .maxstack 8

        IL_0000: ldc.r4 0.0
        IL_0005: ret
    }
}
            """

        let fsharpSource =
            """
module FSharpTest

open ILTest

type C with

    member _.X = obj ()

type C with

    member _.X() = obj ()

let x1: float32 = C.X()

open type C

let x2: float32 = X()
            """

        let ilCmpl =
            CompilationUtil.CreateILCompilation ilSource
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:preview"|], cmplRefs = [ilCmpl])

        CompilerAssert.Compile(fsCmpl)

#if NETCOREAPP

    [<Test>]
    let ``Opening an interface with a static method`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest
    {
        public static void M()
        {
        }
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

open type ITest

[<EntryPoint>]
let main _ =
    M()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Exe, options = [|"--langversion:preview"|], cmplRefs = [csCmpl])

        CompilerAssert.Compile(fsCmpl)

    [<Test>]
    let ``Opening an interface with an internal static method`` () =
        let csharpSource =
            """
using System;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Test")]

namespace CSharpTest
{
    public interface ITest
    {
        internal static void M()
        {
        }
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

open type ITest

[<EntryPoint>]
let main _ =
    M()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Exe, options = [|"--langversion:preview"|], cmplRefs = [csCmpl], name = "Test")

        CompilerAssert.Compile(fsCmpl)

    [<Test>]
    let ``Opening an interface with an internal static method - Error`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public interface ITest
    {
        internal static void M()
        {
        }
    }
}
            """

        let fsharpSource =
            """
open System
open CSharpTest

open type ITest

[<EntryPoint>]
let main _ =
    M()
    0
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Exe, options = [|"--langversion:preview"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpErrorSeverity.Error, 39, (9, 5, 9, 6), "The value or constructor 'M' is not defined.")
        |])

#endif

#if !NETCOREAPP

    [<Test>]
    let ``Opening type providers with abbreviation result in unqualified access to types and members`` () =
        let dir = Core.getTestsDirectory "typeProviders/helloWorld"

        let provider =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provider.fsx"))
            |> withName "provider"
            |> ignoreWarnings
            |> withOptions ["--langversion:preview"]

        let provided =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provided.fs"))
            |> withName "provided"
            |> ignoreWarnings
            |> withOptions ["--langversion:preview"]

        let test =
            Fsx """
type T = FSharp.HelloWorld.HelloWorldTypeWithStaticInt32Parameter<1>

open type T

if NestedType.StaticProperty1 <> "You got a static property" then
    failwith "failed"

open type T.NestedType

if StaticProperty1 <> "You got a static property" then
    failwith "failed"
            """
            |> asExe
            |> ignoreWarnings
            |> withOptions ["--langversion:preview"]
            |> withReferences [provider;provided]

        compileAndRun test
        |> ignore

    [<Test>]
    let ``Opening type providers result in unqualified access to types and members`` () =
        let dir = Core.getTestsDirectory "typeProviders/helloWorld"

        let provider =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provider.fsx"))
            |> withName "provider"
            |> ignoreWarnings
            |> withOptions ["--langversion:preview"]

        let provided =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provided.fs"))
            |> withName "provided"
            |> ignoreWarnings
            |> withOptions ["--langversion:preview"]

        let test =
            Fsx """
open type FSharp.HelloWorld.HelloWorldTypeWithStaticInt32Parameter<1>

if NestedType.StaticProperty1 <> "You got a static property" then
    failwith "failed"

open type NestedType

if StaticProperty1 <> "You got a static property" then
    failwith "failed"
            """
            |> asExe
            |> ignoreWarnings
            |> withOptions ["--langversion:preview"]
            |> withReferences [provider;provided]

        compileAndRun test
        |> ignore

    [<Test>]
    let ``Opening type providers with nested result in unqualified access to types and members`` () =
        let dir = Core.getTestsDirectory "typeProviders/helloWorld"

        let provider =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provider.fsx"))
            |> withName "provider"
            |> ignoreWarnings
            |> withOptions ["--langversion:preview"]

        let provided =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provided.fs"))
            |> withName "provided"
            |> ignoreWarnings
            |> withOptions ["--langversion:preview"]

        let test =
            Fsx """
open type FSharp.HelloWorld.HelloWorldTypeWithStaticInt32Parameter<1>.NestedType

if StaticProperty1 <> "You got a static property" then
    failwith "failed"
            """
            |> asExe
            |> ignoreWarnings
            |> withOptions ["--langversion:preview"]
            |> withReferences [provider;provided]

        compileAndRun test
        |> ignore

    [<Test>]
    let ``Opening generative type providers in unqualified access to types and members`` () =
        let dir = Core.getTestsDirectory "typeProviders/helloWorld"

        let provider =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provider.fsx"))
            |> withName "provider"
            |> ignoreWarnings
            |> withOptions ["--langversion:preview"]

        let provided =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provided.fs"))
            |> withName "provided"
            |> ignoreWarnings
            |> withOptions ["--langversion:preview"]

        let test =
            Fsx """
type TheOuterType = FSharp.HelloWorldGenerative.TheContainerType<"TheOuterType">

open type TheOuterType

let _ : TheNestedGeneratedType = Unchecked.defaultof<_>
            """
            |> asExe
            |> ignoreWarnings
            |> withOptions ["--langversion:preview"]
            |> withReferences [provider;provided]

        compileAndRun test
        |> ignore

    [<Test>]
    let ``Opening generative type providers directly in unqualified access to types and members - Errors`` () =
        let dir = Core.getTestsDirectory "typeProviders/helloWorld"

        let provider =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provider.fsx"))
            |> withName "provider"
            |> ignoreWarnings
            |> withOptions ["--langversion:preview"]

        let provided =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provided.fs"))
            |> withName "provided"
            |> ignoreWarnings
            |> withOptions ["--langversion:preview"]

        let test =
            Fsx """
open type FSharp.HelloWorldGenerative.TheContainerType<"TheOuterType">

let _ : TheNestedGeneratedType = Unchecked.defaultof<_>
            """
            |> asExe
            |> ignoreWarnings
            |> withOptions ["--langversion:preview"]
            |> withReferences [provider;provided]

        compile test
        |> withDiagnostics
            [
                (Error 3039, Line 2, Col 11, Line 2, Col 55, "A direct reference to the generated type 'TheContainerType' is not permitted. Instead, use a type definition, e.g. 'type TypeAlias = <path>'. This indicates that a type provider adds generated types to your assembly.")
                (Error 39, Line 4, Col 9, Line 4, Col 31, "The type 'TheNestedGeneratedType' is not defined. Maybe you want one of the following:
   TheGeneratedType1
   TheGeneratedType2
   TheGeneratedType4
   TheGeneratedType5
   TheGeneratedDelegateType")
            ]
        |> ignore

#endif
