// #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
// Unions
//<Expects status="success"></Expects>

type DU = | A
          | B

type DU2 = | B
           | A

let samples = [  
                 (DU.A,DU.B) = (DU.A,DU.B);
                 hash (DU.A,DU.B) = hash (DU.A,DU.B);
                 (DU.A,DU.B) < (DU.B,DU.A) ;
                 (DU.A,DU.B) <= (DU.B,DU.A) ;
                 [|DU.B|] > [|DU.A|]
                 [|DU.A|] < [|DU.B|]

                 (DU2.A,DU2.B) = (DU2.A,DU2.B);
                 hash (DU2.A,DU2.B) = hash (DU2.A,DU2.B);
                 (DU2.A,DU2.B) > (DU2.B,DU2.A) ;
                 (DU2.A,DU2.B) >= (DU2.B,DU2.A) ;
                 [|DU2.B|] < [|DU2.A|]
                 [|DU2.A|] > [|DU2.B|]

              ]

let r = List.forall id samples

(if r then 0 else 1) |> exit
