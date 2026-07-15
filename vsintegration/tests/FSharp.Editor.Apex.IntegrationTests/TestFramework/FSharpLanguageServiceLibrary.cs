//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.Test.Apex;
using Microsoft.Test.Apex.Services;
using Microsoft.Test.Apex.VisualStudio;
using Microsoft.Test.Apex.VisualStudio.Editor;
using Microsoft.Test.Apex.VisualStudio.Shell;
using Microsoft.Test.Apex.VisualStudio.Solution;

namespace FSharp.Editor.Apex.IntegrationTests.TestFramework
{
    /// <summary>
    /// Compact test library for the F# Apex integration tests. Provides the small facade over the
    /// Apex <see cref="VisualStudioHost"/> that the code-action tests use (project creation,
    /// synchronization, the active editor and document opening), tailored for the F# extension.
    /// </summary>
    public sealed class FSharpLanguageServiceLibrary
    {
        public FSharpLanguageServiceLibrary(VisualStudioHost visualStudio, IOperations operations)
        {
            this.VisualStudio = visualStudio;
            this.Synchronization = new SynchronizationHelper(visualStudio, operations.Get<ISynchronizationService>());
            this.ProjectCreation = new ProjectCreationHelper(visualStudio);
        }

        /// <summary>The Apex Visual Studio host.</summary>
        public VisualStudioHost VisualStudio { get; }

        /// <summary>Synchronization helpers used to wait for background work.</summary>
        public SynchronizationHelper Synchronization { get; }

        /// <summary>F# project and project-item creation helpers.</summary>
        public ProjectCreationHelper ProjectCreation { get; }

        /// <summary>The editor of the active document window, or null if none is active.</summary>
        public IVisualStudioTextEditorTestExtension Editor
            => this.VisualStudio.ObjectModel.WindowManager.ActiveDocumentWindowAsTextEditor?.Editor;

        /// <summary>
        /// Opens the given project item in the text editor and returns a view over it.
        /// </summary>
        public TextDocumentView OpenDocument(ProjectItemTestExtension documentItem)
        {
            var window = documentItem.Open<TextEditorDocumentWindowTestExtension>();
            return new TextDocumentView(window);
        }

        /// <summary>
        /// Finds <paramref name="fileName"/> under <paramref name="project"/>, opens it in the text
        /// editor and returns a view over it. The project's item tree is populated lazily, so the lookup
        /// is polled (TryFindChild is single-shot); on timeout it throws listing the project's actual
        /// top-level item names, which makes a template-filename surprise obvious.
        /// </summary>
        public TextDocumentView OpenProjectFile(ProjectTestExtension project, string fileName)
        {
            ProjectItemTestExtension item = null;
            this.Synchronization.TryWaitForCondition(
                () => (item = project.TryFindChild<ProjectItemTestExtension>(fileName, true)) != null);

            if (item == null)
            {
                var actualNames = string.Join(", ", project.ProjectItems.Select(child => $"'{child.Name}'"));
                throw new InvalidOperationException(
                    $"Project item '{fileName}' was not found in project '{project.Name}'. Items present: {actualNames}.");
            }

            return this.OpenDocument(item);
        }

        /// <summary>The short caption (file name) of the active document window, or null if none is active.</summary>
        public string ActiveDocumentCaption
            => this.VisualStudio.ObjectModel.WindowManager.ActiveDocumentWindow?.Caption;

        /// <summary>The text of the caret's current line in the active document editor, or null if none is active.</summary>
        public string ActiveDocumentCurrentLineText
            => this.Editor?.Caret.GetCurrentLineText();
    }
}
