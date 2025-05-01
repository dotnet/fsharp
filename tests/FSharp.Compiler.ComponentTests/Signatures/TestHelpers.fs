module Signatures.TestHelpers

open System
open Xunit
open FSharp.Test.Compiler

let prependNewline v = String.Concat("\n", v)

let assertEqualIgnoreLineEnding (x: string) (y: string) =
    Assert.Equal(x, y, ignoreLineEndingDifferences = true)

let assertSingleSignatureBinding implementation signature =
    FSharp $"module A\n\n{implementation}"
    |> printSignatures
    |> assertEqualIgnoreLineEnding $"\nmodule A\n\n{signature}"
