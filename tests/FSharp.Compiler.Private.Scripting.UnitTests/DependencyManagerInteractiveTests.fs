// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Scripting.DependencyManager.UnitTests

open System
open System.Collections.Generic
open System.IO
open System.Reflection
open System.Runtime.InteropServices
open FSharp.Compiler.Interactive.Shell
open FSharp.Compiler.Scripting
open FSharp.Compiler.SourceCodeServices
open System.Runtime.InteropServices
open NUnit.Framework
open Microsoft.DotNet.DependencyManager
open FSharp.Compiler.Scripting.UnitTests
open System.Threading

module Native =
    [<DllImport("NoneExistentDll")>]
    extern int NoneSuch()

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

        let nativeProbingRoots () = Seq.empty<string>

        use dp = new DependencyProvider(NativeResolutionProbe(nativeProbingRoots))
        let reportError =
            let report errorType code message =
                match errorType with
                | ErrorReportType.Error -> printfn "PackageManagementError %d : %s" code message
                | ErrorReportType.Warning -> printfn "PackageManagementWarning %d : %s" code message
            ResolvingErrorReport (report)

        let idm = dp.TryFindDependencyManagerByKey(Seq.empty, "", reportError, "nuget")

        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            let result = dp.Resolve(idm, ".fsx", [|"FSharp.Data"|], reportError, "net472")
            Assert.AreEqual(true, result.Success)
            Assert.AreEqual(1, result.Resolutions |> Seq.length)
            Assert.AreEqual(1, result.SourceFiles |> Seq.length)
            Assert.AreEqual(1, result.Roots |> Seq.length)

        let result = dp.Resolve(idm, ".fsx", [|"FSharp.Data"|], reportError, "netcoreapp3.1")
        Assert.AreEqual(true, result.Success)
        Assert.AreEqual(1, result.Resolutions |> Seq.length)
        Assert.AreEqual(1, result.SourceFiles |> Seq.length)
        Assert.AreEqual(1, result.Roots |> Seq.length)
        ()


    [<Test>]
    member __.``Dependency add with nonexistent package should fail``() =

        let nativeProbingRoots () = Seq.empty<string>

        use dp = new DependencyProvider(NativeResolutionProbe(nativeProbingRoots))
        let reportError =
            let report errorType code message =
                match errorType with
                | ErrorReportType.Error -> printfn "PackageManagementError %d : %s" code message
                | ErrorReportType.Warning -> printfn "PackageManagementWarning %d : %s" code message
            ResolvingErrorReport (report)

        let idm = dp.TryFindDependencyManagerByKey(Seq.empty, "", reportError, "nuget")

        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            let result = dp.Resolve(idm, ".fsx", [|"System.Collections.Immutable.DoesNotExist"|], reportError, "net472")
            Assert.AreEqual(false, result.Success)
            Assert.AreEqual(0, result.Resolutions |> Seq.length)
            Assert.AreEqual(0, result.SourceFiles |> Seq.length)
            Assert.AreEqual(0, result.Roots |> Seq.length)

        let result = dp.Resolve(idm, ".fsx", [|"System.Collections.Immutable.DoesNotExist"|], reportError, "netcoreapp3.1")
        Assert.AreEqual(false, result.Success)
        Assert.AreEqual(0, result.Resolutions |> Seq.length)
        Assert.AreEqual(0, result.SourceFiles |> Seq.length)
        Assert.AreEqual(0, result.Roots |> Seq.length)
        ()


    [<Test>]
    member __.``Multiple Instances of DependencyProvider should be isolated``() =

        let assemblyProbingPaths () = Seq.empty<string>
        let nativeProbingRoots () = Seq.empty<string>

        use dp1 = new DependencyProvider(NativeResolutionProbe(nativeProbingRoots))
        let reportError =
            let report errorType code message =
                match errorType with
                | ErrorReportType.Error -> printfn "PackageManagementError %d : %s" code message
                | ErrorReportType.Warning -> printfn "PackageManagementWarning %d : %s" code message
            ResolvingErrorReport (report)

        let idm1 = dp1.TryFindDependencyManagerByKey(Seq.empty, "", reportError, "nuget")
        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            let result1 = dp1.Resolve(idm1, ".fsx", [|"FSharp.Data"|], reportError, "net472")
            Assert.AreEqual(true, result1.Success)
            Assert.AreEqual(1, result1.Resolutions |> Seq.length)
            Assert.IsTrue((result1.Resolutions |> Seq.head).Contains("\\net45\\"))
            Assert.AreEqual(1, result1.SourceFiles |> Seq.length)
            Assert.AreEqual(1, result1.Roots |> Seq.length)
            Assert.IsTrue((result1.Roots |> Seq.head).EndsWith("/fsharp.data/3.3.3/"))

        let result2 = dp1.Resolve(idm1, ".fsx", [|"FSharp.Data, 3.3.3"|], reportError, "netcoreapp3.1")
        Assert.AreEqual(true, result2.Success)
        Assert.AreEqual(1, result2.Resolutions |> Seq.length)
        let expected2 =
            if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
                "\\netstandard2.0\\"
            else
                "/netstandard2.0/"
        Assert.IsTrue((result2.Resolutions |> Seq.head).Contains(expected2))
        Assert.AreEqual(1, result2.SourceFiles |> Seq.length)
        Assert.AreEqual(1, result2.Roots |> Seq.length)
        Assert.IsTrue((result2.Roots |> Seq.head).EndsWith("/fsharp.data/3.3.3/"))

        use dp2 = new DependencyProvider(NativeResolutionProbe(nativeProbingRoots))
        let idm2 = dp2.TryFindDependencyManagerByKey(Seq.empty, "", reportError, "nuget")

        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            let result3 = dp2.Resolve(idm2, ".fsx", [|"System.Json, Version=4.6.0"|], reportError, "net472")
            Assert.AreEqual(true, result3.Success)
            Assert.AreEqual(1, result3.Resolutions |> Seq.length)
            Assert.IsTrue((result3.Resolutions |> Seq.head).Contains("\\netstandard2.0\\"))
            Assert.AreEqual(1, result3.SourceFiles |> Seq.length)
            Assert.AreEqual(1, result3.SourceFiles |> Seq.length)
            Assert.IsTrue((result3.Roots |> Seq.head).EndsWith("/system.json/4.6.0/"))

        let result4 = dp2.Resolve(idm2, ".fsx", [|"System.Json, Version=4.6.0"|], reportError, "netcoreapp3.1")
        Assert.AreEqual(true, result4.Success)
        Assert.AreEqual(1, result4.Resolutions |> Seq.length)
        let expected4 =
            if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
                "\\netstandard2.0\\"
            else
                "/netstandard2.0/"
        Assert.IsTrue((result4.Resolutions |> Seq.head).Contains(expected4))
        Assert.AreEqual(1, result4.SourceFiles |> Seq.length)
        Assert.AreEqual(1, result4.Roots |> Seq.length)
        Assert.IsTrue((result4.Roots |> Seq.head).EndsWith("/system.json/4.6.0/"))
        ()

    [<Test>]
    member __.``Nuget Reference package with dependencies we should get package roots and dependent references``() =

        let nativeProbingRoots () = Seq.empty<string>

        use dp1 = new DependencyProvider(NativeResolutionProbe(nativeProbingRoots))
        let reportError =
            let report errorType code message =
                match errorType with
                | ErrorReportType.Error -> printfn "PackageManagementError %d : %s" code message
                | ErrorReportType.Warning -> printfn "PackageManagementWarning %d : %s" code message
            ResolvingErrorReport (report)

        let idm1 = dp1.TryFindDependencyManagerByKey(Seq.empty, "", reportError, "nuget")

        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            let result1 = dp1.Resolve(idm1, ".fsx", [|"Microsoft.Extensions.Configuration.Abstractions, 3.1.1"|], reportError, "net472")
            Assert.AreEqual(true, result1.Success)
            Assert.AreEqual(6, result1.Resolutions |> Seq.length)
            Assert.IsTrue((result1.Resolutions |> Seq.head).Contains("\\netstandard2.0\\"))
            Assert.AreEqual(1, result1.SourceFiles |> Seq.length)
            Assert.AreEqual(6, result1.Roots |> Seq.length)
            Assert.IsTrue((result1.Roots |> Seq.head).EndsWith("/microsoft.extensions.configuration.abstractions/3.1.1/"))

        // Netstandard gets fewer dependencies than desktop, because desktop framework doesn't contain assemblies like System.Memory
        // Those assemblies must be delivered by nuget for desktop apps
        let result2 = dp1.Resolve(idm1, ".fsx", [|"Microsoft.Extensions.Configuration.Abstractions, 3.1.1"|], reportError, "netcoreapp3.1")
        Assert.AreEqual(true, result2.Success)
        Assert.AreEqual(2, result2.Resolutions |> Seq.length)
        let expected =
            if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
                "\\netcoreapp3.1\\"
            else
                "/netcoreapp3.1/"
        Assert.IsTrue((result2.Resolutions |> Seq.head).Contains(expected))
        Assert.AreEqual(1, result2.SourceFiles |> Seq.length)
        Assert.AreEqual(2, result2.Roots |> Seq.length)
        Assert.IsTrue((result2.Roots |> Seq.head).EndsWith("/microsoft.extensions.configuration.abstractions/3.1.1/"))
        ()

