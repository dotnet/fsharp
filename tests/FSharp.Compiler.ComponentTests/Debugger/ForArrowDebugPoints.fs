// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Debugger

open Xunit
open FSharp.Test.Compiler
open Debugger.DebuggerTestHelpers

/// https://github.com/dotnet/fsharp/issues/13504
module ForArrowDebugPoints =

    [<Fact>]
    let ``For-arrow list comprehension body has debug point`` () =
        verifyAllSequencePoints """
module TestModule

let squares = [
    for x in [1; 2; 3] ->
        x * x
    ]
        """ [
            (Line 4, Col 1, Line 7, Col 6)
            (Line 5, Col 5, Line 5, Col 8)
            (Line 5, Col 11, Line 5, Col 13)
            (Line 6, Col 9, Line 6, Col 14)
            (Line 16707566, Col 0, Line 16707566, Col 0)
        ]

    [<Fact>]
    let ``For-arrow array comprehension body has debug point`` () =
        verifyAllSequencePoints """
module TestModule

let squares = [|
    for x in [|1; 2; 3|] ->
        x * x
    |]
        """ [
            (Line 4, Col 1, Line 7, Col 7)
            (Line 5, Col 5, Line 5, Col 8)
            (Line 5, Col 11, Line 5, Col 13)
            (Line 6, Col 9, Line 6, Col 14)
            (Line 16707566, Col 0, Line 16707566, Col 0)
        ]

    [<Fact>]
    let ``For-do-yield comprehension body has debug point`` () =
        verifyAllSequencePoints """
module TestModule

let squares = [
    for x in [1; 2; 3] do
        yield x * x
    ]
        """ [
            (Line 4, Col 1, Line 7, Col 6)
            (Line 5, Col 5, Line 5, Col 8)
            (Line 5, Col 11, Line 5, Col 13)
            (Line 6, Col 15, Line 6, Col 20)
            (Line 16707566, Col 0, Line 16707566, Col 0)
        ]

    // ---- H3 diagnostic: Are for-line and body-line sequence points in the SAME method? ----

    [<Fact>]
    let ``H3 - Simple arrow body is in same method as for-line`` () =
        FSharp """
module TestModule

let squares = [
    for x in [1; 2; 3] ->
        x * x
    ]
        """
        |> asLibrary
        |> withPortablePdb
        |> compile
        |> shouldSucceed
        |> verifyPdb [ VerifySequencePointsInSameMethod [ Line 5; Line 6 ] ]

    [<Fact>]
    let ``H3 - Multi-line arrow body is in same method as for-line`` () =
        FSharp """
module TestModule

let test1 () =
    [ 3; 2; 1 ]
    |> List.map (fun x -> x + 10)
    |> List.sort

let test3 = [
    for x in test1() ->
        let xx = x * x
        printf "test"
        xx
    ]
        """
        |> asLibrary
        |> withPortablePdb
        |> compile
        |> shouldSucceed
        |> verifyPdb [ VerifySequencePointsInSameMethod [ Line 10; Line 11; Line 12; Line 13 ] ]

    [<Fact>]
    let ``H3 - Multi-line do body is in same method as for-line`` () =
        FSharp """
module TestModule

let test1 () =
    [ 3; 2; 1 ]
    |> List.map (fun x -> x + 10)
    |> List.sort

let test2 = [
    for x in test1() do
        let xx = x * x
        printf "test"
        xx
    ]
        """
        |> asLibrary
        |> withPortablePdb
        |> compile
        |> shouldSucceed
        |> verifyPdb [ VerifySequencePointsInSameMethod [ Line 10; Line 11; Line 12; Line 13 ] ]

    // ---- H4 diagnostic: Do body methods have JMC-suppressing attributes? ----

    [<Fact>]
    let ``H4 - Arrow body method has no JMC-suppressing attributes`` () =
        FSharp """
module TestModule

let test1 () =
    [ 3; 2; 1 ]
    |> List.map (fun x -> x + 10)
    |> List.sort

let test3 = [
    for x in test1() ->
        let xx = x * x
        printf "test"
        xx
    ]
        """
        |> asLibrary
        |> withPortablePdb
        |> compile
        |> shouldSucceed
        |> verifyPdb [ VerifyNoDebuggerHiddenOnMethodWithLine (Line 11) ]

    [<Fact>]
    let ``H4 - Do body method has no JMC-suppressing attributes`` () =
        FSharp """
module TestModule

let test1 () =
    [ 3; 2; 1 ]
    |> List.map (fun x -> x + 10)
    |> List.sort

let test2 = [
    for x in test1() do
        let xx = x * x
        printf "test"
        xx
    ]
        """
        |> asLibrary
        |> withPortablePdb
        |> compile
        |> shouldSucceed
        |> verifyPdb [ VerifyNoDebuggerHiddenOnMethodWithLine (Line 11) ]

    // ---- H1 diagnostic: Does --realsig- change method placement? ----

    [<Fact>]
    let ``H1 - Arrow body same method with realsig off`` () =
        FSharp """
module TestModule

let test1 () =
    [ 3; 2; 1 ]
    |> List.map (fun x -> x + 10)
    |> List.sort

let test3 = [
    for x in test1() ->
        let xx = x * x
        printf "test"
        xx
    ]
        """
        |> asLibrary
        |> withPortablePdb
        |> withOptions ["--realsig-"]
        |> compile
        |> shouldSucceed
        |> verifyPdb [ VerifySequencePointsInSameMethod [ Line 10; Line 11; Line 12; Line 13 ] ]
