module FSharp.Compiler.Service.Tests.PatternMatchCompilationTests

open NUnit.Framework

[<Test>]
let ``Simple 01 - Wild typed pat`` () =
    assertContainsSymbolsWithNames ["x"; "y"] """
match () with
| x -> let y = () in ()
"""

[<Test>]
let ``Wrong type 01`` () =
    assertContainsSymbolsWithNames ["x"; "y"] """
match () with
| "" -> ()
| x -> let y = () in ()
"""

[<Test>]
let ``Binding - Wrong type 01`` () =
    assertContainsSymbolsWithNames ["x"; "y"] """
let ("": unit), (x: int) = let y = () in ()
"""

[<Test>]
let ``Binding - Wrong item count`` () =
    assertContainsSymbolWithName "x" """
let x, y = 1, 2
"""

[<Test>]
let ``Missing case field 01`` () =
    assertContainsSymbolsWithNames ["x"; "y"] """
type U =
    | A
    | B of int * int * int

match A with
| B (1, 2) -> ()
| B (x, _, _) -> let y = () in ()
"""

[<Test>]
let ``Attributes 01 `` () =
    assertContainsSymbolsWithNames ["x"; "y"; "CompiledNameAttribute"] """
match () with
| [<CompiledName("Foo")>] x -> let y = () in ()
"""

[<Test>]
let ``Optional val 01 `` () =
    assertContainsSymbolsWithNames ["x"; "y"] """
match () with
| ?x -> let y = () in ()
"""
