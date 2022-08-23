module FSharp.Compiler.Service.Tests.SyntaxTreeTests.UnionCaseTestsTests

open System
open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework

[<Test>]
let ``Union Case fields can have comments`` () =
    let ast = """
type Foo =
/// docs for Thing
| Thing of
  /// docs for first
  first: string *
  /// docs for anon field
  bool
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types ([
                SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (simpleRepr = SynTypeDefnSimpleRepr.Union(unionCases = [
                    SynUnionCase.SynUnionCase (caseType = SynUnionCaseKind.Fields [
                        SynField.SynField(xmlDoc = firstXml)
                        SynField.SynField(xmlDoc = anonXml)
                    ])
                ])))
            ], _)
        ])
      ])) ->
        let firstDocs = firstXml.ToXmlDoc(false, None).GetXmlText()
        let anonDocs = anonXml.ToXmlDoc(false, None).GetXmlText()

        let nl = Environment.NewLine

        Assert.AreEqual($"<summary>{nl} docs for first{nl}</summary>", firstDocs)
        Assert.AreEqual($"<summary>{nl} docs for anon field{nl}</summary>", anonDocs)

    | _ ->
        failwith "Could not find SynExpr.Do"

[<Test>]
let ``single SynUnionCase has bar range`` () =
    let ast = """
type Foo = | Bar of string
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types ([
                SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (simpleRepr = SynTypeDefnSimpleRepr.Union(unionCases = [
                    SynUnionCase.SynUnionCase (trivia = { BarRange = Some mBar })
                ])))
            ], _)
        ])
      ])) ->
        assertRange (2, 11) (2, 12) mBar
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``multiple SynUnionCases have bar range`` () =
    let ast = """
type Foo =
    | Bar of string
    | Bear of int
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types ([
                SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (simpleRepr = SynTypeDefnSimpleRepr.Union(unionCases = [
                    SynUnionCase.SynUnionCase (trivia = { BarRange = Some mBar1 })
                    SynUnionCase.SynUnionCase (trivia = { BarRange = Some mBar2 })
                ])))
            ], _)
        ])
      ])) ->
        assertRange (3, 4) (3, 5) mBar1
        assertRange (4, 4) (4, 5) mBar2
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``single SynUnionCase without bar`` () =
    let ast = """
type Foo = Bar of string
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types ([
                SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (simpleRepr = SynTypeDefnSimpleRepr.Union(unionCases = [
                    SynUnionCase.SynUnionCase (trivia = { BarRange = None })
                ])))
            ], _)
        ])
      ])) ->
        Assert.Pass()
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``private keyword has range`` () =
    let ast = """
type Currency =
    // Temporary fix until a new Thoth.Json.Net package is released
    // See https://github.com/MangelMaxime/Thoth/pull/70

#if !FABLE_COMPILER
    private
#endif
    | Code of string
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types ([
                SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (simpleRepr = SynTypeDefnSimpleRepr.Union(
                    accessibility = Some (SynAccess.Private mPrivate)
                    unionCases = [ SynUnionCase.SynUnionCase _ ])))
            ], _)
        ])
      ])) ->
        assertRange (7, 4) (7, 11) mPrivate
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``SynUnionCaseKind.FullType`` () =
    let parseResults =
        getParseResults
             """
type X =
    | a: int * z:int
 """

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput(modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types(typeDefns = [
                SynTypeDefn(typeRepr = SynTypeDefnRepr.Simple(simpleRepr =
                    SynTypeDefnSimpleRepr.Union(unionCases = [
                        SynUnionCase(caseType = SynUnionCaseKind.FullType(fullType = SynType.Tuple(path = [
                            SynTupleTypeSegment.Type(SynType.LongIdent _)
                            SynTupleTypeSegment.Star _
                            SynTupleTypeSegment.Type(SynType.SignatureParameter(id = Some z))
                        ])))
                    ])))
            ])
        ])
    ])) ->
        Assert.AreEqual("z", z.idText)
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"