open Types

module CheckNewSyntax =

    type MyType() = 
        static member val StaticProperty = 0 with get, set
        static member StaticMethod x = x + 5
        member val Length = 0 with get, set
        member _.Item with get x = "Hello"
        member _.InstanceMethod x = x + 5

    // Check that "property" and "get_ method" constraints are considered logically equivalent
    let inline f_StaticProperty<'T when 'T : (static member StaticProperty: int) >() : int = 'T.StaticProperty

    let inline f_StaticMethod<'T when 'T : (static member StaticMethod: int -> int) >() : int = 'T.StaticMethod(3)

    let inline f_set_StaticProperty<'T when 'T : (static member StaticProperty: int with set) >() = 'T.set_StaticProperty(3)

    let inline f_InstanceMethod<'T when 'T : (member InstanceMethod: int -> int) >(x: 'T) : int = x.InstanceMethod(3)

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
        
    if f_StaticMethod<MyType>() <> 8 then
        failwith "Unexpected result"

    if f_set_StaticProperty<MyType>() <> () then
        failwith "Unexpected result"

    if f_StaticProperty<MyType>() <> 3 then
        failwith "Unexpected result"

    let myInstance = MyType()

    if f_Length(myInstance) <> 0 then
        failwith "Unexpected result"

// TODO: fix these
//    if f_InstanceMethod(myInstance) <> 8 then
//        failwith "Unexpected result"
//
//    if f_set_Length(myInstance) <> () then
//        failwith "Unexpected result"
//
//    if f_Length(myInstance) <> 3 then
//        failwith "Unexpected result"
//
//    if f_Item(myInstance) <> "Hello" then
//        failwith "Unexpected result"
