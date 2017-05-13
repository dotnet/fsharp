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

namespace TestUtilities.SharedProject {
    /// <summary>
    /// Represents a project property for the startup file in a script
    /// based project system.  When generated the code extension is automatically
    /// appended.
    /// </summary>
    public sealed class StartupFileProjectProperty : ProjectProperty {
        public StartupFileProjectProperty(string filename)
            : base("StartupFile", filename) {
        }

        public override void Generate(ProjectType projectType, Microsoft.Build.Evaluation.Project project) {
            project.SetProperty(Name, Value + projectType.CodeExtension);
        }
    }
}