/// Native dll resolution is not implemented on desktop
#if NETCOREAPP
    [<Test>]
    member __.``Script using TorchSharp``() =
        let text = """
#r "nuget:RestoreSources=https://donsyme.pkgs.visualstudio.com/TorchSharp/_packaging/packages2/nuget/v3/index.json"
#r "nuget:libtorch-cpu,0.3.52118"
#r "nuget:TorchSharp,0.3.52118"

TorchSharp.Tensor.LongTensor.From([| 0L .. 100L |]).Device
"""

        if RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            use script = scriptHost()
            let opt = script.Eval(text) |> getValue
            let value = opt.Value
            Assert.AreEqual(typeof<string>, value.ReflectionType)
            Assert.AreEqual("cpu", value.ReflectionValue :?> string)
        ()


    [<Test>]
    member __.``Use Dependency Manager to restore packages with native dependencies, build and run script that depends on the results``() =
        let packagemanagerlines = [|
            "RestoreSources=https://dotnet.myget.org/F/dotnet-corefxlab/api/v3/index.json"
            "Microsoft.ML,version=1.4.0-preview"
            "Microsoft.ML.AutoML,version=0.16.0-preview"
            "Microsoft.Data.DataFrame,version=0.1.1-e191008-1"
        |]

        let reportError =
            let report errorType code message =
                match errorType with
                | ErrorReportType.Error -> printfn "PackageManagementError %d : %s" code message
                | ErrorReportType.Warning -> printfn "PackageManagementWarning %d : %s" code message
            ResolvingErrorReport (report)

        let mutable resolverPackageRoots = Seq.empty<string>
        let mutable resolverPackageRoots = Seq.empty<string>
        let mutable resolverReferences = Seq.empty<string>

        let nativeProbingRoots () = resolverPackageRoots
        let assemblyProbingPaths () = resolverReferences

        // Restore packages, Get Reference dll paths and package roots
        let result =
            use dp = new DependencyProvider(AssemblyResolutionProbe(assemblyProbingPaths), NativeResolutionProbe(nativeProbingRoots))
            let idm = dp.TryFindDependencyManagerByKey(Seq.empty, "", reportError, "nuget")
            dp.Resolve(idm, ".fsx", packagemanagerlines, reportError, "netcoreapp3.1")

        Assert.IsTrue(result.Success, "resolve failed")

        resolverPackageRoots <- result.Roots
        resolverReferences <- result.Resolutions

