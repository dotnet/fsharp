// #Conformance #TypesAndModules #Modules #ReqNOMT 
#light

namespace N

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>] 
module m1 = begin
             let f x = x + 1
            end
