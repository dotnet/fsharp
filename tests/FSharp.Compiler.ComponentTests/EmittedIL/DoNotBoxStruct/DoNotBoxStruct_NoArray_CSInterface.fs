// #Regression #NoMono #NoMT #CodeGen #EmittedIL 
// Regression test for FSHARP1.0:4237
// F# boxes structs when calling, say, Object.ToString() and interface methods. C# does not
// Non-array
module Program
let F<'T when 'T :> System.IDisposable>(x : 'T) = x.Dispose()
