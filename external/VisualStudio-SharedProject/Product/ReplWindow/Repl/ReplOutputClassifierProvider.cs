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
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    /// <summary>
    /// Provides the classifier for our repl error output buffer.
    /// </summary>
    [Export(typeof(IClassifierProvider)), ContentType(ReplConstants.ReplOutputContentTypeName)]
    class ReplOutputClassifierProvider : IClassifierProvider {
        internal readonly Dictionary<ConsoleColor, IClassificationType> _classTypes = new Dictionary<ConsoleColor, IClassificationType>();

        [ImportingConstructor]
        public ReplOutputClassifierProvider(IClassificationTypeRegistryService classificationService) {
            _classTypes[ConsoleColor.Black] = classificationService.GetClassificationType(InteractiveBlackFormatDefinition.Name);            
            _classTypes[ConsoleColor.DarkBlue] = classificationService.GetClassificationType(InteractiveDarkBlueFormatDefinition.Name);
            _classTypes[ConsoleColor.DarkGreen] = classificationService.GetClassificationType(InteractiveDarkGreenFormatDefinition.Name);
            _classTypes[ConsoleColor.DarkCyan] = classificationService.GetClassificationType(InteractiveDarkCyanFormatDefinition.Name);
            _classTypes[ConsoleColor.DarkRed] = classificationService.GetClassificationType(InteractiveDarkRedFormatDefinition.Name);
            _classTypes[ConsoleColor.DarkMagenta] = classificationService.GetClassificationType(InteractiveDarkMagentaFormatDefinition.Name);
            _classTypes[ConsoleColor.DarkYellow] = classificationService.GetClassificationType(InteractiveDarkYellowFormatDefinition.Name);
            _classTypes[ConsoleColor.Gray] = classificationService.GetClassificationType(InteractiveGrayFormatDefinition.Name);
            _classTypes[ConsoleColor.DarkGray] = classificationService.GetClassificationType(InteractiveDarkGrayFormatDefinition.Name);
            _classTypes[ConsoleColor.Blue] = classificationService.GetClassificationType(InteractiveBlueFormatDefinition.Name);
            _classTypes[ConsoleColor.Green] = classificationService.GetClassificationType(InteractiveGreenFormatDefinition.Name);
            _classTypes[ConsoleColor.Cyan] = classificationService.GetClassificationType(InteractiveCyanFormatDefinition.Name);
            _classTypes[ConsoleColor.Red] = classificationService.GetClassificationType(InteractiveRedFormatDefinition.Name);
            _classTypes[ConsoleColor.Magenta] = classificationService.GetClassificationType(InteractiveMagentaFormatDefinition.Name);
            _classTypes[ConsoleColor.Yellow] = classificationService.GetClassificationType(InteractiveYellowFormatDefinition.Name);
            _classTypes[ConsoleColor.White] = classificationService.GetClassificationType(InteractiveWhiteFormatDefinition.Name);
        }

        #region IClassifierProvider Members

        public IClassifier GetClassifier(ITextBuffer textBuffer) {
            return new ReplOutputClassifier(this, textBuffer);
        }

        #endregion
    }
}
