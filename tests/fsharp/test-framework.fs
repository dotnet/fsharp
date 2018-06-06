module TestFramework

open Microsoft.Win32
open System
open System.IO
open System.Text.RegularExpressions
open Scripting
open NUnit.Framework


[<RequireQualifiedAccess>]
module Commands =

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
        let p = path |> getfullpath dir
        if File.Exists(p) then 
            (log "rm %s" p) |> ignore
            File.Delete(p)
        else
            (log "not found: %s p") |> ignore

    let rmdir dir path =
        let p = path |> getfullpath dir
        if Directory.Exists(p) then 
            (log "rmdir /sy %s" p) |> ignore
            Directory.Delete(p, true)
        else
            (log "not found: %s p") |> ignore

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

    let fsc workDir exec (dotNetExe: FilePath) (fscExe: FilePath) flags srcFiles =
        let args = (sprintf "%s %s" flags (srcFiles |> Seq.ofList |> String.concat " "))

#if FSC_IN_PROCESS
        // This is not yet complete
        let fscCompiler = FSharp.Compiler.Hosted.FscCompiler()
        let exitCode, _stdin, _stdout = FSharp.Compiler.Hosted.CompilerHelpers.fscCompile workDir (FSharp.Compiler.Hosted.CompilerHelpers.parseCommandLine args)

        match exitCode with
        | 0 -> CmdResult.Success
        | err -> 
            let msg = sprintf "Error running command '%s' with args '%s' in directory '%s'" fscExe args workDir 
            CmdResult.ErrorLevel (msg, err)
#else
        ignore workDir 
#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
        ignore dotNetExe
        exec fscExe args
#else
        exec dotNetExe (fscExe + " " + args)
#endif
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


type TestConfig = 
    { EnvironmentVariables : Map<string, string>
      CSC : string
      csc_flags : string
      BUILD_CONFIG : string
      FSC : string
      fsc_flags : string
      FSCBinPath : string
      FSCOREDLLPATH : string
      FSI : string
      FSIANYCPU : string
      FSI_FOR_SCRIPTS : string
      fsi_flags : string
      ILDASM : string
      PEVERIFY : string
      Directory: string 
      DotNetExe: string
      DefaultPlatform: string}


module WindowsPlatform = 
    let Is64BitOperatingSystem envVars =
        // On Windows PROCESSOR_ARCHITECTURE has the value AMD64 on 64 bit Intel Machines
        let value =
            let find s = envVars |> Map.tryFind s
            [| "PROCESSOR_ARCHITECTURE" |] |> Seq.tryPick (fun s -> find s) |> function None -> "" | Some x -> x
        value = "AMD64"

type FSLibPaths = 
    { FSCOREDLLPATH : string }

let requireFile nm = 
    if Commands.fileExists __SOURCE_DIRECTORY__ nm |> Option.isSome then nm else failwith (sprintf "couldn't find %s" nm)

