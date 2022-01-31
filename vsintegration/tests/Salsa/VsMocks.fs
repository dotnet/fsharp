// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

(*
    Mocks of major Visual Studio interfaces.
*)

namespace Salsa

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.FSharp.ProjectSystem
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.OLE.Interop
open System.Diagnostics
open Microsoft.Build.Execution
open Microsoft.Build.Framework
open System.Runtime.InteropServices
open System
open System.Collections.Generic

module internal VsMocks = 
    let private notimpl() = raise (new System.Exception("Not implemented"))
    let private ok = VSConstants.S_OK
    let private fail = VSConstants.E_FAIL
    let private impl1 fo next a =
        match fo with 
            Some(f) -> 
                match f a with | Some(r) -> r | _ -> next a
            | _ -> next a
    let private impl2 fo next a b =
        match fo with 
            Some(f) -> 
                match f a b with | Some(r) -> r | _ -> next a b
            | _ -> next a b
    let private impl3 fo next a b c=
        match fo with 
            Some(f) -> 
                match f a b c with | Some(r) -> r | _ -> next a b c
            | _ -> next a b c
    let private impl4 fo next a b c d=
        match fo with 
            Some(f) -> 
                match f a b c d with | Some(r) -> r | _ -> next a b c d
            | _ -> next a b c d
    let private uimpl fo next = 
        match fo with 
                Some(f) -> 
                    match f() with 
                        Some(r)->r
                      | None -> next()
               | _ -> next()

    type IDelegable<'a> = interface
            abstract SetInner: 'a->unit
            abstract GetInner: unit->'a
        end

    type VSITEMID=int32        


    type VsFileChangeEx() = 
        let fileToEvents = new Dictionary<string,IVsFileChangeEvents list>()
        let Canonicalize (filename:string) = System.IO.Path.GetFullPath(filename)
        
        member c.AddedFile(file) =
//            printfn "VsMocks.VsFileChangeEx: Added file %s " file
            c.SomehowModifiedFile(file, uint32 _VSFILECHANGEFLAGS.VSFILECHG_Add)
        member c.DeletedFile(file) =
//            printfn "VsMocks.VsFileChangeEx: Deleted file %s " file
            c.SomehowModifiedFile(file, uint32 _VSFILECHANGEFLAGS.VSFILECHG_Del)
        member c.ChangedFile(file) =
//            printfn "VsMocks.VsFileChangeEx: Changed file %s " file
            c.SomehowModifiedFile(file, uint32 _VSFILECHANGEFLAGS.VSFILECHG_Time)
        member c.SomehowModifiedFile(file, how) =
            let file = Canonicalize file
            let CallFilesChanged(fce:IVsFileChangeEvents) =
//                printfn "VsMocks.VsFileChangeEx: Calling FilesChanged callback for %s, how = %A " file how
                fce.FilesChanged(1u,[|file|],[|how|])|>ignore
            
            match fileToEvents.TryGetValue(file) with
            | true, events -> events |> List.iter CallFilesChanged
            | false, _ -> ()
            
        interface IVsFileChangeEx with
            member fc.AdviseFileChange(pszMkDocument,grfFilter,pFCE,vsCookie) = 
                let pszMkDocument = Canonicalize pszMkDocument
//                printfn "VsMocks.VsFileChangeEx: Advise %s " pszMkDocument
                match fileToEvents.TryGetValue(pszMkDocument) with
                | true, events -> fileToEvents.[pszMkDocument] <- pFCE :: events
                | false, _ -> fileToEvents.Add(pszMkDocument, [pFCE])
                vsCookie <- 0u
                ok
            member fc.UnadviseFileChange(vsCookie) = 
