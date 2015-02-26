open System
open System.IO
open System.Diagnostics
open System.Reflection

module Helpers =
    let private run exePath args =
        let args = String.concat " " args
        let psi = ProcessStartInfo(FileName = exePath, Arguments = args, CreateNoWindow = true, UseShellExecute = false, RedirectStandardError = true)
        let p = Process.Start(psi)
        match p.WaitForExit(10 * 60 * 1000) with
        | false -> eprintfn "Process timed out"; exit 1
        | true  ->
            if p.ExitCode <> 0 then
               eprintfn "%s %s" exePath args
               eprintfn "%s" (p.StandardError.ReadToEnd())
               exit p.ExitCode

    let private authorCompile compilerPath runtime source =
        run compilerPath ["-a"; "-o:author.dll"; "--noframework"; sprintf "\"-r:%s\"" runtime; source]

    let private consumerCompile compilerPath runtime source =
        run compilerPath ["-o:consumer.exe"; "--noframework"; sprintf "\"-r:%s\"" runtime; "-r:author.dll"; source]

    let private consumerRunFsi fsiPath source =
        run fsiPath ["--exec"; source]

    let private consumerRunExe redirectVer =
        if File.Exists("consumer.exe.config") then
            File.Delete("consumer.exe.config")

        let content = File.ReadAllText("consumer.exe.config.txt").Replace("{ver}", redirectVer)
        File.WriteAllText("consumer.exe.config", content)    

        run "consumer.exe" []

    let getVer dllPath =
        let asm = Assembly.ReflectionOnlyLoadFrom(dllPath)
        asm.GetName().Version.ToString()

    let testExe authorCompiler authorRuntime consumerCompiler consumerRuntime authorSource consumerSource =
        authorCompile authorCompiler authorRuntime authorSource
        consumerCompile consumerCompiler consumerRuntime consumerSource
        consumerRunExe (getVer consumerRuntime)

    let testFsi authorCompiler authorRuntime consumerFsi authorSource consumerSource =
        authorCompile authorCompiler authorRuntime authorSource
        consumerRunFsi consumerFsi consumerSource

module Test = 
    let private env s =
        match Environment.GetEnvironmentVariable(s) with
        | var when not (String.IsNullOrWhiteSpace(var)) -> var
        | _ -> failwithf "Required env var %s not defined" s

    let vPrevCompiler    = env "FSCVPREV"
    let vPrevRuntime     = env "FSCOREDLLVPREVPATH"
    let vPrevFsi         = Path.Combine(env "FSCVPREVBINPATH", "fsi.exe")

    let vCurrentCompiler = env "FSC"
    let vCurrentRuntime  = env "FSCOREDLLPATH"
    let vCurrentFsi      = Path.Combine(env "FSCBINPATH", "fsi.exe")

    let cases =
          //                  compiler/runtime of author     |  compiler/runtime of consumer
        [ 0,  Helpers.testExe vPrevCompiler    vPrevRuntime     vCurrentCompiler vPrevRuntime
          1,  Helpers.testExe vPrevCompiler    vPrevRuntime     vCurrentCompiler vCurrentRuntime
          2,  Helpers.testExe vCurrentCompiler vPrevRuntime     vPrevCompiler    vPrevRuntime
          3,  Helpers.testExe vCurrentCompiler vPrevRuntime     vCurrentCompiler vPrevRuntime
          4,  Helpers.testExe vCurrentCompiler vPrevRuntime     vCurrentCompiler vCurrentRuntime
          5,  Helpers.testExe vCurrentCompiler vCurrentRuntime  vCurrentCompiler vCurrentRuntime

          //                  compiler/runtime of author     |  fsi of consumer
          6,  Helpers.testFsi vPrevCompiler    vPrevRuntime     vCurrentFsi
          7,  Helpers.testFsi vCurrentCompiler vPrevRuntime     vCurrentFsi
          8,  Helpers.testFsi vCurrentCompiler vPrevRuntime     vPrevFsi
          9,  Helpers.testFsi vCurrentCompiler vCurrentRuntime  vCurrentFsi
         ]
    
let authorSource, consumerSource, exclusions =
    match fsi.CommandLineArgs with
    | [| _; arg1; arg2 |] -> arg1, arg2, [| |]
    | [| _; arg1; arg2; arg3 |] -> arg1, arg2, (arg3.Split(',') |> Array.map int)
    | args ->
        eprintfn "Expecting args <author source> <consumer source> [excluded cases], got args %A" args
        exit 1

if not ((Helpers.getVer Test.vCurrentRuntime).StartsWith("4.4.0")) then
    eprintfn "Runtime version has changed, review exclusions lists for these tests"
    exit 1

Test.cases
|> List.filter (fun (id, _) -> not (Array.contains id exclusions))
|> List.iter (fun (id, testCase) ->
    printfn "Case %d" id
    testCase authorSource consumerSource
)
