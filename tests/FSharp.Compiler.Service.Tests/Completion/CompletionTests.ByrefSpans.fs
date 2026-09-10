module FSharp.Compiler.Service.Tests.CompletionByrefSpansTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``CLIEventsWithByRefArgs`` () =
    let info =
        Checker.getCompletionInfo
            """type MyDelegate = delegate of obj * string byref  -> unit
type mytype() = [<CLIEvent>] member this.myEvent = (new DelegateEvent<MyDelegate>()).Publish
let t = mytype()
t.{caret}"""

    assertHasItemWithNames [ "add_myEvent"; "remove_myEvent" ] info
    assertHasNoItemsWithNames [ "myEvent" ] info
