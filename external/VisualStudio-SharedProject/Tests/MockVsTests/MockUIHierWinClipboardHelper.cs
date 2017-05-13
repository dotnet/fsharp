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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockUIHierWinClipboardHelper : IVsUIHierWinClipboardHelper {
        private readonly Dictionary<uint, IVsUIHierWinClipboardHelperEvents> _sinks = new Dictionary<uint, IVsUIHierWinClipboardHelperEvents>();
        private uint _sinkCount;
        bool _wasCut = false;

        public int AdviseClipboardHelperEvents(IVsUIHierWinClipboardHelperEvents pSink, out uint pdwCookie) {
            _sinks[++_sinkCount] = pSink;
            pdwCookie = _sinkCount;
            return VSConstants.S_OK;
        }

        public int Copy(VisualStudio.OLE.Interop.IDataObject pDataObject) {
            _wasCut = false;
            return VSConstants.S_FALSE;
        }

        public int Cut(VisualStudio.OLE.Interop.IDataObject pDataObject) {
            _wasCut = true;
            return VSConstants.S_OK;
        }

        public int Paste(VisualStudio.OLE.Interop.IDataObject pDataObject, uint dwEffects) {
            foreach (var value in _sinks.Values) {
                value.OnPaste(_wasCut ? 1 : 0, dwEffects);
            }
            return VSConstants.S_OK;
        }

        public int UnadviseClipboardHelperEvents(uint dwCookie) {
            _sinks.Remove(dwCookie);
            return VSConstants.S_OK;
        }
    }
}
