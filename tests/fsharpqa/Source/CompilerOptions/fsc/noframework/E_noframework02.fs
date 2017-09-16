// #Regression #NoMT #CompilerOptions  
// Regression test for FSHARP1.0:5976
// On .NET 2.0 System.Core is not in the default reference set
//<Expects status="error" id="FS0039" span="(10,20-10,24)">The type 'Func' is not defined</Expects>
//<Expects status="error" id="FS0072" span="(10,34-10,42)">Lookup on object of indeterminate type based on information prior to this program point\. A type annotation may be needed prior to this program point to constrain the type of the object\. This may allow the lookup to be resolved\.</Expects>
//<Expects status="error" id="FS0039" span="(12,19-12,23)">The type 'Func' is not defined</Expects>

// System.Func<...> is in System.Core.dll (NetFx3.5)

let f ( d : System.Func<int> ) = d.Invoke() + 1;;

if f ( new System.Func<_>(fun _ -> 10) ) <> 11 then exit 1

exit 0
