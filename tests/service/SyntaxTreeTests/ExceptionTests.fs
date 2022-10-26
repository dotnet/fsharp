module FSharp.Compiler.Service.Tests.SyntaxTreeTests.ExceptionTestsTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open NUnit.Framework

[<Test>]
let ``SynExceptionDefn should contains the range of the with keyword`` () =
    let parseResults = 
        getParseResults
            """
namespace X

exception Foo with
    member Meh () = ()
"""

    match parseResults with
    | ParsedInput.ImplFile (ParsedImplFileInput (contents = [ SynModuleOrNamespace(decls = [
        SynModuleDecl.Exception(
            exnDefn=SynExceptionDefn(withKeyword = Some mWithKeyword)
        )
    ]) ])) ->
        assertRange (4, 14) (4, 18) mWithKeyword
    | _ -> Assert.Fail "Could not get valid AST"