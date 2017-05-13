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

using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.Utilities;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif

    [Export(typeof(IClassifierProvider)), ContentType(ReplConstants.ReplContentTypeName)]
    class ReplAggregateClassifierProvider : IClassifierProvider {
        [Import] private IBufferGraphFactoryService _bufferGraphFact = null; // set via MEF

        #region IClassifierProvider Members

        public IClassifier GetClassifier(ITextBuffer textBuffer) {
            ReplAggregateClassifier res;
            if (!textBuffer.Properties.TryGetProperty<ReplAggregateClassifier>(typeof(ReplAggregateClassifier), out res)) {
                res = new ReplAggregateClassifier(_bufferGraphFact, textBuffer);
                textBuffer.Properties.AddProperty(typeof(ReplAggregateClassifier), res);
            }
            return res;
        }

        #endregion
    }
}
