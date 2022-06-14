namespace FSharp.Compiler.ComponentTests.ConstraintSolver

open Xunit
open FSharp.Test.Compiler

module ObjInference =

    [<Fact>]
    let ``Inference of obj``() =
        FSharp """
let f() = ([] = [])
        """
        |> withErrorRanges
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 3524, Line 2, Col 17, Line 2, Col 19, "A type was not refined away from `obj`, which may be unintended. Consider adding explicit type annotations.")
