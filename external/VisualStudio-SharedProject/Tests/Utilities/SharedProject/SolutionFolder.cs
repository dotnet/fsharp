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
using System.IO;
using TestUtilities.SharedProject;
using MSBuild = Microsoft.Build.Evaluation;

namespace TestUtilities {
    public sealed class SolutionFolder : ISolutionElement {
        private readonly string _name;
        private static Guid _solutionFolderGuid = new Guid("2150E333-8FDC-42A3-9474-1A3956D46DE8");
        
        public SolutionFolder(string name) {
            _name = name;
        }
        public MSBuild.Project Save(MSBuild.ProjectCollection collection, string location) {
            Directory.CreateDirectory(Path.Combine(location, _name));
            return null;
        }

        public Guid TypeGuid {
            get { return _solutionFolderGuid; }
        }

        public SolutionElementFlags Flags {
            get { return SolutionElementFlags.ExcludeFromConfiguration; }
        }

        public string Name { get { return _name; } }
    }
}
