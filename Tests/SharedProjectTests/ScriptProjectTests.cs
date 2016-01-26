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

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtilities;
using TestUtilities.SharedProject;
using TestUtilities.UI;

namespace Microsoft.VisualStudioTools.SharedProjectTests {
    /// <summary>
    /// Test cases which are applicable to projects designed for scripting languages.
    /// </summary>
    [TestClass]
    public class ScriptProjectTests : SharedProjectTest {
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void RunWithoutStartupFile() {
            foreach (var projectType in ProjectTypes) {
                var testDef = new ProjectDefinition("RunWithoutStartupFile", projectType);

                using (var solution = testDef.Generate().ToVs()) {
                    solution.OpenDialogWithDteExecuteCommand("Debug.Start");
                    solution.CheckMessageBox("startup file");

                    solution.OpenDialogWithDteExecuteCommand("Debug.StartWithoutDebugging");
                    solution.CheckMessageBox("startup file");
                }
            }
        }

        /// <summary>
        /// Renaming the folder containing the startup script should update the startup script
        /// https://nodejstools.codeplex.com/workitem/476
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void RenameStartupFileFolder() {
            foreach (var projectType in ProjectTypes) {
                var testDef = new ProjectDefinition(
                    "RenameStartupFileFolder", 
                    projectType,
                    Folder("Folder"),
                    Compile("Folder\\server"),
                    Property("StartupFile", "Folder\\server" + projectType.CodeExtension)
                );

                using (var solution = testDef.Generate().ToVs()) {
                    var folder = solution.GetProject("RenameStartupFileFolder").ProjectItems.Item("Folder");
                    folder.Name = "FolderNew";

                    string startupFile = (string)solution.GetProject("RenameStartupFileFolder").Properties.Item("StartupFile").Value;
                    Assert.IsTrue(
                        startupFile.EndsWith(projectType.Code("FolderNew\\server")),
                        "Expected FolderNew in path, got {0}",
                        startupFile
                    );
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void RenameStartupFile() {
            foreach (var projectType in ProjectTypes) {
                var testDef = new ProjectDefinition(
                    "RenameStartupFileFolder",
                    projectType,
                    Folder("Folder"),
                    Compile("Folder\\server"),
                    Property("StartupFile", "Folder\\server" + projectType.CodeExtension)
                );

                using (var solution = testDef.Generate().ToVs()) {
                    var file = solution.GetProject("RenameStartupFileFolder").ProjectItems.Item("Folder").ProjectItems.Item("server" + projectType.CodeExtension);
                    file.Name = "server2" + projectType.CodeExtension;

                    Assert.AreEqual(
                        "server2" + projectType.CodeExtension,
                        Path.GetFileName(
                            (string)solution.GetProject("RenameStartupFileFolder").Properties.Item("StartupFile").Value
                        )
                    );
                }
            }
        }
    }
}
