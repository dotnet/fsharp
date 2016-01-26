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

namespace Microsoft.VisualStudioTools.MockVsTests {
    class MockVsTrackSelectionEx : IVsTrackSelectionEx {
        private readonly MockVsMonitorSelection _monSel;
        private IVsHierarchy _curHierarchy;
        private uint _itemid;
        private IVsMultiItemSelect _multiSel;
        private ISelectionContainer _selectionContainer;
        private static IntPtr HIERARCHY_DONTCHANGE = new IntPtr(-1);

        public MockVsTrackSelectionEx(MockVsMonitorSelection monSel) {
            _monSel = monSel;
        }

        public int GetCurrentSelection(out IntPtr ppHier, out uint pitemid, out IVsMultiItemSelect ppMIS, out IntPtr ppSC) {
            if (_curHierarchy != null) {
                ppHier = Marshal.GetIUnknownForObject(_curHierarchy);
            } else {
                ppHier = IntPtr.Zero;
            }
            pitemid = _itemid;
            ppMIS = _multiSel;
            if (_selectionContainer != null) {
                ppSC = Marshal.GetIUnknownForObject(_selectionContainer);
            } else {
                ppSC = IntPtr.Zero;
            }
            return VSConstants.S_OK;
        }

        public void GetCurrentSelection(out IVsHierarchy ppHier, out uint pitemid, out IVsMultiItemSelect ppMIS, out ISelectionContainer ppSC) {
            ppHier = _curHierarchy;
            pitemid = _itemid;
            ppMIS = _multiSel;
            ppSC = _selectionContainer;
        }

        public int IsMyHierarchyCurrent(out int pfCurrent) {
            throw new NotImplementedException();
        }

        public int OnElementValueChange(uint elementid, int fDontPropagate, object varValue) {
            _monSel.NotifyElementChanged(this, elementid);
            return VSConstants.S_OK;
        }

        public int OnSelectChange(ISelectionContainer pSC) {
            var sc = Marshal.GetIUnknownForObject(pSC);
            try {
                return OnSelectChangeEx(
                    HIERARCHY_DONTCHANGE,
                    VSConstants.VSITEMID_NIL,
                    null,
                    sc
                );
            } finally {
                Marshal.Release(sc);
            }
        }

        public int OnSelectChangeEx(IntPtr pHier, uint itemid, IVsMultiItemSelect pMIS, IntPtr pSC) {
            if (pHier != HIERARCHY_DONTCHANGE) {
                if (pHier != IntPtr.Zero) {
                    _curHierarchy = Marshal.GetObjectForIUnknown(pHier) as IVsHierarchy;
                } else {
                    _curHierarchy = null;
                }
                _itemid = itemid;
            }
            _multiSel = pMIS;
            if (pSC != null) {
                _selectionContainer = (ISelectionContainer)Marshal.GetObjectForIUnknown(pSC);
            } else {
                _selectionContainer = null;
            }

            _monSel.NotifySelectionContextChanged(this);

            return VSConstants.S_OK;
        }

        public void OnSelectChangeEx(IVsHierarchy pHier, uint itemid, IVsMultiItemSelect pMIS, ISelectionContainer pSC) {
            _curHierarchy = pHier;
            _itemid = itemid;
            OnSelectChangeEx(pMIS, pSC);
        }

        public void OnSelectChangeEx(IVsMultiItemSelect pMIS, ISelectionContainer pSC) {
            _multiSel = pMIS;
            _selectionContainer = pSC;

            _monSel.NotifySelectionContextChanged(this);
        }
    }
}
