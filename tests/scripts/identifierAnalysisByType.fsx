// Print some stats about some very very basic code formatting conventions

#r "nuget: Ionide.ProjInfo"

#I @"..\..\artifacts\bin\fsc\Debug\net6.0\"
#r "FSharp.Compiler.Service.dll"


open System
open System.IO
open Ionide.ProjInfo
open System.Runtime.InteropServices
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open Ionide.ProjInfo.Types

let isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)

let dotnet =
    if isWindows then
        "dotnet.exe"
    else
        "dotnet"
let fileExists pathToFile =
    try
        File.Exists(pathToFile)
    with _ ->
        false
// Look for global install of dotnet sdk
let getDotnetGlobalHostPath () =
    let pf = Environment.GetEnvironmentVariable("ProgramW6432")

    let pf =
        if String.IsNullOrEmpty(pf) then
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
        else
            pf

    let candidate = Path.Combine(pf, "dotnet", dotnet)

    if fileExists candidate then
        Some candidate
    else
        // Can't find it --- give up
        None

let getDotnetHostPath () =
    let probePathForDotnetHost () =
        let paths =
            let p = Environment.GetEnvironmentVariable("PATH")

            if not (isNull p) then
                p.Split(Path.PathSeparator)
            else
                [||]

        paths |> Array.tryFind (fun f -> fileExists (Path.Combine(f, dotnet)))

    match (Environment.GetEnvironmentVariable("DOTNET_HOST_PATH")) with
    // Value set externally
    | value when not (String.IsNullOrEmpty(value)) && fileExists value -> Some value
    | _ ->
        // Probe for netsdk install, dotnet. and dotnet.exe is a constant offset from the location of System.Int32
        let candidate =
            let assemblyLocation = Path.GetDirectoryName(typeof<Int32>.Assembly.Location)
            Path.GetFullPath(Path.Combine(assemblyLocation, "..", "..", "..", dotnet))

        if fileExists candidate then
            Some candidate
        else
            match probePathForDotnetHost () with
            | Some f -> Some(Path.Combine(f, dotnet))
            | None -> getDotnetGlobalHostPath ()

let cwd = System.Environment.CurrentDirectory |> System.IO.DirectoryInfo
let dotnetExe = getDotnetHostPath () |> Option.map System.IO.FileInfo
let _toolsPath = Init.init cwd dotnetExe

let projectFile = Path.Combine(__SOURCE_DIRECTORY__, @"..\..\src\Compiler\FSharp.Compiler.Service.fsproj")
//let projectFile = Path.Combine(__SOURCE_DIRECTORY__, @"..\..\src\FSharp.Build\FSharp.Build.fsproj")
let opts = ProjectLoader.getProjectInfo projectFile [] BinaryLogGeneration.Off []

let opts2 = 
    match opts with 
    | Result.Ok res -> res
    | Result.Error err -> failwithf "%s" err

opts2.OtherOptions


let checker = FSharpChecker.Create()

let opts3 = checker.GetProjectOptionsFromCommandLineArgs(projectFile, [| yield! opts2.SourceFiles; yield! opts2.OtherOptions  |] )


let results = checker.ParseAndCheckProject(opts3)
let results2 = results |> Async.RunSynchronously

let symbols = results2.GetAllUsesOfAllSymbols()

let rec stripTy (ty: FSharpType) = 
    if ty.IsAbbreviation then stripTy ty.AbbreviatedType else ty


let getText (sym: FSharpMemberOrFunctionOrValue) =
    let ty = stripTy sym.FullType
    FSharpType.Prettify(ty).Format(FSharpDisplayContext.Empty)

symbols
|> Array.choose (fun s -> match s.Symbol with :? FSharpMemberOrFunctionOrValue as v -> Some v | _ -> None)
|> Array.filter (fun v -> v.GenericParameters.Count = 0)
|> Array.filter (fun v -> v.CurriedParameterGroups.Count = 0)
|> Array.filter (fun v -> not v.FullType.IsGenericParameter)
|> Array.map (fun v -> getText v, v)
|> Array.filter (fun (s, v) -> 
    match s with 
    | "System.String" -> false
    | "System.Boolean" -> false
    | "System.Int32" -> false
    | "System.Int64" -> false
    | "System.Object" -> false
    | "Microsoft.FSharp.Collections.List<Microsoft.FSharp.Core.string>" -> false
    | "Microsoft.FSharp.Core.Option<Microsoft.FSharp.Core.string>" -> false
    | _ when s.EndsWith(" Microsoft.FSharp.Core.[]") -> false // for now filter array types
    | _ when v.DisplayName.StartsWith "_" -> false
    | _ -> true)
|> Array.groupBy fst
|> Array.map (fun (key, g) ->
    key, 
    (g 
     |> Array.map snd 
     |> Array.groupBy (fun v -> v.DisplayName)
     |> Array.sortByDescending (snd >> Array.length)))
|> Array.filter (fun (_, g) -> g.Length > 1)
|> Array.sortByDescending (fun (key, g) -> Array.length g)
|> Array.iter (fun (key, g) -> 
    printfn "Type: %s" key
    for (nm, entries) in g do
       printfn "    %s (%d times)" nm (Array.length entries))

