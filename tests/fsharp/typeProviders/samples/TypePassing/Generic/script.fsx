#r "../../../../../../Debug/net40/bin/generic_tp.dll"

open System
open FSharp.Reflection

open Test

// ======= Existing type testing ========

type If<'cond, 'a, 'b> = Generic.IfThenElse<'cond, 'a, 'b>
type IntOrString<'cond> = If<'cond, int, string>

type XInt = IntOrString<true>

let f (x : XInt) = x

printfn "%A" (f 10)

  

// =========== Alias testing ============

//type G<'a> = | G of 'a
//
//type MyNewProvider<'a> = Generic.ConstType< G<'a> , string >
//type MyOtherProvider<'b> = MyNewProvider<Option<'b>>
//
//type X = MyOtherProvider<int>
//
//let x = X.Invoke((G (Some 5)), "hello")
//
//printfn "%A" x

// ======= Generic method testing =======

//type M = Generic.IdentityMethod

//type T = Generic.IdentityType<int>
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