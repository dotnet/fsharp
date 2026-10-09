module FSharp.Compiler.Service.Tests.CompletionConstraintsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``AutoCompletion.OnTypeConstraintError`` () =
    let info =
        Checker.getCompletionInfo
            """type Foo = Foo
    with
        member _.Bar = 1
        member _.PublicMethodForIntellisense() = 2
        member internal _.InternalMethod() = 3
        member private _.PrivateProperty = 4

let u: Unit =
    [ Foo ]
    |> List.map (fun abcd -> abcd.{caret})"""

    assertHasItemWithNames [ "Bar"; "Equals"; "GetHashCode"; "GetType"; "InternalMethod"; "PublicMethodForIntellisense"; "ToString" ] info

[<Fact>]
let ``ConstrainedTypes`` () =
    let info1 =
        Checker.getCompletionInfo
            """
            type Pet() =
                member x.Name() = "pet"
                member x.Speak() = "this is a pet"
            type Dog() =
                inherit Pet()
                member x.dog() = "this is a dog"
            let dog = new Dog()
            let pet = dog :> Pet
            pet.{caret}
            let dctest = pet :?> Dog
            dctest(*Mdowncast*)
            let f (x : bigint) = x(*Mconstrainedtoint*)
            """

    assertHasItemWithNames [ "Name"; "Speak" ] info1

    let info2 =
        Checker.getCompletionInfo
            """
            type Pet() =
                member x.Name() = "pet"
                member x.Speak() = "this is a pet"
            type Dog() =
                inherit Pet()
                member x.dog() = "this is a dog"
            let dog = new Dog()
            let pet = dog :> Pet
            pet(*Mupcast*)
            let dctest = pet :?> Dog
            dctest.{caret}
            let f (x : bigint) = x(*Mconstrainedtoint*)
            """

    assertHasItemWithNames [ "dog"; "Name" ] info2

    let info3 =
        Checker.getCompletionInfo
            """
            type Pet() =
                member x.Name() = "pet"
                member x.Speak() = "this is a pet"
            type Dog() =
                inherit Pet()
                member x.dog() = "this is a dog"
            let dog = new Dog()
            let pet = dog :> Pet
            pet(*Mupcast*)
            let dctest = pet :?> Dog
            dctest(*Mdowncast*)
            let f (x : bigint) = x.{caret}
            """

    assertHasItemWithNames [ "ToString" ] info3

[<Fact>]
let ``Identifier.EqualityConstraint.Bug65730`` () =
    let info =
        Checker.getCompletionInfo
            """let g3<'a when 'a : equality> (x:'a) = x.{caret}"""

    assertHasItemWithNames [ "Equals"; "GetHashCode" ] info
