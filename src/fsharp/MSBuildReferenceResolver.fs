// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal Microsoft.FSharp.Compiler.MSBuildReferenceResolver 

    open System
    open System.IO
    open System.Reflection

#if FX_RESHAPED_REFLECTION
    open Microsoft.FSharp.Core.ReflectionAdapters
#endif
#if RESHAPED_MSBUILD
    open Microsoft.FSharp.Compiler.MsBuildAdapters
    open Microsoft.FSharp.Compiler.ToolLocationHelper
#endif

    open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library 
    open Microsoft.FSharp.Compiler.ReferenceResolver
    open Microsoft.Build.Tasks
    open Microsoft.Build.Utilities
    open Microsoft.Build.Framework

    /// Get the Reference Assemblies directory for the .NET Framework on Window.
    let DotNetFrameworkReferenceAssembliesRootDirectory = 
        // ProgramFilesX86 is correct for both x86 and x64 architectures 
        // (the reference assemblies are always in the 32-bit location, which is PF(x86) on an x64 machine)
        let PF = 
            match Environment.GetEnvironmentVariable("ProgramFiles(x86)") with
            | null -> Environment.GetEnvironmentVariable("ProgramFiles")  // if PFx86 is null, then we are 32-bit and just get PF
            | s -> s 
        PF + @"\Reference Assemblies\Microsoft\Framework\.NETFramework"


    /// When targeting .NET 2.0-3.5 on Windows, we expand the {WindowsFramework} and {ReferenceAssemblies} paths manually
    let internal ReplaceVariablesForLegacyFxOnWindows(dirs: string list) =
        let windowsFramework = Environment.GetEnvironmentVariable("windir")+ @"\Microsoft.NET\Framework"
        let referenceAssemblies = DotNetFrameworkReferenceAssembliesRootDirectory
        dirs |> List.map(fun d -> d.Replace("{WindowsFramework}",windowsFramework).Replace("{ReferenceAssemblies}",referenceAssemblies))

    
    // ATTENTION!: the following code needs to be updated every time we are switching to the new MSBuild version because new .NET framework version was released
    // 1. List of frameworks
    // 2. DeriveTargetFrameworkDirectoriesFor45Plus
    // 3. HighestInstalledNetFrameworkVersion
    // 4. GetPathToDotNetFrameworkImlpementationAssemblies
    [<Literal>]    
    let private Net10 = "v1.0"

    [<Literal>]    
    let private Net11 = "v1.1"

    [<Literal>]    
    let private Net20 = "v2.0"

    [<Literal>]    
    let private Net30 = "v3.0"

    [<Literal>]    
    let private Net35 = "v3.5"

    [<Literal>]    
    let private Net40 = "v4.0"

    [<Literal>]    
    let private Net45 = "v4.5"

    [<Literal>]    
    let private Net451 = "v4.5.1"

    //[<Literal>]    
    //let private Net452 = "v4.5.2" // not available in Dev15 MSBuild version

    [<Literal>]    
    let private Net46 = "v4.6"

    [<Literal>]    
    let private Net461 = "v4.6.1"

    /// Get the path to the .NET Framework implementation assemblies by using ToolLocationHelper.GetPathToDotNetFramework.
    /// This is only used to specify the "last resort" path for assembly resolution.
    let GetPathToDotNetFrameworkImlpementationAssemblies(v) =
        let v =
            match v with
            | Net11 ->  Some TargetDotNetFrameworkVersion.Version11
            | Net20 ->  Some TargetDotNetFrameworkVersion.Version20
            | Net30 ->  Some TargetDotNetFrameworkVersion.Version30
            | Net35 ->  Some TargetDotNetFrameworkVersion.Version35
            | Net40 ->  Some TargetDotNetFrameworkVersion.Version40
            | Net45 ->  Some TargetDotNetFrameworkVersion.Version45
            | Net451 -> Some TargetDotNetFrameworkVersion.Version451
            //| Net452 -> Some TargetDotNetFrameworkVersion.Version452 // not available in Dev15 MSBuild version
            | Net46 -> Some TargetDotNetFrameworkVersion.Version46
            | Net461 -> Some TargetDotNetFrameworkVersion.Version461
            | _ -> assert false; None
        match v with
        | Some v -> 
            match ToolLocationHelper.GetPathToDotNetFramework v with
            | null -> []
            | x -> [x]
        | _ -> []

    let GetPathToDotNetFrameworkReferenceAssembliesFor40Plus(version) = 
