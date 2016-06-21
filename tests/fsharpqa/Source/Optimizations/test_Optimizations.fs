module ``FSharpQA-Tests-Optimizations``

open NUnit.Framework

open NUnitConf
open RunPlTest



module AssemblyBoundary =

    [<Test; FSharpQASuiteTest("Optimizations/AssemblyBoundary")>]
    let AssemblyBoundary () = runpl |> check



module ForLoop =

    [<Test; FSharpQASuiteTest("Optimizations/ForLoop")>]
    let ForLoop () = runpl |> check


module GenericComparison =

    [<Test; FSharpQASuiteTest("Optimizations/GenericComparison")>]
    let GenericComparison () = runpl |> check


module Inlining =

    [<Test; FSharpQASuiteTest("Optimizations/Inlining")>]
    let Inlining () = runpl |> check

