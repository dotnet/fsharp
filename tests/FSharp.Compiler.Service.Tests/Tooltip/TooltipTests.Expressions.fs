module FSharp.Compiler.Service.Tests.TooltipExpressionsTests

open System
open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Tokenization

let private assertOperatorTooltipContains (expected: string) (operatorName: string) (markedSource: string) =
    let context = SourceContext.fromMarkedSource markedSource
    let _, checkResults = getParseAndCheckResults context.Source

    checkResults.GetToolTip(context.CaretPos.Line, context.CaretPos.Column + 1, context.LineText, [ operatorName ], FSharpTokenTag.Identifier)
    |> foldToolTip
    |> assertFoldedTooltipContains true "operator tooltip" expected

[<Fact>]
let ``Operators.TopLevel`` () =
    assertOperatorTooltipContains
        "tooltip for operator"
        "==="
        "/// tooltip for operator\nlet (===) a b = a + b\nlet _ = \"\" ==={caret} \"\""

[<Fact>]
let ``Operators.Member`` () =
    assertOperatorTooltipContains
        "tooltip for operator"
        "+++"
        "type U = U\n    with\n    /// tooltip for operator\n    static member (+++) (U, U) = U\nlet _ = U +++{caret} U"

[<Fact>]
let ``QuickInfoForQuotedIdentifiers`` () =
    let source =
        "/// The fff function\nlet fff x = x\n/// The gg gg function\nlet ``gg gg`` x = x\nlet r = fff 1 + ``gg gg`` 2  // no tip hovering over"

    let identifier = "``gg gg``"

    for i in 1 .. identifier.Length - 1 do
        let marker = "+ " + identifier.Substring(0, i)
        assertTooltipContains "gg gg" (markAtEndOfMarker source marker)

[<Fact>]
let ``QuickInfoSingleCharQuotedIdentifier`` () =
    assertTooltipContains "val x: int" "let ``x`` = 10\n``x{caret}``|> printfn \"%A\""

[<Fact>]
let ``IntArrayQuickInfo`` () =
    let source =
        "let x(*MIntArray1*) : int array = [| 1; 2; 3 |]\nlet y(*MInt[]*) : int []    = [| 1; 2; 3 |]"

    assertTooltipContains "int array" (markAtStartOfMarker source "(*MIntArray1*)")
    assertTooltipContains "int array" (markAtStartOfMarker source "(*MInt[]*)")

[<Fact>]
let ``LinkNameStringQuickInfo`` () =
    assertTooltipDoesNotContain "val" "let y = 1\nlet f x = \"{caret}x\"(*Marker1*)\nlet g z = \"y\"(*Marker2*)"
    assertTooltipDoesNotContain "val" "let y = 1\nlet f x = \"x\"(*Marker1*)\nlet g z = \"{caret}y\"(*Marker2*)"
    assertTooltipContains "val y: int" "let y{caret} = 1\nlet f x = \"x\"(*Marker1*)\nlet g z = \"y\"(*Marker2*)"

[<Fact>]
let ``IdentifierWithTick`` () =
    let source = "let x = 1\nlet x' = \"foo\"\nif (*aaa*)x = 1 then (*bbb*)x' else \"\""
    assertTooltipContains "val x: int" (markAtEndOfMarker source "(*aaa*)x")
    assertTooltipContains "val x': string" (markAtEndOfMarker source "(*bbb*)x'")

[<Fact>]
let ``NegativeTest.CharLiteralNotConfusedWithIdentifierWithTick`` () =
    assertTooltipDoesNotContain "val x" (markAtEndOfMarker "let x = 1\nlet y = 'x'" "'x")
    assertTooltipContains "val x: int" "let x{caret} = 1\nlet y = 'x'"

[<Fact>]
let ``StringLiteralWithIdentifierLookALikes.Bug2360_A`` () =
    let source = "let y = 1\nlet f x = \"x\"\nlet g z = \"y\""
    assertTooltipDoesNotContain "val" (markAtEndOfMarker source "f x = \"")
    assertTooltipContains "val y: int" (markAtEndOfMarker source "let y")