let config configurationName envVars =

    let SCRIPT_ROOT = __SOURCE_DIRECTORY__
    let packagesDir = SCRIPT_ROOT ++ ".." ++ ".." ++ "packages"
    let FSCBinPath = SCRIPT_ROOT ++ ".." ++ ".." ++ configurationName ++ "net40" ++ "bin"
    let csc_flags = "/nologo" 
    let fsc_flags = "-r:System.Core.dll --nowarn:20 --define:COMPILED"
    let fsi_flags = "-r:System.Core.dll --nowarn:20 --define:INTERACTIVE --maxerrors:1 --abortonerror"
    let Is64BitOperatingSystem = WindowsPlatform.Is64BitOperatingSystem envVars
    let architectureMoniker = if Is64BitOperatingSystem then "x64" else "x86"
    let CSC = requireFile (packagesDir ++ "Microsoft.Net.Compilers.2.7.0" ++ "tools" ++ "csc.exe")
    let ILDASM = requireFile (packagesDir ++ ("runtime.win-" + architectureMoniker + ".Microsoft.NETCore.ILDAsm.2.0.3") ++ "runtimes" ++ ("win-" + architectureMoniker) ++ "native" ++ "ildasm.exe")
    let coreclrdll = requireFile (packagesDir ++ ("runtime.win-" + architectureMoniker + ".Microsoft.NETCore.Runtime.CoreCLR.2.0.3") ++ "runtimes" ++ ("win-" + architectureMoniker) ++ "native" ++ "coreclr.dll")
    let PEVERIFY = requireFile (SCRIPT_ROOT ++ ".." ++ "fsharpqa" ++ "testenv" ++ "src" ++ "PEVerify" ++ "bin" ++ configurationName ++ "net46" ++ "PEVerify.exe")
    let FSI_FOR_SCRIPTS =
        match envVars |> Map.tryFind "_fsiexe" with
        | Some fsiexe when (not (String.IsNullOrWhiteSpace fsiexe)) -> requireFile (SCRIPT_ROOT ++ ".." ++ ".." ++ (fsiexe.Trim([| '\"' |])))
        | _ ->
            // build.cmd sets that var, if it is not set, we are probably called directly from visual studio or the nunit console runner.
            let fsharpCompilerTools = Directory.GetDirectories(packagesDir, "FSharp.Compiler.Tools.*")
            match fsharpCompilerTools with
            | [||] -> failwithf "Could not find any 'FSharp.Compiler.Tools' inside '%s'" packagesDir
            | [| dir |] -> Path.Combine(dir, "tools", "fsi.exe")
            | _ -> failwithf "Found more than one 'FSharp.Compiler.Tools' inside '%s', please clean up." packagesDir
    let toolsDir = SCRIPT_ROOT ++ ".." ++ ".." ++ "Tools"
    let dotNetExe = toolsDir ++ "dotnetcli" ++ "dotnet.exe"
    // ildasm requires coreclr.dll to run which has already been restored to the packages directory
    File.Copy(coreclrdll, Path.GetDirectoryName(ILDASM) ++ "coreclr.dll", overwrite=true)

#if !FSHARP_SUITE_DRIVES_CORECLR_TESTS
    let FSI = requireFile (FSCBinPath ++ "fsi.exe")
    let FSIANYCPU = requireFile (FSCBinPath ++ "fsiAnyCpu.exe")
    let FSC = requireFile (FSCBinPath ++ "fsc.exe")
    let FSCOREDLLPATH = requireFile (FSCBinPath ++ "FSharp.Core.dll") 
#else
    let FSI = SCRIPT_ROOT ++ ".." ++ ".." ++ "tests" ++ "testbin" ++ configurationName ++ "coreclr" ++ "FSC" ++ "fsi.exe"
    let FSIANYCPU = SCRIPT_ROOT ++ ".." ++ ".." ++ "tests" ++ "testbin" ++ configurationName ++ "coreclr" ++ "FSC" ++ "fsiAnyCpu.exe"
    let FSC = SCRIPT_ROOT ++ ".." ++ ".." ++ "tests" ++ "testbin" ++ configurationName ++ "coreclr" ++ "FSC" ++ "fsc.exe"
    let FSCOREDLLPATH = "" 
#endif

    let defaultPlatform = 
        match Is64BitOperatingSystem with 
//        | PlatformID.MacOSX, true -> "osx.10.10-x64"
//        | PlatformID.Unix,true -> "ubuntu.14.04-x64"
        | true -> "win7-x64"
        | false -> "win7-x86"

    { EnvironmentVariables = envVars
      FSCBinPath = FSCBinPath |> Commands.pathAddBackslash
      FSCOREDLLPATH = FSCOREDLLPATH
      ILDASM = ILDASM
      PEVERIFY = PEVERIFY
      CSC = CSC 
      BUILD_CONFIG = configurationName
      FSC = FSC
      FSI = FSI
      FSIANYCPU = FSIANYCPU
      FSI_FOR_SCRIPTS = FSI_FOR_SCRIPTS
      csc_flags = csc_flags
      fsc_flags = fsc_flags 
      fsi_flags = fsi_flags 
      Directory="" 
      DotNetExe = dotNetExe
      DefaultPlatform = defaultPlatform }

