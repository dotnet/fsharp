module FSharp.Compiler.Service.Tests.CompletionConditionalsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``ValueDeclarationHidden.Bug4405`` () =
    let info =
        Checker.getCompletionInfo
            """do  
  let a = "string"
  let a = if true then 0 else a.{caret}"""

    assertHasItemWithNames [ "IndexOf"; "Substring" ] info

[<Fact>]
let ``Parameter.DirectAfterDefined.Bug2884`` () =
    let info =
        Checker.getCompletionInfo
            """if true then
  let aaa1 = 0
  ({caret}"""

    assertHasItemWithNames [ "aaa1" ] info

[<Fact>]
let ``COMPILED.DefineNotPropagatedToIncrementalBuilder`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "--define:COMPILED" |]
            FSharpCodeCompletionOptions.Default
            """module File1 =
#if COMPILED
    let x = 0
#else
    let y = 1
#endif

module File2 =
    File1.{caret}"""

    assertHasItemWithNames [ "x" ] info
    assertHasNoItemsWithNames [ "y" ] info
    Assert.Equal(1, info.Items.Length)

[<Fact>]
let ``Keywords.If`` () =
    let info =
        Checker.getCompletionInfo
            """
                if.{caret} true then
                    () """

    Assert.Equal(0, info.Items.Length)

[<Fact(Skip = "Bug 3627 - Completion lists should be filtered in many contexts")>]
let ``NotShowPInvokeSignature`` () =
    let info =
        Checker.getCompletionInfo
            """let x = "System.Console"
#if RELEASE
System.Console.{caret}
#endif
()"""

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``Regression4405.Identifier.ReBound`` () =
    let info =
        Checker.getCompletionInfo
            """
                let f x =
                    let varA = "string"
                    let varA = if x then varA.{caret} else 2
                    varA"""

    assertHasItemWithNames [ "Chars"; "StartsWith" ] info
