module FSharp.Compiler.Service.Tests.CompletionObjectExpressionsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``ObjInstance.AnonymousClass.MethodsDefInInterface`` () =
    let info =
        Checker.getCompletionInfo
            """
                type IFoo =
                    abstract DoStuff  : unit -> string
                    abstract DoStuff2 : int * int -> string -> string
                // Implement an interface in a class (This is kind of lame if you don't want to actually declare a class)
                type Foo() =
                    interface IFoo with
                        member this.DoStuff () = "Return a string"
                        member this.DoStuff2 (x, y) z = sprintf "Arguments were (%d, %d) %s" x y z
                // instanceOfIFoo is an instance of an anonymous class which implements IFoo
                let instanceOfIFoo = {
                                        new IFoo with
                                            member this.DoStuff () = "Implement IFoo"
                                            member this.DoStuff2 (x, y) z = sprintf "Arguments were (%d, %d) %s" x y z
                                     }.{caret}"""

    assertHasItemWithNames [ "DoStuff"; "DoStuff2" ] info
