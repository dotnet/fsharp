#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module Tests.XmlDoc
#endif

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Symbols
open FSharp.Test.Compiler
open FsUnit
open NUnit.Framework
open FSharp.Compiler.Diagnostics

let getFieldXml symbolName fieldName checkResults =
    let field =
        match findSymbolByName symbolName checkResults with
        | :? FSharpEntity as e -> e.FSharpFields |> Seq.find(fun x -> x.DisplayName = fieldName)
        | :? FSharpUnionCase as c -> c.Fields |> Seq.find(fun x -> x.DisplayName = fieldName)
        | _ -> failwith "Unexpected symbol"
    field.XmlDoc

let compareXml docs xmlDoc  =
    match xmlDoc with
    | FSharpXmlDoc.FromXmlText t -> t.UnprocessedLines |> shouldEqual docs
    | _ -> failwith "wrong XmlDoc kind"

let checkXml symbolName docs checkResults =
    let symbol = findSymbolByName symbolName checkResults
    let xmlDoc =
        match symbol with
        | :? FSharpMemberOrFunctionOrValue as v -> v.XmlDoc
        | :? FSharpEntity as t -> t.XmlDoc
        | :? FSharpUnionCase as u -> u.XmlDoc
        | _ -> failwith $"unexpected symbol type {symbol.GetType()}"
    compareXml docs xmlDoc

let checkXmls data checkResults =
    for symbolName, docs in data do checkXml symbolName docs checkResults

let checkSignatureAndImplementation code checkResultsAction parseResultsAction =
    let checkCode getResultsFunc =
        let parseResults, checkResults = getResultsFunc code
        checkResultsAction checkResults
        parseResultsAction parseResults

    checkCode getParseAndCheckResults
    checkCode getParseAndCheckResultsOfSignatureFile

let checkParsingErrors expected (parseResults: FSharpParseFileResults) =
    parseResults.Diagnostics |> Array.map (fun x ->
        let range = x.Range
        let error = mapDiagnosticSeverity x.Severity x.ErrorNumber
        error, Line range.StartLine, Col range.StartColumn, Line range.EndLine, Col range.EndColumn, x.Message)
    |> shouldEqual expected

[<Test>]
let ``comments after xml-doc``(): unit =
    checkSignatureAndImplementation """
module Test

// b
///A
// b
//// b
(*
 b *)
type A = class end
"""
        (checkXml "A" [|"A"|])
        (checkParsingErrors [||])

[<Test>]
let ``separated by expression``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
///A
()
///B
type A
"""
    checkResults
    |> checkXml "A" [|"B"|]

    parseResults
    |> checkParsingErrors [|Information 3520, Line 2, Col 0, Line 2, Col 4, "XML comment is not placed on a valid language element."|]

[<Test>]
let ``separated by // comment``(): unit =
    checkSignatureAndImplementation """
module Test

///A
// Simple comment delimiter
///B
type A = class end
"""
        (checkXml "A" [|"B"|])
        (checkParsingErrors [|Information 3520, Line 4, Col 0, Line 4, Col 4, "XML comment is not placed on a valid language element."|])

[<Test>]
let ``separated by //// comment``(): unit =
    checkSignatureAndImplementation """
module Test

///A
//// Comment delimiter
///B
type A = class end
"""
        (checkXml "A" [|"B"|])
        (checkParsingErrors [|Information 3520, Line 4, Col 0, Line 4, Col 4, "XML comment is not placed on a valid language element."|])

[<Test>]
let ``separated by multiline comment``(): unit =
    checkSignatureAndImplementation """
module Test

///A
(* Multiline comment
delimiter *)
///B
type A = class end
"""
        (checkXml "A" [|"B"|])
        (checkParsingErrors [|Information 3520, Line 4, Col 0, Line 4, Col 4, "XML comment is not placed on a valid language element."|])

[<Test>]
let ``separated by (*)``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
///A
(*)
///B
type A = class end
"""
    checkResults
    |> checkXml "A" [|"B"|]

    parseResults
    |> checkParsingErrors [|Information 3520, Line 2, Col 0, Line 2, Col 4, "XML comment is not placed on a valid language element."|]

