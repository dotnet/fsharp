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

let checkXml symbolName docs checkResults =
    let symbol = findSymbolByName symbolName checkResults
    let xmlDoc =
        match symbol with
        | :? FSharpMemberOrFunctionOrValue as v -> v.XmlDoc
        | :? FSharpEntity as t -> t.XmlDoc
        | :? FSharpUnionCase as u -> u.XmlDoc
        | _ -> failwith $"unexpected symbol type {symbol.GetType()}"
    match xmlDoc with
    | FSharpXmlDoc.FromXmlText t -> t.UnprocessedLines |> shouldEqual docs
    | _ -> failwith "wrong XmlDoc kind"

let checkXmls data checkResults =
    for symbolName, docs in data do checkXml symbolName docs checkResults

[<Test>]
let ``simple type xml doc``() =
    let _, checkResults = getParseAndCheckResults """
///A
type A = class end
"""
    checkResults
    |> checkXml "A" [|"A"|]

[<Test>]
let ``simple signature type xml doc``() =
    let _, checkResults = getParseAndCheckResultsOfSignatureFile """
///A1
///A2
type
    ///A3
    internal
             ///A4
             A
"""
    checkResults
    |> checkXml "A" [|"A1"; "A2"|]

[<Test>]
let ``xml doc before/after type name``() =
    let _, checkResults = getParseAndCheckResults """
///A1
type
    ///A2
    [<NotNull>]
    A = class end
"""
    checkResults
    |> checkXml "A" [|"A1"|]

[<Test>]
let ``multiline type xml doc``() =
    let _, checkResults = getParseAndCheckResults """
///A

///B
type A = class end
"""
    checkResults
    |> checkXml "A" [|"A"; "B"|]

[<Test>]
let ``separated type xml doc``() =
    let _, checkResults = getParseAndCheckResults """
///A
()
///B
type A = class end
"""
    checkResults
    |> checkXml "A" [|"B"|]

[<Test>]
let ``separated by simple comment type xml doc``() =
    let _, checkResults = getParseAndCheckResults """
///A
// Simple comment delimiter
///B
type A = class end
"""
    checkResults
    |> checkXml "A" [|"B"|]

[<Test>]
let ``separated by multiline comment type xml doc``() =
    let _, checkResults = getParseAndCheckResults """
///A
(* Multiline comment
delimiter *)
///B
type A = class end
"""
    checkResults
    |> checkXml "A" [|"B"|]

[<Test>]
let ``separated by star type xml doc``() =
    let _, checkResults = getParseAndCheckResults """
///A
(*)
///B
type A = class end
"""
    checkResults
    |> checkXml "A" [|"B"|]

[<Test>]
let ``let binding 01 - simple``() =
    let _, checkResults = getParseAndCheckResults """
///f1
let ///f2
    rec ///f3
        private f x = f x
"""
    checkResults
    |> checkXml "f" [|"f1"|]

[<Test>]
let ``let binding 02 - let in``() =
    let _, checkResults = getParseAndCheckResults """
///X
let x = 3 in print x
"""
    checkResults
    |> checkXml "x" [|"X"|]

[<Test>]
let ``let binding 03 - expression delimiter``() =
    let _, checkResults = getParseAndCheckResults """
///A
1 + 1
///B
let f x = ()
"""
    checkResults
    |> checkXml "f" [|"B"|]

[<Test>]
let ``let binding 04 - local binding``() =
    let _, checkResults = getParseAndCheckResults """
let _ =
    ///A
    1 + 1
    ///B
    let x = ()
    ()
"""
    checkResults
    |> checkXml "x" [|"B"|]

[<Test>]
let ``let binding 05 - with attribute``() =
    let _, checkResults = getParseAndCheckResults """
///X
[<Literal>]
let x = 5
"""
    checkResults
    |> checkXml "x" [|"X"|]

[<Test>]
let ``let binding 06 - xml doc after attribute``() =
    let _, checkResults = getParseAndCheckResults """
[<Literal>]
///X
let x = 5
"""
    checkResults
    |> checkXml "x" [||]

[<Test>]
let ``let binding 07 - attribute after 'let'``() =
    let _, checkResults = getParseAndCheckResults """
///X
let [<Literal>] x = 5
"""
    checkResults
    |> checkXml "x" [|"X"|]

[<Test>]
let ``let binding 08 - xml doc before 'and'``() =
    let _, checkResults = getParseAndCheckResults """
let rec f x = g x
///G
and g x = f x
"""
    checkResults
    |> checkXml "g" [|"G"|]

[<Test>]
let ``let binding 09 - xml doc after 'and'``() =
    let _, checkResults = getParseAndCheckResults """
let rec f x = g x
and ///G
    [<NotNull>] g x = f x
"""
    checkResults
    |> checkXml "g" [|"G"|]

