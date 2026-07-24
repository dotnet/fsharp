module FSharp.Compiler.Service.Tests.TooltipTypesTests

open System
open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Symbols
open FSharp.Compiler.Tokenization

[<Fact>]
let ``NestedTypesOrder`` () =
    assertTooltipContainsInOrder
        [ "GetHashCode"; "GetObjectValue" ]
        (markAtStartOfMarker "type t = System.Runtime.CompilerServices.RuntimeHelpers(*M*)" "(*M*)")

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``QuickInfo.HideBaseClassMembersTP`` () =
    assertTooltipContains
        "type HiddenBaseMembersTP =\n  inherit TPBaseTy"
        (markAtStartOfMarker "type foo = HiddenMembersInBaseClass.HiddenBaseMembersTP(*Marker*)" "MembersTP(*Marker*)")

[<Fact>]
let ``QuickInfo.OverridenMethods`` () =
    let source =
        """
type A() =
    abstract member M: unit -> unit
    /// 1234
    default this.M() = ()

type AA() =
    inherit A()
    /// 5678
    override this.M() = ()
let x = new AA()
x.M()

let y = new A()
y.M()
"""

    assertTooltipContains "5678" (markAtEndOfMarker source "x.M")
    assertTooltipContains "1234" (markAtEndOfMarker source "y.M")

[<Fact>]
let ``QuickInfoForTypesWithHiddenRepresentation`` () =
    let signatureListing =
        "type Async =\n  static member AsBeginEnd: computation: ('Arg -> Async<'T>) -> ('Arg * AsyncCallback * objnull -> IAsyncResult) * (IAsyncResult -> 'T) * (IAsyncResult -> unit)\n  static member AwaitEvent: event: IEvent<'Del,'T> * ?cancelAction: (unit -> unit) -> Async<'T> (requires delegate and 'Del :> Delegate and 'Del: not null)\n  static member AwaitIAsyncResult: iar: IAsyncResult * ?millisecondsTimeout: int -> Async<bool>\n  static member AwaitTask: task: Task<'T> -> Async<'T> + 1 overload\n  static member AwaitWaitHandle: waitHandle: WaitHandle * ?millisecondsTimeout: int -> Async<bool>\n  static member CancelDefaultToken: unit -> unit\n  static member Catch: computation: Async<'T> -> Async<Choice<'T,exn>>\n  static member Choice: computations: Async<'T option> seq -> Async<'T option>\n  static member FromBeginEnd: beginAction: (AsyncCallback * objnull -> IAsyncResult) * endAction: (IAsyncResult -> 'T) * ?cancelAction: (unit -> unit) -> Async<'T> + 3 overloads\n  static member FromContinuations: callback: (('T -> unit) * (exn -> unit) * (OperationCanceledException -> unit) -> unit) -> Async<'T>\n  ..."

    assertTooltipContainsInOrder
        [ signatureListing; "Full name: Microsoft.FSharp.Control.Async" ]
        (markAtEndOfMarker "let x = Async.AsBeginEnd\n1" "Asyn")

[<Fact>]
let ``GetterSetterInsideInterfaceImpl.ThisOnceAsserted`` () =
    assertTooltipContains
        "Operators.id"
        """
type IFoo =
    abstract member X: int with get,set

type Bar =
    interface IFoo with
        member this.X
            with get() = 42  // hello
            and set(v) = id{caret}() """

[<Fact>]
let ``Regression.FieldRepeatedInToolTip.Bug3538`` () =
    assertIdentifierInTooltipExactlyOnce
        "Explicit"
        """
open System.Runtime.InteropServices
[<StructLayout(LayoutKind.Expl{caret}icit)>]
type A() =
    [<DefaultValue>]
    val mutable x : int"""

