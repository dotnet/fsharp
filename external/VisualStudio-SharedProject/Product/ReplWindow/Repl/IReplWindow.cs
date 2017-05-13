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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    /// <summary>
    /// An implementation of a Read Eval Print Loop Window for iteratively developing code.
    /// 
    /// Instances of the repl window can be created by using MEF to import the IReplWindowProvider interface.
    /// </summary>
    public interface IReplWindow {
        /// <summary>
        /// Gets the IWpfTextView in which the REPL window is executing.
        /// </summary>
        IWpfTextView TextView {
            get;
        }

        /// <summary>
        /// Returns the current language buffer.
        /// </summary>
        ITextBuffer CurrentLanguageBuffer {
            get; 
        }

        /// <summary>
        /// The language evaluator used in Repl Window
        /// </summary>
        IReplEvaluator Evaluator {
            get;
        }

        /// <summary>
        /// Title of the Repl Window
        /// </summary>
        string Title {
            get;
        }

        /// <summary>
        /// Clears the REPL window screen.
        /// </summary>
        void ClearScreen();

        /// <summary>
        /// Clears the input history.
        /// </summary>
        void ClearHistory();

        /// <summary>
        /// Focuses the window.
        /// </summary>
        void Focus();
        
        /// <summary>
        /// Clears the current input.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Insert the specified text to the active code buffer at the current caret position.
        /// </summary>
        /// <param name="text">Text to insert.</param>
        /// <remarks>
        /// Overwrites the current selection.
        /// 
        /// If the REPL is in the middle of code execution the text is inserted at the end of a pending input buffer.
        /// When the REPL is ready for input the pending input is inserted into the active code input.
        /// </remarks>
        void InsertCode(string text);

        /// <summary>
        /// Submits a sequence of inputs one by one.
        /// </summary>
        /// <param name="inputs">
        /// Code snippets or REPL commands to submit.
        /// </param>
        /// <remarks>
        /// Enques given code snippets for submission at the earliest time the REPL is prepared to accept submissions.
        /// Any submissions are postponed until execution of the current submission (if there is any) is finished or aborted.
        /// 
        /// The REPL processes the given inputs one by one creating a prompt, input span and possibly output span for each input.
        /// This method may be reentered if any of the inputs evaluates to a command that invokes this method.
        /// </remarks>
        void Submit(IEnumerable<string> inputs);

        /// <summary>
        /// Resets the execution context clearing all variables.
        /// </summary>
        Task<ExecutionResult> Reset();

        /// <summary>
        /// Aborts the current command which is executing.
        /// </summary>
        void AbortCommand();

        /// <summary>
        /// Writes a line into the output buffer as if it was outputted by the program.
        /// </summary>
        /// <param name="text"></param>
        void WriteLine(string text);

        /// <summary>
        /// Writes output to the REPL window.
        /// </summary>
        /// <param name="value"></param>
        void WriteOutput(object value);

        /// <summary>
        /// Writes error output to the REPL window.
        /// </summary>
        /// <param name="value"></param>
        void WriteError(object value);

        /// <summary>
        /// Reads input from the REPL window.
        /// </summary>
        /// <returns>The entered input or null if cancelled.</returns>
        string ReadStandardInput();

        /// <summary>
        /// Sets the current value for the specified option.
        /// 
        /// It is safe to call this method from any thread.
        /// </summary>
        void SetOptionValue(ReplOptions option, object value);

        /// <summary>
        /// Gets the current value for the specified option.
        /// 
        /// It is safe to call this method from any thread.
        /// </summary>
        object GetOptionValue(ReplOptions option);

        /// <summary>
        /// Event triggered when the REPL is ready to accept input.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "back compat")]
        event Action ReadyForInput;
    }

    public interface IReplWindow2 : IReplWindow {

        /// <summary>
        /// Executes a special REPL command as if it were submitted as an input.
        /// </summary>
        /// <remarks>
        /// This method can only execute special (prefixed) commands. To evaluate code snippers, use <see cref="Evaluator"/>.
        /// </remarks>
        Task<ExecutionResult> ExecuteCommand(string text);
    }

    public interface IReplWindow3 : IReplWindow2 {

        /// <summary>
        /// Storage
        /// </summary>
        PropertyCollection Properties { get; }
    }
}
