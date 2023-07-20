// #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
// Sample on records
//<Expects status="success"></Expects>

type R = R of int * int

let samples = [
                    R(1,1+1) = R(1,2)                   ;
                    not (R(1,3) = R(1,2))               ;
                    hash (R(1,1+1)) = hash (R(1,2))     ;
                    R(1,2) < R(1,3)                     ;
                    R(1,2) < R(2,3)                     ;
                    R(1,2) < R(2,1)                     ;
                    R(1,2) > R(1,0)                     ;
              ]

let r = List.forall id samples

(if r then 0 else 1) |> exit


