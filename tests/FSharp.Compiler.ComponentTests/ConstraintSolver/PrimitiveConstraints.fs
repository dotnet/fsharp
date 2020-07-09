// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ConstraintSolver.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Test.Utilities.Compiler
open FSharp.Compiler.SourceCodeServices

module PrimitiveConstraints =

    [<Fact>]
    /// Title: Type checking oddity
    ///
    /// This suggestion was resolved as by design,
    /// so the test makes sure, we're emitting error message about 'not being a valid object construction expression'
    let ``Invalid object constructor`` () = // Regression test for FSharp1.0:4189
        baseline
            ((__SOURCE_DIRECTORY__ ++ "../testables/"), "typecheck/constructors/neg_invalid_constructor.fs")
            |> withOptions ["--test:ErrorRanges"]
            |> typecheck
