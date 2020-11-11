namespace global

[<assembly: System.Reflection.AssemblyCopyrightAttribute("hey!")>]
do ()

type TheGeneratedType1() = 
    member x.Prop1 = 1
    static member (+) (x1:TheGeneratedType1,x2:TheGeneratedType1) = x1
    static member (-) (x1:TheGeneratedType1,x2:TheGeneratedType1) = x2

type TheGeneratedType2() = 
    member x.Prop1 = 1
    static member (+) (x1:TheGeneratedType2,x2:TheGeneratedType2) = x1
    static member (-) (x1:TheGeneratedType2,x2:TheGeneratedType2) = x2


type TheGeneratedType3InContainerType1() = 
    member x.Prop1 = 1
    static member (+) (x1:TheGeneratedType3InContainerType1,x2:TheGeneratedType3InContainerType1) = x1
    static member (-) (x1:TheGeneratedType3InContainerType1,x2:TheGeneratedType3InContainerType1) = x2

type TheGeneratedType3InContainerType2() = 
    member x.Prop1 = 1
    static member (+) (x1:TheGeneratedType3InContainerType2,x2:TheGeneratedType3InContainerType2) = x1
    static member (-) (x1:TheGeneratedType3InContainerType2,x2:TheGeneratedType3InContainerType2) = x2

type TheGeneratedType4() = 
    member x.Prop1 = 1
    static member (+) (x1:TheGeneratedType4,x2:TheGeneratedType4) = x1
    static member (-) (x1:TheGeneratedType4,x2:TheGeneratedType4) = x2


type TheGeneratedType1WithIndexer() = 
    let dict = System.Collections.Generic.Dictionary<int,string>()
    [<DefaultValue>]
    val mutable MutableProperty1 : int
    member x.Prop2 = 1
    member x.Item 
        with get (v:int) = if dict.ContainsKey v then dict.[v] else string v 
        and set (idx:int) (v:string) = dict.[idx] <- v

type TheGeneratedType2WithIndexer() = 
    let dict = System.Collections.Generic.Dictionary<int,string>()
    [<DefaultValue>]
    val mutable MutableProperty1 : int
    member x.Prop2 = 1
    member x.Item 
        with get (v:int) = if dict.ContainsKey v then dict.[v] else string v 
        and set (idx:int) (v:string) = dict.[idx] <- v

type TheGeneratedType3WithIndexer() = 
    let dict = System.Collections.Generic.Dictionary<int,string>()
    [<DefaultValue>]
    val mutable MutableProperty1 : int
    member x.Prop2 = 1
    member x.Item 
        with get (v:int) = if dict.ContainsKey v then dict.[v] else string v 
        and set (idx:int) (v:string) = dict.[idx] <- v


type TheGeneratedType5() = 
    member x.Prop1 = 1
    static member (+) (x1:TheGeneratedType5,x2:TheGeneratedType5) = x1
    static member (-) (x1:TheGeneratedType5,x2:TheGeneratedType5) = x2


type TheGeneratedType5WithIndexer() = 
    let dict = System.Collections.Generic.Dictionary<int,string>()
    [<DefaultValue>]
    val mutable MutableProperty1 : int
    member x.Prop2 = 1
    member x.Item 
        with get (v:int) = if dict.ContainsKey v then dict.[v] else string v 
        and set (idx:int) (v:string) = dict.[idx] <- v

type TheGeneratedTypeWithEvent() = 
    let ev = new Event<System.EventHandler<System.EventArgs>, System.EventArgs >()
    member sender.Trigger () = ev.Trigger(sender,System.EventArgs())
    [<CLIEvent>]
    member __.MyEvent = ev.Publish


[<Struct>]
type TheGeneratedStructType(x:int) = 
    member __.StructProperty1 = x + 1
    [<DefaultValue>]
    val mutable StructMutableProperty1 : int
type TheGeneratedDelegateType = delegate of arg1:int -> int

type TheGeneratedEnumType = 
    | Item0 = 0
    | Item1 = 1
    | Item2 = 2

