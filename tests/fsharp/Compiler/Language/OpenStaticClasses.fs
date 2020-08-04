// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open FSharp.Compiler.SourceCodeServices
open NUnit.Framework
open FSharp.Test.Utilities


(*
    Tests in this file evaluate whether the language supports accessing functions on static classes using open
    The feature was added in preview, the test cases ensure that the original errors are reproduced when the langversion:4.6 is specified
*)

[<TestFixture>]
module OpenStaticClassesTests =

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
    let ``OpenStaticClassesTests - OpenSystemMathOnce - langversion:v4.6`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:4.6" |]
            (baseModule + """
module OpenSystemMathOnce =

               open System.Math
               let x = Min(1.0, 2.0)""")
            [|
                (FSharpErrorSeverity.Error, 39, (22,28,22,32), "The namespace 'Math' is not defined.");
                (FSharpErrorSeverity.Error, 39, (23,24,23,27), "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
            |]

    [<Test>]
    let ``OpenStaticClassesTests - OpenSystemMathOnce - langversion:preview`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:preview" |]
            (baseModule + """
module OpenSystemMathOnce =

                       open System.Math
                       let x = Min(1.0, 2.0)""")
            [| |]

    [<Test>]
    let ``OpenStaticClassesTests - OpenSystemMathTwice - langversion:v4.6`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:4.6" |]
            (baseModule + """
module OpenSystemMathTwice = 

    open System.Math
    let x = Min(1.0, 2.0)

    open System.Math
    let x2 = Min(2.0, 1.0)""")
            [|
                (FSharpErrorSeverity.Error, 39, (22,17,22,21), "The namespace 'Math' is not defined.");
                (FSharpErrorSeverity.Error, 39, (23,13,23,16), "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
                (FSharpErrorSeverity.Error, 39, (25,17,25,21), "The namespace 'Math' is not defined.");
                (FSharpErrorSeverity.Error, 39, (26,14,26,17), "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
            |]

    [<Test>]
    let ``OpenStaticClassesTests - OpenSystemMathTwice - langversion:preview`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:preview" |]
            (baseModule + """
module OpenSystemMathOnce =

                   open System.Math
                   let x = Min(1.0, 2.0)""")
            [| |]

    [<Test>]
    let ``OpenStaticClassesTests - OpenMyMathOnce - langversion:v4.6`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:4.6" |]
            (baseModule + """
module OpenMyMathOnce = 

    open MyMath
    let x = Min(1.0, 2.0)
    let x2 = Min(1, 2)""")
            [|
                (FSharpErrorSeverity.Error, 39, (22,10,22,16), "The namespace or module 'MyMath' is not defined. Maybe you want one of the following:\r\n   Math");
                (FSharpErrorSeverity.Error, 39, (23,13,23,16), "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
                (FSharpErrorSeverity.Error, 39, (24,14,24,17), "The value or constructor 'Min' is not defined. Maybe you want one of the following:\r\n   min\r\n   sin")
            |]

    [<Test>]
    let ``OpenStaticClassesTests - OpenMyMathOnce - langversion:preview`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:preview" |]
            (baseModule + """
module OpenMyMathOnce = 

    open MyMath
    let x = Min(1.0, 2.0)
    let x2 = Min(1, 2)""")
            [| |]

    [<Test>]
    let ``OpenStaticClassesTests - DontOpenAutoMath - langversion:v4.6`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:4.6" |]
            (baseModule + """
module DontOpenAutoMath = 

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)""")
            [|
                (FSharpErrorSeverity.Error, 39, (22,13,22,20), "The value or constructor 'AutoMin' is not defined.");
                (FSharpErrorSeverity.Error, 39, (23,14,23,21), "The value or constructor 'AutoMin' is not defined.")
            |]

    [<Test>]
    let ``OpenStaticClassesTests - DontOpenAutoMath - langversion:preview`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:preview" |]
            (baseModule + """
module DontOpenAutoMath = 

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)""")
            [| |]

    [<Test>]
    let ``OpenStaticClassesTests - OpenAutoMath - langversion:v4.6`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:4.6" |]
            (baseModule + """
module OpenAutoMath = 
    open AutoOpenMyMath
    //open NotAllowedToOpen

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)""")
            [|
                (FSharpErrorSeverity.Error, 39, (21,10,21,24), "The namespace or module 'AutoOpenMyMath' is not defined.");
                (FSharpErrorSeverity.Error, 39, (24,13,24,20), "The value or constructor 'AutoMin' is not defined.")
                (FSharpErrorSeverity.Error, 39, (25,14,25,21), "The value or constructor 'AutoMin' is not defined.")
            |]

    [<Test>]
    let ``OpenStaticClassesTests - OpenAutoMath - langversion:preview`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:preview" |]
            (baseModule + """
module OpenAutoMath = 
    open AutoOpenMyMath
    //open NotAllowedToOpen

    let x = AutoMin(1.0, 2.0)
    let x2 = AutoMin(1, 2)""")
            [| |]

    [<Test>]
    let ``OpenStaticClassesTests - OpenAccessibleFields - langversion:preview`` () =
        CompilerAssert.TypeCheckWithErrorsAndOptions
            [| "--langversion:preview" |]
            (baseModule + """
module OpenAFieldFromMath =
    open System.Math
    
    let pi = PI""")
            [||]

    // TODO - wait for Will's integration of testing changes that makes this easlier
    // [<Test>]
    // let ``OpenStaticClassesTests - InternalsVisibleWhenHavingAnIVT - langversion:preview``() = ...
