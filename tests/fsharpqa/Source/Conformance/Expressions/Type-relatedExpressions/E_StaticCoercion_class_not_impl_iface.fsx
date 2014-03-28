// #Regression #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Negative tests on :>
// Cast to an interface not implemented

//<Expects id="FS0193" span="(25,12-25,20)" status="error">Type constraint mismatch\. The type</Expects>

//<Expects id="FS0193" span="(26,12-26,18)" status="error">Type constraint mismatch\. The type</Expects>

//<Expects id="FS0193" span="(27,12-27,18)" status="error">Type constraint mismatch\. The type</Expects>




type I    = interface
               abstract member M : int -> int
            end

type K1() = class
                // interface I with
                  member x.M(y) = y
            end
              
let k = K1()
          
let a' = ( upcast k ) : I
let b' = ( k :> _ ) : I
let c' = ( k :> I )

exit 1
