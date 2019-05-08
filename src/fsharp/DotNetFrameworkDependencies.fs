// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Functions to retrieve framework dependencies

module internal FSharp.Compiler.DotNetFrameworkDependencies

    open System
    open System.Collections.Generic
    open System.Diagnostics
    open System.IO
    open System.Reflection

    type private TypeInThisAssembly = class end

    let getFSharpCoreLibraryName = "FSharp.Core"
    let getFsiLibraryName = "FSharp.Compiler.Interactive.Settings"
    let frameworkDir = Path.GetDirectoryName(typeof<Object>.Assembly.Location)
    let getDefaultFSharpCoreReference = typeof<Microsoft.FSharp.Core.Unit>.Assembly.Location
    let getFSharpCompilerLocation = Path.GetDirectoryName(typeof<TypeInThisAssembly>.Assembly.Location)

    // Use the ValueTuple that is executing with the compiler if it is from System.ValueTuple
    // or the System.ValueTuple.dll that sits alongside the compiler.  (Note we always ship one with the compiler)
    let getDefaultSystemValueTupleReference () =
        try
            let asm = typeof<System.ValueTuple<int, int>>.Assembly
            if asm.FullName.StartsWith("System.ValueTuple", StringComparison.OrdinalIgnoreCase) then
                Some asm.Location
            else
                let location = Path.GetDirectoryName(typeof<TypeInThisAssembly>.Assembly.Location)
                let valueTuplePath = Path.Combine(location, "System.ValueTuple.dll")
                if File.Exists(valueTuplePath) then
                    Some valueTuplePath
                else
                    None
        with _ -> None

    // Compare nuget version strings
    //
    // Format:
    // =======
    //      $(Major).$(Minor).$(Build) [-SomeSuffix]
    //   Major, Minor, Build collates normally
    //   Strings without -SomeSuffix collate higher than SomeSuffix, 
    //   SomeSuffix collates using normal alphanumeric rules
    //
    let deconstructVersion (version:string)  =
        let version, suffix =
            let pos = version.IndexOf("-")
            if pos >= 0 then
                version.Substring(0, pos), version.Substring(pos + 1)
            else version, ""

        let elements = version.Split('.')
        if elements.Length < 3 then
            struct (0, 0, 0, suffix)
        else
            struct (Int32.Parse(elements.[0]), Int32.Parse(elements.[1]), Int32.Parse(elements.[2]), suffix)

    let versionCompare c1 c2 =
        if c1 = c2 then 0
        else
            try
                let struct (major1, minor1, build1, suffix1 ) = deconstructVersion c1
                let struct (major2, minor2, build2, suffix2 ) = deconstructVersion c2
                let v = major1 - major2
                if v <> 0 then v
                else
                    let v = minor1 - minor2
                    if v <> 0 then v
                    else
                        let v = build1 - build2
                        if v <> 0 then v
                        else
                            match String.IsNullOrEmpty(suffix1), String.IsNullOrEmpty(suffix2) with
                            | true, true -> 0
                            | true, false -> 1
                            | false, true -> -1
                            | false, false -> String.Compare(suffix1, suffix2, StringComparison.InvariantCultureIgnoreCase)
            with _ -> 0

    let executionTfm =
        let file =
            try
                let depsJsonPath = Path.ChangeExtension(Assembly.GetEntryAssembly().Location, "deps.json")
                if File.Exists depsJsonPath then 
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
        | _, -1 -> None
        | pos, length -> Some ("netcoreapp" + file.Substring(pos, length))


    let getFrameworkRefsPackDirectoryPath =
        match executionTfm with
        | Some _ ->
            let appRefDir = Path.Combine(getFSharpCompilerLocation, "../../../packs/Microsoft.NETCore.App.Ref")
            if Directory.Exists(appRefDir) then
                Some appRefDir
            else
                None
        | _ -> None

    let isInReferenceAssemblyPackDirectory filename =
        match getFrameworkRefsPackDirectoryPath with
        | Some appRefDir ->
            let path = Path.GetDirectoryName(filename)
            path.StartsWith(appRefDir, StringComparison.OrdinalIgnoreCase)
        | _ -> false

    let getFrameworkRefsPackDirectory =
        match executionTfm, getFrameworkRefsPackDirectoryPath with
        | Some tfm, Some appRefDir ->
            try
                let refDirs = Directory.GetDirectories(appRefDir)
                let versionPath = refDirs |> Array.sortWith (versionCompare) |> Array.last
                Some(Path.Combine(versionPath, "ref", tfm))
            with | _ -> None
        | _ -> None



    let getDependenciesOf assemblyReferences =
        let assemblies = new Dictionary<string, string>()

        // Identify path to a dll in the framework directory from a simple name
        let frameworkPathFromSimpleName simpleName =
            let pathDll = Path.Combine(frameworkDir, simpleName + ".dll")
            if not (File.Exists(pathDll)) then
                let pathExe = Path.Combine(frameworkDir, simpleName + ".exe")
                if not (File.Exists(pathExe)) then
                    pathDll
                else
                    pathExe
            else
                pathDll

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
                    assemblies.Add(referenceName, path) |> ignore
                    if referenceName <> "System.Private.CoreLib" then
                        let asm = System.Reflection.Assembly.LoadFrom(path)
                        for reference in asm.GetReferencedAssemblies() do
                            // System.Private.CoreLib doesn't load with reflection
                            traverseDependencies reference.Name
                with e -> ()

        assemblyReferences |> List.iter(traverseDependencies)
        assemblies

    // This list is the default set of references for "non-project" files. 
    //
    // These DLLs are
    //    (a) included in the environment used for all .fsx files (see service.fs)
    //    (b) included in environment for files 'orphaned' from a project context
    //            -- for orphaned files (files in VS without a project context)
    //            -- for files given on a command line without --noframework set
    let getDesktopDefaultReferences useFsiAuxLib = [
        yield "mscorlib"
        yield "System"
        yield "System.Xml"
        yield "System.Runtime.Remoting"
        yield "System.Runtime.Serialization.Formatters.Soap"
        yield "System.Data"
        yield "System.Drawing"
        yield "System.Core"
        yield getDefaultFSharpCoreReference
        if useFsiAuxLib then yield getFsiLibraryName

        // always include a default reference to System.ValueTuple.dll in scripts and out-of-project sources 
        match getDefaultSystemValueTupleReference() with
        | None -> ()
        | Some v -> yield v

        // These are the Portable-profile and .NET Standard 1.6 dependencies of FSharp.Core.dll.  These are needed
        // when an F# sript references an F# profile 7, 78, 259 or .NET Standard 1.6 component which in turn refers 
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
                            yield Path.Combine(frameworkDir, "netstandard.dll")
                            yield getDefaultFSharpCoreReference
                            if useFsiAuxLib then yield getFsiLibraryName
                        ]).Values |> Seq.toList

                    if useSdkRefs then
                        // Go fetch references
                        match getFrameworkRefsPackDirectory with
                        | Some path ->
                            try [ yield! Directory.GetFiles(path, "*.dll")
                                  yield getDefaultFSharpCoreReference
                                  if useFsiAuxLib then yield getFsiLibraryName
                                ]
                            with | _ -> List.empty<string>
                        | None ->
                            getImplementationReferences ()
                    else
                        getImplementationReferences ()
                dependencies
        results

    let defaultReferencesForScriptsAndOutOfProjectSources assumeDotNetFramework useSdkRefs =
        fetchPathsForDefaultReferencesForScriptsAndOutOfProjectSources false useSdkRefs assumeDotNetFramework

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
