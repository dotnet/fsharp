// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CheckDeclarationsTests

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Xunit.Attributes

module CheckDeclarationsTests =

    [<Fact>]
    let ``CheckingSyntacticTypes - TcTyconDefnCore_CheckForCyclicStructsAndInheritance - Struct DU with Cyclic Tuple`` () =
        FSharp """
namespace FSharpTest

    [<Struct>]
    type Tree =
        | Empty
        | Children of Tree * Tree
"""
        |> compile
        |> shouldFail
        |> withErrorCode 954
        |> ignore

    [<Fact>]
    let ``CheckingSyntacticTypes - TcTyconDefnCore_CheckForCyclicStructsAndInheritance - Struct DU with Cyclic Struct Tuple`` () =
        FSharp """
namespace FSharpTest

    [<Struct>]
    type Tree =
        | Empty
        | Children of struct (Tree * Tree)
"""
        |> compile
        |> shouldFail
        |> withErrorCode 954
        |> ignore

    [<Fact>]
    let ``CheckingSyntacticTypes - TcTyconDefnCore_CheckForCyclicStructsAndInheritance - Struct DU with Cyclic Struct Tuple of int, Tree`` () =
        FSharp """
namespace FSharpTest

    [<Struct>]
    type Tree =
        | Empty
        | Children of struct (int * Tree)
"""
        |> compile
        |> shouldFail
        |> withErrorCode 954
        |> ignore

    [<Fact>]
    let ``CheckingSyntacticTypes - TcTyconDefnCore_CheckForCyclicStructsAndInheritance - Struct DU with Cyclic Tree`` () =
        FSharp """
namespace FSharpTest

    [<Struct>]
    type Tree =
        | Empty
        | Children of Tree
"""
        |> compile
        |> shouldFail
        |> withErrorCode 954
        |> ignore

    [<Fact>]
    let ``CheckingSyntacticTypes - TcTyconDefnCore_CheckForCyclicStructsAndInheritance - Struct DU with Non-cyclic Struct Tuple`` () =
        FSharp """
namespace FSharpTest

    [<Struct>]
    type NotATree =
        | Empty
        | Children of struct (int * string)
"""
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``CheckingSyntacticTypes - TcTyconDefnCore_CheckForCyclicStructsAndInheritance - Non-Struct DU Tree Cyclic Tree`` () =
        FSharp """
namespace FSharpTest

    type Tree =
        | Empty
        | Children of Tree
"""
        |> compile
        |> shouldSucceed
        |> ignore