[<Test>]
let ``let binding 010 - xml doc before and after 'and'``() =
    let _, checkResults = getParseAndCheckResults """
let rec f x = g x
///G1
and ///G2
    g x = f x
"""
    checkResults
    |> checkXml "g" [|"G1"|]

[<Test>]
let ``and type xml doc``() =
    let _, checkResults = getParseAndCheckResults """
type A = class end
///B
and B = class end
"""
    checkResults
    |> checkXml "B" [|"B"|]

[<Test>]
let ``type xml doc after and``() =
    let _, checkResults = getParseAndCheckResults """
type A = class end
and ///B
    B = class end
"""
    checkResults
    |> checkXml "B" [|"B"|]

[<Test>]
let ``type xml docs before/after and``() =
    let _, checkResults = getParseAndCheckResults """
type A = class end
///B1
and ///B2
    B = class end
"""
    checkResults
    |> checkXml "B" [|"B1"|]


[<Test>]
let ``union cases``() =
    let _, checkResults = getParseAndCheckResults """
type A =
    ///One
    One
    ///Two
    | Two
"""
    checkResults
    |> checkXmls [
        "One", [|"One"|]
        "Two", [|"Two"|]
    ]

[<Test>]
let ``type member multiline xml doc``() =
    let _, checkResults = getParseAndCheckResults """
type A =
    member x.A() = ///B1
        ()

    ///B2
    ///B3
    member x.B() = ()
"""
    checkResults
    |> checkXml "B" [|"B2"; "B3"|]

[<Test>]
let ``type member with attributes``() =
    let _, checkResults = getParseAndCheckResults """
type A =
    ///B1
    ///B2
    [<NotNull>]
    ///B3
    member x.B() = ()
"""
    checkResults
    |> checkXml "B" [|"B1"; "B2"|]

[<Test>]
let ``type member xml doc``() =
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
let Test141() =
    let _, checkResults = getParseAndCheckResults """
///A1
///A2
[<NotNull>]
///B
type A = class end
"""
    checkResults
    |> checkXml "A" [|"A1"; "A2"|]

[<Test>]
let Test142() =
    let _, checkResults = getParseAndCheckResults """
type A = class end
and
    ///B1
    [<NotNull>]
    ///B2
    B = class end
"""
    checkResults
    |> checkXml "B" [|"B1"|]

[<Test>]
let ``type specifications xml doc``() =
    let _, checkResults = getParseAndCheckResultsOfSignatureFile """
type A
and
    ///B1
    [<NotNull>]
    ///B2
    B
"""
    checkResults
    |> checkXml "B" [|"B1"|]

[<Test>]
let ``xml docs after attributes``() =
    let _, checkResults = getParseAndCheckResults """
[<NotNull>]
///A
type A = class end
"""
    checkResults
    |> checkXml "A" [||]

[<Test>]
let ``abstract member multiline xml doc``() =
    let _, checkResults = getParseAndCheckResults """
type A =
    ///M1
    ///M2
    abstract member M: unit
"""
    checkResults
    |> checkXml "get_M" [|"M1"; "M2"|]

[<Test>]
let ``abstract type member with attributes``() =
    let _, checkResults = getParseAndCheckResults """
type A =
    ///M1
    [<NotNull>]
    ///M2
    abstract member M: unit
"""
    checkResults
    |> checkXml "get_M" [|"M1"|]

[<Test>]
let ``signature type member with attributes``() =
    let _, checkResults = getParseAndCheckResultsOfSignatureFile """
type A =
    ///M1
    ///M2
    [<NotNull>]
    ///M3
    abstract member M: unit
"""
    checkResults
    |> checkXml "get_M" [|"M1"; "M2"|]

[<Test>]
let ``Property accessors xml doc``() =
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

[<Test; Ignore("TODO")>]
let ``record multiline xml doc``() =
    let _, checkResults = getParseAndCheckResults """
type A =
    {
        ///B1
        ///B2
        B: int
    }

{ B = 1 }.B
"""
    checkResults
    |> checkXml "B" [|"B1"; "B2"|]

[<Test>]
let ``attributes after type keyword``() =
    let _, checkResults = getParseAndCheckResultsOfSignatureFile """
///A1
type ///A2
     [<NotNull>] A = class end
"""
    checkResults
    |> checkXml "A" [|"A1"|]

[<Test>]
let ``module definition``() =
    let _, checkResults = getParseAndCheckResults """
///M1
module
       ///M2
       rec M =
    open M
"""
    checkResults
    |> checkXml "M" [|"M1"|]

[<Test>]
let ``module specification``() =
    let _, checkResults = getParseAndCheckResultsOfSignatureFile """
///M1
module
       ///M2
       rec M
"""
    checkResults
    |> checkXml "M" [|"M1"|]

[<Test>]
let ``extern``() =
    let _, checkResults = getParseAndCheckResults """
///E
[<DllImport("")>]
extern void E()
"""
    checkResults
    |> checkXml "E" [|"E"|]

