// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open System.IO
open System.Collections.Generic
open System.Collections
open System.Configuration
open System.Diagnostics
open System.Globalization
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop 
open Microsoft.VisualStudio.TextManager.Interop 
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.OLE.Interop
open Microsoft.FSharp.Compiler.SourceCodeServices


/// The extract of information from the parsed AST related to navigation bars and regions.
///
///    -- Each time we get a new FSharpParseFileResults, we create a new FSharpNavigationAndRegionInfo that "folds in" the
///       new region information while keeping the same unique identifiers for the regions in the text
///
///    -- The navigation items in the object are computed lazily 
    
type internal FSharpNavigationAndRegionInfo(parseResults:FSharpParseFileResults, prevRegions: Map<string,uint32>, regionGenerator: unit -> uint32) =
        
    // Do we need to update the list?
    let mutable navigationItems : FSharpNavigationItems option = None
    let mutable displayedRegions = prevRegions

    // Utilities
    let copyTo (target:ArrayList) arr selector =
        target.Clear()
        for m in arr do 
            let (m:FSharpNavigationDeclarationItem) = selector m
            let memb = new DropDownMember(m.Name, TextSpanOfRange m.Range, m.Glyph, DROPDOWNFONTATTR.FONTATTR_PLAIN)
            target.Add(memb) |> ignore

    let findDeclaration (declarations:'a[]) allowEqualEndLine (selector:'a -> FSharpNavigationDeclarationItem) line _col = 
        let _, sel, _ = 
            declarations
            |> Array.fold (fun (n, idx, size) decl -> 
                // TODO this looks like an algorithm that was ad-hoc'd to deal with bad ranges from the interactiveChecker, maybe can be simplified now
                let r1 = (selector decl).Range // 1-base line numbers
                let sl = r1.StartLine 
                let el = r1.EndLine
                if ((line >= sl) && (line < el || (allowEqualEndLine && el = line))) && (el - sl) < size then
                    (n+1, n, el - sl) 
                else 
                    (n+1, idx, size)                    
            ) (0, -1, Int32.MaxValue)
                
        if sel<> -1 then sel else
            let mutable lastBefore = -1
            let mutable lastLine = -1
            for i in 0 .. declarations.Length - 1 do
                let decl = declarations.[i]
                let r1 = (selector decl).Range // 1-base line numbers
                let el = r1.EndLine
                if el < line && el > lastLine then 
                    lastBefore <- i
                    lastLine <- el
            if (lastBefore = -1 && declarations.Length > 0) 
            then 0 else lastBefore 
        
    let ensureNavigationItemsUpToDate() =
        if navigationItems.IsNone then
            navigationItems <- Some(parseResults.GetNavigationItems())
        
    member this.FileName  = parseResults.FileName
    member this.Regions = displayedRegions
    member this.RegionGenerator = regionGenerator
        
    static member WithNewParseInfo(parseResults:FSharpParseFileResults, prev:FSharpNavigationAndRegionInfo option) =
            match prev with
            | Some(prev) -> 
                let regs = 
                    if (prev.FileName = parseResults.FileName) then 
                        prev.Regions 
                    else 
                        Map.empty 
                new FSharpNavigationAndRegionInfo(parseResults, regs, prev.RegionGenerator)
            | None -> 
                let generator = 
                  let count = ref 0u
                  (fun () -> count := !count + 1u; !count) // unchecked? overflow?
                new FSharpNavigationAndRegionInfo(parseResults, Map.empty, generator)
                
        // Synchronize...
    member this.SynchronizeNavigationDropDown(file, line, col:int, dropDownTypes:ArrayList, dropDownMembers:ArrayList, selectedType:int byref, selectedMember:int byref) =    
            
            try
                let current = parseResults.FileName
                
                if file <> current then
                    dropDownTypes.Clear()
                    dropDownTypes.Add(new DropDownMember("(Parsing project files)", new TextSpan(), -1, DROPDOWNFONTATTR.FONTATTR_GRAY)) |> ignore
                    dropDownMembers.Clear()
                    selectedType <- 0
                    selectedMember <- -1
                    true
                else
                    ensureNavigationItemsUpToDate () 
                    
                    // Test whether things have changed so that we don't update the dropdown every time
                    copyTo dropDownTypes navigationItems.Value.Declarations (fun decl -> decl.Declaration)    
                    let line = line + 1
                    let selLeft = findDeclaration navigationItems.Value.Declarations true (fun decl -> decl.Declaration) line col
                    selectedType <- selLeft
                    match selLeft with 
                    | n when n >= 0 -> 
                        copyTo dropDownMembers (navigationItems.Value.Declarations.[n].Nested) id
                        selectedMember <- findDeclaration navigationItems.Value.Declarations.[n].Nested true id line col
                    | _ -> 
                        selectedMember <- -1
                    true
            with e-> 
                Assert.Exception(e)
                reraise()        


    member x.GetHiddenRegions(file) =
            ensureNavigationItemsUpToDate()
            let current = parseResults.FileName
            match navigationItems with 
            | Some(res) when file = current ->
                res.Declarations 
                  |> Array.filter(fun decl -> not(decl.Declaration.IsSingleTopLevel))
                  |> Array.fold (fun (toCreate, toUpdate:Map<_,_>) decl ->
                    let declKey = decl.Declaration.UniqueName
                    let context = TextSpanOfRange decl.Declaration.BodyRange
                    match (Map.tryFind declKey displayedRegions) with
                    | Some(uniqueId) ->
                        // do not add if the region hasn't changed
                        (toCreate, toUpdate.Add(uniqueId, context))
                    | None ->
                        let id = regionGenerator()
                        let reg = 
                          new NewHiddenRegion
                            (iType = int HIDDEN_REGION_TYPE.hrtCollapsible, dwBehavior = uint32 HIDDEN_REGION_BEHAVIOR.hrbClientControlled,
                             dwState = uint32 HIDDEN_REGION_STATE.hrsExpanded, tsHiddenText = context, pszBanner = null, dwClient = id)
                        displayedRegions <- displayedRegions.Add(declKey, id)
                        (reg::toCreate, toUpdate)
                        ) ([], Map.empty)
            | _ -> 
                displayedRegions <- Map.empty
                [], Map.empty
        
    member x.ClearDisplayedRegions() =
            displayedRegions <- Map.empty
            
                   
    member x.UpdateHiddenRegions(source:ISource,textView) =

        let toCreate, toUpdate = x.GetHiddenRegions(FilePathUtilities.GetFilePath(VsTextView.Buffer textView))
        if not (toCreate = [] && toUpdate = Map.empty) then
            // Compare the existing regions with the new regions and 
            // remove any that do not match the new regions.
            let session = source.GetHiddenTextSession()
            let (aregion:IVsHiddenRegion[]) = Array.zeroCreate(1)
            
            // Get current regions from Visual Studio        
            let ppenum = Com.ThrowOnFailure1(session.EnumHiddenRegions(uint32 FIND_HIDDEN_REGION_FLAGS.FHR_ALL_REGIONS, 0u, Array.zeroCreate(1)))
            let regions = 
              seq { let fetched = ref 0u
                    while (ppenum.Next(1u, aregion, fetched) = VSConstants.S_OK && !fetched = 1u) do
                      yield aregion.[0] }
            
            for reg in regions do
                let unique = Com.ThrowOnFailure1(reg.GetClientData())
                match toUpdate.TryFind(unique) with
                | Some(span) -> reg.SetSpan( [| span |]) |> Com.ThrowOnFailure0
                | _ -> reg.Invalidate(uint32 CHANGE_HIDDEN_REGION_FLAGS.chrNonUndoable) |> Com.ThrowOnFailure0
                    
            // TODO: this is what MPF comment says...
            //    For very large documents this can take a while, so add them in chunks of 
            //    1000 and stop after 5 seconds. 
            
            if (toCreate.Length > 0) then
                let arr = toCreate |> Array.ofList
                let mutable (outEnum:IVsEnumHiddenRegions[]) = Array.zeroCreate(arr.Length)
                session.AddHiddenRegions(uint32 CHANGE_HIDDEN_REGION_FLAGS.chrNonUndoable, arr.Length, arr, outEnum) |> Com.ThrowOnFailure0
        