#if WINDOWS_ONLY_COMPILER 
// FUTURE CLEANUP: This is the old implementation, equivalent to calling GetPathToStandardLibraries
// FUTURE CLEANUP: on .NET Framework.  But reshapedmsbuild.fs doesn't have an implementation of GetPathToStandardLibraries
// FUTURE CLEANUP: When we remove reshapedmsbuild.fs we can just call GetPathToStandardLibraries directly.
       // starting with .Net 4.0, the runtime dirs (WindowsFramework) are never used by MSBuild RAR
       let v =
           match version with
           | Net40 -> Some TargetDotNetFrameworkVersion.Version40
           | Net45 -> Some TargetDotNetFrameworkVersion.Version45
           | Net451 -> Some TargetDotNetFrameworkVersion.Version451
           //| Net452 -> Some TargetDotNetFrameworkVersion.Version452 // not available in Dev15 MSBuild version
           | Net46 -> Some TargetDotNetFrameworkVersion.Version46
           | Net461 -> Some TargetDotNetFrameworkVersion.Version461
           | _ -> assert false; None // unknown version - some parts in the code are not synced
       match v with
       | Some v -> 
           match ToolLocationHelper.GetPathToDotNetFrameworkReferenceAssemblies v with
           | null -> []
           | x -> [x]
       | None -> []
#else
      match ToolLocationHelper.GetPathToStandardLibraries(".NETFramework",version,"") with
      | null | "" -> []
      | x -> [x]
