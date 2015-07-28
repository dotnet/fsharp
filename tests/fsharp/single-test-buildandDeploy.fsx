#r @"..\..\packages\FSharp.Data.2.2.5\lib\net40\FSharp.Data.dll"

open System
open System.Collections.Generic
open System.Diagnostics
open System.Globalization
open System.IO
open System.Linq
open FSharp.Data
open FSharp.Data.JsonExtensions 

let Arguments = fsi.CommandLineArgs |> Seq.skip 1
let Sources = Arguments             |> Seq.filter(fun t -> printfn "%s" t; t.StartsWith("--source:")) |> Seq.map(fun t -> t.Remove(0, 9).Trim()) |> Seq.distinct
let Defines = Arguments             |> Seq.filter(fun t -> printfn "%s" t; t.StartsWith("--define:")) |> Seq.map(fun t -> t.Remove(0, 9).Trim())

let GetArgumentFromCommandLine switchName defaultValue = 
    match Arguments |> Seq.filter(fun t -> t.StartsWith(switchName)) |> Seq.map(fun t -> t.Remove(0, switchName.Length).Trim()) |> Seq.tryHead with
    | Some(file) -> if file.Length <> 0 then file else defaultValue
    | _ -> defaultValue

let ProjectJsonFile = GetArgumentFromCommandLine "--projectJson:" "project.json.lock"
let LocalProjectJsonFile = GetArgumentFromCommandLine "--lclProjectJson:" "project.json.lock"
let PackagesDir = GetArgumentFromCommandLine "--packagesDir:" "."
let TargetPlatformName = GetArgumentFromCommandLine "--targetPlatformName:" "DNXCore,Version=v5.0"
let Output = GetArgumentFromCommandLine "--output:" @"output"
let FSharpCore = GetArgumentFromCommandLine "--fsharpCore:" "fsharp.core.dll was not specified"
let NugetProjectJson = GetArgumentFromCommandLine "--nugetProjectJson:" "project.json was not specified"
let NugetSources = (GetArgumentFromCommandLine "--nugetSources:" "").Split([|';'|]) |> Seq.fold(fun acc src -> acc + " -s:" + src) ""
let DnuPath = GetArgumentFromCommandLine "--dnuPath:" "..\..\packages\Microsoft.DotNet.BuildTools.1.0.25-prerelease-00053\lib\dnu.cmd"
let FscPath = GetArgumentFromCommandLine "--fscPath:" "Fsc Path not specified"
let TestPlatform = GetArgumentFromCommandLine "--testPlatform:" "Test Platform not specified"
let TestDirectory = GetArgumentFromCommandLine "--testDirectory:" "Test Directory not specified"

let copyFile source dir =
    let dest = 
        if not (Directory.Exists(dir)) then Directory.CreateDirectory(dir) |>ignore
        Path.Combine(dir, Path.GetFileName(source))
    File.Copy(source, dest, true)

let deletePreviousOutput () =
    if (Directory.Exists(TestDirectory)) then Directory.Delete(TestDirectory, true) |>ignore
    ()

let makeOutputDirectory () =
    let dir = Path.GetDirectoryName(Output)
    if not (Directory.Exists(dir)) then Directory.CreateDirectory(dir) |>ignore
    ()

let executeProcess filename arguments =
    let processWriteMessage (chan:TextWriter) (message:string) =
        match message with
        | null -> ()
        | _ as m -> chan.WriteLine(m) |>ignore
    let info = new ProcessStartInfo()
    let p = new Process()
    info.Arguments <- arguments
    info.UseShellExecute <- false
    info.RedirectStandardOutput <- true
    info.RedirectStandardError <- true
    info.CreateNoWindow <- true
    info.FileName <- filename
    p.StartInfo <- info
    p.OutputDataReceived.Add(fun x -> processWriteMessage stdout x.Data)
    p.ErrorDataReceived.Add(fun x ->  processWriteMessage stderr x.Data)
    if p.Start() then
        p.BeginOutputReadLine()
        p.BeginErrorReadLine()
        p.WaitForExit()
        p.ExitCode
    else
        0

