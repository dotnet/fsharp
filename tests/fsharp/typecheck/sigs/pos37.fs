module Pos37

open System

let PlaceStructConstraint<'T when 'T : struct> (x : 'T) = x
let PlaceValueTypeConstraint<'T when 'T :> ValueType> (x : 'T) = x
let PlaceDefaultConstructorConstraint<'T when 'T : (new : unit -> 'T)> (x : 'T) = x

module TestVariableTypes =

    // Variable types are considered to satisfy ValueType constraint if they have that constraint
    // or it is inferred
    // 
    type T1<'T when 'T :> ValueType>(x : 'T) = 
        do x |> PlaceValueTypeConstraint |> ignore

    let f1 x = x |> PlaceValueTypeConstraint |> ignore

    // Variable types are considered to satisfy struct constraint if they have that constraint
    // or it is inferred
    // 
    type T2<'T when 'T : struct>(x : 'T) = 
        do x |> PlaceStructConstraint |> ignore

    let f2 x = x |> PlaceStructConstraint |> ignore

    // Variable types are considered to satisfy default constructor constraint if they have that constraint
    // or it is inferred
    // 
    type T3<'T when 'T : (new : unit -> 'T)>(x : 'T) = 
        do x |> PlaceDefaultConstructorConstraint |> ignore

    let f3 x = x |> PlaceDefaultConstructorConstraint |> ignore

module TestStructTuples =
    // Struct tuple types are always considered to satisfy ValueType constraint 
    // 
    type T1<'T when 'T :> ValueType>(x : struct ('T * 'T)) = 
        do x |> PlaceValueTypeConstraint |> ignore

    let f1 (x : struct ('T * 'T)) = x |> PlaceValueTypeConstraint |> ignore

    // Struct tuple types are considered to satisfy struct constraint if they have that constraint
    // or it is inferred
    // 
    type T2<'T when 'T : struct>(x : struct ('T * 'T)) = 
        do x |> PlaceStructConstraint |> ignore

    let f2 (x : struct ('T * 'T)) = x |> PlaceStructConstraint |> ignore

    // Struct tuple types are considered to satisfy default constructor constraint if each element
    // type is known (without placing any constraints) to have a default value.  The elements may
    // not be variable types.
    //
    // Note: this restriction may be lifted in future versions of F#.
    // 
    type T3a(x : struct (int * string)) = 
        do x |> PlaceDefaultConstructorConstraint |> ignore

    let f3a (x : struct (int * string)) = x |> PlaceDefaultConstructorConstraint |> ignore

    // This is the case that is not supported in F# 5.0 or before. There is no single appropriate "default value" constraint
    // to place on 'T here - and F# 5.0 doesn't split the cases between struct and non-struct types.
    // This may be addressed when nullable reference types and the ": not null" constraint on reference types and " : null " on value types
    // is supported, but needs careful testing.
    //
    //type T3b<'T when 'T : (new : unit -> 'T)>(x : struct (int * 'T)) = 
    //    do x |> PlaceDefaultConstructorConstraint |> ignore

    //let f3a (x : struct (int * 'T)) = x |> PlaceDefaultConstructorConstraint |> ignore

module TestStructAnonRecords =
    // Struct anon record are always considered to satisfy ValueType constraint 
    // 
    type T1<'T when 'T :> ValueType>(x : struct {| X : 'T; Y : 'T |}) = 
        do x |> PlaceValueTypeConstraint |> ignore

    let f1 (x : struct  {| X : 'T; Y : 'T |}) = x |> PlaceValueTypeConstraint |> ignore

    // Struct tuple types are considered to satisfy struct constraint if they have that constraint
    // or it is inferred
    // 
    type T2<'T when 'T : struct>(x : struct  {| X : 'T; Y : 'T |}) = 
        do x |> PlaceStructConstraint |> ignore

    let f2 (x : struct  {| X : 'T; Y : 'T |}) = x |> PlaceStructConstraint |> ignore

    // Struct tuple types are considered to satisfy default constructor constraint if each element
    // type is known (without placing any constraints) to have a default value.  The elements may
    // not be variable types.
    //
    // Note: this restriction may be lifted in future versions of F#.
    // 
    type T3a(x : struct {| X : int; Y : string |}) = 
        do x |> PlaceDefaultConstructorConstraint |> ignore

    let f3a (x : struct {| X : int; Y : string |}) = x |> PlaceDefaultConstructorConstraint |> ignore

    // This is the case that is not supported in F# 5.0 or before. There is no single appropriate "default value" constraint
    // to place on 'T here - and F# 5.0 doesn't split the cases between struct and non-struct types.
    // This may be addressed when nullable reference types and the ": not null" constraint on reference types and " : null " on value types
    // is supported, but needs careful testing.
    //
    //type T3b<'T when 'T : (new : unit -> 'T)>(x : struct {| X : int; Y : 'T |}) = 
    //    do x |> PlaceDefaultConstructorConstraint |> ignore

    //let f3a (x : struct {| X : int; Y : 'T |}) = x |> PlaceDefaultConstructorConstraint |> ignore

module TestNullableStructTuples =
    // Struct tuples are considered to satisfy the combined Nullable<_> constraints
    //    X : struct
    //    X :> ValueType
    //    X : (new : unit -> X)
    // if each element type is nominal and known to have a default value.
    let m1 (x: Nullable<struct(int * string)>) = ()

    let m2 (x: Nullable<ValueTuple<int, string>>) = ()


    // As mentioned above, in F# 5.0 struct tuples containing type variables or inference variables are not considered to satisfy 
    // the collected Nullable constraints
    //    X : struct
    //    X :> ValueType
    //    X : (new : unit -> X)
    // 
    //type T<'T when 'T :> ValueType and 'T : struct and 'T : (new : unit -> 'T)>() = 
    //    let m (x: Nullable<struct(int * 'T)>) = unit

module TestNullableStructAnonRecds =
    // Struct aon records are considered to satisfy the combined Nullable<_> constraints
    //    X : struct
    //    X :> ValueType
    //    X : (new : unit -> X)
    // if each element type is nominal and known to have a default value.
    let m1 (x: Nullable<struct {| X : int; Y : string |} >) = ()

    // As mentioned above, in F# 5.0 struct anon records containing type variables or inference variables are not considered to satisfy 
    // the collected Nullable constraints
