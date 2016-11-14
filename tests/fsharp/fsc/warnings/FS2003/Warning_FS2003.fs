module ``FSharp-Tests-Fsc-Warnings``

open System
open System.IO
open System.Reflection
open NUnit.Framework
open NUnitConf
open PlatformHelpers
open FSharpTestSuiteTypes


[<Test>]
let ``should be raised if AssemblyInformationalVersion has invalid version`` () = check (attempt {
        let cfg = testConfig (Commands.createTempDir())

        let code  =
            """
namespace CST.RI.Anshun
open System.Reflection
[<assembly: AssemblyVersion("4.5.6.7")>]
[<assembly: AssemblyInformationalVersion("45.2048.main1.2-hotfix (upgrade Second Chance security)")>]
()
            """

        File.WriteAllText(cfg.Directory/"test.fs", code)

        do! fsc cfg "%s --nologo -o:lib.dll --target:library" cfg.fsc_flags ["test.fs"]

        let fv = Diagnostics.FileVersionInfo.GetVersionInfo(Commands.getfullpath cfg.Directory "lib.dll")

        fv.ProductVersion |> Assert.areEqual "45.2048.main1.2-hotfix (upgrade Second Chance security)"

        (fv.ProductMajorPart, fv.ProductMinorPart, fv.ProductBuildPart, fv.ProductPrivatePart) 
        |> Assert.areEqual (45,2048,0,0)

        })

