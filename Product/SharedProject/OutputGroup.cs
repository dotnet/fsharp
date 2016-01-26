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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using MSBuildExecution = Microsoft.Build.Execution;

namespace Microsoft.VisualStudioTools.Project {
    /// <summary>
    /// Allows projects to group outputs according to usage.
    /// </summary>
    [ComVisible(true)]
    internal class OutputGroup : IVsOutputGroup2 {
        private readonly ProjectConfig _projectCfg;
        private readonly ProjectNode _project;
        private readonly List<Output> _outputs = new List<Output>();
        private readonly string _name;
        private readonly string _targetName;
        private Output _keyOutput;


        /// <summary>
        /// Constructor for IVSOutputGroup2 implementation
        /// </summary>
        /// <param name="outputName">Name of the output group. See VS_OUTPUTGROUP_CNAME_Build in vsshell.idl for the list of standard values</param>
        /// <param name="msBuildTargetName">MSBuild target name</param>
        /// <param name="projectManager">Project that produce this output</param>
        /// <param name="configuration">Configuration that produce this output</param>
        public OutputGroup(string outputName, string msBuildTargetName, ProjectNode projectManager, ProjectConfig configuration) {
            Utilities.ArgumentNotNull("outputName", outputName);
            Utilities.ArgumentNotNull("msBuildTargetName", msBuildTargetName);
            Utilities.ArgumentNotNull("projectManager", projectManager);
            Utilities.ArgumentNotNull("configuration", configuration);

            _name = outputName;
            _targetName = msBuildTargetName;
            _project = projectManager;
            _projectCfg = configuration;
        }

        /// <summary>
        /// Get the project configuration object associated with this output group
        /// </summary>
        protected ProjectConfig ProjectCfg {
            get { return _projectCfg; }
        }

        /// <summary>
        /// Get the project object that produces this output group.
        /// </summary>
        internal ProjectNode Project {
            get { return _project; }
        }

        /// <summary>
        /// Gets the msbuild target name which is assciated to the outputgroup.
        /// ProjectNode defines a static collection of output group names and their associated MsBuild target
        /// </summary>
        protected string TargetName {
            get { return _targetName; }
        }

        /// <summary>
        /// Easy access to the canonical name of the group.
        /// </summary>
        internal string Name {
            get {
                string canonicalName;
                ErrorHandler.ThrowOnFailure(get_CanonicalName(out canonicalName));
                return canonicalName;
            }
        }

        #region virtual methods

        protected virtual void Refresh() {
            // Let MSBuild know which configuration we are working with
            _project.SetConfiguration(_projectCfg.ConfigName);

            // Generate dependencies if such a task exist
            if (_project.BuildProject.Targets.ContainsKey(_targetName)) {
                bool succeeded = false;
                _project.BuildTarget(_targetName, out succeeded);
                if (!succeeded) {
                    Debug.WriteLine("Failed to build target {0}", _targetName);
                    this._outputs.Clear();
                    return;
                }
            }

            // Rebuild the content of our list of output
            string outputType = _targetName + "Output";
            this._outputs.Clear();

            if (_project.CurrentConfig != null) {
                foreach (MSBuildExecution.ProjectItemInstance assembly in _project.CurrentConfig.GetItems(outputType)) {
                    Output output = new Output(_project, assembly);
                    _outputs.Add(output);

                    // See if it is our key output
                    if (_keyOutput == null ||
                        String.Compare(assembly.GetMetadataValue("IsKeyOutput"), true.ToString(), StringComparison.OrdinalIgnoreCase) == 0) {
                        _keyOutput = output;
                    }
                }
            }

            _project.SetCurrentConfiguration();

            // Now that the group is built we have to check if it is invalidated by a property
            // change on the project.
            _project.OnProjectPropertyChanged += new EventHandler<ProjectPropertyChangedArgs>(OnProjectPropertyChanged);
        }

        public virtual IList<Output> EnumerateOutputs() {
            _project.Site.GetUIThread().Invoke(Refresh);
            return _outputs;
        }

