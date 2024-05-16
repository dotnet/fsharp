namespace FSharp.Compiler.UnitTests

open FSharp.Test.Assert
open Xunit

[<CollectionDefinition("FsiHelpTests", DisableParallelization = false)>]
module FsiHelpTests =

    [<Fact>]
    let ``Can get help for FSharp.Compiler.Xml.PreXmlDoc.Create`` () =
        match FSharp.Compiler.Interactive.FsiHelp.Logic.Quoted.tryGetDocumentation <@ FSharp.Compiler.Xml.PreXmlDoc.Create @> with
        | ValueSome h ->
            h.Assembly |> shouldBe "FSharp.Compiler.Service.dll"
            h.FullName |> shouldBe "FSharp.Compiler.Xml.PreXmlDoc.Create"

            h.Summary
            |> shouldBe "Create a PreXmlDoc from a collection of unprocessed lines"
        | ValueNone -> Assert.True(false, "No xml documentation found")

    [<Fact>]
    let ``Can get help for FSharp.Compiler.Syntax.SyntaxNodes.tryPickLast`` () =
        match FSharp.Compiler.Interactive.FsiHelp.Logic.Quoted.tryGetDocumentation <@ FSharp.Compiler.Syntax.SyntaxNodes.tryPickLast @> with
        | ValueSome h ->
            h.Assembly |> shouldBe "FSharp.Compiler.Service.dll"
            h.FullName |> shouldBe "FSharp.Compiler.Syntax.SyntaxNodesModule.tryPickLast"
            Assert.StartsWith("Applies the given function to each node of the AST and ", h.Summary)
            h.Parameters |> shouldNotBeEmpty
            h.Returns.IsSome |> shouldBeTrue
            h.Examples |> shouldNotBeEmpty
        | ValueNone -> Assert.True(false, "No xml documentation found")
