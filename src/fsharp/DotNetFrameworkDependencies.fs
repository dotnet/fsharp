// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Functions to retrieve framework dependencies
module internal FSharp.Compiler.DotNetFrameworkDependencies

    open System
    open System.Collections.Generic
    open System.Diagnostics
    open System.Globalization
    open System.IO
    open System.Reflection
    open System.Runtime.InteropServices
    open Internal.Utilities
    open Internal.Utilities.FSharpEnvironment

    type private TypeInThisAssembly = class end

    let fSharpCompilerLocation =
        let location = Path.GetDirectoryName(typeof<TypeInThisAssembly>.Assembly.Location)
        match FSharpEnvironment.BinFolderOfDefaultFSharpCompiler (Some location) with
        | Some path -> path
        | None ->
#if DEBUG
            Debug.Print(sprintf """FSharpEnvironment.BinFolderOfDefaultFSharpCompiler (Some '%s') returned None Location
                customized incorrectly: algorithm here: https://github.com/dotnet/fsharp/blob/03f3f1c35f82af26593d025dabca57a6ef3ea9a1/src/utils/CompilerLocationUtils.fs#L171"""
                location)
#endif
            // Use the location of this dll
            location

    let inline ifEmptyUse alternative filename = if String.IsNullOrWhiteSpace filename then alternative else filename
    
    let getFSharpCoreLibraryName = "FSharp.Core"
    let getFsiLibraryName = "FSharp.Compiler.Interactive.Settings"
    let getDefaultFSharpCoreLocation = Path.Combine(fSharpCompilerLocation, getFSharpCoreLibraryName + ".dll")
    let getDefaultFsiLibraryLocation = Path.Combine(fSharpCompilerLocation, getFsiLibraryName + ".dll")
    let implementationAssemblyDir = Path.GetDirectoryName(typeof<obj>.Assembly.Location) |> ifEmptyUse fSharpCompilerLocation

    // Use the ValueTuple that is executing with the compiler if it is from System.ValueTuple
    // or the System.ValueTuple.dll that sits alongside the compiler.  (Note we always ship one with the compiler)
    let getDefaultSystemValueTupleReference () =
        try
            let asm = typeof<System.ValueTuple<int, int>>.Assembly
            if asm.FullName.StartsWith("System.ValueTuple", StringComparison.OrdinalIgnoreCase) then
                Some asm.Location
            else
                let valueTuplePath = Path.Combine(fSharpCompilerLocation, "System.ValueTuple.dll")
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
    let version, frameworkRefsPackDirectoryRoot =
        try
            let version = DirectoryInfo(implementationAssemblyDir).Name
            let microsoftNETCoreAppRef = Path.Combine(implementationAssemblyDir, "../../../packs/Microsoft.NETCore.App.Ref")
            if Directory.Exists(microsoftNETCoreAppRef) then
                Some version, Some microsoftNETCoreAppRef
            else
               Some version,  None
        with | _ -> None, None

    // Tries to figure out the tfm for the compiler instance.
    // On coreclr it uses the deps.json file
    let netcoreTfm =
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
                // Running on coreclr but no deps.json was deployed with the host so default to 3.0
                Some "netcoreapp3.1"
            else
                // Running on desktop
                None
        | pos, length ->
            // use value from the deps.json file
            Some ("netcoreapp" + file.Substring(pos, length))

    // Tries to figure out the tfm for the compiler instance on the Windows desktop.
    // On full clr it uses the mscorlib version number
    let getWindowsDesktopTfm () =
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

    /// Gets the tfm E.g netcore3.0, net472
    let executionTfm =
        match netcoreTfm with
        | Some tfm -> tfm
        | _ -> getWindowsDesktopTfm ()

    // Computer valid dotnet-rids for this environment:
    //      https://docs.microsoft.com/en-us/dotnet/core/rid-catalog
    //
    // Where rid is: win, win-x64, win-x86, osx-x64, linux-x64 etc ...
    let executionRid =
        let processArchitecture = RuntimeInformation.ProcessArchitecture
        let baseRid =
            if RuntimeInformation.IsOSPlatform(OSPlatform.Windows) then "win"
            elif RuntimeInformation.IsOSPlatform(OSPlatform.OSX) then "osx"
            else "linux"
        let platformRid =
            match processArchitecture with
            | Architecture.X64 ->  baseRid + "-x64"
            | Architecture.X86 -> baseRid + "-x86"
            | Architecture.Arm64 -> baseRid + "-arm64"
            | _ -> baseRid + "-arm"
        platformRid

    let isInReferenceAssemblyPackDirectory filename =
        match frameworkRefsPackDirectoryRoot with
        | Some root ->
            let path = Path.GetDirectoryName(filename)
            path.StartsWith(root, StringComparison.OrdinalIgnoreCase)
        | _ -> false

    let frameworkRefsPackDirectory =
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

        match version, frameworkRefsPackDirectoryRoot with
        | Some version, Some root ->
            try
                let ref = Path.Combine(root, version, "ref")
                let highestTfm = DirectoryInfo(ref).GetDirectories()
                                 |> Array.sortWith tfmCompare
                                 |> Array.tryLast

                match highestTfm with
                | Some tfm -> Some (Path.Combine(ref, tfm.Name))
                | None -> None
            with | _ -> None
        | _ -> None

    let getDependenciesOf assemblyReferences =
        let assemblies = new Dictionary<string, string>()

        // Identify path to a dll in the framework directory from a simple name
        let frameworkPathFromSimpleName simpleName =
            let root = Path.Combine(implementationAssemblyDir, simpleName)
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
                                let asm = System.Reflection.Assembly.LoadFrom(path)
                                assemblies.Add(referenceName, path)
                                for reference in asm.GetReferencedAssemblies() do
                                    traverseDependencies reference.Name
                            with e -> ()
                with e -> ()

        assemblyReferences |> List.iter(traverseDependencies)
        assemblies

    // This list is the default set of references for "non-project" files. 
    //
    // These DLLs are
    //    (a) included in the environment used for all .fsx files (see service.fs)
    //    (b) included in environment for files 'orphaned' from a project context
    //            -- for orphaned files (files in VS without a project context)
    let getDesktopDefaultReferences useFsiAuxLib = [
        yield "mscorlib"
        yield "System"
        yield "System.Xml"
        yield "System.Runtime.Remoting"
        yield "System.Runtime.Serialization.Formatters.Soap"
        yield "System.Data"
        yield "System.Drawing"
        yield "System.Core"

        yield getFSharpCoreLibraryName
        if useFsiAuxLib then yield getFsiLibraryName

        // always include a default reference to System.ValueTuple.dll in scripts and out-of-project sources 
        match getDefaultSystemValueTupleReference () with
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

    let fetchPathsForDefaultReferencesForScriptsAndOutOfProjectSources useFsiAuxLib useSdkRefs assumeDotNetFramework =
        let results =
            if assumeDotNetFramework then
                getDesktopDefaultReferences useFsiAuxLib
            else
                let dependencies =
                    let getImplementationReferences () =
                        // Coreclr supports netstandard assemblies only for now
                        (getDependenciesOf [
                            yield! Directory.GetFiles(implementationAssemblyDir, "*.dll")
                            yield getDefaultFSharpCoreLocation
                            if useFsiAuxLib then yield getDefaultFsiLibraryLocation
                        ]).Values |> Seq.toList

                    if useSdkRefs then
                        // Go fetch references
                        match frameworkRefsPackDirectory with
                        | Some path ->
                            try [ yield! Directory.GetFiles(path, "*.dll")
                                  yield getDefaultFSharpCoreLocation
                                  if useFsiAuxLib then yield getDefaultFsiLibraryLocation
                                ]
                            with | _ -> List.empty<string>
                        | None ->
                            getImplementationReferences ()
                    else
                        getImplementationReferences ()
                dependencies
        results

    let defaultReferencesForScriptsAndOutOfProjectSources useFsiAuxLib assumeDotNetFramework useSdkRefs =
        fetchPathsForDefaultReferencesForScriptsAndOutOfProjectSources useFsiAuxLib useSdkRefs assumeDotNetFramework

    // A set of assemblies to always consider to be system assemblies.  A common set of these can be used a shared 
    // resources between projects in the compiler services.  Also all assemblies where well-known system types exist
    // referenced from TcGlobals must be listed here.
    let systemAssemblies =
        HashSet [
            yield "mscorlib"
            yield "netstandard"
            yield "System.Runtime"
            yield getFSharpCoreLibraryName

            yield "System"
            yield "System.Xml" 
            yield "System.Runtime.Remoting"
            yield "System.Runtime.Serialization.Formatters.Soap"
            yield "System.Data"
            yield "System.Deployment"
            yield "System.Design"
            yield "System.Messaging"
            yield "System.Drawing"
            yield "System.Net"
            yield "System.Web"
            yield "System.Web.Services"
            yield "System.Windows.Forms"
            yield "System.Core"
            yield "System.Runtime"
            yield "System.Observable"
            yield "System.Numerics"
            yield "System.ValueTuple"

            // Additions for coreclr and portable profiles
            yield "System.Collections"
            yield "System.Collections.Concurrent"
            yield "System.Console"
            yield "System.Diagnostics.Debug"
            yield "System.Diagnostics.Tools"
            yield "System.Globalization"
            yield "System.IO"
            yield "System.Linq"
            yield "System.Linq.Expressions"
            yield "System.Linq.Queryable"
            yield "System.Net.Requests"
            yield "System.Reflection"
            yield "System.Reflection.Emit"
            yield "System.Reflection.Emit.ILGeneration"
            yield "System.Reflection.Extensions"
            yield "System.Resources.ResourceManager"
            yield "System.Runtime.Extensions"
            yield "System.Runtime.InteropServices"
            yield "System.Runtime.InteropServices.PInvoke"
            yield "System.Runtime.Numerics"
            yield "System.Text.Encoding"
            yield "System.Text.Encoding.Extensions"
            yield "System.Text.RegularExpressions"
            yield "System.Threading"
            yield "System.Threading.Tasks"
            yield "System.Threading.Tasks.Parallel"
            yield "System.Threading.Thread"
            yield "System.Threading.ThreadPool"
            yield "System.Threading.Timer"

            yield "FSharp.Compiler.Interactive.Settings"
            yield "Microsoft.Win32.Registry"
            yield "System.Diagnostics.Tracing"
            yield "System.Globalization.Calendars"
            yield "System.Reflection.Primitives"
            yield "System.Runtime.Handles"
            yield "Microsoft.Win32.Primitives"
            yield "System.IO.FileSystem"
            yield "System.Net.Primitives"
            yield "System.Net.Sockets"
            yield "System.Private.Uri"
            yield "System.AppContext"
            yield "System.Buffers"
            yield "System.Collections.Immutable"
            yield "System.Diagnostics.DiagnosticSource"
            yield "System.Diagnostics.Process"
            yield "System.Diagnostics.TraceSource"
            yield "System.Globalization.Extensions"
            yield "System.IO.Compression"
            yield "System.IO.Compression.ZipFile"
            yield "System.IO.FileSystem.Primitives"
            yield "System.Net.Http"
            yield "System.Net.NameResolution"
            yield "System.Net.WebHeaderCollection"
            yield "System.ObjectModel"
            yield "System.Reflection.Emit.Lightweight"
            yield "System.Reflection.Metadata"
            yield "System.Reflection.TypeExtensions"
            yield "System.Runtime.InteropServices.RuntimeInformation"
            yield "System.Runtime.Loader"
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
            yield "System.Threading.Overlapped"
            yield "System.Threading.Tasks.Extensions"
            yield "System.Xml.ReaderWriter"
            yield "System.Xml.XDocument"
        ]

    // The set of references entered into the TcConfigBuilder for scripts prior to computing the load closure. 
    let basicReferencesForScriptLoadClosure useFsiAuxLib useSdkRefs assumeDotNetFramework =
        fetchPathsForDefaultReferencesForScriptsAndOutOfProjectSources useFsiAuxLib useSdkRefs assumeDotNetFramework