[<Fact>]
let ``Regression.MemberDefinition.DocComments.Bug5856_1`` () =
    assertCompletionItemTooltipContainsInOrder
        "Overload"
        [ "static member MyType.Overload: unit -> int"
          "static member MyType.Overload: x: int -> int"
          "Hello" ]
        """type MyType =
    /// Hello
    static member Overload() = 0
    /// Hello2
    static member Overload(x:int) = 0
    /// Hello3
    static member NonOverload() = 0
let x() = MyType.{caret}"""

[<Fact>]
let ``Regression.Class.Printing.FSharp.Classes.Bug4624`` () =
    let source =
        """type F1() =
    class
        inherit System.Windows.Forms.Form()
        abstract AAA : int  with get
        abstract ZZZ : int  with get
        abstract AAA : bool with set
        val x : F1
        static val x : F1
        static member A() = 12
        member this.B() = 12
        static member C() = 12
        member this.D() = 12
        member this.D with get() = 12 and set(12) = ()
        member this.D(x:int,y:int) = 12
        member this.D(x:int) = 12
        member this.D x y z = [1;x;y;z]
        override this.ToString() = ""
        interface System.IDisposable with
            override this.Dispose() = ()
        end
    end
type A1 = F1"""

    assertTooltipContainsInOrder
        [ "type F1 ="
#if !NETCOREAPP
          "  inherit Form"
#endif
          "  interface IDisposable"
          "  new: unit -> F1"
          "  val x: F1"
          "  member B: unit -> int"
          "  override ToString: unit -> string"
          "  static member A: unit -> int"
          "  static member C: unit -> int"
          "  abstract AAA: int"
          "  member D: int"
          "  ..." ]
        (markAtEndOfMarker source "type A1 = F1")

[<Fact>]
let ``Automation.Regression.BeforeAndAfterIdentifier.Bug4371`` () =
    let baseSrc =
        """module Test
let f arg1 (arg2, arg3, arg4) arg5 = 42
let goo a = f 12 a

type printer = System.Console
let z = printer.BufferWidth"""

    let fSrc = baseSrc.Replace("let goo a = f 12 a", "let goo a = f{caret} 12 a")
    assertTooltipContains "Full name: Test.f" fSrc
    assertTooltipContains "val f" fSrc

    assertTooltipContains
        "property System.Console.BufferWidth: int"
        (baseSrc.Replace("let z = printer.BufferWidth", "let z = printer.BufferWidth{caret}"))

    assertTooltipContains
        "Full name: Test.printer"
        (baseSrc.Replace("let z = printer.BufferWidth", "let z = printer{caret}.BufferWidth"))

[<Fact>]
let ``Automation.Regression.ConstructorWithSameNameAsType.Bug2739`` () =
    let source =
        """namespace AA
module AA =
    type AA = | AA(*Marker1*) = 1
              | BB = 2
type BB = { BB(*Marker2*) : string; }"""

    assertTooltipContainsInFsFile "AA.AA: AA" (markAtStartOfMarker source "(*Marker1*)")
    assertTooltipContainsInFsFile "BB.BB: string" (markAtStartOfMarker source "(*Marker2*)")

[<Fact>]
let ``Automation.Regression.EventImplementation.Bug5471`` () =
    let source =
        """namespace regressiontest
open System.ComponentModel

type CommandReference() =
    let evt = Event<PropertyChangedEventHandler, PropertyChangedEventArgs>()

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member x.PropertyChanged(*Marker*) = evt.Publish"""

    let marked = markAtStartOfMarker source "(*Marker*)"
    assertTooltipContainsInFsFile "override CommandReference.PropertyChanged: IEvent<PropertyChangedEventHandler,PropertyChangedEventArgs>" marked
    assertTooltipContainsInFsFile "regressiontest.CommandReference.PropertyChanged" marked

