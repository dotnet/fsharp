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
using System.Threading.Tasks;

namespace Microsoft.VisualStudioTools {
    /// <summary>
    /// Provides the ability to run code on the VS UI thread.
    /// 
    /// UIThreadBase must be an abstract class rather than an interface because the CLR
    /// doesn't take assembly names into account when generating an interfaces GUID, resulting 
    /// in resolution issues when we reference the interface from multiple assemblies.
    /// </summary>
    public abstract class UIThreadBase {
        public abstract void Invoke(Action action);
        public abstract T Invoke<T>(Func<T> func);
        public abstract Task InvokeAsync(Action action);
        public abstract Task<T> InvokeAsync<T>(Func<T> func);
        public abstract Task InvokeTask(Func<Task> func);
        public abstract Task<T> InvokeTask<T>(Func<Task<T>> func);
        public abstract void MustBeCalledFromUIThreadOrThrow();

        public abstract bool InvokeRequired {
            get;
        }
    }

    /// <summary>
    /// Identifies mock implementations of IUIThread.
    /// </summary>
    public abstract class MockUIThreadBase : UIThreadBase {
        public override void Invoke(Action action) {
            throw new NotImplementedException();
        }

        public override T Invoke<T>(Func<T> func) {
            throw new NotImplementedException();
        }

        public override Task InvokeAsync(Action action) {
            throw new NotImplementedException();
        }

        public override Task<T> InvokeAsync<T>(Func<T> func) {
            throw new NotImplementedException();
        }

        public override Task InvokeTask(Func<Task> func) {
            throw new NotImplementedException();
        }

        public override Task<T> InvokeTask<T>(Func<Task<T>> func) {
            throw new NotImplementedException();
        }

        public override void MustBeCalledFromUIThreadOrThrow() {
            throw new NotImplementedException();
        }

        public override bool InvokeRequired {
            get { throw new NotImplementedException(); }
        }
    }
}
