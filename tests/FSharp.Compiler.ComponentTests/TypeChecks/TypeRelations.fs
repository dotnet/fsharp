module TypeChecks.TypeRelations

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

[<Theory; FileInlineData("CrgpLibrary.fs")>]
let ``Unsolved type variables are not cached`` compilation =
    compilation
    |> getCompilation
    |> typecheck
    |> shouldSucceed