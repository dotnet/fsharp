module FSharp.Compiler.Service.Tests.CompletionGenericsTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact>]
let ``AfterConstructor.5039_3`` () =
    let info =
        Checker.getCompletionInfo
            """
System.Collections.Generic.List<int>().{caret}"""

    assertHasItemWithNames [ "BinarySearch" ] info

let private genericsPreamble = """
type GT<'a> =
  static member P = 12
  static member Q = 13
type GT2 =
  static member R = 12
  static member S = 13
type D = | DD
let td = typeof<D>
let f i = typeof<D>
"""

let genericsMemberCases: obj[] seq =
    [ [| box "let _ = typeof<int>.{caret}"; box [ "Assembly"; "AssemblyQualifiedName" ] |]
      [| box "let _ = GT2.{caret}"; box [ "R"; "S" ] |]
      [| box "let _ = GT<int>.{caret}"; box [ "P"; "Q" ] |] ]

[<Theory; MemberData(nameof genericsMemberCases)>]
let ``Generics member completion`` (completionLine: string) (expected: string list) =
    assertHasItemWithNames expected (Checker.getCompletionInfo (genericsPreamble + "\n" + completionLine))

[<Fact(Skip = "Not worth fixing right now")>]
let ``GenericType.Self.Bug69673_1.01`` () =
    let info =
        Checker.getCompletionInfo
            """
type Base(o:obj) = class end
type Foo() as this =
    inherit Base(th{caret}is) // this
    let o = this // this ok
    do this.Bar() // this ok, dotting ok
    member this.Bar() = ()"""

    assertHasItemWithNames [ "this" ] info

[<Theory>]
[<InlineData("""
type Base(o:obj) = class end
type Foo() as this =
    inherit Base(this) // this
    let o = th{caret}is // this ok
    do this.Bar() // this ok, dotting ok
    member this.Bar() = ()""")>]
[<InlineData("""
type Base(o:obj) = class end
type Foo() as this =
    inherit Base(this) // this
    let o = this // this ok
    do th{caret}is.Bar() // this ok, dotting ok
    member this.Bar() = ()""")>]
let ``GenericType.Self.Bug69673_1.CtrlSpaceForThis`` (markedSource: string) =
    let info = Checker.getCompletionInfo markedSource
    assertHasItemWithNames [ "this" ] info

[<Fact>]
let ``GenericType.Self.Bug69673_1.04`` () =
    let info =
        Checker.getCompletionInfo
            """
type Base(o:obj) = class end
type Foo() as this =
    inherit Base(this) // this
    let o = this // this ok
    do this.{caret}Bar() // this ok, dotting ok
    member this.Bar() = ()"""

    assertHasItemWithNames [ "Bar" ] info

[<Fact(Skip = "this is not worth fixing")>]
let ``GenericType.Self.Bug69673_2.1`` () =
    let info =
        Checker.getCompletionInfo
            """
type Base(o:obj) = class end
type Food() as this =
    class
    inherit Base(th{caret}is) // this
    do
        this |> ignore // this (only repros with explicit class/end)
    end"""

    assertHasItemWithNames [ "this" ] info

[<Fact(Skip = "this is not worth fixing")>]
let ``GenericType.Self.Bug69673_2.2`` () =
    let info =
        Checker.getCompletionInfo
            """
type Base(o:obj) = class end
type Food() as this =
    class
    inherit Base(this) // this
    do
        th{caret}is |> ignore // this (only repros with explicit class/end)
    end"""

    assertHasItemWithNames [ "this" ] info

[<Fact(Skip = "Bug 3627 - Completion lists should be filtered in many contexts")>]
let ``AfterTypeParameter`` () =
    let info1 =
        Checker.getCompletionInfo
            """

            type Type1 = Tag of string.{caret}

            let f x:int -> string(*MarkerParamFunction*)

            let Type2<'a(*MarkerGenericParam*)> = 1
   
            let type1 = typeof<int(*MarkerParamTypeof*)>
            """

    Assert.Equal(0, info1.Items.Length)

    let info2 =
        Checker.getCompletionInfo
            """

            type Type1 = Tag of string(*MarkerDUTypeParam*)

            let f x:int -> string.{caret}

            let Type2<'a(*MarkerGenericParam*)> = 1
   
            let type1 = typeof<int(*MarkerParamTypeof*)>
            """

    Assert.Equal(0, info2.Items.Length)

    let info3 =
        Checker.getCompletionInfo
            """

            type Type1 = Tag of string(*MarkerDUTypeParam*)

            let f x:int -> string(*MarkerParamFunction*)

            let Type2<'a.{caret}> = 1
   
            let type1 = typeof<int(*MarkerParamTypeof*)>
            """

    Assert.Equal(0, info3.Items.Length)

    let info4 =
        Checker.getCompletionInfo
            """

            type Type1 = Tag of string(*MarkerDUTypeParam*)

            let f x:int -> string(*MarkerParamFunction*)

            let Type2<'a(*MarkerGenericParam*)> = 1
   
            let type1 = typeof<int.{caret}>
            """

    Assert.Equal(0, info4.Items.Length)
