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

namespace Microsoft.VisualStudioTools.Project {
    sealed class WaitDialog : IDisposable {
        private readonly int _waitResult;
        private readonly IVsThreadedWaitDialog2 _waitDialog;

        public WaitDialog(string waitCaption, string waitMessage, IServiceProvider serviceProvider, int displayDelay = 1, bool isCancelable = false, bool showProgress = false) {
            _waitDialog = (IVsThreadedWaitDialog2)serviceProvider.GetService(typeof(SVsThreadedWaitDialog));
            _waitResult = _waitDialog.StartWaitDialog(
                waitCaption,
                waitMessage,
                null,
                null,
                null,
                displayDelay,
                isCancelable,
                showProgress
            );
        }

        public void UpdateProgress(int currentSteps, int totalSteps) {
            bool canceled;
            _waitDialog.UpdateProgress(
                null,
                null,
                null,
                currentSteps,
                totalSteps,
                false,
                out canceled
            );

        }

        public bool Canceled {
            get {
                bool canceled;
                ErrorHandler.ThrowOnFailure(_waitDialog.HasCanceled(out canceled));
                return canceled;
            }
        }

        #region IDisposable Members

        public void Dispose() {
            if (ErrorHandler.Succeeded(_waitResult)) {
                int cancelled = 0;
                _waitDialog.EndWaitDialog(out cancelled);
            }
        }

        #endregion
    }
}