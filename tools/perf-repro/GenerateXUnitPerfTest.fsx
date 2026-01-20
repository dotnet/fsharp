#!/usr/bin/env dotnet fsi

// Generate F# test projects with xUnit Assert.Equal calls for performance testing
// This script creates both untyped (slow) and typed (fast) versions for comparison

open System
open System.IO

type TestConfig =
    { TotalAsserts: int
      MethodsCount: int
      AssertsPerMethod: int
      OutputDir: string
      ProjectName: string
      UseTypedAsserts: bool }

// Helper function to generate random test data calls
let generateTestDataCall primitiveType index =
    match primitiveType with
    | "int" -> sprintf "generateRandomInt(%d)" index
    | "string" -> sprintf "generateRandomString(%d)" index
    | "float" -> sprintf "generateRandomFloat(%d)" index
    | "bool" -> sprintf "generateRandomBool(%d)" index
    | "int64" -> sprintf "generateRandomInt64(%d)" index
    | "decimal" -> sprintf "generateRandomDecimal(%d)" index
    | "byte" -> sprintf "generateRandomByte(%d)" index
    | "char" -> sprintf "generateRandomChar(%d)" index
    | _ -> sprintf "generateRandomInt(%d)" index

// Generate expected value based on type
let generateExpectedValue primitiveType index =
    match primitiveType with
    | "int" -> sprintf "%d" index
    | "string" -> sprintf "\"test%d\"" index
    | "float" -> sprintf "%d.0" index
    | "bool" -> if index % 2 = 0 then "true" else "false"
    | "int64" -> sprintf "%dL" index
    | "decimal" -> sprintf "%dM" index
    | "byte" -> sprintf "%duy" (index % 256)
    | "char" -> sprintf "'%c'" (char ((index % 26) + 97))
    | _ -> sprintf "%d" index

// Generate Assert.Equal call
let generateAssertEqual primitiveType index useTyped =
    let expected = generateExpectedValue primitiveType index
    let actual = generateTestDataCall primitiveType index

    if useTyped then
        sprintf "        Assert.Equal<%s>(%s, %s)" primitiveType expected actual
    else
        sprintf "        Assert.Equal(%s, %s)" expected actual

// Generate helper functions for test data
let generateHelperFunctions () =
    """    // Helper functions to generate test data
    let generateRandomInt(seed: int) = seed
    let generateRandomString(seed: int) = sprintf "test%d" seed
    let generateRandomFloat(seed: int) = float seed
    let generateRandomBool(seed: int) = seed % 2 = 0
    let generateRandomInt64(seed: int) = int64 seed
    let generateRandomDecimal(seed: int) = decimal seed
    let generateRandomByte(seed: int) = byte (seed % 256)
    let generateRandomChar(seed: int) = char ((seed % 26) + 97)"""

// Generate a single test method
let generateTestMethod methodIndex assertsPerMethod useTyped =
    let sb = System.Text.StringBuilder()

    sb.AppendLine(sprintf "    [<Fact>]") |> ignore

    sb.AppendLine(sprintf "    member this.``Test Method %d``() =" methodIndex)
    |> ignore

    // Use different primitive types in rotation
    let primitiveTypes =
        [| "int"; "string"; "float"; "bool"; "int64"; "decimal"; "byte"; "char" |]

    for i in 0 .. assertsPerMethod - 1 do
        let typeIndex = i % primitiveTypes.Length
        let primitiveType = primitiveTypes.[typeIndex]
        let globalIndex = methodIndex * assertsPerMethod + i
        sb.AppendLine(generateAssertEqual primitiveType globalIndex useTyped) |> ignore

    sb.ToString()

// Generate complete test file
let generateTestFile config =
    let sb = System.Text.StringBuilder()

    // File header
    sb.AppendLine("namespace XUnitPerfTest") |> ignore
    sb.AppendLine() |> ignore
    sb.AppendLine("open Xunit") |> ignore
    sb.AppendLine() |> ignore

    // Test class
    sb.AppendLine("type Tests() =") |> ignore
    sb.AppendLine() |> ignore

    // Helper functions
    sb.AppendLine(generateHelperFunctions ()) |> ignore
    sb.AppendLine() |> ignore

    // Generate test methods
    for methodIndex in 0 .. config.MethodsCount - 1 do
        sb.AppendLine(generateTestMethod methodIndex config.AssertsPerMethod config.UseTypedAsserts)
        |> ignore

    sb.ToString()

// Generate .fsproj file
let generateProjectFile projectName =
    sprintf
        """<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <GenerateProgramFile>false</GenerateProgramFile>
    <IsTestProject>true</IsTestProject>
    <!-- Use local output directories to avoid conflicts with repo build -->
    <OutputPath>bin\$(Configuration)</OutputPath>
    <BaseIntermediateOutputPath>obj\</BaseIntermediateOutputPath>
    <UseArtifactsOutput>false</UseArtifactsOutput>
    <ArtifactsPath>$(MSBuildProjectDirectory)\artifacts</ArtifactsPath>
  </PropertyGroup>

  <ItemGroup>
    <Compile Include="Tests.fs" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="xunit" Version="2.4.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.4.5">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>

</Project>
"""

