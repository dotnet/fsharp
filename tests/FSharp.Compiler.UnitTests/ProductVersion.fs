namespace FSharp.Compiler.UnitTests

open System
open System.IO
open System.Text
open Xunit
open FSharp.Test

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CreateILModule.MainModuleBuilder

#nowarn "3180"

module FileVersionTest =

    let fileVersionAttrName = typeof<System.Reflection.AssemblyFileVersionAttribute>.FullName

    [<Fact>]
    let parseILVersion () =
        "0.0.0.0" |> parseILVersion |> Assert.shouldBe (ILVersionInfo(0us,0us,0us,0us))
        "1.2.3.4" |> parseILVersion |> Assert.shouldBe (ILVersionInfo(1us,2us,3us,4us))

    [<Fact>]
    let ``should use AssemblyFileVersionAttribute if set`` () =
        let findStringAttr n = n |> Assert.shouldBe fileVersionAttrName; Some "1.2.3.4"
        fileVersion findStringAttr (ILVersionInfo(1us,0us,0us,0us)) |> Assert.shouldBe (ILVersionInfo(1us,2us,3us,4us))

    [<Fact>] 
    let ``should fallback if AssemblyFileVersionAttribute is not a valid version`` () =
        fileVersion (fun _ -> Some "1.2a.3.3") (ILVersionInfo(3us,7us,8us,6us))
        |> Assert.shouldBe (ILVersionInfo(3us,7us,8us,6us))

    [<Fact>] 
    let ``should fallback to assemblyVersion if AssemblyFileVersionAttribute not set`` () = 
        let findStringAttr n = n |> Assert.shouldBe fileVersionAttrName; None;
        fileVersion findStringAttr (ILVersionInfo(1us,0us,0us,4us)) |> Assert.shouldBe (ILVersionInfo(1us,0us,0us,4us))

module ProductVersionTest =

    let informationalVersionAttrName = typeof<System.Reflection.AssemblyInformationalVersionAttribute>.FullName
    let fileVersionAttrName = typeof<System.Reflection.AssemblyFileVersionAttribute>.FullName

    [<Fact>] 
    let ``should use AssemblyInformationalVersionAttribute if set`` () = 
        let mutable args = []
        let findStrAttr x = args <- List.append args [x]; Some "12.34.56.78"
        productVersion findStrAttr (ILVersionInfo(1us,0us,0us,6us)) |> Assert.shouldBe "12.34.56.78"
        args |> Assert.shouldBe [ informationalVersionAttrName ]

    [<Fact>] 
    let ``should fallback if AssemblyInformationalVersionAttribute is not a valid version`` () =
        productVersion (fun _ -> Some "1.2.3-main (build #12)") (ILVersionInfo(1us,0us,0us,6us))
        |> Assert.shouldBe "1.2.3-main (build #12)"

    [<Fact>] 
    let ``should fallback to fileVersion if AssemblyInformationalVersionAttribute not set or empty`` () =
        productVersion (fun _ -> None) (ILVersionInfo(3us,2us,1us,0us)) |> Assert.shouldBe "3.2.1.0"
        productVersion (fun _ -> Some "") (ILVersionInfo(3us,2us,1us,0us)) |> Assert.shouldBe "3.2.1.0"

    let internal validValues () =
        let max = System.UInt16.MaxValue
        [ "1.2.3.4", ILVersionInfo(1us,2us,3us,4us)
          "1.0.3.4", ILVersionInfo(1us,0us,3us,4us)
          "7.0.0.4-SomeExtraInformation", ILVersionInfo(7us,0us,0us,4us)
          "0.0.0.0", ILVersionInfo(0us,0us,0us,0us) 
          "3213.57843.32382.59493", ILVersionInfo(3213us,57843us,32382us,59493us)
          (sprintf "%d.%d.%d.%d" max max max max), ILVersionInfo(max,max,max,max) ]

    [<Fact>]
    let ``should use values if valid major.minor.revision.build version format`` () =
        for (v, expected) in validValues() do 
            v |> productVersionToILVersionInfo |> Assert.shouldBe expected

    let internal invalidValues () =
        [ "1.2.3.4", ILVersionInfo(1us,2us,3us,4us)
          "1.2.3.4a", ILVersionInfo(1us,2us,3us,4us)
          "1.2.c3.4", ILVersionInfo(1us,2us,0us,4us)
          "1.2-d.3.4", ILVersionInfo(1us,0us,3us,4us)
          "1dd.2.3.4", ILVersionInfo(0us,2us,3us,4us)
          "1dd.2da.d3hj.dd4ds", ILVersionInfo(0us,0us,0us,0us)
          "1.5.6.7.dasd", ILVersionInfo(1us,5us,6us,7us)
          "9.3", ILVersionInfo(9us,3us,0us,0us)
          "", ILVersionInfo(0us,0us,0us,0us)
          "70000.80000.90000.100000", ILVersionInfo(0us,0us,0us,0us)
          (sprintf "%d.70000.80000.90000" System.UInt16.MaxValue), ILVersionInfo(System.UInt16.MaxValue,0us,0us,0us) ]

    [<Fact>]
    let ``should zero starting from first invalid version part`` () = 
        for (v, expected) in invalidValues() do
            v |> productVersionToILVersionInfo |> Assert.shouldBe expected
