module FSharp.Compiler.ComponentTests.Signatures.StaticallyResolvedTypeParameterTests

open Xunit
open FsUnit
open FSharp.Test.Compiler
open FSharp.Compiler.ComponentTests.Signatures.TestHelpers

[<Fact>]
let ``Generic function with constraint`` () =
    FSharp
        """
module MyApp.GoodStuff

let inline toString< ^revision when ^revision: (static member GetCommitHash: ^revision -> string)>
    (p: System.Threading.Tasks.Task< ^revision >)
    : string =
    ""
"""
    |> printSignaturesWithPageWidth 80
    |> should
        equal
        """
module MyApp.GoodStuff

val inline toString:
  p: System.Threading.Tasks.Task< ^revision> -> string
    when ^revision: (static member GetCommitHash: ^revision -> string)"""

[<Fact>]
let ``Multiple constraints parameters in type definition`` () =
    FSharp
        """
module Foo

type Meh<'g, 'f> =
    class 
    end

let inline toString< ^revision>
    (p: Meh< ^revision, ^revision>)
    : string =
    ""
"""
    |> printSignaturesWithPageWidth 80
    |> should
        equal
        """
module Foo

type Meh<'g,'f> =
  class end

val inline toString: p: Meh< ^revision,^revision> -> string"""
