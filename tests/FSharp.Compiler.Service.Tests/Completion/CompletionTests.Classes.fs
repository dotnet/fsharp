module FSharp.Compiler.Service.Tests.CompletionClassesTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``AutoCompletion.BeforeThis`` () =
    let plain =
        Checker.getCompletionInfo
            """type A() =
   member _.X = ()
   member this.{caret}"""

    let privateMember =
        Checker.getCompletionInfo
            """type A() =
   member _.X = ()
   member private this.{caret}"""

    let publicMember =
        Checker.getCompletionInfo
            """type A() =
   member _.X = ()
   member public this.{caret}"""

    let internalMember =
        Checker.getCompletionInfo
            """type A() =
   member _.X = ()
   member internal this.{caret}"""

    Assert.Equal(0, plain.Items.Length)
    Assert.Equal(0, privateMember.Items.Length)
    Assert.Equal(0, publicMember.Items.Length)
    Assert.Equal(0, internalMember.Items.Length)

[<Fact>]
let ``Completion.DetectInvalidCompletionContext`` () =
    let dotOnly =
        Checker.getCompletionInfo
            """type X =
    inherit System {caret}."""

    let dotCollections =
        Checker.getCompletionInfo
            """type X =
    inherit System {caret}.Collections"""

    Assert.Equal(0, dotOnly.Items.Length)
    Assert.Equal(0, dotCollections.Items.Length)

[<Fact>]
let ``Completion.LongIdentifiers`` () =
    let trailingSpaces =
        Checker.getCompletionInfo
            """type X = 
   inherit System.   {caret}"""

    let nextLineComment =
        Checker.getCompletionInfo
            """type X = 
   inherit System.
             {caret}"""

    let leadingDotNextLine =
        Checker.getCompletionInfo
            """type X = 
   inherit System
           .{caret}"""

    let moduleCandidates =
        Checker.getCompletionInfo
            """module Mod =
    let x = 1
module Mod2 = 
    let x = 1
type X = 
   inherit Mod{caret}"""

    let partialSystem =
        Checker.getCompletionInfo
            """type X = 
   inherit Sys{caret}"""

    let partialCollection =
        Checker.getCompletionInfo
            """type X = 
   inherit System.Col{caret}lection"""

    let dotSpaceCollections =
        Checker.getCompletionInfo
            """type X = 
   inherit System. {caret} Collections"""

    let dotSpaceArrayList =
        Checker.getCompletionInfo
            """type X = 
   inherit System. {caret} Collections.ArrayList()"""

    assertHasItemWithNames [ "IDisposable"; "Array" ] trailingSpaces
    assertHasItemWithNames [ "IDisposable"; "Array" ] nextLineComment
    assertHasItemWithNames [ "IDisposable"; "Array" ] leadingDotNextLine
    assertHasItemWithNames [ "Mod"; "Mod2" ] moduleCandidates
    assertHasItemWithNames [ "System"; "obj" ] partialSystem
    assertHasItemWithNames [ "Collections"; "IDisposable" ] partialCollection
    assertHasItemWithNames [ "Collections"; "IDisposable" ] dotSpaceCollections
    assertHasItemWithNames [ "Collections"; "IDisposable" ] dotSpaceArrayList

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``AfterConstructor.5039_1`` () =
    let info =
        Checker.getCompletionInfo
            """let someCall(x) = null
let xe = someCall(System.IO.StringReader().{caret}"""

    assertHasItemWithNames [ "ReadBlock" ] info
    assertHasNoItemsWithNames [ "LastIndexOfAny" ] info

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``AfterConstructor.5039_1.CoffeeBreak`` () =
    let info =
        Checker.getCompletionInfo
            """let someCall(x) = null
let xe = someCall(System.IO.StringReader().{caret}"""

    assertHasItemWithNames [ "ReadBlock" ] info
    assertHasNoItemsWithNames [ "LastIndexOfAny" ] info

[<Fact>]
let ``AfterConstructor.5039_2`` () =
    let info = Checker.getCompletionInfo "System.Random().{caret}"

    assertHasItemWithNames [ "NextDouble" ] info

