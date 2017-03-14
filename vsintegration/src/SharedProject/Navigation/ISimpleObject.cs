/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.Navigation {
    public interface ISimpleObject {
        bool CanDelete { get; }
        bool CanGoToSource { get; }
        bool CanRename { get; }
        string Name { get; }
        string UniqueName { get; }
        string FullName { get; }
        string GetTextRepresentation(VSTREETEXTOPTIONS options);
        string TooltipText { get; }
        object BrowseObject { get; }
        CommandID ContextMenuID { get; }
        VSTREEDISPLAYDATA DisplayData { get; }

        uint CategoryField(LIB_CATEGORY lIB_CATEGORY);

        void Delete();
        void DoDragDrop(OleDataObject dataObject, uint grfKeyState, uint pdwEffect);
        void Rename(string pszNewName, uint grfFlags);
        void GotoSource(VSOBJGOTOSRCTYPE SrcType);

        void SourceItems(out IVsHierarchy ppHier, out uint pItemid, out uint pcItems);
        uint EnumClipboardFormats(_VSOBJCFFLAGS _VSOBJCFFLAGS, VSOBJCLIPFORMAT[] rgcfFormats);
        void FillDescription(_VSOBJDESCOPTIONS _VSOBJDESCOPTIONS, IVsObjectBrowserDescription3 pobDesc);

        IVsSimpleObjectList2 FilterView(uint ListType);
    }

}
