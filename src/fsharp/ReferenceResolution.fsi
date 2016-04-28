// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.


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
    
    /// Indicates whether the resolve should follow compile-time rules or runtime rules.                      
    type ResolutionEnvironment = 
        | CompileTimeLike 
        | RuntimeLike      // Don't allow stubbed-out reference assemblies
        | DesigntimeLike 

    /// Get the Reference Assemblies directory for the .NET Framework on Window
    val DotNetFrameworkReferenceAssembliesRootDirectoryOnWindows : string

    /// Information about a resolved file.
    type ResolvedFile = 
        { /// Item specification.
          itemSpec:string
          /// Location that the assembly was resolved from.
          resolvedFrom:ResolvedFrom
          /// The long fusion name of the assembly.
          fusionName:string
          /// The version of the assembly (like 4.0.0.0).
          version:string
          /// The name of the redist the assembly was found in.
          redist:string        
          /// Round-tripped baggage string.
          baggage:string
        }
    
    /// Reference resolution results. All paths are fully qualified.
    type ResolutionResults = 
        { /// Paths to primary references.
          resolvedFiles:ResolvedFile[]
          /// Paths to dependencies.
          referenceDependencyPaths:string[]
          /// Paths to related files (like .xml and .pdb).
          relatedPaths:string[]
          /// Paths to satellite assemblies used for localization.
          referenceSatellitePaths:string[]
          /// Additional files required to support multi-file assemblies.
          referenceScatterPaths:string[]
          /// Paths to files that reference resolution recommend be copied to the local directory.
          referenceCopyLocalPaths:string[]
          /// Binding redirects that reference resolution recommends for the app.config file.
          suggestedBindingRedirects:string[] }
    

    /// Perform assembly resolution on the given references.
    val Resolve:
                resolutionEnvironment: ResolutionEnvironment *
                references:seq<string (* baggage *) * string> * 
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
                logwarning:(string->string->unit) *
                logerror:(string->string->unit)
             -> ResolutionResults
