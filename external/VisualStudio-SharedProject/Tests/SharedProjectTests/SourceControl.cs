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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Automation;
using System.Windows.Input;
using Microsoft.TestSccPackage;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestUtilities;
using TestUtilities.SharedProject;
using TestUtilities.UI;
using Keyboard = TestUtilities.UI.Keyboard;
using Mouse = TestUtilities.UI.Mouse;
using MessageBoxButton = TestUtilities.MessageBoxButton;

namespace Microsoft.VisualStudioTools.SharedProjectTests {
    [TestClass]
    public class SourceControl : SharedProjectTest {
        private static Regex _pathRegex = new Regex(@"\{path:([^}]*)\}");
        const string VSQUERYRENAMEFILEFLAGS_NoFlags = "VSQUERYRENAMEFILEFLAGS_NoFlags";
        const string VSQUERYRENAMEFILEFLAGS_Directory = "VSQUERYRENAMEFILEFLAGS_Directory";
        const string VSRENAMEFILEFLAGS_NoFlags = "VSRENAMEFILEFLAGS_NoFlags";
        const string VSRENAMEFILEFLAGS_Directory = "VSRENAMEFILEFLAGS_Directory";

        /// <summary>
        /// https://nodejstools.codeplex.com/workitem/194
        /// 
        /// Verifies that we deliver the right set of track events to the source
        /// when we move a folder with a file in it.
        /// 
        /// The right set of events are based upon matching the same events the C#
        /// project system delivers.  Those can be using the TestSccPackage with a
        /// C# project.  Once enables in Tools->Options->Source Control you can get
        /// the list of events from Tools->Show Scc Track Document Events.
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        // Currently Fails: https://pytools.codeplex.com/workitem/2609
        public void MoveFolderWithItem() {
            using (var app = new VisualStudioApp()) {

                // close any projects before switching source control...
                app.Dte.Solution.Close();

                app.SelectSourceControlProvider("Test Source Provider");

                ExpectSourceControl();

                foreach (var projectType in ProjectTypes) {
                    var testDef = new ProjectDefinition("SourceControl", projectType,
                        PropertyGroup(
                            Property("SccProjectName", "HelloWorld"),
                            Property("SccLocalPath", "LocalPath"),
                            Property("SccAuxPath", "AuxPath"),
                            Property("SccProvider", "TestProvider")
                        ),
                        ItemGroup(
                            Folder("Fob"),
                            Folder("Fob\\Oar"),
                            Compile("Program"),
                            Compile("Fob\\Oar\\Quox")
                        )
                    );

                    using (var solution = testDef.Generate()) {
                        TestSccProvider.DocumentEvents.Clear();

                        var project = app.OpenProject(solution.Filename);
                        var window = app.SolutionExplorerTreeView;
                        var folder = window.WaitForItem("Solution 'SourceControl' (1 project)", "SourceControl", "Fob", "Oar");
                        var point = folder.GetClickablePoint();
                        Mouse.MoveTo(point);
                        Mouse.Down(MouseButton.Left);

                        var destFolder = window.WaitForItem("Solution 'SourceControl' (1 project)", "SourceControl");
                        Mouse.MoveTo(destFolder.GetClickablePoint());
                        Mouse.Up(MouseButton.Left);

                        window.AssertFileExists(Path.GetDirectoryName(solution.Filename), "Solution 'SourceControl' (1 project)", "SourceControl", "Oar", "Quox" + projectType.CodeExtension);
                        var projectDir = Path.GetDirectoryName(project.FullName);
                        AssertDocumentEvents(projectDir,
                            OnQueryRenameFiles(projectType.Code("Fob\\Oar\\Quox"), projectType.Code("Oar\\Quox"), VSQUERYRENAMEFILEFLAGS_NoFlags),
                            OnQueryRenameFiles("Fob\\Oar\\", "Oar", VSQUERYRENAMEFILEFLAGS_Directory),
                            OnAfterRenameFiles(projectType.Code("Fob\\Oar\\Quox"), projectType.Code("Oar\\Quox"), VSRENAMEFILEFLAGS_NoFlags),
                            OnAfterRenameFiles("Fob\\Oar\\", "Oar", VSRENAMEFILEFLAGS_Directory)
                        );
                    }
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void AddNewItem() {
            using (var app = new VisualStudioApp()) {

                // close any projects before switching source control...
                app.Dte.Solution.Close();

                app.SelectSourceControlProvider("Test Source Provider");
                foreach (var projectType in ProjectTypes) {
                    var testDef = SourceControlProject(projectType);

                    using (var solution = testDef.Generate()) {
                        TestSccProvider.DocumentEvents.Clear();

                        var project = app.OpenProject(solution.Filename);
                        var fileName = "NewFile" + projectType.CodeExtension;

                        using (var newItem = NewItemDialog.FromDte(app)) {
                            newItem.FileName = fileName;
                            newItem.OK();
                        }

                        System.Threading.Thread.Sleep(250);

                        Assert.IsNotNull(project.ProjectItems.Item(fileName));
                        AssertDocumentEvents(Path.GetDirectoryName(project.FullName),
                            OnQueryAddFiles(fileName),
                            OnAfterAddFilesEx(fileName)
                        );
                    }
                }
            }
        }

        public void AddExistingItem() {
            using (var app = new VisualStudioApp()) {

                // close any projects before switching source control...
                app.Dte.Solution.Close();

                app.SelectSourceControlProvider("Test Source Provider");
                foreach (var projectType in ProjectTypes) {
                    var testDef = SourceControlProject(projectType);

                    using (var solution = testDef.Generate()) {
                        TestSccProvider.DocumentEvents.Clear();

                        var project = app.OpenProject(solution.Filename);
                        var fileName = projectType.Code(@"ExcludedFile");

                        using (var newItem = AddExistingItemDialog.FromDte(app)) {
                            newItem.FileName = fileName;
                            newItem.OK();
                        }

                        System.Threading.Thread.Sleep(250);

                        Assert.IsNotNull(project.ProjectItems.Item(fileName));
                        AssertDocumentEvents(Path.GetDirectoryName(project.FullName),
                            OnQueryAddFiles(fileName),
                            OnAfterAddFilesEx(fileName)
                        );
                    }
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void IncludeInProject() {
            using (var app = new VisualStudioApp()) {
                // close any projects before switching source control...
                app.Dte.Solution.Close();

                app.SelectSourceControlProvider("Test Source Provider");
                foreach (var projectType in ProjectTypes) {
                    var testDef = SourceControlProject(projectType);

                    using (var solution = testDef.Generate().ToVs()) {
                        TestSccProvider.DocumentEvents.Clear();
                        var project = app.OpenProject(solution.SolutionFilename);
                        var window = app.SolutionExplorerTreeView;
                        var fileName = projectType.Code(@"ExcludedFile");

                        // Try to select the file.  If it throws, it is likely the issue was that we weren't showing all files.
                        try {
                            window.WaitForChildOfProject(project, fileName).Select();
                        } catch (Exception) {
                            // Show all files so we can see the excluded item if we previously couldn't
                            solution.ExecuteCommand("Project.ShowAllFiles");
                            window.WaitForChildOfProject(project, fileName).Select();
                        }

                        solution.ExecuteCommand("Project.IncludeInProject");

                        System.Threading.Thread.Sleep(250);

                        AssertDocumentEvents(Path.GetDirectoryName(project.FullName),
                            OnQueryAddFiles(fileName),
                            OnAfterAddFilesEx(fileName)
                        );
                    }
                }
            }
        }

        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void RemoveItem() {
            using (var app = new VisualStudioApp()) {

                // close any projects before switching source control...
                app.Dte.Solution.Close();

                app.SelectSourceControlProvider("Test Source Provider");
                foreach (var projectType in ProjectTypes) {
                    var testDef = SourceControlProject(projectType);

                    using (var solution = testDef.Generate()) {
                        TestSccProvider.DocumentEvents.Clear();

                        var project = app.OpenProject(solution.Filename);
                        var window = app.SolutionExplorerTreeView;
                        var fileName = "Program" + projectType.CodeExtension;
                        var program = window.WaitForChildOfProject(project, fileName);

                        program.Select();

                        Keyboard.Type(Key.Delete);
                        app.WaitForDialog();
                        VisualStudioApp.CheckMessageBox(MessageBoxButton.Ok, "will be deleted permanently");
                        app.WaitForDialogDismissed();

                        window.WaitForChildOfProjectRemoved(project, fileName);

                        var projectDir = Path.GetDirectoryName(project.FullName);

                        AssertDocumentEvents(projectDir,
                            OnQueryRemoveFiles(fileName),
                            OnAfterRemoveFiles(fileName)
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Verify we get called w/ a project which does have source control enabled.
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void BasicSourceControl() {
            using (var app = new VisualStudioApp()) {

                // close any projects before switching source control...
                app.Dte.Solution.Close();

                app.SelectSourceControlProvider("Test Source Provider");

                ExpectSourceControl();

                foreach (var projectType in ProjectTypes) {
                    var testDef = SourceControlProject(projectType);

                    using (var solution = testDef.Generate()) {
                        var project = app.OpenProject(solution.Filename);

                        Assert.AreEqual(1, TestSccProvider.LoadedProjects.Count);

                        TestSccProvider.ExpectedAuxPath = null;
                        TestSccProvider.ExpectedLocalPath = null;
                        TestSccProvider.ExpectedProvider = null;
                        TestSccProvider.ExpectedProjectName = null;

                        TestSccProvider.LoadedProjects.First().SccProject.SetSccLocation(
                            "NewProjectName",
                            "NewAuxPath",
                            "NewLocalPath",
                            "NewProvider"
                        );

                        app.Dte.Solution.Close();

                        Assert.AreEqual(0, TestSccProvider.LoadedProjects.Count);
                        if (TestSccProvider.Failures.Count != 0) {
                            Assert.Fail(String.Join(Environment.NewLine, TestSccProvider.Failures));
                        }

                        app.SelectSourceControlProvider("None");
                    }
                }
            }
        }

        /// <summary>
        /// Verify the glyph change APIs update the glyphs appropriately
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void SourceControlGlyphChanged() {
            using (var app = new VisualStudioApp()) {

                // close any projects before switching source control...
                app.Dte.Solution.Close();

                app.SelectSourceControlProvider("Test Source Provider");

                foreach (var projectType in ProjectTypes) {
                    var testDef = SourceControlProject(projectType);
                    using (var solution = testDef.Generate()) {
                        var project = app.OpenProject(solution.Filename);

                        Assert.AreEqual(1, TestSccProvider.LoadedProjects.Count);
                        var sccProject = TestSccProvider.LoadedProjects.First();
                        Microsoft.TestSccPackage.FileInfo fileInfo = null;
                        foreach (var curFile in sccProject.Files) {
                            if (curFile.Key.EndsWith("Program" + projectType.CodeExtension)) {
                                fileInfo = curFile.Value;
                                break;
                            }
                        }
                        Assert.IsNotNull(fileInfo);

                        fileInfo.GlyphChanged(VsStateIcon.STATEICON_CHECKEDOUTEXCLUSIVEOTHER);

                        var programPy = project.ProjectItems.Item("Program" + projectType.CodeExtension);
                        Assert.AreEqual(programPy.Properties.Item("SourceControlStatus").Value, "CHECKEDOUTEXCLUSIVEOTHER");

                        fileInfo.StateIcon = VsStateIcon.STATEICON_READONLY;
                        sccProject.AllGlyphsChanged();

                        Assert.AreEqual(programPy.Properties.Item("SourceControlStatus").Value, "READONLY");

                        app.Dte.Solution.Close();

                        Assert.AreEqual(0, TestSccProvider.LoadedProjects.Count);
                        if (TestSccProvider.Failures.Count != 0) {
                            Assert.Fail(String.Join(Environment.NewLine, TestSccProvider.Failures));
                        }

                        app.SelectSourceControlProvider("None");
                    }
                }
            }
        }

        /// <summary>
        /// Verify we don't get called for a project which doesn't have source control enabled.
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void SourceControlNoControl() {
            using (var app = new VisualStudioApp()) {

                // close any projects before switching source control...
                app.Dte.Solution.Close();

                app.SelectSourceControlProvider("Test Source Provider");
                DontExpectSourceControl();

                foreach (var projectType in ProjectTypes) {
                    var testDef = NoSourceControlProject(projectType);
                    using (var solution = testDef.Generate()) {
                        var project = app.OpenProject(solution.Filename);

                        Assert.AreEqual(0, TestSccProvider.LoadedProjects.Count);

                        app.Dte.Solution.Close();

                        Assert.AreEqual(0, TestSccProvider.LoadedProjects.Count);
                        if (TestSccProvider.Failures.Count != 0) {
                            Assert.Fail(String.Join(Environment.NewLine, TestSccProvider.Failures));
                        }

                        app.SelectSourceControlProvider("None");
                    }
                }
            }
        }

        /// <summary>
        /// Verify non-member items don't get reported as source control files
        /// 
        /// https://pytools.codeplex.com/workitem/1417
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void SourceControlExcludedFilesNotPresent() {
            using (var app = new VisualStudioApp()) {
                foreach (var projectType in ProjectTypes) {
                    using (var solution = SourceControlProject(projectType).Generate()) {
                        // close any projects before switching source control...
                        app.Dte.Solution.Close();

                        app.SelectSourceControlProvider("Test Source Provider");

                        var project = app.OpenProject(solution.Filename);

                        Assert.AreEqual(1, TestSccProvider.LoadedProjects.Count);
                        var sccProject = TestSccProvider.LoadedProjects.First();
                        foreach (var curFile in sccProject.Files) {
                            Assert.IsFalse(curFile.Key.EndsWith("ExcludedFile" + projectType.CodeExtension), "found excluded file");
                        }
                    }
                }

                app.Dte.Solution.Close();
                app.SelectSourceControlProvider("None");
            }
        }

        /// <summary>
        /// Verify we get called w/ a project which does have source control enabled.
        /// </summary>
        [TestMethod, Priority(0), TestCategory("Core")]
        [HostType("VSTestHost")]
        public void SourceControlRenameFolder() {
            using (var app = new VisualStudioApp()) {
                foreach (var projectType in ProjectTypes) {
                    // close any projects before switching source control...
                    app.Dte.Solution.Close();

                    app.SelectSourceControlProvider("Test Source Provider");

                    TestSccProvider.DocumentEvents.Clear();

                    using (var solution = SourceControlProject(projectType).Generate()) {
                        try {
                            var project = app.OpenProject(solution.Filename);

                            project.ProjectItems.Item("TestFolder").Name = "Renamed";

                            AssertDocumentEvents(Path.GetDirectoryName(project.FullName),
                                OnQueryRenameFiles("TestFolder\\", "Renamed\\", VSQUERYRENAMEFILEFLAGS_Directory),
                                OnAfterRenameFiles("TestFolder\\", "Renamed", VSRENAMEFILEFLAGS_Directory)
                            );
                            app.Dte.Solution.Close();
                        } finally {
                            app.SelectSourceControlProvider("None");
                        }
                    }
                }
            }
        }

        #region Helper Methods

        /// <summary>
        /// Creates the document event string for OnQueryRemoveFiles that matches the output of
        /// the TestSccProvider.
        /// </summary>
        private static string OnQueryRemoveFiles(string source) {
            return "OnQueryRemoveFiles " +
                    ToPath(source);
        }

        /// <summary>
        /// Creates the document event string for OnAfterRemoveFiles that matches the output of
        /// the TestSccProvider.
        /// </summary>
        private static string OnAfterRemoveFiles(string source) {
            return "OnAfterRemoveFiles " +
                    ToPath(source);
        }

        /// <summary>
        /// Creates the document event string for OnQueryRenameFiles that matches the output of
        /// the TestSccProvider.
        /// </summary>
        private static string OnQueryRenameFiles(string source, string dest, string flags) {
            return "OnQueryRenameFiles " +
                    ToPath(source) +
                    " " +
                    ToPath(dest) +
                    " " +
                    flags;
        }

        /// <summary>
        /// Creates the document event strin g for OnQueryRenameFiles that matches the output
        /// of the TestSccProvider.
        /// </summary>
        private static string OnAfterRenameFiles(string source, string dest, string flags) {
            return "OnAfterRenameFiles " +
                    ToPath(source) +
                    " " +
                    ToPath(dest) +
                    " " +
                    flags;
        }

        /// <summary>
        /// Creates the document event string for OnQueryAddFiles that matches the output of
        /// the TestSccProvider.
        /// </summary>
        private static string OnQueryAddFiles(string source) {
            return "OnQueryAddFiles " +
                    ToPath(source);
        }

        /// <summary>
        /// Creates the document event strin g for OnAfterAddFilesEx that matches the output
        /// of the TestSccProvider.
        /// </summary>
        private static string OnAfterAddFilesEx(string source) {
            return "OnAfterAddFilesEx " +
                    ToPath(source);
        }

        /// <summary>
        /// Converts a path into the path regex which will be used in
        /// AssertDocumentEvents to combine the path here w/ a Path.Combine
        /// to the project path.
        /// </summary>
        private static string ToPath(string path) {
            var res = "{path:" + path + "}";
            Debug.Assert(_pathRegex.IsMatch(res));
            return res;
        }

        public static string ToFormatString(string format, string projectDir) {
            foreach (Match match in _pathRegex.Matches(format)) {
                format = format.Replace(match.Value, Path.Combine(projectDir, match.Groups[1].Value));
            }
            return format;
        }

        /// <summary>
        /// Asserts that the specified set of document events was received from the TestSccProvider
        /// </summary>
        private static void AssertDocumentEvents(string projectDir, params string[] events) {
            events = events.Select(str => ToFormatString(str, projectDir)).ToArray();

            var expected = String.Join(
                Environment.NewLine,
                events
            );
            string actual = String.Join(
                Environment.NewLine,
                TestSccProvider.DocumentEvents
            );
            if (expected != actual) {
                StringBuilder msg = new StringBuilder();
                msg.AppendLine();
                if (TestSccProvider.DocumentEvents.Count != events.Length) {
                    msg.AppendFormat("Got {0}, expected {1} items", TestSccProvider.DocumentEvents.Count, events.Length);
                    msg.AppendLine();
                }

                for (int i = 0; i < TestSccProvider.DocumentEvents.Count && i < events.Length; i++) {
                    if (TestSccProvider.DocumentEvents[i] != events[i]) {
                        msg.AppendFormat("Event {0} differs:", i);
                        msg.AppendLine();
                        msg.AppendFormat("  Expected: {0}", events[i]);
                        msg.AppendLine();
                        msg.AppendFormat("  Actual  : {0}", TestSccProvider.DocumentEvents[i]);
                    } else {
                        msg.AppendFormat("Event {0} matches", i);
                    }
                    msg.AppendLine();
                }

                Assert.AreEqual(expected, actual, msg.ToString());
            }
        }

        private static void ExpectSourceControl() {
            TestSccProvider.ExpectedAuxPath = "AuxPath";
            TestSccProvider.ExpectedLocalPath = "LocalPath";
            TestSccProvider.ExpectedProvider = "TestProvider";
            TestSccProvider.ExpectedProjectName = "HelloWorld";
        }

        private static void DontExpectSourceControl() {
            TestSccProvider.ExpectedAuxPath = null;
            TestSccProvider.ExpectedLocalPath = null;
            TestSccProvider.ExpectedProvider = null;
            TestSccProvider.ExpectedProjectName = null;
        }

        private static ProjectDefinition SourceControlProject(ProjectType projectType) {
            return new ProjectDefinition("SourceControl", projectType,
                PropertyGroup(
                    Property("SccProjectName", "HelloWorld"),
                    Property("SccLocalPath", "LocalPath"),
                    Property("SccAuxPath", "AuxPath"),
                    Property("SccProvider", "TestProvider")
                ),
                ItemGroup(
                    Folder("TestFolder"),
                    Compile("Program"),
                    Compile("TestFolder\\SubItem"),
                    Compile("ExcludedFile", isExcluded: true)
                )
            );
        }

        private static ProjectDefinition NoSourceControlProject(ProjectType projectType) {
            return new ProjectDefinition("NoSourceControl", projectType,
                ItemGroup(
                    Compile("Program")
                )
            );
        }

        #endregion
    }
}
