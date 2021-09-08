// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Compiler


module ``Unit generic abstract Type`` =

    [<Fact>]
    let ``Unit can not be used as return type of abstract method paramete on return type``() =
        FSharp """
type EDF<'S> =
    abstract member Apply : int -> 'S
type SomeEDF () =
    interface EDF<unit> with
        member this.Apply d =
            // [ERROR] The member 'Apply' does not have the correct type to override the corresponding abstract method.
            ()
        """
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 17, Line 6, Col 21, Line 6, Col 26,
                                 "The member 'Apply: int -> unit' is specialized with 'unit' but 'unit' can't be used as return type of an abstract method parameterized on return type.")