let executeCompiler sources references =
    let listToPrefixedSpaceSeperatedString prefix list = list |> Seq.fold(fun a t -> sprintf "%s %s%s" a prefix t) ""
    let listToSpaceSeperatedString list = list |> Seq.fold(fun a t -> sprintf "%s %s" a t) ""
    let addReferenceSwitch list = list |> Seq.map(fun i -> sprintf "--reference:%s" i)
    let arguments = sprintf @"--out:%s --define:BASIC_TEST --targetprofile:netcore --target:exe -g --times --noframework %s -r:%s %s %s" (Output) (listToSpaceSeperatedString (addReferenceSwitch references)) (FSharpCore) (listToPrefixedSpaceSeperatedString "--define:" Defines) (listToSpaceSeperatedString sources)
    printfn "%s %s" FscPath arguments
    executeProcess FscPath arguments

let restorePackages () =
    let arguments = "restore " + "--packages " + PackagesDir + " " + NugetSources + " " + NugetProjectJson
    printfn "%s %s" DnuPath arguments
    executeProcess DnuPath arguments

restorePackages()

type AssemblyReferenceType = | forBuild = 0 | forExecute = 1

let setPathSeperators (path:string) = path.Replace('/', '\\')

let splitNameAndVersion (ref:string) =
    let elements = ref.Split [| '/' |]
    if elements.Length >= 2 then
        Some(elements.[0], elements.[1])
    else
        None

let collectReferenciesFromProjectJson assemblyReferenceType = 
    let getAssemblyReferenciesFromTargets (targets:JsonValue) =
        let getReferencedFiles (referencedFiles:JsonValue) =
            seq {
                for path, _ in referencedFiles.Properties do
                    yield setPathSeperators path
            }
        let buildReferencePaths name version paths =
            seq {
                for path in paths do
                    yield sprintf @"%s\%s\%s\%s" PackagesDir name version path
            }
        seq {
            let target = targets.TryGetProperty(TargetPlatformName)
            match target with 
            | Some(t) ->
                for ref, value in  t.Properties do
                    match splitNameAndVersion ref with
                    | Some(name,version) -> 
                        if assemblyReferenceType = AssemblyReferenceType.forBuild then 
                            match value.TryGetProperty("compile") with
                            | None -> ()
                            | Some x -> yield! buildReferencePaths name version (getReferencedFiles x)
                        else 
                            match value.TryGetProperty("runtime") with
                            | None -> ()
                            | Some x -> yield! buildReferencePaths name version (getReferencedFiles value?runtime)
                    | _ -> ()
            | _  -> ()
        }

    let getReferencesFromJson (filename:string) =
        let projectJson = JsonValue.Load( filename )
        getAssemblyReferenciesFromTargets projectJson?targets
    (Seq.append (getReferencesFromJson ProjectJsonFile) (getReferencesFromJson LocalProjectJsonFile)) |> Seq.distinct

let getNativeFiles package =
    let packageVersion =
        try
            let pv = Directory.EnumerateDirectories(sprintf @"%s\%s" PackagesDir package) |> Seq.sortDescending |> Seq.head
            Some(pv)
        with e -> None
    seq {
        match packageVersion with
        | None -> ()
        | Some p ->
            let path = sprintf @"%s\runtimes\%s\native" p TestPlatform
            yield!
                try  Directory.EnumerateFiles(path)
                with e -> Enumerable.Empty()
    }

let runtimefiles = 
    seq { 
        yield! getNativeFiles "Microsoft.NETCore.Runtime.CoreCLR-x86"
        yield! getNativeFiles "Microsoft.NETCore.ConsoleHost-x86"
        yield! getNativeFiles "Microsoft.NETCore.TestHost-x86"
        yield! getNativeFiles "Microsoft.NETCore.Windows.ApiSets-x86"
    }

let dependencies = (collectReferenciesFromProjectJson AssemblyReferenceType.forExecute)

deletePreviousOutput ()
makeOutputDirectory ()
let ec = executeCompiler Sources (collectReferenciesFromProjectJson AssemblyReferenceType.forBuild)
if ec > 0 then 
    exit ec
else
    copyFile FSharpCore TestDirectory
    dependencies |> Seq.iter(fun source -> copyFile source TestDirectory)
    runtimefiles |> Seq.iter(fun source -> copyFile source TestDirectory)
    exit 0
