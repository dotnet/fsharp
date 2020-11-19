// #Regression #Conformance #TypeInference 
// Dev11:129910, this legal code was failing to compile

type C<'U> = 
    member x.P with get v = (v :> System.IComparable<'U> |> ignore)

type C2<'U>() = 
    member x.P with get v = (v :> System.IComparable<'U> |> ignore)

let x = C2<int>()
x.P 1

exit 0
