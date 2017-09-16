// #Regression #NoMT #CompilerOptions  
// Regression test for FSHARP1.0:5976
// On .NET 2.0 System.Core is not in the default reference set
//<Expects status="error" id="FS0039" span="(8,20-8,24)">The type 'Func' is not defined</Expects>

// System.Func<...> is in System.Core.dll (NetFx3.5)

let f ( d : System.Func<int> ) = d.Invoke() + 1;;

if f ( new System.Func<_>(fun _ -> 10) ) <> 11 then exit 1

exit 0;;

#q;;


