// #Conformance #SignatureFiles #Structs #Regression
// Regression for Dev11:137942, structs used to not give errors on when member names conflicted with interface members

module M

open System.Collections
open System.Collections.Generic

[<Struct>]
type Foo<'T> =
    val offset : int
    new (x:'T) = { offset = 1 }
    interface IEnumerable<'T> with 
        member this.GetEnumerator() = null :> IEnumerator<'T> 
        member this.GetEnumerator() = null :> IEnumerator

//let foo = Foo<int>(1)
//let e = foo.GetEnumerator()
