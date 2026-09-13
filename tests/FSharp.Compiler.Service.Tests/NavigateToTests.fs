module FSharp.Compiler.Service.Tests.NavigateToTests

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open Xunit

let private implementation =
    """
let value = 1
let curried a b = a + b
let tupledAndCurried (a, b) c = a + b + c
let takesUnit () = 1
let generic<'T> (x: 'T) = x

type C<'T, 'U>() =
    member _.Method(a: int, b: int) = a + b
    member _.Property = 1
    static member Static x y = x + y
"""

let private signature =
    """
module M

val curried: int -> int -> int
val takesUnit: unit -> int
val generic<'T> : 'T * 'T -> 'T

type C<'T> =
    member Method: a: int * b: int -> int
    abstract Abstract: unit -> unit
"""

let private arityOf (parseTree: ParsedInput) name =
    let item =
        NavigateTo.GetNavigableItems parseTree
        |> Array.find (fun item -> item.Name = name)

    item.ParameterCount, item.TypeParameterCount

[<Theory>]
[<InlineData("value", 0, 0)>]
[<InlineData("curried", 2, 0)>]
[<InlineData("tupledAndCurried", 3, 0)>]
[<InlineData("takesUnit", 0, 0)>]
[<InlineData("generic", 1, 1)>]
[<InlineData("C", 0, 2)>]
[<InlineData("Method", 2, 0)>]
[<InlineData("Property", 0, 0)>]
[<InlineData("Static", 2, 0)>]
let ``A declaration in an implementation file counts the parameters it compiles to`` (name: string, parameterCount: int, typeParameterCount: int) =
    Assert.Equal((parameterCount, typeParameterCount), arityOf (getParseResults implementation) name)

[<Theory>]
[<InlineData("curried", 2, 0)>]
[<InlineData("takesUnit", 0, 0)>]
[<InlineData("generic", 2, 1)>]
[<InlineData("C", 0, 1)>]
[<InlineData("Method", 2, 0)>]
[<InlineData("Abstract", 0, 0)>]
let ``A declaration in a signature file counts the parameters it compiles to`` (name: string, parameterCount: int, typeParameterCount: int) =
    Assert.Equal((parameterCount, typeParameterCount), arityOf (getParseResultsOfSignatureFile signature) name)
