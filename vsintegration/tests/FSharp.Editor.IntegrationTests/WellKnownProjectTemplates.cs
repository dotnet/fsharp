// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace FSharp.Editor.IntegrationTests;

// SDK short names consumed by `dotnet new` in SolutionExplorerInProcess.AddProjectAsync.
// The previous values (Microsoft.FSharp.NETCore.*) referenced VS template IDs that never
// existed in any shipping template package -- the tests have never resolved them.
internal static class WellKnownProjectTemplates
{
    public const string FSharpNetCoreClassLibrary = "classlib";
    public const string FSharpNetCoreConsoleApplication = "console";
    public const string FSharpNetCoreXUnitTest = "xunit";
}
