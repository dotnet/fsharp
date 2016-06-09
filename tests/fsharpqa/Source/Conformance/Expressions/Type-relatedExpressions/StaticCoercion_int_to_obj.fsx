// #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Upcast to object - int
//<Expects status="success"></Expects>
#light
let a = ( upcast 1 ) : obj
let b = ( 1 :> _ ) : obj
let c = 1 :> obj

(if (a = b) && (b = c) then 0 else 1) |> exit
