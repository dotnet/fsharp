namespace FooBarSoftware
open System.Collections
open System.Collections.Generic
[<Struct>]
type Foo<'T> =
  val offset : int
  new (x:'T) = { offset = 1 }
  interface IEnumerable<'T> with 
    member this.GetEnumerator() = null :> IEnumerator<'T> 
    member this.GetEnumerator() = null :> IEnumerator

