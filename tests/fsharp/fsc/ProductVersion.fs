module ``FSharp-Tests-Fsc-ProductVersion``

open System
open System.IO
open NUnit.Framework

open NUnitConf
open PlatformHelpers
open FSharpTestSuiteTypes

let testContext = FSharpTestSuite.testContext

open System.Reflection

module ProductVersionTest =

    let informationalVersionAttrName = typeof<System.Reflection.AssemblyInformationalVersionAttribute>.FullName
    let fileVersionAttrName = typeof<System.Reflection.AssemblyFileVersionAttribute>.FullName

    let fallbackTestData () =
        let defAssemblyVersion = (1us,2us,3us,4us)
        let defAssemblyVersionString = let v1,v2,v3,v4 = defAssemblyVersion in sprintf "%d.%d.%d.%d" v1 v2 v3 v4
        [ defAssemblyVersionString, None, None, defAssemblyVersionString
          defAssemblyVersionString, (Some "5.6.7.8"), None, "5.6.7.8"
          defAssemblyVersionString, (Some "5.6.7.8" ), (Some "22.44.66.88"), "22.44.66.88"
          defAssemblyVersionString, None, (Some "22.44.66.88" ), "22.44.66.88" ]
        |> List.map (fun (a,f,i,e) -> FSharpSuiteTestCaseData(Commands.createTempDir(), a, f, i, e))

    [<TestCaseSource("fallbackTestData")>]
    let ``should use correct fallback`` assemblyVersion fileVersion infoVersion expected = check (attempt {
        let { Directory = dir; Config = cfg } = testContext ()

        let fscToLibrary = Printf.ksprintf (fun flags -> FscCommand.fscToLibrary dir (Command.exec dir cfg.EnvironmentVariables) cfg.FSC flags)

        printfn "Directory: %s" dir

        let assemblyAttrsFile _name file =
            let globalAssembly (attr: Type) attrValue =
                sprintf """[<assembly: %s("%s")>]""" attr.FullName attrValue

            let attrs =
                [ assemblyVersion |> (globalAssembly typeof<AssemblyVersionAttribute> >> Some)
                  fileVersion |> Option.map (globalAssembly typeof<AssemblyFileVersionAttribute>)
                  infoVersion |> Option.map (globalAssembly typeof<AssemblyInformationalVersionAttribute>) ]
                |> List.choose id

            fprintf file """
namespace CST.RI.Anshun
%s
()
            """ (attrs |> String.concat Environment.NewLine)

        let! result = fscToLibrary "%s --nologo" cfg.fsc_flags { 
            SourceFiles = [ SourceFile.Content("test.fs", assemblyAttrsFile) ]
            OutLibrary = "lib.dll" }
        
        let fileVersionInfo = Diagnostics.FileVersionInfo.GetVersionInfo(result.OutLibraryFullPath)

        fileVersionInfo.ProductVersion |> Assert.areEqual expected
        })