[<Fact>]
let ``Regression.StringLiteralWithIdentifierLookALikes.Bug2360_B`` () =
    assertTooltipContains "val y: int" (markAtEndOfMarker "let y = 1" "let y")

[<Fact>]
let ``Class.OnlyClassInfo`` () =
    let source = "type TT(x : int, ?y : int) =\n    class end"
    let marked = markAtEndOfMarker source "type T"
    assertTooltipContains "type TT" marked
    assertTooltipDoesNotContain "---" marked

[<Fact>]
let ``Regression.Classes.Bug2362`` () =
    let source = "let append mm nn = fun ac -> mm (nn ac)"
    assertTooltipContains "mm: ('a -> 'b) -> nn: ('c -> 'a) -> ac: 'c -> 'b" (markAtEndOfMarker source "let appen")
    assertTooltipContains "'a -> 'b" (markAtEndOfMarker source "let append m")
    assertTooltipContains "'c -> 'a" (markAtEndOfMarker source "let append mm n")

[<Fact>]
let ``Regression.NoTooltipForOperators.Bug4567`` () =
    assertOperatorTooltipContains
        "val (|+|) : a: int -> b: int -> int"
        "|+|"
        "let ( |+|{caret} ) a b = a + b\nlet n = 1 |+| 2\nlet b = true || false\n()"

    assertOperatorTooltipContains
        "val (|+|) : a: int -> b: int -> int"
        "|+|"
        "let ( |+| ) a b = a + b\nlet n = 1 |+|{caret} 2\nlet b = true || false\n()"

    assertOperatorTooltipContains
        "val (||) : e1: bool -> e2: bool -> bool"
        "||"
        "let ( |+| ) a b = a + b\nlet n = 1 |+| 2\nlet b = true ||{caret} false\n()"

[<Fact>]
let ``Regression.Bug1605`` () =
    assertTooltipContains
        "val string: value: 'T -> string"
        (markAtEndOfMarker "let rec f l =\n    match l with\n    | [] -> string.Format(\n    | x::xs -> \"hello\"" "| [] -> str")

[<Fact>]
let ``Regression.MemberDefinition.DocComments.Bug5856_6`` () =
    let source =
        "module Module =\n    /// A comment\n    exception MyException of int\nlet x() =\n    Module.MyExcep{caret}tion |> ignore"

    assertTooltipContainsInOrder [ "exception MyException of int"; "A comment" ] source

    match (Checker.getSymbolUse source).Symbol with
    | :? FSharpEntity as ent ->
        if ent.XmlDocSig <> "T:Test.Module.MyException" then
            failwithf "Unexpected XmlDocSig for own-code MyException: %s" ent.XmlDocSig

        if ent.Assembly.FileName |> Option.isSome then
            failwithf "Expected own-code MyException to have no backing assembly file, but got %A" ent.Assembly.FileName
    | sym -> failwithf "Expected an entity symbol for MyException, but got %A" sym

let private accessorsAndMutatorsSource =
    """type TestType1(*Marker1*)( x : int , y : int ) =
    let mutable x = x
    let mutable y = y
    member this.X with get () = x
                  and  set x' = x <- x'
    member this.Y with set y' = y <- y'
    member this.Length with get () = sqrt(float (x * x + y * y))
    member this.Item with get (i : int) = match i with | 0 -> x | 1 -> y | _ -> failwith "Incorrect index"
let point = TestType1(10,10)
point.X <- 3
point.Y <- 4
let xx = point.[0]
let yy = point.[1]
let bitArray = new System.Collections.BitArray(*Marker2*)(1)
point.Length |> ignore"""