#endif

    /// Use MSBuild to determine the version of the highest installed framework.
    let HighestInstalledNetFrameworkVersion() =
        if box (ToolLocationHelper.GetPathToDotNetFramework(TargetDotNetFrameworkVersion.Version461)) <> null then Net461
        elif box (ToolLocationHelper.GetPathToDotNetFramework(TargetDotNetFrameworkVersion.Version46)) <> null then Net46
        // 4.5.2 enumeration is not available in Dev15 MSBuild version
        //elif box (ToolLocationHelper.GetPathToDotNetFramework(TargetDotNetFrameworkVersion.Version452)) <> null then Net452 
        elif box (ToolLocationHelper.GetPathToDotNetFramework(TargetDotNetFrameworkVersion.Version451)) <> null then Net451 
        elif box (ToolLocationHelper.GetPathToDotNetFramework(TargetDotNetFrameworkVersion.Version45)) <> null then Net45 
        else Net40 // version is 4.0 assumed since this code is running. 

    /// Derive the target framework directories.        
    let DeriveTargetFrameworkDirectories (targetFrameworkVersion:string, logMessage) =

        let targetFrameworkVersion =
            if not(targetFrameworkVersion.StartsWith("v",StringComparison.Ordinal)) then "v"+targetFrameworkVersion
            else targetFrameworkVersion

        let result =
            if targetFrameworkVersion.StartsWith(Net10, StringComparison.Ordinal) then ReplaceVariablesForLegacyFxOnWindows([@"{WindowsFramework}\v1.0.3705"])
            elif targetFrameworkVersion.StartsWith(Net11, StringComparison.Ordinal) then ReplaceVariablesForLegacyFxOnWindows([@"{WindowsFramework}\v1.1.4322"])
            elif targetFrameworkVersion.StartsWith(Net20, StringComparison.Ordinal) then ReplaceVariablesForLegacyFxOnWindows([@"{WindowsFramework}\v2.0.50727"])
            elif targetFrameworkVersion.StartsWith(Net30, StringComparison.Ordinal) then ReplaceVariablesForLegacyFxOnWindows([@"{ReferenceAssemblies}\v3.0"; @"{WindowsFramework}\v3.0"; @"{WindowsFramework}\v2.0.50727"])
            elif targetFrameworkVersion.StartsWith(Net35, StringComparison.Ordinal) then ReplaceVariablesForLegacyFxOnWindows([@"{ReferenceAssemblies}\v3.5"; @"{WindowsFramework}\v3.5"; @"{ReferenceAssemblies}\v3.0"; @"{WindowsFramework}\v3.0"; @"{WindowsFramework}\v2.0.50727"])
            else GetPathToDotNetFrameworkReferenceAssembliesFor40Plus(targetFrameworkVersion)

        let result = result |> Array.ofList                
        logMessage (sprintf "Derived target framework directories for version %s are: %s" targetFrameworkVersion (String.Join(",", result)))                
        result
 
    /// Describes the location where the reference was found, used only for debug and tooltip output
    type ResolvedFrom =
        | AssemblyFolders
        | AssemblyFoldersEx
        | TargetFrameworkDirectory
        | RawFileName
        | GlobalAssemblyCache
        | Path of string
        | Unknown
            
    /// Decode the ResolvedFrom code from MSBuild.
    let DecodeResolvedFrom(resolvedFrom:string) : ResolvedFrom = 
        match resolvedFrom with
        | "{RawFileName}" -> RawFileName
        | "{GAC}" -> GlobalAssemblyCache
        | "{TargetFrameworkDirectory}" -> TargetFrameworkDirectory
        | "{AssemblyFolders}" -> AssemblyFolders
        | r when r.Length >= 10 &&  "{Registry:" = r.Substring(0,10) -> AssemblyFoldersEx
        | r -> ResolvedFrom.Path r
        
    let TooltipForResolvedFrom(resolvedFrom, fusionName, redist) = 
      fun (originalReference,resolvedPath) -> 
        let originalReferenceName = originalReference

        let resolvedPath = // Don't show the resolved path if it is identical to what was referenced.
            if originalReferenceName = resolvedPath then String.Empty
            else resolvedPath

        let lineIfExists text =
            if String.IsNullOrEmpty text then ""
            else text.Trim(' ')+"\n"

        match resolvedFrom with 
        | AssemblyFolders ->
            lineIfExists resolvedPath
            + lineIfExists fusionName
            + FSComp.SR.assemblyResolutionFoundByAssemblyFoldersKey()
        | AssemblyFoldersEx -> 
            lineIfExists resolvedPath
            + lineIfExists fusionName
            + FSComp.SR.assemblyResolutionFoundByAssemblyFoldersExKey()
        | TargetFrameworkDirectory -> 
            lineIfExists resolvedPath
            + lineIfExists fusionName
            + FSComp.SR.assemblyResolutionNetFramework()
        | Unknown ->
            // Unknown when resolved by plain directory search without help from MSBuild resolver.
            lineIfExists resolvedPath
            + lineIfExists fusionName
        | RawFileName -> 
            lineIfExists fusionName
        | GlobalAssemblyCache -> 
            lineIfExists fusionName
            + lineIfExists (FSComp.SR.assemblyResolutionGAC())
            + lineIfExists redist
        | Path _ ->
            lineIfExists resolvedPath
            + lineIfExists fusionName  

    /// Perform assembly resolution by instantiating the ResolveAssemblyReference task directly from the MSBuild SDK.
    let ResolveCore(resolutionEnvironment: ResolutionEnvironment,
                    references:(string*(*baggage*)string)[], 
                    targetFrameworkVersion: string, 
                    targetFrameworkDirectories: string list,
                    targetProcessorArchitecture: string,                
                    fsharpCoreDir: string,
                    explicitIncludeDirs: string list,
                    implicitIncludeDir: string,
                    allowRawFileName: bool,
                    logMessage: (string -> unit), 
                    logErrorOrWarning: (bool -> string -> string -> unit)) =
                      
        let frameworkRegistryBase, assemblyFoldersSuffix, assemblyFoldersConditions = 
          "Software\Microsoft\.NetFramework", "AssemblyFoldersEx" , ""              
        if Array.isEmpty references then [| |] else

        let backgroundException = ref false

        let protect f = 
            if not !backgroundException then 
                try f() 
                with _ -> backgroundException := true

        let engine = 
            { new IBuildEngine with 
              member __.BuildProjectFile(projectFileName, targetNames, globalProperties, targetOutputs) = true
#if RESHAPED_MSBUILD 
              member __.LogCustomEvent(e) =  protect (fun () -> logMessage ((e.GetPropertyValue("Message")) :?> string))
              member __.LogErrorEvent(e) =   protect (fun () -> logErrorOrWarning true ((e.GetPropertyValue("Code")) :?> string) ((e.GetPropertyValue("Message")) :?> string))
              member __.LogMessageEvent(e) = protect (fun () -> logMessage ((e.GetPropertyValue("Message")) :?> string))
              member __.LogWarningEvent(e) = protect (fun () -> logErrorOrWarning false ((e.GetPropertyValue("Code")) :?> string)  ((e.GetPropertyValue("Message")) :?> string))
#else 
              member __.LogCustomEvent(e) =  protect (fun () -> logMessage e.Message)
              member __.LogErrorEvent(e) =   protect (fun () -> logErrorOrWarning true e.Code e.Message)
              member __.LogMessageEvent(e) = protect (fun () -> logMessage e.Message)
              member __.LogWarningEvent(e) = protect (fun () -> logErrorOrWarning false e.Code e.Message)
#endif 
              member __.ColumnNumberOfTaskNode with get() = 1 
              member __.LineNumberOfTaskNode with get() = 1 
              member __.ContinueOnError with get() = true 
              member __.ProjectFileOfTaskNode with get() = "" } 

        // Derive the target framework directory if none was supplied.
        let targetFrameworkDirectories =
            if targetFrameworkDirectories=[] then DeriveTargetFrameworkDirectories(targetFrameworkVersion, logMessage) 
            else targetFrameworkDirectories |> Array.ofList


        // Filter for null and zero length
        let references = references |> Array.filter(fst >> String.IsNullOrEmpty >> not) 

        // Determine the set of search paths for the resolution
        let searchPaths = 

            let explicitIncludeDirs = explicitIncludeDirs |> List.filter(String.IsNullOrEmpty >> not)

            let registry = sprintf "{Registry:%s,%s,%s%s}" frameworkRegistryBase targetFrameworkVersion assemblyFoldersSuffix assemblyFoldersConditions

            [|  // When compiling scripts, for some reason we have always historically put TargetFrameworkDirectory first
                // It is unclear why.
                match resolutionEnvironment with
                | CompileTimeLike -> yield "{TargetFrameworkDirectory}"
                | DesignTimeLike | RuntimeLike -> ()

                // Quick-resolve straight to filename first 
                if allowRawFileName then 
                    yield "{RawFileName}"
                yield! explicitIncludeDirs     // From -I, #I
                yield fsharpCoreDir    // Location of explicit reference to FSharp.Core, otherwise location of fsc.exe
                yield implicitIncludeDir   // Usually the project directory

                match resolutionEnvironment with
                | DesignTimeLike | RuntimeLike -> yield "{TargetFrameworkDirectory}"
                | CompileTimeLike -> ()

                yield registry
                yield "{AssemblyFolders}"
                yield "{GAC}"
                // use path to implementation assemblies as the last resort
                yield! GetPathToDotNetFrameworkImlpementationAssemblies targetFrameworkVersion 
             |]    
            
        let assemblies = 
