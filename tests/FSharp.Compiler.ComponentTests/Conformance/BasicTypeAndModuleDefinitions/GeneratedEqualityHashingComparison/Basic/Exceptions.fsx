// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
// Sample on exception
// See bug FSHARP1.0:5345
//<Expects status="success"></Expects>

exception E of int * int

let samples = [  E(1,1+1) = E(1,2);                     // tuples support structural equality
                 hash (E(1,1+1)) = hash (E(1,2));       // Function calls return identical values

                 // E(1,2) < E(1,3) ;           // No longer allowed (see FSHARP1.0:5345)
                 // E(1,2) < E(2,3) ;           // No longer allowed (see FSHARP1.0:5345)
                 // E(1,2) < E(2,1) ;           // No longer allowed (see FSHARP1.0:5345)
                 // E(1,2) > E(1,0) ;           // No longer allowed (see FSHARP1.0:5345)
              ]

let r = List.forall id samples

(if r then 0 else 1) |> exit
