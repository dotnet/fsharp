// #Conformance #DeclarationElements #Modules 
//<Expects status="success"></Expects>
#light

module A =
    module B =
        module C =
           type X = | Red  = 1
                    | Blue = 2 
            
            
module Abbreviated_A = A
module Abbreviated_A_B = A.B
module Abbreviated_A_B_C = A.B.C

    
let blue = int Abbreviated_A_B_C.X.Blue
let red = int Abbreviated_A_B.C.X.Red

if (blue = 2) && (red = 1) then () else failwith "Failed: 1"
