module Pos39

module BigAnonRecords4 = 
    let v = Map [ {| A = ""; B = ""; C = ""; D = ""; |}, () ]
    v.TryFind {| A = ""; B = ""; C = ""; D = ""; |} |> sprintf "%A" |> fun s -> if s <> "Some ()" then failwith "unexpected"

module BigAnonRecords5 = 
    let v = Map [ {| A = ""; B = ""; C = ""; D = ""; E = "" ;|}, () ]
    v.TryFind {| A = ""; B = ""; C = ""; D = ""; E = "" |} |> sprintf "%A" |> fun s -> if s <> "Some ()" then failwith "unexpected"

module BigAnonRecords6 = 
    let v = Map [ {| A = ""; B = ""; C = ""; D = ""; E = "" ; F = ""|}, () ]
    v.TryFind {| A = ""; B = ""; C = ""; D = ""; E = ""; F = "" |} |> sprintf "%A" |> fun s -> if s <> "Some ()" then failwith "unexpected"

module BigAnonRecords7 = 
    let v = Map [ {| A = ""; B = ""; C = ""; D = ""; E = "" ; F = ""; G = "" |}, () ]
    v.TryFind {| A = ""; B = ""; C = ""; D = ""; E = ""; F = ""; G = "" |} |> sprintf "%A" |> fun s -> if s <> "Some ()" then failwith "unexpected"

module BigAnonRecords8 = 
    let v = Map [ {| A = ""; B = ""; C = ""; D = ""; E = "" ; F = ""; G = ""; H = 1 |}, ()
                  {| A = ""; B = ""; C = ""; D = ""; E = "" ; F = ""; G = ""; H = 2 |}, ()]
    v.TryFind {| A = ""; B = ""; C = ""; D = ""; E = ""; F = ""; G = ""; H = 2 |} |> sprintf "%A" |> fun s -> if s <> "Some ()" then failwith "unexpected"
    v.TryFind {| A = ""; B = ""; C = ""; D = ""; E = ""; F = ""; G = ""; H = 1 |} |> sprintf "%A" |> fun s -> if s <> "Some ()" then failwith "unexpected"

module BigAnonRecords9 = 
    let v = Map [ {| A = ""; B = ""; C = ""; D = ""; E = "" ; F = ""; G = ""; H = 1; I = 3.0M |}, 1
                  {| A = ""; B = ""; C = ""; D = ""; E = "" ; F = ""; G = ""; H = 2; I = 3.0M |}, 2]
    v.TryFind {| A = ""; B = ""; C = ""; D = ""; E = ""; F = ""; G = ""; H = 2; I = 3.0M |} |> sprintf "%A" |> fun s -> if s <> "Some 2" then failwith "unexpected"
    v.TryFind {| A = ""; B = ""; C = ""; D = ""; E = ""; F = ""; G = ""; H = 1; I = 3.0M |} |> sprintf "%A" |> fun s -> if s <> "Some 1" then failwith "unexpected"

printfn "test completed"
exit 0