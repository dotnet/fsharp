// #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
// Options
// Interesting case: Some vs None
//<Expects status="success"></Expects>

let samples = [  Some(1,1+1) = Some(1,2);                   // tuples support structural equality
                 Some(hash (1,1+1)) = Some(hash (1,2));     // Function calls return identical values

                 Some(1,2) < Some(1,3) ;
                 Some(1,2) < Some(2,3) ;
                 Some(1,2) < Some(2,1) ;
                 Some(1,2) > Some(1,0) ;
                 
                 Some(1,2) > None;
                 not (Some(1,2) < None)
                 not (Some(1,2) = None)
              ]

let r = List.forall id samples

(if r then 0 else 1) |> exit
