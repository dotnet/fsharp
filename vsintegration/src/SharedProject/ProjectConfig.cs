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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
//#define ConfigTrace
using Microsoft.VisualStudio.Shell.Interop;
using MSBuildConstruction = Microsoft.Build.Construction;
using MSBuildExecution = Microsoft.Build.Execution;

namespace Microsoft.VisualStudioTools.Project {
    [ComVisible(true)]
    internal abstract class ProjectConfig :
        IVsCfg,
        IVsProjectCfg,
        IVsProjectCfg2,
        IVsProjectFlavorCfg,
        IVsDebuggableProjectCfg,
        ISpecifyPropertyPages,
        IVsSpecifyProjectDesignerPages,
        IVsCfgBrowseObject {
        internal const string Debug = "Debug";
        internal const string AnyCPU = "AnyCPU";

        private ProjectNode project;
        private string configName;
        private MSBuildExecution.ProjectInstance currentConfig;
        private IVsProjectFlavorCfg flavoredCfg;
        private List<OutputGroup> outputGroups;
        private BuildableProjectConfig buildableCfg;
        private string platformName;

        #region properties

        internal ProjectNode ProjectMgr {
            get {
                return this.project;
            }
        }

        public string ConfigName {
            get {
                return this.configName;
            }
            set {
                this.configName = value;
            }
        }

        public string PlatformName {
            get {
                return platformName;
            }
            set {
                platformName = value;
            }
        }

        internal IList<OutputGroup> OutputGroups {
            get {
                if (null == this.outputGroups) {
                    // Initialize output groups
                    this.outputGroups = new List<OutputGroup>();

                    // If the project is not buildable (no CoreCompile target)
                    // then don't bother getting the output groups.
                    if (this.project.BuildProject != null && this.project.BuildProject.Targets.ContainsKey("CoreCompile")) {
                        // Get the list of group names from the project.
                        // The main reason we get it from the project is to make it easier for someone to modify
                        // it by simply overriding that method and providing the correct MSBuild target(s).
                        IList<KeyValuePair<string, string>> groupNames = project.GetOutputGroupNames();

                        if (groupNames != null) {
                            // Populate the output array
                            foreach (KeyValuePair<string, string> group in groupNames) {
                                OutputGroup outputGroup = CreateOutputGroup(project, group);
                                this.outputGroups.Add(outputGroup);
                            }
                        }
                    }
                }
                return this.outputGroups;
            }
        }

        #endregion

        #region ctors
        internal ProjectConfig(ProjectNode project, string configuration) {
            this.project = project;
            
            if (configuration.Contains("|")) { // If configuration is in the form "<Configuration>|<Platform>"
                string[] configStrArray = configuration.Split('|');
                if (2 == configStrArray.Length) {
                    this.configName = configStrArray[0];
                    this.platformName = configStrArray[1];
                }
                else {
                    throw new Exception(string.Format(CultureInfo.InvariantCulture, "Invalid configuration format: {0}", configuration));
                }
            }
            else { // If configuration is in the form "<Configuration>"          
                this.configName = configuration;
            }

            var flavoredCfgProvider = ProjectMgr.GetOuterInterface<IVsProjectFlavorCfgProvider>();
            Utilities.ArgumentNotNull("flavoredCfgProvider", flavoredCfgProvider);
            ErrorHandler.ThrowOnFailure(flavoredCfgProvider.CreateProjectFlavorCfg(this, out flavoredCfg));
            Utilities.ArgumentNotNull("flavoredCfg", flavoredCfg);

            // if the flavored object support XML fragment, initialize it
            IPersistXMLFragment persistXML = flavoredCfg as IPersistXMLFragment;
            if (null != persistXML) {
                this.project.LoadXmlFragment(persistXML, configName, platformName);
            }
        }
        #endregion

        #region methods

        internal virtual OutputGroup CreateOutputGroup(ProjectNode project, KeyValuePair<string, string> group) {
            OutputGroup outputGroup = new OutputGroup(group.Key, group.Value, project, this);
            return outputGroup;
        }

        public void PrepareBuild(bool clean) {
            project.PrepareBuild(this.configName, clean);
        }

        public virtual string GetConfigurationProperty(string propertyName, bool resetCache) {
            MSBuildExecution.ProjectPropertyInstance property = GetMsBuildProperty(propertyName, resetCache);
            if (property == null)
                return null;

            return property.EvaluatedValue;
        }

        public virtual void SetConfigurationProperty(string propertyName, string propertyValue) {
            if (!this.project.QueryEditProjectFile(false)) {
                throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
            }

            string condition = String.Format(CultureInfo.InvariantCulture, ConfigProvider.configString, this.ConfigName);

            SetPropertyUnderCondition(propertyName, propertyValue, condition);

            // property cache will need to be updated
            this.currentConfig = null;

            return;
        }

        /// <summary>
        /// Emulates the behavior of SetProperty(name, value, condition) on the old MSBuild object model.
        /// This finds a property group with the specified condition (or creates one if necessary) then sets the property in there.
        /// </summary>
        private void SetPropertyUnderCondition(string propertyName, string propertyValue, string condition) {
            string conditionTrimmed = (condition == null) ? String.Empty : condition.Trim();

            if (conditionTrimmed.Length == 0) {
                this.project.BuildProject.SetProperty(propertyName, propertyValue);
                return;
            }

            // New OM doesn't have a convenient equivalent for setting a property with a particular property group condition. 
            // So do it ourselves.
            MSBuildConstruction.ProjectPropertyGroupElement newGroup = null;

            foreach (MSBuildConstruction.ProjectPropertyGroupElement group in this.project.BuildProject.Xml.PropertyGroups) {
                if (String.Equals(group.Condition.Trim(), conditionTrimmed, StringComparison.OrdinalIgnoreCase)) {
                    newGroup = group;
                    break;
                }
            }

            if (newGroup == null) {
                newGroup = this.project.BuildProject.Xml.AddPropertyGroup(); // Adds after last existing PG, else at start of project
                newGroup.Condition = condition;
            }

            foreach (MSBuildConstruction.ProjectPropertyElement property in newGroup.PropertiesReversed) // If there's dupes, pick the last one so we win
            {
                if (String.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase) && property.Condition.Length == 0) {
                    property.Value = propertyValue;
                    return;
                }
            }

            newGroup.AddProperty(propertyName, propertyValue);
        }

