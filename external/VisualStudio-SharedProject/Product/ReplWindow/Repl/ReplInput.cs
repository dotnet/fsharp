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
using Microsoft.VisualStudio.Text;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    internal sealed class ReplSpan {
        private readonly object _span; // ITrackingSpan or string
        public readonly ReplSpanKind Kind;

        public ReplSpan(ITrackingSpan span, ReplSpanKind kind) {
            Debug.Assert(!kind.IsPrompt());
            _span = span;
            Kind = kind;
        }

        public ReplSpan(string litaral, ReplSpanKind kind) {
            _span = litaral;
            Kind = kind;
        }

        public object Span {
            get { return _span; }
        }

        public string Prompt {
            get { return (string)_span; }
        }

        public ITrackingSpan TrackingSpan {
            get { return (ITrackingSpan)_span; }
        }

        public int Length {
            get {
                return _span is string ? Prompt.Length : TrackingSpan.GetSpan(TrackingSpan.TextBuffer.CurrentSnapshot).Length;
            }
        }

        public override string ToString() {
            return String.Format("{0}: {1}", Kind, _span);
        }
    }

    internal enum ReplSpanKind {
        None,
        /// <summary>
        /// The span represents output from the program (standard output)
        /// </summary>
        Output,
        /// <summary>
        /// The span represents a prompt for input of code.
        /// </summary>
        Prompt,
        /// <summary>
        /// The span represents a 2ndary prompt for more code.
        /// </summary>
        SecondaryPrompt,
        /// <summary>
        /// The span represents code inputted after a prompt or secondary prompt.
        /// </summary>
        Language,
        /// <summary>
        /// The span represents the prompt for input for standard input (non code input)
        /// </summary>
        StandardInputPrompt,
        /// <summary>
        /// The span represents the input for a standard input (non code input)
        /// </summary>
        StandardInput,
    }

    internal static class ReplSpanKindExtensions {
        internal static bool IsPrompt(this ReplSpanKind kind) {
            switch (kind) {
                case ReplSpanKind.Prompt:
                case ReplSpanKind.SecondaryPrompt:
                case ReplSpanKind.StandardInputPrompt:
                    return true;
                default:
                    return false;
            }
        }
    }
}
