// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.ComponentTests.Interop

open Xunit
open FSharp.Test.Compiler
open FSharp.Test
open System

module ``Deeply nested CSharpClasses`` =

//Expect error similar to:
//(8,29): error FS0039: The value, constructor, namespace or type 'somefunctoin' is not defined. Maybe you want one of the following:   somefunction

    let cslib =
        CSharp """
using System;

namespace MyNamespace
{
    public class OuterClass
    {

        public class InnerClass
        {

            public class MoreInnerClass
            {
                public static void somefunction() { }
            }
        }
    }
}"""

    [<Fact>]
    let ``Missing type outerclass generates good message and range`` () =

        let fsharpSource =
            """
let loss2 = MyNamespace.OoterClass.InnerClass.MoreInnerClass.somefunction()   //Note the miss-typed functionname we expect a good error message
"""
        FSharp fsharpSource
        |> asExe
        |> withReferences [cslib]
        |> compile
        |> withSingleDiagnostic (Error 39, Line 2, Col 25, Line 2, Col 35, "The value, constructor, namespace or type 'OoterClass' is not defined. Maybe you want one of the following:
   OuterClass")

    [<Fact>]
    let ``Missing type nested type innerclass generates good message and range`` () =

        let fsharpSource =
            """
let loss2 = MyNamespace.OuterClass.InerClass.MoreInnerClass.somefunction()   //Note the miss-typed InnerClass name we expect a good error message
"""
        FSharp fsharpSource
        |> asExe
        |> withReferences [cslib]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 39, Line 2, Col 36, Line 2, Col 45, "The type 'OuterClass' does not define the field, constructor or member 'InerClass'.")

    [<Fact>]
    let ``Missing type nested type moreinnerclass generates good message and range`` () =

        let fsharpSource =
            """
let loss2 = MyNamespace.OuterClass.InnerClass.MoareInnerClass.somefunction()   //Note the miss-typed MoreInnerClass we expect a good error message
"""
        FSharp fsharpSource
        |> asExe
        |> withReferences [cslib]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 39, Line 2, Col 47, Line 2, Col 62, "The type 'InnerClass' does not define the field, constructor or member 'MoareInnerClass'.")

    [<Fact>]
    let ``Missing function generates good message and range`` () =

        let fsharpSource =
            """
let loss2 = MyNamespace.OuterClass.InnerClass.MoreInnerClass.somefunctoion()   //Note the miss-typed somefunction we expect a good error message
"""
        FSharp fsharpSource
        |> asExe
        |> withReferences [cslib]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic ((Error 39, Line 2, Col 62, Line 2, Col 75, """The type 'MoreInnerClass' does not define the field, constructor or member 'somefunctoion'. Maybe you want one of the following:
   somefunction"""))
