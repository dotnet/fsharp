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
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;

namespace Microsoft.VisualStudioTools {
    public abstract class WebSocketProxyBase : IHttpHandler {
        private static long _lastId;
        private static Task _currentSession; // represents the current active debugging session, and completes when it is over
        private static volatile StringWriter _log;

        private readonly long _id;

        public WebSocketProxyBase() {
            _id = Interlocked.Increment(ref _lastId);
        }

        public abstract int DebuggerPort { get; }

        public abstract bool AllowConcurrentConnections { get; }

        public abstract void ProcessHelpPageRequest(HttpContext context);

        public bool IsReusable {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context) {
            if (context.IsWebSocketRequest) {
                context.AcceptWebSocketRequest(WebSocketRequestHandler);
            } else {
                context.Response.ContentType = "text/html";
                context.Response.ContentEncoding = Encoding.UTF8;

                switch (context.Request.QueryString["debug"]) {
                    case "startlog":
                        _log = new StringWriter();
                        context.Response.Write("Logging is now enabled. <a href='?debug=viewlog'>View</a>. <a href='?debug=stoplog'>Disable</a>.");
                        return;

                    case "stoplog":
                        _log = null;
                        context.Response.Write("Logging is now disabled. <a href='?debug=startlog'>Enable</a>.");
                        return;

                    case "clearlog": {
                            var log = _log;
                            if (log != null) {
                                log.GetStringBuilder().Clear();
                            }
                            context.Response.Write("Log is cleared. <a href='?debug=viewlog'>View</a>.");
                            return;
                        }

                    case "viewlog": {
                            var log = _log;
                            if (log == null) {
                                context.Response.Write("Logging is disabled. <a href='?debug=startlog'>Enable</a>.");
                            } else {
                                context.Response.Write("Logging is enabled. <a href='?debug=clearlog'>Clear</a>. <a href='?debug=stoplog'>Disable</a>. <p><pre>");
                                context.Response.Write(HttpUtility.HtmlDecode(log.ToString()));
                                context.Response.Write("</pre>");
                            }
                            context.Response.End();
                            return;
                        }
                }

                ProcessHelpPageRequest(context);
            }
        }

        private async Task WebSocketRequestHandler(AspNetWebSocketContext context) {
            Log("Accepted web socket request from {0}.", context.UserHostAddress);

            TaskCompletionSource<bool> tcs = null;
            if (!AllowConcurrentConnections) {
                tcs = new TaskCompletionSource<bool>();
                while (true) {
                    var currentSession = Interlocked.CompareExchange(ref _currentSession, tcs.Task, null);
                    if (currentSession == null) {
                        break;
                    }
                    Log("Another session is active, waiting for completion.");
                    await currentSession;
                    Log("The other session completed, proceeding.");
                }
            }

            try {
                var webSocket = context.WebSocket;
                using (var tcpClient = new TcpClient("localhost", DebuggerPort)) {
                    try {
                        var stream = tcpClient.GetStream();
                        var cts = new CancellationTokenSource();

                        // Start the workers that copy data from one socket to the other in both directions, and wait until either
                        // completes. The workers are fully async, and so their loops are transparently interleaved when running.
                        // Usually end of session is caused by VS dropping its connection on detach, and so it will be
                        // CopyFromWebSocketToStream that returns first; but it can be the other one if debuggee process crashes.
                        Log("Starting copy workers.");
                        var copyFromStreamToWebSocketTask = CopyFromStreamToWebSocketWorker(stream, webSocket, cts.Token);
                        var copyFromWebSocketToStreamTask = CopyFromWebSocketToStreamWorker(webSocket, stream, cts.Token);
                        Task completedTask = null;
                        try {
                            completedTask = await Task.WhenAny(copyFromStreamToWebSocketTask, copyFromWebSocketToStreamTask);
                        } catch (IOException ex) {
                            Log(ex);
                        } catch (WebSocketException ex) {
                            Log(ex);
                        }

                        // Now that one worker is done, try to gracefully terminate the other one by issuing a cancellation request.
                        // it is normally blocked on a read, and this will cancel it if possible, and throw OperationCanceledException.
                        Log("One of the workers completed, shutting down the remaining one.");
                        cts.Cancel();
                        try {
                            await Task.WhenAny(Task.WhenAll(copyFromStreamToWebSocketTask, copyFromWebSocketToStreamTask), Task.Delay(1000));
                        } catch (OperationCanceledException ex) {
                            Log(ex);
                        }

                        // Try to gracefully close the websocket if it's still open - this is not necessary, but nice to have.
                        Log("Both workers shut down, trying to close websocket.");
                        try {
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                        } catch (WebSocketException ex) {
                            Log(ex);
                        }
                    } finally {
                        // Gracefully close the TCP socket. This is crucial to avoid "Remote debugger already attached" problems.
                        Log("Shutting down TCP socket.");
                        try {
                            tcpClient.Client.Shutdown(SocketShutdown.Both);
                            tcpClient.Client.Disconnect(false);
                        } catch (SocketException ex) {
                            Log(ex);
                        }
                        Log("All done!");
                    }
                }
            } finally {
                if (tcs != null) {
                    Volatile.Write(ref _currentSession, null);
                    tcs.SetResult(true);
                }
            }
        }

        private async Task CopyFromStreamToWebSocketWorker(Stream stream, WebSocket webSocket, CancellationToken ct) {
            var buffer = new byte[0x10000];
            while (webSocket.State == WebSocketState.Open) {
                ct.ThrowIfCancellationRequested();

                Log("TCP -> WS: waiting for packet.");
                int count = await stream.ReadAsync(buffer, 0, buffer.Length, ct);
                Log("TCP -> WS: received packet:\n{0}", Encoding.UTF8.GetString(buffer, 0, count));

                if (count == 0) {
                    Log("TCP -> WS: zero-length TCP packet received, connection closed.");
                    break;
                }

                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, count), WebSocketMessageType.Binary, true, ct);
                Log("TCP -> WS: packet relayed.");
            }
        }

        private async Task CopyFromWebSocketToStreamWorker(WebSocket webSocket, Stream stream, CancellationToken ct) {
            var buffer = new ArraySegment<byte>(new byte[0x10000]);
            while (webSocket.State == WebSocketState.Open) {
                ct.ThrowIfCancellationRequested();

                Log("WS -> TCP: waiting for packet.");
                var recv = await webSocket.ReceiveAsync(buffer, ct);
                Log("WS -> TCP: received packet:\n{0}", Encoding.UTF8.GetString(buffer.Array, 0, recv.Count));

                await stream.WriteAsync(buffer.Array, 0, recv.Count, ct);
                Log("WS -> TCP: packet relayed.");
            }
        }

        private void Log(object o) {
            var log = _log;
            if (log != null) {
                log.WriteLine(_id + " :: " + o);
            }
        }

        private void Log(string format, object arg1) {
            var log = _log;
            if (log != null) {
                log.WriteLine(_id + " :: " + format, arg1);
            }
        }
    }
}

