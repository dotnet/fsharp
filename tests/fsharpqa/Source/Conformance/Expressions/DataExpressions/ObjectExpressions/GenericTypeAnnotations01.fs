// #Regression #Conformance #DataExpressions #ObjectConstructors 
// Regression test for FSHARP1.0:3857
// Incorrect FS0064: This construct causes code to be less generic than indicated by the type annotations. The type variable 'a has been constrained to be type ''a'.
//<Expects status="success"></Expects>

#light
type X = 
    abstract M : unit -> 'a
let v = 
    { new X with 
         member this.M() : 'a = failwith "" }
         
exit 0
