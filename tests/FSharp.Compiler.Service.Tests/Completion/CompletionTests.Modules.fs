module FSharp.Compiler.Service.Tests.CompletionModulesTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``String.BeforeIncompleteModuleDefinition.Bug2385`` () =
    let info =
        Checker.getCompletionInfo
            """let s = "hello".{caret}
module Timer ="""

    assertHasItemWithNames [ "Substring"; "GetHashCode" ] info

[<Fact>]
let ``Identifier.DefineByVal.InFsiFile.Bug882304_1`` () =
    let info =
        Checker.getCompletionInfoOfSignatureFile
            """module BasicTest
val z:int = 1
z.{caret}"""

    assertHasNoItemsWithNames [ "Equals" ] info

[<Fact>]
let ``ShowSetAsModuleAndType`` () =
    let info = Checker.getCompletionInfo "let s = Set{caret}"

    let tip = flattenItemDescription (findCompletionItem "Set" info).Description
    Assert.Contains("module Set", tip)
    Assert.Contains("type Set", tip)

[<Fact>]
let ``Expression.WithPreDefinedMethods`` () =
    let info =
        Checker.getCompletionInfo
            """
                module Module1 =
                    let private PrivateField = 1
                    let private PrivateMethod x =
                        x+1
                    type private PrivateType() =
                        member this.mem = 1
                    let a = {caret}

                    let b = 23
                """

    assertHasItemWithNames [ "PrivateField"; "PrivateMethod"; "PrivateType" ] info

[<Fact>]
let ``Identifier.AsModule`` () =
    let info = Checker.getCompletionInfo "module Module1.{caret}"

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``TypeAbbreviation.Positive`` () =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest

                Microsoft.FSharp.Core.{caret}"""

    assertHasItemWithNames [ "int16"; "int32"; "int64" ] info

[<Fact>]
let ``TypeAbbreviation.Negative`` () =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest

                Microsoft.FSharp.Core.{caret}"""

    assertHasNoItemsWithNames [ "Int16"; "Int32"; "Int64" ] info

[<Fact>]
let ``Verify no completion on dot after module definition`` () =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest.{caret}

                let foo x = x
                let bar = 1"""

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``Verify no completion after module definition`` () =
    let info =
        Checker.getCompletionInfo
            """
                module BasicTest {caret}

                let foo x = x
                let bar = 1"""

    Assert.Equal(0, info.Items.Length)
