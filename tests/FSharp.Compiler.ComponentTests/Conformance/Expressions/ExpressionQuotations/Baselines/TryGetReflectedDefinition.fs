// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let incr x = x + 1
[<ReflectedDefinition>]
let f x = incr x * 2

let q = <@ f 5 @>
let mi = match q with
         | Call (None, m, arg) -> Some(m)
         | _ -> None
let rd = Expr.TryGetReflectedDefinition(mi.Value)
// Get nested call which doesn't have a reflected definition
let mi2 = match rd.Value with
          | Lambda (_, body) -> 
              match body with
              | Call (None, m, args) -> 
                   match args.Head with
                   | Call (None, mi, _) -> Some(mi)
                   | _ -> None
              | _ -> None
          | _ -> None
          
let rd2 = Expr.TryGetReflectedDefinition(mi2.Value)

exit <| match rd, rd2 with
        | Some(x), None -> 0
        | _, _ -> 1