        /// <summary>
        /// If flavored, and if the flavor config can be dirty, ask it if it is dirty
        /// </summary>
        /// <param name="storageType">Project file or user file</param>
        /// <returns>0 = not dirty</returns>
        internal int IsFlavorDirty(_PersistStorageType storageType) {
            int isDirty = 0;
            if (this.flavoredCfg != null && this.flavoredCfg is IPersistXMLFragment) {
                ErrorHandler.ThrowOnFailure(((IPersistXMLFragment)this.flavoredCfg).IsFragmentDirty((uint)storageType, out isDirty));
            }
            return isDirty;
        }

        /// <summary>
        /// If flavored, ask the flavor if it wants to provide an XML fragment
        /// </summary>
        /// <param name="flavor">Guid of the flavor</param>
        /// <param name="storageType">Project file or user file</param>
        /// <param name="fragment">Fragment that the flavor wants to save</param>
        /// <returns>HRESULT</returns>
        internal int GetXmlFragment(Guid flavor, _PersistStorageType storageType, out string fragment) {
            fragment = null;
            int hr = VSConstants.S_OK;
            if (this.flavoredCfg != null && this.flavoredCfg is IPersistXMLFragment) {
                Guid flavorGuid = flavor;
                hr = ((IPersistXMLFragment)this.flavoredCfg).Save(ref flavorGuid, (uint)storageType, out fragment, 1);
            }
            return hr;
        }
        #endregion

        #region IVsSpecifyPropertyPages
        public void GetPages(CAUUID[] pages) {
            this.GetCfgPropertyPages(pages);
        }
        #endregion

