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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.IncrementalSearch;
using TestUtilities.Mocks;

namespace Microsoft.VisualStudioTools.MockVsTests {
    [Export(typeof(IIncrementalSearchFactoryService))]
    class MockIncrementalSearchFactoryService : IIncrementalSearchFactoryService {
        public IIncrementalSearch GetIncrementalSearch(ITextView textView) {
            return new MockIncrementalSearch((MockTextView)textView);
        }
    }
}
