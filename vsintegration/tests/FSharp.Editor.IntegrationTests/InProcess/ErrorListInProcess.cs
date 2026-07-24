// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Extensibility.Testing;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.TableControl;
using Microsoft.VisualStudio.Shell.TableManager;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.CodeAnalysis.Testing.InProcess
{
    [TestService]
    internal partial class ErrorListInProcess
    {
        public async Task ShowBuildErrorsAsync(CancellationToken cancellationToken)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var errorList = await GetRequiredGlobalServiceAsync<SVsErrorList, IErrorList>(cancellationToken);
            errorList.AreBuildErrorSourceEntriesShown = true;
            errorList.AreOtherErrorSourceEntriesShown = false;
            errorList.AreErrorsShown = true;
            errorList.AreMessagesShown = true;
            errorList.AreWarningsShown = false;
        }

        // Shows EVERY entry: all sources (Build + Other/IntelliSense) and all severities. F# live diagnostics are
        // "Other"-source, and ShowBuildErrorsAsync (used by BuildProjectTests) leaves the shared Error List
        // filtered to Build-only + no-warnings; because the whole test collection shares one VS instance, that
        // stale filter otherwise HIDES the F# code-action diagnostics from GetErrorItems/GetAllEntries, making the
        // produce-signal read zero even when diagnostics were published. Call this before reading the Error List.
        public async Task ShowAllEntriesAsync(CancellationToken cancellationToken)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var errorList = await GetRequiredGlobalServiceAsync<SVsErrorList, IErrorList>(cancellationToken);
            errorList.AreBuildErrorSourceEntriesShown = true;
            errorList.AreOtherErrorSourceEntriesShown = true;
            errorList.AreErrorsShown = true;
            errorList.AreWarningsShown = true;
            errorList.AreMessagesShown = true;
        }

        public Task<ImmutableArray<string>> GetBuildErrorsAsync(CancellationToken cancellationToken)
        {
            return GetBuildErrorsAsync(__VSERRORCATEGORY.EC_WARNING, cancellationToken);
        }

        public async Task<ImmutableArray<string>> GetBuildErrorsAsync(__VSERRORCATEGORY minimumSeverity, CancellationToken cancellationToken)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var errorItems = await GetErrorItemsAsync(cancellationToken);
            var list = new List<string>();

            foreach (var item in errorItems)
            {
                if (item.GetCategory() > minimumSeverity)
                {
                    continue;
                }

                if (!item.TryGetValue(StandardTableKeyNames.ErrorSource, out var errorSource)
                    || (ErrorSource)errorSource != ErrorSource.Build)
                {
                    continue;
                }

                var source = item.GetBuildTool();
                var document = Path.GetFileName(item.GetPath() ?? item.GetDocumentName()) ?? "<unknown>";
                var line = item.GetLine() ?? -1;
                var column = item.GetColumn() ?? -1;
                var errorCode = item.GetErrorCode() ?? "<unknown>";
                var text = item.GetText() ?? "<unknown>";
                var severity = item.GetCategory() switch
                {
                    __VSERRORCATEGORY.EC_ERROR => "error",
                    __VSERRORCATEGORY.EC_WARNING => "warning",
                    __VSERRORCATEGORY.EC_MESSAGE => "info",
                    var unknown => unknown.ToString(),
                };

                var message = $"({source}) {document}({line + 1}, {column + 1}): {severity} {errorCode}: {text}";
                list.Add(message);
            }

            return list
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ThenBy(x => x, StringComparer.Ordinal)
                .ToImmutableArray();
        }

        public Task<int> GetErrorCountAsync(CancellationToken cancellationToken)
        {
            return GetErrorCountAsync(__VSERRORCATEGORY.EC_WARNING, cancellationToken);
        }

        public async Task<int> GetErrorCountAsync(__VSERRORCATEGORY minimumSeverity, CancellationToken cancellationToken)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var errorItems = await GetErrorItemsAsync(cancellationToken);
            return errorItems.Count(e => e.GetCategory() <= minimumSeverity);
        }

        // Diagnostic instrumentation: dumps every error-list entry (all sources and severities) so a failing
        // test can report whether a given diagnostic (e.g. unused-opens) was actually published headless.
        public async Task<ImmutableArray<string>> GetAllEntriesAsync(CancellationToken cancellationToken)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var errorItems = await GetErrorItemsAsync(cancellationToken);
            var list = new List<string>();

            foreach (var item in errorItems)
            {
                var source = item.TryGetValue(StandardTableKeyNames.ErrorSource, out ErrorSource errorSource)
                    ? errorSource.ToString()
                    : "<no-source>";
                var tool = item.GetBuildTool();
                var document = Path.GetFileName(item.GetPath() ?? item.GetDocumentName()) ?? "<unknown>";
                var line = item.GetLine() ?? -1;
                var code = item.GetErrorCode() ?? "<none>";
                var text = item.GetText() ?? "<none>";

                list.Add($"[{item.GetCategory()}] source={source} tool={tool} {document}({line + 1}) {code}: {text}");
            }

            return list.ToImmutableArray();
        }

        private async Task<ImmutableArray<ITableEntryHandle>> GetErrorItemsAsync(CancellationToken cancellationToken)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var errorList = await GetRequiredGlobalServiceAsync<SVsErrorList, IErrorList>(cancellationToken);
            var args = await errorList.TableControl.ForceUpdateAsync();
            return args.AllEntries.ToImmutableArray();
        }
    }
}
