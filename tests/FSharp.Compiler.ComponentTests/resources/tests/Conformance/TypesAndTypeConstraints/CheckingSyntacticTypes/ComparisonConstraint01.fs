// #Conformance #TypeConstraints 

open System

let testFunc<'a when 'a:comparison> (x : 'a) =
    let o = obj()
    x.Equals(o)

testFunc 1 |> ignore

type S3(x : int) =
    interface IComparable with
        override this.CompareTo(o : obj) = 0
    
    member this.Test = x + 1

testFunc (S3(2)) |> ignore
testFunc [1;2;3] |> ignore
testFunc [|1;2;3|] |> ignore
testFunc (System.IntPtr(1)) |> ignore

exit 0