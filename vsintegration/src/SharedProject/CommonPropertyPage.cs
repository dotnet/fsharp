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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Build.Construction;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;

namespace Microsoft.VisualStudioTools.Project {
    /// <summary>
    /// Base class for property pages based on a WinForm control.
    /// </summary>
    public abstract class CommonPropertyPage : IPropertyPage {
        private IPropertyPageSite _site;
        private bool _dirty, _loading;
        private CommonProjectNode _project;

        public abstract Control Control {
            get;
        }

        public abstract void Apply();
        public abstract void LoadSettings();

        public abstract string Name {
            get;
        }

        internal virtual CommonProjectNode Project {
            get {
                return _project;
            }
            set {
                _project = value;
            }
        }

        internal virtual IEnumerable<CommonProjectConfig> SelectedConfigs {
            get; set;
        }

        protected void SetProjectProperty(string propertyName, string propertyValue) {
            // SetProjectProperty's implementation will check whether the value
            // has changed.
            Project.SetProjectProperty(propertyName, propertyValue);
        }

        protected string GetProjectProperty(string propertyName) {
            return Project.GetUnevaluatedProperty(propertyName);
        }

        protected void SetUserProjectProperty(string propertyName, string propertyValue) {
            // SetUserProjectProperty's implementation will check whether the value
            // has changed.
            Project.SetUserProjectProperty(propertyName, propertyValue);
        }

        protected string GetUserProjectProperty(string propertyName) {
            return Project.GetUserProjectProperty(propertyName);
        }

        protected string GetConfigUserProjectProperty(string propertyName) {
            if (SelectedConfigs == null) {
                string condition = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    ConfigProvider.configPlatformString,
                    Project.CurrentConfig.GetPropertyValue("Configuration"),
                    Project.CurrentConfig.GetPropertyValue("Platform"));

                return GetUserPropertyUnderCondition(propertyName, condition);
            } else {
                StringCollection values = new StringCollection();

                foreach (var config in SelectedConfigs) {
                    string condition = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        ConfigProvider.configPlatformString,
                        config.ConfigName,
                        config.PlatformName);

                    values.Add(GetUserPropertyUnderCondition(propertyName, condition));
                }