//                printfn "VsMocks.VsFileChangeEx: Unadvise %d " vsCookie
                ok
            member fc.SyncFile(pszMkDocument) = notimpl()
            member fc.IgnoreFile(vsCookie, pszMkDocument, fIgnore) = notimpl()
            member fc.AdviseDirChange(pszDir, fWatchSubDir, pFCE,vsCookie) = notimpl()
            member fc.UnadviseDirChange(vsCookie) = notimpl()
    
    /// Mockable versions of various VS interfaces. Use optional function parameters
    /// so that we don't have to specify all of them.       
    type Vs() = 
        static let FSharpProjectGuid = new Guid(Microsoft.VisualStudio.FSharp.ProjectSystem.GuidList.guidFSharpProjectFactoryString)
        
        static member MakeTextView() = 
            let userData = new Dictionary<Guid,obj>()
            {new IVsTextView with
                member tv.Initialize(pBuffer, hwndParent, initFlags, pInitView) = notimpl()
                member tv.CloseView() = notimpl()
                member tv.GetCaretPos(piLine, piColumn) = notimpl()
                member tv.SetCaretPos(iLine, iColumn) = notimpl()
                member tv.GetSelectionSpan(pSpan) = notimpl()
                member tv.GetSelection(piAnchorLine, piAnchorCol, piEndLine,  piEndCol) = notimpl()
                member tv.SetSelection(iAnchorLine, iAnchorCol, iEndLine, iEndCol) = notimpl()
                member tv.GetSelectionMode() = notimpl()
                member tv.SetSelectionMode(iSelMode) = notimpl()
                member tv.ClearSelection(fMoveToAnchor) = notimpl()
                member tv.CenterLines(iTopLine, iCount) = notimpl()
                member tv.GetSelectedText(pbstrText) = notimpl()
                member tv.GetSelectionDataObject(ppIDataObject) = notimpl()
                member tv.GetTextStream(iTopLine, iTopCol, iBottomLine, iBottomCol, pbstrText) = notimpl()
                member tv.SetBuffer(pBuffer) = notimpl()
                member tv.GetWindowHandle() = notimpl()
                member tv.GetScrollInfo(iBar, piMinUnit, piMaxUnit, piVisibleUnits, piFirstVisibleUnit) = notimpl()
                member tv.SetScrollPosition(iBar, iFirstVisibleUnit) = notimpl()
                member tv.AddCommandFilter(pNewCmdTarg, ppNextCmdTarg) = notimpl()
                member tv.RemoveCommandFilter(pCmdTarg) = notimpl()
                member tv.UpdateCompletionStatus(pCompSet, dwFlags) = notimpl()
                member tv.UpdateTipWindow(pTipWindow, dwFlags) = notimpl()
                member tv.GetWordExtent(iLine, iCol, dwFlags, pSpan) = notimpl()
                member tv.RestrictViewRange(iMinLine, iMaxLine,   pClient) = notimpl()
                member tv.ReplaceTextOnLine(iLine, iStartCol, iCharsToReplace, pszNewText, iNewLen) = notimpl()
                member tv.GetLineAndColumn(iPos, piLine, piIndex) = notimpl()
                member tv.GetNearestPosition(iLine, iCol, piPos, piVirtualSpaces) = notimpl()
                member tv.UpdateViewFrameCaption() = notimpl()
                member tv.CenterColumns(iLine, iLeftCol, iColCount) = notimpl()
                member tv.EnsureSpanVisible(span) = notimpl()
                member tv.PositionCaretForEditing(iLine, cIndentLevels) = notimpl()
                member tv.GetPointOfLineColumn(iLine, iCol, ppt) = notimpl()
                member tv.GetLineHeight(piLineHeight) = notimpl()
                member tv.HighlightMatchingBrace(dwFlags, cSpans, rgBaseSpans) = notimpl()
                member tv.SendExplicitFocus() = notimpl()
                member tv.SetTopLine(iBaseLine) = notimpl()
                member tv.GetBuffer(ppbuffer) = notimpl()
            interface IVsLayeredTextView with
                member ltv.GetSelectedAtom(dwFlags, ppunkAtom) = notimpl()
                member ltv.GetRelativeSelectionState(dwFlags, pReferenceLayer, pSelState) = notimpl()
                member ltv.SetRelativeSelectionState(dwFlags, pReferenceLayer, pSelState) = notimpl()
                member ltv.GetTopmostLayer(ppLayer) = notimpl()
            interface IVsUserData with
                member x.SetData(guid,obj) =
                    if userData.ContainsKey(guid) then userData.Remove(guid) |> ignore
                    match obj with
                    |   null -> ()
                    |   _   -> userData.Add(guid,obj) |> ignore
                    VSConstants.S_OK
                member x.GetData(guid,obj) =
                    if userData.ContainsKey(guid) then 
                        obj <- userData.[guid]
                    else
                        obj <- null
                    VSConstants.S_OK
            }
        
        
        static member VsUserContext(?addAttribute) =
            { new IVsUserContext with
                member this.AddAttribute (usage,key,value) =
                    match addAttribute with
                    |   Some f -> f (usage,key,value)
                    |   None -> notimpl()
                member this.AddSubcontext(_,_,_) = notimpl()
                member this.AdviseUpdate(_,_) = notimpl()
                member this.CountAttributes(_,_,_) = notimpl()
                member this.RemoveAttribute(_,_) = notimpl()
                member this.RemoveSubcontext _ = notimpl()
                member this.GetAttribute(_,_,_,_,_) = notimpl()
                member this.CountSubcontexts _ = notimpl()
                member this.GetSubcontext(_,_) = notimpl()
                member this.IsDirty _ = notimpl()
                member this.SetDirty _ = notimpl()
                member this.Update () = notimpl()
                member this.UnadviseUpdate _ = notimpl()
                member this.GetAttrUsage(_,_,_) = notimpl()
                member this.RemoveAllSubcontext () = notimpl()
                member this.GetPriority _ = notimpl()
                member this.RemoveAttributeIncludeChildren(_,_) = notimpl()
                member this.GetAttributePri(_,_,_,_,_,_) = notimpl()
            }
        
        static member DelegateTextView(oldtv:IVsTextView, ?getBuffer, ?getScrollInfo, ?getTopmostLayer) = 
            let inner:Ref<IVsTextView> = ref oldtv
            {new IVsTextView with
                member tv.Initialize(pBuffer, hwndParent, initFlags, pInitView) = (!inner).Initialize(pBuffer, hwndParent, initFlags, pInitView)
                member tv.CloseView() = (!inner).CloseView()
                member tv.GetCaretPos(piLine, piColumn) = (!inner).GetCaretPos(ref piLine, ref piColumn)
                member tv.SetCaretPos(iLine, iColumn) = (!inner).SetCaretPos(iLine, iColumn)
                member tv.GetSelectionSpan(pSpan) = (!inner).GetSelectionSpan(pSpan)
                member tv.GetSelection(piAnchorLine, piAnchorCol, piEndLine,  piEndCol) = (!inner).GetSelection(ref piAnchorLine, ref piAnchorCol, ref piEndLine,  ref piEndCol)
                member tv.SetSelection(iAnchorLine, iAnchorCol, iEndLine, iEndCol) = (!inner).SetSelection(iAnchorLine, iAnchorCol, iEndLine, iEndCol)
                member tv.GetSelectionMode() = (!inner).GetSelectionMode()
                member tv.SetSelectionMode(iSelMode) = (!inner).SetSelectionMode(iSelMode)
                member tv.ClearSelection(fMoveToAnchor) = (!inner).ClearSelection(fMoveToAnchor)
                member tv.CenterLines(iTopLine, iCount) = (!inner).CenterLines(iTopLine, iCount)
                member tv.GetSelectedText(pbstrText) = (!inner).GetSelectedText(ref pbstrText)
                member tv.GetSelectionDataObject(ppIDataObject) = (!inner).GetSelectionDataObject(ref ppIDataObject)
                member tv.GetTextStream(iTopLine, iTopCol, iBottomLine, iBottomCol, pbstrText) = (!inner).GetTextStream(iTopLine, iTopCol, iBottomLine, iBottomCol, ref pbstrText)
                member tv.SetBuffer(pBuffer) = (!inner).SetBuffer(pBuffer)
                member tv.GetWindowHandle() = (!inner).GetWindowHandle()
                member tv.GetScrollInfo(iBar, piMinUnit, piMaxUnit, piVisibleUnits, piFirstVisibleUnit) = 
                    let next bar = (!inner).GetScrollInfo(bar)
                    let hr,minu,maxu,visu,firstvis= impl1 getScrollInfo next iBar
                    piMinUnit<-minu
                    piMaxUnit<-maxu
                    piVisibleUnits<-visu
                    piFirstVisibleUnit<-firstvis
                    hr
                member tv.SetScrollPosition(iBar, iFirstVisibleUnit) = (!inner).SetScrollPosition(iBar, iFirstVisibleUnit)
                member tv.AddCommandFilter(pNewCmdTarg, ppNextCmdTarg) = (!inner).AddCommandFilter(pNewCmdTarg, ref ppNextCmdTarg)
                member tv.RemoveCommandFilter(pCmdTarg) = (!inner).RemoveCommandFilter(pCmdTarg)
                member tv.UpdateCompletionStatus(pCompSet, dwFlags) = (!inner).UpdateCompletionStatus(pCompSet, dwFlags)
                member tv.UpdateTipWindow(pTipWindow, dwFlags) = (!inner).UpdateTipWindow(pTipWindow, dwFlags)
                member tv.GetWordExtent(iLine, iCol, dwFlags, pSpan) = (!inner).GetWordExtent(iLine, iCol, dwFlags, pSpan)
                member tv.RestrictViewRange(iMinLine, iMaxLine,   pClient) = (!inner).RestrictViewRange(iMinLine, iMaxLine,   pClient)
                member tv.ReplaceTextOnLine(iLine, iStartCol, iCharsToReplace, pszNewText, iNewLen) = (!inner).ReplaceTextOnLine(iLine, iStartCol, iCharsToReplace, pszNewText, iNewLen)
                member tv.GetLineAndColumn(iPos, piLine, piIndex) = (!inner).GetLineAndColumn(iPos, ref piLine, ref piIndex)
                member tv.GetNearestPosition(iLine, iCol, piPos, piVirtualSpaces) = (!inner).GetNearestPosition(iLine, iCol, ref piPos, ref piVirtualSpaces)
                member tv.UpdateViewFrameCaption() = (!inner).UpdateViewFrameCaption()
                member tv.CenterColumns(iLine, iLeftCol, iColCount) = (!inner).CenterColumns(iLine, iLeftCol, iColCount)
                member tv.EnsureSpanVisible(span) = (!inner).EnsureSpanVisible(span)
                member tv.PositionCaretForEditing(iLine, cIndentLevels) = (!inner).PositionCaretForEditing(iLine, cIndentLevels)
                member tv.GetPointOfLineColumn(iLine, iCol, ppt) = (!inner).GetPointOfLineColumn(iLine, iCol, ppt)
                member tv.GetLineHeight(piLineHeight) = (!inner).GetLineHeight(ref piLineHeight)
                member tv.HighlightMatchingBrace(dwFlags, cSpans, rgBaseSpans) = (!inner).HighlightMatchingBrace(dwFlags, cSpans, rgBaseSpans)
                member tv.SendExplicitFocus() = (!inner).SendExplicitFocus()
                member tv.SetTopLine(iBaseLine) = (!inner).SetTopLine(iBaseLine)
                member tv.GetBuffer(ppbuffer) = 
                    let next() = (!inner).GetBuffer()
                    let hr,buf = uimpl getBuffer next
                    ppbuffer<-buf
                    hr
            interface IVsLayeredTextView with
                member ltv.GetSelectedAtom(dwFlags, ppunkAtom) = notimpl()
                member ltv.GetRelativeSelectionState(dwFlags, pReferenceLayer, pSelState) = notimpl()
                member ltv.SetRelativeSelectionState(dwFlags, pReferenceLayer, pSelState) = notimpl()
                member ltv.GetTopmostLayer(ppLayer) = 
                    let next() = ((box (!inner)):?>IVsLayeredTextView).GetTopmostLayer()
                    let hr,l = uimpl getTopmostLayer next
                    ppLayer<-l
                    hr
            interface IDelegable<IVsTextView> with
                member id.GetInner() = !inner
                member id.SetInner(i) = inner:=i
            interface IVsUserData with
                member ud.GetData(guid,o) = ((!inner) :?> IVsUserData).GetData(&guid,&o)
                member ud.SetData(guid,o) = ((!inner) :?> IVsUserData).SetData(&guid,o)
             }                
            
        static member MakeTextLayer() =
           {new IVsTextLayer with
                member tl.LocalLineIndexToBase(iLocalLine, iLocalIndex, piBaseLine, piBaseIndex) = notimpl()
                member tl.BaseLineIndexToLocal(iBaseLine, iBaseIndex, piLocalLine, piLocalIndex) = notimpl()
                member tl.LocalLineIndexToDeeperLayer(pTargetLayer, iLocalLine, iLocalIndex, piTargetLine, piTargetIndex) = notimpl()
                member tl.DeeperLayerLineIndexToLocal(dwFlags, pTargetLayer, iLayerLine, iLayerIndex, piLocalLine, piLocalIndex) = notimpl()
                member tl.GetBaseBuffer(ppiBuf) = notimpl()
                member tl.LockBufferEx(dwFlags) = notimpl()
                member tl.UnlockBufferEx(dwFlags) = notimpl()
                member tl.GetLengthOfLine(iLine, piLength) = notimpl()
                member tl.GetLineCount(piLineCount) = notimpl()
                member tl.GetLastLineIndex(piLine, piIndex) = notimpl()
                member tl.GetMarkerData(iTopLine, iBottomLine, pMarkerData) = notimpl()
                member tl.ReleaseMarkerData(pMarkerData) = notimpl()
                member tl.GetLineDataEx(dwFlags, iLine, iStartIndex, iEndIndex, pLineData, pMarkerData) = notimpl()
                member tl.ReleaseLineDataEx(pLineData) = notimpl()
                member tl.GetLineText(iStartLine, iStartIndex, iEndLine, iEndIndex, pbstrBuf) = notimpl()
                member tl.CopyLineText(iStartLine, iStartIndex, iEndLine, iEndIndex, pszBuf, pcchBuf) = notimpl()
                member tl.ReplaceLines(iStartLine, iStartIndex, iEndLine, iEndIndex, pszText, iNewLen, pChangedSpan) = notimpl()
                member tl.CanReplaceLines(iStartLine, iStartIndex, iEndLine, iEndIndex, iNewLen) = notimpl()
                member tl.CreateTrackingPoint(iLine, iIndex, ppMarker) = notimpl()
                member tl.EnumLayerMarkers(iStartLine, iStartIndex, iEndLine, iEndIndex, iMarkerType, dwFlags, ppEnum) = notimpl()
                member tl.ReplaceLinesEx(dwFlags, iStartLine, iStartIndex, iEndLine, iEndIndex, pszText, iNewLen, pChangedSpan) = notimpl()
                member tl.MapLocalSpansToTextOriginatingLayer(dwFlags, pLocalSpanEnum, ppTargetLayer, ppTargetSpanEnum) = notimpl()
           }
        static member DelegateTextLayer(oldtl:IVsTextLayer, ?localLineIndexToBase) = 
            let inner = ref oldtl
            {new IVsTextLayer with
                member tl.LocalLineIndexToBase(iLocalLine, iLocalIndex, piBaseLine, piBaseIndex) = 
                    let next ll li = (!inner).LocalLineIndexToBase(ll,li)
                    let hr,bl,bi = impl2 localLineIndexToBase next iLocalLine iLocalIndex
                    piBaseLine<-bl
                    piBaseIndex<-bi
                    hr
                member tl.BaseLineIndexToLocal(iBaseLine, iBaseIndex, piLocalLine, piLocalIndex) = notimpl()
                member tl.LocalLineIndexToDeeperLayer(pTargetLayer, iLocalLine, iLocalIndex, piTargetLine, piTargetIndex) = notimpl() 
                member tl.DeeperLayerLineIndexToLocal(dwFlags, pTargetLayer, iLayerLine, iLayerIndex, piLocalLine, piLocalIndex) = notimpl()
                member tl.GetBaseBuffer(ppiBuf) = notimpl()
                member tl.LockBufferEx(dwFlags) = notimpl()
                member tl.UnlockBufferEx(dwFlags) = notimpl()
                member tl.GetLengthOfLine(iLine, piLength) = notimpl()
                member tl.GetLineCount(piLineCount) = notimpl()
                member tl.GetLastLineIndex(piLine, piIndex) = notimpl()
                member tl.GetMarkerData(iTopLine, iBottomLine, pMarkerData) = notimpl()
                member tl.ReleaseMarkerData(pMarkerData) = notimpl()
                member tl.GetLineDataEx(dwFlags, iLine, iStartIndex, iEndIndex, pLineData, pMarkerData) = notimpl()
                member tl.ReleaseLineDataEx(pLineData) = notimpl()
                member tl.GetLineText(iStartLine, iStartIndex, iEndLine, iEndIndex, pbstrBuf) = notimpl()
                member tl.CopyLineText(iStartLine, iStartIndex, iEndLine, iEndIndex, pszBuf, pcchBuf) = notimpl()
                member tl.ReplaceLines(iStartLine, iStartIndex, iEndLine, iEndIndex, pszText, iNewLen, pChangedSpan) = notimpl()
                member tl.CanReplaceLines(iStartLine, iStartIndex, iEndLine, iEndIndex, iNewLen) = notimpl()
                member tl.CreateTrackingPoint(iLine, iIndex, ppMarker) = notimpl()
                member tl.EnumLayerMarkers(iStartLine, iStartIndex, iEndLine, iEndIndex, iMarkerType, dwFlags, ppEnum) = notimpl()
                member tl.ReplaceLinesEx(dwFlags, iStartLine, iStartIndex, iEndLine, iEndIndex, pszText, iNewLen, pChangedSpan) = notimpl()
                member tl.MapLocalSpansToTextOriginatingLayer(dwFlags, pLocalSpanEnum, ppTargetLayer, ppTargetSpanEnum) = notimpl()            
             interface IDelegable<IVsTextLayer> with
                member id.GetInner() = !inner
                member id.SetInner(i) = inner:=i
            }
            
        static member MakeTextLineMarker() =
            let mutable markerSpan = new TextSpan(iStartLine = 0, iEndLine = 0, iStartIndex = 0, iEndIndex = 0)
            {new IVsTextLineMarker with
                member tl.DrawGlyph(hdc, pRect) = notimpl()
                member tl.ExecMarkerCommand(iItem) = notimpl()
                member tl.GetBehavior(pdwBehavior) = notimpl()
                member tl.GetCurrentSpan(pSpan) = pSpan.[0] <- markerSpan ; 0
                member tl.GetLineBuffer(ppBuffer) = notimpl()
                member tl.GetMarkerCommandInfo(iItem, pbstrText, pcmdf) = notimpl()
                member tl.GetPriorityIndex(piPriorityIndex) = notimpl()
                member tl.GetTipText(pbstrText) = notimpl()
                member tl.GetType(piMarkerType) = notimpl()
                member tl.GetVisualStyle(pdwFlags) = notimpl()
                member tl.Invalidate() = 0
                member tl.ResetSpan(iSL, iSI, iEL, iEI) = 
                    markerSpan <- new TextSpan(iStartLine = iSL, iEndLine = iEL, iStartIndex = iSI, iEndIndex = iEI) ; 
                    0
                member tl.SetBehavior(dwBehavior) = notimpl()
                member tl.SetType(iMarkerType) = notimpl()
                member tl.SetVisualStyle(dwFlags) = notimpl()
                member tl.UnadviseClient() = 0
                
            interface IVsTextMarker with
                member tl.DrawGlyph(hdc, pRect) = notimpl()
                member tl.ExecMarkerCommand(iItem) = notimpl()
                member tl.GetBehavior(pdwBehavior) = notimpl()
                member tl.GetMarkerCommandInfo(iItem, pbstrText, pcmdf) = notimpl()
                member tl.GetPriorityIndex(piPriorityIndex) = notimpl()
                member tl.GetTipText(pbstrText) = notimpl()
                member tl.GetType(piMarkerType) = notimpl()
                member tl.GetVisualStyle(pdwFlags) = notimpl();
                member tl.Invalidate() = notimpl()
                member tl.SetBehavior(dwBehavior) = notimpl()
                member tl.SetType(iMarkerType) = notimpl()
                member tl.SetVisualStyle(dwFlags) = notimpl()
                member tl.UnadviseClient() = notimpl()
            }

        static member MakeTextLines() =
           let userData = new Dictionary<Guid,obj>()
           {new IVsTextLines with
                member tl.LockBuffer() = notimpl()
                member tl.UnlockBuffer() = notimpl()
                member tl.InitializeContent(pszText, iLength) = notimpl()
                member tl.GetStateFlags(pdwReadOnlyFlags) = notimpl()
                member tl.SetStateFlags(flags) = notimpl()
                member tl.GetPositionOfLine(line,position) = notimpl()
                member tl.GetPositionOfLineIndex(line, index, x) = notimpl()
                member tl.GetLineIndexOfPosition(position, line, x) = notimpl()
                member tl.GetLengthOfLine(line, length) = notimpl()
                member tl.GetLineCount(x) = notimpl()
                member tl.GetSize(x) = notimpl()
                member tl.GetLanguageServiceID(pguidLangService:System.Guid byref) : System.Int32 = notimpl()
                member tl.SetLanguageServiceID(guidLangServ) = notimpl()
                member tl.GetUndoManager(x) = notimpl()
                member tl.Reserved1() = notimpl()
                member tl.Reserved2() = notimpl()
                member tl.Reserved3() = notimpl()
                member tl.Reserved4() = notimpl()
                member tl.Reload(undoable) = notimpl()
                member tl.LockBufferEx(flags) = notimpl()
                member tl.UnlockBufferEx(flags) = notimpl()
                member tl.GetLastLineIndex(x,y) = notimpl()
                member tl.Reserved5() = notimpl()
                member tl.Reserved6() = notimpl()
                member tl.Reserved7() = notimpl()
                member tl.Reserved8() = notimpl()
                member tl.Reserved9() = notimpl()
                member tl.Reserved10() = notimpl()
                member tl.GetMarkerData(iTopLine, iBottomLine, pMarkerData)= notimpl()
                member tl.ReleaseMarkerData(pMarkerData)= notimpl()
                member tl.GetLineData(iLine, pLineData, pMarkerData)= notimpl()
                member tl.ReleaseLineData(pLineData)= notimpl()
                member tl.GetLineText(iStartLine, iStartIndex, iEndLine, iEndIndex, pbstrBuf) = notimpl()
                member tl.CopyLineText(iStartLine, iStartIndex, iEndLine, iEndIndex, pszBuf, pcchBuf)= notimpl()
                member tl.ReplaceLines(iStartLine, iStartIndex, iEndLine, iEndIndex, pszText, iNewLen, pChangedSpan)= notimpl()
                member tl.CanReplaceLines(iStartLine, iStartIndex, iEndLine, iEndIndex, iNewLen)= notimpl()
                member tl.CreateLineMarker(iMarkerType, iStartLine, iStartIndex, iEndLine, iEndIndex, pClient, ppMarker)= 
                    let textLineMarker = Vs.MakeTextLineMarker()
                    textLineMarker.ResetSpan(iStartLine, iStartIndex, iEndLine, iEndIndex) |> ignore
                    ppMarker.[0] <- textLineMarker ;
                    0
                member tl.EnumMarkers(iStartLine, iStartIndex, iEndLine, iEndIndex, iMarkerType, udwFlags, ppEnum)= notimpl()
                member tl.FindMarkerByLineIndex(iMarkerType, iStartingLine, iStartingIndex, udwFlags, ppMarker)= notimpl()
                member tl.AdviseTextLinesEvents(pSink, updwCookie)= notimpl()
                member tl.UnadviseTextLinesEvents(udwCookie)= notimpl()
                member tl.GetPairExtents(pSpanIn, pSpanOut)= notimpl()
                member tl.ReloadLines(iStartLine, iStartIndex, iEndLine, iEndIndex, pszText, iNewLen, pChangedSpan)= notimpl()
                member tl.IVsTextLinesReserved1(iLine, pLineData, fAttributes)= notimpl()
                member tl.GetLineDataEx(udwFlags, iLine, iStartIndex, iEndIndex, pLineData, pMarkerData)= notimpl()
                member tl.ReleaseLineDataEx(pLineData)= notimpl()
                member tl.CreateEditPoint(iLine, iIndex, ppEditPoint)= notimpl()
                member tl.ReplaceLinesEx(udwFlags, iStartLine, iStartIndex, iEndLine, iEndIndex, pszText, iNewLen, pChangedSpan)= notimpl()
                member tl.CreateTextPoint(iLine, iIndex, ppTextPoint)= notimpl()

            interface IVsTextBuffer with 
                member tl.LockBuffer() = notimpl()
                member tl.UnlockBuffer() = notimpl()
                member tl.InitializeContent(pszText, iLength) = notimpl()
                member tl.GetStateFlags(pdwReadOnlyFlags) = notimpl()
                member tl.SetStateFlags(flags) = notimpl()
                member tl.GetPositionOfLine(line,position) = notimpl()
                member tl.GetPositionOfLineIndex(line, index, x) = notimpl()
                member tl.GetLineIndexOfPosition(position, line, x) = notimpl()
                member tl.GetLengthOfLine(line, length) = notimpl()
                member tl.GetLineCount(x) = notimpl()
                member tl.GetSize(x) = notimpl()
                member tl.GetLanguageServiceID(pguidLangService:System.Guid byref) : System.Int32 = notimpl()
                member tl.SetLanguageServiceID(guidLangServ) = notimpl()
                member tl.GetUndoManager(x) = notimpl()
                member tl.Reserved1() = notimpl()
                member tl.Reserved2() = notimpl()
                member tl.Reserved3() = notimpl()
                member tl.Reserved4() = notimpl()

                member tl.Reload(undoable) = notimpl()
                member tl.LockBufferEx(flags) = notimpl()
                member tl.UnlockBufferEx(flags) = notimpl()
                member tl.GetLastLineIndex(x,y) = notimpl()
                member tl.Reserved5() = notimpl()
                member tl.Reserved6() = notimpl()
                member tl.Reserved7() = notimpl()
                member tl.Reserved8() = notimpl()
                member tl.Reserved9() = notimpl()
                member tl.Reserved10() = notimpl()

            interface IVsTextColorState with   
                member tcs.ReColorizeLines(iTopLine, iBottomLine) = notimpl()
                member tcs.GetColorStateAtStartOfLine(iLine, piState) = notimpl()
                
            interface IVsUserData with
                member x.SetData(guid,obj) =
                    if userData.ContainsKey(guid) then userData.Remove(guid) |> ignore
                    match obj with
                    |   null -> ()
                    |   _   -> userData.Add(guid,obj) |> ignore
                    VSConstants.S_OK
                member x.GetData(guid,obj) =
                    if userData.ContainsKey(guid) then 
                        obj <- userData.[guid]
                    else
                        obj <- null
                    VSConstants.S_OK
            }
        static member DelegateTextLines(oldtl:IVsTextLines, ?getLengthOfLine, ?getLineText, ?getLineCount, ?recolorizeLines, ?getColorStateAtStartOfLine, ?getData) = 
            let inner:Ref<IVsTextLines> = ref oldtl
            {new IVsTextLines with
                member tl.LockBuffer() = (!inner).LockBuffer()
                member tl.UnlockBuffer() = (!inner).UnlockBuffer()
                member tl.InitializeContent(pszText, iLength) = (!inner).InitializeContent(pszText, iLength)
                member tl.GetStateFlags(pdwReadOnlyFlags) = (!inner).GetStateFlags(ref pdwReadOnlyFlags)
                member tl.SetStateFlags(flags) = (!inner).SetStateFlags(flags)
                member tl.GetPositionOfLine(line,position) = (!inner).GetPositionOfLine(line, ref position)
                member tl.GetPositionOfLineIndex(line, index, x) = (!inner).GetPositionOfLineIndex(line, index, ref x)
                member tl.GetLineIndexOfPosition(position, line, x) = (!inner).GetLineIndexOfPosition(position, ref line, ref x)
                member tl.GetLengthOfLine(line, length) = 
                    let next line = (!inner).GetLengthOfLine(line)
                    let hr,l = impl1 getLengthOfLine next line
                    length<-l
                    hr 
                member tl.GetLineCount(count) = 
                    let next () = (!inner).GetLineCount()
                    let hr,c = uimpl getLineCount next 
                    count<-c
                    hr                     
                member tl.GetSize(x) = (!inner).GetSize(ref x)
                member tl.GetLanguageServiceID(pguidLangService:System.Guid byref)  = (!inner).GetLanguageServiceID(ref pguidLangService) 
                member tl.SetLanguageServiceID(guidLangServ) = (!inner).SetLanguageServiceID(ref guidLangServ)
                member tl.GetUndoManager(x) = (!inner).GetUndoManager(ref x)
                member tl.Reserved1() = (!inner).Reserved1()
                member tl.Reserved2() = (!inner).Reserved2()
                member tl.Reserved3() = (!inner).Reserved3()
                member tl.Reserved4() = (!inner).Reserved4()
                member tl.Reload(undoable) = (!inner).Reload(undoable)
                member tl.LockBufferEx(flags) = (!inner).LockBufferEx(flags)
                member tl.UnlockBufferEx(flags) = (!inner).UnlockBufferEx(flags)
                member tl.GetLastLineIndex(x,y) = (!inner).GetLastLineIndex(ref x,ref y)
                member tl.Reserved5() = (!inner).Reserved5()
                member tl.Reserved6() = (!inner).Reserved6()
                member tl.Reserved7() = (!inner).Reserved7()
                member tl.Reserved8() = (!inner).Reserved8()
                member tl.Reserved9() = (!inner).Reserved9()
                member tl.Reserved10() = (!inner).Reserved10()
                member tl.GetMarkerData(iTopLine, iBottomLine, pMarkerData) = (!inner).GetMarkerData(iTopLine, iBottomLine, pMarkerData)
                member tl.ReleaseMarkerData(pMarkerData) = (!inner).ReleaseMarkerData(pMarkerData)
                member tl.GetLineData(iLine, pLineData, pMarkerData) = (!inner).GetLineData(iLine, pLineData, pMarkerData)
                member tl.ReleaseLineData(pLineData)= (!inner).ReleaseLineData(pLineData)
                member tl.GetLineText(iStartLine, iStartIndex, iEndLine, iEndIndex, pbstrBuf) = 
                    let next a b c d = (!inner).GetLineText(a,b,c,d) 
                    let hr,b = impl4 getLineText next iStartLine iStartIndex iEndLine iEndIndex
                    pbstrBuf<-b
                    hr                     
                member tl.CopyLineText(iStartLine, iStartIndex, iEndLine, iEndIndex, pszBuf, pcchBuf)= (!inner).CopyLineText(iStartLine, iStartIndex, iEndLine, iEndIndex, pszBuf, ref pcchBuf)
                member tl.ReplaceLines(iStartLine, iStartIndex, iEndLine, iEndIndex, pszText, iNewLen, pChangedSpan)= (!inner).ReplaceLines(iStartLine, iStartIndex, iEndLine, iEndIndex, pszText, iNewLen, pChangedSpan)
                member tl.CanReplaceLines(iStartLine, iStartIndex, iEndLine, iEndIndex, iNewLen)= (!inner).CanReplaceLines(iStartLine, iStartIndex, iEndLine, iEndIndex, iNewLen)
                member tl.CreateLineMarker(iMarkerType, iStartLine, iStartIndex, iEndLine, iEndIndex, pClient, ppMarker)= (!inner).CreateLineMarker(iMarkerType, iStartLine, iStartIndex, iEndLine, iEndIndex, pClient, ppMarker)
                member tl.EnumMarkers(iStartLine, iStartIndex, iEndLine, iEndIndex, iMarkerType, udwFlags, ppEnum)= (!inner).EnumMarkers(iStartLine, iStartIndex, iEndLine, iEndIndex, iMarkerType, udwFlags, ref ppEnum)
                member tl.FindMarkerByLineIndex(iMarkerType, iStartingLine, iStartingIndex, udwFlags, ppMarker)= (!inner).FindMarkerByLineIndex(iMarkerType, iStartingLine, iStartingIndex, udwFlags, ref ppMarker)
                member tl.AdviseTextLinesEvents(pSink, updwCookie)= (!inner).AdviseTextLinesEvents(pSink, ref updwCookie)
                member tl.UnadviseTextLinesEvents(udwCookie)= (!inner).UnadviseTextLinesEvents(udwCookie)
                member tl.GetPairExtents(pSpanIn, pSpanOut)= (!inner).GetPairExtents(pSpanIn, pSpanOut)
                member tl.ReloadLines(iStartLine, iStartIndex, iEndLine, iEndIndex, pszText, iNewLen, pChangedSpan)= (!inner).ReloadLines(iStartLine, iStartIndex, iEndLine, iEndIndex, pszText, iNewLen, pChangedSpan)
                member tl.IVsTextLinesReserved1(iLine, pLineData, fAttributes)= (!inner).IVsTextLinesReserved1(iLine, pLineData, fAttributes)
                member tl.GetLineDataEx(udwFlags, iLine, iStartIndex, iEndIndex, pLineData, pMarkerData)= (!inner).GetLineDataEx(udwFlags, iLine, iStartIndex, iEndIndex, pLineData, pMarkerData)
                member tl.ReleaseLineDataEx(pLineData)= (!inner).ReleaseLineDataEx(pLineData)
                member tl.CreateEditPoint(iLine, iIndex, ppEditPoint)= (!inner).CreateEditPoint(iLine, iIndex, ref ppEditPoint)
                member tl.ReplaceLinesEx(udwFlags, iStartLine, iStartIndex, iEndLine, iEndIndex, pszText, iNewLen, pChangedSpan)= (!inner).ReplaceLinesEx(udwFlags, iStartLine, iStartIndex, iEndLine, iEndIndex, pszText, iNewLen, pChangedSpan)
                member tl.CreateTextPoint(iLine, iIndex, ppTextPoint)= (!inner).CreateTextPoint(iLine, iIndex, ref ppTextPoint)

             interface IVsTextBuffer with 
                member tl.LockBuffer() = (!inner).LockBuffer()
                member tl.UnlockBuffer() = (!inner).UnlockBuffer()
                member tl.InitializeContent(pszText, iLength) = (!inner).InitializeContent(pszText, iLength)
                member tl.GetStateFlags(pdwReadOnlyFlags) = (!inner).GetStateFlags(ref pdwReadOnlyFlags)
                member tl.SetStateFlags(flags) = (!inner).SetStateFlags(flags)
                member tl.GetPositionOfLine(line,position) = (!inner).GetPositionOfLine(line, ref position)
                member tl.GetPositionOfLineIndex(line, index, x) = (!inner).GetPositionOfLineIndex(line, index, ref x)
                member tl.GetLineIndexOfPosition(position, line, x) = (!inner).GetLineIndexOfPosition(position, ref line, ref x)
                member tl.GetLengthOfLine(line, length) = 
                    let next line = (!inner).GetLengthOfLine(line)
                    let hr,l = impl1 getLengthOfLine next line
                    length<-l
                    hr 
                member tl.GetLineCount(x) = (!inner).GetLineCount(ref x)
                member tl.GetSize(x) = (!inner).GetSize(ref x)
                member tl.GetLanguageServiceID(pguidLangService:System.Guid byref)  = (!inner).GetLanguageServiceID(ref pguidLangService) 
                member tl.SetLanguageServiceID(guidLangServ) = (!inner).SetLanguageServiceID(ref guidLangServ)
                member tl.GetUndoManager(x) = (!inner).GetUndoManager(ref x)
                member tl.Reserved1() = (!inner).Reserved1()
                member tl.Reserved2() = (!inner).Reserved2()
                member tl.Reserved3() = (!inner).Reserved3()
                member tl.Reserved4() = (!inner).Reserved4()
                member tl.Reload(undoable) = (!inner).Reload(undoable)
                member tl.LockBufferEx(flags) = (!inner).LockBufferEx(flags)
                member tl.UnlockBufferEx(flags) = (!inner).UnlockBufferEx(flags)
                member tl.GetLastLineIndex(x,y) = (!inner).GetLastLineIndex(ref x,ref y)
                member tl.Reserved5() = (!inner).Reserved5()
                member tl.Reserved6() = (!inner).Reserved6()
                member tl.Reserved7() = (!inner).Reserved7()
                member tl.Reserved8() = (!inner).Reserved8()
                member tl.Reserved9() = (!inner).Reserved9()
                member tl.Reserved10() = (!inner).Reserved10()

            interface IVsTextColorState with   
                member tcs.ReColorizeLines(iTopLine, iBottomLine) = 
                    let next top bottom = ((box (!inner)):?>IVsTextColorState).ReColorizeLines(top, bottom)
                    let hr:int = impl2 recolorizeLines next iTopLine iBottomLine
                    hr
                member tcs.GetColorStateAtStartOfLine(iLine, piState) = 
                    let next iLine = ((box (!inner)):?>IVsTextColorState).GetColorStateAtStartOfLine(iLine)
                    let hr,state = impl1 getColorStateAtStartOfLine next iLine
                    piState<-state
                    hr
                    
            interface IVsUserData with
                member ud.GetData(riidKey,pvtData) = 
                    let next riidKey = ((box (!inner)):?>IVsUserData).GetData(riidKey)
                    let (hr:int),data = impl1 getData next (ref riidKey)
                    pvtData<-data
                    hr
                member ud.SetData(riidKey,vtData) = ((box (!inner)):?>IVsUserData).SetData(ref riidKey,vtData)                    

            interface IDelegable<IVsTextLines> with
                member id.GetInner() = !inner
                member id.SetInner(i) = inner:=i
             }            
            
