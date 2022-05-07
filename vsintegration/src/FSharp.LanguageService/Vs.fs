// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.TextManager.Interop
open FSharp.Compiler.Text
open System.Runtime.InteropServices

/// Helper methods for interoperating with COM                
module internal Com = 
    let ThrowOnFailure0(hr) = 
        ErrorHandler.ThrowOnFailure(hr)  |> ignore
        
    let ThrowOnFailure1(hr,res) = 
        ErrorHandler.ThrowOnFailure(hr) |> ignore; 
        res
        
    let ThrowOnFailure2(hr,res1,res2) = 
        ErrorHandler.ThrowOnFailure(hr) |> ignore; 
        res1,res2
        
    let ThrowOnFailure3(hr,res1,res2,res3) = 
        ErrorHandler.ThrowOnFailure(hr) |> ignore; 
        res1,res2,res3

    let ThrowOnFailure4(hr,res1,res2,res3,res4) = 
        ErrorHandler.ThrowOnFailure(hr) |> ignore; 
        res1,res2,res3,res4
        
    let Succeeded hr = 
        // REVIEW: Not the correct check for succeeded
        hr = VSConstants.S_OK
        

/// Methods for dealing with IVsHierarchy
module internal VsHierarchy = 
    let private getItemId (o:obj) : uint32 =
        let r:int32 = (unbox o)
        (uint32)r
    
    let rec private selfAndSiblings (hier:IVsHierarchy) (itemid:uint32) = 
        seq {
            yield itemid
            let hr, sibling = hier.GetProperty(itemid, int32 __VSHPROPID.VSHPROPID_NextSibling)
            if Com.Succeeded hr then yield! selfAndSiblings hier (getItemId sibling) 
        } 

    let private VirtualFolderGuid = new Guid("6bb5f8f0-4483-11d3-8bcf-00c04f8ec28c")
    
    let isVirtualFolder (hier:IVsHierarchy) (itemid:uint32) =
        let hr, guid = hier.GetGuidProperty(itemid, int32 __VSHPROPID.VSHPROPID_TypeGuid)
        Com.Succeeded hr && guid = VirtualFolderGuid
        
    /// Get the path+file name for a particular item, return empty string if none
    let filenameOrEmpty (hier:IVsHierarchy) (itemid:uint32) = 
        let hr, canonicalName = hier.GetCanonicalName(itemid)
        if Com.Succeeded hr then canonicalName else System.String.Empty
        
    /// Get the typename for a particular item, return empty string if none
    let typeName (hier:IVsHierarchy) (itemid:uint32) = 
        let hr, typename = hier.GetProperty(itemid, int32 __VSHPROPID.VSHPROPID_TypeName)
        if Com.Succeeded hr then (typename :?> System.String) else System.String.Empty        

    /// Get the name for a particular item, return empty string if none
    let name (hier:IVsHierarchy) (itemid:uint32) = 
        let hr, typename = hier.GetProperty(itemid, int32 __VSHPROPID.VSHPROPID_Name)
        if Com.Succeeded hr then (typename :?> System.String) else System.String.Empty  
        
    /// Get the projectDirectory for a particular item, return empty string if none
    let projectDirectory (hier:IVsHierarchy) (itemid:uint32) = 
        let hr, typename = hier.GetProperty(itemid, int32 __VSHPROPID.VSHPROPID_ProjectDir)
        if Com.Succeeded hr then (typename :?> System.String) else System.String.Empty  
        
    /// Owner key string that identifies the project GUID of the owning project.
    let ownerkey (hier:IVsHierarchy) (itemid:uint32) = 
        let hr, typename = hier.GetProperty(itemid, int32 __VSHPROPID.VSHPROPID_OwnerKey)
        if Com.Succeeded hr then (typename :?> System.String) else System.String.Empty            
        
    /// All the children itemids ov the given item
    let children (hier:IVsHierarchy) (itemid:uint32) = 
        seq {
            let hr, child = hier.GetProperty(itemid, int32 __VSHPROPID.VSHPROPID_FirstChild)
            if Com.Succeeded hr then yield! selfAndSiblings hier (getItemId child) 
        }

    /// Flatten the hierarchy starting at a particular item and return all itemids
    let rec flattenChild (hier:IVsHierarchy) (itemid:uint32) = 
        seq {
                for sibling in selfAndSiblings hier itemid do
                    yield sibling 
                    yield! children hier sibling
        }
        
    /// Flatten starting at the root of the hierarchy
    let flatten (hier:IVsHierarchy) = flattenChild hier VSConstants.VSITEMID_ROOT

