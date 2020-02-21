// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.DependencyManager.UnitTests

open System
open System.Collections.Generic
open System.IO
open FSharp.Compiler.Interactive.Shell
open FSharp.Compiler.Scripting
open FSharp.Compiler.SourceCodeServices
open System.Runtime.InteropServices
open NUnit.Framework

open Interactive.DependencyManager

[<TestFixture>]
type DependencyManagerInteractiveTests() =

    let getValue ((value: Result<FsiValue option, exn>), (errors: FSharpErrorInfo[])) =
        if errors.Length > 0 then
            failwith <| sprintf "Evaluation returned %d errors:\r\n\t%s" errors.Length (String.Join("\r\n\t", errors))
        match value with
        | Ok(value) -> value
        | Error ex -> raise ex

    let ignoreValue = getValue >> ignore

    let scriptHost () = new FSharpScript(additionalArgs=[|"/langversion:preview"|])

    let dependencyManager() = new Interactive.DependencyManager.DependencyProvider()

    [<Test>]
    member __.``SmokeTest - #r nuget``() =
        let text = """
#r @"nuget:Newtonsoft.Json, Version=9.0.1"
0"""
        use script = scriptHost()
        let opt = script.Eval(text) |> getValue
        let value = opt.Value
        Assert.AreEqual(typeof<int>, value.ReflectionType)
        Assert.AreEqual(0, value.ReflectionValue :?> int)

    [<Test>]
    member __.``SmokeTest - #r nuget package not found``() =
        let text = """
#r @"nuget:System.Collections.Immutable.DoesNotExist, version=1.5.0"
0"""
        use script = scriptHost()
        let opt = script.Eval(text) |> getValue
        let value = opt.Value
        Assert.AreEqual(typeof<int>, value.ReflectionType)
        Assert.AreEqual(0, value.ReflectionValue :?> int)

    [<Test>]
    member __.``Use Dependency Manager to resolve dependency FSharp.Data``() =
        use dp = new DependencyProvider()
        let reportError (errorType: ErrorReportType) (code: int, message: string) =
            match errorType with
            | ErrorReportType.Error -> printfn "PackageManagementError %d : %s" code message
            | ErrorReportType.Warning -> printfn "PackageManagementWarning %d : %s" code message

        let idm = dp.TryFindDependencyManagerByKey(Seq.empty, "", reportError, "nuget")

        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            let succeeded, references, sources, packageroots = dp.Resolve(idm, "", "", "", ".fsx", [|"FSharp.Data"|], reportError, "net472")
            Assert.AreEqual(true, succeeded)
            Assert.AreEqual(1, references |> Seq.length)
            Assert.AreEqual(1, sources |> Seq.length)
            Assert.AreEqual(1, packageroots |> Seq.length)

        let succeeded, references, sources, packageroots = dp.Resolve(idm, "", "", "", ".fsx", [|"FSharp.Data"|], reportError, "netcoreapp3.1")
        Assert.AreEqual(true, succeeded)
        Assert.AreEqual(1, references |> Seq.length)
        Assert.AreEqual(1, sources |> Seq.length)
        Assert.AreEqual(1, packageroots |> Seq.length)
        ()


    [<Test>]
    member __.``Dependency add with nonexistent package should fail``() =
        use dp = new DependencyProvider()
        let reportError (errorType: ErrorReportType) (code: int, message: string) =
            match errorType with
            | ErrorReportType.Error -> printfn "PackageManagementError %d : %s" code message
            | ErrorReportType.Warning -> printfn "PackageManagementWarning %d : %s" code message

        let idm = dp.TryFindDependencyManagerByKey(Seq.empty, "", reportError, "nuget")

        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            let succeeded, references, sources, packageroots = dp.Resolve(idm, "", "", "", ".fsx", [|"System.Collections.Immutable.DoesNotExist"|], reportError, "net472")
            Assert.AreEqual(false, succeeded)
            Assert.AreEqual(0, references |> Seq.length)
            Assert.AreEqual(0, sources |> Seq.length)
            Assert.AreEqual(0, packageroots |> Seq.length)

        let succeeded, references, sources, packageroots = dp.Resolve(idm, "", "", "", ".fsx", [|"System.Collections.Immutable.DoesNotExist"|], reportError, "netcoreapp3.1")
        Assert.AreEqual(false, succeeded)
        Assert.AreEqual(0, references |> Seq.length)
        Assert.AreEqual(0, sources |> Seq.length)
        Assert.AreEqual(0, packageroots |> Seq.length)
        ()


    [<Test>]
    member __.``Multiple Instances of DependencyProvider should be isolated``() =

        use dp1 = new DependencyProvider()
        let reportError (errorType: ErrorReportType) (code: int, message: string) =
            match errorType with
            | ErrorReportType.Error -> printfn "PackageManagementError %d : %s" code message
            | ErrorReportType.Warning -> printfn "PackageManagementWarning %d : %s" code message

        let idm1 = dp1.TryFindDependencyManagerByKey(Seq.empty, "", reportError, "nuget")
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            let succeeded1, references1, sources1, packageroots1 = dp1.Resolve(idm1, "", "", "", ".fsx", [|"FSharp.Data"|], reportError, "net472")
            Assert.AreEqual(true, succeeded1)
            Assert.AreEqual(1, references1 |> Seq.length)
            Assert.IsTrue((references1 |> Seq.head).Contains("\\net45\\"))
            Assert.AreEqual(1, sources1 |> Seq.length)
            Assert.AreEqual(1, packageroots1 |> Seq.length)
            Assert.IsTrue((packageroots1 |> Seq.head).EndsWith("/fsharp.data/3.3.3/"))

        let succeeded2, references2, sources2, packageroots2 = dp1.Resolve(idm1, "", "", "", ".fsx", [|"FSharp.Data, 3.3.3"|], reportError, "netcoreapp3.1")
        Assert.AreEqual(true, succeeded2)
        Assert.AreEqual(1, references2 |> Seq.length)
        let expected2 =
            if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
                "\\netstandard2.0\\"
            else
                "/netstandard2.0/"
        Assert.IsTrue((references2 |> Seq.head).Contains(expected2))
        Assert.AreEqual(1, sources2 |> Seq.length)
        Assert.AreEqual(1, packageroots2 |> Seq.length)
        Assert.IsTrue((packageroots2 |> Seq.head).EndsWith("/fsharp.data/3.3.3/"))

        use dp2 = new DependencyProvider()
        let idm2 = dp2.TryFindDependencyManagerByKey(Seq.empty, "", reportError, "nuget")

        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            let succeeded3, references3, sources3, packageroots3 = dp2.Resolve(idm2, "", "", "", ".fsx", [|"System.Json, Version=4.6.0"|], reportError, "net472")
            Assert.AreEqual(true, succeeded3)
            Assert.AreEqual(1, references3 |> Seq.length)
            Assert.IsTrue((references3 |> Seq.head).Contains("\\netstandard2.0\\"))
            Assert.AreEqual(1, sources3 |> Seq.length)
            Assert.AreEqual(1, packageroots3 |> Seq.length)
            Assert.IsTrue((packageroots3 |> Seq.head).EndsWith("/system.json/4.6.0/"))

        let succeeded4, references4, sources4, packageroots4 = dp2.Resolve(idm2, "", "", "", ".fsx", [|"System.Json, Version=4.6.0"|], reportError, "netcoreapp3.1")
        Assert.AreEqual(true, succeeded4)
        Assert.AreEqual(1, references4 |> Seq.length)
        let expected4 =
            if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
                "\\netstandard2.0\\"
            else
                "/netstandard2.0/"
        Assert.IsTrue((references4 |> Seq.head).Contains(expected4))
        Assert.AreEqual(1, sources4 |> Seq.length)
        Assert.AreEqual(1, packageroots4 |> Seq.length)
        Assert.IsTrue((packageroots4 |> Seq.head).EndsWith("/system.json/4.6.0/"))
        ()

    [<Test>]
    member __.``Nuget Reference package with dependencies we should get package roots and dependent references``() =

        use dp1 = new DependencyProvider()
        let reportError (errorType: ErrorReportType) (code: int, message: string) =
            match errorType with
            | ErrorReportType.Error -> printfn "PackageManagementError %d : %s" code message
            | ErrorReportType.Warning -> printfn "PackageManagementWarning %d : %s" code message

        let idm1 = dp1.TryFindDependencyManagerByKey(Seq.empty, "", reportError, "nuget")

        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            let succeeded1, references1, sources1, packageroots1 = dp1.Resolve(idm1, "", "", "", ".fsx", [|"Microsoft.Extensions.Configuration.Abstractions, 3.1.1"|], reportError, "net472")
            Assert.AreEqual(true, succeeded1)
            Assert.AreEqual(6, references1 |> Seq.length)
            Assert.IsTrue((references1 |> Seq.head).Contains("\\netstandard2.0\\"))
            Assert.AreEqual(1, sources1 |> Seq.length)
            Assert.AreEqual(6, packageroots1 |> Seq.length)
            Assert.IsTrue((packageroots1 |> Seq.head).EndsWith("/microsoft.extensions.configuration.abstractions/3.1.1/"))

        // Netstandard gets fewer dependencies than desktop, because desktop framework doesn't contain assemblies like System.Memory
        // Those assemblies must be delivered by nuget for desktop apps
        let succeeded2, references2, sources2, packageroots2 = dp1.Resolve(idm1, "", "", "", ".fsx", [|"Microsoft.Extensions.Configuration.Abstractions, 3.1.1"|], reportError, "netcoreapp3.1")
        Assert.AreEqual(true, succeeded2)
        Assert.AreEqual(2, references2 |> Seq.length)
        let expected =
            if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
                "\\netcoreapp3.1\\"
            else
                "/netcoreapp3.1/"
        Assert.IsTrue((references2 |> Seq.head).Contains(expected))
        Assert.AreEqual(1, sources2 |> Seq.length)
        Assert.AreEqual(2, packageroots2 |> Seq.length)
        Assert.IsTrue((packageroots2 |> Seq.head).EndsWith("/microsoft.extensions.configuration.abstractions/3.1.1/"))
        ()

