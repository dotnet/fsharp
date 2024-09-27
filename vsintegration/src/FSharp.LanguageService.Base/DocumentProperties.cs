// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

#if DOCUMENT_PROPERTIES
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using System.ComponentModel;
using Ole = Microsoft.VisualStudio.OLE.Interop;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;

namespace Microsoft.VisualStudio.FSharp.LanguageService {

/// <summary>
/// This class can be used as a base class for document properties which are 
/// displayed in the Properties Window when the document is active.  Simply add
/// some public properties and they will show up in the properties window.  
/// </summary>
    internal abstract class DocumentProperties : LocalizableProperties, ISelectionContainer, IDisposable {
        internal CodeWindowManager mgr;
        internal IVsTrackSelectionEx tracker;
        private bool visible;

        protected DocumentProperties(CodeWindowManager mgr) {
            this.mgr = mgr;
            this.visible = true;
            if (mgr != null) {
                IOleServiceProvider sp = mgr.CodeWindow as IOleServiceProvider;
                if (sp != null) {
                    ServiceProvider site = new ServiceProvider(sp);
                    this.tracker = site.GetService(typeof(SVsTrackSelectionEx)) as IVsTrackSelectionEx;
                }
            }
        }

        [BrowsableAttribute(false)]
        public bool Visible {
            get { return this.visible; }
            set { if (this.visible != value) { this.visible = value; Refresh(); } }
        }

        /// <summary>
        /// Call this method when you want the document properties window updated with new information.
        /// </summary>
        public void Refresh() {
            if (this.tracker != null && this.visible) {
                NativeMethods.ThrowOnFailure(tracker.OnSelectChange(this));
            }
        }

        /// This is not a property because all public properties show up in the Properties window.
        internal ISource GetSource() {
            if (this.mgr == null) return null;
            return this.mgr.Source;
        }

        /// This is not a property because all public properties show up in the Properties window.
        public CodeWindowManager GetCodeWindowManager() {
            return this.mgr;
        }

        public void Close() {
            if (this.tracker != null && this.visible)
                NativeMethods.ThrowOnFailure(tracker.OnSelectChange(null));

            this.Dispose(true);
        }

        public void Dispose() {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing) {
            // If disposing equals true, dispose all managed 
            // and unmanaged resources.
            if (disposing) {
                // Dispose managed resources.
                
            }
            this.tracker = null;
            this.mgr = null;
        }

        ~DocumentProperties() {
            Dispose(false);
        }

        public virtual int CountObjects(uint flags, out uint pc) {
            pc = this.visible ? (uint)1 : (uint)0;
            return NativeMethods.S_OK;
        }
        public virtual int GetObjects(uint flags, uint count, object[] ppUnk) {
            if (count == 1) {
                ppUnk[0] = this;
            }
            return NativeMethods.S_OK;
        }
        public virtual int SelectObjects(uint sel, object[] selobj, uint flags) {
            // nop
            return NativeMethods.S_OK;
        }
    }

}
#endif
