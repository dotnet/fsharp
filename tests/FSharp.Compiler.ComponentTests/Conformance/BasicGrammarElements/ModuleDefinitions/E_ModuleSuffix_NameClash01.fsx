// #Regression #Conformance #TypesAndModules #Modules 
// Decorating a module with the CompilationRepresentation
// will append 'Module' to the module name.
// This code won't compile since there's a name clash ('mModule')



[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>] 
module m = 
            let f x = x + 1

module mModule =
            let f x = x + 1

exit 0