[<Fact>]
let ``Automation.ExtensionMethod`` () =
    let source =
        """namespace TestQuickinfo

module BCLExtensions =
    type System.Random with
        /// BCL class Extension method
        member this.NextDice()  = this.Next() + 1
        /// new BCL class Extension method with overload
        member this.NextDice(a : bool)  = this.Next() + 1
        /// existing BCL class Extension method with overload
        member this.Next(a : bool)  = this.Next() + 1
        /// BCL class Extension property
        member this.DiceValue with get() = 6

    type System.ConsoleKeyInfo with
        /// BCL struct extension method
        member this.ExtensionMethod()  =  100
        /// BCL struct extension property
        member this.ExtensionProperty with get() = "Foo"

module OwnCode =
    /// fs class
    type FSClass() =
        class
            /// fs class method original
            member this.Method(a:string) = ""
            /// fs class property original
            member this.Prop with get(a:string) = ""
        end

    /// fs struct
    type FSStruct(x:int) =
        struct
        end

module OwnCodeExtensions =
    type OwnCode.FSClass with
        /// fs class extension method
        member this.ExtensionMethod()  =  100
        /// fs class extension property
        member this.ExtensionProperty with get() = "Foo"
        /// fs class method extension overload
        member this.Method(a:int)  =  ""
        /// fs class property extension overload
        member this.Prop with get(a:int)  =  ""

    type OwnCode.FSStruct with
        /// fs struct extension method
        member this.ExtensionMethod()  =  100
        /// fs struct extension property
        member this.ExtensionProperty with get() = "Foo"

module BCLClass =
    open BCLExtensions
    let rnd = new System.Random()
    rnd.DiceValue(*Marker11*) |>ignore
    rnd.NextDice(*Marker12*)() |>ignore
    rnd.NextDice(*Marker13*)(true) |>ignore
    rnd.Next(*Marker14*)(true) |>ignore

module BCLStruct =
    open BCLExtensions
    let cki = new System.ConsoleKeyInfo()
    cki.ExtensionMethod(*Marker21*) |>ignore
    cki.ExtensionProperty(*Marker22*) |>ignore

module OwnClass =
    open OwnCode
    open OwnCodeExtensions
    let rnd = new FSClass()
    rnd.ExtensionMethod(*Marker31*) |>ignore
    rnd.ExtensionProperty(*Marker32*) |>ignore
    rnd.Method(*Marker33*)("") |>ignore
    rnd.Method(*Marker34*)(6) |>ignore
    rnd.Prop(*Marker35*)("") |>ignore
    rnd.Prop(*Marker36*)(6) |>ignore

module OwnStruct =
    open OwnCode
    open OwnCodeExtensions
    let cki = new FSStruct(100)
    cki.ExtensionMethod(*Marker41*) |>ignore
    cki.ExtensionProperty(*Marker42*) |>ignore"""

    let assertAt marker sig' doc =
        let marked = markAtStartOfMarker source marker
        assertTooltipContainsInFsFile sig' marked
        assertTooltipContainsInFsFile doc marked

    assertAt "(*Marker11*)" "property System.Random.DiceValue: int" "BCL class Extension property"
    assertAt "(*Marker12*)" "member System.Random.NextDice: unit -> int" "BCL class Extension method"
    assertAt "(*Marker13*)" "member System.Random.NextDice: a: bool -> int" "new BCL class Extension method with overload"
    assertAt "(*Marker14*)" "member System.Random.Next: a: bool -> int" "existing BCL class Extension method with overload"
    assertAt "(*Marker21*)" "member System.ConsoleKeyInfo.ExtensionMethod: unit -> int" "BCL struct extension method"
    assertAt "(*Marker22*)" "System.ConsoleKeyInfo.ExtensionProperty: string" "BCL struct extension property"
    assertAt "(*Marker31*)" "member FSClass.ExtensionMethod: unit -> int" "fs class extension method"
    assertAt "(*Marker32*)" "FSClass.ExtensionProperty: string" "fs class extension property"
    assertAt "(*Marker33*)" "member FSClass.Method: a: string -> string" "fs class method original"
    assertAt "(*Marker34*)" "member FSClass.Method: a: int -> string" "fs class method extension overload"
    assertAt "(*Marker35*)" "property FSClass.Prop: string -> string" "fs class property original"
    assertAt "(*Marker36*)" "property FSClass.Prop: int -> string" "fs class property extension overload"
    assertAt "(*Marker41*)" "member FSStruct.ExtensionMethod: unit -> int" "fs struct extension method"
    assertAt "(*Marker42*)" "FSStruct.ExtensionProperty: string" "fs struct extension property"

