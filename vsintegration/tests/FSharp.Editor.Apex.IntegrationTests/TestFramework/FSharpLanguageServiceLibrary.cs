//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

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
            return new TextDocumentView(window.Editor);
        }
    }
}