// (The RDT is no longer used by the product, but leaving in VsMocks/Salsa in case we need it again in the future            
        static member MakeRunningDocumentTable() = 
            {new IVsRunningDocumentTable with
                member rdt.RegisterAndLockDocument(grfRDTLockType, pszMkDocument, pHier, itemid, punkDocData, pdwCookie) = notimpl()
                member rdt.LockDocument(grfRDTLockType, dwCookie) = notimpl()
                member rdt.UnlockDocument(grfRDTLockType, dwCookie) = notimpl()
                member rdt.FindAndLockDocument(dwRDTLockType, pszMkDocument, ppHier, pitemid, ppunkDocData, pdwCookie) = fail
                member rdt.RenameDocument(pszMkDocumentOld, pszMkDocumentNew, pHier, itemidNew) = notimpl()
                member rdt.AdviseRunningDocTableEvents(pSink, pdwCookie) = notimpl()
                member rdt.UnadviseRunningDocTableEvents(dwCookie) = notimpl()
                member rdt.GetDocumentInfo(docCookie, pgrfRDTFlags, pdwReadLocks, pdwEditLocks, pbstrMkDocument, ppHier, pitemid, ppunkDocData) = notimpl()
                member rdt.NotifyDocumentChanged(dwCookie, grfDocChanged) = notimpl()
                member rdt.NotifyOnAfterSave(dwCookie) = notimpl()
                member rdt.GetRunningDocumentsEnum(ppenum) = notimpl()
                member rdt.SaveDocuments(grfSaveOpts, pHier, itemid, docCookie) = notimpl()
                member rdt.NotifyOnBeforeSave(dwCookie) = notimpl()
                member rdt.RegisterDocumentLockHolder(grfRDLH, dwCookie, pLockHolder, pdwLHCookie) = notimpl()
                member rdt.UnregisterDocumentLockHolder(dwLHCookie) = notimpl()
                member rdt.ModifyDocumentFlags(docCookie, grfFlags, fSet) = notimpl() }
                
        static member DelegateRunningDocumentTable(oldrdt:IVsRunningDocumentTable, ?getDocumentInfo, ?unadviseRunningDocTableEvents, ?findAndLockDocument) = 
            let inner:Ref<IVsRunningDocumentTable> = ref oldrdt
            {new IVsRunningDocumentTable with
                member rdt.RegisterAndLockDocument(grfRDTLockType, pszMkDocument, pHier, itemid, punkDocData, pdwCookie) = (!inner).RegisterAndLockDocument(grfRDTLockType, pszMkDocument, pHier, itemid, punkDocData, ref pdwCookie)
                member rdt.LockDocument(grfRDTLockType, dwCookie) = (!inner).LockDocument(grfRDTLockType, dwCookie)
                member rdt.UnlockDocument(grfRDTLockType, dwCookie) = (!inner).UnlockDocument(grfRDTLockType, dwCookie)
                member rdt.FindAndLockDocument(dwRDTLockType, pszMkDocument, ppHier, pitemid, ppunkDocData, pdwCookie) = 
                    let next() = (!inner).FindAndLockDocument(dwRDTLockType,pszMkDocument)
                    let (hr:int),hier,itemid,docData,cookie =  
                        match findAndLockDocument with
                            | Some(f) -> 
                                match f dwRDTLockType pszMkDocument with
                                    Some(r)-> r
                                    | _ -> next()
                            | _ -> next()
                    ppHier<-hier
                    pitemid<- itemid
                    ppunkDocData<-docData
                    pdwCookie <- cookie
                    hr                
                
                member rdt.RenameDocument(pszMkDocumentOld, pszMkDocumentNew, pHier, itemidNew) = (!inner).RenameDocument(pszMkDocumentOld, pszMkDocumentNew, pHier, itemidNew)
                member rdt.AdviseRunningDocTableEvents(pSink, pdwCookie) = (!inner).AdviseRunningDocTableEvents(pSink, ref pdwCookie)
                member rdt.UnadviseRunningDocTableEvents(dwCookie) = 
                    let next() = (!inner).UnadviseRunningDocTableEvents(dwCookie)
                    let hr =  
                        match unadviseRunningDocTableEvents with
                            Some(f) -> 
                                match f dwCookie with
                                    Some(r)-> r
                                    | _ -> next()
                            | _ -> next()
                    hr
                member rdt.GetDocumentInfo(docCookie, pgrfRDTFlags, pdwReadLocks, pdwEditLocks, pbstrMkDocument, ppHier, pitemid, ppunkDocData) = 
                    let next() = (!inner).GetDocumentInfo(docCookie)
                    let hr,_,_,_,mkd,hier,itemid,docd =  
                        match getDocumentInfo with
                            Some(f) -> 
                                match f docCookie with
                                    Some(r)-> r
                                    | _ -> next()
                            | _ -> next()
                    pbstrMkDocument<-mkd
                    ppHier<- hier
                    pitemid<-itemid
                    ppunkDocData <- docd
                    hr
                        
                member rdt.NotifyDocumentChanged(dwCookie, grfDocChanged) = (!inner).NotifyDocumentChanged(dwCookie, grfDocChanged)
                member rdt.NotifyOnAfterSave(dwCookie) = (!inner).NotifyOnAfterSave(dwCookie)
                member rdt.GetRunningDocumentsEnum(ppenum) = (!inner).GetRunningDocumentsEnum(ref ppenum)
                member rdt.SaveDocuments(grfSaveOpts, pHier, itemid, docCookie) = (!inner).SaveDocuments(grfSaveOpts, pHier, itemid, docCookie)
                member rdt.NotifyOnBeforeSave(dwCookie) = (!inner).NotifyOnBeforeSave(dwCookie)
                member rdt.RegisterDocumentLockHolder(grfRDLH, dwCookie, pLockHolder, pdwLHCookie) = (!inner).RegisterDocumentLockHolder(grfRDLH, dwCookie, pLockHolder, ref pdwLHCookie)
                member rdt.UnregisterDocumentLockHolder(dwLHCookie) = (!inner).UnregisterDocumentLockHolder(dwLHCookie)
                member rdt.ModifyDocumentFlags(docCookie, grfFlags, fSet) = (!inner).ModifyDocumentFlags(docCookie, grfFlags, fSet) 
             interface IDelegable<IVsRunningDocumentTable> with
                member id.GetInner() = !inner
                member id.SetInner(i) = inner:=i
             }       
