#!/usr/bin/env dotnet fsi
// Generates F# xUnit test projects for overload resolution performance testing
// Usage: dotnet fsi PerfTestGenerator.fsx --total 100 [--untyped|--typed]

open System
open System.IO

type Config = { Total: int; Methods: int; Typed: bool; Output: string }

let types = [| "int"; "string"; "float"; "bool"; "int64"; "decimal"; "byte"; "char" |]

let literal t i =
    match t with
    | "int" -> string i | "string" -> $"\"{i}\"" | "float" -> $"{i}.0"
    | "bool" -> if i % 2 = 0 then "true" else "false"
    | "int64" -> $"{i}L" | "decimal" -> $"{i}M"
    | "byte" -> $"{i % 256}uy" | "char" -> $"'%c{char ((i % 26) + 97)}'"
    | _ -> string i

let genAssert typed i =
    let t = types.[i % types.Length]
    if typed then $"        Assert.Equal<{t}>({literal t i}, {literal t i})"
    else $"        Assert.Equal({literal t i}, {literal t i})"

let genTestFile cfg =
    let perMethod = cfg.Total / cfg.Methods
    let sb = Text.StringBuilder()
    sb.AppendLine("namespace XUnitPerfTest\n\nopen Xunit\n\ntype Tests() =") |> ignore
    for m in 0 .. cfg.Methods - 1 do
        sb.AppendLine($"    [<Fact>]\n    member _.``Test {m}``() =") |> ignore
        for a in 0 .. perMethod - 1 do
            sb.AppendLine(genAssert cfg.Typed (m * perMethod + a)) |> ignore
        sb.AppendLine() |> ignore
    sb.ToString()

let fsproj = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
    <UseArtifactsOutput>false</UseArtifactsOutput>
  </PropertyGroup>
  <ItemGroup><Compile Include="Tests.fs" /></ItemGroup>
  <ItemGroup>
    <PackageReference Include="xunit" Version="2.4.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.5">
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
</Project>"""

let readme cfg =
    $"# XUnit Performance Test\n\n- Asserts: {cfg.Total}\n- Methods: {cfg.Methods}\n- Typed: {cfg.Typed}\n\n```bash\ndotnet build\n```"

let generate cfg =
    let name = if cfg.Typed then "XUnitPerfTest.Typed" else "XUnitPerfTest.Untyped"
    let dir = Path.Combine(cfg.Output, name)
    Directory.CreateDirectory(dir) |> ignore
    File.WriteAllText(Path.Combine(dir, "Tests.fs"), genTestFile cfg)
    File.WriteAllText(Path.Combine(dir, $"{name}.fsproj"), fsproj)
    File.WriteAllText(Path.Combine(dir, "README.md"), readme cfg)
    printfn "Generated: %s" dir
    dir

let parseArgs (args: string[]) =
    let mutable total, methods, typed, output = 1500, 10, false, "./generated"
    let mutable i = 0
    while i < args.Length do
        match args.[i] with
        | "--total" -> total <- int args.[i+1]; i <- i + 2
        | "--methods" -> methods <- int args.[i+1]; i <- i + 2
        | "--output" -> output <- args.[i+1]; i <- i + 2
        | "--typed" -> typed <- true; i <- i + 1
        | "--untyped" -> typed <- false; i <- i + 1
        | "--help" -> printfn "Usage: dotnet fsi PerfTestGenerator.fsx --total N [--untyped|--typed] [--methods N] [--output DIR]"; exit 0
        | _ -> printfn "Unknown: %s" args.[i]; exit 1
    { Total = total; Methods = methods; Typed = typed; Output = output }

try
    let cfg = parseArgs fsi.CommandLineArgs.[1..]
    generate cfg |> ignore
with ex -> printfn "Error: %s" ex.Message; exit 1
