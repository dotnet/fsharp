module FSharp.Compiler.Service.Tests.SyntaxTreeTests.ValTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open NUnit.Framework

[<Test>]
let ``Inline keyword`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """namespace X

val inline meh: int -> int"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [
        SynModuleOrNamespaceSig(decls = [SynModuleSigDecl.Val(valSig = SynValSig(trivia = { InlineKeyword = Some mInline }))]) ])) ->
        assertRange (3, 4) (3,10) mInline
    | ast -> Assert.Fail $"Could not get valid AST, got {ast}"
