module FSharp.Compiler.Service.Tests.CompletionPropertiesTests

open FSharp.Test
open Xunit

[<Fact>]
let ``ObsoleteProperties.6377_1`` () =
    let info =
        Checker.getCompletionInfo
            """type StandIn() =
    [<System.Obsolete>]
    static member val SecurityEnabled = false with get, set
    static member GetStandardSandbox() = 0
StandIn.{caret}"""

    assertHasItemWithNames [ "GetStandardSandbox" ] info
    assertHasNoItemsWithNames [ "get_SecurityEnabled"; "set_SecurityEnabled" ] info

[<Fact>]
let ``ObsoleteProperties.6377_2`` () =
    let info = Checker.getCompletionInfo "System.Threading.Thread.CurrentThread.{caret}"

    assertHasItemWithNames [ "CurrentCulture" ] info
    assertHasNoItemsWithNames [ "get_ApartmentState"; "set_ApartmentState" ] info

[<Fact>]
let ``Class.Property.Bug69150_A`` () =
    let info =
        Checker.getCompletionInfo
            """type ClassType(x : int) =
    member this.Value = x
let z = (new ClassType(23)).{caret}Value"""

    assertHasItemWithNames [ "Value" ] info
    assertHasNoItemsWithNames [ "CompareTo" ] info

[<Fact>]
let ``Class.Property.Bug69150_B`` () =
    let info =
        Checker.getCompletionInfo
            """type ClassType(x : int) =
    member this.Value = x
let z = ClassType(23).{caret}Value"""

    assertHasItemWithNames [ "Value" ] info
    assertHasNoItemsWithNames [ "CompareTo" ] info

[<Fact>]
let ``Class.Property.Bug69150_C`` () =
    let info =
        Checker.getCompletionInfo
            """type ClassType(x : int) =
    member this.Value = x
let f x = new ClassType(x)
let z = f(23).{caret}Value"""

    assertHasItemWithNames [ "Value" ] info
    assertHasNoItemsWithNames [ "CompareTo" ] info

[<Fact>]
let ``Class.Property.Bug69150_D`` () =
    let info =
        Checker.getCompletionInfo
            """type ClassType(x : int) =
    member this.Value = x
let z = ClassType(23).V{caret}alue"""

    assertHasItemWithNames [ "Value" ] info
    assertHasNoItemsWithNames [ "VolatileFieldAttribute" ] info

[<Fact>]
let ``Class.Property.Bug69150_E`` () =
    let info =
        Checker.getCompletionInfo
            """type ClassType(x : int) =
    member this.Value = x
let z = ClassType(23)   . {caret}  Value"""

    assertHasItemWithNames [ "Value" ] info
    assertHasNoItemsWithNames [ "VolatileFieldAttribute" ] info

[<Fact>]
let ``AssignmentToProperty.Bug231283`` () =
    let info =
        Checker.getCompletionInfo
            """
                type Foo() =
                    member val Bar = 0 with get,set
                let f = new Foo()
                f.Bar <-
                    let xyz = 42 {caret}(*Mark*)
                    xyz """

    assertHasItemWithNames [ "AbstractClassAttribute" ] info
    assertHasNoItemsWithNames [ "Bar" ] info

[<Fact>]
let ``Bug130733.LongIdSet.CtrlSpace`` () =
    let info =
        Checker.getCompletionInfo
            """
            type C() =
                member this.XX with get() = 4 and set(x) = ()
            let c = C()
            c.X{caret} <- 42"""

    assertHasItemWithNames [ "XX" ] info

[<Fact>]
let ``Bug130733.LongIdSet.Dot`` () =
    let info =
        Checker.getCompletionInfo
            """
            type C() =
                member this.XX with get() = 4 and set(x) = ()
            let c = C()
            c.{caret}X <- 42"""

    assertHasItemWithNames [ "XX" ] info

[<Fact>]
let ``Bug130733.ExprDotSet.CtrlSpace`` () =
    let info =
        Checker.getCompletionInfo
            """
            type C() =
                member this.XX with get() = 4 and set(x) = ()
            let f(x) = C()
            f(0).X{caret} <- 42"""

    assertHasItemWithNames [ "XX" ] info

[<Fact>]
let ``Bug130733.ExprDotSet.Dot`` () =
    let info =
        Checker.getCompletionInfo
            """
            type C() =
                member this.XX with get() = 4 and set(x) = ()
            let f(x) = C()
            f(0).{caret}X <- 42"""

    assertHasItemWithNames [ "XX" ] info

[<Fact>]
let ``Bug130733.Nested.LongIdSet.CtrlSpace`` () =
    let info =
        Checker.getCompletionInfo
            """
            type C() =
                member this.XX with get() = 4 and set(x) = ()
                member this.CC with get() = C()
            let c = C()
            c.CC.X{caret} <- 42"""

    assertHasItemWithNames [ "XX" ] info

[<Fact>]
let ``Bug130733.Nested.LongIdSet.Dot`` () =
    let info =
        Checker.getCompletionInfo
            """
            type C() =
                member this.XX with get() = 4 and set(x) = ()
                member this.CC with get() = C()
            let c = C()
            c.CC.{caret}X <- 42"""

    assertHasItemWithNames [ "XX" ] info

