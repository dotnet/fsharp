module FSharp.Compiler.Service.Tests.CompletionInterfacesTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``Completion.DetectInterfaces`` () =
    let info1 =
        Checker.getCompletionInfo
            """type X = interface
    inherit {caret}"""

    assertHasItemWithNames [ "seq" ] info1

    let info2 =
        Checker.getCompletionInfo
            """[<Interface>]
type X =
    inherit {caret}"""

    assertHasItemWithNames [ "seq" ] info2

    let info3 =
        Checker.getCompletionInfo
            """[<Interface>]
type X = interface
    inherit {caret}"""

    assertHasItemWithNames [ "seq" ] info3
