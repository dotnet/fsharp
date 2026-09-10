module FSharp.Compiler.Service.Tests.CompletionMutuallyRecursiveTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``ProtectedMembers.SelfOrDerivedClass`` () =
    let info1 =
        Checker.getCompletionInfo
            """type T() = 
   inherit exn()
   member this.Run(x : T) = x.{caret}"""

    assertHasItemWithNames [ "Message"; "HResult" ] info1

    let info2 =
        Checker.getCompletionInfo
            """type T() = 
   inherit exn()
   member this.Run(x : Z) = x.{caret}
and Z() =
   inherit T()"""

    assertHasItemWithNames [ "Message"; "HResult" ] info2
