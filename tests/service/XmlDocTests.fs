#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
[<NUnit.Framework.TestFixture; NUnit.Framework.SetUICulture("en-US"); NUnit.Framework.SetCulture("en-US")>]
module Tests.Service.XmlDocTests.XmlDoc
#endif

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Test.Compiler
open FsUnit
open NUnit.Framework

let (|Types|TypeSigs|) = function
     | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types(range = range; typeDefns = types)])])) ->
        Types(range, types)

     | ParsedInput.SigFile(ParsedSigFileInput(contents = [
        SynModuleOrNamespaceSig.SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.Types(range = range; types = types)])])) ->
         TypeSigs(range, types)

     | x -> failwith $"Unexpected ParsedInput %A{x}"

let (|TypeRange|) = function
    | SynTypeDefn(range = typeRange; typeInfo = SynComponentInfo(range = componentInfoRange)) ->
        typeRange, componentInfoRange

let (|TypeSigRange|) = function
    | SynTypeDefnSig(range = typeRange; typeInfo = SynComponentInfo(range = componentInfoRange)) ->
        typeRange, componentInfoRange

let (|Module|NestedModules|NestedModulesSigs|) = function
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.NestedModule(range = range1)
            SynModuleDecl.NestedModule(range = range2)])])) ->
                NestedModules(range1, range2)
    | ParsedInput.SigFile(ParsedSigFileInput(contents = [
        SynModuleOrNamespaceSig.SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.NestedModule(range = range1)
            SynModuleSigDecl.NestedModule(range = range2)])])) ->
                NestedModulesSigs(range1, range2)

    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(range = range)]))
    | ParsedInput.SigFile(ParsedSigFileInput(contents = [
        SynModuleOrNamespaceSig.SynModuleOrNamespaceSig(range = range)])) ->
            Module(range)

    | x -> failwith $"Unexpected ParsedInput %A{x}"

let (|Exception|) = function
     | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Exception(range = range; exnDefn =
                    SynExceptionDefn(range = exnDefnRange; exnRepr =
                        SynExceptionDefnRepr(range = exnDefnReprRange)))])])) ->
         Exception(range, exnDefnRange, exnDefnReprRange)

     | ParsedInput.SigFile(ParsedSigFileInput(contents = [
            SynModuleOrNamespaceSig.SynModuleOrNamespaceSig(decls = [
                SynModuleSigDecl.Exception(range = range; exnSig =
                    SynExceptionSig(range = exnSpfnRange; exnRepr =
                        SynExceptionDefnRepr(range = exnDefnReprRange)))])])) ->
         Exception(range, exnSpfnRange, exnDefnReprRange)

     | x -> failwith $"Unexpected ParsedInput %A{x}"

let (|UnionCases|) = function
     | Types(_, [SynTypeDefn(typeRepr = SynTypeDefnRepr.Simple(simpleRepr = SynTypeDefnSimpleRepr.Union(_, cases, range)))])
     | TypeSigs(_, [SynTypeDefnSig(typeRepr = SynTypeDefnSigRepr.Simple(repr = SynTypeDefnSimpleRepr.Union(_, cases, range)))]) ->
         UnionCases(range, cases)

     | x -> failwith $"Unexpected ParsedInput %A{x}"

let (|Record|) = function
    | Types(_, [SynTypeDefn(typeRepr = SynTypeDefnRepr.Simple(simpleRepr = SynTypeDefnSimpleRepr.Record(recordFields = fields)))])
     | TypeSigs(_, [SynTypeDefnSig(typeRepr = SynTypeDefnSigRepr.Simple(repr = SynTypeDefnSimpleRepr.Record(recordFields = fields)))]) ->
         Record(fields)

     | x -> failwith $"Unexpected ParsedInput %A{x}"

let (|Members|MemberSigs|) = function
    | Types(_, [SynTypeDefn(typeRepr = SynTypeDefnRepr.ObjectModel(members = members))])
    | Types(_, [SynTypeDefn(members = members)]) ->
        Members(members)

    | TypeSigs(_, [SynTypeDefnSig(typeRepr = SynTypeDefnSigRepr.ObjectModel(memberSigs = members))]) ->
        MemberSigs(members)

    | x -> failwith $"Unexpected ParsedInput %A{x}"

