// #Conformance #TypesAndModules #Records 
// This test is a bit of a kitchen-sink, but its main
// purpose is to validate that we can use fully qualified field names
//<Expects status="success"></Expects>
#light

    type ı = int

    module M0 =
        type T1 = { ı : ı;}                     // no conclict with type defined above
        

    module M1 =
        type T1 = { ı : int; ıı : int; i : int; I : int }
                    member x.ııı = x.ı               // member accessing the field
        
        let r1 = { ı = 10; ıı = 11; i = 12; I = 13 }
        
    let r2 : M1.T1 = { M1.T1.ı = 10; M1.T1.ıı = 11; M1.T1.i = 12; M1.T1.I = 13 }

    open M1
    let f x = x.ı
    let w = f ({ ı = 10; ıı = 11; i = 12; I = 13 })

    module M2 =
        open M1
        
        type T1 = { ı : int; I : int; ıı : int; i : int;  }
                    member x.ııı = x.ı               // member accessing the field
        
        let r1 = { ı = 10; ıı = 11; i = 12; I = 13 }
        
        let r2 : M1.T1 = { M1.T1.ı = 10; M1.T1.ıı = 11; M1.T1.i = 12; M1.T1.I = 13 }

        open M1
    
        let f x = x.ı
        let w = f ({ ı = 10; ıı = 11; i = 12; I = 13 })
