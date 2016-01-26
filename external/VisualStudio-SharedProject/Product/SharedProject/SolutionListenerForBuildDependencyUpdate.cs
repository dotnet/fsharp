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

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using IServiceProvider = System.IServiceProvider;

namespace Microsoft.VisualStudioTools.Project {
    /// <summary>
    /// The purpose of this class is to set a build dependency from a modeling project to all its sub projects
    /// </summary>
    class SolutionListenerForBuildDependencyUpdate : SolutionListener {
        #region ctors
        public SolutionListenerForBuildDependencyUpdate(IServiceProvider serviceProvider)
            : base(serviceProvider) {
        }
        #endregion

        #region overridden methods
        /// <summary>
        /// Update build dependency list if solution is fully loaded
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="added"></param>
        /// <returns></returns>
        public override int OnAfterOpenProject(IVsHierarchy hierarchy, int added) {
            // Return from here if we are at load time
            if (added == 0) {
                return VSConstants.S_OK;
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called at load time when solution has finished opening.
        /// </summary>
        /// <param name="pUnkReserved">reserved</param>
        /// <param name="fNewSolution">true if this is a new solution</param>
        /// <returns></returns>
        public override int OnAfterOpenSolution(object pUnkReserved, int fNewSolution) {
            return VSConstants.S_OK;
        }
        #endregion

    }
}
