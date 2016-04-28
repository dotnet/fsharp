#r @"..\..\..\..\..\packages\FSharp.Data.2.2.5\lib\net40\FSharp.Data.dll"

open System.Diagnostics
open System.IO
open FSharp.Data
open FSharp.Data.JsonExtensions 

type AssemblyReferenceType = | forBuild = 0 | forExecute = 1

// Try head was introduced in F# 4.0
let tryHead (source : seq<_>) =
    let checkNonNull argName arg = 
        match box arg with 
        | null -> nullArg argName 
        | _ -> ()
    checkNonNull "source" source
    use e = source.GetEnumerator() 
    if (e.MoveNext()) then Some e.Current
    else None

let Arguments = fsi.CommandLineArgs |> Seq.skip 1

let GetArgumentFromCommandLine switchName defaultValue = 
    match Arguments |> Seq.filter(fun t -> t.StartsWith(switchName)) |> Seq.map(fun t -> t.Remove(0, switchName.Length).Trim()) |> tryHead with
    | Some(file) -> if file.Length <> 0 then file else defaultValue
    | _ -> defaultValue

let ProjectJson  = GetArgumentFromCommandLine       "--projectJson:"        @"tests\fsharp\project.json"
let configFile   = GetArgumentFromCommandLine       "--nugetConfig:"        @".nuget\nuget.config"
let ProjectJsonLock = GetArgumentFromCommandLine    "--projectJsonLock:"    @"tests\fsharp\project.lock.json"
let PackagesDir  = GetArgumentFromCommandLine       "--packagesDir:"        @"packages"
let TargetPlatformName = GetArgumentFromCommandLine "--targetPlatformName:" @"DNXCore,Version=v5.0/win7-x64"
let FSharpCore   = GetArgumentFromCommandLine       "--fsharpCore:"         @"Release\coreclr\bin\fsharp.core.dll"
let Output       = GetArgumentFromCommandLine       "--output:"             @"."
let NugetPath    = GetArgumentFromCommandLine       "--nugetPath:"          @".nuget\nuget.exe"
let Verbosity    = GetArgumentFromCommandLine       "--v:"                  @"quiet"
let CopyCompiler = GetArgumentFromCommandLine       "--copyCompiler:"       @"no"

let FSharpCompilerFiles =
    let FSharpCoreDir = Path.GetDirectoryName(FSharpCore)
    seq {
        yield Path.Combine(FSharpCoreDir, "fsc.exe")
        yield Path.Combine(FSharpCoreDir, "FSharp.Compiler.dll")
        yield Path.Combine(FSharpCoreDir, "fsharp.core.sigdata")
        yield Path.Combine(FSharpCoreDir, "fsharp.core.optdata")
        yield Path.Combine(FSharpCoreDir, "default.win32manifest")
        yield Path.Combine(FSharpCoreDir, "fsi.exe")
        yield Path.Combine(FSharpCoreDir, "FSharp.Compiler.Interactive.Settings.dll")
    }

let isVerbose = Verbosity = "verbose"

// Utility functions
let copyFile source dir =
    let dest = 
        if not (Directory.Exists(dir)) then Directory.CreateDirectory(dir) |>ignore
        let result = Path.Combine(dir, Path.GetFileName(source))
        result
    if isVerbose then
        printfn "source: %s" source
        printfn "dest:   %s" dest
    File.Copy(source, dest, true)

let deleteDirectory (output) =
    if (Directory.Exists(output)) then Directory.Delete(output, true) |>ignore
    ()

let makeDirectory (output) =
    if not (Directory.Exists(output)) then Directory.CreateDirectory(output) |>ignore
    ()

let executeProcess filename arguments =
    let processWriteMessage (chan:TextWriter) (message:string) =
        match message with
        | null -> ()
        | _ as m -> chan.WriteLine(m) |>ignore
    let info = new ProcessStartInfo()
    let p = new Process()
    if isVerbose then printfn "%s %s" filename arguments
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
let _ =
    let arguments = "restore -configFile "+ configFile + " -PackagesDirectory " + PackagesDir + " " + ProjectJson
    executeProcess NugetPath arguments

let setPathSeperators (path:string) = path.Replace('/', '\\')

let splitNameAndVersion (ref:string) =
    let elements = ref.Split [| '/' |]
    if elements.Length >= 2 then
        Some(elements.[0], elements.[1])
    else
        None

let collectReferenciesFromProjectJson lockFile assemblyReferenceType = 
    let getAssemblyReferenciesFromTargets (targets:JsonValue) =
        let getReferencedFiles (referencedFiles:JsonValue) =
            seq {
                for path, _ in referencedFiles.Properties do
                    let path = setPathSeperators path
                    if Path.GetFileName(path) = "_._" then ()
                    else yield setPathSeperators path
            }
        let buildReferencePaths name version paths =
            seq {
                for path in paths do
                    yield sprintf @"%s\%s\%s\%s" PackagesDir name version path
            }
        if isVerbose then 
            printfn "lockFile:           %A" lockFile
            printfn "TargetPlatformName: %A" TargetPlatformName
            printfn "PackagesDir:        %A" PackagesDir
        seq {
            let target = targets.TryGetProperty(TargetPlatformName)
            match target with 
            | Some(t) ->
                for ref, value in  t.Properties do
                    match splitNameAndVersion ref with
                    | Some(name, version) -> 
                        if isVerbose then
                            printfn "name:              %A" name
                            printfn "version:           %A" version
                        if assemblyReferenceType = AssemblyReferenceType.forBuild then 
                            match value.TryGetProperty("compile") with
                            | None -> ()
                            | Some x -> yield! buildReferencePaths name version (getReferencedFiles x)
                        else 
                            match value.TryGetProperty("runtime") with
                            | None -> ()
                            | Some x -> yield! buildReferencePaths name version (getReferencedFiles value?runtime)
                            match value.TryGetProperty("native") with
                            | None -> ()
                            | Some x -> yield! buildReferencePaths name version (getReferencedFiles value?native)
                    | _ -> ()
            | _  -> ()
        }

    let getReferencesFromJson (filename:string) =
        let projectJson = JsonValue.Load( filename )
        getAssemblyReferenciesFromTargets projectJson?targets
    (getReferencesFromJson lockFile) |> Seq.distinct

let dependencies = (collectReferenciesFromProjectJson ProjectJsonLock AssemblyReferenceType.forExecute)

//Okay copy everything
makeDirectory(Output)
copyFile FSharpCore Output
dependencies |> Seq.iter(fun source -> copyFile source Output)
if CopyCompiler = "yes" then 
    FSharpCompilerFiles |> Seq.iter(fun source -> copyFile source Output)
