// #Regression #Conformance #Quotations
// Regression for FSB 4708
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns

[<ReflectedDefinition>] 
let rec foo () = if true then foo () else 1.0

let t = match <@ foo ()  @> with 
                     | Call(_,MethodWithReflectedDefinition(Lambdas(_,t)),_) -> t
                     | _ -> failwith "?" 

exit 0
