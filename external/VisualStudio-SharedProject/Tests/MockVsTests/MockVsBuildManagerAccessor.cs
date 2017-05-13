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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Execution;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using MSBuild = Microsoft.Build.Evaluation;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockVsBuildManagerAccessor : IVsBuildManagerAccessor {
        public int BeginDesignTimeBuild() {
            BuildParameters buildParameters = new BuildParameters(MSBuild.ProjectCollection.GlobalProjectCollection);
            BuildManager.DefaultBuildManager.BeginBuild(buildParameters);
            return VSConstants.S_OK;
        }

        public int ClaimUIThreadForBuild() {
            return VSConstants.S_OK;
        }

        public int EndDesignTimeBuild() {
            BuildManager.DefaultBuildManager.EndBuild();
            return VSConstants.S_OK;
        }

        public int Escape(string pwszUnescapedValue, out string pbstrEscapedValue) {
            throw new NotImplementedException();
        }

        public int GetCurrentBatchBuildId(out uint pBatchId) {
            throw new NotImplementedException();
        }

        public int GetSolutionConfiguration(object punkRootProject, out string pbstrXmlFragment) {
            throw new NotImplementedException();
        }

        public int RegisterLogger(int submissionId, object punkLogger) {
            return VSConstants.S_OK;
        }

        public int ReleaseUIThreadForBuild() {
            return VSConstants.S_OK;
        }

        public int Unescape(string pwszEscapedValue, out string pbstrUnescapedValue) {
            throw new NotImplementedException();
        }

        public int UnregisterLoggers(int submissionId) {
            return VSConstants.S_OK;
        }
    }
}
