module TypeChecks.FileContentMappingTests

open Xunit
open FSharp.Compiler.GraphChecking
open TestUtils

let private getContent isSignature sourceCode =
    let fileName = if isSignature then "Test.fsi" else "Test.fs"
    let ast = parseSourceCode ("Test.fs", sourceCode)
    FileContentMapping.mkFileContent { Idx = 0; FileName = fileName; ParsedInput = ast }

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

[<Fact>]
let ``Top level module only exposes namespace`` () =
    let content =
        getContent
            false
            """
module X.Y.Z
"""

    match content with
    | [ TopLevelNamespace "X.Y" [] ] -> ()
    | content -> Assert.Fail($"Unexpected content: {content}")

[<Fact>]
let ``Top level namespace`` () =
    let content =
        getContent
            false
            """
namespace X.Y
"""

    match content with
    | [ TopLevelNamespace "X.Y" [] ] -> ()
    | content -> Assert.Fail($"Unexpected content: {content}")

[<Fact>]
let ``Open statement in top level module`` () =
    let content =
        getContent
            true
            """
module X.Y.Z

open A.B.C
"""

    match content with
    | [ TopLevelNamespace "X.Y" [ OpenStatement "A.B.C" ] ] -> ()
    | content -> Assert.Fail($"Unexpected content: {content}")

[<Fact>]
let ``PrefixedIdentifier in type annotation`` () =
    let content =
        getContent
            false
            """
module X.Y.Z

let fn (a: A.B.CType) = ()
"""

    match content with
    | [ TopLevelNamespace "X.Y" [ PrefixedIdentifier "A.B" ] ] -> ()
    | content -> Assert.Fail($"Unexpected content: {content}")

[<Fact>]
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
    | [ TopLevelNamespace "X" [ NestedModule "Z" [] ] ] -> ()
    | content -> Assert.Fail($"Unexpected content: {content}")

[<Fact>]
let ``Single ident module abbreviation`` () =
    let content =
        getContent
            true
            """
module A

module B = C
"""

    match content with
    | [ TopLevelNamespace "" [ PrefixedIdentifier "C" ] ] -> ()
    | content -> Assert.Fail($"Unexpected content: {content}")


module InvalidSyntax =

    [<Fact>]
    let ``Nested module`` () =
        let content =
            getContent
                false
                """
    module A

    module B.C
    """

        match content with
        | [ TopLevelNamespace "" [] ] -> ()
        | content -> Assert.Fail($"Unexpected content: {content}")


    [<Fact>]
    let ``Module above namespace`` () =
        let content =
            getContent
                false
                """
    module

    namespace A.B.C
    """

        match content with
        | [ TopLevelNamespace "" [] ] -> ()
        | content -> Assert.Fail($"Unexpected content: {content}")
