// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Extensibility.Testing;

internal partial class ShellInProcess
{
    public async Task ClearAllBreakpointsAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        var dte = await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken);
        var bps = new List<EnvDTE.Breakpoint>();
        foreach (EnvDTE.Breakpoint bp in dte.Debugger.Breakpoints) bps.Add(bp);
        foreach (var bp in bps) bp.Delete();
    }

    public async Task StartDebuggingAsync(CancellationToken cancellationToken)
        => await ExecuteCommandAsync("Debug.Start", cancellationToken);

    public async Task ContinueDebuggingAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        var dte = await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken);
        dte.Debugger.Go(true);
    }

    public async Task StopDebuggingAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        var dte = await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken);
        if (dte.Debugger.CurrentMode != EnvDTE.dbgDebugMode.dbgDesignMode)
        {
            dte.Debugger.Stop(true);
            await WaitForDesignModeAsync(TimeSpan.FromSeconds(30), cancellationToken);
        }
    }

    public async Task WaitForDesignModeAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            var dte = await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken);
            if (dte.Debugger.CurrentMode == EnvDTE.dbgDebugMode.dbgDesignMode) return;
            if (sw.Elapsed > timeout) throw new TimeoutException($"Not in design mode after {timeout}.");
            await Task.Delay(100, cancellationToken);
        }
    }

    public async Task WaitForBreakpointHitAsync(TimeSpan timeout, bool continueOnStepBreak, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        var seenRun = false;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            var dte = await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken);
            var dbg = dte.Debugger;
            var mode = dbg.CurrentMode;

            if (mode == EnvDTE.dbgDebugMode.dbgRunMode) seenRun = true;

            if (mode == EnvDTE.dbgDebugMode.dbgBreakMode && dbg.BreakpointLastHit is not null)
                return;

            // A step-break (no breakpoint) during startup — resume automatically.
            if (continueOnStepBreak
                && mode == EnvDTE.dbgDebugMode.dbgBreakMode
                && dbg.BreakpointLastHit is null
                && dbg.LastBreakReason == EnvDTE.dbgEventReason.dbgEventReasonStep)
                dbg.Go(true);

            if (seenRun && mode == EnvDTE.dbgDebugMode.dbgDesignMode)
                throw new InvalidOperationException("Debug session ended before breakpoint hit.");

            if (sw.Elapsed > timeout)
                throw new TimeoutException($"No breakpoint hit after {timeout}. Mode={mode}; Breakpoints={BreakpointSummary(dbg)}.");

            await Task.Delay(100, cancellationToken);
        }
    }

    public async Task<int> GetLastHitBreakpointLineAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        var dte = await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken);
        return dte.Debugger.BreakpointLastHit?.FileLine
            ?? throw new InvalidOperationException("No breakpoint was hit.");
    }

    public async Task<int[]> GetConfiguredBreakpointLinesAsync(CancellationToken cancellationToken)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        var dte = await GetRequiredGlobalServiceAsync<SDTE, EnvDTE.DTE>(cancellationToken);
        var lines = new List<int>();
        foreach (EnvDTE.Breakpoint bp in dte.Debugger.Breakpoints)
        {
            var hasChild = false;
            foreach (EnvDTE.Breakpoint child in bp.Children) { hasChild = true; lines.Add(child.FileLine); }
            if (!hasChild) lines.Add(bp.FileLine);
        }
        return lines.ToArray();
    }

    private static string BreakpointSummary(EnvDTE.Debugger dbg)
    {
        var items = new List<string>();
        foreach (EnvDTE.Breakpoint bp in dbg.Breakpoints)
        {
            var children = 0;
            foreach (EnvDTE.Breakpoint _ in bp.Children) children++;
            items.Add($"{Path.GetFileName(bp.File)}:{bp.FileLine}(children={children})");
        }
        return items.Count == 0 ? "none" : string.Join(",", items);
    }
}
