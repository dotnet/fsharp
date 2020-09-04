module Pos37

open System

let PlaceReferenceConstraint<'T when 'T : not struct> (x : 'T) = x
let PlaceNullConstraint<'T when 'T : null> (x : 'T) = x
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
    // Struct tuples are always considered to satisfy ValueType constraint 
    // 
    type T1<'T when 'T :> ValueType>(x : struct ('T * 'T)) = 
        do x |> PlaceValueTypeConstraint |> ignore

    let f1 (x : struct  ('T * 'T)) =
        x |> PlaceValueTypeConstraint |> ignore

    f1 (struct (1, 1))
    f1 (struct (System.DateTime.Now, System.DateTime.Now))

    // Struct tuple types are considered to satisfy struct constraint if they have that constraint
    // or it is inferred
    // 
    type T2<'T when 'T : struct>(x : struct  ('T * 'T)) = 
        do x |> PlaceStructConstraint |> ignore

    let f2 (x : struct  ('T * 'T)) =
        x |> PlaceStructConstraint |> ignore

    f2 (struct (1, 1))
    f2 (struct (System.DateTime.Now, System.DateTime.Now))

    // Struct tuple types are considered to satisfy default constructor constraint if each element
    // type is known (without placing any constraints) to have a default value.  If the element types
    // are variable types they must have been pre-determined through inference or explicit
    // declaration to be either struct or reference variable types.
    type T3a(x : struct (int * string)) = 
        do x |> PlaceDefaultConstructorConstraint |> ignore

    let v3a = T3a (struct (1, "a"))

    // The case for struct and reference element types
    let f3a (x : struct (int * string)) =
        x |> PlaceDefaultConstructorConstraint |> ignore

    f3a (struct (1, "a"))

    // The case for struct type variables (declared)
    type T3b<'T when 'T : struct and 'T : (new : unit -> 'T)>(x : struct (int * 'T)) = 
        do x |> PlaceDefaultConstructorConstraint |> ignore

    let v3b = T3b (struct (1, System.DateTime.Now))

    // The case for struct type variables (inferred)
    let f3b (x : struct (int * 'T)) = 
        let (struct (a,b)) = x
        b |> PlaceStructConstraint |> ignore
        x |> PlaceDefaultConstructorConstraint |> ignore

    f3b (struct (1, 1))
    f3b (struct (1, System.DateTime.Now))

    // The case for reference type variables (declared)
    type T3c<'T when 'T : not struct and 'T : null>(x : struct (int * 'T)) = 
        do x |> PlaceDefaultConstructorConstraint |> ignore

    let v3c = T3c (struct (1, "abc"))

    // The case for reference type variables (inferred)
    let f3c (x : struct (int * 'T)) = 
        let (struct (a,b)) = x
        b |> PlaceReferenceConstraint |> ignore
        x |> PlaceDefaultConstructorConstraint |> ignore

    f3c (struct (1, "abc"))
    f3c (struct (1, obj()))

module TestStructAnonRecords =
    // Struct anon record are always considered to satisfy ValueType constraint 
    // 
    type T1<'T when 'T :> ValueType>(x : struct {| X : 'T; Y : 'T |}) = 
        do x |> PlaceValueTypeConstraint |> ignore

    let f1 (x : struct  {| X : 'T; Y : 'T |}) =
        x |> PlaceValueTypeConstraint |> ignore

    f1 {| X = 1; Y = 1 |}
    f1 {| X = System.DateTime.Now ; Y = System.DateTime.Now |}

    // Struct tuple types are considered to satisfy struct constraint if they have that constraint
    // or it is inferred
    // 
    type T2<'T when 'T : struct>(x : struct  {| X : 'T; Y : 'T |}) = 
        do x |> PlaceStructConstraint |> ignore

    let f2 (x : struct  {| X : 'T; Y : 'T |}) =
        x |> PlaceStructConstraint |> ignore

    f2 {| X = 1; Y = 1 |}
    f2 {| X = System.DateTime.Now ; Y = System.DateTime.Now |}

    // Struct tuple types are considered to satisfy default constructor constraint if each element
    // type is known (without placing any constraints) to have a default value.  If the element types
    // are variable types they must have been pre-determined through inference or explicit
    // declaration to be either struct or reference variable types.
    type T3a(x : struct {| X : int; Y : string |}) = 
        do x |> PlaceDefaultConstructorConstraint |> ignore

    // The case for struct and reference element types
    let f3a (x : struct {| X : int; Y : string |}) =
        x |> PlaceDefaultConstructorConstraint |> ignore

    f3a {| X = 1; Y = "a" |}

    // The case for struct type variables (declared)
    type T3b<'T when 'T : struct and 'T : (new : unit -> 'T)>(x : struct {| X : int; Y : 'T |}) = 
        do x |> PlaceDefaultConstructorConstraint |> ignore

    let v3b = T3b {| X = 1; Y = System.DateTime.Now |}

    // The case for struct type variables (inferred)
    let f3b (x : struct {| X : int; Y : 'T |}) = 
        x.Y |> PlaceStructConstraint |> ignore
        x |> PlaceDefaultConstructorConstraint |> ignore

    f3b {| X = 1; Y = 1 |}
    f3b {| X = 1; Y = System.DateTime.Now |}

    // The case for reference type variables (declared)
    type T3c<'T when 'T : not struct and 'T : null>(x : struct {| X : int; Y : 'T |}) = 
        do x |> PlaceDefaultConstructorConstraint |> ignore

    let v3c = T3c {| X = 1; Y = "abc" |}

    // The case for reference type variables (inferred)
    let f3c (x : struct {| X : int; Y : 'T |}) = 
        x.Y |> PlaceReferenceConstraint |> ignore
        x |> PlaceDefaultConstructorConstraint |> ignore

    f3c {| X = 1; Y = "abc" |}
    f3c {| X = 1; Y = obj() |}

module TestNullableStructTuples =
    // Struct tuples are considered to satisfy the combined Nullable<_> constraints
    //    X : struct
    //    X :> ValueType
    //    X : (new : unit -> X)
    // if each element type is known to have a default value.
    let m1 (x: Nullable<struct(int * string)>) = ()

    let m2 (x: Nullable<ValueTuple<int, string>>) = ()

    type T<'T when 'T :> ValueType and 'T : struct and 'T : (new : unit -> 'T)>() = 
        let m (x: Nullable<struct(int * 'T)>) = ()

module TestNullableStructAnonRecds =
    // Struct anon records are considered to satisfy the combined Nullable<_> constraints
    //    X : struct
    //    X :> ValueType
    //    X : (new : unit -> X)
    // if each element type is known to have a default value.  For variable types
    // this means knowing whether the variable type is a reference or value type
    let m1 (x: Nullable<struct {| X : int; Y : string |} >) = ()

    type Ta<'T when 'T :> ValueType and 'T : struct and 'T : (new : unit -> 'T)>() = 
        let m (x: Nullable<struct {| X : int; Y : 'T |}>) = ()

    type Tb<'T when 'T : not struct and 'T : null>() = 
        let m (x: Nullable<struct {| X : int; Y : 'T |}>) = ()

module TestAnotherStructRecord = 
    
    let inline test<'t when 't: (new: unit -> 't)  and  't: struct and  't:> ValueType>    () = ()

    [<Struct>]
    type Delta =
        { SkipX: byte
          Control: sbyte }

    test<Delta> ()
    test< struct {| SkipX: byte; Control: sbyte |} > ()
    test< struct (byte * sbyte) > ()

module TestAnotherStructTuple = 
    
    let x = System.Nullable<struct (int * int)>(struct (0, 0)) // note, the type annotations are required

module TestAnotherStructTuple2 = 
    
    let x = struct(0, 0)
    let y = System.Nullable(x)

module TestAnotherStructTuple3 = 
    
    let x = struct(0, 0)
    let y = System.Nullable(struct (0, 0) |> unbox<System.ValueTuple<int,int>>)
    let z = System.Nullable(x |> unbox<System.ValueTuple<int,int>>)
