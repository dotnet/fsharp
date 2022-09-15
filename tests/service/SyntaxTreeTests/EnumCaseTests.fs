module FSharp.Compiler.Service.Tests.SyntaxTreeTests.EnumCaseTestsTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework

[<Test>]
let ``single SynEnumCase has bar range`` () =
    let ast = """
type Foo = | Bar = 1
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types ([
                SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (simpleRepr = SynTypeDefnSimpleRepr.Enum(cases = [
                    SynEnumCase.SynEnumCase (trivia = { BarRange = Some mBar; EqualsRange = mEquals })
                ])))
            ], _)
        ])
      ])) ->
        assertRange (2, 11) (2, 12) mBar
        assertRange (2, 17) (2, 18) mEquals
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``multiple SynEnumCases have bar range`` () =
    let ast = """
type Foo =
    | Bar = 1
    | Bear = 2
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types ([
                SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (simpleRepr = SynTypeDefnSimpleRepr.Enum(cases = [
                    SynEnumCase.SynEnumCase (trivia = { BarRange = Some mBar1; EqualsRange = mEquals1 })
                    SynEnumCase.SynEnumCase (trivia = { BarRange = Some mBar2; EqualsRange = mEquals2 })
                ])))
            ], _)
        ])
      ])) ->
        assertRange (3, 4) (3, 5) mBar1
        assertRange (3, 10) (3, 11) mEquals1
        assertRange (4, 4) (4, 5) mBar2
        assertRange (4, 11) (4, 12) mEquals2
    | _ ->
        Assert.Fail "Could not get valid AST"

[<Test>]
let ``single SynEnumCase without bar`` () =
    let ast = """
type Foo = Bar = 1
"""
                    |> getParseResults

    match ast with
    | ParsedInput.ImplFile(ParsedImplFileInput(modules = [
        SynModuleOrNamespace.SynModuleOrNamespace(decls = [
            SynModuleDecl.Types ([
                SynTypeDefn.SynTypeDefn (typeRepr = SynTypeDefnRepr.Simple (simpleRepr = SynTypeDefnSimpleRepr.Enum(cases = [
                    SynEnumCase.SynEnumCase (trivia = { BarRange = None; EqualsRange = mEquals })
                ])))
            ], _)
        ])
      ])) ->
        assertRange (2, 15) (2, 16) mEquals
    | _ ->
        Assert.Fail "Could not get valid AST"