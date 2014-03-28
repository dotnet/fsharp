// #Regression #Conformance #Quotations 
// Regression test for FSHARP1.0:5536
// PropertySet was returning args/value swapped into the wrong positions for multi arg indexers

open System.Collections.Generic
open Microsoft.FSharp.Quotations.Patterns

type PropTest = 
  { data : Dictionary<(string * string),int> }
  member x.Item 
     with get(a,b)   = x.data.[(a,b)]
     and  set(a,b) v = x.data.[(a,b)] <- v 
     
let dict = new Dictionary<string * string, int>()
dict.Add(("a","b"), 1)
let t = { new PropTest with data = dict }
let q = <@ t.["a", "b"] <- 0 @>

exit <| match q with
        | PropertySet(inst, pi, args, value) -> 
            if value.Type = typeof<int> then
                match args with
                | Value x :: Value y :: [] -> if x.ToString() = "(a, System.String)" && y.ToString() = "(b, System.String)" then 0 else 1
                | _ -> 1
            else 1
        | _ -> 1
