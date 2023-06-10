// #Conformance #TypesAndModules #Modules 
// Use [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>] to
// avoid name clashing between types and modules
#light

type m = | M

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>] 
module m = let p = typeof<m>
