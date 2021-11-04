// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open FSharp.Compiler.Diagnostics
open NUnit.Framework
open FSharp.Test
open FSharp.Test.Utilities
open FSharp.Test.Compiler

[<TestFixture>]
module OpenTypeDeclarationTests =

    [<Literal>]
    let targetVersion = "5.0"

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
    let ``OpenSystemMathOnce - langversion:4.6`` () =
        Fsx (baseModule + """
module OpenSystemMathOnce =

               open type System.Math
               let x = Min(1.0, 2.0)""")
        |> withLangVersion46
        |> typecheck
        |> withDiagnostics
            [
                (Error 3350, Line 22, Col 16, Line 22, Col 37, "Feature 'open type declaration' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
                (Error 39, Line 23, Col 24, Line 23, Col 27, "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
            ]
        |> ignore

    [<Test>]
    let ``OpenSystemMathOnce - langversion:5.0`` () =
        Fsx (baseModule + """
module OpenSystemMathOnce =

                       open type System.Math
                       let x = Min(1.0, 2.0)""")
         |> withLangVersion50
         |> typecheck
         |> shouldSucceed
         |> ignore

    [<Test>]
    let ``OpenSystemMathTwice - langversion:4.6`` () =
        Fsx (baseModule + """
module OpenSystemMathTwice = 

    open type System.Math
    let x = Min(1.0, 2.0)

    open type System.Math
    let x2 = Min(2.0, 1.0)""")
        |> withLangVersion46
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
    let ``OpenSystemMathTwice - langversion:50`` () =
        Fsx (baseModule + """
module OpenSystemMathOnce =

                   open type System.Math
                   let x = Min(1.0, 2.0)""")
        |> withLangVersion50
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``OpenMyMathOnce - langversion:4.6`` () =
        Fsx (baseModule + """
module OpenMyMathOnce = 

    open type MyMath
    let x = Min(1.0, 2.0)
    let x2 = Min(1, 2)""")
        |> withLangVersion46
        |> typecheck
        |> withDiagnostics
            [
                (Error 3350, Line 22, Col 5, Line 22, Col 21, "Feature 'open type declaration' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
                (Error 39, Line 23, Col 13, Line 23, Col 16, "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
                (Error 39, Line 24, Col 14, Line 24, Col 17, "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
            ]
        |> ignore

    [<Test>]
    let ``OpenMyMathOnce - langversion:5.0`` () =
        Fsx (baseModule + """
module OpenMyMathOnce = 

    open type MyMath
    let x = Min(1.0, 2.0)
    let x2 = Min(1, 2)""")
        |> withLangVersion50
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``DontOpenAutoMath - langversion:4.6`` () =
        Fsx (baseModule + """
module DontOpenAutoMath = 

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)""")
        |> withLangVersion46
        |> typecheck
        |> withDiagnostics
            [
                (Error 39, Line 22, Col 13, Line 22, Col 20, "The value or constructor 'AutoMin' is not defined.")
                (Error 39, Line 23, Col 14, Line 23, Col 21, "The value or constructor 'AutoMin' is not defined.")
            ]
        |> ignore

    [<Test>]
    let ``DontOpenAutoMath - langversion:5.0`` () =
        Fsx (baseModule + """
module DontOpenAutoMath = 

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)""")
        |> withLangVersion50
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``OpenAutoMath - langversion:4.6`` () =
        Fsx (baseModule + """
module OpenAutoMath = 
    open type AutoOpenMyMath
    //open type NotAllowedToOpen

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)""")
        |> withLangVersion46
        |> typecheck
        |> withDiagnostics
            [
                (Error 3350, Line 21, Col 5, Line 21, Col 29, "Feature 'open type declaration' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
                (Error 39, Line 24, Col 13, Line 24, Col 20, "The value or constructor 'AutoMin' is not defined.")
                (Error 39, Line 25, Col 14, Line 25, Col 21, "The value or constructor 'AutoMin' is not defined.")
            ]
        |> ignore

    [<Test>]
    let ``OpenAutoMath - langversion:5.0`` () =
        Fsx (baseModule + """
module OpenAutoMath = 
    open type AutoOpenMyMath
    //open type NotAllowedToOpen

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)""")
        |> withLangVersion50
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``OpenAccessibleFields - langversion:5.0`` () =
        Fsx (baseModule + """
module OpenAFieldFromMath =
    open type System.Math
    
    let pi = PI""")
        |> withLangVersion50
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
        |> withLangVersion50
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
        |> withLangVersion50
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
        |> withLangVersion50
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
        |> withLangVersion50
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
        |> withLangVersion50
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
        |> withLangVersion50
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
        |> withLangVersion50
        |> withReferences [csharp]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Open generic type and use nested types as unqualified 4`` () =
        let csharp =
            CSharp """
namespace CSharpTest
{
    public class Test<T1, T2, T3, T4, T5>
    {
        public class NestedTest<T6, T7, U>
        {
            public class NestedNestedTest
            {
                public T7 A()
                {
                    return default(T7);
                }
            }

            public class NestedNestedTest<T8>
            {
                public T8 B()
                {
                    return default(T8);
                }
            }

            public class NestedNestedTest<T9, T10>
            {
                public T9 C()
                {
                    return default(T9);
                }
            }
        }
    }
}"""

        FSharp """
namespace FSharpTest

open System
open CSharpTest

open type Test<char, char, char, char, char>.NestedTest<int, string, uint64>

module Test =

    let aa : NestedNestedTest = NestedNestedTest()

    let bb : NestedNestedTest<int list> = NestedNestedTest<int list>()

    let cc : NestedNestedTest<float list, int64 list> = NestedNestedTest<float list, int64 list>()

    let r1 : string = aa.A()

    let r2 : int list = bb.B()

    let r3 : float list = cc.C()

open type Test<int, int16, uint16, byte, sbyte>

module Test2 =

    let a : NestedTest<string, uint32, uint64> = NestedTest<string, uint32, uint64>()

    let aa : NestedTest<string, uint32, uint64>.NestedNestedTest = NestedTest<string, uint32, uint64>.NestedNestedTest()

    let bb : NestedTest<string, int, uint64>.NestedNestedTest<int []> = NestedTest<string, int, uint64>.NestedNestedTest<int []>()

    let cc : NestedTest<string, int64, uint64>.NestedNestedTest<float [], int64 []> = NestedTest<string, int64, uint64>.NestedNestedTest<float [], int64 []>()

    let r1 : uint32 = aa.A()

    let r2 : int [] = bb.B()

    let r3 : float [] = cc.C()

module Test3 =

    let a : Test<byte, sbyte, uint16, int16, int>.NestedTest<uint32, int64, uint64> = Test<byte, sbyte, uint16, int16, int>.NestedTest<uint32, int64, uint64>()

    let aa : Test<byte, sbyte, uint16, int16, int>.NestedTest<uint32, int64, uint64>.NestedNestedTest = Test<byte, sbyte, uint16, int16, int>.NestedTest<uint32, int64, uint64>.NestedNestedTest()

    let bb : Test<byte, sbyte, uint16, int16, int>.NestedTest<uint32, int64, uint64>.NestedNestedTest<string> = Test<byte, sbyte, uint16, int16, int>.NestedTest<uint32, int64, uint64>.NestedNestedTest<string>()

    let cc : Test<byte, sbyte, uint16, int16, int>.NestedTest<uint32, int64, uint64>.NestedNestedTest<int list, int []> = Test<byte, sbyte, uint16, int16, int>.NestedTest<uint32, int64, uint64>.NestedNestedTest<int list, int []>()

    let r1 : int64 = aa.A()

    let r2 : string = bb.B()

    let r3 : int list = cc.C()
        """
        |> withLangVersion50
        |> withReferences [csharp]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Open unit of measure - Errors`` () =
        FSharp """
namespace FSharpTest

open System

[<Measure>]
type kg

open type kg
        """
        |> withLangVersion50
        |> compile
        |> withDiagnostics
            [
                (Error 704, Line 9, Col 11, Line 9, Col 13, "Expected type, not unit-of-measure")
            ]
        |> ignore

    [<Test>]
    let ``Open type with unit of measure`` () =
        FSharp """
namespace FSharpTest

open System
open System.Numerics

[<Measure>]
type kg

open type float<kg>

[<MeasureAnnotatedAbbreviation>]
type vec3<[<Measure>] 'Measure> = Vector3

open type vec3<kg>
        """
        |> withLangVersion50
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Open custom type with unit of measure`` () =
        FSharp """
namespace FSharpTest

[<Measure>]
type kg

type Custom<[<Measure>] 'Measure> =
    {
        X: float<'Measure>
        Y: float<'Measure>
    }

    static member GetX(c: Custom<'Measure>) = c.X

    static member GetY(c: Custom<'Measure>) = c.Y

open type Custom<kg>

module Test =

    let x : float<kg> = GetX(Unchecked.defaultof<_>)

    let y : float<kg> = GetY(Unchecked.defaultof<_>)

        """
        |> withLangVersion50
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Open custom type with unit of measure and more type params`` () =
        FSharp """
namespace FSharpTest

[<Measure>]
type kg

type Custom<'T, [<Measure>] 'Measure, 'U> =
    {
        X: float<'Measure>
        Y: float<'Measure>
        Z: 'T
        W: 'U
    }

    static member GetX(c: Custom<'T, 'Measure, 'U>) = c.X

    static member GetY(c: Custom<'T, 'Measure, 'U>) = c.Y

    static member GetZ(c: Custom<'T, 'Measure, 'U>) = c.Z

    static member GetW(c: Custom<'T, 'Measure, 'U>) = c.W

open type Custom<int, kg, float>

module Test =

    let x : float<kg> = GetX(Unchecked.defaultof<_>)

    let y : float<kg> = GetY(Unchecked.defaultof<_>)

    let z : int = GetZ(Unchecked.defaultof<_>)

    let w : float = GetW(Unchecked.defaultof<_>)
        """
        |> withLangVersion50
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Open custom type with unit of measure should error with measure mismatch`` () =
        FSharp """
namespace FSharpTest

[<Measure>]
type kg

[<Measure>]
type g

type Custom<[<Measure>] 'Measure> =
    {
        X: float<'Measure>
        Y: float<'Measure>
    }

    static member GetX(c: Custom<'Measure>) = c.X

    static member GetY(c: Custom<'Measure>) = c.Y

open type Custom<kg>

module Test =

    let x : float<g> = GetX(Unchecked.defaultof<_>)
        """
        |> withLangVersion50
        |> compile
        |> withErrorCode 1
        |> ignore

    [<Test>]
    let ``Open tuple - Errors`` () =
        FSharp """
namespace FSharpTest

open type (int * int)
        """
        |> withLangVersion50
        |> compile
        |> withDiagnostics
            [
                (Error 756, Line 4, Col 11, Line 4, Col 22, "'open type' may only be used with named types")
            ]
        |> ignore

    [<Test>]
    let ``Open struct tuple - Errors`` () =
        FSharp """
namespace FSharpTest

open type struct (int * int)
        """
        |> withLangVersion50
        |> compile
        |> withDiagnostics
            [
                (Error 756, Line 4, Col 11, Line 4, Col 29, "'open type' may only be used with named types")
            ]
        |> ignore

    [<Test>]
    let ``Open function - Errors`` () =
        FSharp """
namespace FSharpTest

open type (int -> int)
        """
        |> withLangVersion50
        |> compile
        |> withDiagnostics
            [
                (Error 756, Line 4, Col 11, Line 4, Col 23, "'open type' may only be used with named types")
            ]
        |> ignore

    [<Test>]
    let ``Open anon type - Errors`` () =
        FSharp """
namespace FSharpTest

open type {| x: int |}
        """
        |> withLangVersion50
        |> compile
        |> withDiagnostics
            [
                (Error 756, Line 4, Col 11, Line 4, Col 23, "'open type' may only be used with named types")
            ]
        |> ignore

    [<Test>]
    let ``Open struct anon type - Errors`` () =
        FSharp """
namespace FSharpTest

open type struct {| x: int |}
        """
        |> withLangVersion50
        |> compile
        |> withDiagnostics
            [
                (Error 756, Line 4, Col 11, Line 4, Col 30, "'open type' may only be used with named types")
            ]
        |> ignore

    [<Test>]
    let ``Open direct tuple - Errors`` () =
        // Note: `Tuple` is technically a named type but it gets decompiled into F#'s representation of a tuple in its type system.
        //       This test is to verify that behavior.
        FSharp """
namespace FSharpTest

open System

open type Tuple<int, int>
        """
        |> withLangVersion50
        |> compile
        |> withDiagnostics
            [
                (Error 756, Line 6, Col 11, Line 6, Col 26, "'open type' may only be used with named types")
            ]
        |> ignore

    [<Test>]
    let ``Open direct value tuple - Errors`` () =
        // Note: `ValueTuple` is technically a named type but it gets decompiled into F#'s representation of a struct tuple in its type system.
        //       This test is to verify that behavior.
        FSharp """
namespace FSharpTest

open System

open type ValueTuple<int, int>
        """
        |> withLangVersion50
        |> compile
        |> withDiagnostics
            [
                (Error 756, Line 6, Col 11, Line 6, Col 31, "'open type' may only be used with named types")
            ]
        |> ignore

    [<Test>]
    let ``Open direct function - Errors`` () =
        // Note: `FSharpFunc` is technically a named type but it gets decompiled into F#'s representation of a function in its type system.
        //       This test is to verify that behavior.
        FSharp """
namespace FSharpTest

open type FSharpFunc<int, int>
        """
        |> withLangVersion50
        |> compile
        |> withDiagnostics
            [
                (Error 756, Line 4, Col 11, Line 4, Col 31, "'open type' may only be used with named types")
            ]
        |> ignore

    [<Test>]
    let ``Open enum should have access to its cases`` () =
        FSharp """
namespace FSharpTest

type TestEnum =
    | EnumCase1 = 1
    | EnumCase2 = 2

open type TestEnum

module Test =

    let x = EnumCase1
    let y = EnumCase2
        """
        |> withLangVersion50
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Open C# enum should have access to its cases`` () =
        let csharp = 
            CSharp """
namespace CSharpTest
{
    public enum CSharpEnum
    {
        CSharpEnumCase1 = 1,
        CsharpEnumCase2 = 2
    }
}
            """

        FSharp """
namespace FSharpTest

open type CSharpTest.CSharpEnum

module Test =

    let x = CSharpEnumCase1
    let y = CSharpEnumCase2
        """
        |> withLangVersion50
        |> withReferences [csharp]
        |> compile
        |> ignore

    [<Test>]
    let ``Open union should have access to union cases`` () =
        FSharp """
namespace FSharpTest

module Test =

    type TestUnion =
        | UCase1
        | UCase2 with

    static member M() = ()

open type Test.TestUnion

module Test2 =

    let x = UCase1

    let y = M()
        """
        |> withLangVersion50
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Open generic union should have access to union cases with the enclosing type instantiations`` () =
        FSharp """
namespace FSharpTest

module Test =

    type TestUnion<'T> =
        | UCase1 of 'T
        | UCase2 with

    static member M() = ()

open type Test.TestUnion<int>

module Test2 =

    let x = UCase1 ""

    let y = M()
        """
        |> withLangVersion50
        |> compile
        |> withErrorCode 1
        |> ignore

    [<Test>]
    let ``Open generic union should have access to pattern union cases with the enclosing type instantiations - Errors`` () =
        FSharp """
namespace FSharpTest

module Test =

    type TestUnion<'T> =
        | UCase1 of 'T

open type Test.TestUnion<int>

module Test2 =

    let f x : string =
        match x with
        | UCase1 x -> x
        """
        |> withLangVersionPreview
        |> compile
        |> withErrorCode 1
        |> ignore

    [<Test>]
    let ``Open record should have access to construct record via labels`` () =
        FSharp """
namespace FSharpTest

module Test =

    type TestRecord = { X: int } with

        static member M() = ()

open type Test.TestRecord

module Test2 =

    let x = { X = 1 }

    let y = M()
        """
        |> withLangVersion50
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Open generic record should have access to construct record via labels with enclosing type instantiations`` () =
        FSharp """
namespace FSharpTest

module Test =

    type TestRecord<'T> = { X: 'T } with

        static member M() = ()

open Test

module Test2 =

    let x = { X = "" }

open type Test.TestRecord<int>

module Test3 =

    let x = { X = "" }

    let y = M()
        """
        |> withLangVersion50
        |> compile
        |> withErrorCode 1
        |> ignore

    [<Test>]
    let ``Open generic record should have access to pattern record via labels with enclosing type instantiations - Errors`` () =
        FSharp """
namespace FSharpTest

module Test =

    type TestRecord<'T> = { X: 'T }

open Test

module Test2 =

    let f x : string =
        match x with
        | { X = x } -> x

open type Test.TestRecord<int>

module Test3 =

    let f x : string =
        match x with
        | { X = x } -> x
        """
        |> withLangVersionPreview
        |> compile
        |> withErrorCode 1
        |> ignore

    [<Test>]
    let ``Open type should have no access to constructor - Errors`` () =
        FSharp """
namespace FSharpTest

module Test =

    type TestClass() =

        static member M() = ()

open type Test.TestClass

module Test2 =

    let x = TestClass()

    let y = M()
        """
        |> withLangVersion50
        |> compile
        |> withErrorCode 39
        |> ignore

    [<Test>]
    let ``Open type should combine both extension and intrinsic method groups`` () =
        FSharp """
namespace FSharpTest

type Test =

    static member M(_x: int) = ()

module Test =

    type Test with

        static member M(_x: float) : int = 5

open Test
open type Test

module Test2 =

    let test () : int =
        M(1)
        M(2.0)
        """
        |> withLangVersion50
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Open type should combine both extension and intrinsic method groups but error if extensions are added after opening the type`` () =
        FSharp """
namespace FSharpTest

type Test =

    static member M(_x: int) = ()

module Test =

    type Test with

        static member M(_x: float) : int = 5

open type Test
open Test

module Test2 =

    let test () : int =
        M(1)
        M(2.0)
        """
        |> withLangVersion50
        |> compile
        |> withErrorCodes [1;1]
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
        |> withLangVersion50
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
        |> withLangVersion50
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
        |> withLangVersion50
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
        |> withLangVersion50
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
            Compilation.Create(fsharpSource, Fs, Exe, options = [|"--langversion:5.0"|])

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
            Compilation.Create(fsharpSource, Fs, Exe, options = [|"--langversion:5.0"|])

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
            Compilation.Create(fsharpSource, Fs, Exe, options = [|"--langversion:5.0"|])

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
            Compilation.Create(fsharpSource, Fs, Exe, options = [|"--langversion:5.0"|])

        CompilerAssert.ExecutionHasOutput(fsCmpl, "M")

    [<Test>]
    let ``Opened types do no allow unqualified access to their inherited type's members - Error`` () =
        Fsx """
open type System.Math

let x = Equals(2.0, 3.0)
            """
        |> withLangVersion50
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
        |> withLangVersion50
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
        |> withLangVersion50
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
        |> withLangVersion50
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
        |> withLangVersion50
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    [<Test>]
    let ``Opened types with operators`` () =
        FSharp """
type A() =

    static member (+) (x: string, y: string) = x + y

open type A

[<EntryPoint>]
let main _ =
    let _x = 1 + 1
    0"""
        |> withLangVersion50
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
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:5.0"|], cmplRefs = [ilCmpl])

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
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:5.0"|], cmplRefs = [ilCmpl])

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
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:5.0"|], cmplRefs = [ilCmpl])

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
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:5.0"|], cmplRefs = [ilCmpl])

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
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

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
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl], name = "Test")

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
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp31)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Exe, options = [|"--langversion:5.0"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpDiagnosticSeverity.Error, 39, (9, 5, 9, 6), "The value or constructor 'M' is not defined.")
        |])

#endif

#if !NETCOREAPP

    [<Test>]
    let ``Opening type providers with abbreviation result in unqualified access to types and members`` () =
        let dir = getTestsDirectory __SOURCE_DIRECTORY__ "../../typeProviders/helloWorld"

        let provider =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provider.fsx"))
            |> withName "provider"
            |> ignoreWarnings
            |> withLangVersion50

        let provided =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provided.fs"))
            |> withName "provided"
            |> ignoreWarnings
            |> withLangVersion50

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
            |> withLangVersion50
            |> withReferences [provider;provided]

        compileAndRun test
        |> ignore

    [<Test>]
    let ``Opening type providers result in unqualified access to types and members`` () =
        let dir = getTestsDirectory __SOURCE_DIRECTORY__ "../../typeProviders/helloWorld"

        let provider =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provider.fsx"))
            |> withName "provider"
            |> ignoreWarnings
            |> withLangVersion50

        let provided =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provided.fs"))
            |> withName "provided"
            |> ignoreWarnings
            |> withLangVersion50

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
            |> withLangVersion50
            |> withReferences [provider;provided]

        compileAndRun test
        |> ignore

    [<Test>]
    let ``Opening type providers with nested result in unqualified access to types and members`` () =
        let dir = getTestsDirectory __SOURCE_DIRECTORY__ "../../typeProviders/helloWorld"

        let provider =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provider.fsx"))
            |> withName "provider"
            |> withLangVersion50
            |> ignoreWarnings
            |> withLangVersionPreview

        let provided =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provided.fs"))
            |> withName "provided"
            |> withLangVersion50
            |> ignoreWarnings
            |> withLangVersionPreview

        let test =
            Fsx """
open type FSharp.HelloWorld.HelloWorldTypeWithStaticInt32Parameter<1>.NestedType

if StaticProperty1 <> "You got a static property" then
    failwith "failed"
            """
            |> asExe
            |> ignoreWarnings
            |> withLangVersion50
            |> withReferences [provider;provided]

        compileAndRun test
        |> ignore

    [<Test>]
    let ``Opening generative type providers in unqualified access to types and members`` () =
        let dir = getTestsDirectory __SOURCE_DIRECTORY__ "../../typeProviders/helloWorld"

        let provider =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provider.fsx"))
            |> withName "provider"
            |> withLangVersion50
            |> ignoreWarnings
            |> withLangVersionPreview

        let provided =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provided.fs"))
            |> withName "provided"
            |> withLangVersion50
            |> ignoreWarnings
            |> withLangVersionPreview

        let test =
            Fsx """
type TheOuterType = FSharp.HelloWorldGenerative.TheContainerType<"TheOuterType">

open type TheOuterType

let _ : TheNestedGeneratedType = Unchecked.defaultof<_>
            """
            |> asExe
            |> withLangVersion50
            |> withReferences [provider;provided]
            |> ignoreWarnings

        compileAndRun test
        |> ignore

    [<Test>]
    let ``Opening generative type providers directly in unqualified access to types and members - Errors`` () =
        let dir = getTestsDirectory __SOURCE_DIRECTORY__ "../../typeProviders/helloWorld"

        let provider =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provider.fsx"))
            |> withName "provider"
            |> withLangVersion50
            |> ignoreWarnings
            |> withLangVersionPreview

        let provided =
            Fsx (sprintf """
#load @"%s"
            """ (dir ++ "provided.fs"))
            |> withName "provided"
            |> withLangVersion50
            |> ignoreWarnings
            |> withLangVersionPreview

        let test =
            Fsx """
open type FSharp.HelloWorldGenerative.TheContainerType<"TheOuterType">

let _ : TheNestedGeneratedType = Unchecked.defaultof<_>
            """
            |> asExe
            |> withLangVersion50
            |> withReferences [provider;provided]
            |> ignoreWarnings

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
