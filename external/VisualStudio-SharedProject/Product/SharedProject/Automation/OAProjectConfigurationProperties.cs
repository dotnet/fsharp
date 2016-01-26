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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudioTools.Project;
using VSLangProj;

namespace Microsoft.VisualStudioTools.Project.Automation {
    class OAProjectConfigurationProperties : ConnectionPointContainer, ProjectConfigurationProperties, IConnectionPointContainer, IEventSource<IPropertyNotifySink> {
        private readonly ProjectNode _project;
        private readonly List<IPropertyNotifySink> _sinks = new List<IPropertyNotifySink>();
        private readonly HierarchyListener _hierarchyListener;

        public OAProjectConfigurationProperties(ProjectNode node) {
            _project = node;
            AddEventSource<IPropertyNotifySink>(this);
            _hierarchyListener = new HierarchyListener(_project, this);
        }

        #region ProjectConfigurationProperties Members

        public bool AllowUnsafeBlocks {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public uint BaseAddress {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public bool CheckForOverflowUnderflow {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string ConfigurationOverrideFile {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public bool DebugSymbols {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string DefineConstants {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public bool DefineDebug {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public bool DefineTrace {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string DocumentationFile {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public bool EnableASPDebugging {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public bool EnableASPXDebugging {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public bool EnableSQLServerDebugging {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public bool EnableUnmanagedDebugging {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string ExtenderCATID {
            get { throw new NotImplementedException(); }
        }

        public object ExtenderNames {
            get { throw new NotImplementedException(); }
        }

        public uint FileAlignment {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public bool IncrementalBuild {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string IntermediatePath {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public bool Optimize {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string OutputPath {
            get {
                return _project.Site.GetUIThread().Invoke(() => _project.GetProjectProperty("OutputPath"));
            }
            set {
                _project.Site.GetUIThread().Invoke(() => _project.SetProjectProperty("OutputPath", value));
            }
        }

        public bool RegisterForComInterop {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public bool RemoteDebugEnabled {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string RemoteDebugMachine {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public bool RemoveIntegerChecks {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public prjStartAction StartAction {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string StartArguments {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string StartPage {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string StartProgram {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string StartURL {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public bool StartWithIE {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string StartWorkingDirectory {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public bool TreatWarningsAsErrors {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public prjWarningLevel WarningLevel {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string __id {
            get { throw new NotImplementedException(); }
        }

        public object get_Extender(string ExtenderName) {
            throw new NotImplementedException();
        }

        #endregion

        #region IEventSource<IPropertyNotifySink> Members

        public void OnSinkAdded(IPropertyNotifySink sink) {
            _sinks.Add(sink);
        }

        public void OnSinkRemoved(IPropertyNotifySink sink) {
            _sinks.Remove(sink);
        }

        #endregion

        internal class HierarchyListener : IVsHierarchyEvents {
            private readonly IVsHierarchy _hierarchy;
            private readonly uint _cookie;
            private readonly OAProjectConfigurationProperties _props;

            public HierarchyListener(IVsHierarchy hierarchy, OAProjectConfigurationProperties props) {
                _hierarchy = hierarchy;
                _props = props;
                ErrorHandler.ThrowOnFailure(_hierarchy.AdviseHierarchyEvents(this, out _cookie));
            }

            ~HierarchyListener() {
                _hierarchy.UnadviseHierarchyEvents(_cookie);
            }

            #region IVsHierarchyEvents Members

            public int OnInvalidateIcon(IntPtr hicon) {
                return VSConstants.S_OK;
            }

            public int OnInvalidateItems(uint itemidParent) {
                return VSConstants.S_OK;
            }

            public int OnItemAdded(uint itemidParent, uint itemidSiblingPrev, uint itemidAdded) {
                return VSConstants.S_OK;
            }

            public int OnItemDeleted(uint itemid) {
                return VSConstants.S_OK;
            }

            public int OnItemsAppended(uint itemidParent) {
                return VSConstants.S_OK;
            }

            public int OnPropertyChanged(uint itemid, int propid, uint flags) {
                foreach (var sink in _props._sinks) {
                    sink.OnChanged(propid);
                }
                return VSConstants.S_OK;
            }

            #endregion
        }
    }
}
