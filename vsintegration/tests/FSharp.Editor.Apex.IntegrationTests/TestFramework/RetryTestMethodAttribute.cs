// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FSharp.Editor.Apex.IntegrationTests.TestFramework
{
    /// <summary>
    /// Drop-in replacement for <see cref="TestMethodAttribute"/> that re-runs a test up to
    /// <c>maxAttempts</c> times, passing as soon as one attempt passes. These Apex tests drive a real
    /// Visual Studio through UI automation and are occasionally flaky (focus, background analysis and
    /// build/restore timing); a bounded retry keeps them reliable without masking a genuine failure —
    /// every attempt has to fail before the test is reported as failed.
    ///
    /// Each attempt goes through the full per-test pipeline (a fresh test-class instance plus
    /// TestInitialize/TestCleanup, i.e. a fresh VS session for the Apex host), so a retry starts from a
    /// clean state rather than reusing the dirtied state of the previous attempt.
    ///
    /// MSTest's own <c>[Retry]</c> attribute is only available from 3.8; this repo pins 3.6.4, so the
    /// classic "derive from TestMethodAttribute and override Execute" idiom is used instead.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class RetryTestMethodAttribute : TestMethodAttribute
    {
        private readonly int maxAttempts;

        public RetryTestMethodAttribute(int maxAttempts = 2)
        {
            if (maxAttempts < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxAttempts), maxAttempts, "At least one attempt is required.");
            }

            this.maxAttempts = maxAttempts;
        }

        public override TestResult[] Execute(ITestMethod testMethod)
        {
            TestResult[] results = null;

            for (var attempt = 1; attempt <= this.maxAttempts; attempt++)
            {
                results = base.Execute(testMethod);

                if (results.All(result => result.Outcome == UnitTestOutcome.Passed))
                {
                    return results;
                }

                if (attempt < this.maxAttempts)
                {
                    Trace.WriteLine(
                        $"[RetryTestMethod] '{testMethod.TestMethodName}' failed on attempt {attempt} of {this.maxAttempts}; retrying.");
                }
            }

            return results;
        }
    }
}
