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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudioTools;

namespace Microsoft.VisualStudioTools {
    class ProjectEventArgs : EventArgs {
        public IVsProject Project { get; private set; }

        public ProjectEventArgs(IVsProject project) {
            Project = project;
        }
    }

    class SolutionEventsListener : IVsSolutionEvents3, IVsSolutionEvents4, IVsUpdateSolutionEvents2, IVsUpdateSolutionEvents3, IDisposable {
        private readonly IVsSolution _solution;
        private readonly IVsSolutionBuildManager3 _buildManager;
        private uint _cookie1 = VSConstants.VSCOOKIE_NIL;
        private uint _cookie2 = VSConstants.VSCOOKIE_NIL;
        private uint _cookie3 = VSConstants.VSCOOKIE_NIL;

        public event EventHandler SolutionOpened;
        public event EventHandler SolutionClosed;
        public event EventHandler<ProjectEventArgs> ProjectLoaded;
        public event EventHandler<ProjectEventArgs> ProjectUnloading;
        public event EventHandler<ProjectEventArgs> ProjectClosing;
        public event EventHandler<ProjectEventArgs> ProjectRenamed;
        public event EventHandler BuildCompleted;
        public event EventHandler BuildStarted;
        public event EventHandler ActiveSolutionConfigurationChanged;

        public SolutionEventsListener(IServiceProvider serviceProvider) {
            if (serviceProvider == null) {
                throw new ArgumentNullException("serviceProvider");
            }

            _solution = serviceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
            if (_solution == null) {
                throw new InvalidOperationException("Cannot get solution service");
            }
            _buildManager = serviceProvider.GetService(typeof(SVsSolutionBuildManager)) as IVsSolutionBuildManager3;
        }

        public SolutionEventsListener(IVsSolution service, IVsSolutionBuildManager3 buildManager = null) {
            if (service == null) {
                throw new ArgumentNullException("service");
            }
            _solution = service;
            _buildManager = buildManager;
        }

        public void StartListeningForChanges() {
            ErrorHandler.ThrowOnFailure(_solution.AdviseSolutionEvents(this, out _cookie1));
            if (_buildManager != null) {
                var bm2 = _buildManager as IVsSolutionBuildManager2;
                if (bm2 != null) {
                    ErrorHandler.ThrowOnFailure(bm2.AdviseUpdateSolutionEvents(this, out _cookie2));
                }
                ErrorHandler.ThrowOnFailure(_buildManager.AdviseUpdateSolutionEvents3(this, out _cookie3));
            }
        }

        public void Dispose() {
            // Ignore failures in UnadviseSolutionEvents
            if (_cookie1 != VSConstants.VSCOOKIE_NIL) {
                _solution.UnadviseSolutionEvents(_cookie1);
                _cookie1 = VSConstants.VSCOOKIE_NIL;
            }
            if (_cookie2 != VSConstants.VSCOOKIE_NIL) {
                ((IVsSolutionBuildManager2)_buildManager).UnadviseUpdateSolutionEvents(_cookie2);
                _cookie2 = VSConstants.VSCOOKIE_NIL;
            }
            if (_cookie3 != VSConstants.VSCOOKIE_NIL) {
                _buildManager.UnadviseUpdateSolutionEvents3(_cookie3);
                _cookie3 = VSConstants.VSCOOKIE_NIL;
            }
        }

        int IVsUpdateSolutionEvents2.OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        int IVsUpdateSolutionEvents2.UpdateProjectCfg_Begin(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, ref int pfCancel) {
            return VSConstants.E_NOTIMPL;
        }

        int IVsUpdateSolutionEvents2.UpdateProjectCfg_Done(IVsHierarchy pHierProj, IVsCfg pCfgProj, IVsCfg pCfgSln, uint dwAction, int fSuccess, int fCancel) {
            return VSConstants.E_NOTIMPL;
        }

        public int OnActiveProjectCfgChange(IVsHierarchy pIVsHierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        public int UpdateSolution_Begin(ref int pfCancelUpdate) {
            var buildStarted = BuildStarted;
            if (buildStarted != null) {
                buildStarted(this, EventArgs.Empty);
            }
            return VSConstants.S_OK;
        }

        public int UpdateSolution_Cancel() {
            return VSConstants.E_NOTIMPL;
        }

        public int UpdateSolution_Done(int fSucceeded, int fModified, int fCancelCommand) {
            var buildCompleted = BuildCompleted;
            if (buildCompleted != null) {
                buildCompleted(this, EventArgs.Empty);
            }
            return VSConstants.S_OK;
        }

        public int UpdateSolution_StartUpdate(ref int pfCancelUpdate) {
            return VSConstants.E_NOTIMPL;
        }

        int IVsUpdateSolutionEvents3.OnAfterActiveSolutionCfgChange(IVsCfg pOldActiveSlnCfg, IVsCfg pNewActiveSlnCfg) {
            var evt = ActiveSolutionConfigurationChanged;
            if (evt != null) {
                evt(this, EventArgs.Empty);
            }
            return VSConstants.S_OK;
        }

        int IVsUpdateSolutionEvents3.OnBeforeActiveSolutionCfgChange(IVsCfg pOldActiveSlnCfg, IVsCfg pNewActiveSlnCfg) {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterCloseSolution(object pUnkReserved) {
            var evt = SolutionClosed;
            if (evt != null) {
                evt(this, EventArgs.Empty);
            }
            return VSConstants.S_OK;
        }

        public int OnAfterClosingChildren(IVsHierarchy pHierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterMergeSolution(object pUnkReserved) {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded) {
            var project = pHierarchy as IVsProject;
            if (project != null) {
                var evt = ProjectLoaded;
                if (evt != null) {
                    evt(this, new ProjectEventArgs(project));
                }
            }
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution) {
            var evt = SolutionOpened;
            if (evt != null) {
                evt(this, EventArgs.Empty);
            }
            return VSConstants.S_OK;
        }

        public int OnAfterOpeningChildren(IVsHierarchy pHierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved) {
            var project = pHierarchy as IVsProject;
            if (project != null) {
                var evt = ProjectClosing;
                if (evt != null) {
                    evt(this, new ProjectEventArgs(project));
                }
            }
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved) {
            return VSConstants.E_NOTIMPL;
        }

        public int OnBeforeClosingChildren(IVsHierarchy pHierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        public int OnBeforeOpeningChildren(IVsHierarchy pHierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy) {
            var project = pRealHierarchy as IVsProject;
            if (project != null) {
                var evt = ProjectUnloading;
                if (evt != null) {
                    evt(this, new ProjectEventArgs(project));
                }
            }
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel) {
            return VSConstants.E_NOTIMPL;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel) {
            return VSConstants.E_NOTIMPL;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel) {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterAsynchOpenProject(IVsHierarchy pHierarchy, int fAdded) {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterChangeProjectParent(IVsHierarchy pHierarchy) {
            return VSConstants.E_NOTIMPL;
        }

        public int OnAfterRenameProject(IVsHierarchy pHierarchy) {
            var project = pHierarchy as IVsProject;
            if (project != null) {
                var evt = ProjectRenamed;
                if (evt != null) {
                    evt(this, new ProjectEventArgs(project));
                }
            }
            return VSConstants.S_OK;
        }

        public int OnQueryChangeProjectParent(IVsHierarchy pHierarchy, IVsHierarchy pNewParentHier, ref int pfCancel) {
            return VSConstants.E_NOTIMPL;
        }
    }
}
