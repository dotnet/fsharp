// #Regression #Libraries #Reflection 
// Regression test for FSHARP1.0:5113

type MyT = MyC of int * string * bool

let DU = MyC (1,"2",true)

(if (sprintf "%A" DU) = "MyC (1,\"2\",true)" then 0 else 1) |> exit
