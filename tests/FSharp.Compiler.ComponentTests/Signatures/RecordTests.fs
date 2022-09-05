module FSharp.Compiler.ComponentTests.Signatures.RecordTests

open Xunit
open FsUnit
open FSharp.Test.Compiler
open FSharp.Compiler.ComponentTests.Signatures.TestHelpers

[<Fact>]
let ``Internal record with xml comment`` () =
    FSharp
        """
module SignatureFileGeneration.MyModule

type PullActions =
    internal
        {
            /// Any repo which doesn't have a master branch will have one created for it.
            Log : int
        }
"""
    |> printSignaturesWith 80
    |> should
        equal
        """
module SignatureFileGeneration.MyModule

type PullActions =
  internal
    {

      /// Any repo which doesn't have a master branch will have one created for it.
      Log: int
    }"""
