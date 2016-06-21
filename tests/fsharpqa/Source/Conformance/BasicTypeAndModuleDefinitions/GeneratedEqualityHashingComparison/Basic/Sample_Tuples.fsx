// #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
// Sample on records
//<Expects status="success"></Expects>

let samples = [  (1,1+1) = (1,2);               // tuples support structural equality
                 hash (1,1+1) = hash (1,2);     // Function calls return identical values

                 (1,2) < (1,3) ;
                 (1,2) < (2,3) ;
                 (1,2) < (2,1) ;
                 (1,2) > (1,0) ;
              ]

let r = List.forall id samples

(if r then 0 else 1) |> exit
