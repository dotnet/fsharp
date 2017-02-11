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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudioTools.Project.Automation;

namespace Microsoft.VisualStudioTools.Project {
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class CommonProjectNodeProperties : ProjectNodeProperties, IVsCfgBrowseObject, VSLangProj.ProjectProperties {
        private OAProjectConfigurationProperties _activeCfgSettings;

        internal CommonProjectNodeProperties(ProjectNode node)
            : base(node) {
        }

        #region properties
        /// <summary>
        /// Returns/Sets the StartupFile project property
        /// </summary>
        [SRCategoryAttribute(SR.General)]
        [SRDisplayName(SR.StartupFile)]
        [SRDescriptionAttribute(SR.StartupFileDescription)]
        public string StartupFile {
            get {
                return Node.Site.GetUIThread().Invoke(() => {
                    var res = Node.ProjectMgr.GetProjectProperty(CommonConstants.StartupFile, true);
                    if (res != null && !Path.IsPathRooted(res)) {
                        res = CommonUtils.GetAbsoluteFilePath(Node.ProjectMgr.ProjectHome, res);
                    }
                    return res;
                });
            }
            set {
                Node.Site.GetUIThread().Invoke(() => {
                    this.Node.ProjectMgr.SetProjectProperty(
                        CommonConstants.StartupFile,
                        CommonUtils.GetRelativeFilePath(
                            Node.ProjectMgr.ProjectHome,
                            Path.Combine(Node.ProjectMgr.ProjectHome, value)
                        )
                    );
                });
            }
        }

        /// <summary>
        /// Returns/Sets the WorkingDirectory project property
        /// </summary>
        [SRCategoryAttribute(SR.General)]
        [SRDisplayName(SR.WorkingDirectory)]
        [SRDescriptionAttribute(SR.WorkingDirectoryDescription)]
        public string WorkingDirectory {
            get {
                return Node.Site.GetUIThread().Invoke(() => {
                    return this.Node.ProjectMgr.GetProjectProperty(CommonConstants.WorkingDirectory, true);
                });
            }
            set {
                Node.Site.GetUIThread().Invoke(() => {
                    this.Node.ProjectMgr.SetProjectProperty(CommonConstants.WorkingDirectory, value);
                });
            }
        }

        /// <summary>
        /// Returns/Sets the PublishUrl project property which is where the project is published to
        /// </summary>
        [Browsable(false)]
        public string PublishUrl {
            get {
                return Node.Site.GetUIThread().Invoke(() => {
                    return this.Node.ProjectMgr.GetProjectProperty(CommonConstants.PublishUrl, true);
                });
            }
            set {
                Node.Site.GetUIThread().Invoke(() => {
                    this.Node.ProjectMgr.SetProjectProperty(CommonConstants.PublishUrl, value);
                });
            }
        }

        //We don't need this property, but still have to provide it, otherwise
        //Add New Item wizard (which seems to be unmanaged) fails.
        [Browsable(false)]
        public string RootNamespace {
            get {
                return "";
            }
            set {
                //Do nothing
            }
        }

        /// <summary>
        /// Gets the home directory for the project.
        /// </summary>
        [SRCategoryAttribute(SR.Misc)]
        [SRDisplayName(SR.ProjectHome)]
        [SRDescriptionAttribute(SR.ProjectHomeDescription)]
        public string ProjectHome {
            get {
                return Node.ProjectMgr.ProjectHome;
            }
        }

        #endregion

        #region IVsCfgBrowseObject Members

        int IVsCfgBrowseObject.GetCfg(out IVsCfg ppCfg) {
            return Node.ProjectMgr.ConfigProvider.GetCfgOfName(
                Node.ProjectMgr.CurrentConfig.GetPropertyValue(ProjectFileConstants.Configuration),
                Node.ProjectMgr.CurrentConfig.GetPropertyValue(ProjectFileConstants.Platform),
                out ppCfg);
        }

        #endregion

        #region ProjectProperties Members

        [Browsable(false)]
        public string AbsoluteProjectDirectory {
            get {
                return Node.ProjectMgr.ProjectFolder;
            }
        }

        [Browsable(false)]
        public VSLangProj.ProjectConfigurationProperties ActiveConfigurationSettings {
            get {
                if (_activeCfgSettings == null) {
                    _activeCfgSettings = new OAProjectConfigurationProperties(Node.ProjectMgr);
                }
                return _activeCfgSettings;
            }
        }

        [Browsable(false)]
        public string ActiveFileSharePath {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public VSLangProj.prjWebAccessMethod ActiveWebAccessMethod {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public string ApplicationIcon {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public string AssemblyKeyContainerName {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public string AssemblyName {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public string AssemblyOriginatorKeyFile {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public VSLangProj.prjOriginatorKeyMode AssemblyOriginatorKeyMode {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public VSLangProj.prjScriptLanguage DefaultClientScript {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public VSLangProj.prjHTMLPageLayout DefaultHTMLPageLayout {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public string DefaultNamespace {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public VSLangProj.prjTargetSchema DefaultTargetSchema {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public bool DelaySign {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public new object ExtenderNames {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public string FileSharePath {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public bool LinkRepair {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public string LocalPath {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public string OfflineURL {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public VSLangProj.prjCompare OptionCompare {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public VSLangProj.prjOptionExplicit OptionExplicit {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public VSLangProj.prjOptionStrict OptionStrict {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public string OutputFileName {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public VSLangProj.prjOutputType OutputType {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public VSLangProj.prjProjectType ProjectType {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public string ReferencePath {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public string ServerExtensionsVersion {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public string StartupObject {
            get {
                return Node.Site.GetUIThread().Invoke(() => {
                    return Node.ProjectMgr.GetProjectProperty(CommonConstants.StartupFile);
                });
            }
            set {
                Node.Site.GetUIThread().Invoke(() => {
                    Node.ProjectMgr.SetProjectProperty(
                        CommonConstants.StartupFile,
                        CommonUtils.GetRelativeFilePath(Node.ProjectMgr.ProjectHome, value)
                    );
                });
            }
        }

        [Browsable(false)]
        public string URL {
            get { return CommonUtils.MakeUri(Node.ProjectMgr.Url, false, UriKind.Absolute).AbsoluteUri; }
        }

        [Browsable(false)]
        public VSLangProj.prjWebAccessMethod WebAccessMethod {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        [Browsable(false)]
        public string WebServer {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public string WebServerVersion {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public string __id {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public object __project {
            get { throw new NotImplementedException(); }
        }

        [Browsable(false)]
        public object get_Extender(string ExtenderName) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
