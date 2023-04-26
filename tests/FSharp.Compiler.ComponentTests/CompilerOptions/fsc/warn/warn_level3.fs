// #Regression #NoMT #CompilerOptions 
//
//

open System.Collections.Generic

let x : IEnumerator<KeyValuePair<int, int>> = failwith ""
printfn "%A" x.Current.Key // defensive copy

let y : list<KeyValuePair<int, int>> = failwith ""
printfn "%A" y.[0].Key // defensive copy
