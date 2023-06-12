// #Conformance #DeclarationElements #MemberDefinitions #NamedArguments 
#light

open System

module NonGenericStruct =
    type S =
        struct
           val mutable x : int
           val mutable y : int
           member obj.X with set(v) = obj.x <- v
           member obj.Y with set(v) = obj.y <- v
        end

    let x1 : S = S()
    if x1.x <> 0 then failwith "Failed: 1"
    if x1.y <> 0 then failwith "Failed: 2"
    
    let x2 : S = S(X=1, Y=2)
    if x2.x <> 1 then failwith "Failed: 3"
    if x2.y <> 2 then failwith "Failed: 4"
