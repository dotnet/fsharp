module FSharp.Compiler.Service.Tests.CompletionMutabilityTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``AdjacentToDot_20`` () =
    let info = Checker.getCompletionInfo "System.Console.{caret}()<-"

    assertHasItemWithNames [ "BackgroundColor" ] info

[<Fact>]
let ``AdjacentToDot_20_Negative`` () =
    let info = Checker.getCompletionInfo "System.Console.()<-{caret}"

    assertHasItemWithNames [ "abs" ] info
    assertHasNoItemsWithNames [ "BackgroundColor" ] info

let private obsoletePreamble = """[<System.ObsoleteAttribute("!", false)>]
module ObsoleteTop =
  let T = "T"
module Module =
  [<System.ObsoleteAttribute("!", false)>]
  module ObsoleteM =
    let A = "A"
    [<System.ObsoleteAttribute("!", false)>]
    module ObsoleteNested =
      let C = "C"
  [<System.ObsoleteAttribute("!", false)>]
  type ObsoleteT =
    static member B = "B"
  let Other = 0
let mutable level = ""

"""

let obsoleteCases: obj[] seq =
    [
        [| box "level <- O{caret}"; box [ "None" ]; box [ "ObsoleteTop"; "Chars" ] |]
        [| box "level <- Module.{caret}"; box [ "Other" ]; box [ "ObsoleteM"; "ObsoleteT"; "Chars" ] |]
        [| box "level <- Module.ObsoleteM.{caret}"; box [ "A" ]; box [ "ObsoleteNested"; "Chars" ] |]
        [| box "level <- Module.ObsoleteM.ObsoleteNested.{caret}"; box [ "C" ]; box [ "Chars" ] |]
        [| box "level <- Module.ObsoleteT.{caret}"; box [ "B" ]; box [ "Chars" ] |]
    ]

[<Theory; MemberData(nameof obsoleteCases)>]
let ``Obsolete.completion`` (completionLine: string) (included: string list) (excluded: string list) =
    let info = Checker.getCompletionInfo (obsoletePreamble + completionLine)
    assertHasItemWithNames included info
    assertHasNoItemsWithNames excluded info

[<Fact>]
let ``Identifier.InClass.WithoutDef`` () =
    let info =
        Checker.getCompletionInfo
            """
                type Type2 =
                    val mutable x.{caret} : string"""

    Assert.Equal(0, info.Items.Length)
