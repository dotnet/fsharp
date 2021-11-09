// #Regression #NoMT #CompilerOptions 
// Regression test for FSHARP1.0:5976

// System.Func<...> is in System.Core.dll (NetFx3.5)

//<Expects status="success">val f: d: System\.Func<int> -> int</Expects>
//<Expects status="success">val it: int = 11</Expects>

let f ( d : System.Func<int> ) = d.Invoke() + 1;;

f ( new System.Func<_>(fun _ -> 10) );;

exit 0;;

#q;;


