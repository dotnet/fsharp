#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module Tests.XmlDoc
#endif

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Symbols
open FsUnit
open NUnit.Framework

let getFieldXml symbolName fieldName checkResults =
    let field =
        match findSymbolByName symbolName checkResults with
        | :? FSharpEntity as e -> e.FSharpFields |> Seq.find(fun x -> x.DisplayName = fieldName)
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

let checkSignatureAndImplementation code checkAction =
    getParseAndCheckResults code |> snd
    |> checkAction

    getParseAndCheckResultsOfSignatureFile code |> snd
    |> checkAction

[<Test>]
let ``separated by expression``() =
    let _, checkResults = getParseAndCheckResults """
///A
()
///B
type A
"""
    checkResults
    |> checkXml "A" [|"B"|]

[<Test>]
let ``separated by // comment``() =
    checkSignatureAndImplementation """
///A
// Simple comment delimiter
///B
type A = class end
"""
        (checkXml "A" [|"B"|])

[<Test>]
let ``separated by //// comment``() =
    checkSignatureAndImplementation """
///A
//// Comment delimiter
///B
type A = class end
"""
        (checkXml "A" [|"B"|])

[<Test>]
let ``separated by multiline comment``() =
    checkSignatureAndImplementation """
///A
(* Multiline comment
delimiter *)
///B
type A = class end
"""
        (checkXml "A" [|"B"|])

[<Test>]
let ``separated by (*)``() =
    let _, checkResults = getParseAndCheckResults """
///A
(*)
///B
type A = class end
"""
    checkResults
    |> checkXml "A" [|"B"|]

[<Test>]
let ``types 01 - xml doc allowed positions``() =
    checkSignatureAndImplementation """
///A1
///A2
type
    ///A3
    internal
             ///A4
             A
"""
        (checkXml "A" [|"A1"; "A2"|])

[<Test>]
let ``types 02 - xml doc before 'and'``() =
    checkSignatureAndImplementation """
type A = class end
///B1
///B2
and B = class end
"""
        (checkXml "B" [|"B1"; "B2"|])

[<Test>]
let ``types 03 - xml doc after 'and'``() =
    checkSignatureAndImplementation """
type A = class end
and ///B1
    ///B2
    [<Attr>]
    ///B3
    B = class end
"""
        (checkXml "B" [|"B1"; "B2"|])

[<Test>]
let ``types 04 - xml doc before/after 'and'``() =
    checkSignatureAndImplementation """
type A = class end
///B1
and ///B2
    B = class end
"""
        (checkXml "B" [|"B1"|])

[<Test>]
let ``types 05 - attributes after 'type'``() =
    checkSignatureAndImplementation """
///A1
type ///A2
     [<Attr>] A = class end
"""
        (checkXml "A" [|"A1"|])

[<Test>]
let ``types 06 - xml doc after attribute``() =
    checkSignatureAndImplementation """
[<Attr>]
///A
type A = class end
"""
        (checkXml "A" [||])

[<Test>]
let ``let bindings 01 - allowed positions``() =
    let _, checkResults = getParseAndCheckResults """
///f1
let ///f2
    rec ///f3
        inline ///f4
               private f x = f x
"""
    checkResults
    |> checkXml "f" [|"f1"|]

[<Test>]
let ``let bindings 02``() =
    let _, checkResults = getParseAndCheckResults """
///X1
///X2
[<Attr>]
///X3
let x = 3
"""
    checkResults
    |> checkXml "x" [|"X1"; "X2"|]

[<Test>]
let ``let bindings 03 - 'let in'``() =
    let _, checkResults = getParseAndCheckResults """
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

[<Test>]
let ``let bindings 03 - 'let in' with attributes after 'let'``() =
    let _, checkResults = getParseAndCheckResults """
let ///X
    [<Attr>] x = 3 in print x
"""
    checkResults
    |> checkXml "x" [||]

[<Test>]
let ``let bindings 04 - local binding``() =
    let _, checkResults = getParseAndCheckResults """
let _ =
    ///X1
    ///X2
    let x = ()
    ()
"""
    checkResults
    |> checkXml "x" [|"X1"; "X2"|]

[<Test>]
let ``let bindings 05 - use``() =
    let _, checkResults = getParseAndCheckResults """
let _ =
    ///X1
    ///X2
    use x = ()
    ()
"""
    checkResults
    |> checkXml "x" [|"X1"; "X2"|]

[<Test>]
let ``let bindings 06 - xml doc after attribute``() =
    let _, checkResults = getParseAndCheckResults """
[<Literal>]
///X
let x = 5
"""
    checkResults
    |> checkXml "x" [||]

[<Test>]
let ``let bindings 07 - attribute after 'let'``() =
    let _, checkResults = getParseAndCheckResults """
///X1
let ///X2
    [<Literal>] x = 5