        #region IVsSpecifyProjectDesignerPages
        /// <summary>
        /// Implementation of the IVsSpecifyProjectDesignerPages. It will retun the pages that are configuration dependent.
        /// </summary>
        /// <param name="pages">The pages to return.</param>
        /// <returns>VSConstants.S_OK</returns>
        public virtual int GetProjectDesignerPages(CAUUID[] pages) {
            this.GetCfgPropertyPages(pages);
            return VSConstants.S_OK;
        }
        #endregion

        #region IVsCfg methods
        /// <summary>
        /// The display name is a two part item
        /// first part is the config name, 2nd part is the platform name
        /// </summary>
        public virtual int get_DisplayName(out string name) {
            if (!string.IsNullOrEmpty(PlatformName)) {
                name = ConfigName + "|" + PlatformName;

            } else {
                name = DisplayName;
            }
            return VSConstants.S_OK;
        }

        private string DisplayName {
            get {
                string name;
                string[] platform = new string[1];
                uint[] actual = new uint[1];
                name = this.configName;
                // currently, we only support one platform, so just add it..
                IVsCfgProvider provider;
                ErrorHandler.ThrowOnFailure(project.GetCfgProvider(out provider));
                ErrorHandler.ThrowOnFailure(((IVsCfgProvider2)provider).GetPlatformNames(1, platform, actual));
                if (!string.IsNullOrEmpty(platform[0])) {
                    name += "|" + platform[0];
                }
                return name;
            }
        }
        public virtual int get_IsDebugOnly(out int fDebug) {
            fDebug = 0;
            if (this.configName == "Debug") {
                fDebug = 1;
            }
            return VSConstants.S_OK;
        }
        public virtual int get_IsReleaseOnly(out int fRelease) {
            fRelease = 0;
            if (this.configName == "Release") {
                fRelease = 1;
            }
            return VSConstants.S_OK;
        }
        #endregion

        #region IVsProjectCfg methods
        public virtual int EnumOutputs(out IVsEnumOutputs eo) {
            eo = null;
            return VSConstants.E_NOTIMPL;
        }

        public virtual int get_BuildableProjectCfg(out IVsBuildableProjectCfg pb) {
            if (project.BuildProject == null || !project.BuildProject.Targets.ContainsKey("CoreCompile")) {
                // The project is not buildable, so don't return a config. This
                // will hide the 'Build' commands from the VS UI.
                pb = null;
                return VSConstants.E_NOTIMPL;
            }
            if (buildableCfg == null) {
                buildableCfg = new BuildableProjectConfig(this);
            }
            pb = buildableCfg;
            return VSConstants.S_OK;
        }

        public virtual int get_CanonicalName(out string name) {
            name = configName;
            return VSConstants.S_OK;
        }

        public virtual int get_IsPackaged(out int pkgd) {
            pkgd = 0;
            return VSConstants.S_OK;
        }

        public virtual int get_IsSpecifyingOutputSupported(out int f) {
            f = 1;
            return VSConstants.S_OK;
        }

        public virtual int get_Platform(out Guid platform) {
            platform = Guid.Empty;
            return VSConstants.E_NOTIMPL;
        }

        public virtual int get_ProjectCfgProvider(out IVsProjectCfgProvider p) {
            p = null;
            IVsCfgProvider cfgProvider = null;
            this.project.GetCfgProvider(out cfgProvider);
            if (cfgProvider != null) {
                p = cfgProvider as IVsProjectCfgProvider;
            }

            return (null == p) ? VSConstants.E_NOTIMPL : VSConstants.S_OK;
        }

        public virtual int get_RootURL(out string root) {
            root = null;
            return VSConstants.S_OK;
        }

        public virtual int get_TargetCodePage(out uint target) {
            target = (uint)System.Text.Encoding.Default.CodePage;
            return VSConstants.S_OK;
        }

        public virtual int get_UpdateSequenceNumber(ULARGE_INTEGER[] li) {
            Utilities.ArgumentNotNull("li", li);

            li[0] = new ULARGE_INTEGER();
            li[0].QuadPart = 0;
            return VSConstants.S_OK;
        }

        public virtual int OpenOutput(string name, out IVsOutput output) {
            output = null;
            return VSConstants.E_NOTIMPL;
        }
        #endregion

        #region IVsProjectCfg2 Members

