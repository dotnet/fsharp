module FSharp.Compiler.ComponentTests.TypeChecks.Graph.TestUtils

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text

let private checker = FSharpChecker.Create()

let parseSourceCode (name: string, code: string) =
    let sourceText = SourceText.ofString code

    let parsingOptions =
        { FSharpParsingOptions.Default with
            SourceFiles = [| name |]
        }

    let result =
        checker.ParseFile(name, sourceText, parsingOptions) |> Async.RunSynchronously

    result.ParseTree
