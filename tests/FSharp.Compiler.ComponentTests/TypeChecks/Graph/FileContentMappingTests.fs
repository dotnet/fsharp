module FSharp.Compiler.ComponentTests.TypeChecks.Graph.FileContentMappingTests

open NUnit.Framework
open FSharp.Compiler.GraphChecking
open TestUtils

let private getContent isSignature sourceCode =
    let fileName = if isSignature then "Test.fsi" else "Test.fs"
    let ast = parseSourceCode ("Test.fs", sourceCode)
    // We use index 1 in this test because the index 0 will always return an empty list
    FileContentMapping.mkFileContent { Idx = 1; FileName = fileName; ParsedInput = ast }

let private (|TopLevelNamespace|_|) value e =
    match e with
    | FileContentEntry.TopLevelNamespace(path, content) ->
        let combined = String.concat "." path
        if combined = value then Some content else None
    | _ -> None

let private (|OpenStatement|_|) value e =
    match e with
    | FileContentEntry.OpenStatement path ->
        let combined = String.concat "." path
        if combined = value then Some() else None
    | _ -> None

let private (|PrefixedIdentifier|_|) value e =
    match e with
    | FileContentEntry.PrefixedIdentifier path ->
        let combined = String.concat "." path
        if combined = value then Some() else None
    | _ -> None

let private (|NestedModule|_|) value e =
    match e with
    | FileContentEntry.NestedModule(name, nestedContent) -> if name = value then Some(nestedContent) else None
    | _ -> None

[<Test>]
let ``Top level module only exposes namespace`` () =
    let content =
        getContent
            false
            """
module X.Y.Z
"""

    match content with
    | [ TopLevelNamespace "X.Y" [] ] -> Assert.Pass()
    | content -> Assert.Fail($"Unexpected content: {content}")

[<Test>]
let ``Top level namespace`` () =
    let content =
        getContent
            false
            """
namespace X.Y
"""

    match content with
    | [ TopLevelNamespace "X.Y" [] ] -> Assert.Pass()
    | content -> Assert.Fail($"Unexpected content: {content}")

[<Test>]
let ``Open statement in top level module`` () =
    let content =
        getContent
            true
            """
module X.Y.Z

open A.B.C
"""

    match content with
    | [ TopLevelNamespace "X.Y" [ OpenStatement "A.B.C" ] ] -> Assert.Pass()
    | content -> Assert.Fail($"Unexpected content: {content}")

[<Test>]
let ``PrefixedIdentifier in type annotation`` () =
    let content =
        getContent
            false
            """
module X.Y.Z

let fn (a: A.B.CType) = ()
"""

    match content with
    | [ TopLevelNamespace "X.Y" [ PrefixedIdentifier "A.B" ] ] -> Assert.Pass()
    | content -> Assert.Fail($"Unexpected content: {content}")

[<Test>]
let ``Nested module`` () =
    let content =
        getContent
            true
            """
module X.Y

module Z =
    type A = int
"""

    match content with
    | [ TopLevelNamespace "X" [ NestedModule "Z" [] ] ] -> Assert.Pass()
    | content -> Assert.Fail($"Unexpected content: {content}")

[<Test>]
let ``Single ident module abbreviation`` () =
    let content =
        getContent
            true
            """
module A

module B = C
"""

    match content with
    | [ TopLevelNamespace "" [ PrefixedIdentifier "C" ] ] -> Assert.Pass()
    | content -> Assert.Fail($"Unexpected content: {content}")

[<Test>]
let ``First file index always returns an empty list`` () =
    let content = FileContentMapping.mkFileContent { Idx = 0; FileName = "FirstFile.fs"; ParsedInput = Unchecked.defaultof<_> }
    Assert.IsTrue content.IsEmpty
