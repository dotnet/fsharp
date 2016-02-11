module NUnitConf

open System
open System.IO
open NUnit.Framework

open UpdateCmd
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

let defaultConfigurationName =
#if DEBUG
    DEBUG
#else
    RELEASE
#endif

let parseConfigurationName (name: string) =
    match name.ToUpper() with
    | "RELEASE" -> RELEASE
    | "DEBUG" -> DEBUG
    | s -> failwithf "invalid env var FSHARP_TEST_SUITE_CONFIGURATION '%s'" s
    

let initializeSuite () =

    let configurationName = defaultConfigurationName

    let doNgen = true;

    let FSCBinPath = __SOURCE_DIRECTORY__/".."/".."/(sprintf "%O" configurationName)/"net40"/"bin"

    let mapWithDefaults defaults m =
        Seq.concat [ (Map.toSeq defaults) ; (Map.toSeq m) ] |> Map.ofSeq

    let env = 
        envVars ()
        |> mapWithDefaults ( [ "FSCBINPATH", FSCBinPath ] |> Map.ofList )

    let configurationName =
        match env |> Map.tryFind "FSHARP_TEST_SUITE_CONFIGURATION" |> Option.map parseConfigurationName with
        | Some confName -> confName
        | None -> configurationName

    attempt {
        do! updateCmd env { Configuration = configurationName; Ngen = doNgen; }
            |> Attempt.Run
            |> function Success () -> Success () | Failure msg -> genericError msg ()

        let cfg =
            let c = config env
            let usedEnvVars =
                c.EnvironmentVariables 
                |> Map.add "FSC" c.FSC             
            { c with EnvironmentVariables = usedEnvVars }

        logConfig cfg

        let directoryExists = Commands.directoryExists (Path.GetTempPath()) >> Option.isSome 

        let checkfscBinPath () = attempt {

            let fscBinPath = cfg.EnvironmentVariables |> Map.tryFind "FSCBINPATH"
            return!
                match fscBinPath with
                | Some dir when directoryExists dir -> Success
                | None -> genericError "environment variable 'FSCBinPath' is required to be a valid directory, is not set"
                | Some dir -> genericError (sprintf "environment variable 'FSCBinPath' is required to be a valid directory, but is '%s'" dir)
            }

        let smokeTest () = attempt {
            let tempFile ext = 
                let p = Path.ChangeExtension( Path.GetTempFileName(), ext)
                File.AppendAllText (p, """printfn "ciao"; exit 0""")
                p

            let tempDir = Commands.createTempDir ()
            let exec exe args =
                log "%s %s" exe args
                use toLog = redirectToLog ()
                Process.exec { RedirectError = Some toLog.Post; RedirectOutput = Some toLog.Post; RedirectInput = None } tempDir cfg.EnvironmentVariables exe args

            do! Commands.fsc exec cfg.FSC "" [ tempFile ".fs" ] |> checkResult

            do! Commands.fsi exec cfg.FSI "" [ tempFile ".fsx" ] |> checkResult
        
            }
    
        do! checkfscBinPath ()

        do! smokeTest ()

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

    override x.AfterTest details =
        ()

    override x.Targets with get() = ActionTargets.Test ||| ActionTargets.Suite


[<assembly:ParallelizableAttribute(ParallelScope.Fixtures)>]
[<assembly:InitializeSuite()>]
()

module FSharpTestSuite =

    let getTagsOfFile path =
        match File.ReadLines(path) |> Seq.truncate 5 |> Seq.tryFind (fun s -> s.StartsWith("// #")) with
        | None -> []
        | Some line -> 
            line.TrimStart('/').Split([| '#' |], StringSplitOptions.RemoveEmptyEntries)
            |> Seq.map (fun s -> s.Trim())
            |> Seq.filter (fun s -> s.Length > 0)
            |> Seq.distinct
            |> Seq.toList

    let getTestFileMetadata dir =
        Directory.EnumerateFiles(dir, "*.fs*")
        |> Seq.toList
        |> List.collect getTagsOfFile

    let parseTestLst path =
        let dir = Path.GetDirectoryName(path)
        let commentLine (t: string) = t.StartsWith("#")
        let lines =
            File.ReadAllLines(path)
            |> Array.filter (not << commentLine)
            |> Array.filter (not << String.IsNullOrWhiteSpace)
        let parse (t: string) =
            let a = t.Split([| '\t'; '\t' |], StringSplitOptions.RemoveEmptyEntries)
            let testDir = Commands.getfullpath dir a.[1]
            [| for x in a.[0].Split(',') do yield (x, testDir) |]

        lines |> Array.collect parse |> List.ofArray

    let ``test.lst`` = lazy ( 
        parseTestLst ( __SOURCE_DIRECTORY__/".."/"test.lst" ) 
        )

    let getTestLstTags db dir =
        let normalizePath path =
            Uri(path).LocalPath
            |> (fun s -> s.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
            |> (fun s -> s.ToUpperInvariant())

        let sameDir a = (normalizePath dir) = (normalizePath a)
        let normalizedPath = normalizePath dir
        db
        |> List.choose (fun (tag, d) -> if sameDir d then Some tag else None)

    let fsharpSuiteDirectory = __SOURCE_DIRECTORY__

    let setProps dir (props: NUnit.Framework.Interfaces.IPropertyBag) =
        let testDir = dir |> Commands.getfullpath fsharpSuiteDirectory

        if not (Directory.Exists(testDir)) then failwithf "test directory '%s' does not exists" testDir

        let categories = [ dir ] @ (testDir |> getTestFileMetadata) @ (testDir |> getTestLstTags ``test.lst``.Value)
        categories |> List.iter (fun (c: string) -> props.Add(NUnit.Framework.Internal.PropertyNames.Category, c))

        props.Set("DIRECTORY", testDir)

    let testContext () =
        let test = NUnit.Framework.TestContext.CurrentContext.Test
        { Directory = test.Properties.Get("DIRECTORY") :?> string;
          Config = suiteHelpers.Value }

// parametrized test cases does not inherits properties of test ( see https://github.com/nunit/nunit/issues/548 )
// and properties is where the custom context data is saved

type FSharpSuiteTestAttribute(dir: string) =
    inherit NUnitAttribute()

    new() = FSharpSuiteTestAttribute(Commands.createTempDir())

    interface NUnit.Framework.Interfaces.IApplyToTest with
        member x.ApplyToTest(test: NUnit.Framework.Internal.Test) =
            try
                test.Properties |> FSharpTestSuite.setProps dir
            with ex ->
                test.RunState <- NUnit.Framework.Interfaces.RunState.NotRunnable
                test.Properties.Set(NUnit.Framework.Internal.PropertyNames.SkipReason, NUnit.Framework.Internal.ExceptionHelper.BuildMessage(ex))
                test.Properties.Set(NUnit.Framework.Internal.PropertyNames.ProviderStackTrace, NUnit.Framework.Internal.ExceptionHelper.BuildStackTrace(ex))

type FSharpSuiteTestCaseData =
    inherit TestCaseData

    new (dir: string, [<ParamArray>] arguments: Object array) as this = 
        { inherit TestCaseData(arguments) }
        then
            this.Properties |> FSharpTestSuite.setProps dir
            arguments
            |> Array.choose (fun a -> match a with :? Permutation as p -> Some p | _ -> None)
            |> Array.iter (fun p -> this.SetCategory(sprintf "%A" p) |> ignore)

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)>]
type FSharpSuiteTestCaseAttribute =
    inherit TestCaseAttribute

    new (dir: string, [<ParamArray>] arguments: Object array) as this = 
        { inherit TestCaseAttribute(arguments) }
        then
            this.Properties |> FSharpTestSuite.setProps dir


