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
using Microsoft.VisualStudio.TextManager.Interop;

namespace Microsoft.VisualStudioTools.Project {
    internal class EnumBSTR : IVsEnumBSTR {
        private readonly IEnumerable<string> _enumerable;
        private IEnumerator<string> _enumerator;

        public EnumBSTR(IEnumerable<string> items) {
            _enumerable = items;
            _enumerator = _enumerable.GetEnumerator();
        }

        public int Clone(out IVsEnumBSTR ppenum) {
            ppenum = new EnumBSTR(_enumerable);
            return VSConstants.S_OK;
        }

        public int GetCount(out uint pceltCount) {
            var coll = _enumerable as ICollection<string>;
            if (coll != null) {
                pceltCount = checked((uint)coll.Count);
                return VSConstants.S_OK;
            } else {
                pceltCount = 0;
                return VSConstants.E_NOTIMPL;
            }
        }

        public int Next(uint celt, string[] rgelt, out uint pceltFetched) {
            pceltFetched = 0;

            int i = 0;
            for (; i < celt && _enumerator.MoveNext(); ++i) {
                ++pceltFetched;
                rgelt[i] = _enumerator.Current;
            }
            for (; i < celt; ++i) {
                rgelt[i] = null;
            }

            return pceltFetched > 0 ? VSConstants.S_OK : VSConstants.S_FALSE;
        }

        public int Reset() {
            _enumerator = _enumerable.GetEnumerator();
            return VSConstants.S_OK;
        }

        public int Skip(uint celt) {
            while (celt != 0 && _enumerator.MoveNext()) {
                celt--;
            }
            return VSConstants.S_OK;
        }
    }
}