[<Fact>]
let ``Bug130733.Nested.ExprDotSet.CtrlSpace`` () =
    let info =
        Checker.getCompletionInfo
            """
            type C() =
                member this.XX with get() = 4 and set(x) = ()
                member this.CC with get() = C()
            let f(x) = C()
            f(0).CC.X{caret} <- 42"""

    assertHasItemWithNames [ "XX" ] info

[<Fact>]
let ``Bug130733.Nested.ExprDotSet.Dot`` () =
    let info =
        Checker.getCompletionInfo
            """
            type C() =
                member this.XX with get() = 4 and set(x) = ()
                member this.CC with get() = C()
            let f(x) = C()
            f(0).CC.{caret}X <- 42"""

    assertHasItemWithNames [ "XX" ] info

[<Fact>]
let ``Bug130733.NamedIndexedPropertyGet.Dot`` () =
    let info =
        Checker.getCompletionInfo
            """
            let str = "foo"
            str.Chars(3).{caret}"""

    assertHasItemWithNames [ "CompareTo" ] info

[<Fact>]
let ``Bug130733.NamedIndexedPropertyGet.CtrlSpace`` () =
    let info =
        Checker.getCompletionInfo
            """
            let str = "foo"
            str.Chars(3).Co{caret}"""

    assertHasItemWithNames [ "CompareTo" ] info

[<Theory>]
[<InlineData("""
            type Foo() =
                member x.MutableInstanceIndexer
                    with get (i) = 0
                    and  set (i) (v:string) = ()
            let h() = new Foo()
            (h()).Muta{caret}bleInstanceIndexer(0) <- "foo" """)>]
[<InlineData("""
            type Foo() =
                member x.MutableInstanceIndexer
                    with get (i) = 0
                    and  set (i) (v:string) = ()
            type Bar() =
                member this.ZZZ = new Foo()
            let g() = new Bar()
            (g()).ZZZ.Muta{caret}bleInstanceIndexer(0) <- "blah"  """)>]
let ``Bug230533.NamedIndexedPropertySet.CtrlSpace`` (source: string) =
    let info = Checker.getCompletionInfo source
    assertHasItemWithNames [ "MutableInstanceIndexer" ] info

[<Fact>]
let ``Bug230533.ExprDotSet.CtrlSpace.Case1`` () =
    let info =
        Checker.getCompletionInfo
            """
            type C() =
                member this.XX with get() = 4 and set(x) = ()
            type D() =
                member this.CC = new C()
            let f(x) = D()
            f(0).CC.{caret}     <- 42 """

    assertHasItemWithNames [ "XX" ] info

[<Fact>]
let ``Bug230533.ExprDotSet.CtrlSpace.Case2`` () =
    let info =
        Checker.getCompletionInfo
            """
            type C() =
                member this.XX with get() = 4 and set(x) = ()
            type D() =
                member this.CC with get() = new C() and set(x) = ()
            let f(x) = D()
            f(0).CC.{caret}     <- 42 """

    assertHasItemWithNames [ "XX" ] info

[<Fact>]
let ``ExpressionDotting.Regression.Bug187799`` () =
    let info =
        Checker.getCompletionInfo
            """
                type T() =
                    member _.P with get() = new T()
                    member _.M() = [|1..2|]
                let t = new T()
                t.P.M().{caret}  """

    assertHasItemWithNames [ "Clone" ] info

[<Fact>]
let ``ExpressionDotting.Regression.Bug187799.Test8`` () =
    let info =
        Checker.getCompletionInfo
            """
                type C() =
                    static member XXX with get() = 4 and set(x) = ()
                    static member CCC with get() = C()
                C.XXX.{caret} <- 42"""

    assertHasItemWithNames [ "CompareTo" ] info

[<Fact>]
let ``NoInfiniteLoopInProperties`` () =
    let info =
        Checker.getCompletionInfo
            """
                type NodeCollection() =
                    member _.Add(n: Node) = ()
                    member _.Item with get (index: int) = Node()
                and Node() =
                    member _.Nodes = NodeCollection()
                let tn = Node()
                tn.Nodes.{caret}"""

    assertHasNoItemsWithNames [ "Nodes" ] info

[<Fact>]
let ``Identifier.AsProperty`` () =
    let info =
        Checker.getCompletionInfo
            """
                type Type2 =
                    member this.Foo.{caret} = 1"""

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``ExpressionPropertyAssignment.Bug217051`` () =
    let info =
        Checker.getCompletionInfo
            """
                type Foo() =
                    member val Prop = 0 with get, set
                Foo().{caret} <- 4 """

    assertHasItemWithNames [ "Prop" ] info

[<FactForDESKTOP>]
let ``ExpressionProperty.Bug234687`` () =
    let info =
        Checker.getCompletionInfo
            """
                open System.Reflection
                let x = obj()
                let a = x.GetType().Assembly.{caret}
            """

    assertHasItemWithNames [ "CodeBase" ] info