[<Test>]
let ``types 01 - xml doc allowed positions``(): unit =
    checkSignatureAndImplementation """
module Test

///A1
///A2
type
    ///A3
    internal
             ///A4
             A
"""
        (checkXml "A" [|"A1"; "A2"|])
        (checkParsingErrors [|
            Information 3520, Line 7, Col 4, Line 7, Col 9, "XML comment is not placed on a valid language element."
            Information 3520, Line 9, Col 13, Line 9, Col 18, "XML comment is not placed on a valid language element."
         |])

[<Test>]
let ``types 02 - xml doc before 'and'``(): unit =
    checkSignatureAndImplementation """
module Test

type A = class end
///B1
///B2
and B = class end
"""
        (checkXml "B" [|"B1"; "B2"|])
        (checkParsingErrors [||])

[<Test>]
let ``types 03 - xml doc after 'and'``(): unit =
    checkSignatureAndImplementation """
module Test

type A = class end
and ///B1
    ///B2
    [<Attr>]
    ///B3
    B = class end
"""
        (checkXml "B" [|"B1"; "B2"|])
        (checkParsingErrors [|Information 3520, Line 8, Col 4, Line 8, Col 9, "XML comment is not placed on a valid language element."|])

[<Test>]
let ``types 04 - xml doc before/after 'and'``(): unit =
    checkSignatureAndImplementation """
module Test

type A = class end
///B1
and ///B2
    B = class end
"""
        (checkXml "B" [|"B1"|])
        (checkParsingErrors [|Information 3520, Line 6, Col 4, Line 6, Col 9, "XML comment is not placed on a valid language element."|])

[<Test>]
let ``types 05 - attributes after 'type'``(): unit =
    checkSignatureAndImplementation """
module Test

///A1
type ///A2
     [<Attr>] A = class end
"""
        (checkXml "A" [|"A1"|])
        (checkParsingErrors [|Information 3520, Line 5, Col 5, Line 5, Col 10, "XML comment is not placed on a valid language element."|])

[<Test>]
let ``types 06 - xml doc after attribute``(): unit =
    checkSignatureAndImplementation """
module Test

[<Attr>]
///A
type A = class end
"""
        (checkXml "A" [||])
        (checkParsingErrors [|Information 3520, Line 5, Col 0, Line 5, Col 4, "XML comment is not placed on a valid language element."|])

[<Test>]
let ``let bindings 01 - allowed positions``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
///f1
let ///f2
    rec ///f3
        inline ///f4
               private f x = f x
"""
    checkResults
    |> checkXml "f" [|"f1"|]

    parseResults
    |> checkParsingErrors [|
        Information 3520, Line 3, Col 4, Line 3, Col 9, "XML comment is not placed on a valid language element."
        Information 3520, Line 4, Col 8, Line 4, Col 13, "XML comment is not placed on a valid language element."
        Information 3520, Line 5, Col 15, Line 5, Col 20, "XML comment is not placed on a valid language element."
    |]

[<Test>]
let ``let bindings 02``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
///X1
///X2
[<Attr>]
///X3
let x = 3
"""
    checkResults
    |> checkXml "x" [|"X1"; "X2"|]

    parseResults
    |> checkParsingErrors [|Information 3520, Line 5, Col 0, Line 5, Col 5, "XML comment is not placed on a valid language element."|]

[<Test>]
let ``let bindings 03 - 'let in'``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
///X1
///X2
[<Attr>]
///X3
let x = 3 in

