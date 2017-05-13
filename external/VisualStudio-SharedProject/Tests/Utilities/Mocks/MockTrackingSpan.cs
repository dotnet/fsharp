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
using Microsoft.VisualStudio.Text;

namespace TestUtilities.Mocks {
    public class MockTrackingSpan : ITrackingSpan {
        private readonly int _start, _length;
        private readonly MockTextSnapshot _snapshot;
        private readonly SpanTrackingMode _trackingMode;
        private readonly ITrackingPoint _startPoint, _endPoint;

        public MockTrackingSpan(MockTextSnapshot snapshot, int start, int length, SpanTrackingMode trackingMode = SpanTrackingMode.EdgeExclusive) {
            _start = start;
            _length = length;
            _snapshot = snapshot;
            _trackingMode = trackingMode;
            switch(_trackingMode) {
                case SpanTrackingMode.EdgeExclusive:
                    _startPoint = new MockTrackingPoint(snapshot, start, PointTrackingMode.Positive);
                    _endPoint = new MockTrackingPoint(snapshot, start + length, PointTrackingMode.Negative);
                    break;
                case SpanTrackingMode.EdgeInclusive:
                    _startPoint = new MockTrackingPoint(snapshot, start, PointTrackingMode.Negative);
                    _endPoint = new MockTrackingPoint(snapshot, start + length, PointTrackingMode.Positive);
                    break;
                case SpanTrackingMode.EdgeNegative:
                    _startPoint = new MockTrackingPoint(snapshot, start, PointTrackingMode.Negative);
                    _endPoint = new MockTrackingPoint(snapshot, start + length, PointTrackingMode.Negative);
                    break;
                case SpanTrackingMode.EdgePositive:
                    _startPoint = new MockTrackingPoint(snapshot, start, PointTrackingMode.Positive);
                    _endPoint = new MockTrackingPoint(snapshot, start + length, PointTrackingMode.Positive);
                    break;
            }
        }

        public SnapshotPoint GetEndPoint(ITextSnapshot snapshot) {
            return new SnapshotPoint(_snapshot, _start + _length);
        }

        public Span GetSpan(ITextVersion version) {
            return Span.FromBounds(
                _startPoint.GetPosition(version),
                _endPoint.GetPosition(version)
            );
        }

        public SnapshotSpan GetSpan(ITextSnapshot snapshot) {
            return new SnapshotSpan(snapshot, GetSpan(snapshot.Version));
        }

        public SnapshotPoint GetStartPoint(ITextSnapshot snapshot) {
            var span = GetSpan(snapshot.Version);
            return new SnapshotPoint(snapshot, span.Start);
        }

        public string GetText(ITextSnapshot snapshot) {
            var span = GetSpan(snapshot.Version);
            return snapshot.GetText(span);
        }

        public ITextBuffer TextBuffer {
            get { return _snapshot.TextBuffer; }
        }

        public TrackingFidelityMode TrackingFidelity {
            get { throw new NotImplementedException(); }
        }

        public SpanTrackingMode TrackingMode {
            get { return _trackingMode; }
        }
    }
}
