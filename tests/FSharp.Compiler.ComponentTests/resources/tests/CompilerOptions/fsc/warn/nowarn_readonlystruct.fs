// #Regression #NoMT #CompilerOptions 
// See DevDiv:364238
open System.Collections.Generic

let x : IEnumerator<KeyValuePair<int, int>> = failwith ""
printfn "%A" x.Current.Key // no defensive copy needed, because KeyValuePair is a "readonly struct"

let y : list<KeyValuePair<int, int>> = failwith ""  // KeyValuePair<int, int>
printfn "%A" y.[0].Key // no defensive copy needed, because KeyValuePair is a "readonly struct"
