// #Regression #NoMT #CompilerOptions 
// See DevDiv:364238
open System.Collections.Generic

[<Struct>]
type NonReadOnlyStruct=
    member val Property = "" with get, set

let x : IEnumerator<NonReadOnlyStruct> = failwith ""
printfn "%A" x.Current.Property // defensive copy

let y : list<NonReadOnlyStruct> = failwith ""  // KeyValuePair<int, int>
printfn "%A" y.[0].Property // defensive copy