[<Fact>]
let ``Automation.Regression.GenericFunction.Bug2868`` () =
    let marked =
        markAtStartOfMarker
            """module Test
let F (f :_ -> float<_>) = fun x -> f (x+1.0)
let rec Gen<[<Measure>] 'u> (f:float<'u> -> float<'u>) =
  Gen(*Marker*)(F f)"""
            "(*Marker*)"

    assertTooltipContains "val Gen: f: (float -> float) -> 'a" marked
    assertTooltipDoesNotContain "Exception" marked
    assertTooltipDoesNotContain "thrown" marked

[<Fact>]
let ``Automation.Regression.NamesArgument.Bug3818`` () =
    assertTooltipContains
        "property System.AttributeUsageAttribute.AllowMultiple: bool"
        (markAtStartOfMarker
            """module m
[<System.AttributeUsage(System.AttributeTargets.All, AllowMultiple(*Marker1*) = true)>]
type T = class
         end"""
            "(*Marker1*)")

[<Fact(Skip = "DocComment issue")>]
let ``Automation.OnUnitsOfMeasure`` () =
    let source =
        """namespace TestQuickinfo

module TestCase1 =
    [<Measure>]
    /// this type represents kilogram in UOM
    type kg
    let mass(*Marker11*) = 2.0<kg(*Marker12*)>

module TestCase2 =
    [<Measure>]
    /// use Set as the type name of UoM
    type Set

    let v1 = [1.0<Set> .. 2.0<Set> .. 5.0<Set>] |> Seq.item 1

    (if v1 = 3.0<Set> then 0 else 1) |> ignore

    let twoSets = 2.0<Set>

    [1.0<Set(*Marker21*)>]
    |> Set.ofList
    |> Set(*Marker22*).isEmpty
    |> ignore"""

    assertTooltipContainsInFsFile "val mass: float<kg>" (markAtStartOfMarker source "(*Marker11*)")
    assertTooltipContainsInFsFile "Full name: TestQuickinfo.TestCase1.mass" (markAtStartOfMarker source "(*Marker11*)")
    assertTooltipContainsInFsFile "inherits: System.ValueType" (markAtStartOfMarker source "(*Marker11*)")
    assertTooltipContainsInFsFile "[<Measure>]" (markAtStartOfMarker source "(*Marker12*)")
    assertTooltipContainsInFsFile "type kg" (markAtStartOfMarker source "(*Marker12*)")
    assertTooltipContainsInFsFile "this type represents kilogram in UOM" (markAtStartOfMarker source "(*Marker12*)")
    assertTooltipContainsInFsFile "Full name: TestQuickinfo.TestCase1.kg" (markAtStartOfMarker source "(*Marker12*)")
    assertTooltipContainsInFsFile "[<Measure>]" (markAtStartOfMarker source "(*Marker21*)")
    assertTooltipContainsInFsFile "type Set" (markAtStartOfMarker source "(*Marker21*)")
    assertTooltipContainsInFsFile "use Set as the type name of UoM" (markAtStartOfMarker source "(*Marker21*)")
    assertTooltipContainsInFsFile "Full name: TestQuickinfo.TestCase2.Set" (markAtStartOfMarker source "(*Marker21*)")
    assertTooltipContainsInFsFile "module Set" (markAtStartOfMarker source "(*Marker22*)")
    assertTooltipContainsInFsFile "from Microsoft.FSharp.Collections" (markAtStartOfMarker source "(*Marker22*)")
    assertTooltipContainsInFsFile "Functional programming operators related to the Set<_> type." (markAtStartOfMarker source "(*Marker22*)")

