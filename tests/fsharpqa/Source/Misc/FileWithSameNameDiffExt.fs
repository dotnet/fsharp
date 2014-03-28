// #Regression #Misc 
// Regression test for FSHARP1.0:5268
namespace N
module M =
    let x = 1

    (if x = 1 then 0 else 1) |> exit
