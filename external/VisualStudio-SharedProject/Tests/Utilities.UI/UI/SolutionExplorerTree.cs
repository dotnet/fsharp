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
using System.IO;
using System.Linq;
using System.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestUtilities.UI {
    public class SolutionExplorerTree : TreeView {
        public SolutionExplorerTree(AutomationElement element)
            : base(element) {
        }

        public void AssertFileExists(string projectLocation, params string[] path) {
            AssertItemExistsInTree(path);

            var basePath = projectLocation;
            for (int i = 1; i < path.Length; i++) {
                basePath = Path.Combine(basePath, path[i]);
            }
            Assert.IsTrue(File.Exists(basePath), "File doesn't exist: " + basePath);
        }

        public void AssertFileExistsWithContent(string projectLocation, string content, params string[] path) {
            AssertItemExistsInTree(path);

            var basePath = projectLocation;
            for (int i = 1; i < path.Length; i++) {
                basePath = Path.Combine(basePath, path[i]);
            }
            Assert.IsTrue(File.Exists(basePath), "File doesn't exist: " + basePath);
            Assert.AreEqual(File.ReadAllText(basePath), content);
        }

        public void AssertFileDoesntExist(string projectLocation, params string[] path) {
            Assert.IsNull(FindItem(path), "Item exists in solution explorer: " + String.Join("\\", path));

            var basePath = projectLocation;
            for (int i = 1; i < path.Length; i++) {
                basePath = Path.Combine(basePath, path[i]);
            }
            Assert.IsFalse(File.Exists(basePath), "File exists: " + basePath);
        }

        public void AssertFolderExists(string projectLocation, params string[] path) {
            AssertItemExistsInTree(path);

            var basePath = projectLocation;
            for (int i = 1; i < path.Length; i++) {
                basePath = Path.Combine(basePath, path[i]);
            }
            Assert.IsTrue(Directory.Exists(basePath), "File doesn't exist: " + basePath);
        }

        public void AssertFolderDoesntExist(string projectLocation, params string[] path) {
            Assert.IsNull(WaitForItemRemoved(path), "Item exists in solution explorer: " + String.Join("\\", path));

            var basePath = projectLocation;
            for (int i = 1; i < path.Length; i++) {
                basePath = Path.Combine(basePath, path[i]);
            }
            Assert.IsFalse(Directory.Exists(basePath), "File exists: " + basePath);
        }

        private void AssertItemExistsInTree(string[] path) {
            var item = WaitForItem(path);
            if (item == null) {
                string msg = "Item not found in solution explorer " + String.Join("\\", path);
                for (int i = 1; i < path.Length; i++) {
                    item = FindItem(path.Take(i).ToArray());
                    if (item == null) {
                        msg += Environment.NewLine + "Item missing at: " + String.Join("\\", path.Take(i));
                        break;
                    }
                }
                Assert.IsNotNull(item, msg);
            }
        }

        public void SelectProject(EnvDTE.Project project) {
            var slnName = string.Format("Solution '{0}' ({1} project{2})",
                Path.GetFileNameWithoutExtension(project.DTE.Solution.FullName),
                project.DTE.Solution.Projects.Count,
                project.DTE.Solution.Projects.Count == 1 ? "" : "s"
            );
            var item = WaitForItem(slnName, project.Name).AsWrapper();
            Assert.IsNotNull(item);
            item.Select();
        }

        public TreeNode WaitForChildOfProject(EnvDTE.Project project, params string[] path) {
            var item = WaitForItemHelper(p => FindChildOfProjectHelper(project, p, false), path);
            // Check one more time, but now let the assertions be raised.
            return new TreeNode(FindChildOfProjectHelper(project, path, true));
        }

        public AutomationElement WaitForChildOfProjectRemoved(EnvDTE.Project project, params string[] path) {
            return WaitForItemRemovedHelper(p => FindChildOfProjectHelper(project, p, false), path);
        }

        public TreeNode FindChildOfProject(EnvDTE.Project project, params string[] path) {
            return new TreeNode(FindChildOfProjectHelper(project, path, true));
        }

        public TreeNode TryFindChildOfProject(EnvDTE.Project project, params string[] path) {
            return new TreeNode(FindChildOfProjectHelper(project, path, false));
        }

        private AutomationElement FindChildOfProjectHelper(EnvDTE.Project project, string[] path, bool assertOnFailure) {
            var sln = project.DTE.Solution;
            var slnLabel = string.Format(
                "Solution '{0}' ({1} project{2})",
                Path.GetFileNameWithoutExtension(sln.FullName),
                sln.Count,
                sln.Count == 1 ? "" : "s"
            );

            var slnElements = Element.FindAll(TreeScope.Children, new PropertyCondition(
                AutomationElement.NameProperty, slnLabel
            ));
            int slnCount = slnElements.OfType<AutomationElement>().Count();
            if (assertOnFailure) {
                Assert.AreEqual(1, slnCount, string.Format("Did not find single node <{0}>", slnLabel));
            } else if (slnCount != 1) {
                return null;
            }
            var slnElement = slnElements.Cast<AutomationElement>().Single();

            var projLabel = project.Name;
            var projElements = slnElement.FindAll(TreeScope.Children, new PropertyCondition(
                AutomationElement.NameProperty, projLabel
            ));
            int projCount = projElements.OfType<AutomationElement>().Count();
            if (assertOnFailure) {
                Assert.AreEqual(1, projCount, string.Format("Did not find single node <{0}>", projLabel));
            } else if (projCount != 1) {
                return null;
            }
            var projElement = projElements.Cast<AutomationElement>().Single();

            var itemElement = path.Any() ? FindNode(
                projElement.FindAll(TreeScope.Children, Condition.TrueCondition),
                path,
                0
            ) : projElement;

            if (assertOnFailure) {
                AutomationWrapper.DumpElement(Element);
                Assert.IsNotNull(itemElement, string.Format("Did not find element <{0}\\{1}\\{2}>", slnLabel, projLabel, string.Join("\\", path)));
            }
            return itemElement;
        }

    }
}
