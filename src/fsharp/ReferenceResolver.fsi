// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.CodeAnalysis

open System

exception internal LegacyResolutionFailure

[<RequireQualifiedAccess>]
type internal LegacyResolutionEnvironment =
    /// Indicates a script or source being edited or compiled. Uses reference assemblies (not implementation assemblies).
    | EditingOrCompilation of isEditing: bool

    /// Indicates a script or source being dynamically compiled and executed. Uses implementation assemblies.
    | CompilationAndEvaluation 

type internal LegacyResolvedFile =
    { 
        /// Item specification.
        itemSpec:string

        /// Prepare textual information about where the assembly was resolved from, used for tooltip output
        prepareToolTip: string * string -> string

        /// Round-tripped baggage 
        baggage:string
    }
  
[<AllowNullLiteral>]
type internal ILegacyReferenceResolver =
    /// Get the "v4.5.1"-style moniker for the highest installed .NET Framework version.
    /// This is the value passed back to Resolve if no explicit "mscorlib" has been given.
    ///
    /// Note: If an explicit "mscorlib" is given, then --noframework is being used, and the whole ReferenceResolver logic is essentially
    /// unused.  However in the future an option may be added to allow an explicit specification of
    /// a .NET Framework version to use for scripts.
    abstract HighestInstalledNetFrameworkVersion: unit -> string

    /// Perform assembly resolution on the given references under the given conditions
    abstract Resolve: 
            resolutionEnvironment: LegacyResolutionEnvironment *
            references:(string * string) [] *
            targetFrameworkVersion:string *
            targetFrameworkDirectories:string list *
            targetProcessorArchitecture:string *
            fsharpCoreDir:string *
            explicitIncludeDirs:string list *
            implicitIncludeDir:string *
            logMessage:(string -> unit) *
            logDiagnostic:(bool -> string -> string -> unit) ->
                LegacyResolvedFile []

    /// Get the Reference Assemblies directory for the .NET Framework (on Windows)
    /// This is added to the default resolution path for 
    /// design-time compilations.
    abstract DotNetFrameworkReferenceAssembliesRootDirectory: string
  
// Note, two implementations of this are provided, and no further implementations can be added from
// outside FSharp.Compiler.Service
[<Class; AllowNullLiteral; Obsolete("This API is obsolete and not for external use")>]
type LegacyReferenceResolver =
    internal new: impl: ILegacyReferenceResolver -> LegacyReferenceResolver
    member internal Impl: ILegacyReferenceResolver

