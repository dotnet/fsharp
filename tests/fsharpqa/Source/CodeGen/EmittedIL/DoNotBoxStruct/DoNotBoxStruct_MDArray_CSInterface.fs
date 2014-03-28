// #Regression #NoMono #NoMT #CodeGen #EmittedIL #NETFX20Only #NETFX40Only 
// Regression test for FSHARP1.0:4237
// F# boxes structs when calling, say, Object.ToString() and interface methods. C# does not
// Multi-dimentional array
let F<'T when 'T :> System.IDisposable>(x : 'T[,]) = x.[0,0].Dispose()
