open Types

module CheckNewSyntax =

    type MyType() = 
        static member val StaticProperty = 0 with get, set
        static member StaticMethod0 () = 5
        static member StaticMethod1 x = x + 5
        static member StaticMethod2 (x, y) = x + y + 5
        member val Length = 0 with get, set
        member _.Item with get x = "Hello"
        member _.InstanceMethod0 () = 5
        member _.InstanceMethod1 x = x + 5
        member _.InstanceMethod2 (x, y) = x + y + 5

    // Check that "property" and "get_ method" constraints are considered logically equivalent
    let inline f_StaticProperty<'T when 'T : (static member StaticProperty: int) >() : int = 'T.StaticProperty

    let inline f_StaticMethod0<'T when 'T : (static member StaticMethod0: unit -> int) >() : int = 'T.StaticMethod0()

    let inline f_StaticMethod1<'T when 'T : (static member StaticMethod1: int -> int) >() : int = 'T.StaticMethod1(3)

    let inline f_StaticMethod2<'T when 'T : (static member StaticMethod2: int * int -> int) >() : int = 'T.StaticMethod2(3, 3)

    let inline f_set_StaticProperty<'T when 'T : (static member StaticProperty: int with set) >() = 'T.set_StaticProperty(3)

    let inline f_InstanceMethod0<'T when 'T : (member InstanceMethod0: unit -> int) >(x: 'T) : int = x.InstanceMethod0()

    let inline f_InstanceMethod1<'T when 'T : (member InstanceMethod1: int -> int) >(x: 'T) : int = x.InstanceMethod1(3)

    let inline f_InstanceMethod2<'T when 'T : (member InstanceMethod2: int * int -> int) >(x: 'T) : int = x.InstanceMethod2(3, 3)

    let inline f_Length<'T when 'T : (member Length: int) >(x: 'T) = x.Length

    let inline f_set_Length<'T when 'T : (member Length: int with set) >(x: 'T) = x.set_Length(3)

    let inline f_Item<'T when 'T : (member Item: int -> string with get) >(x: 'T) = x.get_Item(3)

    // Limitation by-design: As yet the syntax "'T.StaticProperty <- 3" can't be used
    // Limitation by-design: As yet the syntax "x.Length <- 3" can't be used
    // Limitation by-design: As yet the syntax "x[3]" can't be used, nor can any slicing syntax
    // Limitation by-design: The disposal pattern can't be used with "use"

    //let inline f_set_StaticProperty2<'T when 'T : (static member StaticProperty: int with set) >() = 'T.StaticProperty <- 3
    //let inline f_set_Length2<'T when 'T : (member Length: int with set) >(x: 'T) = x.Length <- 3
    //let inline f_Item2<'T when 'T : (member Item: int -> string with get) >(x: 'T) = x[3]
        
    if f_StaticMethod0<MyType>() <> 5 then
        failwith "Unexpected result"

    if f_StaticMethod1<MyType>() <> 8 then
        failwith "Unexpected result"

    if f_StaticMethod2<MyType>() <> 11 then
        failwith "Unexpected result"

    if f_set_StaticProperty<MyType>() <> () then
        failwith "Unexpected result"

    if f_StaticProperty<MyType>() <> 3 then
        failwith "Unexpected result"

    let myInstance = MyType()

    if f_Length(myInstance) <> 0 then
        failwith "Unexpected result"

    if f_InstanceMethod0(myInstance) <> 5 then
        failwith "Unexpected result"

    if f_InstanceMethod1(myInstance) <> 8 then
        failwith "Unexpected result"

    if f_InstanceMethod2(myInstance) <> 11 then
        failwith "Unexpected result"

    if f_set_Length(myInstance) <> () then
        failwith "Unexpected result"

    if f_Length(myInstance) <> 3 then
        failwith "Unexpected result"

    if f_Item(myInstance) <> "Hello" then
        failwith "Unexpected result"
