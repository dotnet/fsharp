// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open Xunit
open FSharp.Test
open FSharp.Compiler.Diagnostics


module ``List Tests`` =

    [<Fact>]
    let ``List hd should not exist``() =
        // Regression test for FSharp1.0:5641
        // Title: List.hd/tl --> List.head/tail

        CompilerAssert.TypeCheckSingleError
            """
List.hd [1] |> ignore
            """
            FSharpDiagnosticSeverity.Error
            39
            (2, 6, 2, 8)
            "The value, constructor, namespace or type 'hd' is not defined."

        
        
    [<Fact>]
    let ``List tl should not exist``() =
        // Regression test for FSharp1.0:5641
        // Title: List.hd/tl --> List.head/tail
        
        CompilerAssert.TypeCheckSingleError
            """
List.tl [1] |> ignore
            """
            FSharpDiagnosticSeverity.Error
            39
            (2, 6, 2, 8)
            "The value, constructor, namespace or type 'tl' is not defined."

    [<Fact>]
    let ``List head of empty list``() =
        let testDelegate = fun _ -> (List.head [] |> ignore)

        Assert.Throws<System.ArgumentException> testDelegate |> ignore

    [<Fact>]
    let ``List tail of empty list``() =
        let testDelegate = fun _ -> (List.tail [] |> ignore)

        Assert.Throws<System.ArgumentException> testDelegate |> ignore

    [<Fact>]
    let ``List head and tail``() =
        Assert.areEqual 1 (List.head [1 .. 10])
        Assert.areEqual "a" (List.head ["a"])
        Assert.areEqual [2 .. 10] (List.tail [1 .. 10])
        Assert.areEqual [] (List.tail [1])
        Assert.areEqual (List.head (List.tail ['a'; 'a'])) (List.head ['a'; 'a'])
