module NUnitConf

open System
open System.IO
open NUnit.Framework

open TestConfig
open PlatformHelpers
open FSharpTestSuiteTypes

let checkResult result = 
    match result with
    | CmdResult.ErrorLevel (msg1, err) -> Assert.Fail (sprintf "%s. ERRORLEVEL %d" msg1 err)
    | CmdResult.Success -> ()

let checkErrorLevel1 result = 
    match result with
    | CmdResult.ErrorLevel (_,1) -> ()
    | CmdResult.Success | CmdResult.ErrorLevel _ -> Assert.Fail  (sprintf "Command passed unexpectedly")

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
        if details.IsSuite 
        then suiteHelpers.Force() |> ignore

    override x.AfterTest _details =
        ()

    override x.Targets = ActionTargets.Test ||| ActionTargets.Suite


[<assembly:ParallelizableAttribute(ParallelScope.Fixtures)>]
[<assembly:InitializeSuite()>]
()

let fsharpSuiteDirectory = __SOURCE_DIRECTORY__

let testConfig testDir =
    let cfg = suiteHelpers.Value
    let dir = Path.GetFullPath(fsharpSuiteDirectory/testDir)
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

    let exec dir envVars redirect path args =
        let { Output = o; Input = i} = redirect

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
            match i with
            | None -> fCont cmdArgs
            | Some(RedirectInput l) -> fCont { cmdArgs with RedirectInput = Some (inputWriter l) }

        let openWrite rt =
            let fullpath = Commands.getfullpath dir
            match rt with 
            | Append p -> File.AppendText( p |> fullpath)
            | Overwrite p -> new StreamWriter(new FileStream(p |> fullpath, FileMode.Create))

        let outF fCont cmdArgs =
            match o with
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
let execAppendErrExpectFail cfg errPath p = Command.exec cfg.Directory cfg.EnvironmentVariables { execArgs with Output = Error(Overwrite(errPath)) } p
let execStdin cfg l p = Command.exec cfg.Directory cfg.EnvironmentVariables { Output = Inherit; Input = Some(RedirectInput(l)) } p >> checkResult
let execStdinAppendBothIgnoreExitCode cfg stdoutPath stderrPath stdinPath p = Command.exec cfg.Directory cfg.EnvironmentVariables { Output = OutputAndError(Append(stdoutPath), Append(stderrPath)); Input = Some(RedirectInput(stdinPath)) } p >> alwaysSuccess

let fsc cfg arg = Printf.ksprintf (Commands.fsc cfg.Directory (exec cfg) cfg.FSC) arg
let fscIn cfg workDir arg = Printf.ksprintf (Commands.fsc workDir (execIn cfg workDir) cfg.FSC) arg
let fscAppend cfg stdoutPath stderrPath arg = Printf.ksprintf (Commands.fsc cfg.Directory (execAppend cfg stdoutPath stderrPath) cfg.FSC) arg
let fscAppendIgnoreExitCode cfg stdoutPath stderrPath arg = Printf.ksprintf (Commands.fsc cfg.Directory (execAppendIgnoreExitCode cfg stdoutPath stderrPath) cfg.FSC) arg
let fscBothToOut cfg out arg = Printf.ksprintf (Commands.fsc cfg.Directory (execBothToOut cfg cfg.Directory out) cfg.FSC) arg

let fscAppendErrExpectFail cfg errPath arg = Printf.ksprintf (fun flags sources -> Commands.fsc cfg.Directory (execAppendErrExpectFail cfg errPath) cfg.FSC flags sources |> checkErrorLevel1) arg

let csc cfg arg = Printf.ksprintf (Commands.csc (exec cfg) cfg.CSC) arg
let ildasm cfg arg = Printf.ksprintf (Commands.ildasm (exec cfg) cfg.ILDASM) arg
let peverify cfg = Commands.peverify (exec cfg) cfg.PEVERIFY "/nologo"
let sn cfg outfile arg = execAppendOutIgnoreExitCode cfg cfg.Directory outfile cfg.SN arg
let peverifyWithArgs cfg args = Commands.peverify (exec cfg) cfg.PEVERIFY args
let fsi cfg = Printf.ksprintf (Commands.fsi (exec cfg) cfg.FSI)
let fsiExpectFail cfg = Printf.ksprintf (Commands.fsi (execExpectFail cfg) cfg.FSI)
let fsiAppendIgnoreExitCode cfg stdoutPath stderrPath = Printf.ksprintf (Commands.fsi (execAppendIgnoreExitCode cfg stdoutPath stderrPath) cfg.FSI)
let fileguard cfg = (Commands.getfullpath cfg.Directory) >> (fun x -> new FileGuard(x))
let getfullpath cfg = Commands.getfullpath cfg.Directory
let fileExists cfg = Commands.fileExists cfg.Directory >> Option.isSome
let fsiStdin cfg stdinPath = Printf.ksprintf (Commands.fsi (execStdin cfg stdinPath) cfg.FSI)
let fsiStdinAppendBothIgnoreExitCode cfg stdoutPath stderrPath stdinPath = Printf.ksprintf (Commands.fsi (execStdinAppendBothIgnoreExitCode cfg stdoutPath stderrPath stdinPath) cfg.FSI)
let rm cfg x = Commands.rm cfg.Directory x
let mkdir cfg = Commands.mkdir_p cfg.Directory
let copy_y cfg f = Commands.copy_y cfg.Directory f >> checkResult

let fsdiff cfg a b = 
    let out = new ResizeArray<string>()
    let redirectOutputToFile path args =
        log "%s %s" path args
        use toLog = redirectToLog ()
        Process.exec { RedirectOutput = Some (function null -> () | s -> out.Add(s)); RedirectError = Some toLog.Post; RedirectInput = None; } cfg.Directory cfg.EnvironmentVariables path args
    do (Commands.fsdiff redirectOutputToFile cfg.FSDIFF a b) |> (fun _ -> ())
    out.ToArray() |> List.ofArray

let requireENCulture () = 
    match System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName with
        | "en" -> true
        | _ -> false
