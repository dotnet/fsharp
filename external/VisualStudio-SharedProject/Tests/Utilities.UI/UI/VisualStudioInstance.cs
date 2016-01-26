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
using System.IO;
using System.Linq;
using System.Windows.Automation;
using System.Windows.Input;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudioTools.VSTestHost;
using TestUtilities.SharedProject;

namespace TestUtilities.UI {
    using Thread = System.Threading.Thread;

    /// <summary>
    /// Wrapper around a generated SolutionFile.  Provides helpers for simplifying
    /// interacting with the solution loaded into Solution Explorer.
    /// </summary>
    public class VisualStudioInstance : IDisposable, IVisualStudioInstance {
        private readonly SolutionFile _solution;
        private readonly VisualStudioApp _app;
        public readonly EnvDTE.Project Project;
        private SolutionExplorerTree _solutionExplorer;
        private bool _disposed;

        public VisualStudioInstance(SolutionFile solution) {
            _solution = solution;
            _app = new VisualStudioApp();
            Project = _app.OpenProject(solution.Filename);

            ThreadHelper.Generic.Invoke(Keyboard.Reset);
            _solutionExplorer = _app.OpenSolutionExplorer();
            SelectSolutionNode();
        }

        public VisualStudioApp App {
            get {
                return _app;
            }
        }

        public SolutionExplorerTree SolutionExplorer {
            get {
                return _solutionExplorer;
            }
        }

        IEditor IVisualStudioInstance.OpenItem(string project, params string[] path) {
            return OpenItem(project, path);
        }

        /// <summary>
        /// Opens the specified filename from the specified project name.
        /// </summary>
        public EditorWindow OpenItem(string project, params string[] path) {
            foreach (EnvDTE.Project proj in VSTestContext.DTE.Solution.Projects) {
                if (proj.Name == project) {
                    var items = proj.ProjectItems;
                    EnvDTE.ProjectItem item = null;
                    foreach (var itemName in path) {
                        item = items.Item(itemName);
                        items = item.ProjectItems;
                    }
                    Assert.IsNotNull(item);
                    var window = item.Open();
                    window.Activate();
                    return App.GetDocument(item.Document.FullName);
                    
                }
            }

            throw new InvalidOperationException(
                String.Format(
                    "Failed to find {0} item in project {1}",
                    String.Join("\\", path),
                    project
                )
            );
        }

        ITreeNode IVisualStudioInstance.FindItem(params string[] path) {
            var res = FindItem(path);
            if (res != null) {
                return new TreeNode(res);
            }
            return null;
        }

        public AutomationElement FindItem(params string[] path) {
            return SolutionExplorer.FindItem(AddSolutionToPath(path));
        }

        private string[] AddSolutionToPath(string[] path) {
            return new[] { SolutionNodeText }.Concat(path).ToArray();
        }

        public AutomationElement WaitForItem(params string[] path) {
            return SolutionExplorer.WaitForItem(AddSolutionToPath(path));
        }

        ITreeNode IVisualStudioInstance.WaitForItemRemoved(params string[] path) {
            var res = SolutionExplorer.WaitForItemRemoved(AddSolutionToPath(path));
            if (res != null) {
                return new TreeNode(res);
            }
            return null;
        }

        public AutomationElement WaitForItemRemoved(params string[] path) {
            return SolutionExplorer.WaitForItemRemoved(AddSolutionToPath(path));
        }

        public void ExecuteCommand(string command) {
            App.ExecuteCommand(command);
        }

        public string SolutionFilename {
            get {
                return _solution.Filename;
            }
        }

        public IntPtr WaitForDialog() {
            return App.WaitForDialog();
        }

        public void WaitForDialogDismissed() {
            App.WaitForDialogDismissed();
        }

        public string SolutionDirectory {
            get {
                return _solution.Directory;
            }
        }

        private string SolutionNodeText {
            get {
                if (_solution.Projects.Count(sln => !sln.Flags.HasFlag(SolutionElementFlags.ExcludeFromConfiguration) && !sln.Flags.HasFlag(SolutionElementFlags.ExcludeFromSolution)) > 1) {
                    return String.Format(
                        "Solution '{0}' ({1} projects)",
                        Path.GetFileNameWithoutExtension(_solution.Filename),
                        _solution.Projects.Length
                    );
                }
                return String.Format(
                    "Solution '{0}' (1 project)",
                    Path.GetFileNameWithoutExtension(_solution.Filename)
                );

            }
        }

