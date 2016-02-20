module ``FSharp-Tests-Fsc-Warnings``

open System
open System.IO
open NUnit.Framework

open NUnitConf
open PlatformHelpers
open FSharpTestSuiteTypes

let testContext = FSharpTestSuite.testContext

open System.Reflection

module FS2003 =

    [<Test; FSharpSuiteTest()>]
    let ``should be raised if AssemblyInformationalVersion has invalid version`` () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let fscToLibrary = Printf.ksprintf (FscCommand.fscToLibrary dir (Command.exec dir cfg.EnvironmentVariables) cfg.FSC)

        printfn "Directory: %s" dir

        let code _name file =
            fprintf file "%s" """
namespace CST.RI.Anshun
open System.Reflection
[<assembly: AssemblyVersion("4.5.6.7")>]
[<assembly: AssemblyInformationalVersion("45.2048.main1.2-hotfix (upgrade Second Chance security)")>]
()
            """

        let! result = fscToLibrary "%s --nologo" cfg.fsc_flags { 
            SourceFiles = [ SourceFile.Content("test.fs", code) ]
            OutLibrary = "lib.dll" }
        
        let fv = Diagnostics.FileVersionInfo.GetVersionInfo(result.OutLibraryFullPath)

        fv.ProductVersion |> Assert.areEqual "45.2048.main1.2-hotfix (upgrade Second Chance security)"

        (fv.ProductMajorPart, fv.ProductMinorPart, fv.ProductBuildPart, fv.ProductPrivatePart) 
        |> Assert.areEqual (45,2048,0,0)

        let w =
            result.StderrText
            |> FscCommand.parseFscOut
            |> List.tryFind (function FscCommand.FscOutputLine.Warning ("FS2003", desc) -> true | _ -> false)
        
        match w with
        | None -> 
            Assert.failf "expected warning FS2003"
        | Some (FscCommand.FscOutputLine.Warning("FS2003", desc)) ->
            StringAssert.Contains ("System.Reflection.AssemblyInformationalVersionAttribute", desc)
            StringAssert.Contains ("45.2048.main1.2-hotfix (upgrade Second Chance security)", desc)
        | Some warning -> 
            Assert.failf "expected warning FS2003, but was %A" warning

        })

