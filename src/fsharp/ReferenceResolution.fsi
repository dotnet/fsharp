// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Viz

/// This type exists to have a concrete 'Target' type for a DebuggerVisualizerAttribute.
/// Ideally it would be out in its own assembly, but then the compiler would need to take a dependency on that assembly, so instead we 
/// pragmatically just shove this into the compiler assembly itself.
type internal Visualizable =
    new : obj -> Visualizable
    member Data : obj
    /// assuming this assembly is already in the debuggee process, then Viz.Visualiable.Make(foo) in the Watch window will make a visualizer for foo
    static member Make : obj -> Visualizable

namespace Microsoft.FSharp.Compiler

module internal MSBuildResolver = 

    exception ResolutionFailure

    val SupportedNetFrameworkVersions : Set<string>
    val HighestInstalledNetFrameworkVersionMajorMinor : unit -> int * string
    
    /// Describes the location where the reference was found.
    type ResolvedFrom =
        | AssemblyFolders
        | AssemblyFoldersEx
        | TargetFrameworkDirectory
        | RawFileName
        | GlobalAssemblyCache
        | Path of string
        | Unknown
    
    /// Whether the resolve should follow compile-time rules or runtime rules.                      
    type ResolutionEnvironment = 
        | CompileTimeLike 
        | RuntimeLike      // Don't allow stubbed-out reference assemblies
        | DesigntimeLike 

#if SILVERLIGHT
#else

    val DotNetFrameworkReferenceAssembliesRootDirectory : string

    /// Information about a resolved file.
    type ResolvedFile = {
            /// Item specification
            itemSpec:string
            /// Location that the assembly was resolved from
            resolvedFrom:ResolvedFrom
            /// The long fusion name of the assembly
            fusionName:string
            /// The version of the assembly (like 4.0.0.0)
            version:string
            /// The name of the redist the assembly was found in
            redist:string        
            /// Round-tripped baggage string
            baggage:string
        }
    
    /// Reference resolution results. All paths are fully qualified.
    type ResolutionResults = {
        /// Paths to primary references
        resolvedFiles:ResolvedFile array
        /// Paths to dependencies
        referenceDependencyPaths:string array
        /// Paths to related files (like .xml and .pdb)
        relatedPaths:string array
        /// Paths to satellite assemblies used for localization.
        referenceSatellitePaths:string array
        /// Additional files required to support multi-file assemblies.
        referenceScatterPaths:string array
        /// Paths to files that reference resolution recommend be copied to the local directory
        referenceCopyLocalPaths:string array
        /// Binding redirects that reference resolution recommends for the app.config file.
        suggestedBindingRedirects:string array
        }
    
    /// Callback for errors and warnings.
    type ErrorWarningCallbackSig = 
        ((*code:*)string->(*message*)string->unit)


    val Resolve :
                resolutionEnvironment: ResolutionEnvironment *
                references:(string*(*baggage*)string)[] * 
                targetFrameworkVersion:string *
                targetFrameworkDirectories:string list *
                targetProcessorArchitecture:string *
                outputDirectory:string *
                fsharpBinariesDir:string *
                explicitIncludeDirs:string list *
                implicitIncludeDir:string *
                frameworkRegistryBase:string *
                assemblyFoldersSuffix:string *
                assemblyFoldersConditions:string *
                logmessage:(string->unit) *
                logwarning:ErrorWarningCallbackSig *
                logerror:ErrorWarningCallbackSig -> ResolutionResults
#endif
