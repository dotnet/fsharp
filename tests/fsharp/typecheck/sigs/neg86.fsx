
let f1() =
    query { for x in [1] do
            where 1 in 2 }

let f2() =
    query { for c in [1] do
            join 
            select (e,c) }

let f3() =
    query { for c in [1] do
            join e 
            select (e,c) }

let f4() =
    query { for c in [1] do
            join e in   }
    printfn "recover"

let f5() =
    query { for c in [1] do
            join e in  [1;2;3] //on (c = e); 
            select (e,c) }

let f6() =
    query { for c in [1] do
            join e in  [1;2;3] on // (c = e); 
            select (e,c) }

let f7() =
    query { for c in [1] do
            join e in  [1;2;3] on (c); 
            select (e,c) }


