module FSharp.Compiler.Service.Tests.CompletionDiscriminatedUnionsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``AutoCompletion.ObjectMethods`` () =
    let source (tail: string) =
        sprintf
            """type DU1 = DU_1
[<NoEquality>]
type DU2 = DU_2
[<NoEquality>]
type DU3 =
   | DU_3
   with member this.Equals(b : string) = 1
[<NoEquality>]
type DU4 =
   | DU_4
   with member this.GetHashCode(b : string) = 1
module Extensions =
    type System.Object with
       member this.ExtensionPropObj = 42
       member this.ExtensionMethodObj () = 42
open Extensions
%s"""
            tail

    let cases =
        [ "obj().{caret}", [ "Equals"; "ExtensionPropObj"; "ExtensionMethodObj" ], []
          "System.Object.{caret}", [ "Equals"; "ReferenceEquals" ], []
          "System.String.{caret}", [ "Equals" ], []
          "DU_1.{caret}", [ "Equals"; "GetHashCode"; "ExtensionMethodObj"; "ExtensionPropObj" ], []
          "DU_2.{caret}", [ "ExtensionPropObj"; "ExtensionMethodObj" ], [ "Equals"; "GetHashCode" ]
          "DU_3.{caret}", [ "ExtensionPropObj"; "ExtensionMethodObj"; "Equals" ], [ "GetHashCode" ]
          "DU_4.{caret}", [ "ExtensionPropObj"; "ExtensionMethodObj"; "GetHashCode" ], [ "Equals" ] ]

    for tail, expected, notExpected in cases do
        let info = Checker.getCompletionInfo (source tail)
        assertHasItemWithNames expected info

        if not (List.isEmpty notExpected) then
            assertHasNoItemsWithNames notExpected info

[<Fact>]
let ``SimpleTypes.DisUnion`` () =
    let info =
        Checker.getCompletionInfo
            """
                type Route = int
                type Make = string
                type Model = string
                type Transport =
                    | Car of Make * Model
                    | Bicycle
                    | Bus of Route
                let typediscriminatedunion = Car("BMW","360")
                typediscriminatedunion.{caret}"""

    assertHasItemWithNames [ "GetType"; "ToString" ] info

[<Fact>]
let ``VariableIdentifier.MethodsInheritFromBase`` () =
    let info =
        Checker.getCompletionInfo
            """
                namespace MyNamespace1
                module MyModule =
                    type DuType =
                         | Tag of int
                    let f (DuType(*Maftervariable1*).Tag(x)) = 10
                    type Pet() =
                        member x.Name = "pet"
                        member x.Speak() = "this is a pet"
                    type Dog() =
                        inherit Pet()
                        do base.{caret}GetType()
                    let dog = new Dog()"""

    assertHasItemWithNames [ "Name"; "Speak" ] info

[<Theory>]
[<InlineData("""
                namespace MyNamespace1
                module MyModule =
                    type DuType =
                         | Tag of int
                    let f (DuType(*Maftervariable1*).Tag(x)) = 10
                    type Pet() =
                        member x.Name = "pet"
                        member x.Speak() = "this is a pet"
                    type Dog() =
                        inherit Pet()
                        do base(*Maftervariable3*).GetType()
                    let dog = new Dog()
                namespace MyNamespace2
                module MyModule2 =
                    let typeFunc<MyNamespace1.MyModule(*Maftervariable2*)> = [1; 2; 3]
                    let f (x:MyNamespace1.MyModule.{caret}) = 10
                    let y = int System.IO(*Maftervariable5*)""",
             "DuType")>]
[<InlineData("""
                namespace MyNamespace1
                module MyModule =
                    type DuType =
                         | Tag of int
                    let f (DuType(*Maftervariable1*).Tag(x)) = 10
                    type Pet() =
                        member x.Name = "pet"
                        member x.Speak() = "this is a pet"
                    type Dog() =
                        inherit Pet()
                        do base(*Maftervariable3*).GetType()
                    let dog = new Dog()
                namespace MyNamespace2
                module MyModule2 =
                    let typeFunc<MyNamespace1.MyModule(*Maftervariable2*)> = [1; 2; 3]
                    let f (x:MyNamespace1.MyModule(*Maftervariable4*)) = 10
                    let y = int System.IO.{caret}""",
             "BinaryReader;Stream;Directory")>]
let ``VariableIdentifier.DefInDiffNamespace`` (markedSource: string) (names: string) =
    let info = Checker.getCompletionInfo markedSource

    assertHasItemWithNames (names.Split(';') |> List.ofArray) info

[<Theory>]
[<InlineData("""
                namespace MyNamespace1
                module MyModule =
                    type DuType =
                         | Tag of int
                    type Pet() =
                        member x.Name = "pet"
                        member x.Speak() = "this is a pet"
                    type Dog() =
                        inherit Pet()
                namespace MyNamespace2
                module MyModule2 =
                    let foo = MyNamespace1.MyModule.{caret}
                    let f (x:int) = MyNamespace1.MyModule.DuType(*Mtypeparameter2*)
                    let typeFunc<[<MyNamespace1.MyModule(*Mtypeparameter3*)>] 'a> = 10""",
             "Dog;DuType")>]
[<InlineData("""
                namespace MyNamespace1
                module MyModule =
                    type DuType =
                         | Tag of int
                    type Pet() =
                        member x.Name = "pet"
                        member x.Speak() = "this is a pet"
                    type Dog() =
                        inherit Pet()
                namespace MyNamespace2
                module MyModule2 =
                    let foo = MyNamespace1.MyModule(*Mtypeparameter1*)
                    let f (x:int) = MyNamespace1.MyModule.DuType.{caret}
                    let typeFunc<[<MyNamespace1.MyModule(*Mtypeparameter3*)>] 'a> = 10""",
             "Tag")>]
let ``LongIdent.AsTypeParameter.DefInDiffNamespace`` (markedSource: string) (names: string) =
    let info = Checker.getCompletionInfo markedSource

    assertHasItemWithNames (names.Split(';') |> List.ofArray) info

[<Fact>]
let ``Identifier.InDiscUnion.WithoutDef`` () =
    let info =
        Checker.getCompletionInfo
            """
                type DUTag =
                    |Tag.{caret} of int"""

    Assert.Equal(0, info.Items.Length)

[<Fact(Skip = "disabled upstream; membership assertion preserved")>]
let ``VariableIdentifier.AsParameter`` () =
    let info =
        Checker.getCompletionInfo
            """
                module MyModule =
                    type DuType =
                         | Tag of int
                    let f (DuType.{caret}Tag(x)) = 10 """

    assertHasItemWithNames [ "Tag" ] info
