module TypeChecks.typeextensions
open System
open System.IO
open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Compiler.IO
open NUnit.Framework
let private verifyFSXBaseline (cu) : unit =
    match cu with
    | FS fs ->
        match fs.Baseline with
        | None -> failwith "Baseline was not provided."
        | Some bsl ->
            let errorsExpectedBaseLine =
                match bsl.FSBaseline.Content with
                | Some b -> b.Replace("\r\n","\n")
                | None ->  String.Empty
            let errorsActual =
                match fs.Source with
                | SourceCodeFileKind.Fsx _->
                    //let fsxScriptFile = FileInfo fs.Source.GetSourceFileName
                    //let dir = fsxScriptFile.Directory
                    //RunRealScriptWithOptionsAndReturnResult
                    //let cfg = testConfig fsxScriptFile.Directory.FullName
                    let version = ScriptHelpers.LangVersion.Preview
                    let _r = Miscellaneous.FsharpSuiteMigrated.ScriptRunner.runScriptFile version cu
                    //let result = TestFramework.fsi cfg "%s" cfg.fsi_flags []
                    //let fsxResult = CompilerAssert.RunRealScriptWithOptionsAndReturnResult [||] fsxScriptFile
                    
                    //snd fsxResult |> sanitizeFsxOutput
                    "()"
                | _ -> failwith $"not supposed to check %A{fs.Source}"

            if errorsExpectedBaseLine <> errorsActual then
                fs.CreateOutputDirectory()
                createBaselineErrors bsl.FSBaseline errorsActual
            elif FileSystem.FileExistsShim(bsl.FSBaseline.FilePath) then
                FileSystem.FileDeleteShim(bsl.FSBaseline.FilePath)

            Assert.AreEqual(errorsExpectedBaseLine, errorsActual, $"\nExpected:\n{errorsExpectedBaseLine}\nActual:\n{errorsActual}")
    | _ -> failwith $"not supposed to check %A{cu}"

[<Fact>]
let ``issue.16034`` () =
    let scriptPath = Path.Combine(
        __SOURCE_DIRECTORY__
        , "typeextensions"
        , "issue.16034"
        , "issue.16034.fsx"
        )
    RealFsxFromPath scriptPath
    |> withBaseLine
    |> verifyILBaseline
    |> compileAndRun
    |> verifyOutputWithDefaultBaseline
    
[<Fact>]
let ``issue.16034.check1`` () =
     
    let scriptPath = Path.Combine(
        __SOURCE_DIRECTORY__
        , "typeextensions"
        , "issue.16034"
        , "issue.16034.check1.fsx"
        )
    RealFsxFromPath scriptPath
    |> withBaseLine
    |> verifyFSXBaseline

[<Fact>]
let ``issue.16034.check2`` () =
    let scriptPath = Path.Combine(
        __SOURCE_DIRECTORY__
        , "typeextensions"
        , "issue.16034"
        , "issue.16034.check2.fsx"
        )
    RealFsxFromPath scriptPath
    |> withBaseLine
    |> verifyFSXBaseline
    
    