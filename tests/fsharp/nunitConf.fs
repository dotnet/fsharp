module NUnitConf

open System
open System.IO
open NUnit.Framework

open TestConfig
open PlatformHelpers
open FSharpTestSuiteTypes

let checkTestResult result =
    match result with
    | Success () -> ()
    | Failure (GenericError msg) -> Assert.Fail (msg)
    | Failure (ProcessExecError (msg1, err, msg2)) -> Assert.Fail (sprintf "%s. ERRORLEVEL %i %s" msg1 err msg2)
    | Failure (Skipped msg) -> Assert.Ignore(sprintf "skipped. Reason: %s" msg)

let checkResult result = 
    match result with
    | CmdResult.ErrorLevel (msg1, err) -> Failure (RunError.ProcessExecError (msg1, err, sprintf "ERRORLEVEL %d" err))
    | CmdResult.Success -> Success ()

let skip msg () = Failure (Skipped msg)
let genericError msg () = Failure (GenericError msg)

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


    attempt {
        let env = envVars ()

        let cfg =
            let c = config configurationName env
            let usedEnvVars = c.EnvironmentVariables  |> Map.add "FSC" c.FSC             
            { c with EnvironmentVariables = usedEnvVars }

        logConfig cfg

        return cfg
    } 


let suiteHelpers = lazy (
    initializeSuite ()
    |> Attempt.Run 
    |> function Success x -> x | Failure err -> failwith (sprintf "Error %A" err)
)

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
    { cfg with Directory = Path.GetFullPath(fsharpSuiteDirectory/testDir) }

[<AllowNullLiteral>]
type FileGuard(path: string) =
    let remove path = if File.Exists(path) then Commands.rm (Path.GetTempPath()) path
    do if not (Path.IsPathRooted(path)) then failwithf "path '%s' must be absolute" path
    do remove path
    member x.Path = path
    member x.Exists = x.Path |> File.Exists
    member x.CheckExists = attempt {
        if not x.Exists then 
             return! genericError (sprintf "exit code 0 but %s file doesn't exists" (x.Path |> Path.GetFileName))
        }
    interface IDisposable with
        member x.Dispose () = remove path
        

let check (f: Attempt<_,_>) =
    f |> Attempt.Run |> checkTestResult


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
            | Overwrite p -> new StreamWriter(File.OpenWrite(p |> fullpath))

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

let alwaysSuccess _ = Success ()

let execAppend cfg stdoutPath stderrPath p = Command.exec cfg.Directory cfg.EnvironmentVariables { Output = OutputAndError(Append(stdoutPath), Append(stderrPath)); Input = None; } p >> checkResult
let execAppendIgnoreExitCode cfg stdoutPath stderrPath p = Command.exec cfg.Directory cfg.EnvironmentVariables { Output = OutputAndError(Append(stdoutPath), Append(stderrPath)); Input = None; } p >> alwaysSuccess
let exec cfg p = Command.exec cfg.Directory cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
let execIn cfg workDir p = Command.exec workDir cfg.EnvironmentVariables { Output = Inherit; Input = None; } p >> checkResult
let execBothToOut cfg workDir outFile p = Command.exec workDir  cfg.EnvironmentVariables { Output = OutputAndErrorToSameFile(Overwrite(outFile)); Input = None; } p >> checkResult
let execAppendOutIgnoreExitCode cfg workDir outFile p = Command.exec workDir  cfg.EnvironmentVariables { Output = Output(Append(outFile)); Input = None; } p >> alwaysSuccess


let fsc cfg arg = Printf.ksprintf (Commands.fsc cfg.Directory (exec cfg) cfg.FSC) arg
let fscIn cfg workDir arg = Printf.ksprintf (Commands.fsc workDir (execIn cfg workDir) cfg.FSC) arg
let fscAppend cfg stdoutPath stderrPath arg = Printf.ksprintf (Commands.fsc cfg.Directory (execAppend cfg stdoutPath stderrPath) cfg.FSC) arg
let fscAppendIgnoreExitCode cfg stdoutPath stderrPath arg = Printf.ksprintf (Commands.fsc cfg.Directory (execAppendIgnoreExitCode cfg stdoutPath stderrPath) cfg.FSC) arg
let fscBothToOut cfg out arg = Printf.ksprintf (Commands.fsc cfg.Directory (execBothToOut cfg cfg.Directory out) cfg.FSC) arg

let csc cfg arg = Printf.ksprintf (Commands.csc (exec cfg) cfg.CSC) arg
let ildasm cfg arg = Printf.ksprintf (Commands.ildasm (exec cfg) cfg.ILDASM) arg
let peverify cfg = Commands.peverify (exec cfg) cfg.PEVERIFY "/nologo"
let sn cfg outfile arg = execAppendOutIgnoreExitCode cfg cfg.Directory outfile cfg.SN arg
let peverifyWithArgs cfg args = Commands.peverify (exec cfg) cfg.PEVERIFY args
let fsi cfg = Printf.ksprintf (Commands.fsi (exec cfg) cfg.FSI)
let fsiAppendIgnoreExitCode cfg stdoutPath stderrPath = Printf.ksprintf (Commands.fsi (execAppendIgnoreExitCode cfg stdoutPath stderrPath) cfg.FSI)
let fileguard cfg = (Commands.getfullpath cfg.Directory) >> (fun x -> new FileGuard(x))
let getfullpath cfg = Commands.getfullpath cfg.Directory
let fileExists cfg = Commands.fileExists cfg.Directory >> Option.isSome
let execStdin cfg l p = Command.exec cfg.Directory cfg.EnvironmentVariables { Output = Inherit; Input = Some(RedirectInput(l)) } p >> checkResult
let fsiStdin cfg = Printf.ksprintf (fun flags l -> Commands.fsi (execStdin cfg l) cfg.FSI flags [])
let execStdinAppendBothIgnoreExitCode cfg stdoutPath stderrPath  l p = Command.exec cfg.Directory cfg.EnvironmentVariables { Output = OutputAndError(Append(stdoutPath), Append(stderrPath)); Input = Some(RedirectInput(l)) } p >> alwaysSuccess
let fsiStdinAppendBothIgnoreExitCode cfg stdoutPath stderrPath = Printf.ksprintf (fun flags l -> Commands.fsi (execStdinAppendBothIgnoreExitCode cfg stdoutPath stderrPath l) cfg.FSI flags [])
let rm cfg x = Commands.rm cfg.Directory x
let mkdir cfg = Commands.mkdir_p cfg.Directory
let copy_y cfg f = Commands.copy_y cfg.Directory f >> checkResult

let fsdiff cfg a b = attempt {
    let out = new ResizeArray<string>()
    let redirectOutputToFile path args =
        log "%s %s" path args
        use toLog = redirectToLog ()
        Process.exec { RedirectOutput = Some (function null -> () | s -> out.Add(s)); RedirectError = Some toLog.Post; RedirectInput = None; } cfg.Directory cfg.EnvironmentVariables path args
    do! (Commands.fsdiff redirectOutputToFile cfg.FSDIFF a b) |> (fun _ -> Success ())
    return out.ToArray() |> List.ofArray
    }

let requireENCulture () = attempt {
    do! match System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName with
        | "en" -> Success
        | c -> skip (sprintf "Test not supported except en Culture, was %s" c)
    }
