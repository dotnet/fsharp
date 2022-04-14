// #Conformance #TypesAndModules #Records 
#light

// Verify the ability to fully qualify record fields

module Test =

    module A =
        module B = 
            module C =
                module D =
                    type Person = { Name : string; Age : int }
                    type Car    = { Name : string; Age : int; Miles : int }

    open A.B.C.D        
        
    let aCar    = { A.B.C.D.Car.Name    = "Mitsubishi"; Age = 4; Miles = 20000 }
    let aPerson = { A.B.C.D.Person.Name = "Chris"; Age = -831 }

    if aCar.Miles <> 20000 then failwith "Failed"
    if aPerson.Age <> -831 then failwith "Failed"
