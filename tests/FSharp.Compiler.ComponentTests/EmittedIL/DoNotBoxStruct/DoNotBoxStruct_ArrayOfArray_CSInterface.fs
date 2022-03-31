// #Regression #NoMono #NoMT #CodeGen #EmittedIL 
// Regression test for FSHARP1.0:4237
// F# boxes structs when calling, say, Object.ToString() and interface methods. C# does not
// Array of array 
module Program
let F<'T when 'T :> System.IDisposable>(x : 'T[][]) = x.[0].[0].Dispose()
