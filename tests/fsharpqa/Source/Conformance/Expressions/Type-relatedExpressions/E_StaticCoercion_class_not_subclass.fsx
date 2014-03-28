// #Regression #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Negative tests on :>
// Cast to a class that is not parent

//<Expects id="FS0193" span="(20,12-20,20)" status="error">Type constraint mismatch\. The type</Expects>

//<Expects id="FS0193" span="(21,12-21,18)" status="error">Type constraint mismatch\. The type</Expects>

//<Expects id="FS0193" span="(22,12-22,19)" status="error">Type constraint mismatch\. The type</Expects>



type K1() = class
            end
type K2() = class
            end

let k = K2()
          
let a' = ( upcast k ) : K1
let b' = ( k :> _ ) : K1
let c' = ( k :> K1 )
(if (a' = b') && (b' = c') then 0 else 1) |> exit
