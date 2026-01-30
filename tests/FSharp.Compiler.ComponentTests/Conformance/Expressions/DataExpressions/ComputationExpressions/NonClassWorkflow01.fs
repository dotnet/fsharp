// #Conformance #DataExpressions #ComputationExpressions 
// Verify you can have a non-class based computation expression workflow builder

type make = string
type model = string

type Transport =
    | Car of make * model
    | Bike
    | Walking
    
    member this.Bind(result : unit -> 'a, rest : 'a -> 'b) =
        rest <| result()
        
    member this.Return(x) = x + match this with
                                | Car(_)  -> 100
                                | Bike    -> 10
                                | Walking -> 1
    
let mySweetRide = Car("1981", "Tercel")

let result =
    mySweetRide { 
        let! x = fun () -> 42
        return x
    }

if result <> 142 then exit 1

exit 0