[<AutoOpen>]
module internal ServiceProviderExtensions =
    type internal System.IServiceProvider with 
        member sp.GetService<'S,'T>() = sp.GetService(typeof<'S>) :?> 'T

        member sp.TextManager = sp.GetService<SVsTextManager, IVsTextManager>()
        member sp.RunningDocumentTable = sp.GetService<SVsRunningDocumentTable, IVsRunningDocumentTable>()
        member sp.XmlService = sp.GetService<SVsXMLMemberIndexService, IVsXMLMemberIndexService>()
        member sp.DTE = sp.GetService<SDTE, EnvDTE.DTE>()

/// Isolate VsTextManager as much as possible to ease transition into new editor architecture
module internal VsTextManager =

    /// Get the current active view
    let GetActiveView (textManager:IVsTextManager) (mustHaveFocus:bool) buffer = Com.ThrowOnFailure1(textManager.GetActiveView((if mustHaveFocus then 1 else 0), buffer))
    

/// Isolate IVsTextColorState as much as possible to ease transition into new editor architecture
module internal VsTextColorState =

    /// Recolorize the given lines
    let ReColorizeLines (colorState:IVsTextColorState) topLine bottomLine = 
        Com.ThrowOnFailure0(colorState.ReColorizeLines(topLine,bottomLine))

    /// May trigger a recolorization.
    let GetColorStateAtStartOfLine (colorState:IVsTextColorState) line =
        Com.ThrowOnFailure1(colorState.GetColorStateAtStartOfLine(line))
    
/// Isolate IVsTextView as much as possible to ease transition into new editor architecture
module internal VsTextView =

    /// Get the scroll info
    let GetScrollInfo (view:IVsTextView) code = Com.ThrowOnFailure4(view.GetScrollInfo(code))

    /// Get the layered view
    let LayeredTextView (view:IVsTextView) : IVsLayeredTextView = unbox(box(view))

    /// Get the buffer for this view.
    let Buffer (view:IVsTextView) = Com.ThrowOnFailure1(view.GetBuffer())
    
/// Isolate IVsColorState as much as possible to ease transition into new editor architecture
module internal VsColorState = 

    /// Recolorize the given lines
    let RecolorizeLines (cs:IVsTextColorState) top bottom =
        Com.ThrowOnFailure0(cs.ReColorizeLines(top,bottom))
        
module internal VsUserData = 

    let vsBufferMoniker = Guid("978A8E17-4DF8-432A-9623-D530A26452BC")

    // This is the file name of the buffer.
    let GetBufferMonker(ud:IVsUserData) : string = 
        downcast Com.ThrowOnFailure1(ud.GetData(ref vsBufferMoniker))
       
/// Isolate IVsTextLines as much as possible to ease transition into new editor architecture
module internal VsTextLines =

    /// Get the length of the given line.
    let LengthOfLine (buffer:IVsTextBuffer) (line:int) : int = 
        Com.ThrowOnFailure1(buffer.GetLengthOfLine(line))

    /// Get the text for a particular line.
    let LineText (buffer:IVsTextLines) line = 
        Com.ThrowOnFailure1(buffer.GetLineText(line, 0, line, LengthOfLine buffer line))

    /// Get the color state
    let TextColorState (buffer:IVsTextLines) : IVsTextColorState= unbox(box(buffer))

    /// Get the filename of the given buffer (via IVsUserData). Not all buffers have a file. This will be an exception.
    let GetFilename(buffer : IVsTextLines) =
        let ud = (box buffer) :?> IVsUserData
        VsUserData.GetBufferMonker(ud)

    /// Get the string contents of a given buffer (the current snapshot).
    let GetFileContents(buffer: IVsTextBuffer, editorAdaptersFactoryService: IVsEditorAdaptersFactoryService) =
        let dataBuffer = editorAdaptersFactoryService.GetDataBuffer(buffer)
        dataBuffer.CurrentSnapshot.GetText()
    

module internal VsRunningDocumentTable = 

    let FindDocumentWithoutLocking(rdt:IVsRunningDocumentTable, url:string) : (IVsHierarchy * IVsTextLines) option =
        let (hr:int, hier:IVsHierarchy, _itemid:uint32, unkData:IntPtr, _cookie:uint32) = rdt.FindAndLockDocument(uint32 _VSRDTFLAGS.RDT_NoLock, url)
        try
            if Com.Succeeded(hr) then 
                let bufferObject = 
                    if unkData=IntPtr.Zero then null
                    else Marshal.GetObjectForIUnknown(unkData)
                let buffer = 
                    match bufferObject with 
                    | :? IVsTextLines as tl -> tl
                    | _ -> null
                Some(hier, buffer)
            else None
        finally 
            if IntPtr.Zero <> unkData then Marshal.Release(unkData)|>ignore
            
        
[<AutoOpen>]
module internal TextSpanHelpers =

    let TextSpanOfRange (r1:range) =
        let sc = r1.StartColumn
        let sl = r1.StartLine 
        let ec = r1.EndColumn
        let el = r1.EndLine
        TextSpan(iStartLine = sl-1, iStartIndex = sc, iEndLine = el-1, iEndIndex = ec)

    let MakeSpan(ss:ITextSnapshot, sl, sc, el, ec) =
        let makeSnapshotPoint l c =
            let lineNum, fsharpRangeIsPastEOF =
                if l <= ss.LineCount - 1 then
                    l, false
                else
                    ss.LineCount - 1, true
            let line = ss.GetLineFromLineNumber(lineNum)
            line.Start.Add(if fsharpRangeIsPastEOF then line.Length else min c line.Length)
        let start = makeSnapshotPoint sl sc
        let end_  = makeSnapshotPoint el ec
        assert(start.CompareTo(end_) <= 0)
        (new SnapshotSpan(start, end_)).Span

