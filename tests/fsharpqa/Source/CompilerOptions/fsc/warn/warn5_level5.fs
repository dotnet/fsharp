// #Regression #NoMT #CompilerOptions 
// See DevDiv:364238
//<Expects status="error" span="(8,14)" id="FS0052">The value has been copied to ensure the original is not mutated by this operation$</Expects>
//<Expects status="error" span="(11,14)" id="FS0052">The value has been copied to ensure the original is not mutated by this operation$</Expects>
open System.Collections.Generic

let x : IEnumerator<KeyValuePair<int, int>> = failwith ""
printfn "%A" x.Current.Key // defensive copy

let y : list<KeyValuePair<int, int>> = failwith ""
printfn "%A" y.[0].Key // defensive copy
