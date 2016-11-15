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

let copy_y workDir source dest = 
    log "copy /y %s %s" source dest
    File.Copy( source |> getfullpath workDir, dest |> getfullpath workDir, true)
    CmdResult.Success

let mkdir_p workDir dir =
    log "mkdir %s" dir
    Directory.CreateDirectory ( Path.Combine(workDir, dir) ) |> ignore

let rm dir path =
    log "rm %s" path
    let p = path |> getfullpath dir
    if File.Exists(p) then File.Delete(p)

let pathAddBackslash (p: FilePath) = 
    if String.IsNullOrWhiteSpace (p) then p
    else
        p.TrimEnd ([| Path.DirectorySeparatorChar; Path.AltDirectorySeparatorChar |]) 
        + Path.DirectorySeparatorChar.ToString()

let echoAppendToFile workDir text p =
    log "echo %s> %s" text p
    let dest = p |> getfullpath workDir in File.AppendAllText(dest, text + Environment.NewLine)

let appendToFile workDir source p =
    log "type %s >> %s" source p
    let from = source |> getfullpath workDir
    let dest = p |> getfullpath workDir
    let contents = File.ReadAllText(from)
    File.AppendAllText(dest, contents)

let fsc workDir exec (fscExe: FilePath) flags srcFiles =
    let args = (sprintf "%s %s" flags (srcFiles |> Seq.ofList |> String.concat " "))
#if FSC_IN_PROCESS
    let fscCompiler = FSharp.Compiler.Hosted.FscCompiler()
    let exitCode, _stdin, _stdout = FSharp.Compiler.Hosted.CompilerHelpers.fscCompile workDir (FSharp.Compiler.Hosted.CompilerHelpers.parseCommandLine args)

    match exitCode with
    | 0 -> CmdResult.Success
    | err -> 
        let msg = sprintf "Error running command '%s' with args '%s' in directory '%s'" fscExe args workDir 
        CmdResult.ErrorLevel (msg, err)
#else
    ignore workDir 
    exec fscExe args
#endif

let csc exec cscExe flags srcFiles =
    exec cscExe (sprintf "%s %s"  flags (srcFiles |> Seq.ofList |> String.concat " "))

let fsi exec fsiExe flags sources =
    exec fsiExe (sprintf "%s %s"  flags (sources |> Seq.ofList |> String.concat " "))

let internal quotepath (p: FilePath) =
    let quote = '"'.ToString()
    if p.Contains(" ") then (sprintf "%s%s%s" quote p quote) else p

let ildasm exec ildasmExe flags assembly =
    exec ildasmExe (sprintf "%s %s" flags (quotepath assembly))

let peverify exec peverifyExe flags path =
    exec peverifyExe (sprintf "%s %s" (quotepath path) flags)

let createTempDir () =
    let path = Path.GetTempFileName ()
    File.Delete path
    Directory.CreateDirectory path |> ignore
    path

let fsdiff exec fsdiffExe file1 file2 =
    // %FSDIFF% %testname%.err %testname%.bsl
    exec fsdiffExe (sprintf "%s %s normalize" file1 file2)

