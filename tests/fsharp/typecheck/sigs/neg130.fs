module Neg130

open System

let PlaceDefaultConstructorConstraint<'T when 'T : (new : unit -> 'T)> (x : 'T) = x

module TestVariableTypesForTuples =

    // The case for type variables (declared) where struct/reference is not known
    type T3b<'T>(x : struct (int * 'T)) = 
        do x |> PlaceDefaultConstructorConstraint |> ignore

    // The case for type variables (inferred) where struct/reference is not known
    let f3b (x : struct (int * 'T)) = 
        x |> PlaceDefaultConstructorConstraint |> ignore

module TestVariableTypesForAnonRecords =
    // Struct anon record are always considered to satisfy ValueType constraint 
    // 
    type T3b<'T>(x : struct {| X : int; Y : 'T |}) = 
        do x |> PlaceDefaultConstructorConstraint |> ignore

    // The case for type variables (inferred) where struct/reference is not known
    let f3b (x : struct {| X : int; Y : 'T |}) = 
        x |> PlaceDefaultConstructorConstraint |> ignore


