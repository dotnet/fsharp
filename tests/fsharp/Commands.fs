[<RequireQualifiedAccess>]
module Commands

open System
open System.IO

open PlatformHelpers

let getfullpath workDir path =
    let rooted =
        if Path.IsPathRooted(path) then path
        else Path.Combine(workDir, path)
    rooted |> Path.GetFullPath

let fileExists workDir path = 
    if path |> getfullpath workDir |> File.Exists then Some path else None

let directoryExists workDir path = 
    if path |> getfullpath workDir |> Directory.Exists then Some path else None

/// copy /y %source1% tmptest2.ml
let copy_y workDir source to' = 
    log "copy /y %s %s" source to'
    File.Copy( source |> getfullpath workDir, to' |> getfullpath workDir, true)
    CmdResult.Success

/// mkdir orig
let mkdir_p workDir dir =
    log "mkdir %s" dir
    Directory.CreateDirectory ( Path.Combine(workDir, dir) ) |> ignore

/// del test.txt
let rm dir path =
    log "rm %s" path
    let p = path |> getfullpath dir
    if File.Exists(p) then File.Delete(p)

let pathAddBackslash (p: FilePath) = 
    if String.IsNullOrWhiteSpace (p) 
    then p
    else
        p.TrimEnd ([| Path.DirectorySeparatorChar; Path.AltDirectorySeparatorChar |]) 
        + Path.DirectorySeparatorChar.ToString()

// echo. > build.ok
let ``echo._tofile`` workDir text p =
    log "echo.%s> %s" text p
    let to' = p |> getfullpath workDir in File.WriteAllText(to', text + Environment.NewLine)

/// echo // empty file  > tmptest2.mli
let echo_tofile workDir text p =
    log "echo %s> %s" text p
    let to' = p |> getfullpath workDir in File.WriteAllText(to', text + Environment.NewLine)

/// echo // empty file  >> tmptest2.mli
let echo_append_tofile workDir text p =
    log "echo %s> %s" text p
    let to' = p |> getfullpath workDir in File.AppendAllText(to', text + Environment.NewLine)

/// type %source1%  >> tmptest3.ml
let type_append_tofile workDir source p =
    log "type %s >> %s" source p
    let from = source |> getfullpath workDir
    let to' = p |> getfullpath workDir
    let contents = File.ReadAllText(from)
    File.AppendAllText(to', contents)

// %GACUTIL% /if %BINDIR%\FSharp.Core.dll
let gacutil exec exeName flags assembly =
    exec exeName (sprintf """%s "%s" """ flags assembly)

// "%NGEN32%" install "%BINDIR%\fsc.exe" /queue:1
// "%NGEN32%" install "%BINDIR%\fsi.exe" /queue:1
// "%NGEN32%" install "%BINDIR%\FSharp.Build.dll" /queue:1
// "%NGEN32%" executeQueuedItems 1
let ngen exec (ngenExe: FilePath) assemblies =
    let queue = assemblies |> List.map (fun a -> (sprintf "install \"%s\" /queue:1" a))

    List.concat [ queue; ["executeQueuedItems 1"] ]
    |> Seq.ofList
    |> Seq.map (fun args -> exec ngenExe args)
    |> Seq.skipWhile (function ErrorLevel _ -> false | CmdResult.Success -> true)
    |> Seq.tryHead
    |> function None -> CmdResult.Success | Some res -> res

let fsc exec (fscExe: FilePath) flags srcFiles =
    // "%FSC%" %fsc_flags% --define:COMPILING_WITH_EMPTY_SIGNATURE -o:tmptest2.exe tmptest2.mli tmptest2.ml
    exec fscExe (sprintf "%s %s" flags (srcFiles |> Seq.ofList |> String.concat " "))

let csc exec cscExe flags srcFiles =
    exec cscExe (sprintf "%s %s"  flags (srcFiles |> Seq.ofList |> String.concat " "))

let fsi exec fsiExe flags sources =
    exec fsiExe (sprintf "%s %s" flags (sources |> Seq.ofList |> String.concat " "))

// "%MSBUILDTOOLSPATH%\msbuild.exe" PCL.fsproj
let msbuild exec msbuildExe flags srcFiles =
    exec msbuildExe (sprintf "%s %s"  flags (srcFiles |> Seq.ofList |> String.concat " "))

// "%RESGEN%" /compile Resources.resx
let resgen exec resgenExe flags sources =
    exec resgenExe (sprintf "%s %s" flags (sources |> Seq.ofList |> String.concat " "))

let internal quotepath (p: FilePath) =
    let quote = '"'.ToString()
    if p.Contains(" ") 
    then (sprintf "%s%s%s" quote p quote)
    else p

let ildasm exec ildasmExe flags assembly =
    exec ildasmExe (sprintf "%s %s" flags (quotepath assembly))

let peverify exec peverifyExe flags path =
    exec peverifyExe (sprintf "%s %s" (quotepath path) flags)

let createTempDir () =
    let path = Path.GetTempFileName ()
    File.Delete path
    Directory.CreateDirectory path |> ignore
    path

let convertToShortPath path =
    log "convert to short path %s" path
    let result = ref None
    let lastLine = function null -> () | l -> result := Some l

    let cmdArgs = { RedirectOutput = Some lastLine; RedirectError = None; RedirectInput = None }
    
    let args = sprintf """/c for /f "delims=" %%I in ("%s") do echo %%~dfsI""" path

    match Process.exec cmdArgs (Path.GetTempPath()) Map.empty "cmd.exe" args with
    | ErrorLevel _ -> path
    | Ok -> match !result with None -> path | Some p -> p

let where envVars cmd =
    log "where %s" cmd
    let result = ref None
    let lastLine = function null -> () | l -> result := Some l

    let cmdArgs = { RedirectOutput = Some lastLine; RedirectError = None; RedirectInput = None; }

    match Process.exec cmdArgs (Path.GetTempPath()) envVars "cmd.exe" (sprintf "/c where %s" cmd) with
    | ErrorLevel _ -> None
    | CmdResult.Success -> !result    

let fsdiff exec fsdiffExe file1 file2 =
    // %FSDIFF% %testname%.err %testname%.bsl
    exec fsdiffExe (sprintf "%s %s normalize" file1 file2)

let ``for /f`` path = 
    // FOR /F processing of a text file consists of reading the file, one line of text at a time and then breaking the line up into individual
    // items of data called 'tokens'. The DO command is then executed with the parameter(s) set to the token(s) found.
    // By default, /F breaks up the line at each blank space " ", and any blank lines are skipped, this default parsing behavior can be changed 
    // by applying one or more of the "options" parameters. The option(s) must be contained within "a pair of quotes"
    let splitLines lines =
        lines
        |> Array.filter (fun l -> not <| String.IsNullOrWhiteSpace(l))
        |> Array.collect (fun l -> l.Split([| ' ' |], StringSplitOptions.RemoveEmptyEntries))
        |> List.ofArray

    File.ReadAllLines (path) |> splitLines
