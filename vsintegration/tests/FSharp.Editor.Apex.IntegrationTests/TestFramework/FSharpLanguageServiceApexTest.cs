//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using Microsoft.Test.Apex.Editor;
using Microsoft.Test.Apex.VisualStudio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSharp.Editor.Apex.IntegrationTests.TestFramework
{
    /// <summary>
    /// Base test class for F# language service tests built using the Apex framework.
    /// Compact, self-contained equivalent of the TypeScript-VS LanguageServiceApexTest, tailored for
    /// the F# extension and without the JavaScript/TypeScript BrowserTools layer.
    /// </summary>
    [TestClass]
    public abstract class FSharpLanguageServiceApexTest : VisualStudioHostTest
    {
        protected const int ShortTimeoutMS = 1000 * 60 * 12;
        protected const int LongTimeoutMS = 1000 * 60 * 20;

        // Apex launches the Visual Studio instance pointed to by this environment variable, if set.
        private const string InstallationUnderTestPathVariable = "VisualStudio.InstallationUnderTest.Path";

        private FSharpLanguageServiceLibrary libraryCache;

        /// <summary>
        /// Gets the F# language service test library.
        /// </summary>
        public FSharpLanguageServiceLibrary Library
            => this.libraryCache ??= new FSharpLanguageServiceLibrary(this.VisualStudio, this.Operations);

        /// <summary>
        /// Whether message boxes shown during the test run should be dismissed automatically.
        /// </summary>
        protected virtual bool AutomaticallyDismissMessageBoxes => true;

        /// <summary>
        /// The registry root suffix (experimental hive) of the Visual Studio instance to launch.
        /// Defaults to "RoslynDev", where the locally-built F# extension is deployed.
        /// </summary>
        protected virtual string RootSuffix => "RoslynDev";

        /// <summary>
        /// The product milestone of the installed Visual Studio to launch, as reported by vswhere's
        /// catalog.productMilestone. Defaults to "Canary" (the IntCanary channel), which selects the
        /// Canary install rather than Insiders when both are present locally. On CI the installed VS is
        /// a different channel (e.g. "Preview"), so this can be overridden with the
        /// FSHARP_APEX_VS_MILESTONE environment variable; setting VisualStudio.InstallationUnderTest.Path
        /// directly takes precedence over milestone resolution entirely.
        /// </summary>
        protected virtual string TargetProductMilestone
            => Environment.GetEnvironmentVariable("FSHARP_APEX_VS_MILESTONE") is string milestone
               && !string.IsNullOrEmpty(milestone)
                ? milestone
                : "Canary";

        protected override VisualStudioHostConfiguration GetVisualStudioHostConfiguration()
        {
            this.EnsureTargetInstallationSelected();

            var config = base.GetVisualStudioHostConfiguration();
            config.AutomaticallyDismissMessageBoxes = this.AutomaticallyDismissMessageBoxes;
            config.RootSuffix = this.RootSuffix;
            return config;
        }

        /// <summary>
        /// Points Apex at the Visual Studio install matching <see cref="TargetProductMilestone"/>
        /// (e.g. Canary). An explicit VisualStudio.InstallationUnderTest.Path override is respected.
        /// </summary>
        private void EnsureTargetInstallationSelected()
        {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(InstallationUnderTestPathVariable)))
            {
                return;
            }

            string devenvPath = ResolveDevenvByMilestone(this.TargetProductMilestone);
            if (!string.IsNullOrEmpty(devenvPath))
            {
                Environment.SetEnvironmentVariable(InstallationUnderTestPathVariable, devenvPath);
            }
        }

        private static string ResolveDevenvByMilestone(string milestone)
        {
            string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            string vswhere = Path.Combine(programFilesX86, "Microsoft Visual Studio", "Installer", "vswhere.exe");
            if (!File.Exists(vswhere))
            {
                return null;
            }

            var startInfo = new ProcessStartInfo(vswhere, "-all -prerelease -format json -utf8")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            string json;
            using (var process = Process.Start(startInfo))
            {
                json = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }

            if (string.IsNullOrWhiteSpace(json)
                || !(new JavaScriptSerializer { MaxJsonLength = int.MaxValue }.DeserializeObject(json) is object[] installations))
            {
                return null;
            }

            foreach (var installation in installations.OfType<Dictionary<string, object>>())
            {
                if (installation.TryGetValue("catalog", out var catalogObject)
                    && catalogObject is Dictionary<string, object> catalog
                    && catalog.TryGetValue("productMilestone", out var installedMilestone)
                    && string.Equals(installedMilestone as string, milestone, StringComparison.OrdinalIgnoreCase)
                    && installation.TryGetValue("productPath", out var productPath))
                {
                    return productPath as string;
                }
            }

            return null;
        }

        /// <summary>
        /// Moves the caret onto <paramref name="expression"/>, triggers the code-fix request and
        /// expands the resulting light bulb.
        /// </summary>
        protected ILightBulbTestExtension MoveToExpressionAndExpandLightBulb(TextDocumentView document, string expression)
        {
            // Nudging the caret into the diagnostic range triggers a request for code fixes.
            document.MoveToExpression(expression);
            document.MoveRight();
            document.MoveLeft();

            this.Library.Synchronization.WaitForLightBulb();

            Assert.IsTrue(
                this.Library.Editor.LightBulb.Verify.IsLightBulbPresent(TimeSpan.FromSeconds(60)),
                $"Expected a light bulb to be present at expression '{expression}'.");

            var lightBulb = this.Library.Editor.LightBulb.GetActiveLightBulb();
            lightBulb.Expand();

            // Wait for the suggested actions to populate. Poll the already-expanded session directly:
            // re-querying GetActiveLightBulb() here returns null once the light bulb is expanded into
            // its flyout (LightBulbBroker no longer reports it as the active session), and the flyout
            // taking focus can also null out Library.Editor (ActiveDocumentWindowAsTextEditor) — either
            // of which previously threw a NullReferenceException inside the poll. LightBulbTestExtension
            // .Actions reads the captured session and returns an empty (never null) set while pending.
            this.Library.Synchronization.TryWaitForCondition(() => lightBulb.Actions.Any());

            return lightBulb;
        }

        /// <summary>
        /// Creates a fresh single-project F# solution whose Library.fs contains <paramref name="code"/>
        /// and opens it in the editor. Waits for the project to finish loading before editing (so the
        /// F# project system does not reload the empty file over the insert) and confirms
        /// <paramref name="expectedContent"/> reached the buffer before returning, so downstream caret
        /// searches are not racing document initialization.
        /// </summary>
        protected TextDocumentView OpenFSharpDocument(string code, string expectedContent)
        {
            var project = this.Library.ProjectCreation.CreateFSharpLibrary();
            var documentItem = this.Library.ProjectCreation.AddProjectItemFromEmptyFile(project, "Library.fs");
            var document = this.Library.OpenDocument(documentItem);

            this.Library.Synchronization.WaitForSolutionCrawler();

            document.InsertText(code);

            Assert.IsTrue(
                this.Library.Synchronization.TryWaitForCondition(
                    () => document.Contents.Contains(expectedContent), TimeSpan.FromSeconds(15)),
                $"Inserted source did not appear in the document buffer (looking for '{expectedContent}').");

            return document;
        }

        /// <summary>
        /// Builds the current solution and waits for it to finish. Building performs a NuGet restore and
        /// resolves references — which the F# language service needs before it can type-check and surface
        /// SEMANTIC diagnostics (e.g. the unused-open analyzer, or FS0760 for disposables). This is the
        /// Apex counterpart of the source IdeFact tests' RestoreNuGetPackagesAsync + WaitForProjectSystem.
        /// Parse-based fixes (FS0597, FS0010) don't need it. Build success is not asserted: some fixture
        /// sources intentionally contain errors, and a failed compile still restores references.
        /// </summary>
        protected void BuildSolution()
        {
            this.Library.VisualStudio.ObjectModel.Solution.BuildManager.Build(waitForBuildToFinish: true);
            this.Library.Synchronization.WaitForSolutionCrawler();
        }

        /// <summary>
        /// Builds the current solution and returns whether it succeeded.
        /// </summary>
        protected bool BuildSolutionSucceeded()
        {
            this.BuildSolution();
            return this.Library.VisualStudio.ObjectModel.Solution.BuildManager.Succeeded;
        }

        /// <summary>
        /// Places the caret on <paramref name="expression"/>, focuses the editor and invokes Go To
        /// Definition, then waits for navigation to settle. The result is read from the active document
        /// afterwards (which may be the same or a different file).
        /// </summary>
        protected void GoToDefinition(TextDocumentView document, string expression)
        {
            document.MoveToExpression(expression);
            document.Focus();
            document.GoToDefinition();
            this.Library.Synchronization.WaitForSolutionCrawler();
        }

        /// <summary>
        /// Invokes Go To Definition on <paramref name="expression"/> and polls until the active document
        /// settles on a line matching <paramref name="lineMatches"/> and, when given, a window caption
        /// equal to <paramref name="expectedCaption"/>. Go To Definition navigates asynchronously (it may
        /// open/activate another document and only then move the caret), so — exactly like the light-bulb
        /// helper waits for its actions — we poll for the end state instead of reading it immediately.
        /// Reading it immediately is the fire-and-assert race that makes the ported navigation tests flaky.
        /// </summary>
        protected void GoToDefinitionAndWait(
            TextDocumentView document,
            string expression,
            Func<string, bool> lineMatches,
            string expectedCaption = null)
        {
            this.GoToDefinition(document, expression);

            string lastLine = null;
            string lastCaption = null;
            bool landed = this.Library.Synchronization.TryWaitForCondition(
                () =>
                {
                    lastLine = this.Library.ActiveDocumentCurrentLineText;
                    lastCaption = this.Library.ActiveDocumentCaption;
                    return lastLine != null
                        && lineMatches(lastLine)
                        && (expectedCaption == null || string.Equals(lastCaption, expectedCaption, StringComparison.Ordinal));
                },
                TimeSpan.FromSeconds(30));

            Assert.IsTrue(
                landed,
                $"Go To Definition on '{expression}' did not settle on the expected location. "
                + (expectedCaption != null ? $"Expected caption '{expectedCaption}', actual '{lastCaption}'. " : string.Empty)
                + $"Actual current line: '{lastLine}'.");
        }

        /// <summary>
        /// Asserts two pieces of F# source are equal, ignoring line-ending style and any trailing
        /// newline. Template output and editor buffers differ in those incidental ways across SDKs, so
        /// normalizing avoids brittle failures while still comparing the meaningful content exactly.
        /// </summary>
        protected static void AssertSourceEquals(string expected, string actual)
        {
            Assert.AreEqual(NormalizeSource(expected), NormalizeSource(actual));
        }

        /// <summary>
        /// Normalizes F# source for comparison by unifying line-ending style and trimming any trailing
        /// newline, so comparisons ignore those incidental differences.
        /// </summary>
        protected static string NormalizeSource(string source)
            => (source ?? string.Empty).Replace("\r\n", "\n").Replace("\r", "\n").TrimEnd('\n');

        /// <summary>
        /// Verifies that placing the caret on <paramref name="caretExpression"/> in the given F# source
        /// offers exactly one light-bulb action whose text contains <paramref name="expectedActionText"/>.
        /// Mirrors the display-text assertions in FSharp.Editor.IntegrationTests CodeActionTests (the Apex
        /// light-bulb model exposes a flat action list, so the code-fix/error-fix category is not checked).
        /// </summary>
        protected void AssertCodeActionOffered(string code, string caretExpression, string expectedActionText)
        {
            var document = this.OpenFSharpDocument(code, caretExpression);

            // Restore/resolve references so the F# language service can type-check and offer semantic
            // fixes (the unused-open analyzer, FS0760, ...); without this their light bulb never appears.
            this.BuildSolution();

            var lightBulb = this.MoveToExpressionAndExpandLightBulb(document, caretExpression);

            var matchCount = lightBulb.Actions.Count(action =>
                action.Text.IndexOf(expectedActionText, StringComparison.OrdinalIgnoreCase) >= 0);

            Assert.AreEqual(
                1,
                matchCount,
                $"Expected exactly one code action containing '{expectedActionText}'. Offered actions: " +
                string.Join(", ", lightBulb.Actions.Select(action => $"'{action.Text}'")));
        }
    }
}
