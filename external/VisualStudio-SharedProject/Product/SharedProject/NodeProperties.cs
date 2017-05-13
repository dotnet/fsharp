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
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using VSLangProj;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudioTools.Project {

    /// <summary>
    /// All public properties on Nodeproperties or derived classes are assumed to be used by Automation by default.
    /// Set this attribute to false on Properties that should not be visible for Automation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class AutomationBrowsableAttribute : System.Attribute {
        public AutomationBrowsableAttribute(bool browsable) {
            this.browsable = browsable;
        }

        public bool Browsable {
            get {
                return this.browsable;
            }
        }

        private bool browsable;
    }

    /// <summary>
    /// This attribute is used to mark properties that shouldn't be serialized.  Marking properties with this will
    /// result in them not being serialized and not being bold in the properties pane.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class AlwaysSerializedAttribute : Attribute {
        public AlwaysSerializedAttribute() { }
    }

    /// <summary>
    /// To create your own localizable node properties, subclass this and add public properties
    /// decorated with your own localized display name, category and description attributes.
    /// </summary>
    [ComVisible(true)]
    public class NodeProperties : LocalizableProperties,
        ISpecifyPropertyPages,
        IVsGetCfgProvider,
        IVsSpecifyProjectDesignerPages,
        IVsBrowseObject {
        #region fields
        private HierarchyNode node;
        #endregion

        #region properties
        [Browsable(false)]
        [AutomationBrowsable(true)]
        public object Node {
            get { return this.node; }
        }

        internal HierarchyNode HierarchyNode {
            get { return this.node; }
        }


        /// <summary>
        /// Used by Property Pages Frame to set it's title bar. The Caption of the Hierarchy Node is returned.
        /// </summary>
        [Browsable(false)]
        [AutomationBrowsable(false)]
        public virtual string Name {
            get { return this.node.Caption; }
        }

        #endregion

        #region ctors
        internal NodeProperties(HierarchyNode node) {
            Utilities.ArgumentNotNull("node", node);
            this.node = node;
        }
        #endregion

        #region ISpecifyPropertyPages methods
        public virtual void GetPages(CAUUID[] pages) {
            this.GetCommonPropertyPages(pages);
        }
        #endregion

        #region IVsSpecifyProjectDesignerPages
        /// <summary>
        /// Implementation of the IVsSpecifyProjectDesignerPages. It will retun the pages that are configuration independent.
        /// </summary>
        /// <param name="pages">The pages to return.</param>
        /// <returns></returns>
        public virtual int GetProjectDesignerPages(CAUUID[] pages) {
            this.GetCommonPropertyPages(pages);
            return VSConstants.S_OK;
        }
        #endregion

        #region IVsGetCfgProvider methods
        public virtual int GetCfgProvider(out IVsCfgProvider p) {
            p = null;
            return VSConstants.E_NOTIMPL;
        }
        #endregion

        #region IVsBrowseObject methods
        /// <summary>
        /// Maps back to the hierarchy or project item object corresponding to the browse object.
        /// </summary>
        /// <param name="hier">Reference to the hierarchy object.</param>
        /// <param name="itemid">Reference to the project item.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public virtual int GetProjectItem(out IVsHierarchy hier, out uint itemid) {
            Utilities.CheckNotNull(node);

            hier = node.ProjectMgr.GetOuterInterface<IVsHierarchy>();
            itemid = this.node.ID;
            return VSConstants.S_OK;
        }
        #endregion

        #region overridden methods
        /// <summary>
        /// Get the Caption of the Hierarchy Node instance. If Caption is null or empty we delegate to base
        /// </summary>
        /// <returns>Caption of Hierarchy node instance</returns>
        public override string GetComponentName() {
            string caption = this.HierarchyNode.Caption;
            if (string.IsNullOrEmpty(caption)) {
                return base.GetComponentName();
            } else {
                return caption;
            }
        }
        #endregion

        #region helper methods
        protected string GetProperty(string name, string def) {
            string a = this.HierarchyNode.ItemNode.GetMetadata(name);
            return (a == null) ? def : a;
        }

        protected void SetProperty(string name, string value) {
            this.HierarchyNode.ItemNode.SetMetadata(name, value);
        }

        /// <summary>
        /// Retrieves the common property pages. The NodeProperties is the BrowseObject and that will be called to support 
        /// configuration independent properties.
        /// </summary>
        /// <param name="pages">The pages to return.</param>
        private void GetCommonPropertyPages(CAUUID[] pages) {
            // We do not check whether the supportsProjectDesigner is set to false on the ProjectNode.
            // We rely that the caller knows what to call on us.
            Utilities.ArgumentNotNull("pages", pages);

            if (pages.Length == 0) {
                throw new ArgumentException(SR.GetString(SR.InvalidParameter), "pages");
            }

            // Only the project should show the property page the rest should show the project properties.
            if (this.node != null && (this.node is ProjectNode)) {
                // Retrieve the list of guids from hierarchy properties.
                // Because a flavor could modify that list we must make sure we are calling the outer most implementation of IVsHierarchy
                string guidsList = String.Empty;
                IVsHierarchy hierarchy = HierarchyNode.ProjectMgr.GetOuterInterface<IVsHierarchy>();
                object variant = null;
                ErrorHandler.ThrowOnFailure(hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID2.VSHPROPID_PropertyPagesCLSIDList, out variant));
                guidsList = (string)variant;

                Guid[] guids = Utilities.GuidsArrayFromSemicolonDelimitedStringOfGuids(guidsList);
                if (guids == null || guids.Length == 0) {
                    pages[0] = new CAUUID();
                    pages[0].cElems = 0;
                } else {
                    pages[0] = PackageUtilities.CreateCAUUIDFromGuidArray(guids);
                }
            } else {
                pages[0] = new CAUUID();
                pages[0].cElems = 0;
            }
        }
        #endregion

        #region ExtenderSupport
        [Browsable(false)]
        public virtual string ExtenderCATID {
            get {
                Guid catid = this.HierarchyNode.ProjectMgr.GetCATIDForType(this.GetType());
                if (Guid.Empty.CompareTo(catid) == 0) {
                    return null;
                }
                return catid.ToString("B");
            }
        }

        [Browsable(false)]
        public object ExtenderNames() {
            EnvDTE.ObjectExtenders extenderService = (EnvDTE.ObjectExtenders)this.HierarchyNode.GetService(typeof(EnvDTE.ObjectExtenders));
            Utilities.CheckNotNull(extenderService, "Could not get the ObjectExtenders object from the services exposed by this property object");

            return extenderService.GetExtenderNames(this.ExtenderCATID, this);
        }

        public object Extender(string extenderName) {
            EnvDTE.ObjectExtenders extenderService = (EnvDTE.ObjectExtenders)this.HierarchyNode.GetService(typeof(EnvDTE.ObjectExtenders));
            Utilities.CheckNotNull(extenderService, "Could not get the ObjectExtenders object from the services exposed by this property object");
            return extenderService.GetExtender(this.ExtenderCATID, extenderName, this);
        }

        #endregion
    }

    [ComVisible(true)]
    public class FileNodeProperties : NodeProperties {
        #region properties

        [SRCategoryAttribute(SR.Misc)]
        [SRDisplayName(SR.FileName)]
        [SRDescriptionAttribute(SR.FileNameDescription)]
        [AlwaysSerialized]
        public virtual string FileName {
            get {
                return this.HierarchyNode.Caption;
            }
            set {
                this.HierarchyNode.SetEditLabel(value);
            }
        }

        [SRCategoryAttribute(SR.Misc)]
        [SRDisplayName(SR.FullPath)]
        [SRDescriptionAttribute(SR.FullPathDescription)]
        public string FullPath {
            get {
                return this.HierarchyNode.Url;
            }
        }

        #region non-browsable properties - used for automation only

        [Browsable(false)]
        public string URL {
            get {
                return this.HierarchyNode.Url;
            }
        }

        [Browsable(false)]
        public string Extension {
            get {
                return Path.GetExtension(this.HierarchyNode.Caption);
            }
        }

        [Browsable(false)]
        public bool IsLinkFile {
            get {
                return HierarchyNode.IsLinkFile;
            }
        }

        #endregion

        #endregion

        #region ctors
        internal FileNodeProperties(HierarchyNode node)
            : base(node) {
        }
        #endregion

        public override string GetClassName() {
            return SR.GetString(SR.FileProperties);
        }
    }

    [ComVisible(true)]
    public class ExcludedFileNodeProperties : FileNodeProperties {
        internal ExcludedFileNodeProperties(HierarchyNode node)
            : base(node) {
        }

        [SRCategoryAttribute(SR.Advanced)]
        [SRDisplayName(SR.BuildAction)]
        [SRDescriptionAttribute(SR.BuildActionDescription)]
        [TypeConverter(typeof(BuildActionTypeConverter))]
        public prjBuildAction BuildAction {
            get {
                return prjBuildAction.prjBuildActionNone;
            }
        }
    }

    [ComVisible(true)]
    public class IncludedFileNodeProperties : FileNodeProperties {
        internal IncludedFileNodeProperties(HierarchyNode node)
            : base(node) {
        }

        /// <summary>
        /// Specifies the build action as a string so the user can configure it to any value.
        /// </summary>
        [SRCategoryAttribute(SR.Advanced)]
        [SRDisplayName(SR.BuildAction)]
        [SRDescriptionAttribute(SR.BuildActionDescription)]
        [AlwaysSerialized]
        [TypeConverter(typeof(BuildActionStringConverter))]
        public string ItemType {
            get {
                return HierarchyNode.ItemNode.ItemTypeName;
            }
            set {
                HierarchyNode.ItemNode.ItemTypeName = value;
            }
        }

        /// <summary>
        /// Specifies the build action as a projBuildAction so that automation can get the
        /// expected enum value.
        /// </summary>
        [Browsable(false)]
        public prjBuildAction BuildAction {
            get {
                var res = BuildActionTypeConverter.Instance.ConvertFromString(HierarchyNode.ItemNode.ItemTypeName);
                if (res is prjBuildAction) {
                    return (prjBuildAction)res;
                }
                return prjBuildAction.prjBuildActionContent;
            }
            set {
                this.HierarchyNode.ItemNode.ItemTypeName = BuildActionTypeConverter.Instance.ConvertToString(value);
            }
        }

        [SRCategoryAttribute(SR.Advanced)]
        [SRDisplayName(SR.Publish)]
        [SRDescriptionAttribute(SR.PublishDescription)]
        public bool Publish {
            get {
                var publish = this.HierarchyNode.ItemNode.GetMetadata("Publish");
                if (String.IsNullOrEmpty(publish)) {
                    if (this.HierarchyNode.ItemNode.ItemTypeName == ProjectFileConstants.Compile) {
                        return true;
                    }
                    return false;
                }
                return Convert.ToBoolean(publish);
            }
            set {
                this.HierarchyNode.ItemNode.SetMetadata("Publish", value.ToString());
            }
        }

        [Browsable(false)]
        public bool ShouldSerializePublish() {
            // If compile, default should be true, else the default is false.
            if (HierarchyNode.ItemNode.ItemTypeName == ProjectFileConstants.Compile) {
                return !Publish;
            }
            return Publish;
        }

        [Browsable(false)]
        public void ResetPublish() {
            // If compile, default should be true, else the default is false.
            if (HierarchyNode.ItemNode.ItemTypeName == ProjectFileConstants.Compile) {
                Publish = true;
            }
            Publish = false;
        }

        [Browsable(false)]
        public string SourceControlStatus {
            get {
                // remove STATEICON_ and return rest of enum
                return HierarchyNode.StateIconIndex.ToString().Substring(10);
            }
        }

        [Browsable(false)]
        public string SubType {
            get {
                return this.HierarchyNode.ItemNode.GetMetadata("SubType");
            }
            set {
                this.HierarchyNode.ItemNode.SetMetadata("SubType", value.ToString());
            }
        }
    }

    [ComVisible(true)]
    public class LinkFileNodeProperties : FileNodeProperties {
        internal LinkFileNodeProperties(HierarchyNode node)
            : base(node) {

        }

        /// <summary>
        /// Specifies the build action as a string so the user can configure it to any value.
        /// </summary>
        [SRCategoryAttribute(SR.Advanced)]
        [SRDisplayName(SR.BuildAction)]
        [SRDescriptionAttribute(SR.BuildActionDescription)]
        [AlwaysSerialized]
        [TypeConverter(typeof(BuildActionStringConverter))]
        public string ItemType {
            get {
                return HierarchyNode.ItemNode.ItemTypeName;
            }
            set {
                HierarchyNode.ItemNode.ItemTypeName = value;
            }
        }

        [SRCategoryAttribute(SR.Misc)]
        [SRDisplayName(SR.FileName)]
        [SRDescriptionAttribute(SR.FileNameDescription)]
        [ReadOnly(true)]
        public override string FileName {
            get {
                return this.HierarchyNode.Caption;
            }
            set {
                throw new InvalidOperationException();
            }
        }
    }

    [ComVisible(true)]
    public class DependentFileNodeProperties : NodeProperties {
        #region properties

        [SRCategoryAttribute(SR.Misc)]
        [SRDisplayName(SR.FileName)]
        [SRDescriptionAttribute(SR.FileNameDescription)]
        public virtual string FileName {
            get {
                return this.HierarchyNode.Caption;
            }
        }

        [SRCategoryAttribute(SR.Misc)]
        [SRDisplayName(SR.FullPath)]
        [SRDescriptionAttribute(SR.FullPathDescription)]
        public string FullPath {
            get {
                return this.HierarchyNode.Url;
            }
        }
        #endregion

        #region ctors
        internal DependentFileNodeProperties(HierarchyNode node)
            : base(node) {
        }

        #endregion

        public override string GetClassName() {
            return SR.GetString(SR.FileProperties);
        }
    }

    class BuildActionTypeConverter : StringConverter {
        internal static readonly BuildActionTypeConverter Instance = new BuildActionTypeConverter();

        public BuildActionTypeConverter() {
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if (sourceType == typeof(string)) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == typeof(string)) {
                switch ((prjBuildAction)value) {
                    case prjBuildAction.prjBuildActionCompile:
                        return ProjectFileConstants.Compile;
                    case prjBuildAction.prjBuildActionContent:
                        return ProjectFileConstants.Content;
                    case prjBuildAction.prjBuildActionNone:
                        return ProjectFileConstants.None;
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value is string) {
                string strVal = (string)value;
                if (strVal.Equals(ProjectFileConstants.Compile, StringComparison.OrdinalIgnoreCase)) {
                    return prjBuildAction.prjBuildActionCompile;
                } else if (strVal.Equals(ProjectFileConstants.Content, StringComparison.OrdinalIgnoreCase)) {
                    return prjBuildAction.prjBuildActionContent;
                } else if (strVal.Equals(ProjectFileConstants.None, StringComparison.OrdinalIgnoreCase)) {
                    return prjBuildAction.prjBuildActionNone;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            return new StandardValuesCollection(new[] { prjBuildAction.prjBuildActionNone, prjBuildAction.prjBuildActionCompile, prjBuildAction.prjBuildActionContent });
        }
    }

    /// <summary>
    /// This type converter doesn't really do any conversions, but allows us to provide
    /// a list of standard values for the build action.
    /// </summary>
    class BuildActionStringConverter : StringConverter {
        internal static readonly BuildActionStringConverter Instance = new BuildActionStringConverter();

        public BuildActionStringConverter() {
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if (sourceType == typeof(string)) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            return value;
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            return value;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            FileNodeProperties nodeProps = context.Instance as FileNodeProperties;
            IEnumerable<string> itemNames;
            if (nodeProps != null) {
                itemNames = nodeProps.HierarchyNode.ProjectMgr.GetAvailableItemNames();
            } else {
                itemNames = new[] { ProjectFileConstants.None, ProjectFileConstants.Compile, ProjectFileConstants.Content };
            }
            return new StandardValuesCollection(itemNames.ToArray());
        }
    }

    [ComVisible(true)]
    public class ProjectNodeProperties : NodeProperties, EnvDTE80.IInternalExtenderProvider {
        #region properties
        [SRCategoryAttribute(SR.Misc)]
        [SRDisplayName(SR.ProjectFolder)]
        [SRDescriptionAttribute(SR.ProjectFolderDescription)]
        [AutomationBrowsable(false)]
        public string ProjectFolder {
            get {
                return this.Node.ProjectMgr.ProjectFolder;
            }
        }

        [SRCategoryAttribute(SR.Misc)]
        [SRDisplayName(SR.ProjectFile)]
        [SRDescriptionAttribute(SR.ProjectFileDescription)]
        [AutomationBrowsable(false)]
        public string ProjectFile {
            get {
                return this.Node.ProjectMgr.ProjectFile;
            }
            set {
                this.Node.ProjectMgr.ProjectFile = value;
            }
        }

        #region non-browsable properties - used for automation only
        [Browsable(false)]
        public string Guid {
            get {
                return this.Node.ProjectMgr.ProjectIDGuid.ToString();
            }
        }

        [Browsable(false)]
        public string FileName {
            get {
                return this.Node.ProjectMgr.ProjectFile;
            }
            set {
                this.Node.ProjectMgr.ProjectFile = value;
            }
        }


        [Browsable(false)]
        public string FullPath {
            get {
                return CommonUtils.NormalizeDirectoryPath(this.Node.ProjectMgr.ProjectFolder);
            }
        }
        #endregion

        #endregion

        #region ctors
        internal ProjectNodeProperties(ProjectNode node)
            : base(node) {
        }

        internal new ProjectNode Node {
            get {
                return (ProjectNode)base.Node;
            }
        }

        #endregion

        #region overridden methods

        /// <summary>
        /// ICustomTypeDescriptor.GetEditor
        /// To enable the "Property Pages" button on the properties browser
        /// the browse object (project properties) need to be unmanaged
        /// or it needs to provide an editor of type ComponentEditor.
        /// </summary>
        /// <param name="editorBaseType">Type of the editor</param>
        /// <returns>Editor</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "The service provider is used by the PropertiesEditorLauncher")]
        public override object GetEditor(Type editorBaseType) {
            // Override the scenario where we are asked for a ComponentEditor
            // as this is how the Properties Browser calls us
            if (editorBaseType == typeof(ComponentEditor)) {
                IOleServiceProvider sp;
                ErrorHandler.ThrowOnFailure(Node.ProjectMgr.GetSite(out sp));
                return new PropertiesEditorLauncher(new ServiceProvider(sp));
            }

            return base.GetEditor(editorBaseType);
        }

        public override int GetCfgProvider(out IVsCfgProvider p) {
            if (this.Node != null && this.Node.ProjectMgr != null) {
                return this.Node.ProjectMgr.GetCfgProvider(out p);
            }

            return base.GetCfgProvider(out p);
        }

        public override string GetClassName() {
            return SR.GetString(SR.ProjectProperties);
        }

        #endregion

        #region IInternalExtenderProvider Members

        bool EnvDTE80.IInternalExtenderProvider.CanExtend(string extenderCATID, string extenderName, object extendeeObject) {
            EnvDTE80.IInternalExtenderProvider outerHierarchy = Node.GetOuterInterface<EnvDTE80.IInternalExtenderProvider>();

            if (outerHierarchy != null) {
                return outerHierarchy.CanExtend(extenderCATID, extenderName, extendeeObject);
            }
            return false;
        }

        object EnvDTE80.IInternalExtenderProvider.GetExtender(string extenderCATID, string extenderName, object extendeeObject, EnvDTE.IExtenderSite extenderSite, int cookie) {
            EnvDTE80.IInternalExtenderProvider outerHierarchy = Node.GetOuterInterface<EnvDTE80.IInternalExtenderProvider>();

            if (outerHierarchy != null) {
                return outerHierarchy.GetExtender(extenderCATID, extenderName, extendeeObject, extenderSite, cookie);
            }

            return null;
        }

        object EnvDTE80.IInternalExtenderProvider.GetExtenderNames(string extenderCATID, object extendeeObject) {
            return null;
        }

        #endregion
    }

    [ComVisible(true)]
    public class FolderNodeProperties : NodeProperties {
        #region properties
        [SRCategoryAttribute(SR.Misc)]
        [SRDisplayName(SR.FolderName)]
        [SRDescriptionAttribute(SR.FolderNameDescription)]
        public string FolderName {
            get {
                return this.HierarchyNode.Caption;
            }
            set {
                HierarchyNode.ProjectMgr.Site.GetUIThread().Invoke(() => {
                    this.HierarchyNode.SetEditLabel(value);
                    this.HierarchyNode.ProjectMgr.ReDrawNode(HierarchyNode, UIHierarchyElement.Caption);
                });
            }
        }

        #region properties - used for automation only
        [Browsable(false)]
        [AutomationBrowsable(true)]
        public string FileName {
            get {
                return this.HierarchyNode.Caption;
            }
            set {
                HierarchyNode.ProjectMgr.Site.GetUIThread().Invoke(() => {
                    this.HierarchyNode.SetEditLabel(value);
                    this.HierarchyNode.ProjectMgr.ReDrawNode(HierarchyNode, UIHierarchyElement.Caption);
                });
            }
        }

        [Browsable(true)]
        [AutomationBrowsable(true)]
        [SRCategoryAttribute(SR.Misc)]
        [SRDisplayName(SR.FullPath)]
        [SRDescriptionAttribute(SR.FullPathDescription)]
        public string FullPath {
            get {
                return CommonUtils.NormalizeDirectoryPath(this.HierarchyNode.GetMkDocument());
            }
        }
        #endregion

        #endregion

        #region ctors
        internal FolderNodeProperties(HierarchyNode node)
            : base(node) {
        }
        #endregion

        public override string GetClassName() {
            return SR.GetString(SR.FolderProperties);
        }
    }

    [CLSCompliant(false), ComVisible(true)]
    public class ReferenceNodeProperties : NodeProperties {
        #region properties
        [SRCategoryAttribute(SR.Misc)]
        [SRDisplayName(SR.RefName)]
        [SRDescriptionAttribute(SR.RefNameDescription)]
        [Browsable(true)]
        [AutomationBrowsable(true)]
        public override string Name {
            get {
                return this.HierarchyNode.Caption;
            }
        }

        [SRCategoryAttribute(SR.Misc)]
        [SRDisplayName(SR.CopyToLocal)]
        [SRDescriptionAttribute(SR.CopyToLocalDescription)]
        public bool CopyToLocal {
            get {
                string copyLocal = this.GetProperty(ProjectFileConstants.Private, "False");
                if (copyLocal == null || copyLocal.Length == 0)
                    return true;
                return bool.Parse(copyLocal);
            }
            set {
                this.SetProperty(ProjectFileConstants.Private, value.ToString());
            }
        }

        [SRCategoryAttribute(SR.Misc)]
        [SRDisplayName(SR.FullPath)]
        [SRDescriptionAttribute(SR.FullPathDescription)]
        public virtual string FullPath {
            get {
                return this.HierarchyNode.Url;
            }
        }
        #endregion

        #region ctors
        internal ReferenceNodeProperties(HierarchyNode node)
            : base(node) {
        }
        #endregion

        #region overridden methods
        public override string GetClassName() {
            return SR.GetString(SR.ReferenceProperties);
        }
        #endregion
    }

    [ComVisible(true)]
    public class ProjectReferencesProperties : ReferenceNodeProperties {
        #region ctors
        internal ProjectReferencesProperties(ProjectReferenceNode node)
            : base(node) {
        }
        #endregion

        #region overriden methods
        public override string FullPath {
            get {
                return ((ProjectReferenceNode)Node).ReferencedProjectOutputPath;
            }
        }
        #endregion
    }
}