module GeneratedRelatedTypes = 

    type TheGeneratedInterfaceType = 
        abstract member InterfaceMethod0 : unit -> int
        abstract member InterfaceMethod1 : arg1:int -> int
        abstract member InterfaceMethod2 : arg1:int * arg2:int -> int
        abstract member InterfaceProperty1 : int

    type TheGeneratedInterfaceSubType = 
        inherit TheGeneratedInterfaceType
        abstract member InterfaceMethod3 : arg1:int * arg2:int * arg3:int -> int

    type TheGeneratedClassTypeWhichImplementsTheGeneratedInterfaceType() = 
        interface TheGeneratedInterfaceType with
            member __.InterfaceMethod0() = 3
            member __.InterfaceMethod1 arg1 = arg1 + 4
            member __.InterfaceMethod2 (arg1, arg2) = arg1+arg2
            member __.InterfaceProperty1 = 4

    type TheGeneratedClassTypeWhichImplementsTheGeneratedInterfaceSubType() = 
        interface TheGeneratedInterfaceSubType with
            member __.InterfaceMethod0() = 3
            member __.InterfaceMethod1 arg1 = arg1 + 4
            member __.InterfaceMethod2 (arg1, arg2) = arg1+arg2
            member __.InterfaceMethod3 (arg1, arg2, arg3) = arg1+arg2+arg3
            member __.InterfaceProperty1 = 4

    type TheGeneratedInterfaceTypeWithEvent = 
        abstract member InterfaceProperty1 : int
        [<CLIEvent>]
        abstract member MyEvent : IEvent<System.EventHandler<System.EventArgs>, System.EventArgs>

    [<AbstractClass>]
    type TheGeneratedAbstractClass() = 
        abstract member AbstractMethod0 : unit -> int
        abstract member AbstractMethod1 : arg1:int -> int
        abstract member AbstractMethod2 : arg1:int * arg2:int -> int
        abstract member AbstractProperty1 : int

    [<AbstractClass>]
    type TheGeneratedAbstractClassWithEvent() = 
        [<CLIEvent>]
        abstract member MyEvent : IEvent<System.EventHandler<System.EventArgs>, System.EventArgs>



module TheOuterType = 

    type TheNestedGeneratedType() = 
        member x.Prop1 = 1
        static member (+) (x1:TheNestedGeneratedType,x2:TheNestedGeneratedType) = x1
        static member (-) (x1:TheNestedGeneratedType,x2:TheNestedGeneratedType) = x2

    type TheNestedGeneratedType1() = 
        member x.Prop1 = 1

    type TheNestedGeneratedTypeWithIndexer() = 
        let dict = System.Collections.Generic.Dictionary<int,string>()
        member x.Prop2 = 1
        [<DefaultValue>]
        val mutable MutableProperty1 : int
        member x.Item 
            with get (v:int) = if dict.ContainsKey v then dict.[v] else string v 
            and set (idx:int) (v:string) = dict.[idx] <- v

    [<Struct>]
    type TheNestedGeneratedStructType(x:int) = 
        member __.StructProperty1 = x
        [<DefaultValue>]
        val mutable StructMutableProperty1 : int

    type TheNestedGeneratedInterfaceType = 
        abstract member InterfaceMethod0 : unit -> int
        abstract member InterfaceMethod1 : arg1:int -> int
        abstract member InterfaceMethod2 : arg1:int * arg2:int -> int
        abstract member InterfaceProperty1 : int

    type TheNestedGeneratedInterfaceTypeWithEvent = 
        abstract member InterfaceProperty1 : int
        [<CLIEvent>]
        abstract member MyEvent : IEvent<System.EventHandler<System.EventArgs>, System.EventArgs>

    type TheNestedGeneratedDelegateType = delegate of arg1:int -> int

    [<AbstractClass>]
    type TheNestedGeneratedAbstractClass() = 
        abstract member AbstractMethod0 : unit -> int
        abstract member AbstractMethod1 : arg1:int -> int
        abstract member AbstractMethod2 : arg1:int * arg2:int -> int
        abstract member AbstractProperty1 : int

    [<AbstractClass>]
    type TheNestedGeneratedAbstractClassWithEvent() = 
        abstract member AbstractProperty1 : int
        [<CLIEvent>]
        abstract member MyEvent : IEvent<System.EventHandler<System.EventArgs>, System.EventArgs>

    type TheNestedGeneratedEnumType = 
        | Item0 = 0
        | Item1 = 1
        | Item2 = 2

module DllImportSmokeTest = 
    [<System.Runtime.InteropServices.DllImport("kernel32.dll")>]
    extern System.UInt32 GetLastError()

