// #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Upcast with 1-level class hierarchy - cast to interface
//<Expects status="success"></Expects>
#light

type I    = interface
               abstract member M : int -> int
            end

type K1() = class
              interface I with
                 member x.M(y) = y
            end
              
let k = K1()
          
let a' = ( upcast k ) : I
let b' = ( k :> _ ) : I
let c' = ( k :> I )
(if (a'.M(1) = b'.M(1)) && (b'.M(1) = c'.M(1)) then 0 else 1) |> exit
