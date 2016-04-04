// #Regression #NoMT #CompilerOptions 
// Regression test for FSHARP1.0:6108

module M

let ff () =
    match 1 with
    | 0 -> ()