///Y1
///Y2
[<Attr>]
///Y3
let y = x
"""
    checkResults
    |> checkXmls [
        "x", [|"X1"; "X2"|]
        "y", [|"Y1"; "Y2"|]
       ]

    parseResults
    |> checkParsingErrors [|
        Information 3520, Line 5, Col 0, Line 5, Col 5, "XML comment is not placed on a valid language element."
        Information 3520, Line 11, Col 0, Line 11, Col 5, "XML comment is not placed on a valid language element."
    |]

[<Test>]
let ``let bindings 03 - 'let in' with attributes after 'let'``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
let ///X
    [<Attr>] x = 3 in print x
"""
    checkResults
    |> checkXml "x" [||]

    parseResults
    |> checkParsingErrors [|Information 3520, Line 2, Col 4, Line 2, Col 8, "XML comment is not placed on a valid language element."|]

[<Test>]
let ``let bindings 04 - local binding``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
let _ =
    ///X1
    ///X2
    let x = ()
    ()
"""
    checkResults
    |> checkXml "x" [|"X1"; "X2"|]

    parseResults
    |> checkParsingErrors [||]

[<Test>]
let ``let bindings 05 - use``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
let _ =
    ///X1
    ///X2
    use x = ()
    ()
"""
    checkResults
    |> checkXml "x" [|"X1"; "X2"|]

    parseResults
    |> checkParsingErrors [||]

[<Test>]
let ``let bindings 06 - xml doc after attribute``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
[<Literal>]
///X
let x = 5
"""
    checkResults
    |> checkXml "x" [||]

    parseResults
    |> checkParsingErrors [|Information 3520, Line 3, Col 0, Line 3, Col 4, "XML comment is not placed on a valid language element."|]

[<Test>]
let ``let bindings 07 - attribute after 'let'``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
///X1
let ///X2
    [<Literal>] x = 5
"""
    checkResults
    |> checkXml "x" [|"X1"|]

    parseResults
    |> checkParsingErrors [|Information 3520, Line 3, Col 4, Line 3, Col 9, "XML comment is not placed on a valid language element."|]

[<Test>]
let ``let bindings 08 - xml doc before 'and'``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
let rec f x = g x
///G1
///G2
and g x = f x
"""
    checkResults
    |> checkXml "g" [|"G1"; "G2"|]

    parseResults
    |> checkParsingErrors [||]

[<Test>]
let ``let bindings 09 - xml doc after 'and'``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
let rec f x = g x
and ///G1
    ///G2
    [<NotNull>]
    ///G3
    g x = f x
"""
    checkResults
    |> checkXml "g" [|"G1"; "G2"|]

    parseResults
    |> checkParsingErrors [|Information 3520, Line 6, Col 4, Line 6, Col 9, "XML comment is not placed on a valid language element."|]

[<Test>]
let ``let bindings 10 - xml doc before/after 'and'``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
let rec f x = g x
///G1
and ///G2
    g x = f x
"""
    checkResults
    |> checkXml "g" [|"G1"|]

    parseResults
    |> checkParsingErrors [|Information 3520, Line 4, Col 4, Line 4, Col 9, "XML comment is not placed on a valid language element."|]

[<Test>]
let ``let bindings 11 - in type``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
type A() =
    ///data
    let data = 5
"""
    checkResults
    |> checkXml "data" [|"data"|]

    parseResults
    |> checkParsingErrors [||]

[<Test>]
let ``type members 01 - allowed positions``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
type A =
    ///B1
    member
           ///B2
           private x.B(): unit = ()
"""
    checkResults
    |> checkXml "B" [|"B1"|]

    parseResults
    |> checkParsingErrors [|Information 3520, Line 5, Col 11, Line 5, Col 16, "XML comment is not placed on a valid language element."|]

[<Test>]
let ``type members 02``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
type A =
    member x.A() = ///B1
        ()

    ///B2
    ///B3
    [<Attr>]
    ///B4
    member x.B() = ()
"""
    checkResults
    |> checkXml "B" [|"B2"; "B3"|]

    parseResults
    |> checkParsingErrors [|
        Information 3520, Line 3, Col 19, Line 3, Col 24, "XML comment is not placed on a valid language element."
        Information 3520, Line 9, Col 4, Line 9, Col 9, "XML comment is not placed on a valid language element."
    |]

