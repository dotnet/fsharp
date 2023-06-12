// #Conformance #TypesAndModules #Records 
// Field labels have module scope
// Also, in this case we see that there is no collision when the field and type have the same name
//<Expects status="success"></Expects>
#light

    type ı = int

    module M0 =
        type T1 = { ı : ı;}                     // no conclict with type defined above

    let x : M0.T1 = { ı = 1 }

    (if x.ı = 1 then 0 else failwith "Failed")