        public virtual int OpenOutputGroup(string szCanonicalName, out IVsOutputGroup ppIVsOutputGroup) {
            ppIVsOutputGroup = null;
            // Search through our list of groups to find the one they are looking forgroupName
            foreach (OutputGroup group in OutputGroups) {
                string groupName;
                group.get_CanonicalName(out groupName);
                if (String.Compare(groupName, szCanonicalName, StringComparison.OrdinalIgnoreCase) == 0) {
                    ppIVsOutputGroup = group;
                    break;
                }
            }
            return (ppIVsOutputGroup != null) ? VSConstants.S_OK : VSConstants.E_FAIL;
        }

        public virtual int OutputsRequireAppRoot(out int pfRequiresAppRoot) {
            pfRequiresAppRoot = 0;
            return VSConstants.E_NOTIMPL;
        }

        public virtual int get_CfgType(ref Guid iidCfg, out IntPtr ppCfg) {
            // Delegate to the flavored configuration (to enable a flavor to take control)
            // Since we can be asked for Configuration we don't support, avoid throwing and return the HRESULT directly
            int hr = flavoredCfg.get_CfgType(ref iidCfg, out ppCfg);

            return hr;
        }

        public virtual int get_IsPrivate(out int pfPrivate) {
            pfPrivate = 0;
            return VSConstants.S_OK;
        }

        public virtual int get_OutputGroups(uint celt, IVsOutputGroup[] rgpcfg, uint[] pcActual) {
            // Are they only asking for the number of groups?
            if (celt == 0) {
                if ((null == pcActual) || (0 == pcActual.Length)) {
                    throw new ArgumentNullException("pcActual");
                }
                pcActual[0] = (uint)OutputGroups.Count;
                return VSConstants.S_OK;
            }

            // Check that the array of output groups is not null
            if ((null == rgpcfg) || (rgpcfg.Length == 0)) {
                throw new ArgumentNullException("rgpcfg");
            }

            // Fill the array with our output groups
            uint count = 0;
            foreach (OutputGroup group in OutputGroups) {
                if (rgpcfg.Length > count && celt > count && group != null) {
                    rgpcfg[count] = group;
                    ++count;
                }
            }

            if (pcActual != null && pcActual.Length > 0)
                pcActual[0] = count;

            // If the number asked for does not match the number returned, return S_FALSE
            return (count == celt) ? VSConstants.S_OK : VSConstants.S_FALSE;
        }

        public virtual int get_VirtualRoot(out string pbstrVRoot) {
            pbstrVRoot = null;
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IVsDebuggableProjectCfg methods

        /// <summary>
        /// Called by the vs shell to start debugging (managed or unmanaged).
        /// Override this method to support other debug engines.
        /// </summary>
        /// <param name="grfLaunch">A flag that determines the conditions under which to start the debugger. For valid grfLaunch values, see __VSDBGLAUNCHFLAGS</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code</returns>
        public abstract int DebugLaunch(uint grfLaunch);

        /// <summary>
        /// Determines whether the debugger can be launched, given the state of the launch flags.
        /// </summary>
        /// <param name="flags">Flags that determine the conditions under which to launch the debugger. 
        /// For valid grfLaunch values, see __VSDBGLAUNCHFLAGS or __VSDBGLAUNCHFLAGS2.</param>
        /// <param name="fCanLaunch">true if the debugger can be launched, otherwise false</param>
        /// <returns>S_OK if the method succeeds, otherwise an error code</returns>
        public virtual int QueryDebugLaunch(uint flags, out int fCanLaunch) {
            string assembly = this.project.GetAssemblyName(this.ConfigName);
            fCanLaunch = (assembly != null && assembly.ToUpperInvariant().EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) ? 1 : 0;
            if (fCanLaunch == 0) {
                string property = GetConfigurationProperty("StartProgram", true);
                fCanLaunch = (property != null && property.Length > 0) ? 1 : 0;
            }
            return VSConstants.S_OK;
        }

        #endregion

        #region IVsCfgBrowseObject

        /// <summary>
        /// Maps back to the configuration corresponding to the browse object. 
        /// </summary>
        /// <param name="cfg">The IVsCfg object represented by the browse object</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public virtual int GetCfg(out IVsCfg cfg) {
            cfg = this;
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Maps back to the hierarchy or project item object corresponding to the browse object.
        /// </summary>
        /// <param name="hier">Reference to the hierarchy object.</param>
        /// <param name="itemid">Reference to the project item.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public virtual int GetProjectItem(out IVsHierarchy hier, out uint itemid) {
            Utilities.CheckNotNull(this.project);
            Utilities.CheckNotNull(this.project.NodeProperties);

            return this.project.NodeProperties.GetProjectItem(out hier, out itemid);
        }

