// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Interop

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ParamArray =

    [<Fact>]
    let ``C# 13 params enhancements`` () =
        let csharp =
            CSharp """
using System;
using System.Collections.Generic;

namespace CSharpAssembly;

public class CS13ParamArray
{
    public static void WriteNames(params string[] names)
        => Console.WriteLine("First: " + string.Join(" + ", names));

    public static void WriteNames(params List<string> names)
        => Console.WriteLine("Second: " + string.Join(" + ", names));

    public static void WriteNames(params IEnumerable<string> names)
        => Console.WriteLine("Third: " + string.Join(" + ", names));
}"""
            |> withCSharpLanguageVersionPreview

        FSharp """
open System.Collections.Generic;
open CSharpAssembly

CS13ParamArray.WriteNames("Petr", "Jana")
CS13ParamArray.WriteNames(List["Petr"; "Jana"])
CS13ParamArray.WriteNames(["Petr"; "Jana"])
"""
        |> withReferences [csharp]
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "First: Petr + Jana"
            "Second: Petr + Jana"
            "Third: Petr + Jana"
        ]

    [<FactForNETCOREAPP>]
    let ``C# 13 params enhancements - ReadOnlySpan`` () =
        let csharp =
            CSharp """
using System;

namespace CSharpAssembly;

public class CS13ParamArray
{
    public static void WriteNames(params ReadOnlySpan<string> names)
        => Console.WriteLine(string.Join(" + ", names));
}"""
            |> withCSharpLanguageVersionPreview

        FSharp """
open System
open CSharpAssembly

CS13ParamArray.WriteNames(ReadOnlySpan([|"Petr"; "Jana"|]))
"""
        |> withReferences [csharp]
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [ "Petr + Jana" ]

    [<Fact>]
    let ``C# 13 params enhancements - error when no matching overload is available`` () =
        let csharp =
            CSharp """
using System;
using System.Collections.Generic;

namespace CSharpAssembly;

public class CS13ParamArray
{
    public static void WriteNames(params List<string> names)
        => Console.WriteLine("Second: " + string.Join(" + ", names));

    public static void WriteNames(params IEnumerable<string> names)
        => Console.WriteLine("Third: " + string.Join(" + ", names));
}"""
            |> withCSharpLanguageVersionPreview

        FSharp """
open CSharpAssembly

CS13ParamArray.WriteNames("Petr", "Jana")
"""
        |> withReferences [csharp]
        |> asExe
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 503, Line 4, Col 1, Line 4, Col 42,
            "A member or object constructor 'WriteNames' taking 2 arguments is not accessible from this code location. All accessible versions of method 'WriteNames' take 1 arguments.")
        ]