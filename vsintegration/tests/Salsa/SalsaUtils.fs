// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Salsa

open System
open System.IO
open Microsoft.VisualStudio.FSharp.ProjectSystem
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.TextManager.Interop
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open NUnit.Framework

open Salsa.Salsa

/// Utilities related to VsOps
module internal VsOpsUtils =

    // ------------------------------------------------------------------------
    let opsOfProj (p : OpenProject) = p.VS.VsOps
    let opsOfFile (f : OpenFile) = f.VS.VsOps
    let DefaultBuildActionOfFilename(filename) = 
        match Path.GetExtension(filename) with 
        | ".fsx" -> BuildAction.None
        | ".resx"
        | ".resources" -> BuildAction.EmbeddedResource
        | _ -> BuildAction.Compile
        
    let CreateSolution(vs : VisualStudio) = vs.VsOps.CreateSolution(vs)
    let ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients(vs : VisualStudio) = vs.VsOps.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients(vs)
    let GetOutputWindowPaneLines(vs : VisualStudio) = vs.VsOps.GetOutputWindowPaneLines(vs)
    let CloseSolution(soln : OpenSolution)             = soln.VS.VsOps.CloseSolution(soln)

    let CreateProject(solution : OpenSolution, projectname) = 
        solution.VS.VsOps.CreateProject(solution,projectname)
    let NewFile(vs : VisualStudio, filename,lines) = 
        vs.VsOps.NewFile(vs,filename,DefaultBuildActionOfFilename filename,lines)
    let DeleteFileFromDisk(vs : VisualStudio, file) = 
        vs.VsOps.DeleteFileFromDisk(file)
    let AddFileFromText(project, filename, lines) = 
        (opsOfProj project).AddFileFromText(project,filename,filename,DefaultBuildActionOfFilename filename,lines)
    let AddFileFromTextBlob(project : OpenProject, filename, lines : string)  = 
        (opsOfProj project).AddFileFromText(project,filename,filename,DefaultBuildActionOfFilename filename, Array.toList (lines.Split( [| "\r\n" |], StringSplitOptions.None)))
    let AddFileFromTextEx(project : OpenProject,filenameOnDisk,filenameInProject,buildAction,lines)  = 
        (opsOfProj project).AddFileFromText(project,filenameOnDisk,filenameInProject,buildAction,lines)
    let AddLinkedFileFromTextEx(project : OpenProject,filenameOnDisk,includeFilenameInProject,linkFilenameInProject,lines) = 
        (opsOfProj project).AddLinkedFileFromText(project,filenameOnDisk,includeFilenameInProject,linkFilenameInProject,DefaultBuildActionOfFilename filenameOnDisk,lines)
    let AddAssemblyReference(project : OpenProject,reference) = 
        (opsOfProj project).AddAssemblyReference(project,reference,false)
    let AddAssemblyReferenceEx(project : OpenProject,reference,specificVersion) = 
        (opsOfProj project).AddAssemblyReference(project,reference,specificVersion)
    let AddProjectReference(project1,project2) = 
        (opsOfProj project1).AddProjectReference(project1,project2)
    let PlaceIntoProjectFileBeforeImport(project,xml)   = 
        (opsOfProj project).PlaceIntoProjectFileBeforeImport(project,xml)
    let ProjectDirectory(project) = 
        (opsOfProj project).ProjectDirectory(project)
    let ProjectFile(project) = 
        (opsOfProj project).ProjectFile(project)
    let SetVersionFile(project,file)    = 
        (opsOfProj project).SetVersionFile(project,file)
    let SetConfigurationAndPlatform(project,configAndPlatform) = 
        (opsOfProj project).SetConfigurationAndPlatform(project,configAndPlatform)
    let SetOtherFlags(project,flags)    = 
        (opsOfProj project).SetOtherFlags(project,flags)
    let GetErrors(project) = 
        (opsOfProj project).GetErrors(project)
    let SetProjectDefines(project,defines) = 
        (opsOfProj project).SetProjectDefines(project,defines)
    let AddDisabledWarning(project,code) = 
        (opsOfProj project).AddDisabledWarning(project,code)
    let Build(project) = 
        (opsOfProj project).BuildProject(project,null)
    let BuildTarget(project,target) = 
        (opsOfProj project).BuildProject(project,target)
    let GetMainOutputAssembly(project)  = 
        (opsOfProj project).GetMainOutputAssembly(project)
    let Save(project) = 
        (opsOfProj project).SaveProject(project)
    let OpenFileViaOpenFile(vs : VisualStudio, filename) = 
        vs.VsOps.OpenFileViaOpenFile(vs,filename)
    let OpenFile(project,filename) = 
        (opsOfProj project).OpenFile(project,filename)
    let GetOpenFiles(project) = 
        (opsOfProj project).GetOpenFiles(project)
    let OpenExistingProject(vs :VisualStudio, dir,projname) = 
        vs.VsOps.OpenExistingProject(vs,dir,projname)
    let MoveCursorTo(file,line,col) = 
        (opsOfFile file).MoveCursorTo(file,line,col)
    let GetCursorLocation(file) = 
        (opsOfFile file).GetCursorLocation(file)
    let MoveCursorToEndOfMarker(file,marker) = 
        (opsOfFile file).MoveCursorToEndOfMarker(file, marker)
    let GetMatchingBracesForPositionAtCursor(file) = 
        (opsOfFile file).GetMatchingBracesForPositionAtCursor(file)
    let MoveCursorToStartOfMarker(file,marker) = 
        (opsOfFile file).MoveCursorToStartOfMarker(file,marker)
    let GetQuickInfoAtCursor(file) = 
        (opsOfFile file).GetQuickInfoAtCursor(file)
    let GetQuickInfoAndSpanAtCursor(file) = 
        (opsOfFile file).GetQuickInfoAndSpanAtCursor(file)
    let GetNameOfOpenFile(file) = 
        (opsOfFile file).GetNameOfOpenFile(file)
    let GetProjectOptionsOfScript(file) = 
        (opsOfFile file).GetProjectOptionsOfScript(file)
    let GetParameterInfoAtCursor(file) = 
        (opsOfFile file).GetParameterInfoAtCursor(file)
    let GetTokenTypeAtCursor(file) = 
        (opsOfFile file).GetTokenTypeAtCursor(file)
    let GetSquiggleAtCursor(file) = 
        (opsOfFile file).GetSquiggleAtCursor(file)
    let GetSquigglesAtCursor(file) = 
        (opsOfFile file).GetSquigglesAtCursor(file)
    let AutoCompleteAtCursor(file) = 
        (opsOfFile file).AutoCompleteAtCursor(file)
    let CtrlSpaceCompleteAtCursor(file) = 
        (opsOfFile file).CompleteAtCursorForReason(file,Microsoft.VisualStudio.FSharp.LanguageService.BackgroundRequestReason.CompleteWord)
    let CompleteAtCursorForReason(file,reason) = 
        (opsOfFile file).CompleteAtCursorForReason(file,reason)
    let CompletionBestMatchAtCursorFor(file, value, filterText) = 
        (opsOfFile file).CompletionBestMatchAtCursorFor(file, value, filterText)
    let GotoDefinitionAtCursor file  = 
        (opsOfFile file).GotoDefinitionAtCursor (file, false)
    let GotoDefinitionAtCursorForceGeneration file = 
        (opsOfFile file).GotoDefinitionAtCursor (file, true)
    let GetIdentifierAtCursor file = 
        (opsOfFile file).GetIdentifierAtCursor file
    let GetF1KeywordAtCursor file = 
        (opsOfFile file).GetF1KeywordAtCursor file
    let GetLineNumber file n = 
        (opsOfFile file).GetLineNumber (file, n)
    let GetAllLines file= 
        (opsOfFile file).GetAllLines file
    let SwitchToFile (vs : VisualStudio) file = 
        vs.VsOps.SwitchToFile(vs,file)
    let OnIdle(vs : VisualStudio) = vs.VsOps.OnIdle(vs)
    let ShiftKeyDown(vs : VisualStudio) = vs.VsOps.ShiftKeyDown(vs)
    let ShiftKeyUp(vs : VisualStudio) = vs.VsOps.ShiftKeyUp(vs) 
    let TakeCoffeeBreak(vs : VisualStudio) = vs.VsOps.TakeCoffeeBreak(vs)
    let ReplaceFileInMemory(file :OpenFile) lines = (opsOfFile file).ReplaceFileInMemory(file,lines,true)
    let ReplaceFileInMemoryWithoutCoffeeBreak(file :OpenFile) lines   = (opsOfFile file).ReplaceFileInMemory(file,lines,false)
    let SaveFileToDisk(file :OpenFile)  = (opsOfFile file).SaveFileToDisk(file)
    let AutoCompleteMemberDataTipsThrowsScope(vs : VisualStudio, message)  = vs.VsOps.AutoCompleteMemberDataTipsThrowsScope(message)
    let Cleanup(vs : VisualStudio) = vs.VsOps.CleanUp(vs) 

    let OutOfConeFilesAreAddedAsLinks(vs : VisualStudio) = vs.VsOps.OutOfConeFilesAreAddedAsLinks
    let SupportsOutputWindowPane(vs : VisualStudio) = vs.VsOps.SupportsOutputWindowPane


    // ------------------------------------------------------------------------

    type SetMarkerPoint =
        | StartOfMarker
        | EndOfMarker

    /// Creates a single file project/solution
    let CreateSingleFileProject (vs, fileContents) =
        let solution = CreateSolution(vs)
        let project = CreateProject(solution, "testproject")
        let _ = AddFileFromTextBlob(project, "File1.fs", fileContents)
        let file = OpenFile(project, "File1.fs")
        (solution, project, file)

    /// Creates a single file project/solution where the lone file is named.
    let CreateNamedSingleFileProject (vs, (fileContents, fileName)) =
        let solution = CreateSolution(vs)
        let project = CreateProject(solution, "testproject")
        let _ = AddFileFromTextBlob(project, fileName, fileContents)
        let file = OpenFile(project, fileName)
        (solution, project, file)

    // ------------------------------------------------------------------------

    /// Verify that items obtained from the navigation bar contain the specified item
    let AssertRegionListContains(expected:list<(int*int)*(int*int)>, regions:list<NewHiddenRegion>) =
      for (sl,sc), (el,ec) in expected do 
        match regions |> List.tryFind (fun reg -> 
            let span = reg.tsHiddenText
            (span.iStartIndex = sc) && (span.iEndIndex = ec) && 
              (span.iStartLine = sl) && (span.iEndLine = el) ) with
        | None -> 
            printfn "Regions found: %A" (regions |> List.map (fun itm -> 
              ((itm.tsHiddenText.iStartIndex, itm.tsHiddenText.iStartLine),
               (itm.tsHiddenText.iEndIndex, itm.tsHiddenText.iEndLine)) ))
            Assert.Fail(sprintf "Couldn't find region (%d, %d) - (%d, %d)" sl sc el ec)
        | _ -> ()
      
    /// Verify that items obtained from the navigation bar contain the specified item
    let AssertNavigationContains (items:DropDownMember[], expected) =
      match items |> Array.tryFind (fun itm -> itm.Label = expected) with
      | None -> 
          printfn "Navigation bar items: %A" (items |> Array.map (fun itm -> itm.Label))
          Assert.Fail(sprintf "Couldn't find '%s' in drop down bar." expected)
      | _ -> ()

    /// Verify that items obtained from the navigation bar contain all specified item
    let AssertNavigationContainsAll (items:DropDownMember[], allExpected) =
      for expected in allExpected do
        match items |> Array.tryFind (fun itm -> itm.Label = expected) with
        | None -> 
            printfn "Navigation bar items: %A" (items |> Array.map (fun itm -> itm.Label))
            Assert.Fail(sprintf "Couldn't find '%s' in drop down bar." expected)
        | _ -> ()
    
    // ------------------------------------------------------------------------
    
    /// Verify the completion list is empty, typically for negative tests
    let AssertCompListIsEmpty (completions : CompletionItem[]) = 
      if not (Array.isEmpty completions) then
          printfn "Expected empty completion list but got: %A" (completions |> Array.map (fun (CompletionItem(nm, _, _, _, _)) -> nm))
      Assert.IsTrue(Array.isEmpty completions, "Expected empty completion list but got some items")

    /// Verify that the given completion list contains a member with the given name
    let AssertCompListContains(completions : CompletionItem[], membername) =
        let found = completions |> Array.filter(fun (CompletionItem(name,_,_,_,_)) -> name = membername) |> Array.length
        if found = 0 then
            printfn "Failed to find expected value %s in " membername
            let MAX = 25
            printfn "Completion list = %s" (if completions.Length > MAX then sprintf "%A ... and more" completions.[0..MAX] else sprintf "%A" completions)
            Assert.Fail(sprintf "Couldn't find '%s' in completion list: %+A" membername (completions |> Array.map (fun (CompletionItem(name,_,_,_,_)) -> name)))

    /// Verify the completion list does not contain a member with the given name
    let AssertCompListDoesNotContain(completions : CompletionItem[], membername) =
        let found = completions |> Array.filter(fun (CompletionItem(name,_,_,_,_)) -> name = membername) |> Array.length
        if found <> 0 then
            printfn "Value %s should have been absent from " membername
            printfn "Completion list = %A" completions
            Assert.Fail(sprintf "Found unexpected '%s' in completion list" membername)
                         
    // Verify the completion list contains every member in the list
    let rec AssertCompListContainsAll(completions : CompletionItem[], expectedCompletions) =
        match expectedCompletions with
        | [] -> ()
        | h :: t ->
            AssertCompListContains(completions, h)
            AssertCompListContainsAll(completions, t)
            ()

    // Verify the completion list contains every member in the list
    let rec AssertCompListContainsExactly(completions : CompletionItem[], expectedCompletions) =
        AssertCompListContainsAll(completions, expectedCompletions)
        if (completions.Length <> (expectedCompletions |> List.length)) then
            printfn "Completion list contained all the expected completions, but there were additional unexpected completions."
            printfn "Expected = %A" expectedCompletions
            printfn "Actual = %A" completions
            Assert.Fail("Extra completions found in list")

    /// Verify the completion list does not contain any member in the list
    let rec AssertCompListDoesNotContainAny(completions : CompletionItem[], itemsNotInCompList) =
        match itemsNotInCompList with
        | [] -> ()
        | h :: t ->
            AssertCompListDoesNotContain(completions, h)
            AssertCompListDoesNotContainAny(completions, t)
            ()

    /// Simulates pressing '.' at the mark and returns the completion list
    let DotCompletionAtMarker markerDirection (file : OpenFile) marker =
        
        // Simulate pressing '.'
        let orgFileContents = GetAllLines file
        
        // Check that the marker is unique, otherwise we can't determine where to put the '.'
        let markerLines = orgFileContents |> Seq.filter (fun line -> line.Contains(marker)) |> Seq.length 
        if markerLines = 0 then Assert.Fail("Unable to find marker in source code.")
        if markerLines > 1 then Assert.Fail <| sprintf "Found marker [%s] multiple times in source file." marker
        
        // Replace marker with "<marker>."
        let replaceMarker =
            match markerDirection with 
            | StartOfMarker -> (fun (line : string) -> line.Replace(marker, "." + marker))
            | EndOfMarker   -> (fun (line : string) -> line.Replace(marker, marker + "."))
        
        let newFileContents = orgFileContents |> List.map replaceMarker
        
        // Now apply our change & get the comp list
        ReplaceFileInMemory file newFileContents
        
        match markerDirection with 
        | StartOfMarker -> MoveCursorToStartOfMarker(file, marker)
        | EndOfMarker   -> MoveCursorToEndOfMarker(file, marker + ".")

        let compList = AutoCompleteAtCursor(file)
       
        // Now restore the origional file contents
        ReplaceFileInMemory file orgFileContents
        
        compList
        
    /// Gets the completion list as if you pressed '.' at the START of the marker.
    let DotCompletionAtStartOfMarker : (OpenFile -> string -> CompletionItem[]) = DotCompletionAtMarker StartOfMarker

    /// Gets the completion list as if you pressed '.' at the END of the marker.
    let DotCompletionAtEndOfMarker   : (OpenFile -> string -> CompletionItem[]) = DotCompletionAtMarker EndOfMarker

    // ------------------------------------------------------------------------
 
    /// Abbreviation for 'None', to indiciate a GotoDefn failure
    let GotoDefnFailure       = None : (string * string) option
    /// Abbreviation for 'Some(ident, lineOfCode)'
    let GotoDefnSuccess x y = Some (x, y) : (string * string) option
    
    /// Checks that a goto definition result matches the expected value.
    /// Expected = Some(identifierAtCursor, lineOfCodeAtCursor)
    let CheckGotoDefnResult (expected : (string * string) option) (file : OpenFile) (actual : GotoDefnResult) : unit =
        match (expected, actual.ToOption()) with
        // Success cases
        // GotoDefn retrieved a result and we expected to find something
        | (Some (toFind, expLine), Some (span, actFile)) 
            ->  match GetIdentifierAtCursor file with
                | None         ->   Assert.Fail ("No identifier at cursor. This indicates a bug in GotoDefinition.")
                | Some (id, _) ->   // Are we on the identifier we expect?
                                    Assert.AreEqual (toFind, id)
                                    // Do the lines of code match what we expect?
                                    // - Eliminate white space to eliminate trivial errors
                                    // - +1 to adjust for 1-index line numbers
                                    Assert.AreEqual (
                                        expLine.Trim(), 
                                        (span.iStartLine |> (+) 1 |> GetLineNumber (OpenFileViaOpenFile(file.VS, actFile))).Trim ()
                                    ) 
                                    // Looks like it's legit!
                                    ()
        // We expected Goto Definition to fail and it did. 
        // (Such as Goto Definition on keyword or symbol.)
        | (None, None) 
            -> ()
        
        // Error cases
        | (Some (x,_), None)     
            -> Assert.Fail <| sprintf "Expected to find the definition of '%s' but GotoDefn failed." x

        | (None, Some (_,file)) 
            -> Assert.Fail <| sprintf "Expected GotoDefn to fail, but it went to a definition in file %s" file