[<Fact>]
let ``AfterConstructor.5039_4`` () =
    let info = Checker.getCompletionInfo "System.Collections.Generic.List().{caret}"

    assertHasItemWithNames [ "BinarySearch" ] info

[<Fact>]
let ``NameSpace.AsConstructor`` () =
    let info = Checker.getCompletionInfo "new System.DateTime({caret})"

    assertHasItemWithNames [ "System"; "Array2D" ] info
    assertHasNoItemsWithNames [ "DaysInMonth"; "AddDays" ] info

[<Fact>]
let ``Bug243082.DotAfterNewBreaksCompletion`` () =
    let info =
        Checker.getCompletionInfo
            """module A =
    type B() = class end
let s = 1
s.{caret}
let z = new A."""

    assertHasItemWithNames [ "CompareTo"; "ToString" ] info

let bug2884Cases: obj[] seq =
    [
        [| box "type T1(aaa1) =\n  do ({caret}"; box [ "aaa1" ] |]
        [| box "type T1(aaa1) =\n  do ({caret}\nlet a = 0"; box [ "aaa1" ] |]
        [| box "type T1(aaa1) =\n  member x.Foo(aaa2) = \n    do ({caret}\n  member x.Bar = 0"; box [ "aaa1"; "aaa2" ] |]
        [| box "type T1(aaa1) =\n  member x.Foo(aaa2) = \n    let dt = new System.DateTime({caret}"; box [ "aaa1"; "aaa2" ] |]
    ]

[<Theory; MemberData(nameof bug2884Cases)>]
let ``Parameter.Bug2884`` (source: string) (expected: string list) =
    assertHasItemWithNames expected (Checker.getCompletionInfo source)

[<Fact>]
let ``CaseInsensitive`` () =
    let info =
        Checker.getCompletionInfo
            """
                type Test() =
                    member this.Xyzzy = ()
                    member this.xYzzy = ()
                    member this.xyZzy = ()
                    member this.xyzZy = ()
                    member this.xyzzY = ()
                let t = new Test()
                t.XYZ{caret}
                """

    assertHasItemWithNames [ "Xyzzy"; "xYzzy"; "xyZzy"; "xyzZy"; "xyzzY" ] info

[<Fact>]
let ``ObjInstance.InheritedClass.MethodsDefInBase`` () =
    let info =
        Checker.getCompletionInfo
            """
                type Pet() =
                    member x.Name() = "pet"
                    member x.Speak() = "this is a pet"
                type Dog() =
                    inherit Pet()
                    member x.dog() = "this is a dog"
                let dog = new Dog()
                dog.{caret}"""

    assertHasItemWithNames [ "Name"; "dog" ] info

[<Theory>]
[<InlineData 153>] // Class.Self.Bug1544
[<InlineData 441>] // MemberSelf
[<InlineData 450>] // Identifier.AsClassName.InInitial
let ``Identifier.DeclarationPositionDotIsEmpty`` (caseId: int) =
    let source =
        match caseId with
        | 153
        | 441 ->
            """
                type Foo() =
                    member this.{caret}"""
        | _ ->
            """
                type f1.{caret} =
                    val field: int"""

    let info = Checker.getCompletionInfo source

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``SelfParameter.InDoKeywordScope`` () =
    let info =
        Checker.getCompletionInfo
            """
                type foo() as this =
                    do
                        this.{caret}"""

    assertHasItemWithNames [ "ToString" ] info

[<Fact>]
let ``SelfParameter.InDoKeywordScope.Negative`` () =
    let info =
        Checker.getCompletionInfo
            """
                type foo() as this =
                    do
                        this.{caret}"""

    assertHasNoItemsWithNames [ "Value"; "Contents" ] info

[<Fact(Skip = "disabled upstream; exclusion assertion preserved")>]
let ``AutoComplete.Bug72596_A`` () =
    let info =
        Checker.getCompletionInfo
            """type ClassType() =
    let foo = fo{caret}"""

    assertHasNoItemsWithNames [ "foo" ] info