        /// <summary>
        /// Selects the solution node using the mouse.
        /// 
        /// This is used to reset the state of the mouse before a test as some
        /// tests can cause the mouse to be left in an odd state - the mouse up
        /// event is delivered to solution explorer, but selecting items later
        /// doesn't work because the mouse is left in an odd state.  If you
        /// make this method a nop and try and run all of the tests you'll
        /// see the bad behavior.
        /// </summary>
        public void SelectSolutionNode() {
            // May need to reopen Solution Explorer so we can find a clickable
            // point.
            _solutionExplorer = _app.OpenSolutionExplorer();
            var item = SolutionExplorer.WaitForItem(SolutionNodeText);
            SolutionExplorer.CenterInView(item);
            Mouse.MoveTo(item.GetClickablePoint());
            Mouse.Click(MouseButton.Left);
        }

        #region IDisposable Members

        ~VisualStudioInstance() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!_disposed) {
                if (disposing) {
                    _app.Dispose();
                    _solution.Dispose();
                }

                _disposed = true;
            }
        }

        #endregion

        public void AssertFileExists(params string[] path) {
            SolutionExplorer.AssertFileExists(SolutionDirectory, AddSolutionToPath(path));
        }

        public void AssertFileDoesntExist(params string[] path) {
            SolutionExplorer.AssertFileDoesntExist(SolutionDirectory, AddSolutionToPath(path));
        }

        public void AssertFolderExists(params string[] path) {
            SolutionExplorer.AssertFolderExists(SolutionDirectory, AddSolutionToPath(path));
        }

        public void AssertFolderDoesntExist(params string[] path) {
            SolutionExplorer.AssertFolderDoesntExist(SolutionDirectory, AddSolutionToPath(path));
        }

        public void AssertFileExistsWithContent(string content, params string[] path) {
            SolutionExplorer.AssertFileExistsWithContent(SolutionDirectory, content, AddSolutionToPath(path));
        }

        public void CloseActiveWindow(vsSaveChanges save) {
            App.Dte.ActiveWindow.Close(vsSaveChanges.vsSaveChangesNo);
        }

        ITreeNode IVisualStudioInstance.WaitForItem(params string[] items) {
            var res = WaitForItem(items);
            if (res != null) {
                return new TreeNode(res);
            }
            return null;
        }

        public void Type(Key key) {
            Keyboard.Type(key);
        }

        public void ControlC() {
            Keyboard.ControlC();
        }

        public void ControlX() {
            Keyboard.ControlX();
        }

        public void Type(string p) {
            Keyboard.Type(p);
        }

        public void ControlV() {
            Keyboard.ControlV();
        }

        public void PressAndRelease(Key key, params Key[] modifier) {
            Keyboard.PressAndRelease(key, modifier);
        }

        public void CheckMessageBox(params string[] text) {
            VisualStudioApp.CheckMessageBox(text);
        }

        public void CheckMessageBox(MessageBoxButton button, params string[] text) {
            VisualStudioApp.CheckMessageBox(button, text);
        }

        public void Sleep(int ms) {
            Thread.Sleep(ms);
        }

        public void WaitForOutputWindowText(string name, string containsText, int timeout = 5000) {
            App.WaitForOutputWindowText(name, containsText, timeout);
        }

        public IntPtr OpenDialogWithDteExecuteCommand(string commandName, string commandArgs = "") {
            return App.OpenDialogWithDteExecuteCommand(commandName, commandArgs);
        }

        public Project GetProject(string projectName) {
            return App.GetProject(projectName);
        }

        public void SelectProject(Project project) {
            SolutionExplorer.SelectProject(project);
        }

        public IEditor GetDocument(string filename) {
            return App.GetDocument(filename);
        }

        public IAddExistingItem AddExistingItem() {
            return AddExistingItemDialog.FromDte(App);
        }

        public IAddNewItem AddNewItem() {
            return NewItemDialog.FromDte(App);
        }

        public IOverwriteFile WaitForOverwriteFileDialog() {
            return OverwriteFileDialog.Wait(App);
        }

        public void WaitForMode(dbgDebugMode dbgDebugMode) {
            App.WaitForMode(dbgDebugMode);
        }

        public List<IVsTaskItem> WaitForErrorListItems(int expectedItems) {
            return App.WaitForErrorListItems(expectedItems);
        }

        public DTE Dte {
            get { return App.Dte; }
        }

        public void OnDispose(Action action) {
            App.OnDispose(action);
        }
    }
}
