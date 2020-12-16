// #Regression #NoMT #CompilerOptions 
// See DevDiv:364238
//<Expects status="warning" span="(8,14)" id="FS0052">The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed$</Expects>
//<Expects status="warning" span="(11,14)" id="FS0052">The value has been copied to ensure the original is not mutated by this operation or because the copy is implicit when returning a struct from a member and another member is then accessed$</Expects>
open System.Collections.Generic

let x : IEnumerator<KeyValuePair<int, int>> = failwith ""
printfn "%A" x.Current.Key // defensive copy

let y : list<KeyValuePair<int, int>> = failwith ""
printfn "%A" y.[0].Key // defensive copy
