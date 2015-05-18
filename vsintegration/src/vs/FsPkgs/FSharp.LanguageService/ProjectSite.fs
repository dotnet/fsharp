// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.


namespace Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open System.Runtime.InteropServices

// This is the list of known owners of callbacks that may register themselves.
// Consumers not in this list should use a GUID our some other strongly unique key string
module internal KnownAdviseProjectSiteChangesCallbackOwners = 
    let LanguageService = "F# Language Service"

/// Narrow abstraction over the project system.
type internal AdviseProjectSiteChanges = delegate of unit -> unit  
type internal IProjectSite = 
    /// List of files in the project. In the correct order.
    abstract SourceFilesOnDisk : unit -> string[]
    /// Flags that the compiler would need to understand how to compile.
    abstract CompilerFlags : unit -> string[]
    /// Register for notifications for when the above change
    abstract AdviseProjectSiteChanges : (*callbackOwnerKey*)string * AdviseProjectSiteChanges -> unit
    /// Register for notifications when project is cleaned/rebuilt (and thus any live TypeProviders should be refreshed)
    abstract AdviseProjectSiteCleaned : (*callbackOwnerKey*)string * AdviseProjectSiteChanges -> unit
    /// A user-friendly description of the project. Used only for developer/DEBUG tooltips and such.
    abstract DescriptionOfProject : unit -> string
    /// The name of the project file.
    abstract ProjectFileName : unit -> string
    /// The error list task provider (should one exist - null, otherwise)
    abstract ErrorListTaskProvider : unit -> TaskProvider option
    /// The error list task reporter
    abstract ErrorListTaskReporter : unit -> Microsoft.VisualStudio.FSharp.LanguageService.TaskReporter option
    /// False type resolution errors are invalid. This occurs with orphaned source files. The prior 
    /// type checking state is unknown. In this case we don't want to squiggle the type checking files.
    abstract IsTypeResolutionValid : bool
    /// target framework moniker
    abstract TargetFrameworkMoniker : string
    /// timestamp the site was last loaded
    abstract LoadTime : System.DateTime 
    /// 
    abstract AssemblyReferenceIsTypeProvider : string -> unit

  [<ComImport; InterfaceType(ComInterfaceType.InterfaceIsIUnknown); Guid("ad98f020-bad0-0000-0000-abc037459871")>]
 type internal IProvideProjectSite =
    abstract GetProjectSite : unit -> IProjectSite
