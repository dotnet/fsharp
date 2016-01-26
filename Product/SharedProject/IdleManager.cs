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
using Microsoft.VisualStudio.OLE.Interop;
namespace Microsoft.VisualStudioTools {
    using IServiceProvider = System.IServiceProvider;

    /// <summary>
    /// Provides access to Visual Studio's idle processing using a simple .NET event
    /// based API.
    /// 
    /// The IdleManager in instantiated with an IServiceProvider and then the OnIdle
    /// event can be hooked or disconnected as needed.
    /// 
    /// Disposing of the IdleManager will disconnect from Visual Studio idle processing.
    /// </summary>
    sealed class IdleManager : IOleComponent, IDisposable {
        private uint _compId = VSConstants.VSCOOKIE_NIL;
        private readonly IServiceProvider _serviceProvider;
        private IOleComponentManager _compMgr;
        private EventHandler<ComponentManagerEventArgs> _onIdle;

        public IdleManager(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        private void EnsureInit() {
            if (_compId == VSConstants.VSCOOKIE_NIL) {
                lock (this) {
                    if (_compId == VSConstants.VSCOOKIE_NIL) {
                        if (_compMgr == null) {
                            _compMgr = (IOleComponentManager)_serviceProvider.GetService(typeof(SOleComponentManager));
                            OLECRINFO[] crInfo = new OLECRINFO[1];
                            crInfo[0].cbSize = (uint)Marshal.SizeOf(typeof(OLECRINFO));
                            crInfo[0].grfcrf = (uint)_OLECRF.olecrfNeedIdleTime;
                            crInfo[0].grfcadvf = (uint)0;
                            crInfo[0].uIdleTimeInterval = 0;
                            if (ErrorHandler.Failed(_compMgr.FRegisterComponent(this, crInfo, out _compId))) {
                                _compId = VSConstants.VSCOOKIE_NIL;
                            }
                        }
                    }
                }
            }
        }

        #region IOleComponent Members

        public int FContinueMessageLoop(uint uReason, IntPtr pvLoopData, MSG[] pMsgPeeked) {
            return 1;
        }

        public int FDoIdle(uint grfidlef) {
            var onIdle = _onIdle;
            if (onIdle != null) {
                onIdle(this, new ComponentManagerEventArgs(_compMgr));
            }

            return 0;
        }

        internal event EventHandler<ComponentManagerEventArgs> OnIdle {
            add {
                EnsureInit();
                _onIdle += value;
            }
            remove {
                EnsureInit();
                _onIdle -= value;
            }
        }

        public int FPreTranslateMessage(MSG[] pMsg) {
            return 0;
        }

        public int FQueryTerminate(int fPromptUser) {
            return 1;
        }

        public int FReserved1(uint dwReserved, uint message, IntPtr wParam, IntPtr lParam) {
            return 1;
        }

        public IntPtr HwndGetWindow(uint dwWhich, uint dwReserved) {
            return IntPtr.Zero;
        }

        public void OnActivationChange(IOleComponent pic, int fSameComponent, OLECRINFO[] pcrinfo, int fHostIsActivating, OLECHOSTINFO[] pchostinfo, uint dwReserved) {
        }

        public void OnAppActivate(int fActive, uint dwOtherThreadID) {
        }

        public void OnEnterState(uint uStateID, int fEnter) {
        }

        public void OnLoseActivation() {
        }

        public void Terminate() {
        }

        #endregion

        public void Dispose() {
            if (_compId != VSConstants.VSCOOKIE_NIL) {
                _compMgr.FRevokeComponent(_compId);
                _compId = VSConstants.VSCOOKIE_NIL;
            }
        }
    }
}