// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.ProjectSystem

    open Helpers 
    open System
    open System.Threading
    open System.Reflection 
    open System.CodeDom
    open System.CodeDom.Compiler
    open System.Runtime.CompilerServices
    open System.Runtime.InteropServices
    open System.Runtime.Serialization
    open System.Collections.Generic
    open System.Collections
    open System.ComponentModel
    open System.ComponentModel.Design
    open System.Text.RegularExpressions
    open System.Diagnostics
    open System.IO
    open System.Drawing
    open System.Globalization
    open System.Text

    open Microsoft.Win32

    open Microsoft.VisualStudio
    open Microsoft.VisualStudio.Shell
    open Microsoft.VisualStudio.Shell.Interop
    open Microsoft.VisualStudio.OLE.Interop
    open Microsoft.VisualStudio.FSharp.ProjectSystem
    open Microsoft.VisualStudio.FSharp.LanguageService
    open Microsoft.VisualStudio.FSharp.ProjectSystem.Automation
    open Microsoft.VisualStudio.Editors
    open Microsoft.VisualStudio.Editors.PropertyPages
    open Microsoft.VisualStudio.TextManager.Interop

    open EnvDTE

    // A type to help manage app.config files

    type internal LangConfigFileHolder(site : System.IServiceProvider) =
        let site = site
        let mutable project : ProjectNode = null
        let mutable rdtFlags : _VSRDTFLAGS = Unchecked.defaultof<_VSRDTFLAGS>
        let mutable document : System.Xml.Linq.XDocument = null
        let mutable buffer : IVsTextLines = null
        let mutable rdtCookie : uint32 = 0u
        
        member private x.InitDocData(itemid, filename) =
            let mutable hr = VSConstants.E_FAIL
            let rdt = site.GetService(typeof<SVsRunningDocumentTable>) :?> IVsRunningDocumentTable
            
            let (hrres, docData, cookie) =
                let (hrres, _, _, docData, cookie) = rdt.FindAndLockDocument(uint32 rdtFlags, filename)
                if ErrorHandler.Failed(hrres) || docData = IntPtr.Zero then
                    // We cannot find the document. Create a resource for it and add it to the RDT.
                    if docData <> IntPtr.Zero then Marshal.Release(docData) |> ignore
                    
                    let file = project.NodeFromItemId(itemid)
                    let projectResources = file :> IVsProjectResources
                    if projectResources = null then
                        (VSConstants.E_NOINTERFACE, IntPtr.Zero, 0u)
                    else
                        let (hrres, docData) = projectResources.CreateResourceDocData(itemid)
                        if ErrorHandler.Succeeded(hrres) then
                            let (hrres, _, _, docData, cookie) = rdt.FindAndLockDocument(uint32 _VSRDTFLAGS.RDT_NoLock, filename)
                            (hrres, docData, cookie)
                        else
                            (hrres, docData, 0u)
                else
                    (hrres, docData, cookie)
                
            hr <- hrres
            rdtCookie <- cookie
            
            // If we have succeeded thus far
            if ErrorHandler.Succeeded(hr) && docData <> IntPtr.Zero then
                try
                    hr <- VSConstants.S_OK
                    let o =  Marshal.GetObjectForIUnknown(docData)
                    buffer <- o :?> IVsTextLines
                    if buffer = null then
                        // try IVsTextBufferProvider
                        let bufferProvider = o :?> IVsTextBufferProvider
                        if bufferProvider <> null then
                            let (hrres, tempb) = bufferProvider.GetTextBuffer()
                            hr <- hrres
                            if ErrorHandler.Succeeded(hrres) && tempb <> null then
                                buffer <- tempb
                        else
                            hr <- VSConstants.E_NOINTERFACE
                finally
                    Marshal.Release(docData) |> ignore
            hr

        member private x.GetTextBufferExtent() =
            let (hrres, lineCount) = buffer.GetLineCount()
            if ErrorHandler.Succeeded(hrres) then
                let (hrres, charsInLine) = buffer.GetLengthOfLine((if lineCount = 0 then lineCount else lineCount - 1))
                (hrres, lineCount, charsInLine)
            else
                (hrres, 0, 0)
           
        member private x.GetText() =
            Debug.Assert(buffer <> null, "Buffer is NULL when calling GetText")
            let (hrres, lines, index) = x.GetTextBufferExtent()
            if ErrorHandler.Succeeded(hrres) then
                let (hrres, text) = buffer.GetLineText(0, 0, (if lines = 0 then lines else lines - 1), index)
                (hrres, text)
            else
                (hrres, "")

        member x.Init(p : ProjectNode, forceCreate : bool) =
        
            // We shouldn't call init more then once
            Debug.Assert(project = null && document = null && buffer = null, "Init already called?")
            let mutable hr = VSConstants.E_FAIL
        
            try
                project <- p
                let specialFiles = p.InteropSafeIVsHierarchy :?> IVsProjectSpecialFiles
                if specialFiles = null then
                    hr <- VSConstants.E_NOINTERFACE
                else
                    rdtFlags <- _VSRDTFLAGS.RDT_ReadLock ||| _VSRDTFLAGS.RDT_EditLock
                    let psfFlags = if forceCreate then __PSFFLAGS.PSFF_FullPath ||| __PSFFLAGS.PSFF_CreateIfNotExist
                                   else __PSFFLAGS.PSFF_FullPath

                    let (hrres, itemid, filename) = specialFiles.GetFile(int (__PSFFILEID.PSFFILEID_AppConfig), uint32 psfFlags)
                    
                    if ErrorHandler.Succeeded(hrres) then
                        if itemid = VSConstants.VSITEMID_NIL then
                            hr <- NativeMethods.STG_E_FILENOTFOUND
                        else
                            hr <- x.InitDocData(itemid, filename)
                            if ErrorHandler.Succeeded(hr) then
                                let (hrres, docText) = x.GetText()
                                if ErrorHandler.Succeeded(hrres) then
                                    document <- System.Xml.Linq.XDocument.Load(new System.IO.StringReader(docText))
            with
            | e -> let icon = OLEMSGICON.OLEMSGICON_CRITICAL
                   let buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK
                   let defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST
                   VsShellUtilities.ShowMessageBox(site, null, e.Message, icon, buttons, defaultButton) |> ignore
                   reraise()
            hr
            
        member private x.GetBufferStream() =
            Debug.Assert(buffer <> null, "Unexpected null buffer")
            let stream = buffer :?> IVsTextStream
            if stream <> null then
                let batchUpdate = stream :?> IVsBatchUpdate
                if batchUpdate <> null then batchUpdate.FlushPendingUpdates(0u) |> ignore
                stream
            else
                null
            
        member x.SaveChanges() =
            Debug.Assert(buffer <> null, "Unexpected null buffer")
            Debug.Assert(document <> null, "Unexpected null document")
            let xml = document.Declaration.ToString() + "\r\n" + document.ToString()
            let stream = x.GetBufferStream()
            let (hr, size) = stream.GetSize()
            let mutable hr = hr
            if ErrorHandler.Succeeded(hr) then
                let dest = Marshal.AllocCoTaskMem(xml.Length * 2)
                try
                    Marshal.Copy(System.Text.Encoding.Unicode.GetBytes(xml), 0, dest, xml.Length * 2)
                    hr <- stream.ReplaceStream(0, size, dest, xml.Length)
                finally
                    Marshal.FreeCoTaskMem(dest)
                hr
            else
                hr
            
        member x.GetXml() =
            Debug.Assert(document <> null, "Null XmlDocument?")
            document
            
        interface IDisposable with
            member x.Dispose() =
                if rdtCookie <> 0u && rdtFlags <> _VSRDTFLAGS.RDT_NoLock then
                    let rdt = site.GetService(typeof<SVsRunningDocumentTable>) :?> IVsRunningDocumentTable
                    rdt.UnlockDocument((uint32 rdtFlags) ||| (uint32 _VSRDTFLAGS.RDT_Unlock_SaveIfDirty), rdtCookie) |> ignore
                    rdtCookie <- 0u
        

    // This type provides a wrapper around the app.config file and provides methods to update
    // the config file with framework moniker information.

    type internal LangConfigFile(site : System.IServiceProvider) =
        let fileHolder = new LangConfigFileHolder(site)
        let mutable isDirty = false
        
        member x.Open(project : ProjectNode) =
            fileHolder.Init(project, false)
            
        static member EnsureChildElement(node : System.Xml.Linq.XElement, name : string) =
            // This method will ensure that a child node named name exists under
            // node. name should not contain any XML namespace information.
            
            // Let's look through the node given
            let any = node.Elements()
                      |> Seq.filter (fun n -> n.Name = (System.Xml.Linq.XName.Get(name)))
            if (Seq.length any) > 0 then
                // We did not create a new node
                (Seq.head any, false)
            else
                let newNode = new System.Xml.Linq.XElement(System.Xml.Linq.XName.Get(name))
                node.Add(newNode)
                // We did create a new node
                (newNode, true)

        // This method will ensure that we add a <supportedRuntime> tag to the config
        // file with the specified version and SKU.  The <supportedRuntime> tag must be
        // inside <configuration> and <startup> tags, like so:
        //
        //  <configuration>
        //      <startup>
        //          <supportedRuntime version="2.0.50727.0" sku="client"/>
        //      </startup>
        //  </configuration>
        // 
        // If sku is NULL, then the SKU attribute is removed from the <supportedRuntime>
        // tag.
        
        static member PatchUpXml(root : System.Xml.Linq.XElement, version : string, sku : string) =
            // Ok, now that we have <configuration> and <startup> nodes, we need to ensure that
            // we have the right <supportedRuntime> element.  Our algorithm here is very simple:
            // unless there is one and exactly one <supportedRuntime> with the right version/client
            // combination, we remove all <supportedRuntime> nodes from the child node list and
            // insert the proper node.

            let APPCFG_STARTUP = "startup"
            let APPCFG_SUPPORTED_RUNTIME = "supportedRuntime"
            let APPCFG_VERSION = "version"
            let APPCFG_SKU = "sku"
            
            let mutable dirty = false
            let (startupNode, _) = LangConfigFile.EnsureChildElement(root, APPCFG_STARTUP)
            
            if startupNode <> null then
                let mutable foundExistingNode = false
               
                let fixupSku (n : System.Xml.Linq.XElement) =
                    // Figure out what to do with the SKU.  A NULL passed-in SKU parameter
                    // means to remove the SKU attribute altogether; otherwise, we set the
                    // desired SKU attribute on the node.                                    
                    
                    let skuAttribute = n.Attribute(System.Xml.Linq.XName.Get(APPCFG_SKU))   
                    if sku = null  then
                        if skuAttribute <> null then
                            skuAttribute.Remove()
                            dirty <- true
                    else
                        // Check if the SKU attribute is the same
                        if skuAttribute <> null  then
                            if String.Compare(skuAttribute.Value, sku, true, CultureInfo.InvariantCulture) <> 0 then
                                // Ok, set the value
                                skuAttribute.Value <- sku
                                dirty <- true
                        else
                            let newAttribute = new System.Xml.Linq.XAttribute(System.Xml.Linq.XName.Get(APPCFG_SKU), sku)
                            n.Add(newAttribute)
                            dirty <- true                
                
                startupNode.Elements()
                |> Seq.toList
                |> Seq.filter (fun n -> n.Name = (System.Xml.Linq.XName.Get(APPCFG_SUPPORTED_RUNTIME)))
                |> Seq.iter (fun n ->
                                let versionAttribute = n.Attribute(System.Xml.Linq.XName.Get(APPCFG_VERSION))
                                
                                // Correct node?  If so, then keep it; otherwise, remove it.
                                if String.Compare(versionAttribute.Value, version, true, CultureInfo.InvariantCulture) = 0  && not foundExistingNode then
                                    foundExistingNode <- true
                                    fixupSku n
                                else
                                    n.Remove()
                                    dirty <- true
                            )
                
                // Did we find a node?  If not, then we need to create one.
                
                if not foundExistingNode then
                    let (runtimeNode, _) = LangConfigFile.EnsureChildElement(startupNode, APPCFG_SUPPORTED_RUNTIME)
                    
                    // Fix up the version - either add it or update it
                    
                    let versionAttribute = runtimeNode.Attribute(System.Xml.Linq.XName.Get(APPCFG_VERSION))
                    if versionAttribute <> null then
                        versionAttribute.Value <- version
                        dirty <- true
                    else
                        let versionAttribute = new System.Xml.Linq.XAttribute(System.Xml.Linq.XName.Get(APPCFG_VERSION), version)
                        runtimeNode.Add(versionAttribute)
                        dirty <- true
                        
                    // fix up the sku
                    fixupSku runtimeNode
                
            dirty

        member x.EnsureSupportedRuntimeElement(version : string, sku : string) =

            let document = fileHolder.GetXml()

            // First, add the <startup> node.  We assume that the root element is the
            // <configuration> element.

            let root = document.Root
            let dirty = LangConfigFile.PatchUpXml(root, version, sku)
            if dirty then isDirty <- dirty

        /// Updates list of binding redirects to the config file. Content of list is governed by the major version of target framework.
        member x.EnsureHasBindingRedirects(targetFSharpCoreVersion, autoGenerateBindingRedirects) =

            if not autoGenerateBindingRedirects then

                // Binding redirects depend on target framework
                let bindingRedirects =
                    [
                        // How to compute the binding redirects for an app.config file
                        // ===========================================================
                        //
                        // Appconfig files appear by default in a console application and can be added to other F# projects.  
                        // Some test frameworks make use of the for library projects
                        // If the project property <AutoGenerateBindingRedirects> is set to true then this code is not needed
                        // However, for those projects that don't have it turned on this is how we compute the binding redirects to generate.
                        //
                        // The assembly version number scheme, evolved in a somewhat haphazard way:
                        //  .Net 2 fsharp.core.dll has the version# 2.0.0.0 for VS2010 and 2.3.0.0 for VS2012 and up
                        //  .Net 4 fsharp.core.dll has the version# 4.3.0.0 for VS2012, 4.3.1 for VS 2013 and 4.4.0.0
                        //
                        // Portable libraries are much different
                        // There is a systematic scheme for portable libries from VS2013+Oob3.1.2 forward
                        // It is:  3.Profile.Major.Minor
                        //     Profile is .net framework profile number I.e 7,47,78 and 259
                        //     Major.minor is the matching FSharp language major.minor version
                        //
                        // However in VS 2012 we released a Portable library which was profile 47 it has the version 2.3.5.0 in VS2013 it was updated to 2.3.5.1
                        // and in VS 2013 we released an additional portable library based on the Windows 8 profile 7 with the version number 3.3.1.0
                        //
                        // Binding redirects are computed based on target fsharp core an fsharp core will be redirected to the target fsharp.core if it is "compatible"
                        // Each desktop fsharp.core is a superset of the previous desktop fsharp.core dll's and is thus "compatible"
                        // Each desktop fsharp.core.dll is also a superset of the portable libraries that shipped with it.
                        // 
                        // The table below represents the appropriate redirections
                        // If the target version is between TagetMin and TargetMax inclusive then the redirects list contains the appropriate redirects
                        //
                        //TargetMin,  targetMax,    redirects
                        "2.3.0.0",    "2.3.0.0",    ["2.0.0.0";    "2.3.0.0"]
                        "2.3.5.1",    "2.3.5.1",    ["2.3.5.0";    "2.3.5.1"]
                        "3.7.4.0",    "3.7.41.0",   ["3.3.1.0";    "3.7.4.0"]
                        "3.7.41.0",   "3.7.41.0",   ["3.7.41.0"]   
                        "3.47.4.0",   "3.47.41.0",  ["2.3.5.0";    "2.3.5.1";   "3.47.4.0"]
                        "3.47.41.0",  "3.47.41.0",  ["3.47.41.0"]  
                        "3.78.4.0",   "3.78.41.0",  ["3.78.3.1";   "3.78.4.0"]
                        "3.78.41.0",  "3.78.41.0",  ["3.78.41.0"]  
                        "3.259.4.0",  "3.259.41.0", ["3.259.3.1";  "3.259.4.0"]
                        "3.259.41.0", "3.259.41.0", ["3.259.41.0"] 
                        "4.3.0.0",    "4.4.3.0",    ["2.0.0.0";    "2.3.0.0";   "2.3.5.0";    "4.0.0.0";   "4.3.0.0"]
                        "4.3.1.0",    "4.4.3.0",    ["3.3.1.0";    "2.3.5.1";   "3.78.3.1";   "3.259.3.1"; "4.3.1.0"]
                        "4.4.0.0",    "4.4.3.0",    ["3.47.4.0";   "3.78.4.0";  "3.259.4.0";  "4.4.0.0"]
                        "4.4.1.0",    "4.4.3.0",    ["3.47.41.0";  "3.78.41.0"; "3.259.41.0"; "4.4.1.0";   "4.4.3.0"]
                    ] |> Seq.where(fun (min, max, _) -> targetFSharpCoreVersion >= min && targetFSharpCoreVersion <= max)

                // some helpers to simplify work with XLinq
                let xname = System.Xml.Linq.XName.Get
                let xnameAsmV1 name = xname ("{urn:schemas-microsoft-com:asm.v1}" + name)

                let fsCoreAttributes = 
                    [
                        xname "name", "FSharp.Core"
                        xname "publicKeyToken", Microsoft.VisualStudio.FSharp.ProjectSystem.Utilities.FsCorePublicKeyToken
                        xname "culture", "neutral"
                    ]

                // depending on major version of target framework we need to populate corresponding binding redirects in config
                let OldVersion = "oldVersion"
                let NewVersion = "newVersion"
                let BindingRedirect = "bindingRedirect"
                let DependentAssembly = "dependentAssembly"
                let AssemblyIdentity = "assemblyIdentity"

                let create (p : System.Xml.Linq.XElement) name attrs = 
                    let el = new System.Xml.Linq.XElement(name : System.Xml.Linq.XName)
                    p.Add(el)
                    for (name, value) in attrs do
                        let attr = new System.Xml.Linq.XAttribute(name, value)
                        el.Add(attr)
                    el

                let createRedirect p (oldVersion, newVersion) =
                    if oldVersion < newVersion then create p (xnameAsmV1 BindingRedirect) [xname OldVersion, oldVersion; xname NewVersion, newVersion] |> ignore

                let getOrCreate(p : System.Xml.Linq.XElement) name =
                    match p.Element(name) with
                    | null -> create p name []
                    | el -> el

                let document = fileHolder.GetXml()
                let runtime = getOrCreate document.Root (xname "runtime")
                let assemblyBinding = getOrCreate runtime (xnameAsmV1 "assemblyBinding")

                // find dependentAssembly node with attributes that corresponds to the FSharp.Core
                let fsharpCoreDependentAssemblyElement =
                    let n =
                        assemblyBinding.Elements(xnameAsmV1 DependentAssembly)
                        |> Seq.tryFind(
                            fun da -> 
                                match da.Element(xnameAsmV1 AssemblyIdentity) with
                                | null -> false
                                | x -> 
                                    fsCoreAttributes 
                                    |> Seq.forall (
                                        fun (attr, value) ->
                                            match x.Attribute attr with
                                            | null -> false
                                            | v -> v.Value = value
                                    )
                            )
                    match n with
                    | Some el -> 
                        // drop existing redirects for FSharp.Core
                        let existingRedirects = el.Elements (xnameAsmV1 BindingRedirect)  |> List.ofSeq
                        for existingRedirect in existingRedirects do
                            existingRedirect.Remove()
                        el
                    | None ->
                        let dependentAssembly = create assemblyBinding (xnameAsmV1 DependentAssembly) []
                        let _fsCoreIdentity = create dependentAssembly (xnameAsmV1 AssemblyIdentity) fsCoreAttributes
                        dependentAssembly

                let redirects =
                    seq {
                            for _,_,redirects in bindingRedirects do 
                                yield! redirects
                        }

                redirects |> Seq.iter(fun r -> if r <> targetFSharpCoreVersion then createRedirect fsharpCoreDependentAssemblyElement (r, targetFSharpCoreVersion))

        member x.IsDirty() = isDirty

        member x.Save() =
            let hr = fileHolder.SaveChanges()
            if ErrorHandler.Succeeded(hr) then
                isDirty <- false
            hr

        interface IDisposable with
            member x.Dispose() =
                (fileHolder :> IDisposable).Dispose()
