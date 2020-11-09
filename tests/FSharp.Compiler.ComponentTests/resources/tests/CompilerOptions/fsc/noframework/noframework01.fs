// #Regression #NoMT #CompilerOptions 
// Regression test for FSHARP1.0:5976

// System.Func<...> is in System.Core.dll (NetFx3.5)

let f ( d : System.Func<int> ) = d.Invoke() + 1;;

if f ( new System.Func<_>(fun _ -> 10) ) <> 11 then exit 1

exit 0

