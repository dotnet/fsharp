# `dotnet fsi` Dependency Manager Plugins

Since .NET 5.0, `dotnet fsi` ships with dependency manager plugins support that can be called like so:

```fsharp
#r "myextension: my extension parameters"
```

# `#r "nuget:"` [nuget](https://github.com/dotnet/fsharp/tree/main/src/fsharp/FSharp.DependencyManager.Nuget)

Reference nuget packages, ships by default with `dotnet fsi`.

```fsharp
#r "nuget: Newtonsoft.Json"
// Optionally, specify a version explicitly
// #r "nuget: Newtonsoft.Json,11.0.1"

open Newtonsoft.Json

let o = {| X = 2; Y = "Hello" |}

printfn "%s" (JsonConvert.SerializeObject o)
```

# `#r "paket:"` [paket](https://fsprojects.github.io/Paket/fsi-integration.html)

Reference dependencies (nuget, git, gist, github) through [Paket package manager](https://fsprojects.github.io/Paket).

Learn how to use [Paket FSI integration](https://fsprojects.github.io/Paket/fsi-integration.html).

```fsharp
#r "paket: nuget FSharp.Data"

open FSharp.Data

type MyCsv = CsvProvider<"""
X,Y
2,Hello
4,World
""">

for r in MyCsv.GetSample().Rows do
  printfn "%i = %s" r.X r.Y
```
