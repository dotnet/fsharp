// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open FSharp.Compiler.SourceCodeServices
open NUnit.Framework
open FSharp.Test.Utilities
open FSharp.Test.Utilities.Utilities

(*
    Tests in this file evaluate whether the language supports accessing functions on static classes using open
    The feature was added in preview, the test cases ensure that the original errors are reproduced when the langversion:4.6 is specified
*)

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
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:4.6" |]
            (baseModule + """
module OpenSystemMathOnce =

               open type System.Math
               let x = Min(1.0, 2.0)""")
            [|
                (FSharpErrorSeverity.Error, 3350, (22, 16, 22, 37), "Feature 'open type declaration' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
                (FSharpErrorSeverity.Error, 39, (23,24,23,27), "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
            |]

    [<Test>]
    let ``OpenSystemMathOnce - langversion:preview`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:preview" |]
            (baseModule + """
module OpenSystemMathOnce =

                       open type System.Math
                       let x = Min(1.0, 2.0)""")
            [| |]

    [<Test>]
    let ``OpenSystemMathTwice - langversion:v4_6`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:4.6" |]
            (baseModule + """
module OpenSystemMathTwice = 

    open type System.Math
    let x = Min(1.0, 2.0)

    open type System.Math
    let x2 = Min(2.0, 1.0)""")
            [|
                (FSharpErrorSeverity.Error, 3350, (22, 5, 22, 26), "Feature 'open type declaration' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
                (FSharpErrorSeverity.Error, 39, (23,13,23,16), "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
                (FSharpErrorSeverity.Error, 3350, (25, 5, 25, 26), "Feature 'open type declaration' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
                (FSharpErrorSeverity.Error, 39, (26,14,26,17), "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
            |]

    [<Test>]
    let ``OpenSystemMathTwice - langversion:preview`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:preview" |]
            (baseModule + """
module OpenSystemMathOnce =

                   open type System.Math
                   let x = Min(1.0, 2.0)""")
            [| |]

    [<Test>]
    let ``OpenMyMathOnce - langversion:v4_6`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:4.6" |]
            (baseModule + """
module OpenMyMathOnce = 

    open type MyMath
    let x = Min(1.0, 2.0)
    let x2 = Min(1, 2)""")
            [|
                (FSharpErrorSeverity.Error, 3350, (22, 5, 22, 21), "Feature 'open type declaration' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
                (FSharpErrorSeverity.Error, 39, (23,13,23,16), "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
                (FSharpErrorSeverity.Error, 39, (24,14,24,17), "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
            |]

    [<Test>]
    let ``OpenMyMathOnce - langversion:preview`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:preview" |]
            (baseModule + """
module OpenMyMathOnce = 

    open type MyMath
    let x = Min(1.0, 2.0)
    let x2 = Min(1, 2)""")
            [| |]

    [<Test>]
    let ``DontOpenAutoMath - langversion:v4_6`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:4.6" |]
            (baseModule + """
module DontOpenAutoMath = 

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)""")
            [|
                (FSharpErrorSeverity.Error, 39, (22,13,22,20), "The value or constructor 'AutoMin' is not defined.")
                (FSharpErrorSeverity.Error, 39, (23,14,23,21), "The value or constructor 'AutoMin' is not defined.")
            |]

    [<Test>]
    let ``DontOpenAutoMath - langversion:preview`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:preview" |]
            (baseModule + """
module DontOpenAutoMath = 

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)""")
            [| |]

    [<Test>]
    let ``OpenAutoMath - langversion:v4_6`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:4.6" |]
            (baseModule + """
module OpenAutoMath = 
    open type AutoOpenMyMath
    //open type NotAllowedToOpen

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)""")
            [|
                (FSharpErrorSeverity.Error, 3350, (21, 5, 21, 29), "Feature 'open type declaration' is not available in F# 4.6. Please use language version " + targetVersion + " or greater.")
                (FSharpErrorSeverity.Error, 39, (24,13,24,20), "The value or constructor 'AutoMin' is not defined.")
                (FSharpErrorSeverity.Error, 39, (25,14,25,21), "The value or constructor 'AutoMin' is not defined.")
            |]

    [<Test>]
    let ``OpenAutoMath - langversion:preview`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:preview" |]
            (baseModule + """
module OpenAutoMath = 
    open type AutoOpenMyMath
    //open type NotAllowedToOpen

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)""")
            [| |]

    [<Test>]
    let ``OpenAccessibleFields - langversion:preview`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:preview" |]
            (baseModule + """
module OpenAFieldFromMath =
    open type System.Math
    
    let pi = PI""")
            [||]

    [<Test>]
    let ``Open type and use nested types as unqualified`` () =
        let csharpSource =
            """
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
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open type CSharpTest.Test

module Test =
    let x = NestedTest()
    let y = NestedTest<int>()
    let a = x.A()
    let b = y.B()
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:preview"|], cmplRefs = [csCmpl])

        CompilerAssert.Compile(fsCmpl)

    [<Test>]
    let ``Open a type where the type declaration uses a type abbreviation as a qualifier to a real nested type`` () =
        let csharpSource =
            """
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
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open System
type Abbrev = CSharpTest.Test
open type Abbrev.NestedTest
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:preview"|], cmplRefs = [csCmpl])

        CompilerAssert.Compile(fsCmpl)

    [<Test>]
    let ``Open a type where the type declaration uses a type abbreviation`` () =
        let csharpSource =
            """
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
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open System
type Abbrev = CSharpTest.Test
open type Abbrev
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:preview"|], cmplRefs = [csCmpl])

        CompilerAssert.Compile(fsCmpl)

    [<Test>]
    let ``Open a nested type as qualified`` () =
        let csharpSource =
            """
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
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open System
open type CSharpTest.Test.NestedTest

module Test =
    let x = A()
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:preview"|], cmplRefs = [csCmpl])

        CompilerAssert.Compile(fsCmpl)

    [<Test>]
    let ``Open generic type and use nested types as unqualified`` () =
        let csharpSource =
            """
using System;

namespace CSharpTest
{
    public static class Test<T>
    {
        public class NestedTest
        {
            public T A()
            {
                return default(T);
            }
        }

        public class NestedTest<U>
        {
            public U B()
            {
                return default(U);
            }
        }
    }
}
            """

        let fsharpSource =
            """
namespace FSharpTest

open System
open type CSharpTest.Test<byte>

module Test =
    let x = NestedTest<byte, int>()
    let xb = x.B()

    let y = NestedTest<byte>()
    let ya = y.A()
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:preview"|], cmplRefs = [csCmpl])

        CompilerAssert.Compile(fsCmpl)

    [<Test>]
    let ``Using the 'open' declaration on a possible type identifier - Error`` () =
        let csharpSource =
            """
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

        let fsharpSource =
            """
namespace FSharpTest

open System
open CSharpTest.Test

module Test =
    let x = A()
            """

        let csCmpl =
            CompilationUtil.CreateCSharpCompilation(csharpSource, CSharpLanguageVersion.CSharp8, TargetFramework.NetCoreApp30)
            |> CompilationReference.Create

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:preview"|], cmplRefs = [csCmpl])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpErrorSeverity.Error, 39, (5, 17, 5, 21), "The namespace 'Test' is not defined.")
            (FSharpErrorSeverity.Error, 39, (8, 13, 8, 14), "The value or constructor 'A' is not defined.")
        |])

    [<Test>]
    let ``Open type declaration on a namespace - Error`` () =
        let fsharpSource =
            """
namespace FSharpTest

open type System
            """

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:preview"|])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpErrorSeverity.Error, 39, (4, 11, 4, 17), "The type 'System' is not defined.")
        |])

    [<Test>]
    let ``Open type declaration on a module - Error`` () =
        let fsharpSource =
            """
namespace FSharpTest

open type FSharp.Core.Option
            """

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:preview"|])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpErrorSeverity.Error, 33, (4, 11, 4, 29), "The type 'Microsoft.FSharp.Core.Option<_>' expects 1 type argument(s) but is given 0")
        |])

    [<Test>]
    let ``Open type declaration on a byref - Error`` () =
        let fsharpSource =
            """
namespace FSharpTest

open type byref<int>
open type inref<int>
open type outref<int>
            """

        let fsCmpl =
            Compilation.Create(fsharpSource, Fs, Library, options = [|"--langversion:preview"|])

        CompilerAssert.CompileWithErrors(fsCmpl, [|
            (FSharpErrorSeverity.Error, 3252, (4, 11, 4, 21), "Byref types are not allowed in an open type declaration.")
            (FSharpErrorSeverity.Error, 3252, (5, 11, 5, 21), "Byref types are not allowed in an open type declaration.")
            (FSharpErrorSeverity.Error, 3252, (6, 11, 6, 22), "Byref types are not allowed in an open type declaration.")
        |])

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
