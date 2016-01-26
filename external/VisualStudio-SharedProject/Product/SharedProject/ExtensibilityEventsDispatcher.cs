/* ****************************************************************************
 *
 * Copyright (c) DEVSENSE. 
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
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.Project {
    /// <summary>
    /// This is a helper class which fires IVsExtensibility3 events if not in suspended state.
    /// </summary>
    internal sealed class ExtensibilityEventsDispatcher {
        private class SuspendLock : IDisposable {
            private readonly bool _previousState;
            private readonly ExtensibilityEventsDispatcher _owner;

            public SuspendLock(ExtensibilityEventsDispatcher owner) {
                this._owner = owner;
                this._previousState = this._owner._suspended;
                this._owner._suspended = true;
            }

            void IDisposable.Dispose() {
                this._owner._suspended = this._previousState;
            }
        }

        private readonly ProjectNode _project;
        private bool _suspended;

        public ExtensibilityEventsDispatcher(ProjectNode/*!*/ project) {
            Utilities.ArgumentNotNull("project", project);

            this._project = project;
        }

        /// <summary>
        /// Creates a lock which suspends firing of the events until it gets disposed.
        /// </summary>
        public IDisposable Suspend() {
            return new SuspendLock(this);
        }

        public void FireItemAdded(HierarchyNode node) {
            this.Fire(node, (IVsExtensibility3 vsExtensibility, ProjectItem item) => {
                vsExtensibility.FireProjectItemsEvent_ItemAdded(item);
            });
        }

        public void FireItemRemoved(HierarchyNode node) {
            this.Fire(node, (IVsExtensibility3 vsExtensibility, ProjectItem item) => {
                vsExtensibility.FireProjectItemsEvent_ItemRemoved(item);
            });
        }

        public void FireItemRenamed(HierarchyNode node, string oldName) {
            this.Fire(node, (IVsExtensibility3 vsExtensibility, ProjectItem item) => {
                vsExtensibility.FireProjectItemsEvent_ItemRenamed(item, oldName);
            });
        }

        private void Fire(HierarchyNode node, Action<IVsExtensibility3, ProjectItem> fire) {
            // When we are in suspended mode. Do not fire anything
            if (this._suspended) {
                return;
            }

            // Project has to be opened
            if (!this._project.IsProjectOpened) {
                return;
            }

            // We don't want to fire events for references here. OAReferences should do the job
            if (node is ReferenceNode) {
                return;
            }

            IVsExtensibility3 vsExtensibility = this._project.GetService(typeof(IVsExtensibility)) as IVsExtensibility3;
            if (vsExtensibility != null) {
                object obj = node.GetAutomationObject();
                ProjectItem item = obj as ProjectItem;
                if (item != null) {
                    fire(vsExtensibility, item);
                }
            }
        }
    }
}