let logConfig (cfg: TestConfig) =
    log "---------------------------------------------------------------"
    log "Executables"
    log ""
    log "CSC                 =%s" cfg.CSC
    log "BUILD_CONFIG        =%s" cfg.BUILD_CONFIG
    log "csc_flags           =%s" cfg.csc_flags
    log "FSC                 =%s" cfg.FSC
    log "fsc_flags           =%s" cfg.fsc_flags
    log "FSCBINPATH          =%s" cfg.FSCBinPath
    log "FSCOREDLLPATH       =%s" cfg.FSCOREDLLPATH
    log "FSI                 =%s" cfg.FSI
    log "FSIANYCPU                 =%s" cfg.FSIANYCPU
    log "fsi_flags           =%s" cfg.fsi_flags
    log "ILDASM              =%s" cfg.ILDASM
    log "PEVERIFY            =%s" cfg.PEVERIFY
    log "---------------------------------------------------------------"


let checkResult result = 
    match result with
    | CmdResult.ErrorLevel (msg1, err) -> Assert.Fail (sprintf "%s. ERRORLEVEL %d" msg1 err)
    | CmdResult.Success -> ()

let checkErrorLevel1 result = 
    match result with
    | CmdResult.ErrorLevel (_,1) -> ()
    | CmdResult.Success | CmdResult.ErrorLevel _ -> Assert.Fail (sprintf "Command passed unexpectedly")

let envVars () = 
    System.Environment.GetEnvironmentVariables () 
    |> Seq.cast<System.Collections.DictionaryEntry>
    |> Seq.map (fun d -> d.Key :?> string, d.Value :?> string)
    |> Map.ofSeq

let initializeSuite () =

#if DEBUG
    let configurationName = "debug"
#else
    let configurationName = "release"
#endif
    let env = envVars ()

    let cfg =
        let c = config configurationName env
        let usedEnvVars = c.EnvironmentVariables  |> Map.add "FSC" c.FSC             
        { c with EnvironmentVariables = usedEnvVars }

    logConfig cfg

    cfg


let suiteHelpers = lazy (initializeSuite ())

[<AttributeUsage(AttributeTargets.Assembly)>]
type public InitializeSuiteAttribute () =
    inherit TestActionAttribute()

    override x.BeforeTest details =
        try
            if details.IsSuite 
            then suiteHelpers.Force() |> ignore
        with
        | e -> raise (Exception("failed test suite initialization, debug code in InitializeSuiteAttribute", e))
    override x.AfterTest _details =
        ()

    override x.Targets = ActionTargets.Test ||| ActionTargets.Suite


[<assembly:ParallelizableAttribute(ParallelScope.Fixtures)>]
[<assembly:InitializeSuite()>]
()

let fsharpSuiteDirectory = __SOURCE_DIRECTORY__

let testConfig testDir =
    let cfg = suiteHelpers.Value
    let dir = Path.GetFullPath(fsharpSuiteDirectory ++ testDir)
    log "------------------ %s ---------------" dir
    log "cd %s" dir
    { cfg with Directory =  dir}

[<AllowNullLiteral>]
type FileGuard(path: string) =
    let remove path = if File.Exists(path) then Commands.rm (Path.GetTempPath()) path
    do if not (Path.IsPathRooted(path)) then failwithf "path '%s' must be absolute" path
    do remove path
    member x.Path = path
    member x.Exists = x.Path |> File.Exists
    member x.CheckExists() =
        if not x.Exists then 
             failwith (sprintf "exit code 0 but %s file doesn't exists" (x.Path |> Path.GetFileName))

    interface IDisposable with
        member x.Dispose () = remove path
        

