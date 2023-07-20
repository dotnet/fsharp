// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Threading;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.Extensibility.Testing;

internal partial class SolutionExplorerInProcess
{
    public async Task OpenFileAsync(string projectName, string relativeFilePath, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var filePath = await GetAbsolutePathForProjectRelativeFilePathAsync(projectName, relativeFilePath, cancellationToken);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(filePath);
        }

        VsShellUtilities.OpenDocument(ServiceProvider.GlobalProvider, filePath, VSConstants.LOGVIEWID.Code_guid, out _, out _, out _, out var view);

        // Reliably set focus using NavigateToLineAndColumn
        var textManager = await GetRequiredGlobalServiceAsync<SVsTextManager, IVsTextManager>(cancellationToken);
        ErrorHandler.ThrowOnFailure(view.GetBuffer(out var textLines));
        ErrorHandler.ThrowOnFailure(view.GetCaretPos(out var line, out var column));
        ErrorHandler.ThrowOnFailure(textManager.NavigateToLineAndColumn(textLines, VSConstants.LOGVIEWID.Code_guid, line, column, line, column));
    }

    public async Task AddFileAsync(string projectName, string fileName, string contents, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var project = await GetProjectAsync(projectName, cancellationToken);
        var projectDirectory = Path.GetDirectoryName(project.FullName);
        var filePath = Path.Combine(projectDirectory, fileName);
        var directoryPath = Path.GetDirectoryName(filePath);
        
        Directory.CreateDirectory(directoryPath);
        File.WriteAllText(filePath, contents);
        
        _ = project.ProjectItems.AddFromFile(filePath);
    }

    public async Task RenameFileAsync(string projectName, string oldFileName, string newFileName, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        var projectItem = await GetProjectItemAsync(projectName, oldFileName, cancellationToken);

        projectItem.Name = newFileName;
    }

    private async Task<string> GetAbsolutePathForProjectRelativeFilePathAsync(string projectName, string relativeFilePath, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var dte = await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken);
        var solution = dte.Solution;

        var project = solution.Projects.Cast<EnvDTE.Project>().First(x => x.Name == projectName);
        var projectPath = Path.GetDirectoryName(project.FullName);
        return Path.Combine(projectPath, relativeFilePath);
    }

    private async Task<EnvDTE.ProjectItem> GetProjectItemAsync(string projectName, string relativeFilePath, CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

        var solution = (await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken)).Solution;
        var projects = solution.Projects.Cast<EnvDTE.Project>();
        var project = projects.FirstOrDefault(x => x.Name == projectName);
        var projectPath = Path.GetDirectoryName(project.FullName);
        var fullFilePath = Path.Combine(projectPath, relativeFilePath);
        var projectItems = project.ProjectItems.Cast<EnvDTE.ProjectItem>();
        var document = projectItems.FirstOrDefault(d => d.get_FileNames(1).Equals(fullFilePath));

        return document;
    }
}