[<Test>]
let ``type members 03 - abstract``(): unit =
    checkSignatureAndImplementation """
module Test

type A =
    ///M1
    ///M2
    [<NotNull>]
    ///M3
    abstract member M: unit
"""
        (checkXml "get_M" [|"M1"; "M2"|])
        (checkParsingErrors [|Information 3520, Line 8, Col 4, Line 8, Col 9, "XML comment is not placed on a valid language element."|])

[<Test>]
let ``type members 04 - property accessors``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
type B =
    ///A1
    ///A2
    member ///A3
           x.A
                ///GET
                with get (): unit = 5
                ///SET
                and set (_: int) = ()

    member x.C = x.set_A(4)
"""
    checkResults
    |> checkXmls [
        "get_A", [|"A1"; "A2"|]
        "set_A", [|"A1"; "A2"|]
    ]

    parseResults
    |> checkParsingErrors [|
        Information 3520, Line 5, Col 11, Line 5, Col 16, "XML comment is not placed on a valid language element."
        Information 3520, Line 7, Col 16, Line 7, Col 22, "XML comment is not placed on a valid language element."
        Information 3520, Line 9, Col 16, Line 9, Col 22, "XML comment is not placed on a valid language element."
    |]

[<Test>]
let ``type members 05 - auto-property``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
type A() =
    ///B1
    ///B2
    [<NotNull>]
    ///B3
    member val B = 1 with get, set
"""
    checkResults
    |> checkXml "get_B" [|"B1"; "B2"|]

    parseResults
    |> checkParsingErrors [|Information 3520, Line 6, Col 4, Line 6, Col 9, "XML comment is not placed on a valid language element."|]

[<Test>]
let ``type members 06 - implicit ctor``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
type A ///CTOR1
       ///CTOR2
       [<Attr>]
       ///CTOR3
       () = class end
"""
    checkResults
    |> checkXmls [
        "A", [||]
        ".ctor", [|"CTOR1"; "CTOR2"|]
       ]

    parseResults
    |> checkParsingErrors [|Information 3520, Line 5, Col 7, Line 5, Col 15, "XML comment is not placed on a valid language element."|]

[<Test>]
let ``record``(): unit =
    checkSignatureAndImplementation """
module Test

type A =
    {
        ///B1
        ///B2
        [<Attr>]
        ///B3
        B: int
    }
"""
        (getFieldXml "A" "B" >>
         compareXml [|"B1"; "B2"|])
        (checkParsingErrors [|Information 3520, Line 9, Col 8, Line 9, Col 13, "XML comment is not placed on a valid language element."|])

[<Test>]
let ``module 01``(): unit =
    checkSignatureAndImplementation """
///M1
///M2
[<Attr>]
///M3
module
       ///M4
       rec M
"""
        (checkXml "M" [|"M1"; "M2"|])
        (checkParsingErrors [|
            Information 3520, Line 5, Col 0, Line 5, Col 5, "XML comment is not placed on a valid language element."
            Information 3520, Line 7, Col 7, Line 7, Col 12, "XML comment is not placed on a valid language element."
         |])

[<Test>]
let ``module 02 - attributes after 'module'``(): unit =
    checkSignatureAndImplementation """
///M1
module ///M2
       [<Attr>]
       rec M
"""
        (checkXml "M" [|"M1"|])
        (checkParsingErrors [|
            Information 3520, Line 3, Col 7, Line 3, Col 12, "XML comment is not placed on a valid language element."
         |])

[<Test>]
let ``union cases 01 - without bar``(): unit =
    checkSignatureAndImplementation """
module Test

type A =
    ///One1
    ///One2
    One
    ///Two1
    ///Two2
    | Two