let (|Decls|LetBindings|ValSig|LetOrUse|) = function
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Let(bindings = [SynBinding(expr = SynExpr.LetOrUse(range = range; bindings = bindings))])])])) ->
        LetOrUse(range, bindings)

    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Let(range = range; bindings = bindings)])])) ->
        LetBindings(range, bindings)

    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                SynModuleDecl.Expr(expr = SynExpr.LetOrUse(range = range; bindings = bindings))])])) ->
        LetBindings(range, bindings)

    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
            SynModuleOrNamespace.SynModuleOrNamespace(decls = decls)])) ->
        Decls(decls)

     | ParsedInput.SigFile(ParsedSigFileInput(contents = [
         SynModuleOrNamespaceSig.SynModuleOrNamespaceSig(decls = [
            SynModuleSigDecl.Val(valSig = SynValSig(range = valSigRange); range = range)])])) ->
         ValSig(range, valSigRange)

     | x -> failwith $"Unexpected ParsedInput %A{x}"

let findSymbolByName (name: string) (results: FSharpCheckFileResults) =
    let symbolByName (symbolUse:FSharpSymbolUse) =
        match getSymbolName symbolUse.Symbol with
        | Some symbolName -> symbolName = name
        | _ -> false
    let symbolUse = findSymbolUse symbolByName results
    symbolUse.Symbol

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

let checkXmls expected checkResults =
    for symbolName, docs in expected do
        checkXml symbolName docs checkResults

let findSymbolByNameAndType (target: SymbolType) (results: FSharpCheckFileResults) =
    let targetFullName = target.FullName()
    let symbolByNameAndType (symbolUse: FSharpSymbolUse) =
        match getSymbolFullName symbolUse.Symbol with
        | Some fullname -> targetFullName = fullname
        | None -> false
    let symbolUse = findSymbolUse symbolByNameAndType results
    symbolUse.Symbol

let checkXmlSymbol symbol docs checkResults =
    let symbol = findSymbolByNameAndType symbol checkResults
    let xmlDoc =
        match symbol with
        | :? FSharpMemberOrFunctionOrValue as v -> v.XmlDoc
        | :? FSharpEntity as t -> t.XmlDoc
        | :? FSharpUnionCase as u -> u.XmlDoc
        | :? FSharpField as f -> f.XmlDoc
        | _ -> failwith $"unexpected symbol type {symbol.GetType()}"
    compareXml docs xmlDoc

let checkXmlSymbols expected checkResults =
    for symbol, docs in expected do
        checkXmlSymbol symbol docs checkResults

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
let ``xml-doc eof``(): unit =
    checkSignatureAndImplementation """
module Test

/// a"""
        (fun _ -> ())
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [|(Information 3520, Line 4, Col 0, Line 4, Col 5, "XML comment is not placed on a valid language element.")|])

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
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [||]

            match parseResults.ParseTree with
            | Types(range, [TypeRange(typeDefnRange, synComponentRange)])
            | TypeSigs(range, [TypeSigRange(typeDefnRange, synComponentRange)]) ->
                assertRange (5, 0) (10, 18) range
                assertRange (5, 0) (10, 18) typeDefnRange
                assertRange (10, 5) (10, 6) synComponentRange
            | x ->
                failwith $"Unexpected ParsedInput %A{x}")

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
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [|Information 3520, Line 4, Col 0, Line 4, Col 4, "XML comment is not placed on a valid language element."|]

            match parseResults.ParseTree with
            | Types(range, [TypeRange(typeRange, synComponentRange)])
            | TypeSigs(range, [TypeSigRange(typeRange, synComponentRange)]) ->
                assertRange (6, 0) (7, 18) range
                assertRange (6, 0) (7, 18) typeRange
                assertRange (7, 5) (7, 6) synComponentRange
            | x ->
                failwith $"Unexpected ParsedInput %A{x}")

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
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [|Information 3520, Line 4, Col 0, Line 4, Col 4, "XML comment is not placed on a valid language element."|]

            match parseResults.ParseTree with
            | Types(range, [TypeRange(typeRange, synComponentRange)])
            | TypeSigs(range, [TypeSigRange(typeRange, synComponentRange)]) ->
                assertRange (6, 0) (7, 18) range
                assertRange (6, 0) (7, 18) typeRange
                assertRange (7, 5) (7, 6) synComponentRange
            | x ->
                failwith $"Unexpected ParsedInput %A{x}")

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
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [|Information 3520, Line 4, Col 0, Line 4, Col 4, "XML comment is not placed on a valid language element."|]

            match parseResults.ParseTree with
            | Types(range, [TypeRange(typeRange, synComponentRange)])
            | TypeSigs(range, [TypeSigRange(typeRange, synComponentRange)]) ->
                assertRange (7, 0) (8, 18) range
                assertRange (7, 0) (8, 18) typeRange
                assertRange (8, 5) (8, 6) synComponentRange
            | x ->
                failwith $"Unexpected ParsedInput %A{x}")

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
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [|
                Information 3520, Line 7, Col 4, Line 7, Col 9, "XML comment is not placed on a valid language element."
                Information 3520, Line 9, Col 13, Line 9, Col 18, "XML comment is not placed on a valid language element."
            |]

            match parseResults.ParseTree with
            | Types(range, [TypeRange(typeRange, synComponentRange)])
            | TypeSigs(range, [TypeSigRange(typeRange, synComponentRange)]) ->
                assertRange (4, 0) (10, 14) range
                assertRange (4, 0) (10, 14) typeRange
                assertRange (10, 13) (10, 14) synComponentRange
            | x ->
                failwith $"Unexpected ParsedInput %A{x}")

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
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [||]

            match parseResults.ParseTree with
            | Types(range, [TypeRange(typeRange1, synComponentRange1)
                            TypeRange(typeRange2, synComponentRange2)])
            | TypeSigs(range, [TypeSigRange(typeRange1, synComponentRange1)
                               TypeSigRange(typeRange2, synComponentRange2)]) ->
                assertRange (4, 0) (7, 17) range
                assertRange (4, 5) (4, 18) typeRange1
                assertRange (4, 5) (4, 6) synComponentRange1
                assertRange (5, 0) (7, 17) typeRange2
                assertRange (7, 4) (7, 5) synComponentRange2
            | x ->
                failwith $"Unexpected ParsedInput %A{x}")

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
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [|Information 3520, Line 8, Col 4, Line 8, Col 9, "XML comment is not placed on a valid language element."|]

            match parseResults.ParseTree with
            | Types(range, [_; TypeRange(typeRange2, synComponentRange2)])
            | TypeSigs(range, [_; TypeSigRange(typeRange2, synComponentRange2)]) ->
                assertRange (4, 0) (9, 17) range
                assertRange (5, 4) (9, 17) typeRange2
                assertRange (9, 4) (9, 5) synComponentRange2
            | x ->
                failwith $"Unexpected ParsedInput %A{x}")

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
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [|Information 3520, Line 6, Col 4, Line 6, Col 9, "XML comment is not placed on a valid language element."|]

            match parseResults.ParseTree with
            | Types(range, [_; TypeRange(typeRange2, synComponentRange2)])
            | TypeSigs(range, [_; TypeSigRange(typeRange2, synComponentRange2)]) ->
                assertRange (4, 0) (7, 17) range
                assertRange (5, 0) (7, 17) typeRange2
                assertRange (7, 4) (7, 5) synComponentRange2
            | x ->
                failwith $"Unexpected ParsedInput %A{x}")