/// Implements the remainder of the logic from the MPF class TypeAndMemberDropdownBars
/// by forwarding requests for navigation info to the current FSharpNavigationAndRegionInfo.
type internal FSharpNavigationBars(svc:LanguageService, stateFunc:unit -> FSharpNavigationAndRegionInfo option) = 
    inherit TypeAndMemberDropdownBars(svc)        

    override x.OnSynchronizeDropdowns(_, textView, line, col, dropDownTypes, dropDownMembers, selectedType:int byref, selectedMember:int byref) =
            match stateFunc() with
            | Some scope -> 
                let file = FilePathUtilities.GetFilePath(VsTextView.Buffer textView)
                scope.SynchronizeNavigationDropDown(file, line, col, dropDownTypes, dropDownMembers, &selectedType, &selectedMember)
            | _ -> 
                dropDownTypes.Clear()
                dropDownTypes.Add(new DropDownMember("(Parsing project files)", new TextSpan(), -1, DROPDOWNFONTATTR.FONTATTR_GRAY)) |> ignore
                dropDownMembers.Clear()
                selectedType <- 0
                selectedMember <- -1
                true

/// Determines whether the navigation bar and regions are active features.
type internal FSharpNavigationController() =

    // App.config keys for determining whether not-shipping features are turned on or off
    // Edit devenv.exe.config and place these at the end.
    //   <appSettings>
    //      <add key="fsharp-regions-enabled" value="true" />
    //      <add key="fsharp-navigationbar-enabled" value="true" />
    //      <add key="fsharp-standalone-file-intellisense-enabled" value="false" />
    //   </appSettings>
    let enableNavBarKey = "fsharp-navigationbar-enabled"
    let enableRegionsKey = "fsharp-regions-enabled"

    // In case the config file is incorrect, we silently recover and disable the feature
    member val EnableRegions = 
        try "true" = System.Configuration.ConfigurationManager.AppSettings.[enableRegionsKey]
        with e ->  
            Debug.Assert (false, sprintf "Error while loading 'devenv.exe.config' configuration: %A" e)
            false
        
    // In case the config file is incorrect, we silently recover and disable the feature
    member val EnableNavBar = 
        try "true" = ConfigurationManager.AppSettings.[enableNavBarKey]
        with e -> 
            Debug.Assert (false, sprintf "Error while loading 'devenv.exe.config' configuration: %A" e)
            false
            