type RedirectToType = 
    | Overwrite of FilePath
    | Append of FilePath

type RedirectTo = 
    | Inherit
    | Output of RedirectToType
    | OutputAndError of RedirectToType * RedirectToType
    | OutputAndErrorToSameFile of RedirectToType 
    | Error of RedirectToType

type RedirectFrom = 
    | RedirectInput of FilePath

type RedirectInfo = 
    { Output : RedirectTo
      Input : RedirectFrom option }


module Command =

    let logExec _dir path args redirect =
        let inF =
            function
            | None -> ""
            | Some(RedirectInput l) -> sprintf " <%s" l
        let redirectType = function Overwrite x -> sprintf ">%s" x | Append x -> sprintf ">>%s" x
        let outF =
            function
            | Inherit -> ""
            | Output r-> sprintf " 1%s" (redirectType r)
            | OutputAndError (r1, r2) -> sprintf " 1%s 2%s" (redirectType r1)  (redirectType r2)
            | OutputAndErrorToSameFile r -> sprintf " 1%s 2>1" (redirectType r)  
            | Error r -> sprintf " 2%s" (redirectType r)
        sprintf "%s%s%s%s" path (match args with "" -> "" | x -> " " + x) (inF redirect.Input) (outF redirect.Output)

    let exec dir envVars (redirect:RedirectInfo) path args =

        let inputWriter sources (writer: StreamWriter) =
            let pipeFile name = async {
                let path = Commands.getfullpath dir name
                use reader = File.OpenRead (path)
                use ms = new MemoryStream()
                do! reader.CopyToAsync (ms) |> (Async.AwaitIAsyncResult >> Async.Ignore)
                ms.Position <- 0L
                try
                    do! ms.CopyToAsync(writer.BaseStream) |> (Async.AwaitIAsyncResult >> Async.Ignore)
                    do! writer.FlushAsync() |> (Async.AwaitIAsyncResult >> Async.Ignore)
                with
                | :? System.IO.IOException -> //input closed is ok if process is closed
                    ()
                }
            sources |> pipeFile |> Async.RunSynchronously

        let inF fCont cmdArgs =
            match redirect.Input with
            | None -> fCont cmdArgs
            | Some(RedirectInput l) -> fCont { cmdArgs with RedirectInput = Some (inputWriter l) }

        let openWrite rt =
            let fullpath = Commands.getfullpath dir
            match rt with 
            | Append p -> File.AppendText( p |> fullpath)
            | Overwrite p -> new StreamWriter(new FileStream(p |> fullpath, FileMode.Create))

        let outF fCont cmdArgs =
            match redirect.Output with
            | RedirectTo.Inherit ->  
                use toLog = redirectToLog ()
                fCont { cmdArgs with RedirectOutput = Some (toLog.Post); RedirectError = Some (toLog.Post) }
            | Output r ->
                use writer = openWrite r
                use outFile = redirectTo writer
                use toLog = redirectToLog ()
                fCont { cmdArgs with RedirectOutput = Some (outFile.Post); RedirectError = Some (toLog.Post) }
            | OutputAndError (r1,r2) ->
                use writer1 = openWrite r1
                use writer2 = openWrite r2
                use outFile1 = redirectTo writer1
                use outFile2 = redirectTo writer2
                fCont { cmdArgs with RedirectOutput = Some (outFile1.Post); RedirectError = Some (outFile2.Post) }
            | OutputAndErrorToSameFile r ->
                use writer = openWrite r
                use outFile = redirectTo writer
                fCont { cmdArgs with RedirectOutput = Some (outFile.Post); RedirectError = Some (outFile.Post) }
            | Error r ->
                use writer = openWrite r
                use outFile = redirectTo writer
                use toLog = redirectToLog ()
                fCont { cmdArgs with RedirectOutput = Some (toLog.Post); RedirectError = Some (outFile.Post) }
            
        let exec cmdArgs =
            log "%s" (logExec dir path args redirect)
            Process.exec cmdArgs dir envVars path args

        { RedirectOutput = None; RedirectError = None; RedirectInput = None }
        |> (outF (inF exec))

