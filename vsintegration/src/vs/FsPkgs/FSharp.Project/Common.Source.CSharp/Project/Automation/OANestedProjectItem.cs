// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#if UNUSED_NESTED_PROJECTS
using System;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Reflection;
using IServiceProvider = System.IServiceProvider;
using Microsoft.VisualStudio.OLE.Interop;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.Fsharp.ProjectSystem;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem.Automation
{
    [SuppressMessage("Microsoft.Interoperability", "CA1405:ComVisibleTypeBaseTypesShouldBeComVisible")]
    [ComVisible(true), CLSCompliant(false)]
    public class OANestedProjectItem : OAProjectItem<NestedProjectNode>
    {
        EnvDTE.Project nestedProject = null;

        internal OANestedProjectItem(OAProject project, NestedProjectNode node)
            : base(project, node)
        {
            object nestedproject = null;
            if (ErrorHandler.Succeeded(node.NestedHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out nestedproject)))
            {
                this.nestedProject = nestedproject as EnvDTE.Project;
            }
        }

        /// <summary>
        /// Returns the collection of project items defined in the nested project
        /// </summary>
        public override EnvDTE.ProjectItems ProjectItems
        {
            get
            {
                if (this.nestedProject != null)
                {
                    return this.nestedProject.ProjectItems;
                }
                return null;
            }
        }

        /// <summary>
        /// Returns the nested project.
        /// </summary>
        public override EnvDTE.Project SubProject
        {
            get
            {
                return this.nestedProject;
            }
        }
    }
}
#endif