[<Test>]
let ``types 05 - attributes after 'type'``(): unit =
    checkSignatureAndImplementation """
module Test

///A1
type ///A2
     [<Attr>] A = class end
"""
        (checkXml "A" [|"A1"|])
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [|Information 3520, Line 5, Col 5, Line 5, Col 10, "XML comment is not placed on a valid language element."|]

            match parseResults.ParseTree with
            | Types(range, [TypeRange(typeRange, synComponentRange)])
            | TypeSigs(range, [TypeSigRange(typeRange, synComponentRange)]) ->
                assertRange (4, 0) (6, 27) range
                assertRange (4, 0) (6, 27) typeRange
                assertRange (6, 14) (6, 15) synComponentRange
            | x ->
                failwith $"Unexpected ParsedInput %A{x}")

[<Test>]
let ``types 06 - xml doc after attribute``(): unit =
    checkSignatureAndImplementation """
module Test

[<Attr>]
///A
type A = class end
"""
        (checkXml "A" [||])
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [|Information 3520, Line 5, Col 0, Line 5, Col 4, "XML comment is not placed on a valid language element."|]

            match parseResults.ParseTree with
            | Types(range, [TypeRange(typeRange, synComponentRange)])
            | TypeSigs(range, [TypeSigRange(typeRange, synComponentRange)]) ->
                assertRange (4, 0) (6, 18) range
                assertRange (4, 0) (6, 18) typeRange
                assertRange (6, 5) (6, 6) synComponentRange
            | x ->
                failwith $"Unexpected ParsedInput %A{x}")

