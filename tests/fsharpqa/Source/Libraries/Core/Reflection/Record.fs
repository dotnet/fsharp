// #Regression #Libraries #Reflection 
// Regression test for FSHARP1.0:5113

type MyR = {c:int;b:int;a:int}

let s1 = sprintf "%A" {a=1;b=2;c=3}
let s2 = sprintf "%A" {c=3;b=2;a=1}

(if s1 = s2 then 0 else 1) |> exit