[<Fact>]
let ``Automation.Regression.AccessorsAndMutators.Bug4276`` () =
    let m1 = markAtStartOfMarker accessorsAndMutatorsSource "(*Marker1*)"
    assertTooltipContains "type TestType1" m1
    assertTooltipContains "member Length: float" m1
    assertTooltipContains "member Item" m1
    assertTooltipContains "member X: int" m1
    assertTooltipContains "member Y: int" m1

    let m2 = markAtStartOfMarker accessorsAndMutatorsSource "(*Marker2*)"
    assertTooltipContains "type BitArray" m2
    assertTooltipContains "member And: value: BitArray -> BitArray" m2
    assertTooltipDoesNotContain "get_Length" m2
    assertTooltipDoesNotContain "set_Length" m2

let private tupleRecordClassOwnCodeConsumerSource =
    """module Test

open FSTestLib

let AbsTuple =
    fun x ->
        let tuple1 = (x, x.ToString(), (float) x, (fun y -> (y.ToString(), y + 1)))
        let tuple2 = (-x, (-x).ToString(), (float) (-x), (fun y -> (y.ToString(), y + 1)))
        if x >= 0 then tuple1(*Marker1*)
        else tuple2

let GenerateMyEmployee name age =
    let a = MyEmployee.MakeDummy()
    a.Name <- name
    a.Age <- age
    a.IsFTE <- System.Convert.ToBoolean(System.Random().Next(2))
    match a.IsFTE with
    | true -> a
    | _ -> MyEmployee(*Marker2*).MakeDummy()

let myCarQuot = <@ new MyCar(*Marker3*)(19, MyColors.Red) @>

let MaxTuple x y =
    let tuplex = (x, x.ToString())
    let tupley = (y, (y).ToString())
    match x > y with
    | true -> tuplex(*Marker4*)
    | false -> tupley"""

[<Fact>]
let ``Automation.TupleRecordClassfromOwnCode`` () =
    assertTooltipContainsWithFsTestLib
        "val tuple1: int * string * float * (int -> string * int)"
        (markAtStartOfMarker tupleRecordClassOwnCodeConsumerSource "(*Marker1*)")

    let m2 = markAtStartOfMarker tupleRecordClassOwnCodeConsumerSource "(*Marker2*)"
    assertTooltipContainsWithFsTestLib "type MyEmployee" m2
    assertTooltipContainsWithFsTestLib "Full name: FSTestLib.MyEmployee" m2

    let m3 = markAtStartOfMarker tupleRecordClassOwnCodeConsumerSource "(*Marker3*)"
    assertTooltipContainsWithFsTestLib "type MyCar" m3
    assertTooltipContainsWithFsTestLib "Full name: FSTestLib.MyCar" m3

    assertTooltipContainsWithFsTestLib
        "val tuplex: 'a * string"
        (markAtStartOfMarker tupleRecordClassOwnCodeConsumerSource "(*Marker4*)")

[<Fact>]
let ``Fsx.QuickInfo.Bug4979`` () =
    assertTooltipContains
        "<summary>The left or right SHIFT modifier key.</summary>"
        "System.ConsoleModifiers.Sh{caret}ift |> ignore\n(3).ToString().Length |> ignore"

    let tolerantAssemblies =
        set
            [ "netstandard.dll"
              "System.Runtime.dll"
              "System.Private.CoreLib.dll"
              "System.Console.dll"
              "mscorlib.dll" ]

    match (Checker.getSymbolUse "System.ConsoleModifiers.Shift |> ignore\n(3).ToString().Len{caret}gth |> ignore").Symbol with
    | :? FSharpMemberOrFunctionOrValue as m ->
        if m.XmlDocSig <> "P:System.String.Length" then
            failwithf "Unexpected XmlDocSig for String.Length: %s" m.XmlDocSig

        match m.Assembly.FileName |> Option.map System.IO.Path.GetFileName with
        | Some basename when tolerantAssemblies.Contains basename -> ()
        | other -> failwithf "Expected String.Length to be defined in one of %A, but got %A" tolerantAssemblies other
    | sym -> failwithf "Expected a member symbol for String.Length, but got %A" sym
