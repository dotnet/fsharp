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

using System;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudioTools.Project;

namespace Microsoft.VisualStudioTools.Navigation {
    class ProjectLibraryNode : LibraryNode {
        private readonly CommonProjectNode _project;

        public ProjectLibraryNode(CommonProjectNode project)
            : base(null, project.Caption, project.Caption, LibraryNodeType.PhysicalContainer) {
            _project = project;
        }

        public override uint CategoryField(LIB_CATEGORY category) {
            switch (category) {
                case LIB_CATEGORY.LC_NODETYPE:
                    return (uint)_LIBCAT_NODETYPE.LCNT_PROJECT;
            }
            return base.CategoryField(category);
        }

        public override VSTREEDISPLAYDATA DisplayData {
            get {
                var res = new VSTREEDISPLAYDATA();
#if DEV14_OR_LATER
                // Use the default Reference icon for projects
                res.hImageList = IntPtr.Zero;
                res.Image = res.SelectedImage = 192;
#else
                res.hImageList = _project.ImageHandler.ImageList.Handle;
                res.Image = res.SelectedImage = (ushort)_project.ImageIndex;
#endif
                return res;
            }
        }

        public override StandardGlyphGroup GlyphType {
            get {
                return StandardGlyphGroup.GlyphCoolProject;
            }
        }
    }
}
