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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.Project.Automation {
    /// <summary>
    /// Contains ProjectItem objects
    /// </summary>
    [ComVisible(true)]
    public class OAProjectItems : OANavigableProjectItems {
        #region ctor
        internal OAProjectItems(OAProject project, HierarchyNode nodeWithItems)
            : base(project, nodeWithItems) {
        }
        #endregion

        #region EnvDTE.ProjectItems

        /// <summary>
        /// Creates a new project item from an existing directory and all files and subdirectories
        /// contained within it.
        /// </summary>
        /// <param name="directory">The full path of the directory to add.</param>
        /// <returns>A ProjectItem object.</returns>
        public override ProjectItem AddFromDirectory(string directory) {
            CheckProjectIsValid();

            return Project.ProjectNode.Site.GetUIThread().Invoke<EnvDTE.ProjectItem>(() => {
                ProjectItem result = AddFolder(directory, null);

                foreach (string subdirectory in Directory.EnumerateDirectories(directory)) {
                    result.ProjectItems.AddFromDirectory(Path.Combine(directory, subdirectory));
                }

                foreach (var extension in this.Project.ProjectNode.CodeFileExtensions) {
                    foreach (string filename in Directory.EnumerateFiles(directory, "*" + extension)) {
                        result.ProjectItems.AddFromFile(Path.Combine(directory, filename));
                    }
                }
                return result;
            });
        }

        /// <summary>
        /// Creates a new project item from an existing item template file and adds it to the project. 
        /// </summary>
        /// <param name="fileName">The full path and file name of the template project file.</param>
        /// <param name="name">The file name to use for the new project item.</param>
        /// <returns>A ProjectItem object. </returns>
        public override EnvDTE.ProjectItem AddFromTemplate(string fileName, string name) {
            CheckProjectIsValid();

            ProjectNode proj = this.Project.ProjectNode;
            EnvDTE.ProjectItem itemAdded = null;

            using (AutomationScope scope = new AutomationScope(this.Project.ProjectNode.Site)) {
                // Determine the operation based on the extension of the filename.
                // We should run the wizard only if the extension is vstemplate
                // otherwise it's a clone operation
                VSADDITEMOPERATION op;
                Project.ProjectNode.Site.GetUIThread().Invoke(() => {
                    if (Utilities.IsTemplateFile(fileName)) {
                        op = VSADDITEMOPERATION.VSADDITEMOP_RUNWIZARD;
                    } else {
                        op = VSADDITEMOPERATION.VSADDITEMOP_CLONEFILE;
                    }

                    VSADDRESULT[] result = new VSADDRESULT[1];

                    // It is not a very good idea to throw since the AddItem might return Cancel or Abort.
                    // The problem is that up in the call stack the wizard code does not check whether it has received a ProjectItem or not and will crash.
                    // The other problem is that we cannot get add wizard dialog back if a cancel or abort was returned because we throw and that code will never be executed. Typical catch 22.
                    ErrorHandler.ThrowOnFailure(proj.AddItem(this.NodeWithItems.ID, op, name, 0, new string[1] { fileName }, IntPtr.Zero, result));

                    string fileDirectory = proj.GetBaseDirectoryForAddingFiles(this.NodeWithItems);
                    string templateFilePath = System.IO.Path.Combine(fileDirectory, name);
                    itemAdded = this.EvaluateAddResult(result[0], templateFilePath);
                });
            }

            return itemAdded;
        }

        private void CheckProjectIsValid() {
            if (this.Project == null || this.Project.ProjectNode == null || this.Project.ProjectNode.Site == null || this.Project.ProjectNode.IsClosed) {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Adds a folder to the collection of ProjectItems with the given name.
        /// 
        /// The kind must be null, empty string, or the string value of vsProjectItemKindPhysicalFolder.
        /// Virtual folders are not supported by this implementation.
        /// </summary>
        /// <param name="name">The name of the new folder to add</param>
        /// <param name="kind">A string representing a Guid of the folder kind.</param>
        /// <returns>A ProjectItem representing the newly added folder.</returns>
        public override ProjectItem AddFolder(string name, string kind) {
            Project.CheckProjectIsValid();

            //Verify name is not null or empty
            Utilities.ValidateFileName(this.Project.ProjectNode.Site, name);

            //Verify that kind is null, empty, or a physical folder
            if (!(string.IsNullOrEmpty(kind) || kind.Equals(EnvDTE.Constants.vsProjectItemKindPhysicalFolder))) {
                throw new ArgumentException("Parameter specification for AddFolder was not meet", "kind");
            }

            return Project.ProjectNode.Site.GetUIThread().Invoke<EnvDTE.ProjectItem>(() => {
                var existingChild = this.NodeWithItems.FindImmediateChildByName(name);
                if (existingChild != null) {
                    if (existingChild.IsNonMemberItem && ErrorHandler.Succeeded(existingChild.IncludeInProject(false))) {
                        return existingChild.GetAutomationObject() as ProjectItem;
                    }
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Folder already exists with the name '{0}'", name));
                }

                ProjectNode proj = this.Project.ProjectNode;

                HierarchyNode newFolder = null;
                using (AutomationScope scope = new AutomationScope(this.Project.ProjectNode.Site)) {

                    //In the case that we are adding a folder to a folder, we need to build up
                    //the path to the project node.
                    name = Path.Combine(NodeWithItems.FullPathToChildren, name);

                    newFolder = proj.CreateFolderNodes(name);
                }

                return newFolder.GetAutomationObject() as ProjectItem;
            });
        }

        /// <summary>
        /// Copies a source file and adds it to the project.
        /// </summary>
        /// <param name="filePath">The path and file name of the project item to be added.</param>
        /// <returns>A ProjectItem object. </returns>
        public override EnvDTE.ProjectItem AddFromFileCopy(string filePath) {
            return this.AddItem(filePath, VSADDITEMOPERATION.VSADDITEMOP_CLONEFILE);
        }

        /// <summary>
        /// Adds a project item from a file that is installed in a project directory structure. 
        /// </summary>
        /// <param name="fileName">The file name of the item to add as a project item. </param>
        /// <returns>A ProjectItem object. </returns>
        public override EnvDTE.ProjectItem AddFromFile(string fileName) {
            return this.AddItem(fileName, VSADDITEMOPERATION.VSADDITEMOP_LINKTOFILE);
        }

        #endregion

        #region helper methods
        /// <summary>
        /// Adds an item to the project.
        /// </summary>
        /// <param name="path">The full path of the item to add.</param>
        /// <param name="op">The <paramref name="VSADDITEMOPERATION"/> to use when adding the item.</param>
        /// <returns>A ProjectItem object. </returns>
        protected virtual EnvDTE.ProjectItem AddItem(string path, VSADDITEMOPERATION op) {
            CheckProjectIsValid();
            return Project.ProjectNode.Site.GetUIThread().Invoke<EnvDTE.ProjectItem>(() => {
                ProjectNode proj = this.Project.ProjectNode;
                EnvDTE.ProjectItem itemAdded = null;
                using (AutomationScope scope = new AutomationScope(this.Project.ProjectNode.Site)) {
                    VSADDRESULT[] result = new VSADDRESULT[1];
                    ErrorHandler.ThrowOnFailure(proj.AddItem(this.NodeWithItems.ID, op, path, 0, new string[1] { path }, IntPtr.Zero, result));

                    string realPath = null;
                    if (op != VSADDITEMOPERATION.VSADDITEMOP_LINKTOFILE) {
                        string fileName = Path.GetFileName(path);
                        string fileDirectory = proj.GetBaseDirectoryForAddingFiles(this.NodeWithItems);
                        realPath = Path.Combine(fileDirectory, fileName);
                    } else {
                        realPath = path;
                    }

                    itemAdded = this.EvaluateAddResult(result[0], realPath);
                }

                return itemAdded;
            });
        }

        /// <summary>
        /// Evaluates the result of an add operation.
        /// </summary>
        /// <param name="result">The <paramref name="VSADDRESULT"/> returned by the Add methods</param>
        /// <param name="path">The full path of the item added.</param>
        /// <returns>A ProjectItem object.</returns>
        private EnvDTE.ProjectItem EvaluateAddResult(VSADDRESULT result, string path) {
            return Project.ProjectNode.Site.GetUIThread().Invoke<EnvDTE.ProjectItem>(() => {
                if (result != VSADDRESULT.ADDRESULT_Failure) {
                    if (Directory.Exists(path)) {
                        path = CommonUtils.EnsureEndSeparator(path);
                    }
                    HierarchyNode nodeAdded = this.NodeWithItems.ProjectMgr.FindNodeByFullPath(path);
                    Debug.Assert(nodeAdded != null, "We should have been able to find the new element in the hierarchy");
                    if (nodeAdded != null) {
                        EnvDTE.ProjectItem item = null;
                        var fileNode = nodeAdded as FileNode;
                        if (fileNode != null) {
                            item = new OAFileItem(this.Project, fileNode);
                        } else {
                            item = new OAProjectItem(this.Project, nodeAdded);
                        }

                        return item;
                    }
                }
                return null;
            });
        }
        #endregion

    }
}
