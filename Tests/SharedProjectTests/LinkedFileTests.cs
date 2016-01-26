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
using System.Text.RegularExpressions;
using System.Threading;
using EnvDTE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtilities;
using TestUtilities.SharedProject;
using TestUtilities.UI;
using MSBuild = Microsoft.Build.Evaluation;
using MessageBoxButton = TestUtilities.MessageBoxButton;
using Microsoft.VisualStudioTools.VSTestHost;

namespace VisualStudioToolsUITests {
    [TestClass]
    public class LinkedFileTests : SharedProjectTest {
        private static ProjectDefinition LinkedFiles(ProjectType projectType) {
            return new ProjectDefinition(
                "LinkedFiles",
                projectType,
                ItemGroup(
                    Folder("MoveToFolder"),
                    Folder("FolderWithAFile"),
                    Folder("Fob"),
                    Folder("..\\LinkedFilesDir", isExcluded: true),
                    Folder("AlreadyLinkedFolder"),

                    Compile("Program"),
                    Compile("..\\ImplicitLinkedFile"),
                    Compile("..\\ExplicitLinkedFile")
                        .Link("ExplicitDir\\ExplicitLinkedFile"),
                    Compile("..\\ExplicitLinkedFileWrongFilename")
                        .Link("ExplicitDir\\Blah"),
                    Compile("..\\MovedLinkedFile"),
                    Compile("..\\MovedLinkedFileOpen"),
                    Compile("..\\MovedLinkedFileOpenEdit"),
                    Compile("..\\FileNotInProject"),
                    Compile("..\\DeletedLinkedFile"),
                    Compile("LinkedInModule")
                        .Link("Fob\\LinkedInModule"),
                    Compile("SaveAsCreateLink"),
                    Compile("..\\SaveAsCreateFile"),
                    Compile("..\\SaveAsCreateFileNewDirectory"),
                    Compile("FolderWithAFile\\ExistsOnDiskAndInProject"),
                    Compile("FolderWithAFile\\ExistsInProjectButNotOnDisk", isMissing: true),
                    Compile("FolderWithAFile\\ExistsOnDiskButNotInProject"),
                    Compile("..\\LinkedFilesDir\\SomeLinkedFile")
                        .Link("Oar\\SomeLinkedFile"),
                    Compile("..\\RenamedLinkFile")
                        .Link("Fob\\NewNameForLinkFile"),
                    Compile("..\\BadLinkPath")
                        .Link("..\\BadLinkPathFolder\\BadLinkPath"),
                    Compile("..\\RootedLinkIgnored")
                        .Link("C:\\RootedLinkIgnored"),
                    Compile("C:\\RootedIncludeIgnored", isMissing: true)
                        .Link("RootedIncludeIgnored"),
                    Compile("Fob\\AddExistingInProjectDirButNotInProject"),
                    Compile("..\\ExistingItem", isExcluded: true),
                    Compile("..\\ExistsInProjectButNotOnDisk", isExcluded: true),
                    Compile("..\\ExistsOnDiskAndInProject", isExcluded: true),
                    Compile("..\\ExistsOnDiskButNotInProject", isExcluded: true)
                )
            );
        }

        private static SolutionFile MultiProjectLinkedFiles(ProjectType projectType) {
            return SolutionFile.Generate(
                "MultiProjectLinkedFiles",
                new ProjectDefinition(
                    "LinkedFiles1",
                    projectType,
                    ItemGroup(
                        Compile("..\\FileNotInProject1"),
                        Compile("..\\FileNotInProject2")
                    )
                ),
                new ProjectDefinition(
                    "LinkedFiles2",
                    projectType,
                    ItemGroup(
                        Compile("..\\FileNotInProject2", isMissing: true)
                    )
                )
            );
        }


