// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Functions to retrieve framework dependencies
namespace FSharp.Compiler

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Diagnostics
open System.Globalization
open System.IO
open System.Reflection
open System.Runtime.InteropServices
open System.Text
open Internal.Utilities.FSharpEnvironment
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range

/// Resolves the references for a chosen or currently-executing framework, for
///   - script execution
///   - script editing
///   - script compilation
///   - out-of-project sources editing
///   - default references for fsc.exe
///   - default references for fsi.exe
type internal FxResolver(assumeDotNetFramework: bool option, projectDir: string, useSdkRefs: bool, isInteractive: bool, rangeForErrors: range, sdkDirOverride: string option) =

    static let isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)

    static let dotnet =
        if isWindows then "dotnet.exe" else "dotnet"

    static let getDotnetDirectory() =
        if isRunningOnCoreClr then
            // Probe for netsdk install, dotnet. and dotnet.exe is a constant offset from the location of System.Int32
            let candidate =
                let assemblyLocation = Path.GetDirectoryName(typeof<Int32>.GetTypeInfo().Assembly.Location)
                Path.GetFullPath(Path.Combine(assemblyLocation, "../../.."))

            if Directory.Exists(candidate) then
                Some candidate
            else
                None
        elif isWindows then
            let pf = Environment.GetEnvironmentVariable("ProgramW6432")
            let pf = if String.IsNullOrEmpty(pf) then Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) else pf
            let candidate = Path.Combine(pf,"dotnet")
            if Directory.Exists(candidate) then
                Some candidate
            else None
        else
            None

    static let getDotnetHostPath() =
        match (Environment.GetEnvironmentVariable("DOTNET_HOST_PATH")) with
        | value when not (String.IsNullOrEmpty(value)) ->
            // Set externally
            value
        | _ ->
            match getDotnetDirectory() with
            | Some dotnetDir ->
                let candidate = Path.Combine(dotnetDir, dotnet)
                if File.Exists(candidate) then candidate else dotnet
            | None -> dotnet

    /// We only try once for each directory (cleared on solution unload) to prevent conditions where 
    /// we repeatedly try to run dotnet.exe on every keystroke for a script
    static let desiredDotNetSdkVersionForDirectoryCache = ConcurrentDictionary<string, Result<string, exn>>()

    // Execute the process pathToExe passing the arguments: arguments with the working directory: workingDir timeout after timeout milliseconds -1 = wait forever
    // returns exit code, stdio and stderr as string arrays
    let executeProcess pathToExe arguments (workingDir:string option) timeout =
        if not (String.IsNullOrEmpty pathToExe) then
            let errorsList = ResizeArray()
            let outputList = ResizeArray()
            let mutable errorslock = obj
            let mutable outputlock = obj
            let outputDataReceived (message: string) =
                if not (isNull message) then
                    lock outputlock (fun () -> outputList.Add(message))

            let errorDataReceived (message: string) =
                if not (isNull message) then
                    lock errorslock (fun () -> errorsList.Add(message))

            let psi = ProcessStartInfo()
            psi.FileName <- pathToExe
            if workingDir.IsSome  then
                psi.WorkingDirectory <- workingDir.Value
            psi.RedirectStandardOutput <- true
            psi.RedirectStandardError <- true
            psi.Arguments <- arguments
            psi.CreateNoWindow <- true
            psi.EnvironmentVariables.Remove("MSBuildSDKsPath")          // Host can sometimes add this, and it can break things
            psi.UseShellExecute <- false

            use p = new Process()
            p.StartInfo <- psi

            p.OutputDataReceived.Add(fun a -> outputDataReceived a.Data)
            p.ErrorDataReceived.Add(fun a ->  errorDataReceived a.Data)

            if p.Start() then
                p.BeginOutputReadLine()
                p.BeginErrorReadLine()
                if not(p.WaitForExit(timeout)) then
                    // Timed out resolving throw a diagnostic.
                    raise (new TimeoutException(sprintf "Timeout executing command '%s' '%s'" (psi.FileName) (psi.Arguments)))
                else
                    p.WaitForExit()
#if DEBUG
            if workingDir.IsSome then
                File.WriteAllLines(Path.Combine(workingDir.Value, "StandardOutput.txt"), outputList)
                File.WriteAllLines(Path.Combine(workingDir.Value, "StandardError.txt"), errorsList)
