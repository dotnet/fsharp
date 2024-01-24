namespace TypeChecks

open Xunit
open NUnit.Framework
open FSharp.Test
open FSharp.Test.Compiler

module PatternMatchTests =

    [<Fact>]
    let ``Over 9000 match clauses`` () =
        let max = 9001

        let aSource =
            let me =
                [ 0 .. max ]
                |> List.map (fun i -> $"    | %i{i} -> %i{i} + 1")
                |> String.concat "\n"
                |> sprintf """let f (a: int) : int =
    match a with
%s
    | i -> i + 1
            """

            $"module A\n\n%s{me}"

        let bSource = """module B

open A

let g = f 0
"""

        FSharp aSource
        |> withAdditionalSourceFile (FsSource bSource)
        |> typecheckResults
        |> fun results ->
            Assert.IsEmpty results.Diagnostics