// Generate README for the generated project
let generateReadme config =
    let typeAnnotation =
        if config.UseTypedAsserts then
            "Yes (fast path)"
        else
            "No (slow path)"

    sprintf
        "# XUnit Performance Test Project\n\n\
This project was auto-generated to test F# compiler performance with xUnit Assert.Equal calls.\n\n\
## Configuration\n\
- Total Assert.Equal calls: %d\n\
- Test methods: %d\n\
- Asserts per method: %d\n\
- Type annotations: %s\n\n\
## Building\n\
```bash\n\
dotnet restore\n\
dotnet build\n\
```\n\n\
## Note\n\
This project is for compiler performance testing only.\n\
The tests themselves are not meaningful - they are designed to stress the F# compiler's\n\
overload resolution mechanism with many untyped Assert.Equal calls.\n"
        config.TotalAsserts
        config.MethodsCount
        config.AssertsPerMethod
        typeAnnotation

// Main generation function
let generateTestProject config =
    printfn "Generating test project: %s" config.ProjectName
    printfn "  Total asserts: %d" config.TotalAsserts
    printfn "  Methods: %d" config.MethodsCount
    printfn "  Asserts per method: %d" config.AssertsPerMethod
    printfn "  Typed asserts: %b" config.UseTypedAsserts

    // Create output directory
    let projectDir = Path.Combine(config.OutputDir, config.ProjectName)
    Directory.CreateDirectory(projectDir) |> ignore

    // Generate and write test file
    let testContent = generateTestFile config
    File.WriteAllText(Path.Combine(projectDir, "Tests.fs"), testContent)
    printfn "  Generated: Tests.fs"

    // Generate and write project file
    let projContent = generateProjectFile config.ProjectName
    File.WriteAllText(Path.Combine(projectDir, config.ProjectName + ".fsproj"), projContent)
    printfn "  Generated: %s.fsproj" config.ProjectName

    // Generate README
    let readmeContent = generateReadme config
    File.WriteAllText(Path.Combine(projectDir, "README.md"), readmeContent)
    printfn "  Generated: README.md"

    printfn "Project generated successfully at: %s" projectDir
    projectDir

// CLI interface
let printUsage () =
    printfn
        """
Usage: dotnet fsi GenerateXUnitPerfTest.fsx [options]

Options:
  --total <n>        Total number of Assert.Equal calls (default: 1500)
  --methods <n>      Number of test methods (default: 10)
  --output <path>    Output directory (default: ./generated)
  --typed            Generate typed Assert.Equal calls (fast path)
  --untyped          Generate untyped Assert.Equal calls (slow path, default)
  --help             Show this help message

Examples:
  # Generate untyped version (slow path) with 1500 asserts
  dotnet fsi GenerateXUnitPerfTest.fsx --total 1500 --untyped

  # Generate typed version (fast path) with 1500 asserts
  dotnet fsi GenerateXUnitPerfTest.fsx --total 1500 --typed
"""

// Parse command line arguments
let parseArgs (args: string[]) =
    let mutable totalAsserts = 1500
    let mutable methodsCount = 10
    let mutable outputDir = "./generated"
    let mutable useTyped = false
    let mutable i = 0

    while i < args.Length do
        match args.[i] with
        | "--total" when i + 1 < args.Length ->
            totalAsserts <- Int32.Parse(args.[i + 1])
            i <- i + 2
        | "--methods" when i + 1 < args.Length ->
            methodsCount <- Int32.Parse(args.[i + 1])
            i <- i + 2
        | "--output" when i + 1 < args.Length ->
            outputDir <- args.[i + 1]
            i <- i + 2
        | "--typed" ->
            useTyped <- true
            i <- i + 1
        | "--untyped" ->
            useTyped <- false
            i <- i + 1
        | "--help" ->
            printUsage ()
            exit 0
        | _ ->
            printfn "Unknown argument: %s" args.[i]
            printUsage ()
            exit 1

    let assertsPerMethod = totalAsserts / methodsCount

    let projectName =
        if useTyped then
            "XUnitPerfTest.Typed"
        else
            "XUnitPerfTest.Untyped"

    { TotalAsserts = totalAsserts
      MethodsCount = methodsCount
      AssertsPerMethod = assertsPerMethod
      OutputDir = outputDir
      ProjectName = projectName
      UseTypedAsserts = useTyped }

// Main entry point
let main (args: string[]) =
    try
        if args.Length = 0 || args |> Array.contains "--help" then
            printUsage ()
            0
        else
            let config = parseArgs args
            generateTestProject config |> ignore
            0
    with ex ->
        printfn "Error: %s" ex.Message
        printfn "%s" ex.StackTrace
        1

// Execute if running as script
let exitCode = main fsi.CommandLineArgs.[1..]
exit exitCode
