#r "../../../../../../Debug/net40/bin/generic_tp.dll"

open System
open FSharp.Reflection

open Test

//type M = Generic.IdentityMethod

//type T = Generic.IdentityType<int>

provider MyNewProvider<'a> = Generic.IdentityType<Generic.IdentityType<'a>>

//let myIdentity (x : 'a) =
//  M.Create<'a option> (Some x)
//
//let myIdentity2 (x : 'a) =
//  Generic.IdentityType<'a>.Invoke x
//
//printfn "%A; %A" (myIdentity "body")
//  (myIdentity2 "body")

// let myApply (f : 'a -> 'a) =
//   // myApply f x = f (f (f (f x)))
//   Repeat.Create<'a, 4> f
//
// [<Interface>]
// type MyType<'a> =
//   abstract member __.F (x : 'a) : unit
//
// let makeInterface<'a> : MyType<'a> =
//   Mock.generate<MyType<'a>>