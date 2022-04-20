// #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
// Structs
//<Expects status="success"></Expects>

type S<'a> = struct
                 new ( a:'a ) = { f = a }
                 val f : 'a
             end

let samples = [  S((1,1+1)) = S((1,2));
                 S(hash (1,1+1)) = S(hash (1,2));

                 S((1,2)) < S((1,3)) ;
                 S((1,2)) < S((2,3)) ;
                 S((1,2)) < S((2,1)) ;
                 S((1,2)) > S((1,0)) ;
              ]

let r = List.forall id samples

(if r then 0 else 1) |> exit
