// #Regression #NoMono #NoMT #CodeGen #EmittedIL   
// Regression test for FSHARP1.0:4237
// F# boxes structs when calling, say, Object.ToString() and interface methods. C# does not
// Multi-dimentional array - use an F# interface (IEvent<'a> for example)
// Note: the 'AddHandler' is some non-extension method
module Program
let F<'T when 'T :> IEvent<int>>(x : 'T[,]) = x.[0,0].AddHandler(fun x _ -> ())
