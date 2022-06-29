namespace FSharp.Compiler.ComponentTests.Conformance.LexicalFiltering.Basic

open Xunit
open FSharp.Test.Compiler

module FunctionTypes =

    [<Fact>]
    let ``function type alias`` () =
        Fsx """
type Meh =
    int ->
    string ->
        double
"""
        |> withLangVersionPreview
        |> withOptions ["--nowarn:20"]
        |> parse
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``function type alias with generics`` () =
        Fsx """
type GetParseResultsForFile =
    string<LocalPath> ->
    FSharp.Compiler.Text.Position ->
        Async<ResultOrString<ParseAndCheckResults * string * FSharp.Compiler.Text.ISourceText>>
        """
        |> withLangVersionPreview
        |> withOptions ["--nowarn:20"]
        |> parse
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``function type alias with parentheses`` () =
        Fsi """
module Foo.Bar

val GetParseResultsForFile :
    (A ->
     B ->
      C)
"""     |> withLangVersionPreview
        |> withOptions ["--nowarn:20"]
        |> parse
        |> shouldSucceed
        |> ignore
        
    [<Fact>]
    let ``record types with function type alias declarations`` () =
        Fsx """
type R =
    { F: (A ->
          B ->
            C) }
"""     |> withLangVersionPreview
        |> withOptions ["--nowarn:20"]
        |> parse
        |> shouldSucceed
        |> ignore