#if RESHAPED_MSBUILD
            ignore references
            [||]
#else
            [| for (referenceName,baggage) in references -> 
               let item = new Microsoft.Build.Utilities.TaskItem(referenceName) :> ITaskItem
               item.SetMetadata("Baggage", baggage)
               item |]
#endif
        let rar = 
            ResolveAssemblyReference(BuildEngine=engine, TargetFrameworkDirectories=targetFrameworkDirectories,
                                     FindRelatedFiles=false, FindDependencies=false, FindSatellites=false, 
                                     FindSerializationAssemblies=false, Assemblies=assemblies, 
                                     SearchPaths=searchPaths, 
                                     AllowedAssemblyExtensions= [| ".dll" ; ".exe" |])
#if FX_RESHAPED_REFLECTION
        ignore targetProcessorArchitecture // Not implemented in reshapedmsbuild.fs
#else
        rar.TargetProcessorArchitecture <- targetProcessorArchitecture
        let targetedRuntimeVersionValue = typeof<obj>.Assembly.ImageRuntimeVersion
#if WINDOWS_ONLY_COMPILER
        rar.TargetedRuntimeVersion <- targetedRuntimeVersionValue
        rar.CopyLocalDependenciesWhenParentReferenceInGac <- true
#else
        // The properties TargetedRuntimeVersion and CopyLocalDependenciesWhenParentReferenceInGac 
        // are not available on Mono. So we only set them if available (to avoid a compile-time dependency). 
        if not Microsoft.FSharp.Compiler.AbstractIL.IL.runningOnMono then  
            typeof<ResolveAssemblyReference>.InvokeMember("TargetedRuntimeVersion",(BindingFlags.Instance ||| BindingFlags.SetProperty ||| BindingFlags.Public),null,rar,[| box targetedRuntimeVersionValue |])  |> ignore 
            typeof<ResolveAssemblyReference>.InvokeMember("CopyLocalDependenciesWhenParentReferenceInGac",(BindingFlags.Instance ||| BindingFlags.SetProperty ||| BindingFlags.Public),null,rar,[| box true |])  |> ignore 
