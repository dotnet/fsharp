// #Regression #NoMono #NoMT #CodeGen #EmittedIL   
// Regression test for FSHARP1.0:4237
// F# boxes structs when calling, say, Object.ToString() and interface methods. C# does not
// Array of array - use an F# interface (IEvent<'a> for example)
let F<'T when 'T :> IEvent<int>>(x : 'T[][]) = x.[0].[0].Add(fun x -> ())
