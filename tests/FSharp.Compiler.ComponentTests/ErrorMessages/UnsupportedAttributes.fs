// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ErrorMessages

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Compiler.Assertions.StructuredResultsAsserts

module ``Unsupported Attributes`` =

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Warn successfully`` () =
        """
open System.Runtime.CompilerServices
let f (w, [<CallerArgumentExpression "w">] x : string) = ()
let [<ModuleInitializer>] g () = ()
type C() =

    [<System.Runtime.CompilerServices.ModuleInitializer>]
    member _.G() = ()
        """
        |> FSharp
        |> typecheck
        |> shouldFail
        |> withResults [
            { Error = Warning 202
              Range = { StartLine = 4
                        StartColumn = 7
                        EndLine = 4
                        EndColumn = 24 }
              Message =
               "This attribute is currently unsupported by the F# compiler. Applying it will not achieve its intended effect." }
            { Error = Warning 202
              Range = { StartLine = 7
                        StartColumn = 7
                        EndLine = 7
                        EndColumn = 56 }
              Message =
               "This attribute is currently unsupported by the F# compiler. Applying it will not achieve its intended effect." }
        ]