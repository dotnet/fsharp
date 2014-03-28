// #Regression #Conformance #DataExpressions #ComputationExpressions 
// Verify you can create a workflow builder via extension methods
// Regression test for FSHARP1.0:5040
type System.String with
    
    member this.Bind(result : string -> string, rest : string -> 'b) =
        rest <| result this
        
    member this.Bind(result : unit -> 'a, rest : 'a -> 'b) =
        rest <| result()
        
    member this.Return(x) = x + this
    
    
let result =
    "foo" {
        let! x = (fun (str : string) -> str + "." + str)
        let! y = (fun () -> "!")
        return x + y
    }
    
if result <> "foo.foo!foo" then exit 1

exit 0
