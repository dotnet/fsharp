// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal rec Microsoft.VisualStudio.FSharp.Editor.SiteProvider

open System
open System.IO
open System.Collections.Concurrent
open System.Diagnostics

open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.Implementation.TaskList
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.TextManager.Interop

open VSLangProj

/// An additional interface that an IProjectSite object can implement to indicate it has an FSharpProjectOptions 
/// already available, so we don't have to recreate it
type private IHaveCheckOptions = 
    abstract OriginalCheckOptions : unit -> string[] * FSharpProjectOptions

let projectDisplayNameOf projectFileName =
    if String.IsNullOrWhiteSpace projectFileName then projectFileName
    else Path.GetFileNameWithoutExtension projectFileName

/// A value and a function to recompute/refresh the value.  The function is passed a flag indicating if a refresh is happening.
type Refreshable<'T> = 'T * (bool -> 'T)

