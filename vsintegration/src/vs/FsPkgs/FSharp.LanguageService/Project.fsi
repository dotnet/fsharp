// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.Shell.Interop

module internal ProjectSiteOptions = 
    val Create : IProjectSite*string -> CheckOptions
    val ToProjectSite : filename : string * CheckOptions -> IProjectSite
    
[<Sealed>]
type internal Artifacts = 
    new : adviseProjectSiteChanges: unit -> Artifacts        
    member SetSource : buffer:IVsTextLines * source:IdealSource -> unit
    member UnsetSource : buffer:IVsTextLines -> unit
    member TryGetProjectSite : hierarchy:IVsHierarchy -> IProjectSite option
    member TryFindOwningProject : rdt:IVsRunningDocumentTable * filename:string -> IProjectSite option
    /// Find the project that "owns" this filename.  That is,
    ///  - if the file is associated with an F# IVsHierarchy in the RDT, and
    ///  - the .fsproj has this file in its list of <Compile> items,
    /// then the project is considered the 'owner'.  Otherwise a 'single file project' is returned. 
    member FindOwningProject : rdt:IVsRunningDocumentTable * filename:string * enableStandaloneFileIntellisense:bool -> IProjectSite
    member GetSourceOfFilename : rdt:IVsRunningDocumentTable * filename:string -> IdealSource option // REVIEW: Should be TryGetSourceOfFilename
    member GetDefinesForFile : rdt:IVsRunningDocumentTable * filename:string * enableStandaloneFileIntellisense:bool-> string list
    
    