let allPermutations = 
    [ FSI_FILE; 
      FSI_STDIN; 
      FSI_STDIN_OPT; 
      FSI_STDIN_GUI;
      FSC_BASIC; 
      GENERATED_SIGNATURE; 
      FSC_OPT_MINUS_DEBUG; 
      FSC_OPT_PLUS_DEBUG; 
      SPANISH;
      AS_DLL]

let codeAndInferencePermutations = 
    [ GENERATED_SIGNATURE; 
      FSI_FILE; 
      FSC_OPT_PLUS_DEBUG;  
      AS_DLL ]

let codePermutations = 
    [ FSI_FILE; 
      FSC_OPT_PLUS_DEBUG;  ]

let slowLibCodePermutations = 
    [ FSC_OPT_PLUS_DEBUG;  ]

let BuildFrom(dir, builder:NUnit.Framework.Internal.Builders.NUnitTestCaseBuilder, methodInfo, suite, permutations: Permutation list) =
    permutations
    |> List.map (fun p -> (new FSharpSuiteTestCaseData (dir, p)))
    |> List.map (fun tc -> builder.BuildTestMethod(methodInfo, suite, tc))
    |> Seq.ofList

type FSharpSuiteAllPermutationsAttribute(dir: string) =
    inherit NUnitAttribute()

    let _builder = NUnit.Framework.Internal.Builders.NUnitTestCaseBuilder()
    interface NUnit.Framework.Interfaces.ITestBuilder with
        member x.BuildFrom(methodInfo, suite) = BuildFrom(dir, _builder, methodInfo, suite, allPermutations)

type FSharpSuiteCodeAndSignaturePermutationsAttribute(dir: string) =
    inherit NUnitAttribute()

    let _builder = NUnit.Framework.Internal.Builders.NUnitTestCaseBuilder()
    interface NUnit.Framework.Interfaces.ITestBuilder with
        member x.BuildFrom(methodInfo, suite) = BuildFrom(dir, _builder, methodInfo, suite, codeAndInferencePermutations)

