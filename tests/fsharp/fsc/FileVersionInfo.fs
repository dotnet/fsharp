module ``FSharp-Tests-Fsc-FileVersionInfo``

open System
open System.IO
open NUnit.Framework

open NUnitConf
open PlatformHelpers
open FSharpTestSuiteTypes

let testContext = FSharpTestSuite.testContext

open System.Reflection

module FileVersionInfoTest =

    [<Test; FSharpSuiteTest()>]
    let ``should set file version info on generated file`` () = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let fscToLibrary = Printf.ksprintf (FscCommand.fscToLibrary dir (Command.exec dir cfg.EnvironmentVariables) cfg.FSC)

        printfn "Directory: %s" dir

        let code _name file =
            fprintf file "%s" """
namespace CST.RI.Anshun
open System.Reflection
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
[<assembly: AssemblyTitle("CST.RI.Anshun.TreloarStation")>]
[<assembly: AssemblyDescription("Assembly is a part of Restricted Intelligence of Anshun planet")>]
[<assembly: AssemblyConfiguration("RELEASE")>]
[<assembly: AssemblyCompany("Compressed Space Transport")>]
[<assembly: AssemblyProduct("CST.RI.Anshun")>]
[<assembly: AssemblyCopyright("Copyright \u00A9 Compressed Space Transport 2380")>]
[<assembly: AssemblyTrademark("CST \u2122")>]
[<assembly: AssemblyVersion("12.34.56.78")>]
[<assembly: AssemblyFileVersion("99.88.77.66")>]
[<assembly: AssemblyInformationalVersion("17.56.2912.14")>]
()
            """

        let! result = fscToLibrary "%s --nologo" cfg.fsc_flags { 
            SourceFiles = [ SourceFile.Content("test.fs", code) ]
            OutLibrary = "lib.dll" }

        let fv = System.Diagnostics.FileVersionInfo.GetVersionInfo(result.OutLibraryFullPath)
        fv.CompanyName |> Assert.areEqual "Compressed Space Transport"
        fv.FileVersion |> Assert.areEqual "99.88.77.66"
        
        (fv.FileMajorPart, fv.FileMinorPart, fv.FileBuildPart, fv.FilePrivatePart)
        |> Assert.areEqual (99,88,77,66)
        
        fv.ProductVersion |> Assert.areEqual "17.56.2912.14"
        (fv.ProductMajorPart, fv.ProductMinorPart, fv.ProductBuildPart, fv.ProductPrivatePart) 
        |> Assert.areEqual (17,56,2912,14)
        
        fv.LegalCopyright |> Assert.areEqual "Copyright \u00A9 Compressed Space Transport 2380"
        fv.LegalTrademarks |> Assert.areEqual "CST \u2122"
        
        result.StderrText
        |> FscCommand.parseFscOut 
        |> List.choose (function FscCommand.FscOutputLine.Warning(w,e) -> Some w | _ -> None)
        |> Assert.areEqual []
    
        })