                switch (values.Count) {
                case 0:
                    return null;
                case 1:
                    return values[0];
                default:
                    return "<different values>";
                }
            }
        }

        protected void SetConfigUserProjectProperty(string propertyName, string propertyValue) {
            if (SelectedConfigs == null) {
                string condition = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    ConfigProvider.configPlatformString,
                    Project.CurrentConfig.GetPropertyValue("Configuration"),
                    Project.CurrentConfig.GetPropertyValue("Platform"));

                SetUserPropertyUnderCondition(propertyName, propertyValue, condition);
            } else {
                foreach (var config in SelectedConfigs) {
                    string condition = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        ConfigProvider.configPlatformString,
                        config.ConfigName,
                        config.GetConfigurationProperty("Platform", false));

                    SetUserPropertyUnderCondition(propertyName, propertyValue, condition);
                }
            }
        }

        private string GetUserPropertyUnderCondition(string propertyName, string condition) {
            string conditionTrimmed = (condition == null) ? String.Empty : condition.Trim();

            if (Project.UserBuildProject != null) {
                if (conditionTrimmed.Length == 0) {
                    return Project.UserBuildProject.GetProperty(propertyName).UnevaluatedValue;
                }

                // New OM doesn't have a convenient equivalent for setting a property with a particular property group condition. 
                // So do it ourselves.
                ProjectPropertyGroupElement matchingGroup = null;

                foreach (ProjectPropertyGroupElement group in Project.UserBuildProject.Xml.PropertyGroups) {
                    if (String.Equals(group.Condition.Trim(), conditionTrimmed, StringComparison.OrdinalIgnoreCase)) {
                        matchingGroup = group;
                        break;
                    }
                }

                if (matchingGroup != null) {
                    foreach (ProjectPropertyElement property in matchingGroup.PropertiesReversed) // If there's dupes, pick the last one so we win
                    {
                        if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase)
                            && (property.Condition == null || property.Condition.Length == 0)) {
                            return property.Value;
                        }
                    }
                }

            }

            return null;
        }

        /// <summary>
        /// Emulates the behavior of SetProperty(name, value, condition) on the old MSBuild object model.
        /// This finds a property group with the specified condition (or creates one if necessary) then sets the property in there.
        /// </summary>
        private void SetUserPropertyUnderCondition(string propertyName, string propertyValue, string condition) {
            string conditionTrimmed = (condition == null) ? String.Empty : condition.Trim();
            const string userProjectCreateProperty = "UserProject";

            if (Project.UserBuildProject == null) {
                Project.SetUserProjectProperty(userProjectCreateProperty, null);
            }

            if (conditionTrimmed.Length == 0) {
                var userProp = Project.UserBuildProject.GetProperty(userProjectCreateProperty);
                if (userProp != null) {
                    Project.UserBuildProject.RemoveProperty(userProp);
                }
                Project.UserBuildProject.SetProperty(propertyName, propertyValue);
                return;
            }

            // New OM doesn't have a convenient equivalent for setting a property with a particular property group condition. 
            // So do it ourselves.
            ProjectPropertyGroupElement newGroup = null;

            foreach (ProjectPropertyGroupElement group in Project.UserBuildProject.Xml.PropertyGroups) {
                if (String.Equals(group.Condition.Trim(), conditionTrimmed, StringComparison.OrdinalIgnoreCase)) {
                    newGroup = group;
                    break;
                }
            }

            if (newGroup == null) {
                newGroup = Project.UserBuildProject.Xml.AddPropertyGroup(); // Adds after last existing PG, else at start of project
                newGroup.Condition = condition;
            }

            foreach (ProjectPropertyElement property in newGroup.PropertiesReversed) // If there's dupes, pick the last one so we win
            {
                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase) 
                    && (property.Condition == null || property.Condition.Length == 0)) {
                    property.Value = propertyValue;
                    return;
                }
            }

            newGroup.AddProperty(propertyName, propertyValue);
        }

        public bool Loading {
            get {
                return _loading;
            }
            set {
                _loading = value;
            }
        }

        public bool IsDirty {
            get {
                return _dirty;
            }
            set {
                if (_dirty != value && !Loading) {
                    _dirty = value;
                    if (_site != null) {
                        _site.OnStatusChange((uint)(_dirty ? PropPageStatus.Dirty : PropPageStatus.Clean));
                    }
                }
            }
        }

        void IPropertyPage.Activate(IntPtr hWndParent, RECT[] pRect, int bModal) {
            NativeMethods.SetParent(Control.Handle, hWndParent);
        }

        int IPropertyPage.Apply() {
            try {
                Apply();
                return VSConstants.S_OK;
            } catch (Exception e) {
                return Marshal.GetHRForException(e);
            }
        }

        void IPropertyPage.Deactivate() {
            Project = null;
            Control.Dispose();
        }

        void IPropertyPage.GetPageInfo(PROPPAGEINFO[] pPageInfo) {
            Utilities.ArgumentNotNull("pPageInfo", pPageInfo);

            PROPPAGEINFO info = new PROPPAGEINFO();

            info.cb = (uint)Marshal.SizeOf(typeof(PROPPAGEINFO));
            info.dwHelpContext = 0;
            info.pszDocString = null;
            info.pszHelpFile = null;
            info.pszTitle = Name;
            info.SIZE.cx = Control.Width;
            info.SIZE.cy = Control.Height;
            pPageInfo[0] = info;
        }

        void IPropertyPage.Help(string pszHelpDir) {
        }

        int IPropertyPage.IsPageDirty() {
            return (IsDirty ? (int)VSConstants.S_OK : (int)VSConstants.S_FALSE);
        }

        void IPropertyPage.Move(RECT[] pRect) {
            Utilities.ArgumentNotNull("pRect", pRect);

            RECT r = pRect[0];

            Control.Location = new Point(r.left, r.top);
            Control.Size = new Size(r.right - r.left, r.bottom - r.top);
        }

        void IPropertyPage.SetObjects(uint count, object[] punk) {
            if (punk == null) {
                return;
            }

            if (count > 0) {
                if (punk[0] is ProjectConfig) {
                    if (_project == null) {
                        _project = (CommonProjectNode)((CommonProjectConfig)punk.First()).ProjectMgr;
                    }

                    var configs = new List<CommonProjectConfig>();

                    for (int i = 0; i < count; i++) {
                        CommonProjectConfig config = (CommonProjectConfig)punk[i];

                        configs.Add(config);
                    }

                    SelectedConfigs = configs;
                } else if (punk[0] is NodeProperties) {
                    if (_project == null) {
                        Project = (CommonProjectNode)(punk[0] as NodeProperties).HierarchyNode.ProjectMgr;
                    }
                }
            } else {
                Project = null;
            }

            if (_project != null) {
                LoadSettings();
            }
        }

        void IPropertyPage.SetPageSite(IPropertyPageSite pPageSite) {
            _site = pPageSite;
        }

        void IPropertyPage.Show(uint nCmdShow) {
            Control.Visible = true; // TODO: pass SW_SHOW* flags through      
            Control.Show();
        }

        int IPropertyPage.TranslateAccelerator(MSG[] pMsg) {
            Utilities.ArgumentNotNull("pMsg", pMsg);

            MSG msg = pMsg[0];

            if ((msg.message < NativeMethods.WM_KEYFIRST || msg.message > NativeMethods.WM_KEYLAST) && (msg.message < NativeMethods.WM_MOUSEFIRST || msg.message > NativeMethods.WM_MOUSELAST)) {
                return VSConstants.S_FALSE;
            }

            return (NativeMethods.IsDialogMessageA(Control.Handle, ref msg)) ? VSConstants.S_OK : VSConstants.S_FALSE;
        }
    }
}
