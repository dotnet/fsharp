// #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
// Arrays
//<Expects status="success"></Expects>

let samples = [  [|1;1+1|] = [|1;2|];
                 [|hash (1,1+1)|] = [|hash (1,2)|];

                 [|(1,2)|] < [|(1,3)|] ;
                 [|(1,2)|] < [|(2,3)|] ;
                 [|(1,2)|] < [|(2,1)|] ;
                 [|(1,2)|] > [|(1,0)|] ;
              ]

let r = List.forall ( fun x -> x) samples

(if r then 0 else 1) |> exit
