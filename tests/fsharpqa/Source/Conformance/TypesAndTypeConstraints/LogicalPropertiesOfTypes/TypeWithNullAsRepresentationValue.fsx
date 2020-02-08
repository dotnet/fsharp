// #Conformance #TypeConstraints 
#light

// Type With Null As representation value

// Unit type
let u : unit = ()

// Option type: None value
let o : int option = None

// Same as above, but in general for DU
[<Microsoft.FSharp.Core.CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type DU = | NULL
          | Case of int

let du = NULL

printfn "%A" u
printfn "%A" o
printfn "%A" du
let test = [ 
               (sprintf "%A" u) = "()";
               (sprintf "%A" o) = "None";
               (sprintf "%A" du) = "NULL";
           ]  |> List.forall (fun e -> e=true)

(if test then 0 else 1) |> exit
