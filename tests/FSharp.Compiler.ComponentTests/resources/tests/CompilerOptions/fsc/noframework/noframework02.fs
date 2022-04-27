// #Regression #NoMT #CompilerOptions   
// Regression test for FSHARP1.0:5976
// See FSHARP1.0:6181 - it is no longer an error to specify --noframework and not specify -r to mscorlib/fscore
//<Expects status="success"></Expects>

// System.Func<...> is in System.Core.dll (NetFx3.5)

open System

let f ( d : System.Func<int> ) = d.Invoke() + 1;;

if f ( new System.Func<_>(fun _ -> 10) ) <> 11 then raise (new Exception("if f ( new System.Func<_>(fun _ -> 10) ) <> 11"))

printfn "Finished"
