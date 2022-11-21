module FSharp.Compiler.Service.Tests.SyntaxTreeTests.ModuleOrNamespaceSigTests

open FSharp.Compiler.Service.Tests.Common
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open NUnit.Framework

[<Test>]
let ``Range member returns range of SynModuleOrNamespaceSig`` () =
    let parseResults =
        getParseResultsOfSignatureFile
            """
namespace Foobar

type Bar = | Bar of string * int
"""

    match parseResults with
    | ParsedInput.SigFile(ParsedSigFileInput(contents = [
        SynModuleOrNamespaceSig(kind = SynModuleOrNamespaceKind.DeclaredNamespace) as singleModule
    ])) ->
        assertRange (2,0) (4,32) singleModule.Range
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``GlobalNamespace should start at namespace keyword`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """// foo
// bar
namespace  global

type Bar = | Bar of string * int
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [
        SynModuleOrNamespaceSig(kind = SynModuleOrNamespaceKind.GlobalNamespace; range = r) ])) ->
        assertRange (3, 0) (5, 32) r
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Module range should start at first attribute`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
 [<  Foo  >]
module Bar

val s : string
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [
        SynModuleOrNamespaceSig.SynModuleOrNamespaceSig(kind = SynModuleOrNamespaceKind.NamedModule; range = r) ])) ->
        assertRange (2, 1) (5, 14) r
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Module should contain module keyword`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
module Bar

val a: int
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [
        SynModuleOrNamespaceSig(
            kind = SynModuleOrNamespaceKind.NamedModule
            trivia = { LeadingKeyword = SynModuleOrNamespaceLeadingKeyword.Module mModule }) ])) ->
        assertRange (2, 0) (2, 6) mModule
    | _ -> Assert.Fail "Could not get valid AST"

[<Test>]
let ``Namespace should contain namespace keyword`` () =
    let parseResults = 
        getParseResultsOfSignatureFile
            """
namespace Foo
module Bar =
val a: int
"""

    match parseResults with
    | ParsedInput.SigFile (ParsedSigFileInput (contents = [
        SynModuleOrNamespaceSig(
            kind = SynModuleOrNamespaceKind.DeclaredNamespace
            trivia = { LeadingKeyword = SynModuleOrNamespaceLeadingKeyword.Namespace mNamespace }) ])) ->
        assertRange (2, 0) (2, 9) mNamespace
    | _ -> Assert.Fail "Could not get valid AST"