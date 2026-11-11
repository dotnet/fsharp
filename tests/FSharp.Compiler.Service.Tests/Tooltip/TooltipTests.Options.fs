module FSharp.Compiler.Service.Tests.TooltipOptionsTests

open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Test

let private multilineParameters = { FSharpToolTipOptions.Default with MultilineParameters = true }

let private nullableCompilerOptions = [| "--checknulls+"; "--langversion:preview" |]

let private normalizeExpected (expected: string) = expected.Replace("\r\n", "\n")

let private getTooltipWithCompilerOptions compilerOptions (options: FSharpToolTipOptions) source =
    let context = Checker.getResolveContext source
    let _, checkResults = getParseAndCheckResultsWithOptions compilerOptions context.Source
    checkResults.GetTooltip(context, options)

let private assertTooltipWithCompilerOptions compilerOptions options expected source =
    let text, _, _ = getTooltipWithCompilerOptions compilerOptions options source |> assertAndExtractTooltip
    text |> Assert.shouldBeEquivalentTo (normalizeExpected expected)

let private assertTooltip options expected source =
    assertTooltipWithCompilerOptions [||] options expected source

let private curriedFunctionSource = """
module Foo

let ba{caret}r (fileName: string) (version: int) = 0
"""

[<Theory>]
[<InlineData(false, true, "static member AssertionExtensions.ContainExactlyOneItemMatching: sequence: 'a seq * predicate: ('a -> bool) * [<Optional; DefaultParameterValue ((null :> obj))>] because: string -> 'a")>]
[<InlineData(false, false, "static member AssertionExtensions.ContainExactlyOneItemMatching: sequence: 'a seq * predicate: ('a -> bool) * because: string -> 'a")>]
[<InlineData(true, false, "static member AssertionExtensions.ContainExactlyOneItemMatching:\n   sequence : 'a seq *\n   predicate: ('a -> bool) *\n   because  : string\n           -> 'a")>]
let ``Method parameters with attributes`` multiline showAttributes expected =
    assertTooltip { MultilineParameters = multiline; ShowAttributes = showAttributes } expected """
module Foo

open System.Runtime.InteropServices

type AssertionExtensions =
    static member ContainExactlyOne{caret}ItemMatching(sequence: seq<'a>, predicate: 'a -> bool, [<Optional; DefaultParameterValue(null)>] because: string) : 'a =
        Seq.find predicate sequence
"""

[<TheoryForNETCOREAPP>]
[<InlineData("let success, version = System.Version.TryPar{caret}se(null)", "System.Version.TryParse(input: string | null, result: byref<System.Version | null>) : bool")>]
[<InlineData("let fileName = System.IO.Path.GetFile{caret}Name(null: string)", "System.IO.Path.GetFileName(path: string | null) : string | null")>]
let ``.NET code analysis attributes are hidden when attributes are turned off`` usage expected =
    assertTooltipWithCompilerOptions nullableCompilerOptions { FSharpToolTipOptions.Default with ShowAttributes = false } expected $"""
module Foo

{usage}
"""

[<Fact>]
let ``Curried function parameters are put on separate lines`` () =
    assertTooltip multilineParameters """val bar:
   fileName: string ->
   version : int
          -> int""" curriedFunctionSource

let private classWithConstructorSource usage = $"""
module Foo

type C(a: int, b: string) =
    member _.M(x: int, y: string) = a + x

{usage}
"""

let private constructorTooltip = """new:
   a: int *
   b: string
   -> C"""

let private classTooltip = """type C =
  new: a: int * b: string -> C
  member M: x: int * y: string -> int"""

[<Fact>]
let ``Constructor call parameters are put on separate lines`` () =
    assertTooltip multilineParameters constructorTooltip (classWithConstructorSource "let c = C{caret}(1, \"\")")

[<Fact>]
let ``Constructor call with new shows the constructor with parameters on separate lines and the type`` () =
    let (ToolTipText items) = getTooltipWithCompilerOptions [||] multilineParameters (classWithConstructorSource "let c = new C{caret}(1, \"\")")

    [ for item in items do
        match item with
        | ToolTipElement.Group elements -> for element in elements -> element.MainDescription.Text
        | _ -> () ]
    |> Assert.shouldBeEquivalentTo [ normalizeExpected constructorTooltip; normalizeExpected classTooltip ]

[<Fact>]
let ``Constructor and method parameters stay on the same line in the tooltip of the type`` () =
    assertTooltip multilineParameters classTooltip (classWithConstructorSource "let f (c: C{caret}) = c")

[<Fact>]
let ``Optional parameter is aligned with the others`` () =
    assertTooltip multilineParameters """static member C.M:
   y : int *
   ?x: int
    -> unit""" """
module Foo

type C =
    static member M(y: int, ?x: int) = ()

C.M{caret}(1)
"""

[<Fact>]
let ``.NET method parameters are put on separate lines`` () =
    assertTooltip multilineParameters """System.Char.IsSurrogatePair(
   highSurrogate: char,
   lowSurrogate : char
) : bool""" """
module Foo

let b = System.Char.IsSurrogate{caret}Pair('a', 'b')
"""

[<Fact>]
let ``Single parameter stays on the same line`` () =
    assertTooltip multilineParameters "val f: x: int -> int" """
module Foo

let f{caret} (x: int) = x
"""

[<Fact>]
let ``Parameters stay on the same line by default`` () =
    assertTooltip FSharpToolTipOptions.Default "val bar: fileName: string -> version: int -> int" curriedFunctionSource
