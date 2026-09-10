module FSharp.Compiler.Service.Tests.CompletionAccessibilityTests

open Xunit

[<Fact>]
let ``PrivateVisible`` () =
    let info =
        Checker.getCompletionInfo
            """
module CodeAccessibility

module Module1 =
    let private fieldPrivate = 1
    let private MethodPrivate x =
        x+1
    type private TypePrivate() =
        member this.mem = 1
    let a = (*Marker1*) {caret}"""

    assertHasItemWithNames [ "fieldPrivate"; "MethodPrivate"; "TypePrivate" ] info

[<Fact>]
let ``InternalVisible`` () =
    let info =
        Checker.getCompletionInfo
            """
module CodeAccessibility

module Module1 =
    let internal fieldInternal = 1
    let internal MethodInternal x =
        x+1
    type internal TypeInternal() =
        member this.mem = 1
    let a = (*Marker1*) {caret}"""

    assertHasItemWithNames [ "fieldInternal"; "MethodInternal"; "TypeInternal" ] info

let private widgetInheritanceSource =
    """
open System
//define the base class
type Widget() =
    let mutable state = 0
    member internal x.MethodInternal() = state
    member public x.MethodPublic(n) = state <- state + n
    member private x.MethodPrivate() = (state <> 0)
    [<DefaultValue>]
    val mutable internal fieldInternal:int
    [<DefaultValue>]
    val mutable public fieldPublic:int
    [<DefaultValue>]
    val mutable private fieldPrivate:int
//define the divided class which inherent "Widget"
type Divided() =
    inherit Widget()
    member x.myPrint() =
        base.{caret}
Console.ReadKey(true)"""

[<Fact>]
let ``InheritedClass.BaseClassPrivateMethod.Negative`` () =
    let info = Checker.getCompletionInfo widgetInheritanceSource
    assertHasNoItemsWithNames [ "MethodPrivate"; "fieldPrivate" ] info

[<Fact>]
let ``InheritedClass.BaseClassPublicMethodAndProperty`` () =
    let info = Checker.getCompletionInfo widgetInheritanceSource
    assertHasItemWithNames [ "MethodPublic"; "fieldPublic" ] info

[<Fact>]
let ``Visibility.InternalNestedClass.Negative`` () =
    let info = Checker.getCompletionInfo "System.Console.{caret}"

    assertHasNoItemsWithNames [ "ControlCDelegateData" ] info

[<Fact>]
let ``Visibility.PrivateIdentifierInDiffModule.Negative`` () =
    let info =
        Checker.getCompletionInfo
            """
module Module1 =
    let private fieldPrivate = 1
    let private MethodPrivate x =
        x+1
    type private TypePrivate()=
        member this.mem = 1
module Module2 =
    Module1.{caret}"""

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``Visibility.PrivateIdentifierInDiffClass.Negative`` () =
    let info =
        Checker.getCompletionInfo
            """
open System
module Module1 =
    type Type1()=
        [<DefaultValue>]
        val mutable private fieldPrivate:int
        member private x.MethodPrivate() = 1
    type Type2()=
        let M1=
        let type1 = new Type1()
        type1.{caret}"""

    assertHasNoItemsWithNames [ "fieldPrivate"; "MethodPrivate" ] info

[<Theory>]
[<InlineData("""
open System

module Module1 =
    type Type1()=
        [<DefaultValue>]
        val mutable private PrivateField:int
        static member private PrivateMethod() = 1
        member this.Field1 with get () = this.{caret}
        member x.MethodTest() = Type1(*MarkerMethodInType*)
    let type1 = new Type1() """,
            "PrivateField")>]
[<InlineData("""
open System

module Module1 =
    type Type1()=
        [<DefaultValue>]
        val mutable private PrivateField:int
        static member private PrivateMethod() = 1
        member this.Field1 with get () = this(*MarkerFieldInType*)
        member x.MethodTest() = Type1.{caret}
    let type1 = new Type1() """,
            "PrivateMethod")>]
let ``Visibility.PrivateMemberInSameClass`` (markedSource: string) (expected: string) =
    let info = Checker.getCompletionInfo markedSource

    assertHasItemWithNames [ expected ] info

[<Fact>]
let ``Visibility.InternalMethods.DefInSameAssembly`` () =
    let info =
        Checker.getCompletionInfo
            """
module CodeAccessibility
open System
module Module1 =
type Type1()=
    [<DefaultValue>]
    val mutable internal fieldInternal:int
    member internal x.MethodInternal (x:int) = x+2
let type1 = new Type1()
type1.{caret}"""

    assertHasItemWithNames [ "fieldInternal"; "MethodInternal" ] info

[<Theory>]
[<InlineData("""type Base =
   val mutable baseField : int
   val mutable private baseFieldPrivate : int
   new () = { baseField = 0; baseFieldPrivate=1 }
type Derived =
    val mutable derivedField : int
    val mutable private derivedFieldPrivate : int
    inherit Base
    new () = { derivedField = 0;derivedFieldPrivate = 0 }
let derived = Derived()
derived.{caret}derivedField""")>]
[<InlineData("""type Base =
   val mutable baseField : int
   val mutable private baseFieldPrivate : int
   new () = { baseField = 0; baseFieldPrivate=1 }
type Derived =
    val mutable baseField : int
    val mutable derivedField : int
    val mutable private derivedFieldPrivate : int
    inherit Base
    new () = { baseField = 0; derivedField = 0; derivedFieldPrivate = 0 }
let derived = Derived()
derived.{caret}derivedField""")>]
let ``ObjInstance.InheritedClass.MethodsWithDiffAccessibility`` (markedSource: string) =
    let info = Checker.getCompletionInfo markedSource
    assertHasItemWithNames [ "baseField"; "derivedField" ] info
    assertHasNoItemsWithNames [ "baseFieldPrivate"; "derivedFieldPrivate" ] info

[<Theory>]
[<InlineData("""type Base =
   val mutable baseField : int
   val mutable private baseFieldPrivate : int
   new () = { baseField = 0; baseFieldPrivate=1 }
type Derived =
    val mutable derivedField : int
    val mutable private derivedFieldPrivate : int
    inherit Base
    new () = { derivedField = 0;derivedFieldPrivate = 0 }
    member this.Method() =
        (*marker*)this.{caret}baseField""")>]
[<InlineData("""type Base =
   val mutable baseField : int
   val mutable private baseFieldPrivate : int
   new () = { baseField = 0; baseFieldPrivate=1 }
type Derived =
    val mutable baseField : int
    val mutable derivedField : int
    val mutable private derivedFieldPrivate : int
    inherit Base
    new () = { baseField = 0; derivedField = 0; derivedFieldPrivate = 0 }
    member this.Method() =
        (*marker*)this.{caret}baseField""")>]
let ``Visibility.InheritedClass.MethodsWithDiffAccessibility`` (markedSource: string) =
    let info = Checker.getCompletionInfo markedSource
    assertHasItemWithNames [ "baseField"; "derivedField"; "derivedFieldPrivate" ] info
    assertHasNoItemsWithNames [ "baseFieldPrivate" ] info

[<Fact>]
let ``Visibility.InheritedClass.MethodsWithSameNameMethod`` () =
    let info =
        Checker.getCompletionInfo
            """type MyClass =
    val foo : int
    new (foo) = { foo = foo }
type MyClass2 =
    inherit MyClass
    val foo : int
    new (foo) = {
        inherit MyClass(foo)
        foo = foo
        }
let x = new MyClass2(0)
(*marker*)x.{caret}foo"""

    assertHasItemWithNames [ "foo" ] info