let alwaysSuccess _ = ()

let execArgs = { Output = Inherit; Input = None; }
let execAppend cfg stdoutPath stderrPath p = Command.exec cfg.Directory cfg.EnvironmentVariables { execArgs with Output = OutputAndError(Append(stdoutPath), Append(stderrPath)) } p >> checkResult
let execAppendIgnoreExitCode cfg stdoutPath stderrPath p = Command.exec cfg.Directory cfg.EnvironmentVariables { execArgs with Output = OutputAndError(Append(stdoutPath), Append(stderrPath)) } p >> alwaysSuccess
let exec cfg p = Command.exec cfg.Directory cfg.EnvironmentVariables execArgs p >> checkResult
let execExpectFail cfg p = Command.exec cfg.Directory cfg.EnvironmentVariables execArgs p >> checkErrorLevel1
let execIn cfg workDir p = Command.exec workDir cfg.EnvironmentVariables execArgs p >> checkResult
let execBothToOut cfg workDir outFile p = Command.exec workDir  cfg.EnvironmentVariables { execArgs with Output = OutputAndErrorToSameFile(Overwrite(outFile)) } p >> checkResult
let execAppendOutIgnoreExitCode cfg workDir outFile p = Command.exec workDir  cfg.EnvironmentVariables { execArgs with Output = Output(Append(outFile)) } p >> alwaysSuccess
let execAppendErrExpectFail cfg errPath p = Command.exec cfg.Directory cfg.EnvironmentVariables { execArgs with Output = Error(Overwrite(errPath)) } p >> checkErrorLevel1
let execStdin cfg l p = Command.exec cfg.Directory cfg.EnvironmentVariables { Output = Inherit; Input = Some(RedirectInput(l)) } p >> checkResult
let execStdinAppendBothIgnoreExitCode cfg stdoutPath stderrPath stdinPath p = Command.exec cfg.Directory cfg.EnvironmentVariables { Output = OutputAndError(Append(stdoutPath), Append(stderrPath)); Input = Some(RedirectInput(stdinPath)) } p >> alwaysSuccess
let fsc cfg arg = Printf.ksprintf (Commands.fsc cfg.Directory (exec cfg) cfg.DotNetExe cfg.FSC) arg
let fscIn cfg workDir arg = Printf.ksprintf (Commands.fsc workDir (execIn cfg workDir) cfg.DotNetExe  cfg.FSC) arg
let fscAppend cfg stdoutPath stderrPath arg = Printf.ksprintf (Commands.fsc cfg.Directory (execAppend cfg stdoutPath stderrPath) cfg.DotNetExe  cfg.FSC) arg
let fscAppendIgnoreExitCode cfg stdoutPath stderrPath arg = Printf.ksprintf (Commands.fsc cfg.Directory (execAppendIgnoreExitCode cfg stdoutPath stderrPath) cfg.DotNetExe  cfg.FSC) arg
let fscBothToOut cfg out arg = Printf.ksprintf (Commands.fsc cfg.Directory (execBothToOut cfg cfg.Directory out) cfg.DotNetExe  cfg.FSC) arg
let fscAppendErrExpectFail cfg errPath arg = Printf.ksprintf (Commands.fsc cfg.Directory (execAppendErrExpectFail cfg errPath) cfg.DotNetExe  cfg.FSC) arg
let csc cfg arg = Printf.ksprintf (Commands.csc (exec cfg) cfg.CSC) arg
let ildasm cfg arg = Printf.ksprintf (Commands.ildasm (exec cfg) cfg.ILDASM) arg
let peverify cfg = Commands.peverify (exec cfg) cfg.PEVERIFY "/nologo"
let peverifyWithArgs cfg args = Commands.peverify (exec cfg) cfg.PEVERIFY args
let fsi cfg = Printf.ksprintf (Commands.fsi (exec cfg) cfg.FSI)
let fsiAnyCpu cfg = Printf.ksprintf (Commands.fsi (exec cfg) cfg.FSIANYCPU)
let fsi_script cfg = Printf.ksprintf (Commands.fsi (exec cfg) cfg.FSI_FOR_SCRIPTS)
let fsiExpectFail cfg = Printf.ksprintf (Commands.fsi (execExpectFail cfg) cfg.FSI)
let fsiAppendIgnoreExitCode cfg stdoutPath stderrPath = Printf.ksprintf (Commands.fsi (execAppendIgnoreExitCode cfg stdoutPath stderrPath) cfg.FSI)
let fileguard cfg = (Commands.getfullpath cfg.Directory) >> (fun x -> new FileGuard(x))
let getfullpath cfg = Commands.getfullpath cfg.Directory
let fileExists cfg = Commands.fileExists cfg.Directory >> Option.isSome
let fsiStdin cfg stdinPath = Printf.ksprintf (Commands.fsi (execStdin cfg stdinPath) cfg.FSI)
let fsiStdinAppendBothIgnoreExitCode cfg stdoutPath stderrPath stdinPath = Printf.ksprintf (Commands.fsi (execStdinAppendBothIgnoreExitCode cfg stdoutPath stderrPath stdinPath) cfg.FSI)
let rm cfg x = Commands.rm cfg.Directory x
let rmdir cfg x = Commands.rmdir cfg.Directory x
let mkdir cfg = Commands.mkdir_p cfg.Directory
let copy_y cfg f = Commands.copy_y cfg.Directory f >> checkResult

