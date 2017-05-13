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
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace TestUtilities.Mocks {
    [ComVisible(true)]
    public class MockSettingsManager : IVsSettingsManager {
        public readonly MockSettingsStore Store = new MockSettingsStore();

        public int GetApplicationDataFolder(uint folder, out string folderPath) {
            throw new NotImplementedException();
        }

        public int GetCollectionScopes(string collectionPath, out uint scopes) {
            throw new NotImplementedException();
        }

        public int GetCommonExtensionsSearchPaths(uint paths, string[] commonExtensionsPaths, out uint actualPaths) {
            throw new NotImplementedException();
        }

        public int GetPropertyScopes(string collectionPath, string propertyName, out uint scopes) {
            throw new NotImplementedException();
        }

        public int GetReadOnlySettingsStore(uint scope, out IVsSettingsStore store) {
            store = Store;
            return VSConstants.S_OK;
        }

        public int GetWritableSettingsStore(uint scope, out IVsWritableSettingsStore writableStore) {
            writableStore = Store;
            return VSConstants.S_OK;
        }
    }
}
