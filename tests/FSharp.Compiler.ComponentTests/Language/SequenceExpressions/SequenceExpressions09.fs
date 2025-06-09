module OverridingRangeOperator
        
open System.Collections
open System.Collections.Generic

type X(elements: X list) =
    member this.Elements = elements

    interface IEnumerable<X> with
        member this.GetEnumerator() =
            (this.Elements :> IEnumerable<X>).GetEnumerator()

        member this.GetEnumerator() : IEnumerator =
            (this.Elements :> IEnumerable).GetEnumerator()

    static member Combine(x1: X, x2: X) =
        X(x1.Elements @ x2.Elements)

let (..) a b = seq { X.Combine(a,b) }

let a = X([])
let b = X([])

let whatIsThis = seq { a..b }

let result1 = [ whatIsThis ;a ; b]
let result2 = [ yield! whatIsThis; a; b ]

printfn $"As is: {result1}"
printfn $"Implicit yield! {result2}"