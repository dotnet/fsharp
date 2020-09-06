module Pos39

module BigAnonRecords4 = 
    let v = Map [ {| A = ""; B = ""; C = ""; D = ""; |}, () ]
    v.TryFind {| A = ""; B = ""; C = ""; D = ""; |} |> printfn "%A"

module BigAnonRecords5 = 
    let v = Map [ {| A = ""; B = ""; C = ""; D = ""; E = "" ;|}, () ]
    v.TryFind {| A = ""; B = ""; C = ""; D = ""; E = "" |} |> printfn "%A"

module BigAnonRecords6 = 
    let v = Map [ {| A = ""; B = ""; C = ""; D = ""; E = "" ; F = ""|}, () ]
    v.TryFind {| A = ""; B = ""; C = ""; D = ""; E = ""; F = "" |} |> printfn "%A"

module BigAnonRecords7 = 
    let v = Map [ {| A = ""; B = ""; C = ""; D = ""; E = "" ; F = ""; G = "" |}, () ]
    v.TryFind {| A = ""; B = ""; C = ""; D = ""; E = ""; F = ""; G = "" |} |> printfn "%A"

module BigAnonRecords8 = 
    let v = Map [ {| A = ""; B = ""; C = ""; D = ""; E = "" ; F = ""; G = ""; H = 1 |}, ()
                  {| A = ""; B = ""; C = ""; D = ""; E = "" ; F = ""; G = ""; H = 2 |}, ()]
    v.TryFind {| A = ""; B = ""; C = ""; D = ""; E = ""; F = ""; G = ""; H = 2 |} |> printfn "%A"
    v.TryFind {| A = ""; B = ""; C = ""; D = ""; E = ""; F = ""; G = ""; H = 1 |} |> printfn "%A"

module BigAnonRecords9 = 
    let v = Map [ {| A = ""; B = ""; C = ""; D = ""; E = "" ; F = ""; G = ""; H = 1; I = 3.0M |}, ()
                  {| A = ""; B = ""; C = ""; D = ""; E = "" ; F = ""; G = ""; H = 2; I = 3.0M |}, ()]
    v.TryFind {| A = ""; B = ""; C = ""; D = ""; E = ""; F = ""; G = ""; H = 2; I = 3.0M |} |> printfn "%A"
    v.TryFind {| A = ""; B = ""; C = ""; D = ""; E = ""; F = ""; G = ""; H = 1; I = 3.0M |} |> printfn "%A"

