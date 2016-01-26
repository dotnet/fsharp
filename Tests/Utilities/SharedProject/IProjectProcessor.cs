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

using MSBuild = Microsoft.Build.Evaluation;

namespace TestUtilities.SharedProject {
    /// <summary>
    /// Updates the generated file before and/or after the project file is generated.
    /// 
    /// This can insert extra data into the project which is required for proper functioning
    /// of the project system.
    /// 
    /// Classes implementing this interface should be exported with a ProjectExtensionAttribute
    /// specifying which project type the processor applies to.
    /// </summary>
    public interface IProjectProcessor {
        /// <summary>
        /// Runs before any test case defined content is added to the project.
        /// 
        /// This should be used to setup must haves for your project system.  Individual
        /// test cases may override your defaults here as appropriate.
        /// </summary>
        void PreProcess(MSBuild.Project project);

        /// <summary>
        /// Runs after all test case defined content is added to the project.
        /// 
        /// This allows any post generation fixups which might be necessary for the project
        /// system.
        /// </summary>
        void PostProcess(MSBuild.Project project);
    }
}
