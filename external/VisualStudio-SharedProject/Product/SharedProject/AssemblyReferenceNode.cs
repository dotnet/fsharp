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
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.Project {
    internal class AssemblyReferenceNode : ReferenceNode {
        #region fieds
        /// <summary>
        /// The name of the assembly this refernce represents
        /// </summary>
        private System.Reflection.AssemblyName assemblyName;
        private AssemblyName resolvedAssemblyName;

        private string assemblyPath = String.Empty;

        /// <summary>
        /// Defines the listener that would listen on file changes on the nested project node.
        /// </summary>
        private FileChangeManager fileChangeListener;

        /// <summary>
        /// A flag for specifying if the object was disposed.
        /// </summary>
        private bool isDisposed;
        #endregion

        #region properties
        /// <summary>
        /// The name of the assembly this reference represents.
        /// </summary>
        /// <value></value>
        internal System.Reflection.AssemblyName AssemblyName {
            get {
                return this.assemblyName;
            }
        }

        /// <summary>
        /// Returns the name of the assembly this reference refers to on this specific
        /// machine. It can be different from the AssemblyName property because it can
        /// be more specific.
        /// </summary>
        internal System.Reflection.AssemblyName ResolvedAssembly {
            get { return resolvedAssemblyName; }
        }

        public override string Url {
            get {
                return this.assemblyPath;
            }
        }

        public override string Caption {
            get {
                return this.assemblyName.Name;
            }
        }

        private Automation.OAAssemblyReference assemblyRef;
        internal override object Object {
            get {
                if (null == assemblyRef) {
                    assemblyRef = new Automation.OAAssemblyReference(this);
                }
                return assemblyRef;
            }
        }
        #endregion

        #region ctors
        /// <summary>
        /// Constructor for the ReferenceNode
        /// </summary>
        public AssemblyReferenceNode(ProjectNode root, ProjectElement element)
            : base(root, element) {
            this.GetPathNameFromProjectFile();

            this.InitializeFileChangeEvents();

            if (File.Exists(assemblyPath)) {
                this.fileChangeListener.ObserveItem(this.assemblyPath);
            }

            string include = this.ItemNode.GetMetadata(ProjectFileConstants.Include);

            this.CreateFromAssemblyName(new System.Reflection.AssemblyName(include));
        }

        /// <summary>
        /// Constructor for the AssemblyReferenceNode
        /// </summary>
        public AssemblyReferenceNode(ProjectNode root, string assemblyPath)
            : base(root) {
            // Validate the input parameters.
            if (null == root) {
                throw new ArgumentNullException("root");
            }
            if (string.IsNullOrEmpty(assemblyPath)) {
                throw new ArgumentNullException("assemblyPath");
            }

            this.InitializeFileChangeEvents();

            // The assemblyPath variable can be an actual path on disk or a generic assembly name.
            if (File.Exists(assemblyPath)) {
                // The assemblyPath parameter is an actual file on disk; try to load it.
                this.assemblyName = System.Reflection.AssemblyName.GetAssemblyName(assemblyPath);
                this.assemblyPath = assemblyPath;

                // We register with listeningto chnages onteh path here. The rest of teh cases will call into resolving the assembly and registration is done there.
                this.fileChangeListener.ObserveItem(this.assemblyPath);
            } else {
                // The file does not exist on disk. This can be because the file / path is not
                // correct or because this is not a path, but an assembly name.
                // Try to resolve the reference as an assembly name.
                this.CreateFromAssemblyName(new System.Reflection.AssemblyName(assemblyPath));
            }
        }
        #endregion

        #region methods

        /// <summary>
        /// Links a reference node to the project and hierarchy.
        /// </summary>
        protected override void BindReferenceData() {
            ProjectMgr.Site.GetUIThread().MustBeCalledFromUIThread();

            Debug.Assert(this.assemblyName != null, "The AssemblyName field has not been initialized");

            // If the item has not been set correctly like in case of a new reference added it now.
            // The constructor for the AssemblyReference node will create a default project item. In that case the Item is null.
            // We need to specify here the correct project element. 
            if (this.ItemNode == null || this.ItemNode is VirtualProjectElement) {
                this.ItemNode = new MsBuildProjectElement(this.ProjectMgr, this.assemblyName.FullName, ProjectFileConstants.Reference);
            }

            // Set the basic information we know about
            this.ItemNode.SetMetadata(ProjectFileConstants.Name, this.assemblyName.Name);
            this.ItemNode.SetMetadata(ProjectFileConstants.AssemblyName, Path.GetFileName(this.assemblyPath));

            this.SetReferenceProperties();
        }

        /// <summary>
        /// Disposes the node
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing) {
            if (this.isDisposed) {
                return;
            }

            try {
                this.UnregisterFromFileChangeService();
            } finally {
                base.Dispose(disposing);
                this.isDisposed = true;
            }
        }

        private void CreateFromAssemblyName(AssemblyName name) {
            this.assemblyName = name;

            // Use MsBuild to resolve the assemblyname 
            this.ResolveAssemblyReference();

            if (String.IsNullOrEmpty(this.assemblyPath) && (this.ItemNode is MsBuildProjectElement)) {
                // Try to get the assmbly name from the hintpath.
                this.GetPathNameFromProjectFile();
                if (this.assemblyPath == null) {
                    // Try to get the assembly name from the path
                    this.assemblyName = System.Reflection.AssemblyName.GetAssemblyName(this.assemblyPath);
                }
            }
            if (null == resolvedAssemblyName) {
                resolvedAssemblyName = assemblyName;
            }
        }

        /// <summary>
        /// Checks if an assembly is already added. The method parses all references and compares the full assemblynames, or the location of the assemblies to decide whether two assemblies are the same.
        /// </summary>
        /// <returns>true if the assembly has already been added.</returns>
        protected override bool IsAlreadyAdded() {
            ReferenceContainerNode referencesFolder = this.ProjectMgr.GetReferenceContainer() as ReferenceContainerNode;
            Debug.Assert(referencesFolder != null, "Could not find the References node");
            if (referencesFolder == null) {
                // Return true so that our caller does not try and add us.
                return true;
            }

            bool shouldCheckPath = !string.IsNullOrEmpty(this.Url);

            for (HierarchyNode n = referencesFolder.FirstChild; n != null; n = n.NextSibling) {
                AssemblyReferenceNode assemblyRefererenceNode = n as AssemblyReferenceNode;
                if (null != assemblyRefererenceNode) {
                    // We will check if the full assemblynames are the same or if the Url of the assemblies is the same.
                    if (String.Compare(assemblyRefererenceNode.AssemblyName.FullName, this.assemblyName.FullName, StringComparison.OrdinalIgnoreCase) == 0 ||
                        (shouldCheckPath && CommonUtils.IsSamePath(assemblyRefererenceNode.Url, this.Url))) {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if this is node a valid node for painting the default reference icon.
        /// </summary>
        /// <returns></returns>
        protected override bool CanShowDefaultIcon() {
            return File.Exists(assemblyPath);
        }

        private void GetPathNameFromProjectFile() {
            string result = this.ItemNode.GetMetadata(ProjectFileConstants.HintPath);
            if (String.IsNullOrEmpty(result)) {
                result = this.ItemNode.GetMetadata(ProjectFileConstants.AssemblyName);
                if (String.IsNullOrEmpty(result)) {
                    this.assemblyPath = String.Empty;
                } else if (!result.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) {
                    result += ".dll";
                    this.assemblyPath = result;
                }
            } else {
                this.assemblyPath = CommonUtils.GetAbsoluteFilePath(this.ProjectMgr.ProjectHome, result);
            }
        }

        protected override void ResolveReference() {
            this.ResolveAssemblyReference();
        }

        private void SetHintPathAndPrivateValue() {

            // Private means local copy; we want to know if it is already set to not override the default
            string privateValue = this.ItemNode.GetMetadata(ProjectFileConstants.Private);

            // Get the list of items which require HintPath
            var references = this.ProjectMgr.CurrentConfig.GetItems(MsBuildGeneratedItemType.ReferenceCopyLocalPaths);


            // Now loop through the generated References to find the corresponding one
            foreach (var reference in references) {
                string fileName = Path.GetFileNameWithoutExtension(reference.EvaluatedInclude);
                if (String.Compare(fileName, this.assemblyName.Name, StringComparison.OrdinalIgnoreCase) == 0) {
                    // We found it, now set some properties based on this.

                    // Remove the HintPath, we will re-add it below if it is needed
                    if (!String.IsNullOrEmpty(this.assemblyPath)) {
                        this.ItemNode.SetMetadata(ProjectFileConstants.HintPath, null);
                    }

                    string hintPath = reference.GetMetadataValue(ProjectFileConstants.HintPath);
                    if (!String.IsNullOrEmpty(hintPath)) {
                        hintPath = CommonUtils.GetRelativeFilePath(this.ProjectMgr.ProjectHome, hintPath);

                        this.ItemNode.SetMetadata(ProjectFileConstants.HintPath, hintPath);
                        // If this is not already set, we default to true
                        if (String.IsNullOrEmpty(privateValue)) {
                            this.ItemNode.SetMetadata(ProjectFileConstants.Private, true.ToString());
                        }
                    }
                    break;
                }

            }

        }

        /// <summary>
        /// This function ensures that some properies of the reference are set.
        /// </summary>
        private void SetReferenceProperties() {
            ProjectMgr.Site.GetUIThread().MustBeCalledFromUIThread();

            // Set a default HintPath for msbuild to be able to resolve the reference.
            this.ItemNode.SetMetadata(ProjectFileConstants.HintPath, this.assemblyPath);

            // Resolve assembly referernces. This is needed to make sure that properties like the full path
            // to the assembly or the hint path are set.
            if (!ProjectMgr.BuildProject.Targets.ContainsKey(MsBuildTarget.ResolveAssemblyReferences)) {
                return;
            }

            if (this.ProjectMgr.Build(MsBuildTarget.ResolveAssemblyReferences) != MSBuildResult.Successful) {
                return;
            }

            // Check if we have to resolve again the path to the assembly.
            if (string.IsNullOrEmpty(this.assemblyPath)) {
                ResolveReference();
            }

            // Make sure that the hint path if set (if needed).
            SetHintPathAndPrivateValue();
        }

        /// <summary>
        /// Does the actual job of resolving an assembly reference. We need a private method that does not violate 
        /// calling virtual method from the constructor.
        /// </summary>
        private void ResolveAssemblyReference() {
            if (this.ProjectMgr == null || this.ProjectMgr.IsClosed) {
                return;
            }

            var group = this.ProjectMgr.CurrentConfig.GetItems(ProjectFileConstants.ReferencePath);
            foreach (var item in group) {
                string fullPath = CommonUtils.GetAbsoluteFilePath(this.ProjectMgr.ProjectHome, item.EvaluatedInclude);

                System.Reflection.AssemblyName name = System.Reflection.AssemblyName.GetAssemblyName(fullPath);

                // Try with full assembly name and then with weak assembly name.
                if (String.Equals(name.FullName, this.assemblyName.FullName, StringComparison.OrdinalIgnoreCase) || String.Equals(name.Name, this.assemblyName.Name, StringComparison.OrdinalIgnoreCase)) {
                    if (!CommonUtils.IsSamePath(fullPath, this.assemblyPath)) {
                        // set the full path now.
                        this.assemblyPath = fullPath;

                        // We have a new item to listen too, since the assembly reference is resolved from a different place.
                        this.fileChangeListener.ObserveItem(this.assemblyPath);
                    }

                    this.resolvedAssemblyName = name;

                    // No hint path is needed since the assembly path will always be resolved.
                    return;
                }
            }
        }

        /// <summary>
        /// Registers with File change events
        /// </summary>
        private void InitializeFileChangeEvents() {
            this.fileChangeListener = new FileChangeManager(this.ProjectMgr.Site);
            this.fileChangeListener.FileChangedOnDisk += this.OnAssemblyReferenceChangedOnDisk;
        }

        /// <summary>
        /// Unregisters this node from file change notifications.
        /// </summary>
        private void UnregisterFromFileChangeService() {
            this.fileChangeListener.FileChangedOnDisk -= this.OnAssemblyReferenceChangedOnDisk;
            this.fileChangeListener.Dispose();
        }

        /// <summary>
        /// Event callback. Called when one of the assembly file is changed.
        /// </summary>
        /// <param name="sender">The FileChangeManager object.</param>
        /// <param name="e">Event args containing the file name that was updated.</param>
        protected virtual void OnAssemblyReferenceChangedOnDisk(object sender, FileChangedOnDiskEventArgs e) {
            Debug.Assert(e != null, "No event args specified for the FileChangedOnDisk event");
            if (e == null) {
                return;
            }

            // We only care about file deletes, so check for one before enumerating references.
            if ((e.FileChangeFlag & _VSFILECHANGEFLAGS.VSFILECHG_Del) == 0) {
                return;
            }


            if (CommonUtils.IsSamePath(e.FileName, this.assemblyPath)) {
                ProjectMgr.OnInvalidateItems(this.Parent);
            }
        }

        /// <summary>
        /// Overridden method. The method updates the build dependency list before removing the node from the hierarchy.
        /// </summary>
        public override void Remove(bool removeFromStorage) {
            if (this.ProjectMgr == null) {
                return;
            }
            base.RemoveNonDocument(removeFromStorage);
            this.ItemNode.RemoveFromProjectFile();

            // Notify hierarchy event listeners that items have been invalidated
            ProjectMgr.OnInvalidateItems(this);

            // Dispose the node now that is deleted.
            Dispose(true);
        }


        #endregion
    }
}
