open System

let legacyConcat1 (x: string) (y: string) = x ^ y
let legacyConcat2 (x: string) (y: string) = x ^y
let legacyConcat3 (x: string) (y: string) = x^ y
let legacyConcat4 (x: string) (y: string) = x^y

let testSlicingOne() =
    let arr = [| 1;2;3;4;5 |]
    arr.[^3..]

let testSlicingTwo() =
    let arr = [| 1;2;3;4;5 |]
    arr[^3..]

type IStaticProperty<'T when 'T :> IStaticProperty<'T>> =
    static abstract StaticProperty: 'T

type IStaticMethod<'T when 'T :> IStaticMethod<'T>> =
    static abstract StaticMethod: 'T -> 'T

type IUnitMethod<'T when 'T :> IUnitMethod<'T>> =
    static abstract UnitMethod: unit -> unit

type IAdditionOperator<'T when 'T :> IAdditionOperator<'T>> =
    static abstract op_Addition: 'T * 'T -> 'T

type C(c: int) =
    member _.Value = c
    interface IAdditionOperator<C> with
        static member op_Addition(x, y) = C(x.Value + y.Value) 
    interface IStaticProperty<C> with
        static member StaticProperty = C(7)
    interface IStaticMethod<C> with
        static member StaticMethod(x) = C(x.Value + 4)
    interface IUnitMethod<C> with
        static member UnitMethod() = ()

let f_IWSAM_explicit_operator_name<'T when 'T :> IAdditionOperator<'T>>(x: 'T, y: 'T) =
    'T.op_Addition(x, y)

let f_IWSAM_pretty_operator_name<'T when 'T :> IAdditionOperator<'T>>(x: 'T, y: 'T) =
    'T.(+)(x, y)

let f_IWSAM_StaticProperty<'T when 'T :> IStaticProperty<'T>>() =
    'T.StaticProperty

let f_IWSAM_declared_StaticMethod<'T when 'T :> IStaticMethod<'T>>(x: 'T) =
    'T.StaticMethod(x)

let f_IWSAM_declared_UnitMethod<'T when 'T :> IUnitMethod<'T>>() =
    'T.UnitMethod()

let f_IWSAM_declared_UnitMethod_list<'T when 'T :> IUnitMethod<'T>>() =
    let v = 'T.UnitMethod()
    [ v ]