let diff normalize path1 path2 =
    let result = System.Text.StringBuilder()
    let append s = result.AppendLine s |> ignore
    let cwd = Directory.GetCurrentDirectory()

    if not <| File.Exists(path1) then failwithf "Invalid path %s" path1
    if not <| File.Exists(path2) then failwithf "Invalid path %s" path2

    let lines1 = File.ReadAllLines(path1)
    let lines2 = File.ReadAllLines(path2)

    let minLines = min lines1.Length lines2.Length

    for i = 0 to (minLines - 1) do
        let normalizePath (line:string) =
            if normalize then
                let x = line.IndexOf(cwd, StringComparison.OrdinalIgnoreCase)
                if x >= 0 then line.Substring(x+cwd.Length) else line
            else line

        let line1 = normalizePath lines1.[i]
        let line2 = normalizePath lines2.[i]

        if line1 <> line2 then
            append <| sprintf "diff between [%s] and [%s]" path1 path2
            append <| sprintf "line %d" (i+1)
            append <| sprintf " - %s" line1
            append <| sprintf " + %s" line2

    if lines1.Length <> lines2.Length then
        append <| sprintf "diff between [%s] and [%s]" path1 path2
        append <| sprintf "diff at line %d" minLines
        lines1.[minLines .. (lines1.Length - 1)] |> Array.iter (append << sprintf "- %s")
        lines2.[minLines .. (lines2.Length - 1)] |> Array.iter (append << sprintf "+ %s")

    result.ToString()

let fsdiff cfg a b = 
    let actualFile = System.IO.Path.Combine(cfg.Directory, a)
    let expectedFile = System.IO.Path.Combine(cfg.Directory, b)
    let errorText = System.IO.File.ReadAllText (System.IO.Path.Combine(cfg.Directory, a))

    let result = diff false expectedFile actualFile
    if result <> "" then
        log "%s" result
        log "New error file:"
        log "%s" errorText

    result
        
let requireENCulture () = 
    match System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName with
    | "en" -> true
    | _ -> false
