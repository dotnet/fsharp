// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Test.Apex;
using Microsoft.Test.Apex.VisualStudio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSharp.Compiler.LanguageServer.IntegrationTests;

/// <summary>
/// Base class for F# LSP integration tests that run inside Visual Studio.
/// Uses Apex to launch VS experimental instance with the F# extension deployed.
/// </summary>
[TestClass]
public class FSharpApexTestBase : ApexTest
{
    private static readonly VisualStudioHostConfiguration s_configuration = CreateConfiguration();
    internal static VisualStudioHost VsHost = default!;

    private static VisualStudioHostConfiguration CreateConfiguration()
    {
        return new VisualStudioHostConfiguration
        {
            RootSuffix = "Exp",
            SkipCodeMarkerInitialization = true,
            EnableUnifiedSettings = true,
            // Automatically dismiss unexpected dialogs (e.g., the file
            // recovery dialog that appears after a crash or forced kill).
            AutomaticallyDismissMessageBoxes = true,
        };
    }

    [TestInitialize]
    public void Initialize()
    {
        if (VsHost?.IsRunning != true)
        {
            // Previous host may be a zombie (crashed but not disposed).
            // Dispose it before creating a new one.
            try { VsHost?.Stop(); } catch { /* best effort cleanup */ }
            VsHost = Operations.CreateHost<VisualStudioHost>(s_configuration);
            VsHost.Start();
        }
    }

    [TestCleanup]
    public override void TestCleanup()
    {
        try
        {
            if (VsHost?.IsRunning == true)
            {
                if (VsHost.ObjectModel.Solution?.IsOpen == true)
                {
                    // SaveAndClose avoids leaving unsaved recovery files that
                    // trigger a file-recovery dialog on the next VS launch.
                    VsHost.ObjectModel.Solution.SaveAndClose();
                }
            }
        }
        catch (Exception)
        {
            // VS may have crashed — mark the host as dead so Initialize
            // creates a fresh instance for the next test.
            VsHost = null!;
        }

        base.TestCleanup();
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        if (VsHost?.IsRunning == true)
        {
            VsHost?.Stop();
        }
    }
}
