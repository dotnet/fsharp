// #Regression #NoMono #NoMT #CodeGen #EmittedIL 
// Regression test for FSHARP1.0:4237
// F# boxes structs when calling, say, Object.ToString() and interface methods. C# does not
// ToString
module Program
let F<'T >(x : 'T) = x.ToString()
