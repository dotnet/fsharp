// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Runtime.InteropServices

/// Narrow abstraction over the project system.
type internal AdviseProjectSiteChanges = delegate of unit -> unit  

[<ComImport; InterfaceType(ComInterfaceType.InterfaceIsIUnknown); Guid("ad98f020-bad0-0000-0000-abc037459871")>]
type internal IProvideProjectSite =
    abstract GetProjectSite : unit -> IProjectSite

/// Represents known F#-specific information about a project.
and internal IProjectSite = 

    /// List of files in the project. In the correct order.
    abstract CompilationSourceFiles : string[]

    /// Flags that the compiler would need to understand how to compile. Includes '-r'
    /// options but not source files
    abstract CompilationOptions : string[]

    /// The normalized '-r:' assembly references, without the '-r:'
    abstract CompilationReferences : string []

    /// The '-o:' output bin path, without the '-o:'
    abstract CompilationBinOutputPath : string option

    /// The name of the project file.
    abstract ProjectFileName : string

    /// Register for notifications for when the above change
    abstract AdviseProjectSiteChanges : callbackOwnerKey: string * AdviseProjectSiteChanges -> unit

    /// Register for notifications when project is cleaned/rebuilt (and thus any live TypeProviders should be refreshed)
    abstract AdviseProjectSiteCleaned : callbackOwnerKey: string * AdviseProjectSiteChanges -> unit
    
    // Register for notifications when project is closed.
    abstract AdviseProjectSiteClosed : callbackOwnerKey: string * AdviseProjectSiteChanges -> unit
 
    /// A user-friendly description of the project. Used only for developer/DEBUG tooltips and such.
    abstract Description : string

    /// The error list task reporter
    abstract BuildErrorReporter : Microsoft.VisualStudio.Shell.Interop.IVsLanguageServiceBuildErrorReporter2 option with get, set

    /// False type resolution errors are invalid. This occurs with orphaned source files. The prior 
    /// type checking state is unknown. In this case we don't want to squiggle the type checking files.
    abstract IsIncompleteTypeCheckEnvironment : bool

    /// target framework moniker
    abstract TargetFrameworkMoniker : string

    /// Project Guid
    abstract ProjectGuid : string

    /// timestamp the site was last loaded
    abstract LoadTime : System.DateTime 

    abstract ProjectProvider : IProvideProjectSite option