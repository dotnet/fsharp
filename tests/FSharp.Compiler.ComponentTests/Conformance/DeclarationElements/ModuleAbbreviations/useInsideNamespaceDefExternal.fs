// #Conformance #DeclarationElements #Modules #ReqNOMT 
// Use module abbreviation inside a namespace
// Module is defined in an external assembly
//<Expects status="success"></Expects>
#light

namespace Faa.Bor

    module IO =  A.B.C

    type Faabor<'a>() =
        let dsc = IO.DirectorySeparatorChar
        member this.PrintDSC () = printfn "%A" dsc
    
