// #Conformance #TypesAndModules #Modules 
// Decorating a module with the CompilationRepresentation
//<Expects status="success"></Expects> 
#light

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>] 
module m = 
            let f x = x + 1

open m
(if f(1) = 2 then 0 else 1) |> exit