// )             
        static member MakeHierarchy(projectSiteFactory:IProvideProjectSite) = 
            {new IVsHierarchy with
                member h.SetSite(psp) = notimpl()
                member h.GetSite(ppSP) = notimpl()
                member h.QueryClose(pfCanClose) = notimpl()
                member h.Close() = notimpl()
                member h.GetGuidProperty(itemid, propid, pguid) = 
                    pguid <- FSharpProjectGuid   
                    VSConstants.S_OK
                member h.SetGuidProperty(itemid, propid, rguid) = notimpl()
                member h.GetProperty(itemid, propid, pvar) = notimpl()
                member h.SetProperty(itemid, propid, var) = notimpl()
                member h.GetNestedHierarchy(itemid, iidHierarchyNested, ppHierarchyNested, pitemidNested) = notimpl()
                member h.GetCanonicalName(itemid, pbstrName) = notimpl()
                member h.ParseCanonicalName(pszName, pitemid) = notimpl()
                member h.Unused0() = notimpl()
                member h.AdviseHierarchyEvents(pEventSink, pdwCookie) = notimpl()
                member h.UnadviseHierarchyEvents(dwCookie) = notimpl()
                member h.Unused1() = notimpl()
                member h.Unused2() = notimpl()
                member h.Unused3() = notimpl()
                member h.Unused4() = notimpl()
             interface IProvideProjectSite with
                member x.GetProjectSite() = projectSiteFactory.GetProjectSite()
            }
            
        static member DelegateHierarchy(oldh:IVsHierarchy, ?getCanonicalName, ?getProperty) = 
            let mutable inner = oldh
            {new IVsHierarchy with            
                member h.SetSite(psp) = inner.SetSite(psp)
                member h.GetSite(ppSP) = inner.GetSite(ref ppSP)
                member h.QueryClose(pfCanClose) = inner.QueryClose(ref pfCanClose)
                member h.Close() = inner.Close()
                member h.GetGuidProperty(itemid, propid, pguid) = inner.GetGuidProperty(itemid, propid, ref pguid)
                member h.SetGuidProperty(itemid, propid, rguid) = inner.SetGuidProperty(itemid, propid, ref rguid)
                member h.GetProperty(itemid, propid, pvar) = 
                    let propid:__VSHPROPID  = enum propid
                    let next itemid propid = inner.GetProperty(itemid, int32 propid)
                    let hr,var = impl2 getProperty next itemid propid
                    pvar<-var
                    hr
                member h.SetProperty(itemid, propid, var) = inner.SetProperty(itemid, propid, var)
                member h.GetNestedHierarchy(itemid, iidHierarchyNested, ppHierarchyNested, pitemidNested) = inner.GetNestedHierarchy(itemid,ref iidHierarchyNested, ref ppHierarchyNested, ref pitemidNested)
                /// For project files, returns the fully qualified path to the file including the filename itself.
                member h.GetCanonicalName(itemid, pbstrName) = 
                    let next itemid = inner.GetCanonicalName(itemid)
                    let hr,n = impl1 getCanonicalName next itemid
                    pbstrName<-n
                    hr                     
                member h.ParseCanonicalName(pszName, pitemid) = inner.ParseCanonicalName(pszName, ref pitemid)
                member h.Unused0() = inner.Unused0()
                member h.AdviseHierarchyEvents(pEventSink, pdwCookie) = inner.AdviseHierarchyEvents(pEventSink, ref pdwCookie)
                member h.UnadviseHierarchyEvents(dwCookie) = inner.UnadviseHierarchyEvents(dwCookie)
                member h.Unused1() = inner.Unused1()
                member h.Unused2() = inner.Unused2()
                member h.Unused3() = inner.Unused3()
                member h.Unused4() = inner.Unused4()
             interface IDelegable<IVsHierarchy> with
                member id.GetInner() = inner
                member id.SetInner(i) = inner <- i
                
             interface IProvideProjectSite with
                member x.GetProjectSite() = (inner :?> IProvideProjectSite).GetProjectSite()

             }       
             
        static member MakeTextManager() = 
            {new IVsTextManager with
                member tm.RegisterView(pView,pBuffer) = notimpl()
                member tm.UnregisterView(pView) = notimpl()
                member tm.EnumViews(pBuffer,ppEnum) = notimpl()
                member tm.CreateSelectionAction(pBuffer,ppAction) = notimpl()
                member tm.MapFilenameToLanguageSID(pszFileName,pguidLangSID) = notimpl()
                member tm.GetRegisteredMarkerTypeID(pguidMarker,piMarkerTypeID) = notimpl()
                member tm.GetMarkerTypeInterface(iMarkerTypeID, ppMarkerType) = notimpl()
                member tm.GetMarkerTypeCount(piMarkerTypeCount) = notimpl()
                member tm.GetActiveView(fMustHaveFocus,pBuffer,ppView) = notimpl()
                member tm.GetUserPreferences(pViewPrefs, pFramePrefs, pLangPrefs, pColorPrefs) = notimpl()
                member tm.SetUserPreferences(pViewPrefs,pFramePrefs,pLangPrefs,pColorPrefs) = notimpl()
                member tm.SetFileChangeAdvise(pszFileName,fStart) = notimpl()
                member tm.SuspendFileChangeAdvise(pszFileName,fSuspend) = notimpl()
                member tm.NavigateToPosition(pBuffer,guidDocViewType,iPos,iLen) = notimpl()
                member tm.NavigateToLineAndColumn(pBuffer,guidDocViewType,iStartRow,iStartIndex,iEndRow,iEndIndex) = notimpl()
                member tm.GetBufferSccStatus(pBufData,pbNonEditable) = notimpl()
                member tm.RegisterBuffer(pBuffer) = notimpl()
                member tm.UnregisterBuffer(pBuffer) = notimpl()
                member tm.EnumBuffers(ppEnum) = notimpl()
                member tm.GetPerLanguagePreferences(pLangPrefs) = notimpl()
                member tm.SetPerLanguagePreferences(pLangPrefs) = notimpl()
                member tm.AttemptToCheckOutBufferFromScc(pBufData,pfCheckoutSucceeded) = notimpl()
                member tm.GetShortcutManager(ppShortcutMgr) = notimpl()
                member tm.RegisterIndependentView(pUnk,pBuffer) = notimpl()
                member tm.UnregisterIndependentView(pUnk,pBuffer) = notimpl()
                member tm.IgnoreNextFileChange(pBuffer) = notimpl()
                member tm.AdjustFileChangeIgnoreCount(pBuffer,fIgnore) = notimpl()
                member tm.GetBufferSccStatus2(pszFileName,pbNonEditable,piStatusFlags) = notimpl()
                member tm.AttemptToCheckOutBufferFromScc2(pszFileName,pfCheckoutSucceeded,piStatusFlags) = notimpl()
                member tm.EnumLanguageServices(ppEnum) = notimpl()
                member tm.EnumIndependentViews(pBuffer,ppEnum) = notimpl()              
            }
            
        static member DelegateTextManager(oldtm:IVsTextManager, ?getActiveView) = 
            let inner = ref oldtm
            {new IVsTextManager with
                member tm.RegisterView(pView,pBuffer) = notimpl()
                member tm.UnregisterView(pView) = notimpl()
                member tm.EnumViews(pBuffer,ppEnum) = notimpl()
                member tm.CreateSelectionAction(pBuffer,ppAction) = notimpl()
                member tm.MapFilenameToLanguageSID(pszFileName,pguidLangSID) = notimpl()
                member tm.GetRegisteredMarkerTypeID(pguidMarker,piMarkerTypeID) = notimpl()
                member tm.GetMarkerTypeInterface(iMarkerTypeID, ppMarkerType) = notimpl()
                member tm.GetMarkerTypeCount(piMarkerTypeCount) = notimpl()
                member tm.GetActiveView(fMustHaveFocus,pBuffer,ppView) = 
                    let next fMustHaveFocus pBuffer = (!inner).GetActiveView(fMustHaveFocus,pBuffer)
                    let hr,v = impl2 getActiveView next fMustHaveFocus pBuffer
                    ppView<-v
                    hr   
                member tm.GetUserPreferences(pViewPrefs, pFramePrefs, pLangPrefs, pColorPrefs) = notimpl()
                member tm.SetUserPreferences(pViewPrefs,pFramePrefs,pLangPrefs,pColorPrefs) = notimpl()
                member tm.SetFileChangeAdvise(pszFileName,fStart) = notimpl()
                member tm.SuspendFileChangeAdvise(pszFileName,fSuspend) = notimpl()
                member tm.NavigateToPosition(pBuffer,guidDocViewType,iPos,iLen) = notimpl()
                member tm.NavigateToLineAndColumn(pBuffer,guidDocViewType,iStartRow,iStartIndex,iEndRow,iEndIndex) = notimpl()
                member tm.GetBufferSccStatus(pBufData,pbNonEditable) = notimpl()
                member tm.RegisterBuffer(pBuffer) = notimpl()
                member tm.UnregisterBuffer(pBuffer) = notimpl()
                member tm.EnumBuffers(ppEnum) = notimpl()
                member tm.GetPerLanguagePreferences(pLangPrefs) = notimpl()
                member tm.SetPerLanguagePreferences(pLangPrefs) = notimpl()
                member tm.AttemptToCheckOutBufferFromScc(pBufData,pfCheckoutSucceeded) = notimpl()
                member tm.GetShortcutManager(ppShortcutMgr) = notimpl()
                member tm.RegisterIndependentView(pUnk,pBuffer) = notimpl()
                member tm.UnregisterIndependentView(pUnk,pBuffer) = notimpl()
                member tm.IgnoreNextFileChange(pBuffer) = notimpl()
                member tm.AdjustFileChangeIgnoreCount(pBuffer,fIgnore) = notimpl()
                member tm.GetBufferSccStatus2(pszFileName,pbNonEditable,piStatusFlags) = notimpl()
                member tm.AttemptToCheckOutBufferFromScc2(pszFileName,pfCheckoutSucceeded,piStatusFlags) = notimpl()
                member tm.EnumLanguageServices(ppEnum) = notimpl()
                member tm.EnumIndependentViews(pBuffer,ppEnum) = notimpl()              
             interface IDelegable<IVsTextManager> with
                member id.GetInner() = !inner
                member id.SetInner(i) = inner := i
            }
    let private getInner (o:'a) = 
        let d = (box o):?>(IDelegable<'a>)
        d.GetInner()
        
    let private setInner (o:'a) i = 
        let d = (box o):?>(IDelegable<'a>)
        d.SetInner(i)
        

    // IVsTextLayer ---------------------------------------------------------------------------------------------------------        
    let createTextLayer() = 
        let tl = Vs.DelegateTextLayer (Vs.MakeTextLayer())
        let inner = getInner tl
        let localLineIndexToBase (iLocalLine:int) (iLocalIndex:int) =
            Some(ok,iLocalLine,iLocalIndex) // For now, just forward the values with no translation
        let inner = Vs.DelegateTextLayer(inner, localLineIndexToBase=localLineIndexToBase)
        setInner tl inner
        tl
        
    // IVsTextView ---------------------------------------------------------------------------------------------------------        
    let createTextView() : IVsTextView = Vs.DelegateTextView(Vs.MakeTextView())
    let setFileText (filename:string) (tv:IVsTextView) (lines:string array) (recolorizeLines:int->int->unit) (getColorStateAtStartOfLine:int->int) = 
        let filename = System.IO.Path.GetFullPath(filename)
        let inner = getInner tv
        let lineCount = lines.Length
        let getLineCount() = Some(ok, lines.Length)
        // Line number is zero-relative.
        let getLengthOfLine line = Some(ok, lines.[line].Length)
        // It looks like VS has GetLineText taking zero-relative values.
        // The actually cursor position is 1-relative though
        let getLineText iStartLine iStartIndex iEndLine iEndIndex = 
            let slice = [iStartLine..iEndLine] 
                            |>List.map(fun i->(lines.[i].Substring(iStartIndex, iEndIndex-iStartIndex)))
            Some(ok, System.String.Join("\r\n",Array.ofList slice))
        let recolorizeLines (top:int) (bottom:int) = 
            recolorizeLines top bottom
            Some(ok)
        let getColorStateAtStartOfLine (line:int) = 
            Some(ok,getColorStateAtStartOfLine line)
            
        let vsBufferMoniker = Guid("978A8E17-4DF8-432A-9623-D530A26452BC")
            
        let getData (riidKey:Guid ref) =            
            if !riidKey = vsBufferMoniker then Some(ok, box filename)
            else None
                        
        let tl = Vs.DelegateTextLines(Vs.MakeTextLines(),
                                            getLineCount=getLineCount,
                                            getLengthOfLine=getLengthOfLine, 
                                            getLineText=getLineText,
                                            recolorizeLines=recolorizeLines,
                                            getColorStateAtStartOfLine=getColorStateAtStartOfLine,
                                            getData=getData
                                            )
                                                    
        // Maybe overly simplistic: Make the whole file visible in one page
        let getScrollInfo _iBar =
            Some(ok,0,lineCount-1,lineCount-1,0)
        let toplayer:IVsTextLayer = createTextLayer()
        let getTopmostLayer() = Some(ok,toplayer)
        let getBuffer() = Some(ok, tl)
        let inner = Vs.DelegateTextView(inner, getBuffer=getBuffer, getScrollInfo=getScrollInfo, getTopmostLayer=getTopmostLayer)
        setInner tv inner

        
// (The RDT is no longer used by the product, but leaving in VsMocks/Salsa in case we need it again in the future            
    // IVsRunningDocumentTable ---------------------------------------------------------------------------------------------------------        
    let createRdt() = 
        let unadviseRunningDocTableEvents _ = Some(ok)
        Vs.DelegateRunningDocumentTable (Vs.MakeRunningDocumentTable(),unadviseRunningDocTableEvents=unadviseRunningDocTableEvents)
    let openDocumentInRdt rdt cookie filename (textview:IVsTextView) hier = 
        let filename = System.IO.Path.GetFullPath(filename) 
        let inner = getInner rdt
        let _hr, textlines = textview.GetBuffer()
        let getDocumentInfoResult = (Some(ok,0u,0u,0u,filename,hier,cookie,Marshal.GetIUnknownForObject(textlines)))
        let refGetDocumentInfoResult = ref getDocumentInfoResult
        let getDocumentInfo c =
            if cookie = c then !refGetDocumentInfoResult
            else None
        let findAndLockDocumentResult = (Some(ok,hier,0u,Marshal.GetIUnknownForObject(textlines),0u))
        let refFindAndLockDocumentResult = ref findAndLockDocumentResult
        let findAndLockDocument _dwRDTLockType pszMkDocument = 
            if pszMkDocument = filename then !refFindAndLockDocumentResult
            else None
        let inner = Vs.DelegateRunningDocumentTable(inner,getDocumentInfo=getDocumentInfo,findAndLockDocument=findAndLockDocument)
        setInner rdt inner
// )

    let createLanguagePreferences() = null:>LanguagePreferences
    
    // IVsHierarchy ---------------------------------------------------------------------------------------------------------        
    let createHier(projectSiteFactory) =  
        let getProperty _id _prop = Some(VSConstants.E_FAIL,null)
        let hier = Vs.DelegateHierarchy(Vs.MakeHierarchy(projectSiteFactory), getProperty=getProperty)
        hier
    let setHierRoot hier projectdir projectname = 
        let guid = System.Guid.NewGuid().ToString()
        let inner = getInner hier
        let getCanonicalName itemid = 
            if itemid = VSConstants.VSITEMID_ROOT then Some(ok,projectdir)
            else None
        let getProperty id prop =
            if id = VSConstants.VSITEMID_ROOT then
                match prop with
                | __VSHPROPID.VSHPROPID_Name->Some(ok, box projectname)
                | __VSHPROPID.VSHPROPID_ProjectDir->Some(ok, box projectdir)
                | __VSHPROPID.VSHPROPID_OwnerKey->Some(ok, box guid)
                | _ -> Some(fail,null)
            else None
        let inner = Vs.DelegateHierarchy(inner, getCanonicalName=getCanonicalName,getProperty=getProperty)
        setInner hier inner
    let rec getLastSiblingId (hier:IVsHierarchy) (last:uint32) =
        let hr,next = hier.GetProperty(last, int32 __VSHPROPID.VSHPROPID_NextSibling)
        if hr = VSConstants.S_OK then 
            let nid:int32 = downcast next
            getLastSiblingId hier (uint32 nid)
        else last
        
    let addChild (hier:IVsHierarchy) parentItemId childItemId filename =
        let hr, child = hier.GetProperty(parentItemId, int32 __VSHPROPID.VSHPROPID_FirstChild)
        let inner = getInner hier
        let cid = (int32)childItemId // VS is confused about whether it wants item IDs to be signed or unsigned.
        let getCanonicalName id = 
            if id = childItemId then Some(ok,filename)
            else None            
        if hr = VSConstants.S_OK then
            // There's already a first child, add as a sibling
            let childi32:int32 = downcast child
            let lastSiblingId = getLastSiblingId hier (uint32 childi32)
            let getProperty id prop = 
                if id = lastSiblingId then 
                    match prop with
                    | __VSHPROPID.VSHPROPID_NextSibling -> Some(ok, box cid)
                    | _ -> None
                else if id = childItemId then
                    match prop with
                    | __VSHPROPID.VSHPROPID_Name-> Some(ok, box filename)
                    | __VSHPROPID.VSHPROPID_NextSibling -> Some(fail, null)
                    | _ -> None
                else None
            let inner = Vs.DelegateHierarchy(inner, getProperty=getProperty, getCanonicalName=getCanonicalName)
            setInner hier inner
        else 
            let getProperty id prop =
                if id = parentItemId then 
                    match prop with 
                    | __VSHPROPID.VSHPROPID_FirstChild -> Some(ok, box cid)
                    | _ -> None
                else if id = childItemId then
                    match prop with
                    | __VSHPROPID.VSHPROPID_Name-> Some(ok, box filename)
                    | __VSHPROPID.VSHPROPID_NextSibling -> Some(fail, null)
                    | _ -> None
                else None
            let inner = Vs.DelegateHierarchy(inner, getProperty=getProperty, getCanonicalName=getCanonicalName)
            setInner hier inner
    
    let addRootChild hier childItemId filename = 
        addChild hier VSConstants.VSITEMID_ROOT childItemId filename 
        
    // IVsTextManager ---------------------------------------------------------------------------------------------------------        
    let createTextManager() = Vs.DelegateTextManager (Vs.MakeTextManager())
    let setActiveView tm (view:IVsTextView) =
        let inner = getInner tm
        let getActiveView (_mustHaveFocus:int) (_buffer:IVsTextBuffer) = Some(ok,view)
        let inner = Vs.DelegateTextManager(inner, getActiveView=getActiveView)
        setInner tm inner
        
    type MuxLogger() =
        let mutable innerLoggers = []
        let mutable iEventSource = null
        member x.Add(logger : ILogger) = 
            innerLoggers <- logger :: innerLoggers
            if iEventSource <> null then
                logger.Initialize(iEventSource)  // hacky, but works well enough for unit tests, unfortunately 'register' calls can come after 'initialize' call
        interface ILogger with
            member x.Initialize(eventSource) =
                innerLoggers |> List.iter (fun l -> l.Initialize(eventSource))
                iEventSource <- eventSource
            member x.Shutdown() =
                innerLoggers |> List.iter (fun l -> l.Shutdown())
                iEventSource <- null
            member x.Parameters with get() = match innerLoggers with [] -> "" | h::t -> h.Parameters
                                and set(s) = innerLoggers |> List.iter (fun l -> l.Parameters <- s)
            member x.Verbosity with get() = match innerLoggers with [] -> LoggerVerbosity.Normal | h::t -> h.Verbosity
                               and set(s) = innerLoggers |> List.iter (fun l -> l.Verbosity <- s)

    /////////////////////////////////
    // mocks
    let err(line) : int = 
        printfn "err() called on line %s with %s" line System.Environment.StackTrace 
        failwith "not implemented"
    let nothing() = 0
            
    let vsShell() =
        let dict = new Dictionary<int,Object>()
        let shell = { new IVsShell with
            member this.AdviseBroadcastMessages(sink, cookie) = err(__LINE__)
            member this.AdviseShellPropertyChanges(sink, cookie) = err(__LINE__)
            member this.GetPackageEnum e = err(__LINE__)
            member this.GetProperty (propId : int, result : byref<Object>) = 
                let value = ref (null : Object)
                let ok = dict.TryGetValue(propId, value)
                if ok then
                    result <- !value
                    0
                else
                    result <- null
                    1
            member this.IsPackageInstalled (guid, inst) = err(__LINE__)
            member this.IsPackageLoaded (guid, pack) = err(__LINE__)
            member this.LoadPackage (guid, pack) = err(__LINE__)
            member this.LoadPackageString (guid, id, str) = err(__LINE__)
            member this.LoadUILibrary(guid, flags, lib) = err(__LINE__)
            member this.SetProperty (propId : int, x : Object) =
                dict.Add(propId, x)
                0
            member this.UnadviseBroadcastMessages cookie = err(__LINE__)
            member this.UnadviseShellPropertyChanges cookie = err(__LINE__)
            }
        shell.SetProperty(int __VSSPROPID.VSSPROPID_IsInCommandLineMode, box false) |> ignore
        shell.SetProperty(int __VSSPROPID.VSSPROPID_InstallDirectory, box "") |> ignore
        shell
        

    let vsUIHierarchyWindow =
        { new IVsUIHierarchyWindow with
            member this.Init(  pUIH,   grfUIHWF,    ppunkOut) = err(__LINE__)
            member this.ExpandItem(  pUIH,   itemid,   expf) =
                0
            member this.AddUIHierarchy(  pUIH,   grfAddOptions) = err(__LINE__)
            member this.RemoveUIHierarchy(  pUIH) = err(__LINE__)
            member this.SetWindowHelpTopic(  lpszHelpFile,   dwContext) = err(__LINE__)
            member this.GetItemState(  pHier,   itemid,   dwStateMask,    pdwState) = 
                0
            member this.FindCommonSelectedHierarchy(  grfOpt,    lppCommonUIH) = err(__LINE__)
            member this.SetCursor(  hNewCursor,    phOldCursor) = err(__LINE__)
            member this.GetCurrentSelection(  ppHier,    pitemid,    ppMIS) = err(__LINE__)
            }

    let vsWindowFrame =
        { new IVsWindowFrame with
            member this.Show() = err(__LINE__)
            member this.Hide() = err(__LINE__)
            member this.IsVisible() = err(__LINE__)
            member this.ShowNoActivate() = err(__LINE__)
            member this.CloseFrame(  grfSaveOptions) = err(__LINE__)
            member this.SetFramePos(  dwSFP,    rguidRelativeTo,   x,   y,   cx,   cy) = err(__LINE__)
            member this.GetFramePos(  pdwSFP,   pguidRelativeTo,   px,   py,   pcx,   pcy) = err(__LINE__)
            member this.GetProperty(  propid,    pvar : byref<obj>) = 
                pvar <- vsUIHierarchyWindow
                0
            member this.SetProperty(  propid,   var) = err(__LINE__)
            member this.GetGuidProperty(  propid,   pguid) = err(__LINE__)
            member this.SetGuidProperty(  propid,    rguid) = err(__LINE__)
            member this.QueryViewInterface(   riid,   ppv) = err(__LINE__)
            member this.IsOnScreen(   pfOnScreen) = err(__LINE__)
            }

    let mutable vsUIShellShowMessageBoxResult = None
    
    let vsUIShell =
        { new IVsUIShell with
            member this.GetToolWindowEnum(   ppenum) = err(__LINE__)
            member this.GetDocumentWindowEnum(   ppenum) =
                // cons up a fake empty enumerator (e.g. don't really support this)
                ppenum <- { new IEnumWindowFrames with
                                member this.Next(celt, rgelt, pceltFetched) = pceltFetched <- 0u; 0
                                member this.Skip(celt) = 0
                                member this.Reset() = 0
                                member this.Clone(ppenum) = ppenum <- this; 0 }
                0
            member this.FindToolWindow(  grfFTW,    rPersistenceSlot,    ppWindowFrame: byref<IVsWindowFrame> ) =
                ppWindowFrame <- vsWindowFrame
                0
            member this.CreateToolWindow(  grfCTW,   dwToolWindowId,   punkTool,    rclsidTool,    rPersistenceSlot,    rAutoActivate,   psp,   pszCaption,   pfDefaultPosition,    ppWindowFrame) = err(__LINE__)
            member this.CreateDocumentWindow(  grfCDW,   pszMkDocument,   pUIH,   itemid,   punkDocView,   punkDocData,    rEditorType,   pszPhysicalView,    rCmdUI,   psp,   pszOwnerCaption,   pszEditorCaption,   pfDefaultPosition,    ppWindowFrame) = err(__LINE__)
            member this.SetErrorInfo(  hr,   pszDescription,   dwReserved,   pszHelpKeyword,   pszSource) = err(__LINE__)
            member this.ReportErrorInfo(  hr) = err(__LINE__)
            member this.GetDialogOwnerHwnd(  phwnd) = err(__LINE__)
            member this.EnableModeless(  fEnable) = err(__LINE__)
            member this.SaveDocDataToFile(  grfSave,   pPersistFile,   pszUntitledPath,    pbstrDocumentNew,    pfCanceled) = err(__LINE__)
            member this.SetupToolbar(  hwnd,   ptwt,    pptwth) = err(__LINE__)
            member this.SetForegroundWindow() = err(__LINE__)
            member this.TranslateAcceleratorAsACmd(  pMsg) = err(__LINE__)
            member this.UpdateCommandUI(  fImmediateUpdate) = err(__LINE__)
            member this.UpdateDocDataIsDirtyFeedback(  docCookie,   fDirty) = err(__LINE__)
            member this.RefreshPropertyBrowser(  dispid) = err(__LINE__)
            member this.SetWaitCursor() = err(__LINE__)
            member this.PostExecCommand(   pCmdGroup,   nCmdID,   nCmdexecopt,    pvaIn) = err(__LINE__)
            member this.ShowContextMenu(  dwCompRole,    rclsidActive,   nMenuId,   pos,   pCmdTrgtActive) = err(__LINE__)
            member this.ShowMessageBox(  dwCompRole,    rclsidComp,   pszTitle,   pszText,   pszHelpFile,   dwHelpContextID,   msgbtn,   msgdefbtn,   msgicon,   fSysAlert,    pnResult) = 
                match vsUIShellShowMessageBoxResult with
                | None -> err(__LINE__)
                | Some(result) -> pnResult <- result; VSConstants.S_OK 
            member this.SetMRUComboText(   pCmdGroup,   dwCmdID,   lpszText,   fAddToList) = err(__LINE__)
            member this.SetToolbarVisibleInFullScreen(  pCmdGroup,   dwToolbarId,   fVisibleInFullScreen) = err(__LINE__)
            member this.FindToolWindowEx(  grfFTW,    rPersistenceSlot,   dwToolWinId,    ppWindowFrame : byref<IVsWindowFrame> ) = err(__LINE__)
            member this.GetAppName(   pbstrAppName) = err(__LINE__)
            member this.GetVSSysColor(  dwSysColIndex,    pdwRGBval) = err(__LINE__)
            member this.SetMRUComboTextW(  pCmdGroup,   dwCmdID,   pwszText,   fAddToList) = err(__LINE__)
            member this.PostSetFocusMenuCommand(   pCmdGroup,   nCmdID) = err(__LINE__)
            member this.GetCurrentBFNavigationItem(   ppWindowFrame,    pbstrData,    ppunk) = err(__LINE__)
            member this.AddNewBFNavigationItem(  pWindowFrame,   bstrData,   punk,   fReplaceCurrent) = err(__LINE__)
            member this.OnModeChange(  dbgmodeNew) = err(__LINE__)
            member this.GetErrorInfo(   pbstrErrText) = err(__LINE__)
            member this.GetOpenFileNameViaDlg(  pOpenFileName) = err(__LINE__)
            member this.GetSaveFileNameViaDlg(  pSaveFileName) = err(__LINE__)
            member this.GetDirectoryViaBrowseDlg(  pBrowse) = err(__LINE__)
            member this.CenterDialogOnWindow(  hwndDialog,   hwndParent) = err(__LINE__)
            member this.GetPreviousBFNavigationItem(   ppWindowFrame,    pbstrData,    ppunk) = err(__LINE__)
            member this.GetNextBFNavigationItem(   ppWindowFrame,    pbstrData,    ppunk) = err(__LINE__)
            member this.GetURLViaDlg(  pszDlgTitle,   pszStaticLabel,   pszHelpTopic,    pbstrURL) = err(__LINE__)
            member this.RemoveAdjacentBFNavigationItem(  rdDir) = err(__LINE__)
            member this.RemoveCurrentNavigationDupes(  rdDir) = err(__LINE__)
            }

    // peekhole to IVsTrackProjectDocuments2 - allows to receive notifications about removed files
    type public IVsTrackProjectDocuments2Listener = 
        abstract member OnAfterRemoveFiles: IEvent<IVsProject * int * string[] * VSREMOVEFILEFLAGS[]>


    let vsTrackProjectDocuments2 = 
        let onAfterRemoveFiles = Event<_>()
        let track = { 
            new IVsTrackProjectDocuments2 with            
                member this.BeginBatch() = err(__LINE__)
                member this.EndBatch() = err(__LINE__)
                member this.Flush() = err(__LINE__)
                member this.OnQueryAddFiles(pProject, cFiles, rgpszMkDocuments, rgFlags, pSummaryResult, rgResults) =
                    pSummaryResult.[0] <- VSQUERYADDFILERESULTS.VSQUERYADDFILERESULTS_AddOK
                    0
                member this.OnAfterAddFilesEx( pProject,  cFiles,  rgpszMkDocuments,  rgFlags) = 
                    let proj = (pProject :?> FSharpProjectNode) :> IVsTrackProjectDocumentsEvents2
                    proj.OnAfterAddFilesEx(1, cFiles, [|pProject|], Array.create cFiles 0, rgpszMkDocuments, rgFlags)
                member this.OnAfterAddFiles( pProject,  cFiles,  rgpszMkDocuments) = nothing()
                member this.OnAfterAddDirectoriesEx( pProject,  cDirectories,  rgpszMkDocuments,  rgFlags) = err(__LINE__)
                member this.OnAfterAddDirectories( pProject,  cDirectories,  rgpszMkDocuments) = err(__LINE__)
                member this.OnAfterRemoveFiles(  pProject, cFiles, rgpszMkDocuments,  rgFlags) = 
                    onAfterRemoveFiles.Trigger(pProject, cFiles, rgpszMkDocuments, rgFlags)
                    0
                member this.OnAfterRemoveDirectories(  pProject, cDirectories, rgpszMkDocuments,  rgFlags) = err(__LINE__)
                member this.OnQueryRenameFiles(  pProject, cFiles, rgszMkOldNames, rgszMkNewNames,  rgFlags,  pSummaryResult,  rgResults) = err(__LINE__)
                member this.OnQueryRenameFile(  pProject, pszMkOldName, pszMkNewName,  flags,  outpfRenameCanContinue) =
                    outpfRenameCanContinue <- 1
                    0
                member this.OnAfterRenameFiles(  pProject, cFiles, rgszMkOldNames, rgszMkNewNames,  rgFlags) = err(__LINE__)
                member this.OnAfterRenameFile(  pProject, pszMkOldName, pszMkNewName,  flags) =
                    0
                member this.OnQueryRenameDirectories(  pProject, cDirs, rgszMkOldNames, rgszMkNewNames,  rgFlags,  pSummaryResult,  rgResults) = err(__LINE__)
                member this.OnAfterRenameDirectories(  pProject, cDirs, rgszMkOldNames, rgszMkNewNames,  rgFlags) = err(__LINE__)
                member this.AdviseTrackProjectDocumentsEvents( pEventSink,  pdwCookie) = nothing()
                member this.UnadviseTrackProjectDocumentsEvents( dwCookie) = nothing()
                member this.OnQueryAddDirectories(  pProject, cDirectories, rgpszMkDocuments,  rgFlags,  pSummaryResult,  rgResults) = err(__LINE__)
                member this.OnQueryRemoveFiles(  pProject, cFiles, rgpszMkDocuments,  rgFlags,  pSummaryResult,  rgResults) = 
                    if rgResults <> null then
                        for i = 0 to rgResults.Length-1 do
                            rgResults.[i] <- VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveOK
                    pSummaryResult.[0] <- VSQUERYREMOVEFILERESULTS.VSQUERYREMOVEFILERESULTS_RemoveOK
                    0
                member this.OnQueryRemoveDirectories(  pProject, cDirectories, rgpszMkDocuments,  rgFlags,  pSummaryResult,  rgResults) = err(__LINE__)
                member this.OnAfterSccStatusChanged(  pProject, cFiles, rgpszMkDocuments,  rgdwSccStatus) = err(__LINE__)

            interface IVsTrackProjectDocuments2Listener with
                member this.OnAfterRemoveFiles = onAfterRemoveFiles.Publish
            }
        track 
    
    let vsTaskList() = 
            { new IVsTaskList with
            member this.RegisterTaskProvider( pProvider, pdwProviderCookie : byref<uint32>) = 
                pdwProviderCookie <- 0u
                0
            member this.UnregisterTaskProvider( dwProviderCookie) =
                0
            member this.RefreshTasks( dwProviderCookie) = 
                0
            member this.EnumTaskItems( ppenum : byref<IVsEnumTaskItems> ) = err(__LINE__)
            member this.AutoFilter( cat) = err(__LINE__)
            member this.UpdateProviderInfo( dwProviderCookie) = err(__LINE__)
            member this.SetSilentOutputMode( fSilent) = err(__LINE__)
            member this.DumpOutput( dwReserved,  cat,  pstmOutput,  pfOutputWritten : byref<int> ) = err(__LINE__)
            member this.RegisterCustomCategory( guidCat : byref<Guid>,  dwSortOrder,  pCat) = err(__LINE__)
            member this.UnregisterCustomCategory( catAssigned) = err(__LINE__)
            member this.AutoFilter2( guidCustomView : byref<Guid>) = err(__LINE__)
            }
    let vsMonitorSelection = 
        { new IVsMonitorSelection with
        member this.GetCurrentSelection(ppHier,  pitemid,  ppMIS, ppSC) =
            0
        member this.AdviseSelectionEvents( pSink,  pdwCookie) = 
            0
        member this.UnadviseSelectionEvents( dwCookie) = err(__LINE__)
        member this.GetCurrentElementValue( elementid,  pvarValue) = err(__LINE__)
        member this.GetCmdUIContextCookie( rguidCmdUI,  pdwCmdUICookie) = err(__LINE__)
        member this.IsCmdUIContextActive( dwCmdUICookie,  pfActive) = err(__LINE__)
        member this.SetCmdUIContext( dwCmdUICookie,  fActive) = err(__LINE__)
        }
        
    let vsFileChangeManager =
        { new IVsFileChangeEx with
            member this.AdviseDirChange(pszDir, fWatchSubDir, pFCE, pvsCookie) = err(__LINE__)
            member this.AdviseFileChange(pszMkDocument, grfFilter, pFCE, pvsCookie) = nothing()
            member this.IgnoreFile(vscookie, pszMkDocument, fIgnore) = 
                0
            member this.SyncFile(pszMkDocument) = err(__LINE__)
            member this.UnadviseDirChange(vscookie) = err(__LINE__)
            member this.UnadviseFileChange(vscookie) = nothing()
        }

    let vsSolution =
        { new IVsSolution with
            member x.GetProjectEnum(grfEnumFlags, rguidEnumOnlyThisType, ppenum) = err(__LINE__)
            member x.CreateProject(rguidProjectType, lpszMoniker, lpszLocation, lpszName, grfCreateFlags, iidProject,ppProject) = err(__LINE__)
            member x.GenerateUniqueProjectName(lpszRoot,  outpbstrProjectName) = err(__LINE__)
            member x.GetProjectOfGuid(rguidProjectID,  outppHierarchy) = err(__LINE__)
            member x.GetGuidOfProject(pHierarchy,pguidProjectID) = err(__LINE__)
            member x.GetSolutionInfo( outpbstrSolutionDirectory,  outpbstrSolutionFile,  outpbstrUserOptsFile) = 
                outpbstrSolutionDirectory <- ""
                outpbstrSolutionFile <- ""
                outpbstrUserOptsFile <- ""
                0
            member x.AdviseSolutionEvents( pSink,  outpdwCookie) = err(__LINE__)
            member x.UnadviseSolutionEvents(dwCookie) = err(__LINE__)
            member x.SaveSolutionElement(grfSaveOpts, pHier, docCookie) = err(__LINE__)
            member x.CloseSolutionElement(grfCloseOpts, pHier, docCookie) = err(__LINE__)
            member x.GetProjectOfProjref(pszProjref,  outppHierarchy,  outpbstrUpdatedProjref,   puprUpdateReason) = err(__LINE__)
            member x.GetProjrefOfProject(pHierarchy,  outpbstrProjref) = err(__LINE__)
            member x.GetProjectInfoOfProjref(pszProjref, propid, pvar) = err(__LINE__)
            member x.AddVirtualProject(pHierarchy, grfAddVPFlags) = err(__LINE__)
            member x.GetItemOfProjref(pszProjref,  outppHierarchy,  outpitemid,  outpbstrUpdatedProjref,   puprUpdateReason) = err(__LINE__)
            member x.GetProjrefOfItem(pHierarchy, itemid,  outpbstrProjref) = err(__LINE__)
            member x.GetItemInfoOfProjref(pszProjref, propid, pvar) = err(__LINE__)
            member x.GetProjectOfUniqueName(pszUniqueName,  outppHierarchy) = err(__LINE__)
            member x.GetUniqueNameOfProject(pHierarchy,  outpbstrUniqueName) = err(__LINE__)
            member x.GetProperty(propid, pvar) = err(__LINE__)
            member x.SetProperty(propid,  var) = err(__LINE__)
            member x.OpenSolutionFile(grfOpenOpts, pszFilename) = err(__LINE__)
            member x.QueryEditSolutionFile( outpdwEditResult) = err(__LINE__)
            member x.CreateSolution(lpszLocation, lpszName, grfCreateFlags) = err(__LINE__)
            member x.GetProjectFactory(dwReserved, pguidProjectType, pszMkProject, ppProjectFactory) = err(__LINE__)
            member x.GetProjectTypeGuid(dwReserved, pszMkProject,pguidProjectType) = err(__LINE__)
            member x.OpenSolutionViaDlg(pszStartDirectory, fDefaultToAllProjectsFilter) = err(__LINE__)
            member x.AddVirtualProjectEx(pHierarchy, grfAddVPFlags, rguidProjectID) = err(__LINE__)
            member x.QueryRenameProject( pProject, pszMkOldName, pszMkNewName, dwReserved, pfRenameCanContinue) = err(__LINE__)
            member x.OnAfterRenameProject( pProject, pszMkOldName, pszMkNewName, dwReserved) = err(__LINE__)
            member x.RemoveVirtualProject(pHierarchy, grfRemoveVPFlags) = err(__LINE__)
            member x.CreateNewProjectViaDlg(pszExpand, pszSelect, dwReserved) = err(__LINE__)
            member x.GetVirtualProjectFlags(pHierarchy,  outpgrfAddVPFlags) = err(__LINE__)
            member x.GenerateNextDefaultProjectName(pszBaseName, pszLocation,  outpbstrProjectName) = err(__LINE__)
            member x.GetProjectFilesInSolution(grfGetOpts, cProjects, rgbstrProjectNames,  outpcProjectsFetched) = err(__LINE__)
            member x.CanCreateNewProjectAtLocation(fCreateNewSolution, pszFullProjectFilePath, pfCanCreate) = err(__LINE__)
        }

    let dummyEmptyIEnumRunningDocuments =
        {new IEnumRunningDocuments with
            member ierd.Clone(ppenum) = err(__LINE__)
            member ierd.Next(celt, rgelt, pceltFetched) = 
                pceltFetched <- 0u
                VSConstants.S_FALSE 
            member ierd.Reset() =
                0
            member ierd.Skip(celt) = err(__LINE__)
        }
    let mutable vsRunningDocumentTableFindAndLockDocumentVsHierarchyMock = null : IVsHierarchy
    let mutable vsRunningDocumentTableNextRenameDocumentCallThrows = false
    
    let vsRunningDocumentTable =
        {new IVsRunningDocumentTable with
            member rdt.RegisterAndLockDocument(grfRDTLockType, pszMkDocument, pHier, itemid, punkDocData, pdwCookie) = err(__LINE__)
            member rdt.LockDocument(grfRDTLockType, dwCookie) = err(__LINE__)
            member rdt.UnlockDocument(grfRDTLockType, dwCookie) = err(__LINE__)
            member rdt.FindAndLockDocument(dwRDTLockType, pszMkDocument, ppHier, pitemid, ppunkDocData, pdwCookie) =
                ppHier <- vsRunningDocumentTableFindAndLockDocumentVsHierarchyMock
                0
            member rdt.RenameDocument(pszMkDocumentOld, pszMkDocumentNew, pHier, itemidNew) =
                if vsRunningDocumentTableNextRenameDocumentCallThrows then
                    vsRunningDocumentTableNextRenameDocumentCallThrows <- false
                    VSConstants.E_FAIL
                else
                    0
            member rdt.AdviseRunningDocTableEvents(pSink, pdwCookie) = err(__LINE__)
            member rdt.UnadviseRunningDocTableEvents(dwCookie) = err(__LINE__)
            member rdt.GetDocumentInfo(docCookie, pgrfRDTFlags, pdwReadLocks, pdwEditLocks, pbstrMkDocument, ppHier, pitemid, ppunkDocData) = err(__LINE__)
            member rdt.NotifyDocumentChanged(dwCookie, grfDocChanged) = err(__LINE__)
            member rdt.NotifyOnAfterSave(dwCookie) = err(__LINE__)
            member rdt.GetRunningDocumentsEnum(ppenum) = 
                ppenum <- dummyEmptyIEnumRunningDocuments  // just lie, we don't mock enough of this
                0
            member rdt.SaveDocuments(grfSaveOpts, pHier, itemid, docCookie) = err(__LINE__)
            member rdt.NotifyOnBeforeSave(dwCookie) = err(__LINE__)
            member rdt.RegisterDocumentLockHolder(grfRDLH, dwCookie, pLockHolder, pdwLHCookie) = err(__LINE__)
            member rdt.UnregisterDocumentLockHolder(dwLHCookie) = err(__LINE__)
            member rdt.ModifyDocumentFlags(docCookie, grfFlags, fSet) = err(__LINE__) 
        }

    let MockVsBuildManagerAccessor() =
        let muxLogger = ref Unchecked.defaultof<MuxLogger>
        { new Microsoft.VisualStudio.Shell.Interop.IVsBuildManagerAccessor with
            member x.RegisterLogger(submissionId, logger : obj) =
                let iLogger = logger :?> ILogger 
                (!muxLogger).Add(iLogger)
                0
            member x.ClaimUIThreadForBuild() = 0
            member x.ReleaseUIThreadForBuild() = 0
            member x.UnregisterLoggers(submissionId) = 0
            member x.BeginDesignTimeBuild() =
                muxLogger := new MuxLogger()
                let buildParameters = new BuildParameters(Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection)
                buildParameters.Loggers <- ([ ((!muxLogger) :> ILogger) ] :> System.Collections.Generic.IEnumerable<ILogger>)
                buildParameters.HostServices <- Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.HostServices
                BuildManager.DefaultBuildManager.BeginBuild(buildParameters)
                0
            member x.EndDesignTimeBuild() =
                BuildManager.DefaultBuildManager.EndBuild()
                muxLogger := Unchecked.defaultof<MuxLogger>
                0
            member x.GetCurrentBatchBuildId(batchId) = 
                batchId <- 0u // in the product, this would be non-zero if there are any in-progress builds, but we are not mocking solution manager, so we just always report no build in progress for unit tests
                0
            member x.GetSolutionConfiguration(rootProject: obj, xmlFragment: byref<string>) =
                xmlFragment <- ""
                0
            member x.Escape(unescapedValue: string, escapedValue: byref<string>) = 0
            member x.Unescape(escapedValue: string, unescapedValue: byref<string>) = 0
        }

    let vsTrackProjectRetargeting =
        // This very simple mock will just do nothing but assert that the cookies
        // match up for advise/unadvise
        let _cookie = ref 0u
        { new IVsTrackProjectRetargeting with
            member this.OnSetTargetFramework(hier, currentFx, newFx, callback, reload) = 0
            member this.AdviseTrackProjectRetargetingEvents(sink, cookie) =
                _cookie := cookie
                0
            member this.UnadviseTrackProjectRetargetingEvents(cookie) =
                Debug.Assert((!_cookie = cookie), sprintf "Invalid cookie for advise/unadvise!")
                0
            member this.AdviseTrackBatchRetargetingEvents(sink, cookie) = 0
            member this.UnadviseTrackBatchRetargetingEvents(cookie) = 0    
            member this.BeginRetargetingBatch() = 0
            member this.BatchRetargetProject(hier, newFx, unloadProjectIfErrorOrCancel) = 0
            member this.EndRetargetingBatch() = 0
        }

    let vsTargetFrameworkAssembliesN (n:uint32) =
        { new IVsTargetFrameworkAssemblies with
            member this.GetSupportedFrameworks(pTargetFrameworks) = notimpl()
            member this.GetTargetFrameworkDescription(targetVersion, pszDescription) = notimpl()
            member this.GetSystemAssemblies(targetVersion, pAssemblies) = notimpl()
            member this.IsSystemAssembly(szAssemblyFile, pIsSystem, pTargetFrameworkVersion) = notimpl()
            member this.GetRequiredTargetFrameworkVersion(szAssemblyFile, pTargetFrameworkVersion) = notimpl()
            member this.GetRequiredTargetFrameworkVersionFromDependency(szAssemblyFile, pTargetFrameworkVersion) =
                pTargetFrameworkVersion <- n
                0
        }

    let vsTargetFrameworkAssemblies20 = vsTargetFrameworkAssembliesN 0x20000u
    let vsTargetFrameworkAssemblies30 = vsTargetFrameworkAssembliesN 0x30000u
    let vsTargetFrameworkAssemblies35 = vsTargetFrameworkAssembliesN 0x30005u
    let vsTargetFrameworkAssemblies40 = vsTargetFrameworkAssembliesN 0x40000u
    let vsTargetFrameworkAssemblies45 = vsTargetFrameworkAssembliesN 0x40005u
    let vsTargetFrameworkAssemblies46 = vsTargetFrameworkAssembliesN 0x40006u
    
    let vsFrameworkMultiTargeting =
        { new IVsFrameworkMultiTargeting with
            member this.GetInstallableFrameworkForTargetFx(fx, res) =
                let fn = new System.Runtime.Versioning.FrameworkName(fx)
                if String.IsNullOrEmpty(fn.Profile) then
                    res <- fn.FullName
                else
                    res <- fn.Profile
                0
            member this.IsReferenceableInTargetFx(a,b,c) = notimpl()
            member this.GetTargetFramework(a,b,c) = notimpl()
            member this.GetSupportedFrameworks(a) = notimpl()
            member this.GetFrameworkAssemblies(a,b,c) = notimpl()
            member this.CheckFrameworkCompatibility(a,b,c) = notimpl()
            member this.ResolveAssemblyPath(a,b,c) = notimpl()
            member this.GetDisplayNameForTargetFx(a,b) = notimpl()
            member this.ResolveAssemblyPathsInTargetFx(a,b,c,d,e) = notimpl()
        }
        
    // This provides a mock for LocalRegistry because for the multitargeting
    // code we use this to create a text buffer
        
    let vsLocalRegistry f =
        { new ILocalRegistry3 with
            member this.CreateInstance(a,b,c,d,e) =
                e <- Marshal.GetIUnknownForObject(f())
                0
            member this.GetTypeLibOfClsid(a,b) = notimpl()
            member this.GetClassObjectOfClsid(a,b,c,d,e) = notimpl()
            member this.GetLocalRegistryRoot(a) =
                a <- ""
                0
            member this.CreateManagedInstance(a,b,c,d,e) = notimpl()
            member this.GetClassObjectOfManagedClass(a,b,c,d,e) = notimpl()
            
          interface ILocalRegistry2 with
            member this.CreateInstance(a,b,c,d,e) = notimpl()
            member this.GetTypeLibOfClsid(a,b) = notimpl()
            member this.GetClassObjectOfClsid(a,b,c,d,e) = notimpl()
            member this.GetLocalRegistryRoot(a) =
                a <- ""
                0
            
          interface ILocalRegistry with
            member this.CreateInstance(a,b,c,d,e) = notimpl()
            member this.GetTypeLibOfClsid(a,b) = notimpl()
            member this.GetClassObjectOfClsid(a,b,c,d,e) = notimpl()            
        }

    let MakeVsSolutionBuildManagerAndConfigChangeNotifier() =
        let mkEventsStorage () = 
            let listeners = Dictionary()
            let id = ref 0u
            let add l = 
                let cookie = !id
                listeners.Add(cookie, l)
                id := !id + 1u
                cookie
            let remove v =
                listeners.Remove(v) |> ignore
            let enumerate() = listeners.Values
            add, remove, enumerate

        let add1, remove1, enumerate1 = mkEventsStorage()
        let add2, remove2, _ = mkEventsStorage()
        let add4, remove4, _ = mkEventsStorage()
        let configDict = new Dictionary<IVsHierarchy,string>()
        let configChangeNotifier(h : IVsHierarchy, s : string) = 
            if configDict.ContainsKey(h) then
                configDict.[h] <- s
            else
                configDict.Add(h,s)
            for (kvp : IVsUpdateSolutionEvents) in enumerate1() do
                kvp.OnActiveProjectCfgChange(h) |> ignore 
        let vsSolutionBuildManager = 
            { new IVsSolutionBuildManager with
                member x.DebugLaunch(grfLaunch) = err(__LINE__)
                member x.StartSimpleUpdateSolutionConfiguration(dwFlags, dwDefQueryResults,   fSuppressUI) = err(__LINE__)
                member x.AdviseUpdateSolutionEvents(  pIVsUpdateSolutionEvents,  outpdwCookie : byref<uint32> ) =
                    outpdwCookie <- add1 pIVsUpdateSolutionEvents 
                    0
                member x.UnadviseUpdateSolutionEvents(dwCookie) =
                    remove1 dwCookie
                    0
                member x.UpdateSolutionConfigurationIsActive(pfIsActive) = err(__LINE__)
                member x.CanCancelUpdateSolutionConfiguration(pfCanCancel) = err(__LINE__)
                member x.CancelUpdateSolutionConfiguration() = err(__LINE__)
                member x.QueryDebugLaunch(grfLaunch, pfCanLaunch) = err(__LINE__)
                member x.QueryBuildManagerBusy(pfBuildManagerBusy) = err(__LINE__)
                member x.FindActiveProjectCfg( pvReserved1,  pvReserved2,   pIVsHierarchy_RequestedProject,   ppIVsProjectCfg_Active) =
                    let mutable s = ""
                    if not(configDict.TryGetValue(pIVsHierarchy_RequestedProject, &s)) then
                        s <- ""
                    let (_, vsCfgProvider : IVsCfgProvider) =(pIVsHierarchy_RequestedProject :?> IVsGetCfgProvider).GetCfgProvider()
                    let cfgName = ConfigCanonicalName(s)
                    let (_, cfg : IVsCfg) = (vsCfgProvider :?> IVsCfgProvider2).GetCfgOfName(cfgName.ConfigName, cfgName.Platform)
                    ppIVsProjectCfg_Active.[0] <- downcast cfg 
                    0                    
                member x.get_IsDebug(pfIsDebug) = err(__LINE__)
                member x.put_IsDebug(  fIsDebug) = err(__LINE__)
                member x.get_CodePage( outpuiCodePage) = err(__LINE__)
                member x.put_CodePage(uiCodePage) = err(__LINE__)
                member x.StartSimpleUpdateProjectConfiguration(  pIVsHierarchyToBuild,   pIVsHierarchyDependent,   pszDependentConfigurationCanonicalName, dwFlags, dwDefQueryResults,   fSuppressUI) = err(__LINE__)
                member x.get_StartupProject( ppHierarchy) = err(__LINE__)
                member x.set_StartupProject(  pHierarchy) = err(__LINE__)
                member x.GetProjectDependencies(  pHier, celt,   rgpHier, pcActual) = err(__LINE__)
              interface IVsSolutionBuildManager2 with
                member x.AdviseUpdateSolutionEvents(pIVsUpdateSolutionEvents, pdwCookie) =
                    pdwCookie <- add2 pIVsUpdateSolutionEvents
                    0
                member x.CalculateProjectDependencies() = err(__LINE__)
                member x.CanCancelUpdateSolutionConfiguration(_pfCanCancel) = err(__LINE__)
                member x.CancelUpdateSolutionConfiguration() = err(__LINE__)
                member x.DebugLaunch(_grfLaunch) = err(__LINE__)
                member x.FindActiveProjectCfg(_pvReserved1, _pvReserved2, _pIVsHierarchy_RequestedProject, _ppIVsProjectCfg_Active) = err(__LINE__)
                member x.GetProjectDependencies(_pHier, _celt, _rgpHier, _pcActual) = err(__LINE__)
                member x.QueryBuildManagerBusy(_pfBuildManagerBusy) = err(__LINE__)
                member x.QueryDebugLaunch(_grfLaunch, _pfCanLaunch) = err(__LINE__)
                member x.QueryProjectDependency(_pHier, _pHierDependentOn, _pfIsDependentOn) = err(__LINE__)
                member x.SaveDocumentsBeforeBuild(_pHier, _itemid, _docCookie) = err(__LINE__)
                member x.StartSimpleUpdateProjectConfiguration(_pIVsHierarchyToBuild, _pIVsHierarchyDependent, _pszDependentConfigurationCanonicalName, _dwFlags, _dwDefQueryResults, _fSuppressUI) = err(__LINE__)
                member x.StartSimpleUpdateSolutionConfiguration(_dwFlags, _dwDefQueryResults, _fSuppressUI) = err(__LINE__)
                member x.StartUpdateProjectConfigurations(_cProjs, _rgpHierProjs, _dwFlags, _fSuppressUI) = err(__LINE__)
                member x.StartUpdateSpecificProjectConfigurations(_cProjs, _rgpHier, _rgpcfg, _rgdwCleanFlags, _rgdwBuildFlags, _rgdwDeployFlags, _dwFlags, _fSuppressUI) = err(__LINE__)
                member x.UnadviseUpdateSolutionEvents(dwCookie) = 
                    remove2 dwCookie
                    0
                member x.UpdateSolutionConfigurationIsActive(_pfIsActive) = err(__LINE__)
                member x.get_CodePage(_puiCodePage) = err(__LINE__)
                member x.get_IsDebug(_pfIsDebug) = err(__LINE__)
                member x.get_StartupProject(_ppHierarchy) = err(__LINE__)
                member x.put_CodePage(_uiCodePage) = err(__LINE__)
                member x.put_IsDebug(_fIsDebug) = err(__LINE__)
                member x.set_StartupProject(_pHierarchy) = err(__LINE__)
              interface IVsSolutionBuildManager3 with
                member x.AdviseUpdateSolutionEvents3(a,b) =
                    0
                member x.AreProjectsUpToDate(a) = err(__LINE__)
                member x.HasHierarchyChangedSinceLastDTEE () = err(__LINE__)
                member x.QueryBuildManagerBusyEx(a) = err(__LINE__)
                member x.UnadviseUpdateSolutionEvents3(a) =
                    0
              interface IVsSolutionBuildManager5 with
                member x.AdviseUpdateSolutionEvents4(pIVsUpdateSolutionEvents, pdwCookie) =
                    pdwCookie <- add4 pIVsUpdateSolutionEvents
                member x.AdviseUpdateSolutionEventsAsync(a,b) = err(__LINE__) |> ignore
                member x.FindActiveProjectCfgName(_projectGuid,outCfgString) = 
                    // For now ignore the _projectGuid and just return the last notified configuration
                    match configDict |> Seq.tryHead with 
                    | None -> err(__LINE__) 
                    | Some (KeyValue(_,v)) -> 
                        outCfgString <- v
                        0

                member x.UnadviseUpdateSolutionEventsAsync(a) = err(__LINE__) |> ignore
                member x.UnadviseUpdateSolutionEvents4(dwCookie) =
                    remove4 dwCookie
        }
        vsSolutionBuildManager, configChangeNotifier
    
    let vsThreadedWaitDialogFactory =
        { new IVsThreadedWaitDialogFactory with
            override x.CreateInstance(vsThreadedWaitDialog) =
                vsThreadedWaitDialog <-
                    { new IVsThreadedWaitDialog2 with
                        override x.EndWaitDialog(_) = 0
                        override x.HasCanceled(_) = 0
                        override x.StartWaitDialog(_, _, _, _, _, _, _, _) = 0
                        override x.StartWaitDialogWithPercentageProgress(_, _, _, _, _, _, _, _, _) = 0
                        override x.UpdateProgress(_, _, _, _, _, _, _) = 0
                    }
                0
        }
        
    let MakeMockServiceProviderAndConfigChangeNotifierNoTargetFrameworkAssembliesService() =
        let vsSolutionBuildManager, configChangeNotifier = MakeVsSolutionBuildManagerAndConfigChangeNotifier()
        let sp = new OleServiceProvider()

        // allows to receive notifications about called methods of IVsTrackProjectDocuments2 interface
        sp.AddService(typeof<IVsTrackProjectDocuments2Listener>, box vsTrackProjectDocuments2, false)

        sp.AddService(typeof<SVsTrackProjectDocuments>, box vsTrackProjectDocuments2, false)
        sp.AddService(typeof<SVsShell>, box (vsShell()), false)
        sp.AddService(typeof<SVsUIShell>, box vsUIShell, false)
        sp.AddService(typeof<SVsTaskList>, box(vsTaskList()), false) 
        sp.AddService(typeof<SVsShellMonitorSelection>, box vsMonitorSelection, false) 
        sp.AddService(typeof<SVsFileChangeEx>, box vsFileChangeManager, false)
        sp.AddService(typeof<SVsSolution>, box vsSolution, false)
        sp.AddService(typeof<SVsSolutionBuildManager>, box vsSolutionBuildManager, false)
        sp.AddService(typeof<SVsRunningDocumentTable>, box vsRunningDocumentTable, false)
        sp.AddService(typeof<Microsoft.VisualStudio.Shell.Interop.SVsBuildManagerAccessor>, box (MockVsBuildManagerAccessor()), false)
        sp.AddService(typeof<SVsTrackProjectRetargeting>, box vsTrackProjectRetargeting, false)
        sp.AddService(typeof<SVsThreadedWaitDialogFactory>, box vsThreadedWaitDialogFactory, false)
        sp, configChangeNotifier

    let MakeMockServiceProviderAndConfigChangeNotifier20() =
        let sp, ccn = MakeMockServiceProviderAndConfigChangeNotifierNoTargetFrameworkAssembliesService()
        sp.AddService(typeof<SVsTargetFrameworkAssemblies>, box vsTargetFrameworkAssemblies20, false)
        sp.AddService(typeof<SVsFrameworkMultiTargeting>, box vsFrameworkMultiTargeting, false)
        sp, ccn
    let MakeMockServiceProviderAndConfigChangeNotifier30() =
        let sp, ccn = MakeMockServiceProviderAndConfigChangeNotifierNoTargetFrameworkAssembliesService()
        sp.AddService(typeof<SVsTargetFrameworkAssemblies>, box vsTargetFrameworkAssemblies30, false)
        sp.AddService(typeof<SVsFrameworkMultiTargeting>, box vsFrameworkMultiTargeting, false)
        sp, ccn
    let MakeMockServiceProviderAndConfigChangeNotifier35() =
        let sp, ccn = MakeMockServiceProviderAndConfigChangeNotifierNoTargetFrameworkAssembliesService()
        sp.AddService(typeof<SVsTargetFrameworkAssemblies>, box vsTargetFrameworkAssemblies35, false)
        sp.AddService(typeof<SVsFrameworkMultiTargeting>, box vsFrameworkMultiTargeting, false)
        sp, ccn
    let MakeMockServiceProviderAndConfigChangeNotifier40() =
        let sp, ccn = MakeMockServiceProviderAndConfigChangeNotifierNoTargetFrameworkAssembliesService()
        sp.AddService(typeof<SVsTargetFrameworkAssemblies>, box vsTargetFrameworkAssemblies40, false)
        sp.AddService(typeof<SVsFrameworkMultiTargeting>, box vsFrameworkMultiTargeting, false)
        sp, ccn
    let MakeMockServiceProviderAndConfigChangeNotifier45() =
        let sp, ccn = MakeMockServiceProviderAndConfigChangeNotifierNoTargetFrameworkAssembliesService()
        sp.AddService(typeof<SVsTargetFrameworkAssemblies>, box vsTargetFrameworkAssemblies45, false)
        sp.AddService(typeof<SVsFrameworkMultiTargeting>, box vsFrameworkMultiTargeting, false)
        sp, ccn
    let MakeMockServiceProviderAndConfigChangeNotifier46() =
        let sp, ccn = MakeMockServiceProviderAndConfigChangeNotifierNoTargetFrameworkAssembliesService()
        sp.AddService(typeof<SVsTargetFrameworkAssemblies>, box vsTargetFrameworkAssemblies46, false)
        sp.AddService(typeof<SVsFrameworkMultiTargeting>, box vsFrameworkMultiTargeting, false)
        sp, ccn

    // This is the mock thing that all tests, except the multitargeting tests call.
    // By default, let it use the 4.0 assembly version.

    let MakeMockServiceProviderAndConfigChangeNotifier() =
        MakeMockServiceProviderAndConfigChangeNotifier40()

    let mockServiceProvider = 
        let sp, _ = MakeMockServiceProviderAndConfigChangeNotifier()
        sp 
            
    let vsOutputWindowPane(owpe : string list ref) = 
        { new IVsOutputWindowPane with
            member this.Activate () = err(__LINE__)
            member this.Clear () = owpe := []; 0
            member this.FlushToTaskList () = VSConstants.S_OK
            member this.GetName(pbstrPaneName) = err(__LINE__)
            member this.Hide () = err(__LINE__)
            member this.OutputString(pszOutputString) = owpe := pszOutputString :: !owpe ; 0
            member this.OutputStringThreadSafe(pszOutputString) = owpe := pszOutputString :: !owpe ; 0
            member this.OutputTaskItemString(pszOutputString, nPriority, nCategory, pszSubcategory, nBitmap, pszFilename, nLineNum, pszTaskItemText) = err(__LINE__)
            member this.OutputTaskItemStringEx(pszOutputString, nPriority, nCategory, pszSubcategory, nBitmap, pszFilename, nLineNum, pszTaskItemText, pszLookupKwd) = err(__LINE__)
            member this.SetName(pszPaneName) = err(__LINE__)
        }            

module internal VsActual = 
    // Since the editor exports MEF components, we can use those components directly from unit tests without having to load too many heavy
    // VS assemblies.  Use editor MEF components directly from the VS product.

    open System.Reflection
    open System.IO
    open System.ComponentModel.Composition.Hosting
    open System.ComponentModel.Composition.Primitives
    open Microsoft.VisualStudio.Text
    open Microsoft.VisualStudio.Threading

    type TestExportJoinableTaskContext () =

        static let jtc = new JoinableTaskContext()

        [<System.ComponentModel.Composition.Export(typeof<JoinableTaskContext>)>]
        member public _.JoinableTaskContext : JoinableTaskContext = jtc

    let vsInstallDir =
        // use the environment variable to find the VS installdir
        let vsvar =
            let var =
                let v = Environment.GetEnvironmentVariable("VS170COMNTOOLS")
                if String.IsNullOrEmpty v then
                    Environment.GetEnvironmentVariable("VS160COMNTOOLS")
                else
                    v

            if String.IsNullOrEmpty var then
                Environment.GetEnvironmentVariable("VSAPPIDDIR")
            else
                var
        if String.IsNullOrEmpty vsvar then failwith "VS170COMNTOOLS and VSAPPIDDIR environment variables not found."
        Path.Combine(vsvar, "..")

    let CreateEditorCatalog() =
        let thisAssembly = Assembly.GetExecutingAssembly().Location
        let thisAssemblyDir = Path.GetDirectoryName(thisAssembly)
        let list = new ResizeArray<ComposablePartCatalog>()
        let add p =
            let fullPath = Path.GetFullPath(Path.Combine(thisAssemblyDir, p))
            if File.Exists(fullPath) then
                list.Add(new AssemblyCatalog(fullPath))
            else
                
                failwith <| sprintf "unable to find assembly '%s' in location '%s'" p thisAssemblyDir

        list.Add(new AssemblyCatalog(thisAssembly))
        [ "Microsoft.VisualStudio.Text.Data.dll"
          "Microsoft.VisualStudio.Text.Logic.dll"
          "Microsoft.VisualStudio.Text.Internal.dll"
          "Microsoft.VisualStudio.Text.UI.dll"
          "Microsoft.VisualStudio.Text.UI.Wpf.dll"
          "Microsoft.VisualStudio.Threading.dll"
          "Microsoft.VisualStudio.Platform.VSEditor.dll"
          "Microsoft.VisualStudio.Editor.dll"
          "Microsoft.VisualStudio.ComponentModelHost.dll"
          "Microsoft.VisualStudio.Shell.15.0.dll" ]
        |> List.iter add
        new AggregateCatalog(list)

    let exportProvider = new CompositionContainer(new AggregateCatalog(CreateEditorCatalog()), true, null)
    let iTextBufferFactoryService = exportProvider.GetExportedValue<ITextBufferFactoryService>()
    let createTextBuffer(text:string) = iTextBufferFactoryService .CreateTextBuffer(text, iTextBufferFactoryService .TextContentType)