// Build and execute reference
        let referenceText =
#if DISABLED_DUE_TO_ISSUE_8588
//
// https://github.com/dotnet/fsharp/issues/8588
            // For this test case use Assembly Qualified References
            ("", result.Resolutions)
            ||> Seq.fold(fun acc r ->
                let assemblyName = AssemblyName.GetAssemblyName(r)
                acc + "#r @\"" + assemblyName.FullName + "\"" + Environment.NewLine)
#else
            // use standard #r for now
            ("", result.Resolutions)
            ||> Seq.fold(fun acc r ->
                acc + "#r @\"" + r + "\"" + Environment.NewLine)
#endif

        let code = @"
$(REFERENCES)

open System
open System.IO
open System.Linq
open Microsoft.Data

let Shuffle (arr:int[]) =
    let rnd = Random()
    for i in 0 .. arr.Length - 1 do
        let r = i + rnd.Next(arr.Length - i)
        let temp = arr.[r]
        arr.[r] <- arr.[i]
        arr.[i] <- temp
    arr

let housingPath = ""housing.csv""
let housingData = DataFrame.ReadCsv(housingPath)
let randomIndices = (Shuffle(Enumerable.Range(0, (int (housingData.RowCount) - 1)).ToArray()))
let testSize = int (float (housingData.RowCount) * 0.1)
let trainRows = randomIndices.[testSize..]
let testRows = randomIndices.[..testSize]
let housing_train = housingData.[trainRows]