[<Fact(Skip = "Cannot get QuickInfo tips")>]
let ``Automation.Setter`` () =
    let source =
        """type T() =
     member this.XX
       with set ((a:int), (b:int), (c:int)) = ()

(new T()).XX(*Marker1*) <- (1,2,3)

type IFoo = interface
    abstract foo : int -> int
    end
let i : IFoo = Unchecked.defaultof<IFoo>
i.foo(*Marker2*) |> ignore

type Rec =  { bar:int->int->int }
let r = {bar = fun x y -> x + y }

r.bar(*Marker3*) 1 2 |>ignore

type M() =
    member this.baz x y = x + y
let m = new M()
m.baz(*Marker3*) 1 2 |>ignore

type T2() =
     member this.Foo(a,b) = ""
let t = new T2()
t.Foo(*Marker4*)(1,2) |>ignore

let foo (x:int) (y:int) : int = 1
foo(*Marker5*) 2 3 |> ignore"""

    assertTooltipContains "T.XX: int * int * int" (markAtStartOfMarker source "(*Marker1*)")
    assertTooltipDoesNotContain "->" (markAtStartOfMarker source "(*Marker1*)")
    assertTooltipContains "IFoo.foo: int -> int" (markAtStartOfMarker source "(*Marker2*)")
    assertTooltipContains "Rec.bar: int -> int -> int" (markAtStartOfMarker source "(*Marker3*)")
    assertTooltipContains "T2.Foo: a: 'a * b: 'b -> string" (markAtStartOfMarker source "(*Marker4*)")
    assertTooltipContains "val foo: int -> int -> int" (markAtStartOfMarker source "(*Marker5*)")

[<Fact>]
let ``Automation.Regression.TypeInferenceScenarios.Bug2362_3538`` () =
    let source =
        """module Test.Module1

open System
open System.Diagnostics
open System.Runtime.InteropServices

#nowarn "9"

let append m(*Marker1*) n(*Marker2*) = fun ac(*Marker3*) -> m (n ac)

type Foo() as this(*Marker4*) =
    do this(*Marker5*) |> ignore
    member this.Bar() =
        this(*Marker6*) |> ignore
        ()

[<StructLayout(LayoutKind.Explicit)>]
type A =
    [<DefaultValue>]
    val mutable x : int
    new () = { }
    member this.Prop = this.x

let x = new (*Marker7*)A()"""

    assertTooltipContains "val m: ('a -> 'b)" (markAtStartOfMarker source "(*Marker1*)")
    assertTooltipContains "val n: ('c -> 'a)" (markAtStartOfMarker source "(*Marker2*)")
    assertTooltipContains "val ac: 'c" (markAtStartOfMarker source "(*Marker3*)")
    assertTooltipContains "val this: Foo" (markAtStartOfMarker source "(*Marker4*)")
    assertTooltipContains "val this: Foo" (markAtStartOfMarker source "(*Marker5*)")
    assertTooltipContains "val this: Foo" (markAtStartOfMarker source "(*Marker6*)")

    let mSrc7 = source.Replace("new (*Marker7*)A()", "new A{caret}()")
    assertTooltipContains "type A =" mSrc7
    assertTooltipContains "val mutable x: int" mSrc7

[<Fact>]
let ``Automation.Regression.XmlDocCommentsOnExtensionMembers.Bug138112`` () =
    let source =
        """module Module1 =
    type T() =
        /// XmlComment M1
        member this.M1() = ()
    type T with
        /// XmlComment M2
        member this.M2() = ()
    module public Extension =
        type T with
            /// XmlComment M3
            member this.M3() = ()
open Module1
open Extension

let x1 = T().M1(*Marker1*)()
let x2 = T().M2(*Marker2*)()
let x3 = T().M3(*Marker3*)()"""

    assertTooltipContains "XmlComment M1" (markAtStartOfMarker source "(*Marker1*)")
    assertTooltipContains "XmlComment M2" (markAtStartOfMarker source "(*Marker2*)")
    assertTooltipContains "XmlComment M3" (markAtStartOfMarker source "(*Marker3*)")