        public virtual void InvalidateGroup() {
            // Set keyOutput to null so that a refresh will be performed the next time
            // a property getter is called.
            if (null != _keyOutput) {
                // Once the group is invalidated there is no more reason to listen for events.
                _project.OnProjectPropertyChanged -= new EventHandler<ProjectPropertyChangedArgs>(OnProjectPropertyChanged);
            }
            _keyOutput = null;
        }
        #endregion

        #region event handlers
        private void OnProjectPropertyChanged(object sender, ProjectPropertyChangedArgs args) {
            // In theory here we should decide if we have to invalidate the group according with the kind of property
            // that is changed.
            InvalidateGroup();
        }
        #endregion

        #region IVsOutputGroup2 Members

        public virtual int get_CanonicalName(out string pbstrCanonicalName) {
            pbstrCanonicalName = this._name;
            return VSConstants.S_OK;
        }

        public virtual int get_DeployDependencies(uint celt, IVsDeployDependency[] rgpdpd, uint[] pcActual) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int get_Description(out string pbstrDescription) {
            pbstrDescription = null;

            string description;
            int hr = this.get_CanonicalName(out description);
            if (ErrorHandler.Succeeded(hr))
                pbstrDescription = this.Project.GetOutputGroupDescription(description);
            return hr;
        }

        public virtual int get_DisplayName(out string pbstrDisplayName) {
            pbstrDisplayName = null;

            string displayName;
            int hr = this.get_CanonicalName(out displayName);
            if (ErrorHandler.Succeeded(hr))
                pbstrDisplayName = this.Project.GetOutputGroupDisplayName(displayName);
            return hr;
        }

        public virtual int get_KeyOutput(out string pbstrCanonicalName) {
            pbstrCanonicalName = null;
            if (_keyOutput == null)
                Refresh();
            if (_keyOutput == null) {
                pbstrCanonicalName = String.Empty;
                return VSConstants.S_FALSE;
            }
            return _keyOutput.get_CanonicalName(out pbstrCanonicalName);
        }

        public virtual int get_KeyOutputObject(out IVsOutput2 ppKeyOutput) {
            if (_keyOutput == null) {
                Refresh();
                if (_keyOutput == null) {
                    // horrible hack: we don't really have outputs but the Cider designer insists 
                    // that we have an output so it can figure out our output assembly name.  So we
                    // lie here, and then lie again to give a path in Output.get_Property
                    _keyOutput = new Output(_project, null);
                }
            }
            ppKeyOutput = _keyOutput;
            if (ppKeyOutput == null)
                return VSConstants.S_FALSE;
            return VSConstants.S_OK;
        }

        public virtual int get_Outputs(uint celt, IVsOutput2[] rgpcfg, uint[] pcActual) {
            // Ensure that we are refreshed.  This is somewhat of a hack that enables project to
            // project reference scenarios to work.  Normally, output groups are populated as part
            // of build.  However, in the project to project reference case, what ends up happening
            // is that the referencing projects requests the referenced project's output group
            // before a build is done on the referenced project.
            //
            // Furthermore, the project auto toolbox manager requires output groups to be populated
            // on project reopen as well...
            //
            // In the end, this is probably the right thing to do, though -- as it keeps the output
            // groups always up to date.
            Refresh();

            // See if only the caller only wants to know the count
            if (celt == 0 || rgpcfg == null) {
                if (pcActual != null && pcActual.Length > 0)
                    pcActual[0] = (uint)_outputs.Count;
                return VSConstants.S_OK;
            }

            // Fill the array with our outputs
            uint count = 0;
            foreach (Output output in _outputs) {
                if (rgpcfg.Length > count && celt > count && output != null) {
                    rgpcfg[count] = output;
                    ++count;
                }
            }

            if (pcActual != null && pcActual.Length > 0)
                pcActual[0] = count;

            // If the number asked for does not match the number returned, return S_FALSE
            return (count == celt) ? VSConstants.S_OK : VSConstants.S_FALSE;
        }

        public virtual int get_ProjectCfg(out IVsProjectCfg2 ppIVsProjectCfg2) {
            ppIVsProjectCfg2 = (IVsProjectCfg2)this._projectCfg;
            return VSConstants.S_OK;
        }

        public virtual int get_Property(string pszProperty, out object pvar) {
            pvar = _project.GetProjectProperty(pszProperty);
            return VSConstants.S_OK;
        }

        #endregion
    }
}