open Microsoft.ML
open Microsoft.ML.Data
open Microsoft.ML.AutoML

let mlContext = MLContext()
let experiment = mlContext.Auto().CreateRegressionExperiment(maxExperimentTimeInSeconds = 15u)
let result = experiment.Execute(housing_train, labelColumnName = ""median_house_value"")
let details = result.RunDetails
printfn ""%A"" result
123
"
        let scriptText = code.Replace("$(REFERENCES)", referenceText)

        // Use the dependency manager to resolve assemblies and native paths
        use dp = new DependencyProvider(AssemblyResolutionProbe(assemblyProbingPaths), NativeResolutionProbe(nativeProbingRoots))

        use script = new FSharpScript()
        let opt = script.Eval(scriptText)  |> getValue
        let value = opt.Value
        Assert.AreEqual(123, value.ReflectionValue :?> int32)

    [<Test>]
    member __.``Use NativeResolver to resolve native dlls.``() =
        let packagemanagerlines = [|
            "RestoreSources=https://dotnet.myget.org/F/dotnet-corefxlab/api/v3/index.json"
            "Microsoft.ML,version=1.4.0-preview"
            "Microsoft.ML.AutoML,version=0.16.0-preview"
            "Microsoft.Data.DataFrame,version=0.1.1-e191008-1"
        |]

        let reportError =
            let report errorType code message =
                match errorType with
                | ErrorReportType.Error -> printfn "PackageManagementError %d : %s" code message
                | ErrorReportType.Warning -> printfn "PackageManagementWarning %d : %s" code message
            ResolvingErrorReport (report)

        let mutable resolverPackageRoots = Seq.empty<string>
        let mutable resolverPackageRoots = Seq.empty<string>

        let mutable resolverReferences = Seq.empty<string>
        let nativeProbingRoots () = resolverPackageRoots
        let assemblyPaths () = resolverReferences

        // Restore packages, Get Reference dll paths and package roots
        let result =
            use dp = new DependencyProvider(NativeResolutionProbe(nativeProbingRoots))
            let idm = dp.TryFindDependencyManagerByKey(Seq.empty, "", reportError, "nuget")
            dp.Resolve(idm, ".fsx", packagemanagerlines, reportError, "netcoreapp3.1")

        Assert.IsTrue(result.Success, "resolve failed")

        resolverPackageRoots <- result.Roots
        resolverReferences <- result.Resolutions

        use _nativeDepencyResolver = new NativeDllResolveHandler(NativeResolutionProbe(nativeProbingRoots))

        // Build and execute script
        let referenceText =
            ("", result.Resolutions) ||> Seq.fold(fun acc r -> acc + @"#r @""" + r + "\"" + Environment.NewLine)

        let code = @"