        [ClassInitialize]
        public static void DoDeployment(TestContext context) {
            AssertListener.Initialize();
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void RenameLinkedNode() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {
                    // implicitly linked node
                    var projectNode = solution.FindItem("LinkedFiles", "ImplicitLinkedFile" + projectType.CodeExtension);
                    Assert.IsNotNull(projectNode, "projectNode");
                    AutomationWrapper.Select(projectNode);

                    try {
                        solution.ExecuteCommand("File.Rename");
                        Assert.Fail("Should have failed to rename");
                    } catch (Exception e) {
                        Debug.WriteLine(e.ToString());
                    }


                    // explicitly linked node
                    var explicitLinkedFile = solution.FindItem("LinkedFiles", "ExplicitDir", "ExplicitLinkedFile" + projectType.CodeExtension);
                    Assert.IsNotNull(explicitLinkedFile, "explicitLinkedFile");
                    AutomationWrapper.Select(explicitLinkedFile);

                    try {
                        solution.ExecuteCommand("File.Rename");
                        Assert.Fail("Should have failed to rename");
                    } catch (Exception e) {
                        Debug.WriteLine(e.ToString());
                    }

                    var autoItem = solution.GetProject("LinkedFiles").ProjectItems.Item("ImplicitLinkedFile" + projectType.CodeExtension);
                    try {
                        autoItem.Properties.Item("FileName").Value = "Fob";
                        Assert.Fail("Should have failed to rename");
                    } catch (InvalidOperationException) {
                    }

                    autoItem = solution.GetProject("LinkedFiles").ProjectItems.Item("ExplicitDir").ProjectItems.Item("ExplicitLinkedFile" + projectType.CodeExtension);
                    try {
                        autoItem.Properties.Item("FileName").Value = "Fob";
                        Assert.Fail("Should have failed to rename");
                    } catch (InvalidOperationException) {
                    }
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void MoveLinkedNode() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {

                    var projectNode = solution.FindItem("LinkedFiles", "MovedLinkedFile" + projectType.CodeExtension);
                    Assert.IsNotNull(projectNode, "projectNode");
                    AutomationWrapper.Select(projectNode);

                    solution.ExecuteCommand("Edit.Cut");

                    var folderNode = solution.FindItem("LinkedFiles", "MoveToFolder");
                    AutomationWrapper.Select(folderNode);

                    solution.ExecuteCommand("Edit.Paste");

                    // item should have moved
                    var movedLinkedFile = solution.WaitForItem("LinkedFiles", "MoveToFolder", "MovedLinkedFile" + projectType.CodeExtension);
                    Assert.IsNotNull(movedLinkedFile, "movedLinkedFile");

                    // file should be at the same location
                    Assert.IsTrue(File.Exists(Path.Combine(solution.SolutionDirectory, "MovedLinkedFile" + projectType.CodeExtension)));
                    Assert.IsFalse(File.Exists(Path.Combine(solution.SolutionDirectory, "MoveToFolder\\MovedLinkedFile" + projectType.CodeExtension)));

                    // now move it back
                    AutomationWrapper.Select(movedLinkedFile);
                    solution.ExecuteCommand("Edit.Cut");

                    var originalFolder = solution.FindItem("LinkedFiles");
                    AutomationWrapper.Select(originalFolder);
                    solution.ExecuteCommand("Edit.Paste");

                    var movedLinkedFilePaste = solution.WaitForItem("LinkedFiles", "MovedLinkedFile" + projectType.CodeExtension);
                    Assert.IsNotNull(movedLinkedFilePaste, "movedLinkedFilePaste");

                    // and make sure we didn't mess up the path in the project file
                    MSBuild.Project buildProject = new MSBuild.Project(solution.GetProject("LinkedFiles").FullName);
                    bool found = false;
                    foreach (var item in buildProject.GetItems("Compile")) {
                        if (item.UnevaluatedInclude == "..\\MovedLinkedFile" + projectType.CodeExtension) {
                            found = true;
                            break;
                        }
                    }

                    Assert.IsTrue(found);
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void MultiProjectMove() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = MultiProjectLinkedFiles(projectType).ToVs()) {

                    var fileNode = solution.FindItem("LinkedFiles1", "FileNotInProject1" + projectType.CodeExtension);
                    Assert.IsNotNull(fileNode, "projectNode");
                    AutomationWrapper.Select(fileNode);

                    solution.ExecuteCommand("Edit.Copy");

                    var folderNode = solution.FindItem("LinkedFiles2");
                    AutomationWrapper.Select(folderNode);

                    solution.ExecuteCommand("Edit.Paste");

                    // item should have moved
                    var copiedFile = solution.WaitForItem("LinkedFiles2", "FileNotInProject1" + projectType.CodeExtension);
                    Assert.IsNotNull(copiedFile, "movedLinkedFile");

                    Assert.AreEqual(
                        true,
                        solution.GetProject("LinkedFiles2").ProjectItems.Item("FileNotInProject1" + projectType.CodeExtension).Properties.Item("IsLinkFile").Value
                    );
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void MultiProjectMoveExists2() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = MultiProjectLinkedFiles(projectType).ToVs()) {

                    var fileNode = solution.FindItem("LinkedFiles1", "FileNotInProject2" + projectType.CodeExtension);
                    Assert.IsNotNull(fileNode, "projectNode");
                    AutomationWrapper.Select(fileNode);

                    solution.ExecuteCommand("Edit.Copy");

                    var folderNode = solution.FindItem("LinkedFiles2");
                    AutomationWrapper.Select(folderNode);

                    ThreadPool.QueueUserWorkItem(x => solution.ExecuteCommand("Edit.Paste"));

                    string path = Path.Combine(solution.SolutionDirectory, "FileNotInProject2" + projectType.CodeExtension);
                    VisualStudioApp.CheckMessageBox(String.Format("There is already a link to '{0}'. You cannot have more than one link to the same file in a project.", path));

                    solution.WaitForDialogDismissed();
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void MoveLinkedNodeOpen() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {

                    var openWindow = solution.GetProject("LinkedFiles").ProjectItems.Item("MovedLinkedFileOpen" + projectType.CodeExtension).Open();
                    Assert.IsNotNull(openWindow, "openWindow");

                    var projectNode = solution.FindItem("LinkedFiles", "MovedLinkedFileOpen" + projectType.CodeExtension);

                    Assert.IsNotNull(projectNode, "projectNode");
                    AutomationWrapper.Select(projectNode);

                    solution.ExecuteCommand("Edit.Cut");

                    var folderNode = solution.FindItem("LinkedFiles", "MoveToFolder");
                    AutomationWrapper.Select(folderNode);

                    solution.ExecuteCommand("Edit.Paste");

                    var movedLinkedFileOpen = solution.WaitForItem("LinkedFiles", "MoveToFolder", "MovedLinkedFileOpen" + projectType.CodeExtension);
                    Assert.IsNotNull(movedLinkedFileOpen, "movedLinkedFileOpen");

                    Assert.IsTrue(File.Exists(Path.Combine(solution.SolutionDirectory, Path.Combine(solution.SolutionDirectory, "MovedLinkedFileOpen" + projectType.CodeExtension))));
                    Assert.IsFalse(File.Exists(Path.Combine(solution.SolutionDirectory, "MoveToFolder\\MovedLinkedFileOpen" + projectType.CodeExtension)));

                    // window sholudn't have changed.
                    Assert.AreEqual(VSTestContext.DTE.Windows.Item("MovedLinkedFileOpen" + projectType.CodeExtension), openWindow);
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void MoveLinkedNodeOpenEdited() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {

                    var openWindow = solution.GetProject("LinkedFiles").ProjectItems.Item("MovedLinkedFileOpenEdit" + projectType.CodeExtension).Open();
                    Assert.IsNotNull(openWindow, "openWindow");

                    var selection = ((TextSelection)openWindow.Selection);
                    selection.SelectAll();
                    selection.Delete();

                    var projectNode = solution.FindItem("LinkedFiles", "MovedLinkedFileOpenEdit" + projectType.CodeExtension);

                    Assert.IsNotNull(projectNode, "projectNode");
                    AutomationWrapper.Select(projectNode);

                    solution.ExecuteCommand("Edit.Cut");

                    var folderNode = solution.FindItem("LinkedFiles", "MoveToFolder");
                    AutomationWrapper.Select(folderNode);

                    solution.ExecuteCommand("Edit.Paste");

                    var movedLinkedFileOpenEdit = solution.WaitForItem("LinkedFiles", "MoveToFolder", "MovedLinkedFileOpenEdit" + projectType.CodeExtension);
                    Assert.IsNotNull(movedLinkedFileOpenEdit, "movedLinkedFileOpenEdit");

                    Assert.IsTrue(File.Exists(Path.Combine(solution.SolutionDirectory, "MovedLinkedFileOpenEdit" + projectType.CodeExtension)));
                    Assert.IsFalse(File.Exists(Path.Combine(solution.SolutionDirectory, "MoveToFolder\\MovedLinkedFileOpenEdit" + projectType.CodeExtension)));

                    // window sholudn't have changed.
                    Assert.AreEqual(VSTestContext.DTE.Windows.Item("MovedLinkedFileOpenEdit" + projectType.CodeExtension), openWindow);

                    Assert.AreEqual(openWindow.Document.Saved, false);
                    openWindow.Document.Save();

                    Assert.AreEqual(new FileInfo(Path.Combine(solution.SolutionDirectory, "MovedLinkedFileOpenEdit" + projectType.CodeExtension)).Length, (long)0);
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void MoveLinkedNodeFileExistsButNotInProject() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {

                    var fileNode = solution.FindItem("LinkedFiles", "FileNotInProject" + projectType.CodeExtension);
                    Assert.IsNotNull(fileNode, "projectNode");
                    AutomationWrapper.Select(fileNode);

                    solution.ExecuteCommand("Edit.Cut");

                    var folderNode = solution.FindItem("LinkedFiles", "FolderWithAFile");
                    AutomationWrapper.Select(folderNode);

                    solution.ExecuteCommand("Edit.Paste");

                    // item should have moved
                    var fileNotInProject = solution.WaitForItem("LinkedFiles", "FolderWithAFile", "FileNotInProject" + projectType.CodeExtension);
                    Assert.IsNotNull(fileNotInProject, "fileNotInProject");

                    // but it should be the linked file on disk outside of our project, not the file that exists on disk at the same location.
                    var autoItem = solution.GetProject("LinkedFiles").ProjectItems.Item("FolderWithAFile").ProjectItems.Item("FileNotInProject" + projectType.CodeExtension);
                    Assert.AreEqual(Path.Combine(solution.SolutionDirectory, "FileNotInProject" + projectType.CodeExtension), autoItem.Properties.Item("FullPath").Value);
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void DeleteLinkedNode() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {
                    var projectNode = solution.FindItem("LinkedFiles", "DeletedLinkedFile" + projectType.CodeExtension);
                    Assert.IsNotNull(projectNode, "projectNode");
                    AutomationWrapper.Select(projectNode);

                    solution.ExecuteCommand("Edit.Delete");

                    projectNode = solution.FindItem("LinkedFiles", "DeletedLinkedFile" + projectType.CodeExtension);
                    Assert.AreEqual(null, projectNode);
                    Assert.IsTrue(File.Exists(Path.Combine(solution.SolutionDirectory, "DeletedLinkedFile" + projectType.CodeExtension)));
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void LinkedFileInProjectIgnored() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {
                    var projectNode = solution.FindItem("LinkedFiles", "Fob", "LinkedInModule" + projectType.CodeExtension);

                    Assert.IsNull(projectNode);
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void SaveAsCreateLink() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {

                    var autoItem = solution.GetProject("LinkedFiles").ProjectItems.Item("SaveAsCreateLink" + projectType.CodeExtension);
                    var isLinkFile = autoItem.Properties.Item("IsLinkFile").Value;
                    Assert.AreEqual(isLinkFile, false);

                    var itemWindow = autoItem.Open();

                    autoItem.SaveAs("..\\SaveAsCreateLink" + projectType.CodeExtension);


                    autoItem = solution.GetProject("LinkedFiles").ProjectItems.Item("SaveAsCreateLink" + projectType.CodeExtension);
                    isLinkFile = autoItem.Properties.Item("IsLinkFile").Value;
                    Assert.AreEqual(isLinkFile, true);
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void SaveAsCreateFile() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {

                    var autoItem = solution.GetProject("LinkedFiles").ProjectItems.Item("SaveAsCreateFile" + projectType.CodeExtension);
                    var isLinkFile = autoItem.Properties.Item("IsLinkFile").Value;
                    Assert.AreEqual(isLinkFile, true);

                    var itemWindow = autoItem.Open();

                    autoItem.SaveAs(Path.Combine(solution.SolutionDirectory, "LinkedFiles\\SaveAsCreateFile" + projectType.CodeExtension));

                    autoItem = solution.GetProject("LinkedFiles").ProjectItems.Item("SaveAsCreateFile" + projectType.CodeExtension);
                    isLinkFile = autoItem.Properties.Item("IsLinkFile").Value;
                    Assert.AreEqual(isLinkFile, false);
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void SaveAsCreateFileNewDirectory() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {

                    var autoItem = solution.GetProject("LinkedFiles").ProjectItems.Item("SaveAsCreateFileNewDirectory" + projectType.CodeExtension);
                    var isLinkFile = autoItem.Properties.Item("IsLinkFile").Value;
                    Assert.AreEqual(isLinkFile, true);

                    var itemWindow = autoItem.Open();

                    Directory.CreateDirectory(Path.Combine(solution.SolutionDirectory, "LinkedFiles\\CreatedDirectory"));
                    autoItem.SaveAs(Path.Combine(solution.SolutionDirectory, "LinkedFiles\\CreatedDirectory\\SaveAsCreateFileNewDirectory" + projectType.CodeExtension));


                    autoItem = solution.GetProject("LinkedFiles").ProjectItems.Item("CreatedDirectory").ProjectItems.Item("SaveAsCreateFileNewDirectory" + projectType.CodeExtension);
                    isLinkFile = autoItem.Properties.Item("IsLinkFile").Value;
                    Assert.AreEqual(isLinkFile, false);
                }
            }
        }

        /// <summary>
        /// Adding a duplicate link to the same item
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void AddExistingItem() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {
                    var projectNode = solution.FindItem("LinkedFiles", "FolderWithAFile");
                    Assert.IsNotNull(projectNode, "projectNode");
                    AutomationWrapper.Select(projectNode);

                    using (var addExistingDlg = solution.AddExistingItem()) {
                        addExistingDlg.FileName = Path.Combine(solution.SolutionDirectory, "ExistingItem" + projectType.CodeExtension);
                        addExistingDlg.AddLink();
                    }

                    var existingItem = solution.WaitForItem("LinkedFiles", "FolderWithAFile", "ExistingItem" + projectType.CodeExtension);
                    Assert.IsNotNull(existingItem, "existingItem");
                }
            }
        }

        /// <summary>
        /// Adding a link to a folder which is already linked in somewhere else.
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void AddExistingItemAndItemIsAlreadyLinked() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {
                    var projectNode = solution.FindItem("LinkedFiles", "AlreadyLinkedFolder");
                    Assert.IsNotNull(projectNode, "projectNode");
                    AutomationWrapper.Select(projectNode);

                    using (var addExistingDlg = solution.AddExistingItem()) {
                        addExistingDlg.FileName = Path.Combine(solution.SolutionDirectory, "FileNotInProject" + projectType.CodeExtension);
                        addExistingDlg.AddLink();
                    }

                    solution.WaitForDialog();
                    solution.CheckMessageBox(MessageBoxButton.Ok, "There is already a link to", "A project cannot have more than one link to the same file.", "FileNotInProject" + projectType.CodeExtension);
                }
            }
        }

        /// <summary>
        /// Adding a duplicate link to the same item.
        /// 
        /// Also because the linked file dir is "LinkedFilesDir" which is a substring of "LinkedFiles" (our project name)
        /// this verifies we deal with the project name string comparison correctly (including a \ at the end of the
        /// path).
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void AddExistingItemAndLinkAlreadyExists() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {
                    var projectNode = solution.FindItem("LinkedFiles", "Oar");
                    Assert.IsNotNull(projectNode, "projectNode");
                    AutomationWrapper.Select(projectNode);

                    using (var addExistingDlg = solution.AddExistingItem()) {
                        addExistingDlg.FileName = Path.Combine(solution.SolutionDirectory, "LinkedFilesDir\\SomeLinkedFile" + projectType.CodeExtension);
                        addExistingDlg.AddLink();
                    }

                    solution.WaitForDialog();
                    solution.CheckMessageBox(MessageBoxButton.Ok, "There is already a link to", "SomeLinkedFile" + projectType.CodeExtension);
                }
            }
        }

        /// <summary>
        /// Adding new linked item when file of same name exists (when the file only exists on disk)
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void AddExistingItemAndFileByNameExistsOnDiskButNotInProject() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {
                    var projectNode = solution.FindItem("LinkedFiles", "FolderWithAFile");
                    Assert.IsNotNull(projectNode, "projectNode");
                    AutomationWrapper.Select(projectNode);


                    using (var addExistingDlg = solution.AddExistingItem()) {
                        addExistingDlg.FileName = Path.Combine(solution.SolutionDirectory, "ExistsOnDiskButNotInProject" + projectType.CodeExtension);
                        addExistingDlg.AddLink();
                    }

                    solution.WaitForDialog();
                    solution.CheckMessageBox(MessageBoxButton.Ok, "There is already a file of the same name in this folder.");
                }
            }
        }

