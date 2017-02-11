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

using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.Project {

    public class EnumDependencies : IVsEnumDependencies {
        private List<IVsDependency> dependencyList = new List<IVsDependency>();

        private uint nextIndex;

        public EnumDependencies(IList<IVsDependency> dependencyList) {
            Utilities.ArgumentNotNull("dependencyList", dependencyList);

            foreach (IVsDependency dependency in dependencyList) {
                this.dependencyList.Add(dependency);
            }
        }

        public EnumDependencies(IList<IVsBuildDependency> dependencyList) {
            Utilities.ArgumentNotNull("dependencyList", dependencyList);

            foreach (IVsBuildDependency dependency in dependencyList) {
                this.dependencyList.Add(dependency);
            }
        }

        public int Clone(out IVsEnumDependencies enumDependencies) {
            enumDependencies = new EnumDependencies(this.dependencyList);
            ErrorHandler.ThrowOnFailure(enumDependencies.Skip(this.nextIndex));
            return VSConstants.S_OK;
        }

        public int Next(uint elements, IVsDependency[] dependencies, out uint elementsFetched) {
            elementsFetched = 0;
            Utilities.ArgumentNotNull("dependencies", dependencies);

            uint fetched = 0;
            int count = this.dependencyList.Count;

            while (this.nextIndex < count && elements > 0 && fetched < count) {
                dependencies[fetched] = this.dependencyList[(int)this.nextIndex];
                this.nextIndex++;
                fetched++;
                elements--;

            }

            elementsFetched = fetched;

            // Did we get 'em all?
            return (elements == 0 ? VSConstants.S_OK : VSConstants.S_FALSE);
        }

        public int Reset() {
            this.nextIndex = 0;
            return VSConstants.S_OK;
        }

        public int Skip(uint elements) {
            this.nextIndex += elements;
            uint count = (uint)this.dependencyList.Count;

            if (this.nextIndex > count) {
                this.nextIndex = count;
                return VSConstants.S_FALSE;
            }

            return VSConstants.S_OK;
        }
    }
}
