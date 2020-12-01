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
    //     Use the reference assemblies for the highest netcoreapp tfm that we find in that location that is 
    //     lower than or equal to the implementation version.
    let zeroVersion = Version("0.0.0.0")
    let version, frameworkRefsPackDirectoryRoot =
        try
            let computeVersion version =
                match Version.TryParse(version) with
                | true, v -> v
                | false, _ -> zeroVersion

            let version = computeVersion (DirectoryInfo(implementationAssemblyDir).Name)
            let microsoftNETCoreAppRef = Path.Combine(implementationAssemblyDir, "../../../packs/Microsoft.NETCore.App.Ref")
            if Directory.Exists(microsoftNETCoreAppRef) then
                let directory = DirectoryInfo(microsoftNETCoreAppRef).GetDirectories()
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
        yield "System.Configuration"

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
            // NOTE: duplicates are ok in this list

            // .NET Framework list
            "mscorlib"
            "netstandard"
            "System.Runtime"
            getFSharpCoreLibraryName

            "System"
            "System.Xml" 
            "System.Runtime.Remoting"
            "System.Runtime.Serialization.Formatters.Soap"
            "System.Configuration"
            "System.Data"
            "System.Deployment"
            "System.Design"
            "System.Messaging"
            "System.Drawing"
            "System.Net"
            "System.Web"
            "System.Web.Services"
            "System.Windows.Forms"
            "System.Core"
            "System.Runtime"
            "System.Observable"
            "System.Numerics"
            "System.ValueTuple"

            // Additions for coreclr and portable profiles
            "System.Collections"
            "System.Collections.Concurrent"
            "System.Console"
            "System.Diagnostics.Debug"
            "System.Diagnostics.Tools"
            "System.Globalization"
            "System.IO"
            "System.Linq"
            "System.Linq.Expressions"
            "System.Linq.Queryable"
            "System.Net.Requests"
            "System.Reflection"
            "System.Reflection.Emit"
            "System.Reflection.Emit.ILGeneration"
            "System.Reflection.Extensions"
            "System.Resources.ResourceManager"
            "System.Runtime.Extensions"
            "System.Runtime.InteropServices"
            "System.Runtime.InteropServices.PInvoke"
            "System.Runtime.Numerics"
            "System.Text.Encoding"
            "System.Text.Encoding.Extensions"
            "System.Text.RegularExpressions"
            "System.Threading"
            "System.Threading.Tasks"
            "System.Threading.Tasks.Parallel"
            "System.Threading.Thread"
            "System.Threading.ThreadPool"
            "System.Threading.Timer"

            "FSharp.Compiler.Interactive.Settings"
            "Microsoft.Win32.Registry"
            "System.Diagnostics.Tracing"
            "System.Globalization.Calendars"
            "System.Reflection.Primitives"
            "System.Runtime.Handles"
            "Microsoft.Win32.Primitives"
            "System.IO.FileSystem"
            "System.Net.Primitives"
            "System.Net.Sockets"
            "System.Private.Uri"
            "System.AppContext"
            "System.Buffers"
            "System.Collections.Immutable"
            "System.Diagnostics.DiagnosticSource"
            "System.Diagnostics.Process"
            "System.Diagnostics.TraceSource"
            "System.Globalization.Extensions"
            "System.IO.Compression"
            "System.IO.Compression.ZipFile"
            "System.IO.FileSystem.Primitives"
            "System.Net.Http"
            "System.Net.NameResolution"
            "System.Net.WebHeaderCollection"
            "System.ObjectModel"
            "System.Reflection.Emit.Lightweight"
            "System.Reflection.Metadata"
            "System.Reflection.TypeExtensions"
            "System.Runtime.InteropServices.RuntimeInformation"
            "System.Runtime.Loader"
            "System.Security.Claims"
            "System.Security.Cryptography.Algorithms"
            "System.Security.Cryptography.Cng"
            "System.Security.Cryptography.Csp"
            "System.Security.Cryptography.Encoding"
            "System.Security.Cryptography.OpenSsl"
            "System.Security.Cryptography.Primitives"
            "System.Security.Cryptography.X509Certificates"
            "System.Security.Principal"
            "System.Security.Principal.Windows"
            "System.Threading.Overlapped"
            "System.Threading.Tasks.Extensions"
            "System.Xml.ReaderWriter"
            "System.Xml.XDocument"

            // .NET Core App 3.1 list
            "Microsoft.CSharp"
            "Microsoft.VisualBasic.Core"
            "Microsoft.VisualBasic"
            "Microsoft.Win32.Primitives"
            "mscorlib"
            "netstandard"
            "System.AppContext"
            "System.Buffers"
            "System.Collections.Concurrent"
            "System.Collections"
            "System.Collections.Immutable"
            "System.Collections.NonGeneric"
            "System.Collections.Specialized"
            "System.ComponentModel.Annotations"
            "System.ComponentModel.DataAnnotations"
            "System.ComponentModel"
            "System.ComponentModel.EventBasedAsync"
            "System.ComponentModel.Primitives"
            "System.ComponentModel.TypeConverter"
            "System.Configuration"
            "System.Console"
            "System.Core"
            "System.Data.Common"
            "System.Data.DataSetExtensions"
            "System.Data"
            "System.Diagnostics.Contracts"
            "System.Diagnostics.Debug"
            "System.Diagnostics.DiagnosticSource"
            "System.Diagnostics.FileVersionInfo"
            "System.Diagnostics.Process"
            "System.Diagnostics.StackTrace"
            "System.Diagnostics.TextWriterTraceListener"
            "System.Diagnostics.Tools"
            "System.Diagnostics.TraceSource"
            "System.Diagnostics.Tracing"
            "System"
            "System.Drawing"
            "System.Drawing.Primitives"
            "System.Dynamic.Runtime"
            "System.Globalization.Calendars"
            "System.Globalization"
            "System.Globalization.Extensions"
            "System.IO.Compression.Brotli"
            "System.IO.Compression"
            "System.IO.Compression.FileSystem"
            "System.IO.Compression.ZipFile"
            "System.IO"
            "System.IO.FileSystem"
            "System.IO.FileSystem.DriveInfo"
            "System.IO.FileSystem.Primitives"
            "System.IO.FileSystem.Watcher"
            "System.IO.IsolatedStorage"
            "System.IO.MemoryMappedFiles"
            "System.IO.Pipes"
            "System.IO.UnmanagedMemoryStream"
            "System.Linq"
            "System.Linq.Expressions"
            "System.Linq.Parallel"
            "System.Linq.Queryable"
            "System.Memory"
            "System.Net"
            "System.Net.Http"
            "System.Net.HttpListener"
            "System.Net.Mail"
            "System.Net.NameResolution"
            "System.Net.NetworkInformation"
            "System.Net.Ping"
            "System.Net.Primitives"
            "System.Net.Requests"
            "System.Net.Security"
            "System.Net.ServicePoint"
            "System.Net.Sockets"
            "System.Net.WebClient"
            "System.Net.WebHeaderCollection"
            "System.Net.WebProxy"
            "System.Net.WebSockets.Client"
            "System.Net.WebSockets"
            "System.Numerics"
            "System.Numerics.Vectors"
            "System.ObjectModel"
            "System.Reflection.DispatchProxy"
            "System.Reflection"
            "System.Reflection.Emit"
            "System.Reflection.Emit.ILGeneration"
            "System.Reflection.Emit.Lightweight"
            "System.Reflection.Extensions"
            "System.Reflection.Metadata"
            "System.Reflection.Primitives"
            "System.Reflection.TypeExtensions"
            "System.Resources.Reader"
            "System.Resources.ResourceManager"
            "System.Resources.Writer"
            "System.Runtime.CompilerServices.Unsafe"
            "System.Runtime.CompilerServices.VisualC"
            "System.Runtime"
            "System.Runtime.Extensions"
            "System.Runtime.Handles"
            "System.Runtime.InteropServices"
            "System.Runtime.InteropServices.RuntimeInformation"
            "System.Runtime.InteropServices.WindowsRuntime"
            "System.Runtime.Intrinsics"
            "System.Runtime.Loader"
            "System.Runtime.Numerics"
            "System.Runtime.Serialization"
            "System.Runtime.Serialization.Formatters"
            "System.Runtime.Serialization.Json"
            "System.Runtime.Serialization.Primitives"
            "System.Runtime.Serialization.Xml"
            "System.Security.Claims"
            "System.Security.Cryptography.Algorithms"
            "System.Security.Cryptography.Csp"
            "System.Security.Cryptography.Encoding"
            "System.Security.Cryptography.Primitives"
            "System.Security.Cryptography.X509Certificates"
            "System.Security"
            "System.Security.Principal"
            "System.Security.SecureString"
            "System.ServiceModel.Web"
            "System.ServiceProcess"
            "System.Text.Encoding.CodePages"
            "System.Text.Encoding"
            "System.Text.Encoding.Extensions"
            "System.Text.Encodings.Web"
            "System.Text.Json"
            "System.Text.RegularExpressions"
            "System.Threading.Channels"
            "System.Threading"
            "System.Threading.Overlapped"
            "System.Threading.Tasks.Dataflow"
            "System.Threading.Tasks"
            "System.Threading.Tasks.Extensions"
            "System.Threading.Tasks.Parallel"
            "System.Threading.Thread"
            "System.Threading.ThreadPool"
            "System.Threading.Timer"
            "System.Transactions"
            "System.Transactions.Local"
            "System.ValueTuple"
            "System.Web"
            "System.Web.HttpUtility"
            "System.Windows"
            "System.Xml"
            "System.Xml.Linq"
            "System.Xml.ReaderWriter"
            "System.Xml.Serialization"
            "System.Xml.XDocument"
            "System.Xml.XmlDocument"
            "System.Xml.XmlSerializer"
            "System.Xml.XPath"
            "System.Xml.XPath.XDocument"
            "WindowsBase"

            // .NET 5.0 list
            "System.Numerics"
            "netstandard"
            "Microsoft.CSharp"
            "Microsoft.VisualBasic.Core"
            "Microsoft.VisualBasic"
            "Microsoft.Win32.Primitives"
            "mscorlib"
            "netstandard"
            "System.AppContext"
            "System.Buffers"
            "System.Collections.Concurrent"
            "System.Collections"
            "System.Collections.Immutable"
            "System.Collections.NonGeneric"
            "System.Collections.Specialized"
            "System.ComponentModel.Annotations"
            "System.ComponentModel.DataAnnotations"
            "System.ComponentModel"
            "System.ComponentModel.EventBasedAsync"
            "System.ComponentModel.Primitives"
            "System.ComponentModel.TypeConverter"
            "System.Configuration"
            "System.Console"
            "System.Core"
            "System.Data.Common"
            "System.Data.DataSetExtensions"
            "System.Data"
            "System.Diagnostics.Contracts"
            "System.Diagnostics.Debug"
            "System.Diagnostics.DiagnosticSource"
            "System.Diagnostics.FileVersionInfo"
            "System.Diagnostics.Process"
            "System.Diagnostics.StackTrace"
            "System.Diagnostics.TextWriterTraceListener"
            "System.Diagnostics.Tools"
            "System.Diagnostics.TraceSource"
            "System.Diagnostics.Tracing"
            "System"
            "System.Drawing"
            "System.Drawing.Primitives"
            "System.Dynamic.Runtime"
            "System.Formats.Asn1"
            "System.Globalization.Calendars"
            "System.Globalization"
            "System.Globalization.Extensions"
            "System.IO.Compression.Brotli"
            "System.IO.Compression"
            "System.IO.Compression.FileSystem"
            "System.IO.Compression.ZipFile"
            "System.IO"
            "System.IO.FileSystem"
            "System.IO.FileSystem.DriveInfo"
            "System.IO.FileSystem.Primitives"
            "System.IO.FileSystem.Watcher"
            "System.IO.IsolatedStorage"
            "System.IO.MemoryMappedFiles"
            "System.IO.Pipes"
            "System.IO.UnmanagedMemoryStream"
            "System.Linq"
            "System.Linq.Expressions"
            "System.Linq.Parallel"
            "System.Linq.Queryable"
            "System.Memory"
            "System.Net"
            "System.Net.Http"
            "System.Net.Http.Json"
            "System.Net.HttpListener"
            "System.Net.Mail"
            "System.Net.NameResolution"
            "System.Net.NetworkInformation"
            "System.Net.Ping"
            "System.Net.Primitives"
            "System.Net.Requests"
            "System.Net.Security"
            "System.Net.ServicePoint"
            "System.Net.Sockets"
            "System.Net.WebClient"
            "System.Net.WebHeaderCollection"
            "System.Net.WebProxy"
            "System.Net.WebSockets.Client"
            "System.Net.WebSockets"
            "System.Numerics"
            "System.Numerics.Vectors"
            "System.ObjectModel"
            "System.Reflection.DispatchProxy"
            "System.Reflection"
            "System.Reflection.Emit"
            "System.Reflection.Emit.ILGeneration"
            "System.Reflection.Emit.Lightweight"
            "System.Reflection.Extensions"
            "System.Reflection.Metadata"
            "System.Reflection.Primitives"
            "System.Reflection.TypeExtensions"
            "System.Resources.Reader"
            "System.Resources.ResourceManager"
            "System.Resources.Writer"
            "System.Runtime.CompilerServices.Unsafe"
            "System.Runtime.CompilerServices.VisualC"
            "System.Runtime"
            "System.Runtime.Extensions"
            "System.Runtime.Handles"
            "System.Runtime.InteropServices"
            "System.Runtime.InteropServices.RuntimeInformation"
            "System.Runtime.Intrinsics"
            "System.Runtime.Loader"
            "System.Runtime.Numerics"
            "System.Runtime.Serialization"
            "System.Runtime.Serialization.Formatters"
            "System.Runtime.Serialization.Json"
            "System.Runtime.Serialization.Primitives"
            "System.Runtime.Serialization.Xml"
            "System.Security.Claims"
            "System.Security.Cryptography.Algorithms"
            "System.Security.Cryptography.Csp"
            "System.Security.Cryptography.Encoding"
            "System.Security.Cryptography.Primitives"
            "System.Security.Cryptography.X509Certificates"
            "System.Security"
            "System.Security.Principal"
            "System.Security.SecureString"
            "System.ServiceModel.Web"
            "System.ServiceProcess"
            "System.Text.Encoding.CodePages"
            "System.Text.Encoding"
            "System.Text.Encoding.Extensions"
            "System.Text.Encodings.Web"
            "System.Text.Json"
            "System.Text.RegularExpressions"
            "System.Threading.Channels"
            "System.Threading"
            "System.Threading.Overlapped"
            "System.Threading.Tasks.Dataflow"
            "System.Threading.Tasks"
            "System.Threading.Tasks.Extensions"
            "System.Threading.Tasks.Parallel"
            "System.Threading.Thread"
            "System.Threading.ThreadPool"
            "System.Threading.Timer"
            "System.Transactions"
            "System.Transactions.Local"
            "System.ValueTuple"
            "System.Web"
            "System.Web.HttpUtility"
            "System.Windows"
            "System.Xml"
            "System.Xml.Linq"
            "System.Xml.ReaderWriter"
            "System.Xml.Serialization"
            "System.Xml.XDocument"
            "System.Xml.XmlDocument"
            "System.Xml.XmlSerializer"
            "System.Xml.XPath"
            "System.Xml.XPath.XDocument"
            "WindowsBase"
        ]

    // The set of references entered into the TcConfigBuilder for scripts prior to computing the load closure. 
    let basicReferencesForScriptLoadClosure useFsiAuxLib useSdkRefs assumeDotNetFramework =
        fetchPathsForDefaultReferencesForScriptsAndOutOfProjectSources useFsiAuxLib useSdkRefs assumeDotNetFramework
