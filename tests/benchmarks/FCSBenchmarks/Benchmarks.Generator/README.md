# Benchmarks.Generator

## What is it
A command-line app for generating and running benchmarks of FCS using high-level analysis definitions

## How it works
### Dependency graph
```mermaid
graph LR;
    subgraph Generation
        A1(Ionide.ProjInfo.FCS) --> A2(Benchmarks.Generator)
        A3 --> A1
        A3(FSharp.Compiler.Service NuGet) --> A2
        style A3 fill:Blue
    end

    A2 -.->|JSON| R2
    
    subgraph Running
        R1(FSharp.Compiler.Service source) --> R2(Benchmarks.Runner)
        style R1 fill:green
    end
```
### Process steps graph
```mermaid
graph TD;
    AA(description.json)
    AA-->A
    AB(GitHub/Git server)
    AB-->B
    subgraph Benchmarks.Generator
        A(Codebase spec and analysis actions)-->|libgit2sharp| B(Locally checked out codebase);
        B-->|Ionide.ProjInfo.FCS| C(FSharpProjectOptions)
        A-->D(BenchmarkSpec)
        C-->D
        D-->E(JSON-friendly DTO)
    end
    E-->|Newtonsoft.Json|F(FCS inputs.json)
    F-->|Newtonsoft.Json|G(JSON-friendly DTO)
    subgraph Benchmarks.Runner
        G-->H(BenchmarkSpec' - separate type)
        J(FSharp.Compiler.Service source)-->K(FSharp.Compiler.Service dll)
        H-->K
    end
```
## How to use it
### Run `Benchmark.Generator` to generate and run the benchmark
```bash
dotnet run -i inputs/50_leaves.json 
```

## Benchmark description format
The benchmark description is a high-level definition of code analysis that we want to benchmark. It consists of two parts:
- a codebase to be analysed
- specific analysis actions (eg. analyse file `A.fs` in project `B`)

[inputs/](inputs/) directory contains existing samples.

Let's look at [inputs/fantomas.json](inputs/fantomas.json):
```json5
// Checkout a revision of Fantomas codebase and perform a single analysis action on the top-level file
{
  // Repository to checkout as input for code analysis benchmarking
  "Repo": {
    // Short name used for determining local checkout directory
    "Name": "fantomas",
    // Full URL to a publicy-available Git repository
    "GitUrl": "https://github.com/fsprojects/fantomas",
    // Revision to use for 'git checkout' - using a commit hash rather than a branch/tag name guarantees reproducability
    "Revision" : "0fe6785076e045f28e4c88e6a57dd09b649ce671"
  },
  // Commands to run to prepare a checked out codebase for `dotnet run`
  "CodebasePrep": [
    {
      "Command": "dotnet",
      "Args": "tool restore"
    },
    {
      "Command": "dotnet",
      "Args": "paket restore"
    }
  ],
  // Solution to open relative to the repo's root - all projects in the solution will be available in action definitions below
  "SlnRelative": "fantomas.sln",
  // A sequence of actions to be performed by FCS on the above codebase
  "CheckActions": [
    // Analyse DaemonTests.fs in the project named Fantomas.Tests
    {
      "FileName": "Integration/DaemonTests.fs",
      // This is a project name only - not project file's path (we currently assume names are unique)
      "ProjectName": "Fantomas.Tests"
    }
  ]
}
```
#### Local codebase
For local testing only a local codebase can be used instead of a publicly available GitHub repo.

Since the specification provides no guarantees about the local codebase's contents, this mode should not be used for comparing results between machines/users (even if the same code is available locally on multiple machines).

See an example from [inputs/local_example.json](inputs/local_example.json): 
```json5
{
  // Path to the locally available codebase
  "LocalCodeRoot": "Path/To/Local/Code/Root",
  // The rest of the options are the same
  "SlnRelative": "solution.sln",
  "CheckActions": [
    {
      "FileName": "library.fs",
      "ProjectName": "root"
    }
  ]
}
```

## Known issues
* Benchmark assumes that the FSharp.Compiler.Service has already been built
* Only `Release` configuration is supported
* Currently BenchmarkDotNet is not used - a single iteration of the listed actions is performed
* Little customization of the actual benchmark's environment
* Console output could be improved
* Depends on having the correct MSBuild/Dotnet setup available