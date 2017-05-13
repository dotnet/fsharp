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
using Microsoft.VisualStudio.Text.Projection;

namespace TestUtilities.Mocks {
    public class MockMappingPoint : IMappingPoint {
        private readonly ITrackingPoint _trackingPoint;

        public MockMappingPoint(ITrackingPoint trackingPoint) {
            _trackingPoint = trackingPoint;
        }

        public ITextBuffer AnchorBuffer {
            get { throw new NotImplementedException(); }
        }

        public IBufferGraph BufferGraph {
            get { throw new NotImplementedException(); }
        }

        public SnapshotPoint? GetInsertionPoint(Predicate<ITextBuffer> match) {
            throw new NotImplementedException();
        }

        public SnapshotPoint? GetPoint(Predicate<ITextBuffer> match, PositionAffinity affinity) {
            throw new NotImplementedException();
        }

        public SnapshotPoint? GetPoint(ITextSnapshot targetSnapshot, PositionAffinity affinity) {
            try {
                return _trackingPoint.GetPoint(targetSnapshot);
            } catch (ArgumentException) {
                return null;
            }
        }

        public SnapshotPoint? GetPoint(ITextBuffer targetBuffer, PositionAffinity affinity) {
            return GetPoint(targetBuffer.CurrentSnapshot, affinity);
        }
    }
}
