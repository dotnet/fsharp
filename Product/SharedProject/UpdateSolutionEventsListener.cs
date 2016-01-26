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
    /// <summary>
    /// Defines an abstract class implementing IVsUpdateSolutionEvents interfaces.
    /// </summary>
    class UpdateSolutionEventsListener : IVsUpdateSolutionEvents3, IVsUpdateSolutionEvents2, IDisposable {
        #region fields
        /// <summary>
        /// The cookie associated to the the events based IVsUpdateSolutionEvents2.
        /// </summary>
        private uint solutionEvents2Cookie;

        /// <summary>
        /// The cookie associated to the theIVsUpdateSolutionEvents3 events.
        /// </summary>
        private uint solutionEvents3Cookie;

        /// <summary>
        /// The IVsSolutionBuildManager2 object controlling the update solution events.
        /// </summary>
        private IVsSolutionBuildManager2 solutionBuildManager;


        /// <summary>
        /// The associated service provider.
        /// </summary>
        private IServiceProvider serviceProvider;

        /// <summary>
        /// Flag determining if the object has been disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Defines an object that will be a mutex for this object for synchronizing thread calls.
        /// </summary>
        private static volatile object Mutex = new object();
        #endregion

        #region ctors
        /// <summary>
        /// Overloaded constructor.
        /// </summary>
        /// <param name="serviceProvider">A service provider.</param>
        public UpdateSolutionEventsListener(IServiceProvider serviceProvider) {
            if (serviceProvider == null) {
                throw new ArgumentNullException("serviceProvider");
            }

            this.serviceProvider = serviceProvider;

            this.solutionBuildManager = this.serviceProvider.GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager2;

            if (this.solutionBuildManager == null) {
                throw new InvalidOperationException();
            }

            ErrorHandler.ThrowOnFailure(this.solutionBuildManager.AdviseUpdateSolutionEvents(this, out this.solutionEvents2Cookie));

            Debug.Assert(this.solutionBuildManager is IVsSolutionBuildManager3, "The solution build manager object implementing IVsSolutionBuildManager2 does not implement IVsSolutionBuildManager3");
            ErrorHandler.ThrowOnFailure(this.SolutionBuildManager3.AdviseUpdateSolutionEvents3(this, out this.solutionEvents3Cookie));
        }
        #endregion

        #region properties

        /// <summary>
        /// The associated service provider.
        /// </summary>
        protected IServiceProvider ServiceProvider {
            get {
                return this.serviceProvider;
            }
        }

        /// <summary>
        /// The solution build manager object controlling the solution events.
        /// </summary>
        protected IVsSolutionBuildManager2 SolutionBuildManager2 {
            get {
                return this.solutionBuildManager;
            }
        }

        /// <summary>
        /// The solution build manager object controlling the solution events.
        /// </summary>
        protected IVsSolutionBuildManager3 SolutionBuildManager3 {
            get {
                return (IVsSolutionBuildManager3)this.solutionBuildManager;
            }

        }
        #endregion

        #region IVsUpdateSolutionEvents3 Members

        /// <summary>
        /// Fired after the active solution config is changed (pOldActiveSlnCfg can be NULL).
        /// </summary>
        /// <param name="oldActiveSlnCfg">Old configuration.</param>
        /// <param name="newActiveSlnCfg">New configuration.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int OnAfterActiveSolutionCfgChange(IVsCfg oldActiveSlnCfg, IVsCfg newActiveSlnCfg) {
            var handlers = AfterActiveSolutionConfigurationChange;
            if (handlers != null) {
                handlers(this, EventArgs.Empty);
            }
            return VSConstants.S_OK;
        }

        public event EventHandler AfterActiveSolutionConfigurationChange;

        /// <summary>
        /// Fired before the active solution config is changed (pOldActiveSlnCfg can be NULL
        /// </summary>
        /// <param name="oldActiveSlnCfg">Old configuration.</param>
        /// <param name="newActiveSlnCfg">New configuration.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int OnBeforeActiveSolutionCfgChange(IVsCfg oldActiveSlnCfg, IVsCfg newActiveSlnCfg) {
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region IVsUpdateSolutionEvents2 Members

        /// <summary>
        /// Called when the active project configuration for a project in the solution has changed. 
        /// </summary>
        /// <param name="hierarchy">The project whose configuration has changed.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int OnActiveProjectCfgChange(IVsHierarchy hierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        /// <summary>
        /// Called right before a project configuration begins to build. 
        /// </summary>
        /// <param name="hierarchy">The project that is to be build.</param>
        /// <param name="configProject">A configuration project object.</param>
        /// <param name="configSolution">A configuration solution object.</param>
        /// <param name="action">The action taken.</param>
        /// <param name="cancel">A flag indicating cancel.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        /// <remarks>The values for the action are defined in the enum _SLNUPDACTION env\msenv\core\slnupd2.h</remarks>
        public int UpdateProjectCfg_Begin(IVsHierarchy hierarchy, IVsCfg configProject, IVsCfg configSolution, uint action, ref int cancel) {
            return VSConstants.E_NOTIMPL;
        }

        /// <summary>
        /// Called right after a project configuration is finished building. 
        /// </summary>
        /// <param name="hierarchy">The project that has finished building.</param>
        /// <param name="configProject">A configuration project object.</param>
        /// <param name="configSolution">A configuration solution object.</param>
        /// <param name="action">The action taken.</param>
        /// <param name="success">Flag indicating success.</param>
        /// <param name="cancel">Flag indicating cancel.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        /// <remarks>The values for the action are defined in the enum _SLNUPDACTION env\msenv\core\slnupd2.h</remarks>
        public virtual int UpdateProjectCfg_Done(IVsHierarchy hierarchy, IVsCfg configProject, IVsCfg configSolution, uint action, int success, int cancel) {
            return VSConstants.E_NOTIMPL;
        }

        /// <summary>
        /// Called before any build actions have begun. This is the last chance to cancel the build before any building begins. 
        /// </summary>
        /// <param name="cancelUpdate">Flag indicating cancel update.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int UpdateSolution_Begin(ref int cancelUpdate) {
            return VSConstants.E_NOTIMPL;
        }

        /// <summary>
        /// Called when a build is being cancelled. 
        /// </summary>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int UpdateSolution_Cancel() {
            return VSConstants.E_NOTIMPL;
        }

        /// <summary>
        /// Called when a build is completed. 
        /// </summary>
        /// <param name="succeeded">true if no update actions failed.</param>
        /// <param name="modified">true if any update action succeeded.</param>
        /// <param name="cancelCommand">true if update actions were canceled.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand) {
            return VSConstants.E_NOTIMPL;
        }

        /// <summary>
        /// Called before the first project configuration is about to be built. 
        /// </summary>
        /// <param name="cancelUpdate">A flag indicating cancel update.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int UpdateSolution_StartUpdate(ref int cancelUpdate) {
            return VSConstants.E_NOTIMPL;
        }

        #endregion


        #region IDisposable Members

        /// <summary>
        /// The IDispose interface Dispose method for disposing the object determinastically.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region methods
        /// <summary>
        /// The method that does the cleanup.
        /// </summary>
        /// <param name="disposing">true if called from IDispose.Dispose; false if called from Finalizer.</param>
        protected virtual void Dispose(bool disposing) {
            // Everybody can go here.
            if (!this.isDisposed) {
                // Synchronize calls to the Dispose simultaniously.
                lock (Mutex) {
                    if (this.solutionEvents2Cookie != (uint)ShellConstants.VSCOOKIE_NIL) {
                        ErrorHandler.ThrowOnFailure(this.solutionBuildManager.UnadviseUpdateSolutionEvents(this.solutionEvents2Cookie));
                        this.solutionEvents2Cookie = (uint)ShellConstants.VSCOOKIE_NIL;
                    }

                    if (this.solutionEvents3Cookie != (uint)ShellConstants.VSCOOKIE_NIL) {
                        ErrorHandler.ThrowOnFailure(this.SolutionBuildManager3.UnadviseUpdateSolutionEvents3(this.solutionEvents3Cookie));
                        this.solutionEvents3Cookie = (uint)ShellConstants.VSCOOKIE_NIL;
                    }

                    this.isDisposed = true;
                }
            }
        }
        #endregion
    }
}