"""
        (checkXmls [
            "One", [|"One1"; "One2"|]
            "Two", [|"Two1"; "Two2"|]
        ])
        (checkParsingErrors [||])

[<Test>]
let ``union cases 02``(): unit =
    checkSignatureAndImplementation """
module Test

type A =
    ///One1
    ///One2
    | [<Attr>]
      ///One3
      One
    ///Two1
    ///Two2
    | [<Attr>]
      ///Two3
      Two
"""
       (checkXmls [
           "One", [|"One1"; "One2"|]
           "Two", [|"Two1"; "Two2"|]
       ])
       (checkParsingErrors [|
            Information 3520, Line 8, Col 6, Line 8, Col 13, "XML comment is not placed on a valid language element."
            Information 3520, Line 13, Col 6, Line 13, Col 13, "XML comment is not placed on a valid language element."
        |])

[<Test>]
let ``union cases 03 - union case fields``(): unit =
    checkSignatureAndImplementation """
module Test

type Foo =
| Thing of
  ///A1
  ///A2
  a: string *
  ///B1
  ///B2
  bool
"""
        (fun checkResults ->
            getFieldXml "Thing" "a" checkResults
            |> compareXml [|"A1"; "A2"|]

            getFieldXml "Thing" "Item2" checkResults
            |> compareXml [|"B1"; "B2"|])

        (checkParsingErrors [||])

[<Test>]
let ``extern``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
///E1
///E2
[<DllImport("")>]
///E3
extern void E()
"""
    checkResults
    |> checkXml "E" [|"E1"; "E2"|]

    parseResults
    |> checkParsingErrors [|Information 3520, Line 5, Col 0, Line 5, Col 5, "XML comment is not placed on a valid language element."|]

[<Test>]
let ``exception 01 - allowed positions``(): unit =
    checkSignatureAndImplementation """
module Test

///E1
///E2
[<Attr>]
///E3
exception ///E4
          E of string
"""
        (checkXml "E" [|"E1"; "E2"|])
        (checkParsingErrors [|
            Information 3520, Line 7, Col 0, Line 7, Col 5, "XML comment is not placed on a valid language element."
            Information 3520, Line 8, Col 10, Line 8, Col 15, "XML comment is not placed on a valid language element."
         |])

[<Test>]
let ``exception 02 - attribute after 'exception'``(): unit =
    checkSignatureAndImplementation """
module Test

exception ///E
          [<Attr>]
          E of string
"""
        (checkXml "E" [||])
        (checkParsingErrors [|Information 3520, Line 4, Col 10, Line 4, Col 14, "XML comment is not placed on a valid language element."|])

[<Test>]
let ``val 01 - type``(): unit =
    checkSignatureAndImplementation """
module Test

type A =
    ///B1
    ///B2
    [<Attr>]
    ///B3
    ///B4
    val mutable private B: int
"""
     (getFieldXml "A" "B" >>
      compareXml [|"B1"; "B2"|])
     (checkParsingErrors [|Information 3520, Line 8, Col 4, Line 9, Col 9, "XML comment is not placed on a valid language element."|])

[<Test>]
let ``val 02 - struct``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
type Point =
    struct
        ///X1
        ///X2
        [<Attr>]
        ///X3
        val x: float
    end
"""
    checkResults
    |> getFieldXml "Point" "x"
    |> compareXml [|"X1"; "X2"|]

    parseResults
    |> checkParsingErrors [|Information 3520, Line 7, Col 8, Line 7, Col 13, "XML comment is not placed on a valid language element."|]

[<Test>]
let ``val 03 - module``(): unit =
    let parseResults, checkResults = getParseAndCheckResultsOfSignatureFile """
module Test

///A1
///A2
[<Attr>]
///A3
val a: int
"""
    checkResults
    |> checkXml "a" [|"A1"; "A2"|]

    parseResults
    |> checkParsingErrors [|Information 3520, Line 7, Col 0, Line 7, Col 5, "XML comment is not placed on a valid language element."|]