[<Test>]
let ``types 07``(): unit =
    let parseResults, checkResults = getParseAndCheckResultsOfSignatureFile """
module Test

type A

///B
and B = int -> int
"""
    checkResults
    |> checkXmls ["B", [|"B"|]]

    parseResults
    |> checkParsingErrors [||]

    match parseResults.ParseTree with
    | TypeSigs(_, [SynTypeDefnSig(range = range1); SynTypeDefnSig(range = range2)]) ->
        assertRange (4, 5) (4, 6) range1
        assertRange (6, 0) (7, 18) range2
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

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

    match parseResults.ParseTree with
    | LetBindings(range, [SynBinding(range = bindingRange)]) ->
        assertRange (2, 0) (6, 32) range
        assertRange (2, 0) (6, 26) bindingRange
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

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

    match parseResults.ParseTree with
    | LetBindings(letBindingsRange, [binding]) ->
        assertRange (2, 0) (6, 9) letBindingsRange
        assertRange (2, 0) (6, 9) binding.RangeOfBindingWithRhs
    | _ ->
        failwith "Unexpected ParsedInput"

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

    match parseResults.ParseTree with
    | Decls([SynModuleDecl.Let(range = range1; bindings = [binding1])
             SynModuleDecl.Let(range = range2; bindings = [binding2])]) ->
        assertRange (2, 0) (6, 9) range1
        assertRange (2, 0) (6, 9) binding1.RangeOfBindingWithRhs
        assertRange (8, 0) (12, 9) range2
        assertRange (8, 0) (12, 9) binding2.RangeOfBindingWithRhs
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

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

    match parseResults.ParseTree with
    | LetBindings(range, [binding]) ->
        assertRange (2, 0) (3, 29) range
        assertRange (3, 4) (3, 18) binding.RangeOfBindingWithRhs
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

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

    match parseResults.ParseTree with
    | LetOrUse(range, [binding]) ->
        assertRange (3, 4) (6, 6) range
        assertRange (3, 4) (5, 14) binding.RangeOfBindingWithRhs
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

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

    match parseResults.ParseTree with
    | LetOrUse(range, [binding]) ->
        assertRange (3, 4) (6, 6) range
        assertRange (3, 4) (5, 14) binding.RangeOfBindingWithRhs
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

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

    match parseResults.ParseTree with
    | LetBindings(range, [binding]) ->
        assertRange (2, 0) (4, 9) range
        assertRange (2, 0) (4, 9) binding.RangeOfBindingWithRhs
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

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

    match parseResults.ParseTree with
    | LetBindings(range, [binding]) ->
        assertRange (2, 0) (4, 21) range
        assertRange (2, 0) (4, 21) binding.RangeOfBindingWithRhs
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

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

    match parseResults.ParseTree with
    | LetBindings(range, [binding1; binding2]) ->
        assertRange (2, 0) (5, 13) range
        assertRange (2, 8) (2, 17) binding1.RangeOfBindingWithRhs
        assertRange (3, 0) (5, 13) binding2.RangeOfBindingWithRhs
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

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

    match parseResults.ParseTree with
    | LetBindings(range, [binding1; binding2]) ->
        assertRange (2, 0) (7, 13) range
        assertRange (2, 8) (2, 17) binding1.RangeOfBindingWithRhs
        assertRange (3, 4) (7, 13) binding2.RangeOfBindingWithRhs
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

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

    match parseResults.ParseTree with
    | LetBindings(range, [binding1; binding2]) ->
        assertRange (2, 0) (5, 13) range
        assertRange (2, 8) (2, 17) binding1.RangeOfBindingWithRhs
        assertRange (3, 0) (5, 13) binding2.RangeOfBindingWithRhs
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

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

    match parseResults.ParseTree with
    | Members([_; SynMemberDefn.LetBindings(bindings = [SynBinding(range = range)])]) ->
        assertRange (3, 4) (4, 12) range
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

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

    match parseResults.ParseTree with
    | Members([SynMemberDefn.Member(range = range; memberDefn = SynBinding _)]) ->
        assertRange (3, 4) (6, 35) range
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

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

    match parseResults.ParseTree with
    | Members([SynMemberDefn.Member(range = range1)
               SynMemberDefn.Member(range = range2; memberDefn = SynBinding (range = bindingRange2))]) ->
        assertRange (3, 4) (4, 10) range1
        assertRange (6, 4) (10, 21) range2
        assertRange (6, 4) (10, 16) bindingRange2
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

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
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [|Information 3520, Line 8, Col 4, Line 8, Col 9, "XML comment is not placed on a valid language element."|]

            match parseResults.ParseTree with
            | Members([SynMemberDefn.AbstractSlot(range = range; slotSig = SynValSig(range = slotRange))])
            | MemberSigs([SynMemberSig.Member(range = range; memberSig = SynValSig(range = slotRange))]) ->
                assertRange (5, 4) (9, 27) range
                assertRange (5, 4) (9, 27) slotRange
            | x -> failwith $"Unexpected ParsedInput: %A{x}")

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

    match parseResults.ParseTree with
    | Members([ SynMemberDefn.GetSetMember(Some (SynBinding(xmlDoc = xmlDoc) as binding), _, range, _); _ ]) ->
        assertRange (3, 4) (10, 37) range
        assertRange (3, 4) (8, 37) binding.RangeOfBindingWithRhs
        assertRange (3, 4) (4, 9) xmlDoc.Range
        assertRange (3, 4) (4, 9) (xmlDoc.ToXmlDoc(false, None).Range)
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

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

    match parseResults.ParseTree with
    | Members([_; SynMemberDefn.AutoProperty(range = range)]) ->
        assertRange (3, 4) (7, 20) range
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

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

    match parseResults.ParseTree with
    | Members([SynMemberDefn.ImplicitCtor(range = range)]) ->
        assertRange (2, 5) (2, 6) range
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

