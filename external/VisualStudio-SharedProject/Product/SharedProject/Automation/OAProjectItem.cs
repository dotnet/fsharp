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
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.Project.Automation {
    [ComVisible(true)]
    public class OAProjectItem : EnvDTE.ProjectItem {

        #region fields
        private HierarchyNode node;
        private OAProject project;
        #endregion

        #region properties
        internal HierarchyNode Node {
            get {
                return this.node;
            }
        }

        /// <summary>
        /// Returns the automation project
        /// </summary>
        protected OAProject Project {
            get {
                return this.project;
            }
        }
        #endregion

        #region ctors
        internal OAProjectItem(OAProject project, HierarchyNode node) {
            this.node = node;
            this.project = project;
        }
        #endregion

        #region EnvDTE.ProjectItem

        /// <summary>
        /// Gets the requested Extender if it is available for this object
        /// </summary>
        /// <param name="extenderName">The name of the extender.</param>
        /// <returns>The extender object.</returns>
        public virtual object get_Extender(string extenderName) {
            return null;
        }

        /// <summary>
        /// Gets an object that can be accessed by name at run time.
        /// </summary>
        public virtual object Object {
            get {
                return this.node.Object;
            }
        }

        /// <summary>
        /// Gets the Document associated with the item, if one exists.
        /// </summary>
        public virtual EnvDTE.Document Document {
            get {
                return null;
            }
        }

        /// <summary>
        /// Gets the number of files associated with a ProjectItem.
        /// </summary>
        public virtual short FileCount {
            get {
                return (short)1;
            }
        }

        /// <summary>
        /// Gets a collection of all properties that pertain to the object. 
        /// </summary>
        public virtual EnvDTE.Properties Properties {
            get {
                if (this.node.NodeProperties == null) {
                    return null;
                }
                return new OAProperties(this.node.NodeProperties);
            }
        }


        /// <summary>
        /// Gets the FileCodeModel object for the project item.
        /// </summary>
        public virtual EnvDTE.FileCodeModel FileCodeModel {
            get {
                return null;
            }
        }

        /// <summary>
        /// Gets a ProjectItems for the object.
        /// </summary>
        public virtual EnvDTE.ProjectItems ProjectItems {
            get {
                return null;
            }
        }

        /// <summary>
        /// Gets a GUID string indicating the kind or type of the object.
        /// </summary>
        public virtual string Kind {
            get {
                Guid guid;
                ErrorHandler.ThrowOnFailure(this.node.GetGuidProperty((int)__VSHPROPID.VSHPROPID_TypeGuid, out guid));
                return guid.ToString("B");
            }
        }

        /// <summary>
        /// Saves the project item. 
        /// </summary>
        /// <param name="fileName">The name with which to save the project or project item.</param>
        /// <remarks>Implemented by subclasses.</remarks>
        public virtual void Save(string fileName) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the top-level extensibility object.
        /// </summary>
        public virtual EnvDTE.DTE DTE {
            get {
                return (EnvDTE.DTE)this.project.DTE;
            }
        }

        /// <summary>
        /// Gets the ProjectItems collection containing the ProjectItem object supporting this property.
        /// </summary>
        public virtual EnvDTE.ProjectItems Collection {
            get {
                // Get the parent node
                HierarchyNode parentNode = this.node.Parent;
                Debug.Assert(parentNode != null, "Failed to get the parent node");

                // Get the ProjectItems object for the parent node
                if (parentNode is ProjectNode) {
                    // The root node for the project
                    return ((OAProject)parentNode.GetAutomationObject()).ProjectItems;
                } else {
                    // Not supported. Override this method in derived classes to return appropriate collection object
                    throw new InvalidOperationException();
                }
            }
        }
        /// <summary>
        /// Gets a list of available Extenders for the object.
        /// </summary>
        public virtual object ExtenderNames {
            get {
                return null;
            }
        }

        /// <summary>
        /// Gets the ConfigurationManager object for this ProjectItem. 
        /// </summary>
        /// <remarks>We do not support config management based per item.</remarks>
        public virtual EnvDTE.ConfigurationManager ConfigurationManager {
            get {
                return null;
            }
        }

        /// <summary>
        /// Gets the project hosting the ProjectItem.
        /// </summary>
        public virtual EnvDTE.Project ContainingProject {
            get {
                return this.project;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the object has been modified since last being saved or opened.
        /// </summary>
        public virtual bool Saved {
            get {
                return !this.IsDirty;

            }
            set {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the Extender category ID (CATID) for the object.
        /// </summary>
        public virtual string ExtenderCATID {
            get {
                return null;
            }
        }

        /// <summary>
        /// If the project item is the root of a subproject, then the SubProject property returns the Project object for the subproject.
        /// </summary>
        public virtual EnvDTE.Project SubProject {
            get {
                return null;
            }
        }

        /// <summary>
        /// Microsoft Internal Use Only. Checks if the document associated to this item is dirty.
        /// </summary>
        public virtual bool IsDirty {
            get {
                return false;
            }
            set {
            }
        }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public virtual string Name {
            get {
                return this.node.Caption;
            }
            set {
                CheckProjectIsValid();

                using (AutomationScope scope = new AutomationScope(this.Node.ProjectMgr.Site)) {
                    Node.ProjectMgr.Site.GetUIThread().Invoke(() => this.node.SetEditLabel(value));
                }
            }
        }

        protected void CheckProjectIsValid() {
            Utilities.CheckNotNull(this.node);
            Utilities.CheckNotNull(this.node.ProjectMgr);
            Utilities.CheckNotNull(this.node.ProjectMgr.Site);
            if (this.node.ProjectMgr.IsClosed) {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Removes the project item from hierarchy.
        /// </summary>
        public virtual void Remove() {
            CheckProjectIsValid();

            using (AutomationScope scope = new AutomationScope(this.Node.ProjectMgr.Site)) {
                Node.ProjectMgr.Site.GetUIThread().Invoke(() => this.node.Remove(false));
            }
        }

        /// <summary>
        /// Removes the item from its project and its storage. 
        /// </summary>
        public virtual void Delete() {
            CheckProjectIsValid();

            using (AutomationScope scope = new AutomationScope(this.Node.ProjectMgr.Site)) {
                Node.ProjectMgr.Site.GetUIThread().Invoke(() => this.node.Remove(true));
            }
        }

        /// <summary>
        /// Saves the project item.
        /// </summary>
        /// <param name="newFileName">The file name with which to save the solution, project, or project item. If the file exists, it is overwritten.</param>
        /// <returns>true if save was successful</returns>
        /// <remarks>This method is implemented on subclasses.</remarks>
        public virtual bool SaveAs(string newFileName) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating whether the project item is open in a particular view type. 
        /// </summary>
        /// <param name="viewKind">A Constants.vsViewKind* indicating the type of view to check./param>
        /// <returns>A Boolean value indicating true if the project is open in the given view type; false if not. </returns>
        public virtual bool get_IsOpen(string viewKind) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the full path and names of the files associated with a project item.
        /// </summary>
        /// <param name="index"> The index of the item</param>
        /// <returns>The full path of the associated item</returns>
        /// <exception cref="ArgumentOutOfRangeException">Is thrown if index is not one</exception>
        public virtual string get_FileNames(short index) {
            // This method should really only be called with 1 as the parameter, but
            // there used to be a bug in VB/C# that would work with 0. To avoid breaking
            // existing automation they are still accepting 0. To be compatible with them
            // we accept it as well.
            if (index < 0) {
                throw new ArgumentOutOfRangeException("index");
            }
            return this.node.Url;
        }

        /// <summary>
        /// Expands the view of Solution Explorer to show project items. 
        /// </summary>
        public virtual void ExpandView() {
            CheckProjectIsValid();

            using (AutomationScope scope = new AutomationScope(this.Node.ProjectMgr.Site)) {
                Node.ProjectMgr.Site.GetUIThread().Invoke(() => node.ExpandItem(EXPANDFLAGS.EXPF_ExpandFolder));
            }
        }

        /// <summary>
        /// Opens the project item in the specified view. Not implemented because this abstract class dont know what to open
        /// </summary>
        /// <param name="ViewKind">Specifies the view kind in which to open the item</param>
        /// <returns>Window object</returns>
        public virtual EnvDTE.Window Open(string ViewKind) {
            throw new NotImplementedException();
        }

        // We're managed and we don't use COM’s IDispatch which would resolve parametrized property FileNames and IsOpen correctly. 
        // Powershell scripts are using reflection to find (or rather, not find) the methodss. Thus FileNames call ends with exception: method's not defined.
        // Implementing these as regular methods satisfies the situation.
        // This is required for Nuget support.

        public string FileNames(short index) {
            return get_FileNames(index);
        }

        public bool IsOpen(string viewKind) {
            return get_IsOpen(viewKind);
        }

        #endregion

    }
}
