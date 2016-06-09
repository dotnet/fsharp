// #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Upcast to object - null
//<Expects status="success"></Expects>
#light
let a = ( upcast null ) : obj
let b = ( null :> _ ) : obj
let c = null :> obj

(if (a = b) && (b = c) then 0 else 1) |> exit
