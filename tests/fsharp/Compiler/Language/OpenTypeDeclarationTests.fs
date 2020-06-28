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

    // TODO - wait for Will's integration of testing changes that makes this easlier
    // [<Test>]
    // let ``OpenStaticClassesTests - InternalsVisibleWhenHavingAnIVT - langversion:preview``() = ...
