#r "../../../../../../Debug/net40/bin/generic_tp.dll"

open System
open FSharp.Reflection

open Test

type I = Generic.Identity

let myIdentity (x : 'a) =
  I.Create<'a option> (Some x)

printfn "%A" (myIdentity "body")

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