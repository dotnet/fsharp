// #Regression #NoMT #FSI 
#light

// Regression test for FSHARP1.0:3628 - "do 1" expression code gen error, unverifiable code

let fA (x:int) = do x

let resA = fA 12

type T() = class let x = do 1
                 member this.X = x
           end

exit 0

// ';;' to end FSI session
;;
