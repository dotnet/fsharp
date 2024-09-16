// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace Interop

open Xunit
open FSharp.Test.Compiler
open FSharp.Test
open System

module ``Deeply nested CSharpClasses`` =

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
let loss2 = MyNamespace.OoterClass.InnerClass.MoreInnerClass.somefunction()   //Note the mistyped functionname we expect a good error message
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
let loss2 = MyNamespace.OuterClass.InnerClass_.MoreInnerClass.somefunction()   //Note the mistyped InnerClass name we expect a good error message
"""
        FSharp fsharpSource
        |> asExe
        |> withReferences [cslib]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic (Error 39, Line 2, Col 36, Line 2, Col 47, "The type 'OuterClass' does not define the field, constructor or member 'InnerClass_'.")

    [<Fact>]
    let ``Missing type nested type moreinnerclass generates good message and range`` () =

        let fsharpSource =
            """
let loss2 = MyNamespace.OuterClass.InnerClass.MoareInnerClass.somefunction()   //Note the mistyped MoreInnerClass we expect a good error message
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
let loss2 = MyNamespace.OuterClass.InnerClass.MoreInnerClass.somefunction_()   //Note the mistyped somefunction we expect a good error message
"""
        FSharp fsharpSource
        |> asExe
        |> withReferences [cslib]
        |> compile
        |> shouldFail
        |> withSingleDiagnostic ((Error 39, Line 2, Col 62, Line 2, Col 75, """The type 'MoreInnerClass' does not define the field, constructor or member 'somefunction_'. Maybe you want one of the following:
   somefunction"""))
