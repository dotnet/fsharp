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


using System.Threading.Tasks;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    /// <summary>
    /// The result of command execution.  
    /// </summary>
    public struct ExecutionResult {
        public static readonly ExecutionResult Success = new ExecutionResult(true);
        public static readonly ExecutionResult Failure = new ExecutionResult(false);
        public static readonly Task<ExecutionResult> Succeeded = MakeSucceeded();
        public static readonly Task<ExecutionResult> Failed = MakeFailed();
 
        private readonly bool _successful;

        public ExecutionResult(bool isSuccessful) {
            _successful = isSuccessful;
        }

        public bool IsSuccessful {
            get {
                return _successful;
            }
        }

        private static Task<ExecutionResult> MakeSucceeded() {
            var taskSource = new TaskCompletionSource<ExecutionResult>();
            taskSource.SetResult(Success);
            return taskSource.Task;
        }

        private static Task<ExecutionResult> MakeFailed() {
            var taskSource = new TaskCompletionSource<ExecutionResult>();
            taskSource.SetResult(Failure);
            return taskSource.Task;
        }
    }
}
