// #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Upcast with 1-level class hierarchy
//<Expects status="success"></Expects>
#light

type K1() = class
            end
type K2() = class
             inherit K1()
            end
let k = K2()
          
let a' = ( upcast k ) : K1
let b' = ( k :> _ ) : K1
let c' = ( k :> K1 )
(if (a' = b') && (b' = c') then 0 else 1) |> exit
