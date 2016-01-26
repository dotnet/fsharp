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
using Microsoft.VisualStudio.Shell.Interop;
using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;

namespace Microsoft.VisualStudioTools.Project.Automation
{
    /// <summary>
    /// Helper class that handle the scope of an automation function.
    /// It should be used inside a "using" directive to define the scope of the
    /// automation function and make sure that the ExitAutomation method is called.
    /// </summary>
    internal sealed class AutomationScope : IDisposable
    {
        private IVsExtensibility3 extensibility;
        private bool inAutomation;
        private bool isDisposed;

        /// <summary>
        /// Defines the beginning of the scope of an automation function. This constuctor
        /// calls EnterAutomationFunction to signal the Shell that the current function is
        /// changing the status of the automation objects.
        /// </summary>
        public AutomationScope(IServiceProvider provider)
        {
            Utilities.ArgumentNotNull("provider", provider);

            extensibility = provider.GetService(typeof(EnvDTE.IVsExtensibility)) as IVsExtensibility3;
            if (null == extensibility)
            {
                throw new InvalidOperationException();
            }
            ErrorHandler.ThrowOnFailure(extensibility.EnterAutomationFunction());
            inAutomation = true;
        }

        /// <summary>
        /// Ends the scope of the automation function. This function is also called by the
        /// Dispose method.
        /// </summary>
        public void ExitAutomation()
        {
            if (inAutomation)
            {
                ErrorHandler.ThrowOnFailure(extensibility.ExitAutomationFunction());
                inAutomation = false;
            }
        }

        /// <summary>
        /// Gets the IVsExtensibility3 interface used in the automation function.
        /// </summary>
        public IVsExtensibility3 Extensibility
        {
            get { return extensibility; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region IDisposable Members
        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    ExitAutomation();
                }

                this.isDisposed = true;
            }
        }
        #endregion
    }
}
