# `dotnet fsi`: NuGet Dependency Manager Plugin

This extension ships by default since .NET 5. It can be used in F# interactive through `dotnet fsi` without further setup through `#r "nuget:"` references.

```fsharp
#r "nuget: Newtonsoft.Json"
// Optionally, specify a version explicitly
// #r "nuget: Newtonsoft.Json,11.0.1"

open Newtonsoft.Json

let o = {| X = 2; Y = "Hello" |}

printfn "%s" (JsonConvert.SerializeObject o)"
```