[<Test>]
let ``type members 07 - explicit ctor signature``(): unit =
    let parseResults, checkResults = getParseAndCheckResultsOfSignatureFile """
module Test

type A =
    ///ctor
    new: unit -> A
"""
    checkResults
    |> checkXmls [
        "A", [||]
        ".ctor", [|"ctor"|]
       ]

    parseResults
    |> checkParsingErrors [||]

    match parseResults.ParseTree with
    | MemberSigs([SynMemberSig.Member(range = range)]) ->
        assertRange (5, 4) (6, 18) range
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

[<Test>]
let ``type members 08 - explicit ctor definition``(): unit =
    let parseResults, checkResults = getParseAndCheckResults """
type A =
    ///ctor
    new() = ()
"""
    checkResults
    |> checkXmls [
        "A", [||]
        ".ctor", [|"ctor"|]
       ]

    parseResults
    |> checkParsingErrors [||]

    match parseResults.ParseTree with
    | Members([SynMemberDefn.Member(range = range)]) ->
        assertRange (3, 4) (4, 14) range
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"


[<Test>]
let record(): unit =
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

        (fun parseResults ->
            parseResults |>
            checkParsingErrors [|Information 3520, Line 9, Col 8, Line 9, Col 13, "XML comment is not placed on a valid language element."|]

            match parseResults.ParseTree with
            | Record([SynField(range = range)]) -> assertRange (6, 8) (10, 14) range
            | x -> failwith $"Unexpected ParsedInput: %A{x}")


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
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [|
                Information 3520, Line 5, Col 0, Line 5, Col 5, "XML comment is not placed on a valid language element."
                Information 3520, Line 7, Col 7, Line 7, Col 12, "XML comment is not placed on a valid language element."
            |]

            match parseResults.ParseTree with
            | Module(range) -> assertRange (2, 0) (8, 12) range
            | x -> failwith $"Unexpected ParsedInput: %A{x}")

[<Test>]
let ``module 02 - attributes after 'module'``(): unit =
    checkSignatureAndImplementation """
///M1
module ///M2
       [<Attr>]
       rec M
"""
        (checkXml "M" [|"M1"|])
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [|
                Information 3520, Line 3, Col 7, Line 3, Col 12, "XML comment is not placed on a valid language element."
            |]

            match parseResults.ParseTree with
            | Module(range) -> assertRange (2, 0) (5, 12) range
            | x -> failwith $"Unexpected ParsedInput: %A{x}")