$(REFERENCES)

open System
open System.IO
open System.Linq
open Microsoft.Data

let Shuffle (arr:int[]) =
    let rnd = Random()
    for i in 0 .. arr.Length - 1 do
        let r = i + rnd.Next(arr.Length - i)
        let temp = arr.[r]
        arr.[r] <- arr.[i]
        arr.[i] <- temp
    arr

let housingPath = ""housing.csv""
let housingData = DataFrame.ReadCsv(housingPath)
let randomIndices = (Shuffle(Enumerable.Range(0, (int (housingData.RowCount) - 1)).ToArray()))
let testSize = int (float (housingData.RowCount) * 0.1)
let trainRows = randomIndices.[testSize..]
let testRows = randomIndices.[..testSize]
let housing_train = housingData.[trainRows]

open Microsoft.ML
open Microsoft.ML.Data
open Microsoft.ML.AutoML

let mlContext = MLContext()
let experiment = mlContext.Auto().CreateRegressionExperiment(maxExperimentTimeInSeconds = 15u)
let result = experiment.Execute(housing_train, labelColumnName = ""median_house_value"")
let details = result.RunDetails
printfn ""%A"" result
123
"
        let scriptText = code.Replace("$(REFERENCES)", referenceText)

        use script = new FSharpScript()
        let opt = script.Eval(scriptText)  |> getValue
        let value = opt.Value
        Assert.AreEqual(123, value.ReflectionValue :?> int32)

    [<Test>]
    member __.``Use AssemblyResolver to resolve assemblies``() =
        let packagemanagerlines = [|
            "RestoreSources=https://dotnet.myget.org/F/dotnet-corefxlab/api/v3/index.json"
            "Microsoft.ML,version=1.4.0-preview"
            "Microsoft.ML.AutoML,version=0.16.0-preview"
            "Microsoft.Data.DataFrame,version=0.1.1-e191008-1"
        |]

        let reportError =
            let report errorType code message =
                match errorType with
                | ErrorReportType.Error -> printfn "PackageManagementError %d : %s" code message
                | ErrorReportType.Warning -> printfn "PackageManagementWarning %d : %s" code message
            ResolvingErrorReport (report)

        let mutable resolverPackageRoots = Seq.empty<string>
        let mutable resolverReferences = Seq.empty<string>

        let nativeProbingRoots () = resolverPackageRoots
        let assemblyProbingPaths () = resolverReferences

        // Restore packages, Get Reference dll paths and package roots
        let result =
            use dp = new DependencyProvider(NativeResolutionProbe(nativeProbingRoots))
            let idm = dp.TryFindDependencyManagerByKey(Seq.empty, "", reportError, "nuget")
            dp.Resolve(idm, ".fsx", packagemanagerlines, reportError, "netcoreapp3.1")

        Assert.IsTrue(result.Success, "resolve failed")

        resolverPackageRoots <- result.Roots
        resolverReferences <- result.Resolutions

        use _assemblyResolver = new AssemblyResolveHandler(AssemblyResolutionProbe(assemblyProbingPaths))

        // Build and execute script
        let referenceText =
            ("", result.Resolutions)
            ||> Seq.fold(fun acc r ->
                acc + "    @\"" + r + "\";" + Environment.NewLine)

        let code = """
open System.Reflection

let x = [|
$(REFERENCES)
    |]

x |> Seq.iter(fun r ->
    let name = AssemblyName.GetAssemblyName(r)
    let asm = Assembly.Load(name)
    printfn "%A" (asm.FullName)
    )
123
"""
        let scriptText = code.Replace("$(REFERENCES)", referenceText)

        use script = new FSharpScript()
        let opt = script.Eval(scriptText)  |> getValue
        let value = opt.Value
        Assert.AreEqual(123, value.ReflectionValue :?> int32)

    [<Test>]
    member __.``Verify that referencing FSharp.Core fails with FSharp Scripts``() =
        let packagemanagerlines = [| "FSharp.Core,version=4.7.1" |]

        let reportError =
            let report errorType code message =
                match errorType with
                | ErrorReportType.Error -> printfn "PackageManagementError %d : %s" code message
                | ErrorReportType.Warning -> printfn "PackageManagementWarning %d : %s" code message
            ResolvingErrorReport (report)

        let mutable resolverPackageRoots = Seq.empty<string>
        let mutable resolverReferences = Seq.empty<string>

        let nativeProbingRoots () = resolverPackageRoots
        let assemblyProbingPaths () = resolverReferences

        // Restore packages, Get Reference dll paths and package roots
        let result =
            use dp = new DependencyProvider(NativeResolutionProbe(nativeProbingRoots))
            let idm = dp.TryFindDependencyManagerByKey(Seq.empty, "", reportError, "nuget")
            dp.Resolve(idm, ".fsx", packagemanagerlines, reportError, "netcoreapp3.1")

        // Expected: error FS3217: PackageManager can not reference the System Package 'FSharp.Core'
        Assert.IsFalse(result.Success, "resolve succeeded but should have failed")

    [<Test>]
    member __.``Verify that referencing FSharp.Core succeeds with CSharp Scripts``() =
        let packagemanagerlines = [| "FSharp.Core,version=4.7.1" |]

        let reportError =
            let report errorType code message =
                match errorType with
                | ErrorReportType.Error -> printfn "PackageManagementError %d : %s" code message
                | ErrorReportType.Warning -> printfn "PackageManagementWarning %d : %s" code message
            ResolvingErrorReport (report)

        let mutable resolverPackageRoots = Seq.empty<string>
        let mutable resolverReferences = Seq.empty<string>

        let nativeProbingRoots () = resolverPackageRoots
        let assemblyProbingPaths () = resolverReferences

        // Restore packages, Get Reference dll paths and package roots
        let result =
            use dp = new DependencyProvider(NativeResolutionProbe(nativeProbingRoots))
            let idm = dp.TryFindDependencyManagerByKey(Seq.empty, "", reportError, "nuget")
            dp.Resolve(idm, ".csx", packagemanagerlines, reportError, "netcoreapp3.1")

        Assert.IsTrue(result.Success, "resolve failed but should have succeeded")


    [<Test>]
    member __.``Verify that Dispose on DependencyProvider unhooks ResolvingUnmanagedDll event handler``() =

        let mutable found = false
        let nativeProbingRoots () =
            found <- true
            Seq.empty<string>

        let reportError =
            let report errorType code message =
                match errorType with
                | ErrorReportType.Error -> printfn "PackageManagementError %d : %s" code message
                | ErrorReportType.Warning -> printfn "PackageManagementWarning %d : %s" code message
            ResolvingErrorReport (report)

        // Set up native resolver to resolve dll's
        do
            use dp = new DependencyProvider(NativeResolutionProbe(nativeProbingRoots))

            // Invoking a non-existent dll via pinvoke cause a probe. which should invoke the call back
            try Native.NoneSuch() |> ignore with _ -> ()
            Assert.IsTrue (found, "Failed to invoke the nativeProbingRoots callback")

        // Here the dispose was invoked which should clear the ResolvingUnmanagedDll handler
        found <- false
        try Native.NoneSuch() |> ignore with _ -> ()
        Assert.IsFalse (found, "Invoke the nativeProbingRoots callback -- Error the ResolvingUnmanagedDll still fired ")

        use dp = new DependencyProvider(NativeResolutionProbe(nativeProbingRoots))
        let idm = dp.TryFindDependencyManagerByKey(Seq.empty, "", reportError, "nuget")

        if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then
            let result = dp.Resolve(idm, ".fsx", [|"FSharp.Data"|], reportError, "net472")
            Assert.AreEqual(true, result.Success)
            Assert.AreEqual(1, result.Resolutions |> Seq.length)
            Assert.AreEqual(1, result.SourceFiles |> Seq.length)
            Assert.AreEqual(1, result.Roots |> Seq.length)

        let result = dp.Resolve(idm, ".fsx", [|"FSharp.Data"|], reportError, "netcoreapp3.1")
        Assert.AreEqual(true, result.Success)
        Assert.AreEqual(1, result.Resolutions |> Seq.length)
        Assert.AreEqual(1, result.SourceFiles |> Seq.length)
        Assert.AreEqual(1, result.Roots |> Seq.length)
        ()


    [<Test>]
    member __.``Verify that Dispose on DependencyProvider unhooks ResolvingUnmanagedDll and AssemblyResolver event handler``() =

        let mutable assemblyFound = false
        let assemblyProbingPaths () =
            assemblyFound <- true
            Seq.empty<string>

        let mutable nativeFound = false
        let nativeProbingRoots () =
            nativeFound <- true
            Seq.empty<string>

        // Set up native resolver to resolve dll's
        do
            use dp = new DependencyProvider(AssemblyResolutionProbe(assemblyProbingPaths), NativeResolutionProbe(nativeProbingRoots))

            // Invoking a non-existent dll via pinvoke cause a probe. which should invoke the call back
            try Native.NoneSuch() |> ignore with _ -> ()
            Assert.IsTrue (nativeFound, "Failed to invoke the nativeProbingRoots callback")

            // Invoking a non-existent assembly causes a probe. which should invoke the call back
            try Assembly.Load("NoneSuchAssembly") |> ignore with _ -> ()
            Assert.IsTrue (assemblyFound, "Failed to invoke the AssemblyResolve handler")

        // Here the dispose was invoked which should clear the ResolvingUnmanagedDll handler
        nativeFound <- false
        assemblyFound <- false

        try Native.NoneSuch() |> ignore with _ -> ()
        Assert.IsFalse (nativeFound, "Invoke the nativeProbingRoots callback -- Error the ResolvingUnmanagedDll still fired ")

        try Assembly.Load("NoneSuchAssembly") |> ignore with _ -> ()
        Assert.IsFalse (assemblyFound, "Invoke the assemblyProbingRoots callback -- Error the AssemblyResolve still fired ")
