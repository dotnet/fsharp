// Print some stats about identifiers grouped by type
//

#r "nuget: Ionide.ProjInfo, 0.57.2"
#I @"..\..\artifacts\bin\fsc\Debug\net7.0\"
#r "FSharp.Compiler.Service.dll"

open System
open System.IO
open Ionide.ProjInfo
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open Ionide.ProjInfo.Types

let argv = fsi.CommandLineArgs

if argv.Length = 1 then
    eprintfn "usage:"
    eprintfn "    dotnet fsi tests/scripts/identifierAnalysisByType.fsx <project-file>"
    eprintfn ""
    eprintfn "examples:"
    eprintfn "    dotnet artifacts\bin\fsi\Debug\net7.0\fsi.dll tests/scripts/identifierAnalysisByType.fsx src/FSharp.Build/FSharp.Build.fsproj"
    eprintfn "    dotnet artifacts\bin\fsi\Debug\net7.0\fsi.dll tests/scripts/identifierAnalysisByType.fsx src/Compiler/FSharp.Compiler.Service.fsproj"
    eprintfn ""
    eprintfn "Sample output is at https://gist.github.com/dsyme/abfa11bebf0713251418906d55c08804"

//let projectFile = Path.Combine(__SOURCE_DIRECTORY__, @"..\..\src\Compiler\FSharp.Compiler.Service.fsproj")
//let projectFile = Path.Combine(__SOURCE_DIRECTORY__, @"..\..\src\FSharp.Build\FSharp.Build.fsproj")
let projectFile = Path.GetFullPath(argv[1])

let cwd = System.Environment.CurrentDirectory |> System.IO.DirectoryInfo

let _toolsPath = Init.init cwd None

printfn "Cracking project options...."
let opts =
    match ProjectLoader.getProjectInfo projectFile [("TargetFramework", "net472")] BinaryLogGeneration.Off [] with 
    | Result.Ok res -> res
    | Result.Error err -> failwithf "%s" err

let checker = FSharpChecker.Create()

let options = [| yield! opts.SourceFiles; yield! opts.OtherOptions  |]
for pi in opts.Items do
    printfn "pi:    %s" (pi.GetType().ToString())

for rp in opts.ReferencedProjects do
    printfn "rp:    %s" rp.RelativePath

for rp in opts.PackageReferences do
    printfn "pr:    %s" rp.FullPath

for opt in options do
    printfn "    %s" opt

let checkerOpts = checker.GetProjectOptionsFromCommandLineArgs(projectFile, options)

printfn "Checking project...."
let results = checker.ParseAndCheckProject(checkerOpts) |> Async.RunSynchronously

printfn "Grouping symbol uses...."
let symbols = results.GetAllUsesOfAllSymbols()

let rec stripTy (ty: FSharpType) = 
    if ty.IsAbbreviation then stripTy ty.AbbreviatedType else ty

let getTypeText (sym: FSharpMemberOrFunctionOrValue) =
    let ty = stripTy sym.FullType
    FSharpType.Prettify(ty).Format(FSharpDisplayContext.Empty)

symbols
|> Array.choose (fun vUse -> match vUse.Symbol with :? FSharpMemberOrFunctionOrValue as v -> Some (v, vUse.Range) | _ -> None)
|> Array.filter (fun (v, _) -> v.GenericParameters.Count = 0)
|> Array.filter (fun (v, _) -> v.CurriedParameterGroups.Count = 0)
|> Array.filter (fun (v, _) -> not v.FullType.IsGenericParameter)
|> Array.map (fun (v, vUse) -> getTypeText v, v, vUse.ToString())
|> Array.filter (fun (vTypeText, v, _) -> 
    match vTypeText with 
    | "System.String" -> false
    | "System.Boolean" -> false
    | "System.Int32" -> false
    | "System.Int64" -> false
    | "System.Object" -> false
    | "Microsoft.FSharp.Collections.List<Microsoft.FSharp.Core.string>" -> false
    | "Microsoft.FSharp.Core.Option<Microsoft.FSharp.Core.string>" -> false
    | s when s.EndsWith(" Microsoft.FSharp.Core.[]") -> false // for now filter array types
    | _ when v.DisplayName.StartsWith "_" -> false
    | _ -> true)
|> Array.groupBy (fun (vTypeText, _, _) -> vTypeText)
|> Array.map (fun (key, g) ->
    key, 
    (g 
     |> Array.distinctBy (fun (_, _, vUse) -> vUse)
     |> Array.groupBy (fun (_, v, _) -> v.DisplayName)
     |> Array.sortByDescending (snd >> Array.length)))
|> Array.filter (fun (_, g) -> g.Length > 1)
|> Array.sortByDescending (fun (key, g) -> Array.length g)
|> Array.iter (fun (key, g) -> 
    let key = key.Replace("Microsoft.FSharp", "FSharp").Replace("FSharp.Core.", "")
    printfn "Type: %s" key
    for (nm, entries) in g do
       printfn "    %s (%d times)" nm (Array.length entries)
       for (_, _, vUse) in entries do
           printfn "        %s" vUse
    printfn "")
