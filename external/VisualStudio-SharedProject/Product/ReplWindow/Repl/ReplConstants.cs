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
using System.Linq;
using System.Text;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    public static class ReplConstants {
#if NTVS_FEATURE_INTERACTIVEWINDOW
        public const string ReplContentTypeName = "NodejsREPLCode";
        public const string ReplOutputContentTypeName = "NodejsREPLOutput";

        /// <summary>
        /// The additional role found in any REPL editor window.
        /// </summary>
        public const string ReplTextViewRole = "NodejsREPL";
#else
        public const string ReplContentTypeName = "REPLCode";
        public const string ReplOutputContentTypeName = "REPLOutput";

        /// <summary>
        /// The additional role found in any REPL editor window.
        /// </summary>
        public const string ReplTextViewRole = "REPL";
#endif
    }
}
