// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.FSharp.LanguageService
open System
open System.IO
open System.Collections
open System.Collections.Generic
open System.Reflection
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.OLE.Interop
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Internal.Utilities.Debug
open Com 
open System.Runtime.InteropServices

/// Methods for dealing with IVsHierarchy
module internal VsHierarchy = 
    let private getItemId (o:obj) : uint32 =
        let r:int32 = (unbox o)
        (uint32)r
    
    let rec private selfAndSiblings (hier:IVsHierarchy) (itemid:uint32) = 
        seq {
            yield itemid
            let hr, sibling = hier.GetProperty(itemid, int32 __VSHPROPID.VSHPROPID_NextSibling)
            if Succeeded hr then yield! selfAndSiblings hier (getItemId sibling) 
        } 

    let private VirtualFolderGuid = new Guid("6bb5f8f0-4483-11d3-8bcf-00c04f8ec28c")
    
    let isVirtualFolder (hier:IVsHierarchy) (itemid:uint32) =
        let hr, guid = hier.GetGuidProperty(itemid, int32 __VSHPROPID.VSHPROPID_TypeGuid)
        Succeeded hr && guid = VirtualFolderGuid
        
    /// Get the path+filename for a particular item, return empty string if none
    let filenameOrEmpty (hier:IVsHierarchy) (itemid:uint32) = 
        let hr, canonicalName = hier.GetCanonicalName(itemid)
        if Succeeded hr then canonicalName else System.String.Empty
        
    /// Get the typename for a particular item, return empty string if none
    let typeName (hier:IVsHierarchy) (itemid:uint32) = 
        let hr, typename = hier.GetProperty(itemid, int32 __VSHPROPID.VSHPROPID_TypeName)
        if Succeeded hr then (typename :?> System.String) else System.String.Empty        

    /// Get the name for a particular item, return empty string if none
    let name (hier:IVsHierarchy) (itemid:uint32) = 
        let hr, typename = hier.GetProperty(itemid, int32 __VSHPROPID.VSHPROPID_Name)
        if Succeeded hr then (typename :?> System.String) else System.String.Empty  
        
    /// Get the projectDirectory for a particular item, return empty string if none
    let projectDirectory (hier:IVsHierarchy) (itemid:uint32) = 
        let hr, typename = hier.GetProperty(itemid, int32 __VSHPROPID.VSHPROPID_ProjectDir)
        if Succeeded hr then (typename :?> System.String) else System.String.Empty  
        
    /// Owner key string that identifies the project GUID of the owning project.
    let ownerkey (hier:IVsHierarchy) (itemid:uint32) = 
        let hr, typename = hier.GetProperty(itemid, int32 __VSHPROPID.VSHPROPID_OwnerKey)
        if Succeeded hr then (typename :?> System.String) else System.String.Empty            
        
    /// All the children itemids ov the given item
    let children (hier:IVsHierarchy) (itemid:uint32) = 
        seq {
            let hr, child = hier.GetProperty(itemid, int32 __VSHPROPID.VSHPROPID_FirstChild)
            if Succeeded hr then yield! selfAndSiblings hier (getItemId child) 
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

/// Strongly typed QueryService    
type internal ServiceProvider(getService:Type->obj) = 
    member private sp.GetService<'S,'T>():'T = unbox(box(getService (typeof<'T>)))
    member sp.TextManager:IVsTextManager = downcast (getService (typeof<SVsTextManager>))
    member sp.Rdt:IVsRunningDocumentTable = downcast (getService (typeof<SVsRunningDocumentTable>))
    member sp.XmlService:IVsXMLMemberIndexService = downcast (getService (typeof<SVsXMLMemberIndexService>))
    static member Stub = ServiceProvider(fun _t->raise (Error.UseOfUnitializedServiceProvider))

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
    
/// Isolate IVsTextLayer as much as possible to ease transition into new editor architecture
module internal VsTextLayer =
    // Convert a local line and index to base.
    let LocalLineIndexToBase (layer:IVsTextLayer) localLine localIndex = Com.ThrowOnFailure2(layer.LocalLineIndexToBase(localLine,localIndex))

/// Isolate IVsTextView as much as possible to ease transition into new editor architecture
module internal VsTextView =
    /// Get the scroll info
    let GetScrollInfo (view:IVsTextView) code = Com.ThrowOnFailure4(view.GetScrollInfo(code))
    /// Get the layered view
    let LayeredTextView (view:IVsTextView) : IVsLayeredTextView = unbox(box(view))
    /// Get the buffer for this view.
    let Buffer (view:IVsTextView) = ThrowOnFailure1(view.GetBuffer())
    
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
    // Get the length of the given line.
    let LengthOfLine (buffer:IVsTextBuffer) (line:int) : int = 
        ThrowOnFailure1(buffer.GetLengthOfLine(line))
    /// Get the text for a particular line.
    let LineText (buffer:IVsTextLines) line = 
        ThrowOnFailure1(buffer.GetLineText(line, 0, line, LengthOfLine buffer line))
    /// Get the color state
    let TextColorState (buffer:IVsTextLines) : IVsTextColorState= unbox(box(buffer))
    /// Get the filename of the given buffer (via IVsUserData). Not all buffers have a file. This will be an exception.
    let GetFilename(buffer : IVsTextLines) =
        let ud = (box buffer) :?> IVsUserData
        VsUserData.GetBufferMonker(ud)
    

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
            
        
