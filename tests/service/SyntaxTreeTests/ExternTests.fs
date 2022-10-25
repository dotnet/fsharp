module FSharp.Compiler.Service.Tests.SyntaxTreeTests.ExternTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open NUnit.Framework

[<Test>]
let ``extern keyword is present in trivia`` () =
    let parseResults = getParseResults "extern void GetProcessHeap()"

    match parseResults with
    | ParsedInput.ImplFile(ParsedImplFileInput(contents = [
                SynModuleOrNamespace.SynModuleOrNamespace(decls = [
                    SynModuleDecl.Let(bindings = [
                        SynBinding(trivia = { ExternKeyword = Some mExtern  })
                    ])
                ])
            ])) ->
        assertRange (1, 0) (1, 6) mExtern
    | _ -> Assert.Fail $"Could not get valid AST, got {parseResults}"