let f_IWSAM_flex_StaticProperty(x: #IStaticProperty<'T>) =
    'T.StaticProperty

let f_IWSAM_flex_StaticMethod(x: #IStaticMethod<'T>) =
    'T.StaticMethod(x)


let inline f3<^T when ^T :> IAdditionOperator<^T>>(x: ^T, y: ^T) =
    'T.op_Addition(x,y)

let inline f4<^T when ^T : (static member (+): ^T * ^T -> ^T)>(x: ^T, y: ^T) =
    'T.op_Addition(x,y)

let inline f5<^T when ^T : (static member (+): ^T * ^T -> ^T)>(x: ^T, y: ^T) =
    'T.(+)(x,y)

let inline f6<^T when ^T : (static member (+): ^T * ^T -> ^T)>(x: ^T, y: ^T) =
    x + y

let inline f_StaticProperty_IWSAM<'T when 'T :> IStaticProperty<'T>>() =
    'T.StaticProperty

let inline f_StaticProperty_SRTP<^T when ^T : (static member StaticProperty: ^T) >() =
    'T.StaticProperty

let inline f_StaticProperty_BOTH<^T when ^T :> IStaticProperty<^T> and ^T : (static member StaticProperty: ^T) >() =
    'T.StaticProperty

module CheckExecution =
    if f_IWSAM_explicit_operator_name<C>(C(3), C(4)).Value <> 7 then
        failwith "incorrect value"

    if f_IWSAM_pretty_operator_name<C>(C(3), C(4)).Value <> 7 then
        failwith "incorrect value"

    if f_IWSAM_StaticProperty<C>().Value <> 7 then
        failwith "incorrect value"

module EquivalenceOfPropertiesAndGetters =
    // Check that "property" and "get_ method" constraints are considered logically equivalent
    let inline f_StaticProperty<^T when ^T : (static member StaticProperty: int) >() = (^T : (static member StaticProperty: int) ())
    let inline f_StaticProperty_explicit<^T when ^T : (static member get_StaticProperty: unit -> int) >() = (^T : (static member get_StaticProperty: unit -> int) ())
    let inline f_StaticProperty_mixed<^T when ^T : (static member get_StaticProperty: unit -> int) >() = (^T : (static member StaticProperty: int) ())
    let inline f_StaticProperty_mixed2<^T when ^T : (static member StaticProperty: int) >() = (^T : (static member get_StaticProperty: unit -> int) ())

    let inline f_set_StaticProperty<^T when ^T : (static member StaticProperty: int with set) >() = (^T : (static member StaticProperty: int with set) (3))
    let inline f_set_StaticProperty_explicit<^T when ^T : (static member set_StaticProperty: int -> unit) >() = (^T : (static member set_StaticProperty: int -> unit) (3))
    let inline f_set_StaticProperty_mixed<^T when ^T : (static member set_StaticProperty: int -> unit) >() = (^T : (static member StaticProperty: int with set) (3))
    let inline f_set_StaticProperty_mixed2<^T when ^T : (static member StaticProperty: int with set) >() = (^T : (static member set_StaticProperty: int -> unit) (3))

    let inline f_Length<^T when ^T : (member Length: int) >(x: ^T) = (^T : (member Length: int) (x))
    let inline f_Length_explicit<^T when ^T : (member get_Length: unit -> int) >(x: ^T) = (^T : (member get_Length: unit -> int) (x))
    let inline f_Length_mixed<^T when ^T : (member get_Length: unit -> int) >(x: ^T) = (^T : (member Length: int) (x))
    let inline f_Length_mixed2<^T when ^T : (member Length: int) >(x: ^T) = (^T : (member get_Length: unit -> int) (x))

    let inline f_set_Length<^T when ^T : (member Length: int with set) >(x: ^T) = (^T : (member Length: int with set) (x, 3))
    let inline f_set_Length_explicit<^T when ^T : (member set_Length: int -> unit) >(x: ^T) = (^T : (member set_Length: int -> unit) (x, 3))
    let inline f_set_Length_mixed<^T when ^T : (member set_Length: int -> unit) >(x: ^T) = (^T : (member Length: int with set) (x, 3))
    let inline f_set_Length_mixed2<^T when ^T : (member Length: int with set) >(x: ^T) = (^T : (member set_Length: int -> unit) (x, 3))

    let inline f_Item<^T when ^T : (member Item: int -> string with get) >(x: ^T) = (^T : (member Item: int -> string with get) (x, 3))
    let inline f_Item_explicit<^T when ^T : (member get_Item: int -> string) >(x: ^T) = (^T : (member get_Item: int -> string) (x, 3))
    let inline f_Item_mixed<^T when ^T : (member get_Item: int -> string) >(x: ^T) = (^T : (member Item: int -> string with get) (x, 3))
    let inline f_Item_mixed2<^T when ^T : (member Item: int -> string with get) >(x: ^T) = (^T : (member get_Item: int -> string) (x, 3))

    //let inline f_set_Item<^T when ^T : (member Item: int -> string with set) >(x: ^T) = (^T : (member Item: int -> string with set) (x, 3, "a"))
    //let inline f_set_Item_explicit<^T when ^T : (member set_Item: int * string -> int) >(x: ^T) = (^T : (member set_Item: int * string -> int) (x, 3, "a"))


module CheckSelfConstrainedSRTP =
    type WithStaticProperty<^T when ^T : (static member StaticProperty: int)> = ^T
    type WithStaticMethod<^T when ^T : (static member StaticMethod: int -> int)> = ^T
    type WithBoth<^T when WithStaticProperty<^T> and WithStaticMethod<^T>> = ^T

    let inline f_StaticProperty<^T when WithStaticProperty<^T>>() = 'T.StaticProperty
    let inline f_StaticMethod<^T when WithStaticMethod<^T>>() = 'T.StaticMethod(3)
    let inline f_Both<^T when WithBoth<^T> >() =
        let v1 = 'T.StaticProperty
        let v2 = 'T.StaticMethod(3)
        v1 + v2

    type AverageOps<^T when ^T: (static member (+): ^T * ^T -> ^T)
                       and  ^T: (static member DivideByInt : ^T*int -> ^T)
                       and  ^T: (static member Zero : ^T)> = ^T

    let inline f_OK1<^T when WithBoth<^T>>() =
        'T.StaticMethod(3)
        'T.StaticMethod(3)

    let inline f_OK2<^T when WithBoth<^T>>() =
        'T.StaticMethod(3)
        'T.StaticMethod(3)

    let inline f_Bug1<^T when WithBoth<^T>>() =
        printfn ""
        'T.StaticMethod(3)
    //let inline f_Bug1<^T when WithBoth<^T>>() =
    //    'T.StaticMethod(3)
    //    'T.StaticMethod(3)
    //let inline f_Bug2<^T when WithBoth<^T>>() =
    //    'T.StaticMethod(3)
    //    'T.StaticMethod(3)
// BUG
    //let inline f_Both<^T when WithBoth<^T>>() =
    //    'T.StaticMethod(3)
    //    'T.StaticMethod(3)

        //'T.StaticMethod(3) |> ignore

module CheckSelfSRTP =
    type IStaticProperty<'T when IStaticProperty<'T>> =
        static abstract StaticProperty: 'T

    type IStaticMethod<'T when IStaticMethod<'T>> =
        static abstract StaticMethod: 'T -> 'T

    type IUnitMethod<'T when IUnitMethod<'T>> =
        static abstract UnitMethod: unit -> unit

    type IAdditionOperator<'T when IAdditionOperator<'T>> =
        static abstract op_Addition: 'T * 'T -> 'T

    type C(c: int) =
        member _.Value = c
        interface IAdditionOperator<C> with
            static member op_Addition(x, y) = C(x.Value + y.Value) 
        interface IStaticProperty<C> with
            static member StaticProperty = C(7)
        interface IStaticMethod<C> with
            static member StaticMethod(x) = C(x.Value + 4)
        interface IUnitMethod<C> with
            static member UnitMethod() = ()

    let f_IWSAM_explicit_operator_name<'T when IAdditionOperator<'T>>(x: 'T, y: 'T) =
        'T.op_Addition(x, y)

    let f_IWSAM_pretty_operator_name<'T when IAdditionOperator<'T>>(x: 'T, y: 'T) =
        'T.(+)(x, y)

    let f_IWSAM_StaticProperty<'T when IStaticProperty<'T>>() =
        'T.StaticProperty

    let f_IWSAM_declared_StaticMethod<'T when IStaticMethod<'T>>(x: 'T) =
        'T.StaticMethod(x)

    let f_IWSAM_declared_UnitMethod<'T when IUnitMethod<'T>>() =
        'T.UnitMethod()

    let f_IWSAM_declared_UnitMethod_list<'T when IUnitMethod<'T>>() =
        let v = 'T.UnitMethod()
        [ v ]

    let inline f3<^T when IAdditionOperator<^T>>(x: ^T, y: ^T) =
        'T.op_Addition(x,y)

    let inline f_StaticProperty_IWSAM<'T when IStaticProperty<'T>>() =
        'T.StaticProperty

module CheckNewSyntax =
    // Check that "property" and "get_ method" constraints are considered logically equivalent
    let inline f_StaticProperty<^T when ^T : (static member StaticProperty: int) >() : int = 'T.StaticProperty

    let inline f_StaticMethod<^T when ^T : (static member StaticMethod: int -> int) >() : int = 'T.StaticMethod(3)

    let inline f_set_StaticProperty<^T when ^T : (static member StaticProperty: int with set) >() = 'T.set_StaticProperty(3)

    let inline f_Length<^T when ^T : (member Length: int) >(x: ^T) = x.Length

    let inline f_set_Length<^T when ^T : (member Length: int with set) >(x: ^T) = x.set_Length(3)

    let inline f_Item1<^T when ^T : (member Item: int -> string with get) >(x: ^T) = x.get_Item(3)

    // Limitation: As yet the syntax "'T.StaticProperty <- 3" can't be used
    // Limitation: As yet the syntax "x.Length <- 3" can't be used
    // Limitation: As yet the syntax "x[3]" can't be used, nor can any slicing syntax
    // Limitation: The disposal pattern can't be used with "use"

    //let inline f_set_StaticProperty2<^T when ^T : (static member StaticProperty: int with set) >() = 'T.StaticProperty <- 3
    //let inline f_set_Length2<^T when ^T : (member Length: int with set) >(x: ^T) = x.Length <- 3
    //let inline f_Item2<^T when ^T : (member Item: int -> string with get) >(x: ^T) = x[3]

let f_StaticMethod_IWSAM<'T when 'T :> IStaticMethod<'T>>(x: 'T) =
    'T.StaticMethod(x)

let inline f_StaticMethod_SRTP<^T when  ^T : (static member StaticMethod: ^T -> ^T) >(x: ^T) =
    'T.StaticMethod(x)

let inline f_StaticMethod_BOTH<^T when ^T :> IStaticMethod<^T> and ^T : (static member StaticMethod: ^T -> ^T) >(x: ^T) =
    'T.StaticMethod(x)


#if NEGATIVE
module Negative =
    let inline f_TraitWithOptional<^T when ^T : (static member StaticMethod: ?x: int -> int) >() = ()
    let inline f_TraitWithIn<^T when ^T : (static member StaticMethod: x: inref<int> -> int) >() = ()
    let inline f_TraitWithOut<^T when ^T : (static member StaticMethod: x: outref<int> -> int) >() = ()
    let inline f_TraitWithParamArray<^T when ^T : (static member StaticMethod: [<ParamArray>] x: int[] -> int) >() = ()
    let inline f_TraitWithCallerName<^T when ^T : (static member StaticMethod: [<System.Runtime.CompilerServices.CallerMemberNameAttribute>] x: int[] -> int) >() = ()
    let inline f_TraitWithExpression<^T when ^T : (static member StaticMethod: x: System.Linq.Expressions.Expression<Func<int,int>> -> int) >() = ()
#endif


//let f7<'T when 'T :> IAdditionOperator<'T>>(x: 'T, y: 'T) =
//    x + y

(*
let inline f_SRTP_GoToDefinition_FindAllReferences (x: ^T) = 
    let y = x + x // implicitly adds constraint to type inference variable ^T
    let z = 'T.op_Addition(x, x) // where would go-to-definition go? what does find-all-references do?
    y + z
*)