        #endregion

        #region helper methods

        private MSBuildExecution.ProjectInstance GetCurrentConfig(bool resetCache = false) {
            if (resetCache || currentConfig == null) {
                // Get properties for current configuration from project file and cache it
                project.SetConfiguration(ConfigName);
                project.BuildProject.ReevaluateIfNecessary();
                // Create a snapshot of the evaluated project in its current state
                currentConfig = project.BuildProject.CreateProjectInstance();

                // Restore configuration
                project.SetCurrentConfiguration();
            }
            return currentConfig;
        }

        private MSBuildExecution.ProjectPropertyInstance GetMsBuildProperty(string propertyName, bool resetCache) {
            var current = GetCurrentConfig(resetCache);

            if (current == null)
                throw new Exception("Failed to retrieve properties");

            // return property asked for
            return current.GetProperty(propertyName);
        }

        /// <summary>
        /// Retrieves the configuration dependent property pages.
        /// </summary>
        /// <param name="pages">The pages to return.</param>
        private void GetCfgPropertyPages(CAUUID[] pages) {
            // We do not check whether the supportsProjectDesigner is set to true on the ProjectNode.
            // We rely that the caller knows what to call on us.
            Utilities.ArgumentNotNull("pages", pages);


            if (pages.Length == 0) {
                throw new ArgumentException(SR.GetString(SR.InvalidParameter), "pages");
            }

            // Retrive the list of guids from hierarchy properties.
            // Because a flavor could modify that list we must make sure we are calling the outer most implementation of IVsHierarchy
            string guidsList = String.Empty;
            IVsHierarchy hierarchy = project.GetOuterInterface<IVsHierarchy>();
            object variant = null;
            ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID2.VSHPROPID_CfgPropertyPagesCLSIDList, out variant), new int[] { VSConstants.DISP_E_MEMBERNOTFOUND, VSConstants.E_NOTIMPL });
            guidsList = (string)variant;

            Guid[] guids = Utilities.GuidsArrayFromSemicolonDelimitedStringOfGuids(guidsList);
            if (guids == null || guids.Length == 0) {
                pages[0] = new CAUUID();
                pages[0].cElems = 0;
            } else {
                pages[0] = PackageUtilities.CreateCAUUIDFromGuidArray(guids);
            }
        }

        internal virtual bool IsInputGroup(string groupName) {
            return groupName == "SourceFiles";
        }

        private static DateTime? TryGetLastWriteTimeUtc(string path, Redirector output = null) {
            try {
                return File.GetLastWriteTimeUtc(path);
            } catch (UnauthorizedAccessException ex) {
                if (output != null) {
                    output.WriteErrorLine(string.Format("Failed to access {0}: {1}", path, ex.Message));
#if DEBUG
                    output.WriteErrorLine(ex.ToString());
#endif
                }
            } catch (ArgumentException ex) {
                if (output != null) {
                    output.WriteErrorLine(string.Format("Failed to access {0}: {1}", path, ex.Message));
#if DEBUG
                    output.WriteErrorLine(ex.ToString());
#endif
                }
            } catch (PathTooLongException ex) {
                if (output != null) {
                    output.WriteErrorLine(string.Format("Failed to access {0}: {1}", path, ex.Message));
#if DEBUG
                    output.WriteErrorLine(ex.ToString());
#endif
                }
            } catch (NotSupportedException ex) {
                if (output != null) {
                    output.WriteErrorLine(string.Format("Failed to access {0}: {1}", path, ex.Message));
#if DEBUG
                    output.WriteErrorLine(ex.ToString());
#endif
                }
            }
            return null;
        }

        internal virtual bool IsUpToDate() {
            var outputWindow = OutputWindowRedirector.GetGeneral(ProjectMgr.Site);
#if DEBUG
            outputWindow.WriteLine(string.Format("Checking whether {0} needs to be rebuilt:", ProjectMgr.Caption));
#endif

            var latestInput = DateTime.MinValue;
            var earliestOutput = DateTime.MaxValue;
            bool mustRebuild = false;

            var allInputs = new HashSet<string>(OutputGroups
                .Where(g => IsInputGroup(g.Name))
                .SelectMany(x => x.EnumerateOutputs())
                .Select(input => input.CanonicalName),
                StringComparer.OrdinalIgnoreCase
            );
            foreach (var group in OutputGroups.Where(g => !IsInputGroup(g.Name))) {
                foreach (var output in group.EnumerateOutputs()) {
                    var path = output.CanonicalName;
#if DEBUG
                    var dt = TryGetLastWriteTimeUtc(path);
                    outputWindow.WriteLine(string.Format(
                        "  Out: {0}: {1} [{2}]",
                        group.Name,
                        path,
                        dt.HasValue ? dt.Value.ToString("s") : "err"
                    ));
#endif
                    DateTime? modifiedTime;

                    if (!File.Exists(path) ||
                        !(modifiedTime = TryGetLastWriteTimeUtc(path, outputWindow)).HasValue) {
                        mustRebuild = true;
                        break;
                    }

                    string inputPath;
                    if (File.Exists(inputPath = output.GetMetadata("SourceFile"))) {
                        var inputModifiedTime = TryGetLastWriteTimeUtc(inputPath, outputWindow);
                        if (inputModifiedTime.HasValue && inputModifiedTime.Value > modifiedTime.Value) {
                            mustRebuild = true;
                            break;
                        } else {
                            continue;
                        }
                    }

                    // output is an input, ignore it...
                    if (allInputs.Contains(path)) {
                        continue;
                    }

                    if (modifiedTime.Value < earliestOutput) {
                        earliestOutput = modifiedTime.Value;
                    }
                }

                if (mustRebuild) {
                    // Early exit if we know we're going to have to rebuild
                    break;
                }
            }

            if (mustRebuild) {
#if DEBUG
                outputWindow.WriteLine(string.Format(
                    "Rebuilding {0} because mustRebuild is true",
                    ProjectMgr.Caption
                ));
#endif
                return false;
            }

            foreach (var group in OutputGroups.Where(g => IsInputGroup(g.Name))) {
                foreach (var input in group.EnumerateOutputs()) {
                    var path = input.CanonicalName;
#if DEBUG
                    var dt = TryGetLastWriteTimeUtc(path);
                    outputWindow.WriteLine(string.Format(
                        "  In:  {0}: {1} [{2}]",
                        group.Name,
                        path,
                        dt.HasValue ? dt.Value.ToString("s") : "err"
                    ));
#endif
                    if (!File.Exists(path)) {
                        continue;
                    }

                    var modifiedTime = TryGetLastWriteTimeUtc(path, outputWindow);
                    if (modifiedTime.HasValue && modifiedTime.Value > latestInput) {
                        latestInput = modifiedTime.Value;
                        if (earliestOutput < latestInput) {
                            break;
                        }
                    }
                }

                if (earliestOutput < latestInput) {
                    // Early exit if we know we're going to have to rebuild
                    break;
                }
            }

            if (earliestOutput < latestInput) {
#if DEBUG
                outputWindow.WriteLine(string.Format(
                    "Rebuilding {0} because {1:s} < {2:s}",
                    ProjectMgr.Caption,
                    earliestOutput,
                    latestInput
                ));
#endif
                return false;
            } else {
#if DEBUG
                outputWindow.WriteLine(string.Format(
                    "Not rebuilding {0} because {1:s} >= {2:s}",
                    ProjectMgr.Caption,
                    earliestOutput,
                    latestInput
                ));
#endif
                return true;
            }
        }

        #endregion

        #region IVsProjectFlavorCfg Members
        /// <summary>
        /// This is called to let the flavored config let go
        /// of any reference it may still be holding to the base config
        /// </summary>
        /// <returns></returns>
        int IVsProjectFlavorCfg.Close() {
            // This is used to release the reference the flavored config is holding
            // on the base config, but in our scenario these 2 are the same object
            // so we have nothing to do here.
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Actual implementation of get_CfgType.
        /// When not flavored or when the flavor delegate to use
        /// we end up creating the requested config if we support it.
        /// </summary>
        /// <param name="iidCfg">IID representing the type of config object we should create</param>
        /// <param name="ppCfg">Config object that the method created</param>
        /// <returns>HRESULT</returns>
        int IVsProjectFlavorCfg.get_CfgType(ref Guid iidCfg, out IntPtr ppCfg) {
            ppCfg = IntPtr.Zero;

            // See if this is an interface we support
            if (iidCfg == typeof(IVsDebuggableProjectCfg).GUID) {
                ppCfg = Marshal.GetComInterfaceForObject(this, typeof(IVsDebuggableProjectCfg));
            } else if (iidCfg == typeof(IVsBuildableProjectCfg).GUID) {
                IVsBuildableProjectCfg buildableConfig;
                this.get_BuildableProjectCfg(out buildableConfig);
                //
                //In some cases we've intentionally shutdown the build options
                //  If buildableConfig is null then don't try to get the BuildableProjectCfg interface
                //  
                if (null != buildableConfig) {
                    ppCfg = Marshal.GetComInterfaceForObject(buildableConfig, typeof(IVsBuildableProjectCfg));
                }
            }

            // If not supported
            if (ppCfg == IntPtr.Zero)
                return VSConstants.E_NOINTERFACE;

            return VSConstants.S_OK;
        }

        #endregion
    }

    [ComVisible(true)]
    internal class BuildableProjectConfig : IVsBuildableProjectCfg {
        #region fields
        ProjectConfig config = null;
        EventSinkCollection callbacks = new EventSinkCollection();
        #endregion

        #region ctors
        public BuildableProjectConfig(ProjectConfig config) {
            this.config = config;
        }
        #endregion

        #region IVsBuildableProjectCfg methods

        public virtual int AdviseBuildStatusCallback(IVsBuildStatusCallback callback, out uint cookie) {
            cookie = callbacks.Add(callback);
            return VSConstants.S_OK;
        }

        public virtual int get_ProjectCfg(out IVsProjectCfg p) {
            p = config;
            return VSConstants.S_OK;
        }

        public virtual int QueryStartBuild(uint options, int[] supported, int[] ready) {
            if (supported != null && supported.Length > 0)
                supported[0] = 1;
            if (ready != null && ready.Length > 0)
                ready[0] = (this.config.ProjectMgr.BuildInProgress) ? 0 : 1;
            return VSConstants.S_OK;
        }

        public virtual int QueryStartClean(uint options, int[] supported, int[] ready) {
            if (supported != null && supported.Length > 0)
                supported[0] = 1;
            if (ready != null && ready.Length > 0)
                ready[0] = (this.config.ProjectMgr.BuildInProgress) ? 0 : 1;
            return VSConstants.S_OK;
        }

        public virtual int QueryStartUpToDateCheck(uint options, int[] supported, int[] ready) {
            if (supported != null && supported.Length > 0)
                supported[0] = 1;
            if (ready != null && ready.Length > 0)
                ready[0] = (this.config.ProjectMgr.BuildInProgress) ? 0 : 1;
            return VSConstants.S_OK;
        }

        public virtual int QueryStatus(out int done) {
            done = (this.config.ProjectMgr.BuildInProgress) ? 0 : 1;
            return VSConstants.S_OK;
        }

        public virtual int StartBuild(IVsOutputWindowPane pane, uint options) {
            config.PrepareBuild(false);

            // Current version of MSBuild wish to be called in an STA
            uint flags = VSConstants.VS_BUILDABLEPROJECTCFGOPTS_REBUILD;

            // If we are not asked for a rebuild, then we build the default target (by passing null)
            this.Build(options, pane, ((options & flags) != 0) ? MsBuildTarget.Rebuild : null);

            return VSConstants.S_OK;
        }

        public virtual int StartClean(IVsOutputWindowPane pane, uint options) {
            config.PrepareBuild(true);
            // Current version of MSBuild wish to be called in an STA
            this.Build(options, pane, MsBuildTarget.Clean);
            return VSConstants.S_OK;
        }

        public virtual int StartUpToDateCheck(IVsOutputWindowPane pane, uint options) {
            return config.IsUpToDate() ?
                VSConstants.S_OK :
                VSConstants.E_FAIL;
        }

        public virtual int Stop(int fsync) {
            return VSConstants.S_OK;
        }

        public virtual int UnadviseBuildStatusCallback(uint cookie) {
            callbacks.RemoveAt(cookie);
            return VSConstants.S_OK;
        }

        public virtual int Wait(uint ms, int fTickWhenMessageQNotEmpty) {
            return VSConstants.E_NOTIMPL;
        }
        #endregion

        #region helpers

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private bool NotifyBuildBegin() {
            int shouldContinue = 1;
            foreach (IVsBuildStatusCallback cb in callbacks) {
                try {
                    ErrorHandler.ThrowOnFailure(cb.BuildBegin(ref shouldContinue));
                    if (shouldContinue == 0) {
                        return false;
                    }
                } catch (Exception e) {
                    // If those who ask for status have bugs in their code it should not prevent the build/notification from happening
                    Debug.Fail(SR.GetString(SR.BuildEventError, e.Message));
                }
            }

            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void NotifyBuildEnd(MSBuildResult result, string buildTarget) {
            int success = ((result == MSBuildResult.Successful) ? 1 : 0);

            foreach (IVsBuildStatusCallback cb in callbacks) {
                try {
                    ErrorHandler.ThrowOnFailure(cb.BuildEnd(success));
                } catch (Exception e) {
                    // If those who ask for status have bugs in their code it should not prevent the build/notification from happening
                    Debug.Fail(SR.GetString(SR.BuildEventError, e.Message));
                } finally {
                    // We want to refresh the references if we are building with the Build or Rebuild target or if the project was opened for browsing only.
                    bool shouldRepaintReferences = (buildTarget == null || buildTarget == MsBuildTarget.Build || buildTarget == MsBuildTarget.Rebuild);

                    // Now repaint references if that is needed. 
                    // We hardly rely here on the fact the ResolveAssemblyReferences target has been run as part of the build.
                    // One scenario to think at is when an assembly reference is renamed on disk thus becomming unresolvable, 
                    // but msbuild can actually resolve it.
                    // Another one if the project was opened only for browsing and now the user chooses to build or rebuild.
                    if (shouldRepaintReferences && (result == MSBuildResult.Successful)) {
                        this.RefreshReferences();
                    }
                }
            }
        }

        private void Build(uint options, IVsOutputWindowPane output, string target) {
            if (!this.NotifyBuildBegin()) {
                return;
            }

            try {
                config.ProjectMgr.BuildAsync(options, this.config.ConfigName, output, target, (result, buildTarget) => this.NotifyBuildEnd(result, buildTarget));
            } catch (Exception e) {
                if (e.IsCriticalException()) {
                    throw;
                }
                Trace.WriteLine("Exception : " + e.Message);
                ErrorHandler.ThrowOnFailure(output.OutputStringThreadSafe("Unhandled Exception:" + e.Message + "\n"));
                this.NotifyBuildEnd(MSBuildResult.Failed, target);
                throw;
            } finally {
                ErrorHandler.ThrowOnFailure(output.FlushToTaskList());
            }
        }

        /// <summary>
        /// Refreshes references and redraws them correctly.
        /// </summary>
        private void RefreshReferences() {
            // Refresh the reference container node for assemblies that could be resolved.
            IReferenceContainer referenceContainer = this.config.ProjectMgr.GetReferenceContainer();
            if (referenceContainer != null) {
                foreach (ReferenceNode referenceNode in referenceContainer.EnumReferences()) {
                    referenceNode.RefreshReference();
                }
            }
        }
        #endregion
    }

}