        /// <summary>
        /// Adding new linked item when file of same name exists (both in the project and on disk)
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void AddExistingItemAndFileByNameExistsOnDiskAndInProject() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {
                    var projectNode = solution.FindItem("LinkedFiles", "FolderWithAFile");
                    Assert.IsNotNull(projectNode, "projectNode");
                    AutomationWrapper.Select(projectNode);


                    using (var addExistingDlg = solution.AddExistingItem()) {
                        addExistingDlg.FileName = Path.Combine(solution.SolutionDirectory, "ExistsOnDiskAndInProject" + projectType.CodeExtension);
                        addExistingDlg.AddLink();
                    }

                    solution.WaitForDialog();
                    solution.CheckMessageBox(MessageBoxButton.Ok, "There is already a file of the same name in this folder.");
                }
            }
        }

        /// <summary>
        /// Adding new linked item when file of same name exists (in the project, but not on disk)
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void AddExistingItemAndFileByNameExistsInProjectButNotOnDisk() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {
                    var projectNode = solution.FindItem("LinkedFiles", "FolderWithAFile");
                    Assert.IsNotNull(projectNode, "projectNode");
                    AutomationWrapper.Select(projectNode);

                    using (var addExistingDlg = solution.AddExistingItem()) {
                        addExistingDlg.FileName = Path.Combine(solution.SolutionDirectory, "ExistsInProjectButNotOnDisk" + projectType.CodeExtension);
                        addExistingDlg.AddLink();
                    }

                    solution.WaitForDialog();
                    solution.CheckMessageBox(MessageBoxButton.Ok, "There is already a file of the same name in this folder.");
                }
            }
        }

        /// <summary>
        /// Adding new linked item when the file lives in the project dir but not in the directory we selected
        /// Add Existing Item from.  We should add the file to the directory where it lives.
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void AddExistingItemAsLinkButFileExistsInProjectDirectory() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {
                    var projectNode = solution.FindItem("LinkedFiles", "Fob");
                    Assert.IsNotNull(projectNode, "projectNode");
                    AutomationWrapper.Select(projectNode);

                    using (var addExistingDlg = solution.AddExistingItem()) {
                        addExistingDlg.FileName = Path.Combine(solution.SolutionDirectory, "LinkedFiles\\Fob\\AddExistingInProjectDirButNotInProject" + projectType.CodeExtension);
                        addExistingDlg.AddLink();
                    }

                    var addExistingInProjectDirButNotInProject = solution.WaitForItem("LinkedFiles", "Fob", "AddExistingInProjectDirButNotInProject" + projectType.CodeExtension);
                    Assert.IsNotNull(addExistingInProjectDirButNotInProject, "addExistingInProjectDirButNotInProject");
                }
            }
        }

        /// <summary>
        /// Reaming the file name in the Link attribute is ignored.
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void RenamedLinkedFile() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {
                    var projectNode = solution.FindItem("LinkedFiles", "Fob", "NewNameForLinkFile" + projectType.CodeExtension);
                    Assert.IsNull(projectNode);

                    var renamedLinkFile = solution.FindItem("LinkedFiles", "Fob", "RenamedLinkFile" + projectType.CodeExtension);
                    Assert.IsNotNull(renamedLinkFile, "renamedLinkFile");
                }
            }
        }

        /// <summary>
        /// A link path outside of our project dir will result in the link being ignored.
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void BadLinkPath() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {
                    var projectNode = solution.FindItem("LinkedFiles", "..");
                    Assert.IsNull(projectNode);

                    projectNode = solution.FindItem("LinkedFiles", "BadLinkPathFolder");
                    Assert.IsNull(projectNode);
                }
            }
        }

        /// <summary>
        /// A rooted link path is ignored.
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void RootedLinkIgnored() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {
                    var projectNode = solution.FindItem("LinkedFiles", "RootedLinkIgnored" + projectType.CodeExtension);
                    Assert.IsNull(projectNode);
                }
            }
        }

        /// <summary>
        /// A rooted link path is ignored.
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void RootedIncludeIgnored() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = LinkedFiles(projectType).Generate().ToVs()) {
                    var rootedIncludeIgnored = solution.FindItem("LinkedFiles", "RootedIncludeIgnored" + projectType.CodeExtension);
                    Assert.IsNotNull(rootedIncludeIgnored, "rootedIncludeIgnored");
                }
            }
        }

        /// <summary>
        /// Test linked files with a project home set (done by save as in this test)
        /// https://nodejstools.codeplex.com/workitem/1511
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void TestLinkedWithProjectHome() {
            foreach (var projectType in ProjectTypes) {
                using (var solution = MultiProjectLinkedFiles(projectType).ToVs()) {
                    var project = (solution as VisualStudioInstance).Project;
                    
                    // save the project to an odd location.  This will result in project home being set.
                    var newProjName = "TempFile";
                    try {
                        project.SaveAs(Path.GetTempPath() +  newProjName + projectType.ProjectExtension);
                    } catch (UnauthorizedAccessException) {
                        Assert.Inconclusive("Couldn't save the file");
                    }
                    
                    // create a temporary file and add a link to it in the project
                    solution.FindItem(newProjName).Select();
                    var tempFile  = Path.GetTempFileName();
                    using (var addExistingDlg = AddExistingItemDialog.FromDte((solution as VisualStudioInstance).App)) {
                        addExistingDlg.FileName = tempFile;
                        addExistingDlg.AddLink();
                    }

                    // Save the project to commit that link to the project file
                    project.Save();

                    // verify that the project file contains the correct text for Link
                    var fileText = File.ReadAllText(project.FullName);
                    var pattern = string.Format(
                        @"<Content Include=""{0}"">\s*<Link>{1}</Link>\s*</Content>", 
                        Regex.Escape(tempFile),
                        Regex.Escape(Path.GetFileName(tempFile)));
                    AssertUtil.AreEqual(new Regex(pattern), fileText);
                }
            }
        }
    }
}
