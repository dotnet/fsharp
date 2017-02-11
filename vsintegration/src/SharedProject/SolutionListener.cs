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
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using IServiceProvider = System.IServiceProvider;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;

namespace Microsoft.VisualStudioTools.Project {


    internal abstract class SolutionListener : IVsSolutionEvents3, IVsSolutionEvents4, IDisposable {

        #region fields
        private uint eventsCookie;
        private IVsSolution solution;
        private IServiceProvider serviceProvider;
        private bool isDisposed;
        /// <summary>
        /// Defines an object that will be a mutex for this object for synchronizing thread calls.
        /// </summary>
        private static volatile object Mutex = new object();
        #endregion

        #region ctors
        protected SolutionListener(IServiceProvider serviceProviderParameter) {
            if (serviceProviderParameter == null) {
                throw new ArgumentNullException("serviceProviderParameter");
            }

            this.serviceProvider = serviceProviderParameter;
            this.solution = this.serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;

            if (this.solution == null) {
                throw new InvalidOperationException("Could not get the IVsSolution object from the services exposed by this project");
            }
        }
        #endregion

        #region properties
        protected uint EventsCookie {
            get {
                return this.eventsCookie;
            }
        }

        protected IVsSolution Solution {
            get {
                return this.solution;
            }
        }

        protected IServiceProvider ServiceProvider {
            get {
                return this.serviceProvider;
            }
        }
        #endregion

        #region IVsSolutionEvents3, IVsSolutionEvents2, IVsSolutionEvents methods
        public virtual int OnAfterCloseSolution(object reserved) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnAfterClosingChildren(IVsHierarchy hierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnAfterLoadProject(IVsHierarchy stubHierarchy, IVsHierarchy realHierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnAfterMergeSolution(object pUnkReserved) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnAfterOpenProject(IVsHierarchy hierarchy, int added) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnAfterOpenSolution(object pUnkReserved, int fNewSolution) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnAfterOpeningChildren(IVsHierarchy hierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnBeforeCloseProject(IVsHierarchy hierarchy, int removed) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnBeforeCloseSolution(object pUnkReserved) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnBeforeClosingChildren(IVsHierarchy hierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnBeforeOpeningChildren(IVsHierarchy hierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnBeforeUnloadProject(IVsHierarchy realHierarchy, IVsHierarchy rtubHierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnQueryCloseProject(IVsHierarchy hierarchy, int removing, ref int cancel) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnQueryCloseSolution(object pUnkReserved, ref int cancel) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int cancel) {
            return VSConstants.E_NOTIMPL;
        }
        #endregion

        #region IVsSolutionEvents4 methods
        public virtual int OnAfterAsynchOpenProject(IVsHierarchy hierarchy, int added) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnAfterChangeProjectParent(IVsHierarchy hierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OnAfterRenameProject(IVsHierarchy hierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        /// <summary>
        /// Fired before a project is moved from one parent to another in the solution explorer
        /// </summary>
        public virtual int OnQueryChangeProjectParent(IVsHierarchy hierarchy, IVsHierarchy newParentHier, ref int cancel) {
            return VSConstants.E_NOTIMPL;
        }
        #endregion

        #region Dispose

        /// <summary>
        /// The IDispose interface Dispose method for disposing the object determinastically.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region methods
        public void Init() {
            if (this.solution != null) {
                ErrorHandler.ThrowOnFailure(this.solution.AdviseSolutionEvents(this, out this.eventsCookie));
            }
        }

        /// <summary>
        /// The method that does the cleanup.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            // Everybody can go here.
            if (!this.isDisposed) {
                // Synchronize calls to the Dispose simulteniously.
                lock (Mutex) {
                    if (disposing && this.eventsCookie != (uint)ShellConstants.VSCOOKIE_NIL && this.solution != null) {
                        this.solution.UnadviseSolutionEvents((uint)this.eventsCookie);
                        this.eventsCookie = (uint)ShellConstants.VSCOOKIE_NIL;
                    }

                    this.isDisposed = true;
                }
            }
        }
        #endregion
    }
}
