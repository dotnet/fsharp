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
using System.Runtime.InteropServices;
using Microsoft.Build.Evaluation;
using Microsoft.VisualStudio;
using MSBuild = Microsoft.Build.Evaluation;

namespace Microsoft.VisualStudioTools.Project {
    internal class MsBuildProjectElement : ProjectElement {
        private MSBuild.ProjectItem _item;
        private string _url; // cached Url

        /// <summary>
        /// Constructor to create a new MSBuild.ProjectItem and add it to the project
        /// Only have internal constructors as the only one who should be creating
        /// such object is the project itself (see Project.CreateFileNode()).
        /// </summary>
        internal MsBuildProjectElement(ProjectNode project, string itemPath, string itemType)
            : base(project) {
            Utilities.ArgumentNotNullOrEmpty("itemPath", itemPath);
            Utilities.ArgumentNotNullOrEmpty("itemType", itemType);

            // create and add the item to the project

            _item = project.BuildProject.AddItem(itemType, Microsoft.Build.Evaluation.ProjectCollection.Escape(itemPath))[0];
            _url = base.Url;
        }

        /// <summary>
        /// Constructor to Wrap an existing MSBuild.ProjectItem
        /// Only have internal constructors as the only one who should be creating
        /// such object is the project itself (see Project.CreateFileNode()).
        /// </summary>
        /// <param name="project">Project that owns this item</param>
        /// <param name="existingItem">an MSBuild.ProjectItem; can be null if virtualFolder is true</param>
        /// <param name="virtualFolder">Is this item virtual (such as reference folder)</param>
        internal MsBuildProjectElement(ProjectNode project, MSBuild.ProjectItem existingItem)
            : base(project) {
            Utilities.ArgumentNotNull("existingItem", existingItem);

            // Keep a reference to project and item
            _item = existingItem;
            _url = base.Url;
        }

        protected override string ItemType {
            get {
                return _item.ItemType;
            }
            set {
                _item.ItemType = value;
                OnItemTypeChanged();
            }
        }

        /// <summary>
        /// Set an attribute on the project element
        /// </summary>
        /// <param name="attributeName">Name of the attribute to set</param>
        /// <param name="attributeValue">Value to give to the attribute</param>
        public override void SetMetadata(string attributeName, string attributeValue) {
            Debug.Assert(String.Compare(attributeName, ProjectFileConstants.Include, StringComparison.OrdinalIgnoreCase) != 0, "Use rename as this won't work");

            // Build Action is the type, not a property, so intercept
            if (String.Compare(attributeName, ProjectFileConstants.BuildAction, StringComparison.OrdinalIgnoreCase) == 0) {
                _item.ItemType = attributeValue;
                return;
            }

            // Check out the project file.
            if (!ItemProject.QueryEditProjectFile(false)) {
                throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
            }

            if (attributeValue == null) {
                _item.RemoveMetadata(attributeName);
            } else {
                _item.SetMetadataValue(attributeName, attributeValue);
            }
        }

        /// <summary>
        /// Get the value of an attribute on a project element
        /// </summary>
        /// <param name="attributeName">Name of the attribute to get the value for</param>
        /// <returns>Value of the attribute</returns>
        public override string GetMetadata(string attributeName) {
            // cannot ask MSBuild for Include, so intercept it and return the corresponding property
            if (String.Compare(attributeName, ProjectFileConstants.Include, StringComparison.OrdinalIgnoreCase) == 0) {
                return _item.EvaluatedInclude;
            }

            // Build Action is the type, not a property, so intercept this one as well
            if (String.Compare(attributeName, ProjectFileConstants.BuildAction, StringComparison.OrdinalIgnoreCase) == 0) {
                return _item.ItemType;
            }

            return _item.GetMetadataValue(attributeName);
        }

        public override void Rename(string newPath) {
            string escapedPath = Microsoft.Build.Evaluation.ProjectCollection.Escape(newPath);

            _item.Rename(escapedPath);
            this.RefreshProperties();
        }

        public override void RefreshProperties() {
            ItemProject.BuildProject.ReevaluateIfNecessary();

            _url = base.Url;

            IEnumerable<ProjectItem> items = ItemProject.BuildProject.GetItems(_item.ItemType);
            foreach (ProjectItem projectItem in items) {
                if (projectItem != null && projectItem.UnevaluatedInclude.Equals(_item.UnevaluatedInclude)) {
                    _item = projectItem;
                    return;
                }
            }
        }

        /// <summary>
        /// Calling this method remove this item from the project file.
        /// Once the item is delete, you should not longer be using it.
        /// Note that the item should be removed from the hierarchy prior to this call.
        /// </summary>
        public override void RemoveFromProjectFile() {
            if (!Deleted) {
                ItemProject.BuildProject.RemoveItem(_item);
            }

            base.RemoveFromProjectFile();
        }

        internal MSBuild.ProjectItem Item {
            get {
                return _item;
            }
        }

        public override string Url {
            get {
                return _url;
            }
        }

        public override bool Equals(object obj) {
            // Do they reference the same element?
            if (Object.ReferenceEquals(this, obj)) {
                return true;
            }

            MsBuildProjectElement msBuildProjElem = obj as MsBuildProjectElement;
            if (Object.ReferenceEquals(msBuildProjElem, null)) {
                return false;
            }

            // Do they reference the same project?
            if (!ItemProject.Equals(msBuildProjElem.ItemProject))
                return false;

            // Do they have the same include?
            string include1 = GetMetadata(ProjectFileConstants.Include);
            string include2 = msBuildProjElem.GetMetadata(ProjectFileConstants.Include);

            // Unfortunately the checking for nulls have to be done again, since neither String.Equals nor String.Compare can handle nulls.
            // Virtual folders should not be handled here.
            if (include1 == null || include2 == null) {
                return false;
            }

            return String.Equals(include1, include2, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode() {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(GetMetadata(ProjectFileConstants.Include));
        }
    }
}