#endif

    [<Test>]
    member __.``Verify that Dispose on AssemblyResolveHandler unhooks AssemblyResolve event handler``() =

        let mutable assemblyFound = false
        let assemblyProbingPaths () =
            assemblyFound <- true
            Seq.empty<string>

        // Set up AssemblyResolver to resolve dll's
        do
            use dp = new AssemblyResolveHandler(AssemblyResolutionProbe(assemblyProbingPaths))

            // Invoking a non-existent assembly causes a probe. which should invoke the call back
            try Assembly.Load("NoneSuchAssembly") |> ignore with _ -> ()
            Assert.IsTrue (assemblyFound, "Failed to invoke the AssemblyResolve handler")

        // Here the dispose was invoked which should clear the ResolvingUnmanagedDll handler
        assemblyFound <- false

        try Assembly.Load("NoneSuchAssembly") |> ignore with _ -> ()
        Assert.IsFalse (assemblyFound, "Invoke the assemblyProbingRoots callback -- Error the AssemblyResolve still fired ")


    [<Test>]
    member __.``Verify that #help produces help text for fsi + dependency manager``() =
        let expected = [|
            """  F# Interactive directives:"""
            """"""
            """    #r "file.dll";;                   // Reference (dynamically load) the given DLL"""
            """    #I "path";;                       // Add the given search path for referenced DLLs"""
            """    #load "file.fs" ...;;             // Load the given file(s) as if compiled and referenced"""
            """    #time ["on"|"off"];;              // Toggle timing on/off"""
            """    #help;;                           // Display help"""
            """    #quit;;                           // Exit"""
            """"""
            """  F# Interactive command line options:"""
            """"""

            // this is the end of the line each different platform has a different mechanism for starting fsi
            // Actual output looks similar to: """      See 'testhost --help' for options"""
            """--help' for options"""

            """"""
            """"""
        |]

        let mutable found = 0
        let lines = System.Collections.Generic.List()
        use sawExpectedOutput = new ManualResetEvent(false)
        let verifyOutput (line: string) =
            let compareLine (s: string) =
                if s = "" then line = ""
                else line.EndsWith(s)
            lines.Add(line)
            match expected |> Array.tryFind(compareLine) with
            | None -> ()
            | Some t ->
                found <- found + 1
                if found = expected.Length then sawExpectedOutput.Set() |> ignore

        let text = "#help"
        use output = new RedirectConsoleOutput()
        use script = new FSharpScript(quiet = false, langVersion = LangVersion.V47)
        let mutable found = 0
        output.OutputProduced.Add (fun line -> verifyOutput line)
        let opt = script.Eval(text) |> getValue
        Assert.True(sawExpectedOutput.WaitOne(TimeSpan.FromSeconds(5.0)), sprintf "Expected to see error sentinel value written\nexpected:%A\nactual:%A" expected lines)


    [<Test>]
    member __.``Verify that #help produces help text for fsi + dependency manager language version preview``() =
        let expected = [|
            """  F# Interactive directives:"""
            """"""
            """    #r "file.dll";;                   // Reference (dynamically load) the given DLL"""
            """    #I "path";;                       // Add the given search path for referenced DLLs"""
            """    #load "file.fs" ...;;             // Load the given file(s) as if compiled and referenced"""
            """    #time ["on"|"off"];;              // Toggle timing on/off"""
            """    #help;;                           // Display help"""
            """    #r "nuget:FSharp.Data, 3.1.2";;   // Load Nuget Package 'FSharp.Data' version '3.1.2'"""
            """    #r "nuget:FSharp.Data";;          // Load Nuget Package 'FSharp.Data' with the highest version"""
            """    #quit;;                           // Exit"""
            """"""
            """  F# Interactive command line options:"""
            """"""

            // this is the end of the line each different platform has a different mechanism for starting fsi
            // Actual output looks similar to: """      See 'testhost --help' for options"""
            """--help' for options"""

            """"""
            """"""
        |]

        let mutable found = 0
        let lines = System.Collections.Generic.List()
        use sawExpectedOutput = new ManualResetEvent(false)
        let verifyOutput (line: string) =
            let compareLine (s: string) =
                if s = "" then line = ""
                else line.EndsWith(s)
            lines.Add(line)
            match expected |> Array.tryFind(compareLine) with
            | None -> ()
            | Some t ->
                found <- found + 1
                if found = expected.Length then sawExpectedOutput.Set() |> ignore

        let text = "#help"
        use output = new RedirectConsoleOutput()
        use script = new FSharpScript(quiet = false, langVersion = LangVersion.Preview)
        let mutable found = 0
        output.OutputProduced.Add (fun line -> verifyOutput line)
        let opt = script.Eval(text) |> getValue
        Assert.True(sawExpectedOutput.WaitOne(TimeSpan.FromSeconds(5.0)), sprintf "Expected to see error sentinel value written\nexpected:%A\nactual:%A" expected lines)
