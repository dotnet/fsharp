// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// This file contains idealized versions of the MLS in which things have been factored into interfaces rather
/// than being coupled everywhere.

namespace Microsoft.VisualStudio.FSharp.LanguageService
open System.Text
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.FSharp.Compiler.SourceCodeServices


/// XmlDocumentation provider
type internal IdealDocumentationProvider =
    /// Append the given raw XML formatted into the string builder
    abstract AppendDocumentationFromProcessedXML : StringBuilder * string * bool * bool * string option-> unit
    /// Appends text for the given filename and signature into the StringBuilder
    abstract AppendDocumentation : StringBuilder * string * string * bool * bool * string option-> unit

type internal ProjectSiteRebuildCallbackSignature = IProjectSite -> unit
/// Invoked when a file that IdealSource depends on changes
type internal DependencyFileChangeCallbackSignature = string -> unit

/// Ideal Source 
type internal IdealSource =
    /// Request colorization of the whole source file
    abstract RecolorizeWholeFile : unit -> unit
    abstract RecolorizeLine : line:int -> unit
    // Called to notify the source that the user has changed the source text in the editor.
    abstract RecordChangeToView: unit -> unit
    // Called to notify the source the file has been redisplayed.
    abstract RecordViewRefreshed: unit -> unit
    // If true, the file displayed has changed and needs to be redisplayed to some extent.
    abstract NeedsVisualRefresh : bool with get
    /// Number of most recent change to this file.
    abstract ChangeCount : int with get,set
    /// Timestamp of the last change
    abstract DirtyTime : int with get,set
    /// Whether or not this source is closed.
    abstract IsClosed: unit -> bool with get
    /// Store a ProjectSite for obtaining a task provider
    abstract ProjectSite : IProjectSite option with get,set
    /// Specify the files that should trigger a rebuild for the project behind this source
    abstract SetDependencyFiles : string list -> bool
    /// Function the source should call when there have been changes to files.
    abstract SetDependencyFileChangeCallback : ProjectSiteRebuildCallbackSignature*DependencyFileChangeCallbackSignature -> unit
    
    
