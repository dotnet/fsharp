// #Regression #Conformance #DeclarationElements #LetBindings #TypeInference #RequiresPowerPack 
// Regression test for FSharp1.0:4472
// Title: Problem with Expr_tchoose and initialization graphs
// Compile with reference to FSharp.PowerPack

let rec all_integers_starting_from n = LazyList.consf n (fun _ -> all_integers_starting_from (n+1))
and combine x1 x2 = 1
and next_primes x = failwith ""
and primes = 
    let twos = all_integers_starting_from 3
    LazyList.consf 2 (fun _ -> next_primes twos)

exit 0
