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
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudioTools {
    /// <summary>
    /// Wraps a <see cref="WebSocket"/> instance and exposes its interface as a generic <see cref="Stream"/>.
    /// </summary>
    internal class WebSocketStream : Stream {
        private readonly WebSocket _webSocket;
        private bool _ownsSocket;

        public WebSocketStream(WebSocket webSocket, bool ownsSocket = false) {
            _webSocket = webSocket;
            _ownsSocket = ownsSocket;
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            if (disposing && _ownsSocket) {
                _webSocket.Dispose();
            }
        }

        public override bool CanRead {
            get { return true; }
        }

        public override bool CanWrite {
            get { return true; }
        }

        public override bool CanSeek {
            get { return false; }
        }

        public override void Flush() {
        }

        public override Task FlushAsync(CancellationToken cancellationToken) {
            return Task.FromResult(true);
        }

        public override int Read(byte[] buffer, int offset, int count) {
            return ReadAsync(buffer, offset, count).GetAwaiter().GetResult();
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
            try {
                return (await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer, offset, count), cancellationToken).ConfigureAwait(false)).Count;
            } catch (WebSocketException ex) {
                throw new IOException(ex.Message, ex);
            }
        }

        public override void Write(byte[] buffer, int offset, int count) {
            WriteAsync(buffer, offset, count).GetAwaiter().GetResult();
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
            try {
                await _webSocket.SendAsync(new ArraySegment<byte>(buffer, offset, count), WebSocketMessageType.Binary, true, cancellationToken).ConfigureAwait(false);
            } catch (WebSocketException ex) {
                throw new IOException(ex.Message, ex);
            }
        }

        public override long Length {
            get { throw new NotSupportedException(); }
        }

        public override long Position {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override void SetLength(long value) {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin) {
            throw new NotSupportedException();
        }
    }
}
