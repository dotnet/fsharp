// #Conformance #PatternMatching #Tuples 
#light
                      
open System
open System.Collections.Generic

let f (items: IEnumerable<Tuple<int, string>>) =
    items
    |> Seq.iter (fun i -> printf "%d %s" i.Item1 i.Item2)
    
let x = seq { yield Tuple.Create(10, "ten") }

f x

exit 0
