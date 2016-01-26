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
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace TestUtilities.Mocks {
    public class MockTextViewModel : ITextViewModel {
        public ITextBuffer DataBuffer { get; set; }

        public ITextDataModel DataModel {
            get { throw new NotImplementedException(); }
        }

        public ITextBuffer EditBuffer { get; set; }

        public SnapshotPoint GetNearestPointInVisualBuffer(SnapshotPoint editBufferPoint) {
            throw new NotImplementedException();
        }

        public SnapshotPoint GetNearestPointInVisualSnapshot(SnapshotPoint editBufferPoint, ITextSnapshot targetVisualSnapshot, PointTrackingMode trackingMode) {
            throw new NotImplementedException();
        }

        public bool IsPointInVisualBuffer(SnapshotPoint editBufferPoint, PositionAffinity affinity) {
            throw new NotImplementedException();
        }

        public ITextBuffer VisualBuffer {
            get { throw new NotImplementedException(); }
        }

        public PropertyCollection Properties {
            get { throw new NotImplementedException(); }
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}