"""
    checkResults
    |> checkXml "x" [|"X1"|]

[<Test>]
let ``let bindings 08 - xml doc before 'and'``() =
    let _, checkResults = getParseAndCheckResults """
let rec f x = g x
///G1
///G2
and g x = f x
"""
    checkResults
    |> checkXml "g" [|"G1"; "G2"|]

[<Test>]
let ``let bindings 09 - xml doc after 'and'``() =
    let _, checkResults = getParseAndCheckResults """
let rec f x = g x
and ///G1
    ///G2
    [<NotNull>]
    ///G3
    g x = f x
"""
    checkResults
    |> checkXml "g" [|"G1"; "G2"|]

[<Test>]
let ``let bindings 10 - xml doc before/after 'and'``() =
    let _, checkResults = getParseAndCheckResults """
let rec f x = g x
///G1
and ///G2
    g x = f x
"""
    checkResults
    |> checkXml "g" [|"G1"|]

[<Test>]
let ``let bindings 11 - in type``() =
    let _, checkResults = getParseAndCheckResults """
type A() =
    ///data
    let data = 5
"""
    checkResults
    |> checkXml "data" [|"data"|]

[<Test>]
let ``type members 01 - allowed positions``() =
    let _, checkResults = getParseAndCheckResults """
type A =
    ///B1
    member
           ///B2
           private x.B() = ()
"""
    checkResults
    |> checkXml "B" [|"B1"|]

[<Test>]
let ``type members 02``() =
    let _, checkResults = getParseAndCheckResults """
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

[<Test>]
let ``type members 03 - abstract``() =
    checkSignatureAndImplementation """
type A =
    ///M1
    ///M2
    [<NotNull>]
    ///M3
    abstract member M: unit
"""
        (checkXml "get_M" [|"M1"; "M2"|])

[<Test>]
let ``type members 04 - property accessors``() =
    let _, checkResults = getParseAndCheckResults """
type B =
    ///A1
    ///A2
    member ///A3
           x.A
                ///GET
                with get () = 5
                ///SET
                and set (_: int) = ()

    member x.C = x.set_A(4)
"""
    checkResults
    |> checkXmls [
        "get_A", [|"A1"; "A2"|]
        "set_A", [|"A1"; "A2"|]
    ]

[<Test>]
let ``type members 05 - auto-property``() =
    let _, checkResults = getParseAndCheckResults """
type A() =
    ///B1
    ///B2
    [<NotNull>]
    ///B3
    member val B = 1 with get, set
"""
    checkResults
    |> checkXml "get_B" [|"B1"; "B2"|]

[<Test>]
let ``type members 06 - implicit ctor``() =
    let _, checkResults = getParseAndCheckResults """
type A ///CTOR1
       ///CTOR2
       [<Attr>]
       ///CTOR3
       () =
"""
    checkResults
    |> checkXmls [
        "A", [||]
        ".ctor", [|"CTOR1"; "CTOR2"|]
       ]

[<Test>]
let ``record``() =
    checkSignatureAndImplementation """
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

[<Test>]
let ``module 01``() =
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

[<Test>]
let ``module 02 - attributes after 'module'``() =
    checkSignatureAndImplementation """
///M1
module ///M2
       [<Attr>]
       rec M
"""
        (checkXml "M" [|"M1"|])

[<Test>]
let ``union cases 01 - without bar``() =
    checkSignatureAndImplementation """
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

[<Test>]
let ``union cases 02``() =
    checkSignatureAndImplementation """
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

[<Test>]
let ``extern``() =
    let _, checkResults = getParseAndCheckResults """
///E1
///E2
[<DllImport("")>]
///E3
extern void E()
"""
    checkResults
    |> checkXml "E" [|"E1"; "E2"|]

[<Test>]
let ``exception 01 - allowed positions``() =
    checkSignatureAndImplementation """
///E1
///E2
[<Attr>]
///E3
exception ///E4
          E of string
"""
        (checkXml "E" [|"E1"; "E2"|])

[<Test>]
let ``exception 02 - attribute after 'exception'``() =
    checkSignatureAndImplementation """
exception ///E
          [<Attr>]
          E of string
"""
        (checkXml "E" [||])

[<Test>]
let ``val 01 - type``() =
    checkSignatureAndImplementation """
type A =
    ///B1
    ///B2
    [<Attr>]
    ///B3
    val mutable private B: int
"""
     (getFieldXml "A" "B" >>
      compareXml [|"B1"; "B2"|])

[<Test>]
let ``val 02 - struct``() =
    let _, checkResults = getParseAndCheckResults """
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

[<Test>]
let ``val 03 - module``() =
    let _, checkResults = getParseAndCheckResultsOfSignatureFile """
///A1
///A2
[<Attr>]
///A3
val a: int
"""
    checkResults
    |> checkXml "a" [|"A1"; "A2"|]
