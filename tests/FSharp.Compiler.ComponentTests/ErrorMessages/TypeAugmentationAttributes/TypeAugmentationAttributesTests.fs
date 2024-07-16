module FSharp.Compiler.ComponentTests.ErrorMessages.TypeAugmentationAttributes
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"attributes.on.nonok.types.fsx"|])>]
let ``attributes on non OK types`` compilation =
    compilation
    |> asFsx
    |> verifyBaselines
    |> compile
    |> shouldFail