#endif
            p.ExitCode, outputList.ToArray(), errorsList.ToArray()
        else
            -1, Array.empty, Array.empty

    /// Find the relevant sdk version by running `dotnet --version` in the script/project location,
    /// taking into account any global.json
    let tryGetDesiredDotNetSdkVersionForDirectoryInfo() =
        desiredDotNetSdkVersionForDirectoryCache.GetOrAdd(projectDir, (fun _ -> 
            let dotnetHostPath = getDotnetHostPath()
            try
                let workingDir =
                    if Directory.Exists(projectDir) then
                        Some projectDir
                    else
                        None
                let exitCode, output, errors = executeProcess dotnetHostPath "--version" workingDir 30000
                if exitCode <> 0 then
                    Result.Error (Error(FSComp.SR.scriptSdkNotDetermined(dotnetHostPath, projectDir, (errors |> String.concat "\n"), exitCode), rangeForErrors))
                else
                    Result.Ok (output |> String.concat "\n")
            with err -> 
                Result.Error (Error(FSComp.SR.scriptSdkNotDetermined(dotnetHostPath, projectDir, err.Message, 1), rangeForErrors))))

    let tryGetDesiredDotNetSdkVersionForDirectory() =
    // Make sure the warning gets replayed each time we call this
        match tryGetDesiredDotNetSdkVersionForDirectoryInfo() with
        | Result.Ok res -> Some res
        | Result.Error exn -> warning(exn); None

    /// Get the .NET Core SDK directory relevant to projectDir, used to infer the default target framework assemblies.
    let tryGetSdkDir() =
        // This path shouldn't be used with reflective processes
        assert not isInteractive
        match assumeDotNetFramework with 
        | Some true -> None
        | _ when not useSdkRefs -> None
        | _ ->
        match sdkDirOverride with 
        | Some sdkDir -> Some sdkDir
        | None ->
            let sdksDir = 
                match getDotnetDirectory() with
                | Some dotnetDir ->
                    let candidate = Path.GetFullPath(Path.Combine(dotnetDir, "sdk"))
                    if Directory.Exists(candidate) then Some candidate else None
                | None -> None

            match sdksDir with 
            | Some sdksDir ->
                // Find the sdk version by running `dotnet --version` in the script/project location
                let desiredSdkVer = tryGetDesiredDotNetSdkVersionForDirectory()

                let sdkDir =
                    DirectoryInfo(sdksDir).GetDirectories()
                    |> Array.filter (fun di -> di.Name |> Seq.forall (fun c -> Char.IsDigit(c) || c = '.'))
                    // Filter to the version reported by `dotnet --version` in the location, if that succeeded
                    |> Array.filter (fun di -> match desiredSdkVer with None -> true | Some v -> di.Name.Contains(v))
                    |> Array.sortBy (fun di -> di.FullName)
                    |> Array.tryLast
                    |> Option.map (fun di -> di.FullName)
                sdkDir
            | _ -> 
                None

    /// Get the framework implementation directory of the currently running process
    let getRunningImplementationAssemblyDir() =
        let filename = Path.GetDirectoryName(typeof<obj>.Assembly.Location) 
        if String.IsNullOrWhiteSpace filename then getFSharpCompilerLocation() else filename

    /// Get the framework implementation directory, either of the selected SDK or the currently running process as a backup
    /// F# interactive/reflective scenarios use the implementation directory of the currently running process
    let getImplementationAssemblyDir() =
        if isInteractive then
            getRunningImplementationAssemblyDir()
        else
            let sdkDir = tryGetSdkDir()
            match sdkDir with 
            | Some dir -> 
                let dotnetConfigFile = Path.Combine(dir, "dotnet.runtimeconfig.json")
                let dotnetConfig = File.ReadAllText(dotnetConfigFile)
                let pattern = "\"version\": \""
                let startPos = dotnetConfig.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) + pattern.Length
                let endPos = dotnetConfig.IndexOf("\"", startPos)
                let ver = dotnetConfig.[startPos..endPos-1]
                let path = Path.GetFullPath(Path.Combine(dir, "..", "..", "shared", "Microsoft.NETCore.App", ver))
                if Directory.Exists(path) then
                    path
                else
                    getRunningImplementationAssemblyDir()
            | None ->
                let path = getRunningImplementationAssemblyDir()
                path

    let getFSharpCoreLibraryName = "FSharp.Core"

    let getFsiLibraryName = "FSharp.Compiler.Interactive.Settings"

    // Use the FSharp.Core that is executing with the compiler as a backup reference
    let getFSharpCoreImplementationReference() = Path.Combine(getFSharpCompilerLocation(), getFSharpCoreLibraryName + ".dll")

    // Use the FSharp.Compiler.Interactive.Settings executing with the compiler as a backup reference
    let getFsiLibraryImplementationReference() = Path.Combine(getFSharpCompilerLocation(), getFsiLibraryName + ".dll")

    // Use the ValueTuple that is executing with the compiler if it is from System.ValueTuple
    // or the System.ValueTuple.dll that sits alongside the compiler.  (Note we always ship one with the compiler)
    let getSystemValueTupleImplementationReference () =
        let probeFile = Path.Combine(getImplementationAssemblyDir(), "System.ValueTuple.dll")
        if File.Exists(probeFile) then 
            Some probeFile 
        else
            try
                let asm = typeof<System.ValueTuple<int, int>>.Assembly
                if asm.FullName.StartsWith("System.ValueTuple", StringComparison.OrdinalIgnoreCase) then
                    Some asm.Location
                else
                    let valueTuplePath = Path.Combine(getFSharpCompilerLocation(), "System.ValueTuple.dll")
                    if File.Exists(valueTuplePath) then
                        Some valueTuplePath
                    else
                        None
            with _ -> None

    // Algorithm:
    //     use implementation location of obj type, on shared frameworks it will always be in:
    //
    //        dotnet\shared\Microsoft.NETCore.App\sdk-version\System.Private.CoreLib.dll
    //
    //     if that changes we will need to find another way to do this.  Hopefully the sdk will eventually provide an API
    //     use the well know location for obj to traverse the file system towards the
    //
    //          packs\Microsoft.NETCore.App.Ref\sdk-version\netcoreappn.n
    //     we will rely on the sdk-version match on the two paths to ensure that we get the product that ships with the
    //     version of the runtime we are executing on
    //     Use the reference assemblies for the highest netcoreapp tfm that we find in that location.
    let tryGetNetCoreRefsPackDirectoryRoot() =
        try
            //     Use the reference assemblies for the highest netcoreapp tfm that we find in that location that is 
            //     lower than or equal to the implementation version.
            let zeroVersion = Version("0.0.0.0")
            let computeVersion version =
                match Version.TryParse(version) with
                | true, v -> v
                | false, _ -> zeroVersion

            let implDir = getImplementationAssemblyDir()
            let version = computeVersion (DirectoryInfo(implDir).Name)
            let microsoftNETCoreAppRef = Path.Combine(implDir, "../../../packs/Microsoft.NETCore.App.Ref")
            if Directory.Exists(microsoftNETCoreAppRef) then
                let directory = 
                    DirectoryInfo(microsoftNETCoreAppRef).GetDirectories()
                    |> Array.map (fun di -> computeVersion di.Name)
                    |> Array.sort
                    |> Array.filter(fun v -> v <= version)
                    |> Array.last
                Some (directory.ToString()), Some microsoftNETCoreAppRef
            else
               None,  None
        with | _ -> None, None

    // Tries to figure out the tfm for the compiler instance.
    // On coreclr it uses the deps.json file
    let tryGetRunningDotNetCoreTfm() =
        let file =
            try
                let asm = Assembly.GetEntryAssembly()
                match asm with
                | null -> ""
                | asm ->
                    let depsJsonPath = Path.ChangeExtension(asm.Location, "deps.json")
                    if File.Exists(depsJsonPath) then
                        File.ReadAllText(depsJsonPath)
                    else
                        ""
            with _ -> ""

        let tfmPrefix=".NETCoreApp,Version=v"
        let pattern = "\"name\": \"" + tfmPrefix
        let startPos =
            let startPos = file.IndexOf(pattern, StringComparison.OrdinalIgnoreCase)
            if startPos >= 0  then startPos + (pattern.Length) else startPos

        let length =
            if startPos >= 0 then
                let ep = file.IndexOf("\"", startPos)
                if ep >= 0 then ep - startPos else ep
            else -1
        match startPos, length with
        | -1, _
        | _, -1 ->
            if isRunningOnCoreClr then
                // Running on coreclr but no deps.json was deployed with the host so default to 3.1
                Some "netcoreapp3.1"
            else
                // Running on desktop
                None
        | pos, length ->
            // use value from the deps.json file
            Some ("netcoreapp" + file.Substring(pos, length))

    // Tries to figure out the tfm for the compiler instance on the Windows desktop.
    // On full clr it uses the mscorlib version number
    let getRunningDotNetFrameworkTfm () =
        let defaultMscorlibVersion = 4,8,3815,0
        let desktopProductVersionMonikers = [|
            // major, minor, build, revision, moniker
               4,     8,      3815,     0,    "net48"
               4,     8,      3761,     0,    "net48"
               4,     7,      3190,     0,    "net472"
               4,     7,      3062,     0,    "net472"
               4,     7,      2600,     0,    "net471"
               4,     7,      2558,     0,    "net471"
               4,     7,      2053,     0,    "net47"
               4,     7,      2046,     0,    "net47"
               4,     6,      1590,     0,    "net462"
               4,     6,        57,     0,    "net462"
               4,     6,      1055,     0,    "net461"
               4,     6,        81,     0,    "net46"
               4,     0,     30319, 34209,    "net452"
               4,     0,     30319, 17020,    "net452"
               4,     0,     30319, 18408,    "net451"
               4,     0,     30319, 17929,    "net45"
               4,     0,     30319,     1,    "net4"
            |]

        let majorPart, minorPart, buildPart, privatePart=
            try
                let attrOpt = typeof<Object>.Assembly.GetCustomAttributes(typeof<AssemblyFileVersionAttribute>) |> Seq.tryHead
                match attrOpt with
                | Some attr ->
                    let fv = (downcast attr : AssemblyFileVersionAttribute).Version.Split([|'.'|]) |> Array.map(fun e ->  Int32.Parse(e))
                    fv.[0], fv.[1], fv.[2], fv.[3]
                | _ -> defaultMscorlibVersion
            with _ -> defaultMscorlibVersion

        // Get the ProductVersion of this framework compare with table yield compatible monikers
        match desktopProductVersionMonikers
              |> Array.tryFind (fun (major, minor, build, revision, _) ->
                    (majorPart >= major) &&
                    (minorPart >= minor) &&
                    (buildPart >= build) &&
                    (privatePart >= revision)) with
        | Some (_,_,_,_,moniker) ->
            moniker
        | None ->
            // no TFM could be found, assume latest stable?
            "net48"

    let tryGetSdkRefsPackDirectory() =
        let tfmPrefix = "netcoreapp"
        let tfmCompare c1 c2 =
            let deconstructTfmApp (netcoreApp: DirectoryInfo) =
                let name = netcoreApp.Name
                try
                    if name.StartsWith(tfmPrefix, StringComparison.InvariantCultureIgnoreCase) then
                        Some (Double.Parse(name.Substring(tfmPrefix.Length), NumberStyles.AllowDecimalPoint,  CultureInfo.InvariantCulture))
                    else
                        None
                with _ -> None

            if c1 = c2 then 0
            else
                match (deconstructTfmApp c1), (deconstructTfmApp c2) with
                | Some c1, Some c2 -> int(c1 - c2)
                | None, Some _ -> -1
                | Some _, None -> 1
                | _ -> 0

        match tryGetNetCoreRefsPackDirectoryRoot() with
        | Some version, Some root ->
            try
                let ref = Path.Combine(root, version, "ref")
                let highestTfm =
                    DirectoryInfo(ref).GetDirectories()
                    |> Array.sortWith tfmCompare
                    |> Array.tryLast

                match highestTfm with
                | Some tfm -> Some (Path.Combine(ref, tfm.Name))
                | None -> None
            with _ -> None
        | _ -> None

    let getDependenciesOf assemblyReferences =
        let assemblies = new Dictionary<string, string>()

        // Identify path to a dll in the framework directory from a simple name
        let frameworkPathFromSimpleName simpleName =
            let root = Path.Combine(getImplementationAssemblyDir(), simpleName)
            let pathOpt =
                [| ""; ".dll"; ".exe" |]
                |> Seq.tryPick(fun ext ->
                    let path = root + ext
                    if File.Exists(path) then Some path
                    else None)
            match pathOpt with
            | Some path -> path
            | None -> root

        // Collect all assembly dependencies into assemblies dictionary
        let rec traverseDependencies reference =
            // Reference can be either path to a file on disk or a Assembly Simple Name
            let referenceName, path =
                try
                    if File.Exists(reference) then
                        // Reference is a path to a file on disk
                        Path.GetFileNameWithoutExtension(reference), reference
                    else
                        // Reference is a SimpleAssembly name
                        reference, frameworkPathFromSimpleName reference

                with _ -> reference, frameworkPathFromSimpleName reference

            if not (assemblies.ContainsKey(referenceName)) then
                try
                    if File.Exists(path) then
                        match referenceName with
                        | "System.Runtime.WindowsRuntime"
                        | "System.Runtime.WindowsRuntime.UI.Xaml" ->
                            // The Windows compatibility pack included in the runtime contains a reference to
                            // System.Runtime.WindowsRuntime, but to properly use that type the runtime also needs a
                            // reference to the Windows.md meta-package, which isn't referenced by default.  To avoid
                            // a bug where types from `Windows, Version=255.255.255.255` can't be found we're going to
                            // not default include this assembly.  It can still be manually referenced if it's needed
                            // via the System.Runtime.WindowsRuntime NuGet package.
                            //
                            // In the future this branch can be removed because WinRT support is being removed from the
                            // .NET 5 SDK (https://github.com/dotnet/runtime/pull/36715)
                            ()
                        | "System.Private.CoreLib" ->
                            // System.Private.CoreLib doesn't load with reflection
                            assemblies.Add(referenceName, path)
                        | _ ->
                            try
                                let opts = 
                                    { metadataOnly = MetadataOnlyFlag.Yes // turn this off here as we need the actual IL code
                                      reduceMemoryUsage = ReduceMemoryFlag.Yes
                                      pdbDirPath = None
                                      tryGetMetadataSnapshot = (fun _ -> None) (* tryGetMetadataSnapshot *) } 

                                let reader = OpenILModuleReader path opts
                                assemblies.Add(referenceName, path)
                                for reference in reader.ILAssemblyRefs do
                                    traverseDependencies reference.Name

                            // There are many native assemblies which can't be cracked, raising exceptions
                            with _ -> ()
                with _ -> ()

        assemblyReferences |> List.iter traverseDependencies
        assemblies

    // This list is the default set of references for "non-project" files. 
    //
    // These DLLs are
    //    (a) included in the environment used for all .fsx files (see service.fs)
    //    (b) included in environment for files 'orphaned' from a project context
    //            -- for orphaned files (files in VS without a project context)
    let getDotNetFrameworkDefaultReferences useFsiAuxLib = [
        yield "mscorlib"
        yield "System"
        yield "System.Xml"
        yield "System.Runtime.Remoting"
        yield "System.Runtime.Serialization.Formatters.Soap"
        yield "System.Data"
        yield "System.Drawing"
        yield "System.Core"
        yield "System.Configuration"

        yield getFSharpCoreLibraryName
        if useFsiAuxLib then yield fsiLibraryName

        // always include a default reference to System.ValueTuple.dll in scripts and out-of-project sources 
        match getSystemValueTupleImplementationReference () with
        | None -> ()
        | Some v -> yield v

        // These are the Portable-profile and .NET Standard 1.6 dependencies of FSharp.Core.dll.  These are needed
        // when an F# script references an F# profile 7, 78, 259 or .NET Standard 1.6 component which in turn refers 
        // to FSharp.Core for profile 7, 78, 259 or .NET Standard.
        yield "netstandard"
        yield "System.Runtime"          // lots of types
        yield "System.Linq"             // System.Linq.Expressions.Expression<T> 
        yield "System.Reflection"       // System.Reflection.ParameterInfo
        yield "System.Linq.Expressions" // System.Linq.IQueryable<T>
        yield "System.Threading.Tasks"  // valuetype [System.Threading.Tasks]System.Threading.CancellationToken
        yield "System.IO"               //  System.IO.TextWriter
        yield "System.Net.Requests"     //  System.Net.WebResponse etc.
        yield "System.Collections"      // System.Collections.Generic.List<T>
        yield "System.Runtime.Numerics" // BigInteger
        yield "System.Threading"        // OperationCanceledException
        yield "System.Web"
        yield "System.Web.Services"
        yield "System.Windows.Forms"
        yield "System.Numerics"
    ]

    let getDotNetCoreImplementationReferences useFsiAuxLib =
        let implDir = getImplementationAssemblyDir()
        let roots = 
            [ yield! Directory.GetFiles(implDir, "*.dll")
              yield getFSharpCoreImplementationReference()
              if useFsiAuxLib then yield getFsiLibraryImplementationReference() ]
        (getDependenciesOf roots).Values |> Seq.toList

    // A set of assemblies to always consider to be system assemblies.  A common set of these can be used a shared 
    // resources between projects in the compiler services.  Also all assemblies where well-known system types exist
    // referenced from TcGlobals must be listed here.
    let systemAssemblies =
        HashSet [
            // NOTE: duplicates are ok in this list

            // .NET Framework list
            yield "mscorlib"
            yield "netstandard"
            yield "System"
            yield getFSharpCoreLibraryName
            yield "FSharp.Compiler.Interactive.Settings"
            yield "Microsoft.CSharp"
            yield "Microsoft.VisualBasic"
            yield "Microsoft.VisualBasic.Core"
            yield "Microsoft.Win32.Primitives"
            yield "Microsoft.Win32.Registry"
            yield "System.AppContext"
            yield "System.Buffers"
            yield "System.Collections"
            yield "System.Collections.Concurrent"
            yield "System.Collections.Immutable"
            yield "System.Collections.NonGeneric"
            yield "System.Collections.Specialized"
            yield "System.ComponentModel"
            yield "System.ComponentModel.Annotations"
            yield "System.ComponentModel.DataAnnotations"
            yield "System.ComponentModel.EventBasedAsync"
            yield "System.ComponentModel.Primitives"
            yield "System.ComponentModel.TypeConverter"
            yield "System.Configuration"
            yield "System.Console"
            yield "System.Core"
            yield "System.Data"
            yield "System.Data.Common"
            yield "System.Data.DataSetExtensions"
            yield "System.Deployment"
            yield "System.Design"
            yield "System.Diagnostics.Contracts"
            yield "System.Diagnostics.Debug"
            yield "System.Diagnostics.DiagnosticSource"
            yield "System.Diagnostics.FileVersionInfo"
            yield "System.Diagnostics.Process"
            yield "System.Diagnostics.StackTrace"
            yield "System.Diagnostics.TextWriterTraceListener"
            yield "System.Diagnostics.Tools"
            yield "System.Diagnostics.TraceSource"
            yield "System.Diagnostics.Tracing"
            yield "System.Drawing"
            yield "System.Drawing.Primitives"
            yield "System.Dynamic.Runtime"
            yield "System.Formats.Asn1"
            yield "System.Globalization"
            yield "System.Globalization.Calendars"
            yield "System.Globalization.Extensions"
            yield "System.IO"
            yield "System.IO.Compression"
            yield "System.IO.Compression.Brotli"
            yield "System.IO.Compression.FileSystem"
            yield "System.IO.Compression.ZipFile"
            yield "System.IO.FileSystem"
            yield "System.IO.FileSystem.DriveInfo"
            yield "System.IO.FileSystem.Primitives"
            yield "System.IO.FileSystem.Watcher"
            yield "System.IO.IsolatedStorage"
            yield "System.IO.MemoryMappedFiles"
            yield "System.IO.Pipes"
            yield "System.IO.UnmanagedMemoryStream"
            yield "System.Linq"
            yield "System.Linq.Expressions"
            yield "System.Linq.Expressions"
            yield "System.Linq.Parallel"
            yield "System.Linq.Queryable"
            yield "System.Memory"
            yield "System.Messaging"
            yield "System.Net"
            yield "System.Net.Http"
            yield "System.Net.Http.Json"
            yield "System.Net.HttpListener"
            yield "System.Net.Mail"
            yield "System.Net.NameResolution"
            yield "System.Net.NetworkInformation"
            yield "System.Net.Ping"
            yield "System.Net.Primitives"
            yield "System.Net.Requests"
            yield "System.Net.Security"
            yield "System.Net.ServicePoint"
            yield "System.Net.Sockets"
            yield "System.Net.WebClient"
            yield "System.Net.WebHeaderCollection"
            yield "System.Net.WebProxy"
            yield "System.Net.WebSockets"
            yield "System.Net.WebSockets.Client"
            yield "System.Numerics"
            yield "System.Numerics.Vectors"
            yield "System.ObjectModel"
            yield "System.Observable"
            yield "System.Private.Uri"
            yield "System.Reflection"
            yield "System.Reflection.DispatchProxy"
            yield "System.Reflection.Emit"
            yield "System.Reflection.Emit.ILGeneration"
            yield "System.Reflection.Emit.Lightweight"
            yield "System.Reflection.Extensions"
            yield "System.Reflection.Metadata"
            yield "System.Reflection.Primitives"
            yield "System.Reflection.TypeExtensions"
            yield "System.Resources.Reader"
            yield "System.Resources.ResourceManager"
            yield "System.Resources.Writer"
            yield "System.Runtime"
            yield "System.Runtime.CompilerServices.Unsafe"
            yield "System.Runtime.CompilerServices.VisualC"
            yield "System.Runtime.Extensions"
            yield "System.Runtime.Handles"
            yield "System.Runtime.InteropServices"
            yield "System.Runtime.InteropServices.PInvoke"
            yield "System.Runtime.InteropServices.RuntimeInformation"
            yield "System.Runtime.InteropServices.WindowsRuntime"
            yield "System.Runtime.Intrinsics"
            yield "System.Runtime.Loader"
            yield "System.Runtime.Numerics"
            yield "System.Runtime.Remoting"
            yield "System.Runtime.Serialization"
            yield "System.Runtime.Serialization.Formatters"
            yield "System.Runtime.Serialization.Formatters.Soap"
            yield "System.Runtime.Serialization.Json"
            yield "System.Runtime.Serialization.Primitives"
            yield "System.Runtime.Serialization.Xml"
            yield "System.Security"
            yield "System.Security.Claims"
            yield "System.Security.Cryptography.Algorithms"
            yield "System.Security.Cryptography.Cng"
            yield "System.Security.Cryptography.Csp"
            yield "System.Security.Cryptography.Encoding"
            yield "System.Security.Cryptography.OpenSsl"
            yield "System.Security.Cryptography.Primitives"
            yield "System.Security.Cryptography.X509Certificates"
            yield "System.Security.Principal"
            yield "System.Security.Principal.Windows"
            yield "System.Security.SecureString"
            yield "System.ServiceModel.Web"
            yield "System.ServiceProcess"
            yield "System.Text.Encoding"
            yield "System.Text.Encoding.CodePages"
            yield "System.Text.Encoding.Extensions"
            yield "System.Text.Encodings.Web"
            yield "System.Text.Json"
            yield "System.Text.RegularExpressions"
            yield "System.Threading"
            yield "System.Threading.Channels"
            yield "System.Threading.Overlapped"
            yield "System.Threading.Tasks"
            yield "System.Threading.Tasks.Dataflow"
            yield "System.Threading.Tasks.Extensions"
            yield "System.Threading.Tasks.Parallel"
            yield "System.Threading.Thread"
            yield "System.Threading.ThreadPool"
            yield "System.Threading.Timer"
            yield "System.Transactions"
            yield "System.Transactions.Local"
            yield "System.ValueTuple"
            yield "System.Web"
            yield "System.Web.HttpUtility"
            yield "System.Web.Services"
            yield "System.Windows"
            yield "System.Windows.Forms"
            yield "System.Xml"
            yield "System.Xml.Linq"
            yield "System.Xml.ReaderWriter"
            yield "System.Xml.Serialization"
            yield "System.Xml.XDocument"
            yield "System.Xml.XmlDocument"
            yield "System.Xml.XmlSerializer"
            yield "System.Xml.XPath"
            yield "System.Xml.XPath.XDocument"
            yield "WindowsBase"
        ]

    member _.GetSystemAssemblies() = systemAssemblies

    member _.IsInReferenceAssemblyPackDirectory filename =
        match tryGetNetCoreRefsPackDirectoryRoot() with
        | _, Some root ->
            let path = Path.GetDirectoryName(filename)
            path.StartsWith(root, StringComparison.OrdinalIgnoreCase)
        | _ -> false

    member _.TryGetSdkDir() = tryGetSdkDir()

    /// Gets the selected target framework moniker, e.g netcore3.0, net472, and the running rid of the current machine
    member _.GetTfmAndRid() = 
        // Interactive processes read their own configuration to find the running tfm

        let tfm =
            if isInteractive then
                match tryGetRunningDotNetCoreTfm() with
                | Some tfm -> tfm
                | _ -> getRunningDotNetFrameworkTfm ()
            else
                let sdkDir = tryGetSdkDir()
                match sdkDir with 
                | Some dir -> 
                    let dotnetConfigFile = Path.Combine(dir, "dotnet.runtimeconfig.json")
                    let dotnetConfig = File.ReadAllText(dotnetConfigFile)
                    let pattern = "\"tfm\": \""
                    let startPos = dotnetConfig.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) + pattern.Length
                    let endPos = dotnetConfig.IndexOf("\"", startPos)
                    let tfm = dotnetConfig.[startPos..endPos-1]
                    //printfn "GetTfmAndRid, tfm = '%s'" tfm
                    tfm
                | None ->
                match tryGetRunningDotNetCoreTfm() with
                | Some tfm -> tfm
                | _ -> getRunningDotNetFrameworkTfm ()

        // Computer valid dotnet-rids for this environment:
        //      https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
        //
        // Where rid is: win, win-x64, win-x86, osx-x64, linux-x64 etc ...
        let runningRid =
            let processArchitecture = RuntimeInformation.ProcessArchitecture
            let baseRid =
                if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then "win"
                elif RuntimeInformation.IsOSPlatform(OSPlatform.OSX) then "osx"
                else "linux"
            match processArchitecture with
            | Architecture.X64 ->  baseRid + "-x64"
            | Architecture.X86 -> baseRid + "-x86"
            | Architecture.Arm64 -> baseRid + "-arm64"
            | _ -> baseRid + "-arm"

        tfm, runningRid

    static member ClearStaticCaches() = 
        desiredDotNetSdkVersionForDirectoryCache.Clear()

    member _.GetFrameworkRefsPackDirectory() = tryGetSdkRefsPackDirectory()

    member _.TryGetDesiredDotNetSdkVersionForDirectory() = tryGetDesiredDotNetSdkVersionForDirectoryInfo()

    // The set of references entered into the TcConfigBuilder for scripts prior to computing the load closure. 
    member _.GetDefaultReferences (useFsiAuxLib, assumeDotNetFramework) =
        let defaultReferences =
            if assumeDotNetFramework then
                getDotNetFrameworkDefaultReferences useFsiAuxLib, assumeDotNetFramework
            else
                if useSdkRefs then
                    // Go fetch references
                    match tryGetSdkRefsPackDirectory() with
                    | Some path ->
                        try 
                            let sdkReferences = 
                                [ yield! Directory.GetFiles(path, "*.dll")
                                  yield getFSharpCoreImplementationReference()
                                  if useFsiAuxLib then yield getFsiLibraryImplementationReference()
                                ]
                            sdkReferences, false
                        with _ -> 
                            if isRunningOnCoreClr then
                                // If running on .NET Core and something goes wrong with getting the
                                // .NET Core references then use .NET Core implementation assemblies for running process
                                getDotNetCoreImplementationReferences useFsiAuxLib, false
                            else
                                // If running on .NET Framework and something goes wrong with getting the
                                // .NET Core references then default back to .NET Framework and return a flag indicating this has been done
                                getDotNetFrameworkDefaultReferences useFsiAuxLib, true
                    | None ->
                        if isRunningOnCoreClr then
                            // If running on .NET Core and there is no Sdk refs pack directory
                            // then use .NET Core implementation assemblies for running process
                            getDotNetCoreImplementationReferences useFsiAuxLib, false
                        else
                            // If running on .NET Framework and there is no Sdk refs pack directory
                            // then default back to .NET Framework and return a flag indicating this has been done
                            getDotNetFrameworkDefaultReferences useFsiAuxLib, true
                else
                    getDotNetCoreImplementationReferences useFsiAuxLib, assumeDotNetFramework
        defaultReferences

