// #Regression #Conformance #TypesAndModules #Modules 
// Clashing module names and use of CompilationRepresentation attribute
// This is regression test for FSHARP1.0:3749


namespace N

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>] 

module ``module`` = 
            let f x = x + 1

module ``module`` =
            let g x = x + 1
