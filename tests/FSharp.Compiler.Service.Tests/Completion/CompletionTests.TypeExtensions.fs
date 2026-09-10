module FSharp.Compiler.Service.Tests.CompletionTypeExtensionsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``ObjectInitializer.CompletionForSettableExtensionProperties`` () =
    let info1 =
        Checker.getCompletionInfo
            """type A() = member this.SetXYZ(v: int) = ()
module Ext = type A with member this.XYZ with set(v) = this.SetXYZ(v)
open Ext
A((**){caret})"""

    assertHasItemWithNames [ "XYZ" ] info1

    let info2 =
        Checker.getCompletionInfo
            """type A() = member this.SetXYZ(v: int) = ()
module Ext = type A with member this.XYZ with set(v) = this.SetXYZ(v)
A((**){caret})"""

    assertHasNoItemsWithNames [ "XYZ" ] info2

[<Fact>]
let ``AfterMethod.Bug2296`` () =
    let info =
        Checker.getCompletionInfo
            """type System.Int32 with
  member x.Int32Member() = 0
"".CompareTo("a").{caret}"""

    assertHasItemWithNames [ "Int32Member" ] info

[<Fact>]
let ``AfterMethod.Overloaded.Bug2296`` () =
    let info =
        Checker.getCompletionInfo
            """type System.Boolean with
  member x.BooleanMember() = 0
"".Contains("a").{caret}"""

    assertHasItemWithNames [ "BooleanMember" ] info

[<Fact>]
let ``ObjInstance.ExtensionMethods.WithDef.Positive`` () =
    let info =
        Checker.getCompletionInfo
            """
                open System
                type System.Random with
                    member this.NextDice() = true
                    member this.DiceValue = 6
                let rnd = new System.Random()
                rnd.{caret}"""

    assertHasItemWithNames [ "NextDice"; "DiceValue" ] info
