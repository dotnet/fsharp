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
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Execution;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.Project {
    class Output : IVsOutput2 {
        private ProjectNode project;
        private ProjectItemInstance output;

        /// <summary>
        /// Constructor for IVSOutput2 implementation
        /// </summary>
        /// <param name="projectManager">Project that produce this output</param>
        /// <param name="outputAssembly">MSBuild generated item corresponding to the output assembly (by default, these would be of type MainAssembly</param>
        public Output(ProjectNode projectManager, ProjectItemInstance outputAssembly) {
            Utilities.ArgumentNotNull("projectManager", projectManager);

            project = projectManager;
            output = outputAssembly;
        }

        internal string CanonicalName {
            get {
                string canonicalName;
                return ErrorHandler.Succeeded(get_CanonicalName(out canonicalName)) ? canonicalName : null;
            }
        }

        internal string GetMetadata(string name) {
            object value;
            return ErrorHandler.Succeeded(get_Property(name, out value)) ? value as string : null;
        }

        #region IVsOutput2 Members

        public int get_CanonicalName(out string pbstrCanonicalName) {
            if (output == null) {
                pbstrCanonicalName = project.Url;
                return VSConstants.S_OK;
            }

            // Get the output assembly path (including the name)
            pbstrCanonicalName = output.GetMetadataValue("FullPath");
            Debug.Assert(!String.IsNullOrEmpty(pbstrCanonicalName), "Output Assembly not defined");

            // Make sure we have a full path
            pbstrCanonicalName = CommonUtils.GetAbsoluteFilePath(project.ProjectHome, pbstrCanonicalName);
            return VSConstants.S_OK;
        }

        /// <summary>
        /// This path must start with file:/// if it wants other project
        /// to be able to reference the output on disk.
        /// If the output is not on disk, then this requirement does not
        /// apply as other projects probably don't know how to access it.
        /// </summary>
        public virtual int get_DeploySourceURL(out string pbstrDeploySourceURL) {
            if (output == null) {
                // we're lying here to keep callers happy who expect a path...  See also OutputGroup.get_KeyOutputObject
                pbstrDeploySourceURL = GetType().Assembly.CodeBase;
                return VSConstants.S_OK;
            }

            string path = output.GetMetadataValue(ProjectFileConstants.FinalOutputPath);
            if (string.IsNullOrEmpty(path)) {
                pbstrDeploySourceURL = new Url(output.GetMetadataValue("FullPath")).Uri.AbsoluteUri;
                return VSConstants.S_OK;
            }
            if (path.Length < 9 || String.Compare(path.Substring(0, 8), "file:///", StringComparison.OrdinalIgnoreCase) != 0)
                path = "file:///" + path;
            pbstrDeploySourceURL = path;
            return VSConstants.S_OK;
        }

        public int get_DisplayName(out string pbstrDisplayName) {
            return this.get_CanonicalName(out pbstrDisplayName);
        }

        public virtual int get_Property(string szProperty, out object pvar) {
            if (output == null) {
                switch (szProperty) {
                    case "FinalOutputPath":
                        pvar = typeof(string).Assembly.CodeBase;
                        return VSConstants.S_OK;
                }
                pvar = null;
                return VSConstants.E_NOTIMPL;
            }
            String value = output.GetMetadataValue(szProperty);
            pvar = value;

            // If we don't have a value, we are expected to return unimplemented
            return String.IsNullOrEmpty(value) ? VSConstants.E_NOTIMPL : VSConstants.S_OK;
        }

        public int get_RootRelativeURL(out string pbstrRelativePath) {
            if (output == null) {
                pbstrRelativePath = project.ProjectHome;
                return VSConstants.E_FAIL;
            }

            pbstrRelativePath = String.Empty;
            object variant;
            // get the corresponding property

            if (ErrorHandler.Succeeded(this.get_Property("TargetPath", out variant))) {
                string var = variant as String;

                if (var != null) {
                    pbstrRelativePath = var;
                }
            } else {
                string baseDir = project.ProjectHome;
                string fullPath = output.GetMetadataValue("FullPath");
                if (CommonUtils.IsSubpathOf(baseDir, fullPath)) {
                    pbstrRelativePath = CommonUtils.GetRelativeFilePath(baseDir, fullPath);
                }
            }

            return VSConstants.S_OK;
        }

        public virtual int get_Type(out Guid pguidType) {
            pguidType = Guid.Empty;
            throw new NotImplementedException();
        }

        #endregion
    }
}
