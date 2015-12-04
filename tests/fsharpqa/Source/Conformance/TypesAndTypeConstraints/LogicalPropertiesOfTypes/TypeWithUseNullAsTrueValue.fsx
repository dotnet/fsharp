// #Conformance #TypeConstraints 
#light

[<Microsoft.FSharp.Core.CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type DU = | NULL
          | Case of int

let du = NULL

[<Microsoft.FSharp.Core.CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type D6 = | NULL_OF_6
          | Case2 of int
          | Case3 of int
          | Case4 of int
          | Case5 of int
          | Case6 of int

let du6 = NULL_OF_6

let test = [ 
               (sprintf "%A" du) = "<null>";
               (sprintf "%A" du6) = "<null>";
           ]  |> List.forall (fun e -> e=true)

(if test then 0 else 1) |> exit
