module FSharp.Compiler.Service.Tests.CompletionLetBindingsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``CtrlSpaceCompletion.Bug130670.Case2`` () =
    let info =
        Checker.getCompletionInfo
            """
let x = 42
let r = x + 1 {caret}"""

    assertHasItemWithNames [ "AbstractClassAttribute" ] info
    assertHasNoItemsWithNames [ "CompareTo" ] info

[<Fact(Skip = "non-FCS: completion suppression inside a string literal is an editor-layer concern (CompletionUtils.shouldProvideCompletion), not reproducible via the FCS GetDeclarationListInfo API which resolves the qualified 'System.' island regardless of the enclosing string (empirically returns 251 items here).")>]
let ``InComment`` () =
    let info = Checker.getCompletionInfo """ let s = "System.C{caret}" """

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``TopLevelIdentifier.AfterPartialToken1`` () =
    let info =
        Checker.getCompletionInfo
            """let foobaz = 1
(*marker*)fo{caret}"""

    assertHasItemWithNames [ "System"; "Array2D"; "foobaz" ] info
    assertHasNoItemsWithNames [ "Int32" ] info

[<Fact>]
let ``TopLevelIdentifier.AfterPartialToken2`` () =
    let info =
        Checker.getCompletionInfo
            """let foobaz = 1
{caret}fo"""

    assertHasItemWithNames [ "System"; "Array2D"; "foobaz" ] info

[<Fact>]
let ``NonDotCompletion`` () =
    let info = Checker.getCompletionInfo "let x = S{caret}"

    assertHasItemWithNames [ "Some" ] info

[<Theory>]
[<InlineData("""let x = 42
x   .  C{caret}""")>]
[<InlineData("""let x = 42
x   .  {caret}""")>]
[<InlineData("""let x = 42
id(x)   .  C{caret}""")>]
[<InlineData("""let x = 42
id(x)   .  {caret}""")>]
let ``Residues`` (source: string) =
    let info = Checker.getCompletionInfo source

    assertHasItemWithNames [ "CompareTo" ] info
    assertHasNoItemsWithNames [ "CLIEventAttribute"; "Checked"; "Choice" ] info

[<Fact>]
let ``CompletionInDifferentEnvs2`` () =
    let info =
        Checker.getCompletionInfo
            """let aaa = 1
let aab = 2
(aa{caret}
let aac = 3"""

    assertHasItemWithNames [ "aaa"; "aab" ] info
    assertHasNoItemsWithNames [ "aac" ] info

[<Fact>]
let ``Selection`` () =
    let info =
        Checker.getCompletionInfo
            """
let preSelectedItem = 1
let r = (*MarkerPreSelectedItem*)pre{caret}"""

    assertHasItemWithNames [ "preSelectedItem" ] info

[<Fact>]
let ``CompListInDiffFileTypes`` () =
    let sigInfo =
        Checker.getCompletionInfoOfSignatureFile
            """
val x:int = 1
x.{caret}"""

    Assert.Equal(0, sigInfo.Items.Length)

    let info =
        Checker.getCompletionInfo
            """
let i = 1
i.{caret}"""

    assertHasItemWithNames [ "CompareTo"; "Equals" ] info

[<Fact>]
let ``Keywords.Let`` () =
    let info = Checker.getCompletionInfo "let.{caret} a = 1"

    Assert.Equal(0, info.Items.Length)

[<Fact(Skip = "non-FCS: completion suppression inside a string literal is an editor-layer concern (CompletionUtils.shouldProvideCompletion), not reproducible via the FCS GetDeclarationListInfo API which resolves the qualified 'System.Console' island regardless of the enclosing string (empirically returns 48 items here).")>]
let ``Expression.InString`` () =
    let info = Checker.getCompletionInfo """let x = "System.Console.{caret}" """

    Assert.Equal(0, info.Items.Length)