#endif
#endif        
        
        let succeeded = rar.Execute()
        
        if not succeeded then 
            raise ResolutionFailure

        let resolvedFiles = 
            [| for p in rar.ResolvedFiles -> 
                let resolvedFrom = DecodeResolvedFrom(p.GetMetadata("ResolvedFrom"))
                let fusionName = p.GetMetadata("FusionName")
                let redist = p.GetMetadata("Redist") 
                { itemSpec = p.ItemSpec
                  prepareToolTip = TooltipForResolvedFrom(resolvedFrom, fusionName, redist)
                  baggage = p.GetMetadata("Baggage") } |]

        resolvedFiles

    let Resolver =
       { new ReferenceResolver.Resolver with 
           member __.HighestInstalledNetFrameworkVersion() = HighestInstalledNetFrameworkVersion()
           member __.DotNetFrameworkReferenceAssembliesRootDirectory =  DotNetFrameworkReferenceAssembliesRootDirectory

           /// Perform the resolution on rooted and unrooted paths, and then combine the results.
           member __.Resolve(resolutionEnvironment, references, targetFrameworkVersion, targetFrameworkDirectories, targetProcessorArchitecture,                
                             fsharpCoreDir, explicitIncludeDirs, implicitIncludeDir, logMessage, logErrorOrWarning) =

                // The {RawFileName} target is 'dangerous', in the sense that is uses <c>Directory.GetCurrentDirectory()</c> to resolve unrooted file paths.
                // It is unreliable to use this mutable global state inside Visual Studio.  As a result, we partition all references into a "rooted" set
                // (which contains e.g. C:\MyDir\MyAssem.dll) and "unrooted" (everything else).  We only allow "rooted" to use {RawFileName}.  Note that
                // unrooted may still find 'local' assemblies by virtue of the fact that "implicitIncludeDir" is one of the places searched during 
                // assembly resolution.
                let references = 
                    [| for ((file,baggage) as data) in references -> 
                            // However, MSBuild will not resolve 'relative' paths, even when e.g. implicitIncludeDir is part of the search.  As a result,
                            // if we have an unrooted path+filename, we'll assume this is relative to the project directory and root it.
                            if FileSystem.IsPathRootedShim(file) then
                                data  // fine, e.g. "C:\Dir\foo.dll"
                            elif not(file.Contains("\\") || file.Contains("/")) then
                                data  // fine, e.g. "System.Transactions.dll"
                            else
                                // We have a 'relative path', e.g. "bin/Debug/foo.exe" or "..\Yadda\bar.dll"
                                // turn it into an absolute path based at implicitIncludeDir
                                (Path.Combine(implicitIncludeDir, file), baggage) |]

                let rooted, unrooted = references |> Array.partition (fst >> FileSystem.IsPathRootedShim)

                let rootedResults = ResolveCore(resolutionEnvironment, rooted,  targetFrameworkVersion, targetFrameworkDirectories, targetProcessorArchitecture, fsharpCoreDir, explicitIncludeDirs, implicitIncludeDir, true, logMessage, logErrorOrWarning)

                let unrootedResults = ResolveCore(resolutionEnvironment, unrooted,  targetFrameworkVersion, targetFrameworkDirectories, targetProcessorArchitecture, fsharpCoreDir, explicitIncludeDirs, implicitIncludeDir, false, logMessage, logErrorOrWarning)

                // now unify the two sets of results
                Array.concat [| rootedResults; unrootedResults |]
       } 
