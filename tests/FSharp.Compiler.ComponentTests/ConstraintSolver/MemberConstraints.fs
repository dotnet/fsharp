// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ConstraintSolver.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Test.Utilities.Compiler
open FSharp.Compiler.SourceCodeServices

module MemberConstraints =
    
    [<Fact>]
    let ``Invalid member constraint with ErrorRanges``() = // Regression test for FSharp1.0:2262
        FSharp """
 let inline length (x: ^a) : int = (^a : (member Length : int with get, set) (x, ()))
        """
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withError (697, (2, 43, 2, 76), "Invalid constraint")
