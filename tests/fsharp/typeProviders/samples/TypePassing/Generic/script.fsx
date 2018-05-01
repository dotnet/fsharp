#r "../../../../../../Debug/net40/bin/generic_tp.dll"

open System
open FSharp.Reflection

open Test

//type M = Generic.IdentityMethod

//type T = Generic.IdentityType<int>


type MyNewProvider<'a> = Generic.IdentityType<'a>

//type A = | A with static member Invoke(i) = 0


//type A<'a> = Option<MP<'a>>
//
//type Y = A<int>

type X = MyNewProvider<int>

let x = X.Invoke 1
//
//type X = MyNewProvider<int>

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