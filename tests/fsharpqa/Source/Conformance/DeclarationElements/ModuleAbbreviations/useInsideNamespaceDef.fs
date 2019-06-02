// #Conformance #DeclarationElements #Modules 
// Use module abbreviation inside a namespace
//<Expects status="success"></Expects>
#light

namespace Faa.Bor
    module A =
        module B =
            module C =
               type X = | Red  = 1
                        | Blue = 2 
               let DirectorySeparatorChar = System.IO.Path.DirectorySeparatorChar 

    module IO = A.B.C

    type Faabor<'a>() =
        let dsc = IO.DirectorySeparatorChar
        member this.PrintDSC () = printfn "%A" dsc
