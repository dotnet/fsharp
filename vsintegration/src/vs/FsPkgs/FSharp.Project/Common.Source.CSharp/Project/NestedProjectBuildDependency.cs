// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#if UNUSED_NESTED_PROJECTS
using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using System.Globalization;
using System.Collections;
using System.IO;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    /// <summary>
    /// Used for adding a build dependency to nested project (not a real project reference)
    /// </summary>
    internal class NestedProjectBuildDependency : IVsBuildDependency
    {
        IVsHierarchy dependentHierarchy = null;

        [CLSCompliant(false)]
        public NestedProjectBuildDependency(IVsHierarchy dependentHierarchy)
        {
            this.dependentHierarchy = dependentHierarchy;
        }

        public int get_CanonicalName(out string canonicalName)
        {
            canonicalName = null;
            return VSConstants.S_OK;
        }

        public int get_Type(out System.Guid guidType)
        {
            // All our dependencies are build projects
            guidType = VSConstants.GUID_VS_DEPTYPE_BUILD_PROJECT;

            return VSConstants.S_OK;
        }

        public int get_Description(out string description)
        {
            description = null;
            return VSConstants.S_OK;
        }

        [CLSCompliant(false)]
        public int get_HelpContext(out uint helpContext)
        {
            helpContext = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int get_HelpFile(out string helpFile)
        {
            helpFile = null;
            return VSConstants.E_NOTIMPL;
        }

        public int get_MustUpdateBefore(out int mustUpdateBefore)
        {
            // Must always update dependencies
            mustUpdateBefore = 1;

            return VSConstants.S_OK;
        }

        public int get_ReferredProject(out object unknownProject)
        {
            unknownProject = this.dependentHierarchy;

            return (unknownProject == null) ? VSConstants.E_FAIL : VSConstants.S_OK;
        }

    }
}
#endif