[<Test>]
let ``module 03 - signature - multiple``(): unit =
    let parseResults, checkResults = getParseAndCheckResultsOfSignatureFile """
///M1
[<Attr>]
module M1 =
    type A with
        member B: unit -> unit

///M2
[<Attr>]
module M2 = type A
"""

    checkResults |>
    checkXmls [|
        "M1", [|"M1"|]
        "M2", [|"M2"|]
    |]

    match parseResults.ParseTree with
    | NestedModulesSigs(range1, range2) ->
        assertRange (2, 0) (6, 30) range1
        assertRange (8, 0) (10, 18) range2
    | x ->
        failwith $"Unexpected ParsedInput: %A{x}"

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
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [||]

            match parseResults.ParseTree with
            | UnionCases(range,
                         [SynUnionCase(range = unionCaseRange1);
                          SynUnionCase(range = unionCaseRange2)]) ->
                assertRange (5, 4) (10, 9) range
                assertRange (5, 4) (7, 7) unionCaseRange1
                assertRange (8, 4) (10, 9) unionCaseRange2
            | x ->
                failwith $"Unexpected ParsedInput %A{x}")

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
       (fun parseResults ->
            parseResults |>
            checkParsingErrors [|
                Information 3520, Line 8, Col 6, Line 8, Col 13, "XML comment is not placed on a valid language element."
                Information 3520, Line 13, Col 6, Line 13, Col 13, "XML comment is not placed on a valid language element."
            |]

            match parseResults.ParseTree with
            | UnionCases(range,
                         [SynUnionCase(range = unionCaseRange1);
                          SynUnionCase(range = unionCaseRange2)]) ->
                assertRange (5, 4) (14, 9) range
                assertRange (5, 4) (9, 9) unionCaseRange1
                assertRange (10, 4) (14, 9) unionCaseRange2
            | x ->
                failwith $"Unexpected ParsedInput %A{x}")

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

        (fun parseResults ->
            parseResults |>
            checkParsingErrors [||]

            match parseResults.ParseTree with
            | UnionCases(_, [SynUnionCase(caseType = SynUnionCaseKind.Fields(cases = [
                SynField (range = fieldRange1)
                SynField(range = fieldRange2)]))]) ->
                    assertRange (6, 2) (8, 11) fieldRange1
                    assertRange (9, 2) (11, 6) fieldRange2
            | x -> failwith $"Unexpected ParsedInput %A{x}")

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

    match parseResults.ParseTree with
    | LetBindings(range, [SynBinding(range = bindingRange)]) ->
        assertRange (2, 0) (6, 15) range
        assertRange (2, 0) (6, 15) bindingRange
    | _ ->
        failwith "Unexpected ParsedInput"

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
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [|
                Information 3520, Line 7, Col 0, Line 7, Col 5, "XML comment is not placed on a valid language element."
                Information 3520, Line 8, Col 10, Line 8, Col 15, "XML comment is not placed on a valid language element."
            |]

            match parseResults.ParseTree with
            | Exception(exnRange, exnDefnRange, exnDefnReprRange) ->
                assertRange (4, 0) (9, 21) exnRange
                assertRange (4, 0) (9, 21) exnDefnRange
                assertRange (4, 0) (9, 21) exnDefnReprRange)

[<Test>]
let ``exception 02 - attribute after 'exception'``(): unit =
    checkSignatureAndImplementation """
module Test

exception ///E
          [<Attr>]
          E of string
"""
        (checkXml "E" [||])
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [|Information 3520, Line 4, Col 10, Line 4, Col 14, "XML comment is not placed on a valid language element."|]

            match parseResults.ParseTree with
            | Exception(exnRange, exnDefnRange, exnDefnReprRange) ->
                assertRange (4, 0) (6, 21) exnRange
                assertRange (4, 0) (6, 21) exnDefnRange
                assertRange (4, 0) (6, 21) exnDefnReprRange)

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
     (fun parseResults ->
        parseResults |>
        checkParsingErrors [|Information 3520, Line 8, Col 4, Line 9, Col 9, "XML comment is not placed on a valid language element."|]

        match parseResults.ParseTree with
        | Members([SynMemberDefn.ValField(range = range)])
        | MemberSigs([SynMemberSig.ValField(range = range)]) ->
            assertRange (5, 4) (10, 30) range
        | x ->
            failwith $"Unexpected ParsedInput %A{x}")

[<Test>]
let ``val 02 - type - static``(): unit =
    checkSignatureAndImplementation """
module Test

type A =
    ///B1
    [<Attr>]
    static val B: int
"""
     (getFieldXml "A" "B" >>
      compareXml [|"B1"|])

     (fun parseResults ->
        parseResults |> checkParsingErrors [||]

        match parseResults.ParseTree with
        | Members([SynMemberDefn.ValField(range = range)])
        | MemberSigs([SynMemberSig.ValField(range = range)]) ->
            assertRange (5, 4) (7, 21) range
        | x ->
            failwith $"Unexpected ParsedInput %A{x}")

[<Test>]
let ``val 03 - struct``(): unit =
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

    match parseResults.ParseTree with
    | Members([SynMemberDefn.ValField(fieldInfo = SynField(range = fieldRange); range = range)]) ->
        assertRange (4, 8) (8, 20) range
        assertRange (8, 12) (8, 20) fieldRange
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

[<Test>]
let ``val 04 - module``(): unit =
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

    match parseResults.ParseTree with
    | ValSig(valRange, valSigRange) ->
        assertRange (4, 0) (8, 10) valRange
        assertRange (4, 0) (8, 10) valSigRange
    | x ->
        failwith $"Unexpected ParsedInput %A{x}"

[<Test>]
let ``namespace 01``(): unit =
    checkSignatureAndImplementation """
///N
namespace N
"""
        (checkXml "N" [||])
        (fun parseResults ->
            parseResults |>
            checkParsingErrors [|Information 3520, Line 2, Col 0, Line 2, Col 4, "XML comment is not placed on a valid language element."|]

            match parseResults.ParseTree with
            | ParsedInput.ImplFile(ParsedImplFileInput(contents = [SynModuleOrNamespace.SynModuleOrNamespace(range = range)]))
            | ParsedInput.SigFile(ParsedSigFileInput(contents = [SynModuleOrNamespaceSig.SynModuleOrNamespaceSig(range = range)])) ->
                assertRange (3, 0) (3, 11) range
            | x ->
                failwith $"Unexpected ParsedInput %A{x}")

