<!-- this content is pointed at by url https://aka.ms/dotnetdepmanager please keep the F# team in the loop to update the redirect if the file is moved -->

# `dotnet fsi` Dependency Manager Plugins

Since .NET 5.0, `dotnet fsi` ships with dependency manager plugins support that can be called like so:

```fsharp
#r "myextension: my extension parameters"
```

The initial [RFC](https://github.com/fsharp/fslang-design/blob/main/tooling/FST-1027-fsi-references.md) for this feature overviews how it is designed.

The current implementation follows pattern that can be deducted by refering to [implementation in DependencyProvider.fs](https://github.com/dotnet/fsharp/blob/b9687a58cee795a94eb88cf84e309767cc25f6cb/src/Compiler/DependencyManager/DependencyProvider.fs#L145-L322); due to the system working without having a staticically linked dependency, it uses a late binding approach leveraging runtime reflection to identify if an extension conforms to patterns understood by the specifics of the compiler implementation.


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