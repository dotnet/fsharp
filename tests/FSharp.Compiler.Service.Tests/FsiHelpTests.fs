namespace FSharp.Compiler.UnitTests

open FSharp.Test.Assert
open Xunit

module FsiHelpTests =

    [<Fact>]
    let ``Can get help for FSharp.Compiler.Xml.PreXmlDoc.Create`` () =
        match FSharp.Compiler.Interactive.FsiHelp.Logic.Quoted.tryGetHelp <@ FSharp.Compiler.Xml.PreXmlDoc.Create @> with
        | ValueSome h ->
            h.Assembly |> shouldBe "FSharp.Compiler.Service.dll"
            h.FullName |> shouldBe "FSharp.Compiler.Xml.PreXmlDoc.Create"

            h.Summary
            |> shouldBe "Create a PreXmlDoc from a collection of unprocessed lines"
        | ValueNone -> Assert.True(false, "No xml documentation found")

    [<Fact>]
    let ``Can get help for FSharp.Compiler.Syntax.SyntaxNodes.tryPickLast`` () =
        match FSharp.Compiler.Interactive.FsiHelp.Logic.Quoted.tryGetHelp <@ FSharp.Compiler.Syntax.SyntaxNodes.tryPickLast @> with
        | ValueSome h ->
            h.Assembly |> shouldBe "FSharp.Compiler.Service.dll"
            h.FullName |> shouldBe "FSharp.Compiler.Syntax.SyntaxNodesModule.tryPickLast"
            Assert.StartsWith("Applies the given function to each node of the AST and ", h.Summary)
            h.Parameters |> shouldNotBeEmpty
            h.Returns.IsSome |> shouldBeTrue
            h.Examples |> shouldNotBeEmpty
        | ValueNone -> Assert.True(false, "No xml documentation found")

    [<Fact>]
    let ``Can get help for FSComp.SR.considerUpcast`` () =
        match FSharp.Compiler.Interactive.FsiHelp.Logic.Quoted.tryGetHelp <@ FSComp.SR.considerUpcast @> with
        | ValueSome h ->
            h.Assembly |> shouldBe "FSharp.Compiler.Service.dll"
            h.FullName |> shouldBe "FSComp.SR.considerUpcast"
            Assert.StartsWith("The conversion from %s to %s is a compile-time safe upcast", h.Summary)
        | ValueNone -> Assert.True(false, "No xml documentation found")

    [<Fact>]
    let ``Can get help for FSharp.Test.ReflectionHelper.shouldn't`` () =
        match FSharp.Compiler.Interactive.FsiHelp.Logic.Quoted.tryGetHelp <@ FSharp.Test.ReflectionHelper.shouldn't @> with
        | ValueSome h ->
            h.Assembly |> shouldBe "FSharp.Test.Utilities.dll"
            h.FullName |> shouldBe "FSharp.Test.ReflectionHelper.shouldn't"
            Assert.StartsWith("Assert that function f ", h.Summary)
        | ValueNone -> Assert.True(false, "No xml documentation found")

    [<Fact>]
    let ``Can't get help for non-identifier`` () =
        match FSharp.Compiler.Interactive.FsiHelp.Logic.Quoted.tryGetHelp <@ 23 @> with
        | ValueSome h -> Assert.True(false, "No xml documentation expected")
        | ValueNone -> ()