type FSharpSuiteScriptPermutationsAttribute(dir: string) =
    inherit NUnitAttribute()

    let _builder = NUnit.Framework.Internal.Builders.NUnitTestCaseBuilder()
    interface NUnit.Framework.Interfaces.ITestBuilder with
        member x.BuildFrom(methodInfo, suite) = BuildFrom(dir, _builder, methodInfo, suite, codePermutations)

type FSharpSuiteFscCodePermutationAttribute(dir: string) =
    inherit NUnitAttribute()

    let _builder = NUnit.Framework.Internal.Builders.NUnitTestCaseBuilder()
    interface NUnit.Framework.Interfaces.ITestBuilder with
        member x.BuildFrom(methodInfo, suite) = BuildFrom(dir, _builder, methodInfo, suite, slowLibCodePermutations)


module FileGuard =
    let private remove path = if File.Exists(path) then Commands.rm (Path.GetTempPath()) path

    [<AllowNullLiteral>]
    type T (path: string) =
        member x.Path = path
        interface IDisposable with
            member x.Dispose () = remove path

    let create path =
        if not (Path.IsPathRooted(path)) then failwithf "path '%s' must be absolute" path
        remove path
        new T(path)
    
    let exists (guard: T) = guard.Path |> File.Exists
        

let checkGuardExists guard = attempt {
    if not <| (guard |> FileGuard.exists)
    then return! genericError (sprintf "exit code 0 but %s file doesn't exists" (guard.Path |> Path.GetFileName))
    }


let check (f: Attempt<_,_>) =
    f |> Attempt.Run |> checkTestResult


type RedirectInfo = 
    { Output : RedirectTo
      Input : RedirectFrom option }

and RedirectTo = 
    | Inherit
    | Output of RedirectToType
    | OutputAndError of RedirectToType * RedirectToType
    | OutputAndErrorToSameFile of RedirectToType 
    | Error of RedirectToType

and RedirectToType = 
    | Overwrite of FilePath
    | Append of FilePath

and RedirectFrom = 
    | RedirectInput of FilePath


module Command =

    let logExec dir path args redirect =
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
                | :? System.IO.IOException as ex -> //input closed is ok if process is closed
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
            | Append p -> new StreamWriter (p |> fullpath, true) 
            | Overwrite p -> new StreamWriter (p |> fullpath, false)

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


[<AutoOpen>]
module CommandTypes =
    type SourceFile = 
    | File of FilePath
    | Content of string * (string -> TextWriter -> unit)

module FscCommand =

    type FscOutputLine =
        | Error of string * string
        | Warning of string * string
        | Text of string
    
    type FscToLibraryArgs = {
        OutLibrary: FilePath
        SourceFiles: SourceFile list 
        }

    type FscToLibraryResult = {
        OutLibraryFullPath: FilePath
        StderrText: string list
        }

    let private parseFscOutLine line =
        let (|RegexFsc|_|) outType line =
            let pattern = sprintf "%s (?<code>.+): (?<descr>.*)" outType
            match System.Text.RegularExpressions.Regex.Match(line, pattern) with
            | m when m.Success -> Some (m.Groups.["code"].Value, m.Groups.["descr"].Value)
            | _ -> None
        
        match line with
        | RegexFsc "error" (code, descr) -> FscOutputLine.Error(code, descr)
        | RegexFsc "warning" (code, descr) -> FscOutputLine.Warning(code, descr)
        | l -> FscOutputLine.Text(line)

    let parseFscOut = List.map parseFscOutLine

    let fscToLibrary dir exec (fscExe: FilePath) flags (args: FscToLibraryArgs) = attempt {
        let ``exec 2>a`` a p = exec { RedirectInfo.Output = RedirectTo.Error(a); Input = None; } p >> checkResult
            
        let outStream = Path.GetTempFileName ()
        let fsc = Printf.ksprintf (Commands.fsc (``exec 2>a`` (Overwrite(outStream))) fscExe)
    
        let sourceFiles = 
            args.SourceFiles 
            |> List.map (fun sf ->
                match sf with
                | SourceFile.Content (name, writer) ->
                    let filePath = dir/name
                    use file = File.CreateText(filePath)
                    writer name file
                    name
                | SourceFile.File path -> path )

        let outDll = args.OutLibrary
        
        let logOutputOnFailure x =
            match x with
            | Success x -> Success x
            | Failure(e) ->
                printf "%s" (File.ReadAllText(outStream))
                Failure(e)

        do! (fsc "%s -a -o:%s" flags outDll sourceFiles) |> logOutputOnFailure
            
        let outText = File.ReadAllLines(outStream) |> List.ofArray

        return { FscToLibraryResult.OutLibraryFullPath = (Commands.getfullpath dir outDll)
                 StderrText = outText }
        }