[<Test>]
let ``Verify that OCaml style xml-doc are gone (i.e. treated as regular comments)``(): unit =
    let parseResults, checkResults = getParseAndCheckResultsOfSignatureFile """
module Test
(** I'm an ocaml style xml comment! --- treated merely as a comment by F#*)
type e =
  | A
  | B
"""
    parseResults |> checkParsingErrors [||]
    checkResults |> checkXml "e" [||]

[<Test>]
let ``Verify that //// yields an informational error``(): unit =
    let parseResults, checkResults = getParseAndCheckResultsOfSignatureFile """
module Test
//// I am NOT an xml comment
type e =
  | A
  | B
"""
    parseResults |> checkParsingErrors [||]
    checkResults |> checkXml "e" [||]


// https://github.com/dotnet/fsharp/blob/6c6588730c4d650a354e5ea3d46fb4630d7bba01/tests/fsharpqa/Source/XmlDoc/Basic/xmlDoc004.fs
[<Test>]
let ``Verify that XmlDoc items are correctly generated for various syntax items``(): unit =
    let parseResults, checkResults = getParseAndCheckResultsOfSignatureFile """
// Verify that XmlDoc value is correctly generated

namespace My.Rather.Deep.Namespace

///shape
type Shape =
    | Rectangle of width : float * length : float
    ///circle
    | Circle of radius : float
    | Prism of width : float * float * height : float

///freeRecord
type Enum =
    ///xxx
    | XXX = 3
    ///zzz
    | ZZZ = 11

///testModule
module Module =
    ///testRecord
    type TestRecord = {
        ///Record string
        MyProperty : string
    }

    ///point3D
    type Point3D =
       struct
          ///point.x
          val x: float
          ///point.y
          val y: float
          val z: float
       end

    ///test enum
    type TestEnum =
        ///enumValue1
        | VALUE1 = 1
        ///enumValue2
        | VALUE2 = 2

    ///test union
    type TestUnion =
        ///union - enum
        | TestEnum
        ///union - record
        | TestRecord

    ///nested module
    module NestedModule =
        ///testRecord nested
        type TestRecord = {
            ///Record string nested
            MyProperty : string
        }

        ///test enum nested
        type TestEnum =
            ///enumValue1 nested
            | VALUE1 = 1
            ///enumValue2 nested
            | VALUE2 = 2

        ///test union nested
        type TestUnion =
            ///union - enum nested
            | TestEnum
            ///union - record nested
            | TestRecord
"""
    parseResults |> checkParsingErrors [||]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Enum", [|"freeRecord"|] ]
    checkResults |> checkXmlSymbols [ Field  "My.Rather.Deep.Namespace.Enum.ZZZ", [|"zzz"|] ]
    checkResults |> checkXmlSymbols [ Field  "My.Rather.Deep.Namespace.Enum.XXX", [|"xxx"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Enum", [|"freeRecord"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Shape.Circle", [|"circle"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Shape", [|"shape"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Module", [|"testModule"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Module.TestEnum", [|"test enum"|] ]
    checkResults |> checkXmlSymbols [ Field "My.Rather.Deep.Namespace.Module.TestEnum.VALUE1", [|"enumValue1"|] ]
    checkResults |> checkXmlSymbols [ Field "My.Rather.Deep.Namespace.Module.TestEnum.VALUE2", [|"enumValue2"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Module.TestRecord", [|"testRecord"|] ]
    checkResults |> checkXmlSymbols [ Parameter "My.Rather.Deep.Namespace.Module.TestRecord.MyProperty", [|"Record string"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Module.TestUnion", [|"test union"|] ]
    checkResults |> checkXmlSymbols [ Field "My.Rather.Deep.Namespace.Module.TestUnion.TestEnum", [|"union - enum"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Module.Point3D", [|"point3D"|] ]
    checkResults |> checkXmlSymbols [ Field "My.Rather.Deep.Namespace.Module.Point3D.x", [|"point.x"|] ]
    checkResults |> checkXmlSymbols [ Field "My.Rather.Deep.Namespace.Module.Point3D.y", [|"point.y"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Module.TestUnion.TestRecord", [|"union - record"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Module.TestUnion.TestEnum", [|"union - enum"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Module.TestUnion", [|"test union"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Module.NestedModule.TestUnion.TestRecord", [|"union - record nested"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Module.NestedModule.TestUnion.TestEnum", [|"union - enum nested"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Module.NestedModule.TestUnion", [|"test union nested"|] ]
    checkResults |> checkXmlSymbols [ Field "My.Rather.Deep.Namespace.Module.NestedModule.TestEnum.VALUE2", [|"enumValue2 nested"|] ]
    checkResults |> checkXmlSymbols [ Field "My.Rather.Deep.Namespace.Module.NestedModule.TestEnum.VALUE1", [|"enumValue1 nested"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Module.NestedModule.TestEnum", [|"test enum nested"|] ]
    checkResults |> checkXmlSymbols [ Parameter "My.Rather.Deep.Namespace.Module.NestedModule.TestRecord.MyProperty", [|"Record string nested"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Module.NestedModule.TestRecord", [|"testRecord nested"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Namespace.Module.NestedModule", [|"nested module"|] ]


[<Test>]
let ``Verify that leading space is retained in XmlDoc items for various syntax items``(): unit =
    let parseResults, checkResults = getParseAndCheckResultsOfSignatureFile """
// Verify that leading space is retained in XmlDoc items for various syntax items

namespace My.Leading.Space.TestCase

/// Test Enum
type Enum =
    | XXX = 3

/// Test Module
module Module =
    /// Test Struct
    type TestStruct =
       struct
          val x: float
       end

    /// Test Nested Module
    module NestedModule =
        /// Test Union
        type TestUnion =
            | TestEnum
            | TestRecord
"""
    parseResults |> checkParsingErrors [||]
    checkResults |> checkXmlSymbols [ Entity "My.Leading.Space.TestCase.Enum", [|" Test Enum"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Leading.Space.TestCase.Module", [|" Test Module"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Leading.Space.TestCase.Module.TestStruct", [|" Test Struct"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Leading.Space.TestCase.Module.NestedModule", [|" Test Nested Module"|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Leading.Space.TestCase.Module.NestedModule.TestUnion", [|" Test Union"|] ]

[<Test; Ignore("https://github.com/dotnet/fsharp/issues/12517")>]
let ``Verify that XmlDoc items are correctly generated for Generic classes``(): unit =
    let parseResults, checkResults = getParseAndCheckResultsOfSignatureFile """
// Verify that XmlDoc items are correctly generated for Generic classes

namespace My.Rather.Deep.Generic.Namespace

    ///testClass
    type GenericClass<'a> (x: 'a) = start end

    ///nested module
    module NestedModule =
        ///testClass nested
        type GenericClass<'a> (x: 'a) = start end
"""
    parseResults |> checkParsingErrors [||]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Generic.Namespace.GenericClass`1", [|"testClass"|] ]
    //checkResults |> checkXmlSymbols [ MemberOrFunctionOrValue "My.Rather.Deep.Generic.Namespace.GenericClass`1.TestMethod", [|"testClass member"|] ]
    //checkResults |> checkXmlSymbols [ Parameter "My.Rather.Deep.Generic.Namespace.GenericClass`1.MyReadWriteProperty", [|"A read-write property."|] ]
    checkResults |> checkXmlSymbols [ Entity "My.Rather.Deep.Generic.Namespace.Module.NestedModule.GenericClass`1", [|"testClass nested"|] ]
    //checkResults |> checkXmlSymbols [ MemberOrFunctionOrValue "My.Rather.Deep.Generic.Namespace.Module.GenericClass`1.TestMethod", [|"testClass member nested"|] ]
    //checkResults |> checkXmlSymbols [ Parameter "My.Rather.Deep.Generic.Namespace.Module.GenericClass`1.ReadWriteProperty", [|"A read-write property."|] ]



//https://github.com/dotnet/fsharp/blob/6c6588730c4d650a354e5ea3d46fb4630d7bba01/tests/fsharpqa/Source/XmlDoc/Basic/xmlDoc005.fs
[<Test; Ignore("https://github.com/dotnet/fsharp/issues/12517")>]
let ``Verify that XmlDoc names are generated, but no empty members are generated re: issue #148``(): unit =
    let parseResults, checkResults = getParseAndCheckResultsOfSignatureFile """
// Verify that XmlDoc names are generated, but no empty members are generated re: issue #148

namespace MyRather.MyDeep.MyNamespace
open System.Xml

/// class1
type Class1() =
    /// x
    member this.X = "X"

type Class2() =
    member this.Y = "Y"
"""
    parseResults |> checkParsingErrors [||]
    checkResults |> checkXmlSymbols [ Parameter "MyRather.MyDeep.MyNamespace.Class1.X", [|"x"|] ]
    checkResults |> checkXmlSymbols [ Parameter "MyRather.MyDeep.MyNamespace.Class1", [|"class1"|] ]
