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
using System.Diagnostics;
using System.Text;
using System.Windows.Threading;
using Microsoft.VisualStudio.Text;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    internal sealed class OutputBuffer : IDisposable {
        private readonly DispatcherTimer _timer;
        private int _maxSize;
        private readonly object _lock;
        private readonly List<OutputEntry> _outputEntries = new List<OutputEntry>();
        private int _bufferLength;
        private long _lastFlush;
        private static readonly Stopwatch _stopwatch;
        private readonly ReplWindow _window;
        private const int _initialMaxSize = 1024;
        private bool _processAnsiEscapes;
        private ConsoleColor _outColor = ConsoleColor.Black, _errColor = ConsoleColor.Red;

        static OutputBuffer() {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public OutputBuffer(ReplWindow window) {
            _maxSize = _initialMaxSize;
            _lock = new object();
            _timer = new DispatcherTimer();
            _timer.Tick += (sender, args) => Flush();
            _timer.Interval = TimeSpan.FromMilliseconds(400);
            _window = window;

            ResetColors();
        }

        public void ResetColors() {
            _outColor = ConsoleColor.Black; 
            _errColor = ConsoleColor.Red;
        }

        public void Write(string text, bool isError = false) {
            bool needsFlush = false;
            lock (_lock) {
                int escape;
                if (_processAnsiEscapes && (escape = text.IndexOf('\x1b')) != -1) {
                    AppendEscapedText(text, isError, escape);
                } else {
                    AppendText(text, isError ? OutputEntryKind.StdErr : OutputEntryKind.StdOut, isError ? _errColor : _outColor);
                }

                _bufferLength += text.Length;
                needsFlush = (_bufferLength > _maxSize);
                if (!needsFlush && !_timer.IsEnabled) {
                    _timer.IsEnabled = true;
                }
            }
            if (needsFlush) {
                Flush();
            }
        }

        private void AppendEscapedText(string text, bool isError, int escape) {
            OutputEntryKind kind = isError ? OutputEntryKind.StdErr : OutputEntryKind.StdOut;
            ConsoleColor color = isError ? _errColor : _outColor;

            // http://en.wikipedia.org/wiki/ANSI_escape_code
            // process any ansi color sequences...

            int start = 0;
            List<int> codes = new List<int>();
            do {
                if (escape != start) {
                    // add unescaped text
                    AppendText(text.Substring(start, escape - start), kind, color);
                }

                // process the escape sequence                
                if (escape < text.Length - 1 && text[escape + 1] == '[') {
                    // We have the Control Sequence Introducer (CSI) - ESC [

                    codes.Clear();
                    int? value = 0;

                    for (int i = escape + 2; i < text.Length; i++) { // skip esc + [
                        if (text[i] >= '0' && text[i] <= '9') {
                            // continue parsing the integer...
                            if (value == null) {
                                value = 0;
                            }
                            value = 10 * value.Value + (text[i] - '0');
                        } else if (text[i] == ';') {
                            if (value != null) {
                                codes.Add(value.Value);
                                value = null;
                            } else {
                                // CSI ; - invalid or CSI ### ;;, both invalid
                                break;
                            }
                        } else if (text[i] == 'm') {
                            if (value != null) {
                                codes.Add(value.Value);
                            }

                            // parsed a valid code
                            start = i + 1;
                            if (codes.Count == 0) {
                                // reset
                                color = isError ? ConsoleColor.Red : ConsoleColor.White;
                            } else {
                                for (int j = 0; j < codes.Count; j++) {
                                    switch (codes[j]) {
                                        case 0: color = ConsoleColor.White; break;
                                        case 1: // bright/bold
                                            color |= ConsoleColor.DarkGray;
                                            break;
                                        case 2: // faint

                                        case 3: // italic
                                        case 4: // single underline
                                            break;
                                        case 5: // blink slow
                                        case 6: // blink fast
                                            break;
                                        case 7: // negative
                                        case 8: // conceal
                                        case 9: // crossed out
                                        case 10: // primary font
                                        case 11: // 11-19, n-th alternate font
                                            break;
                                        case 21: // bright/bold off 
                                            color &= ~ConsoleColor.DarkGray;
                                            break;
                                        case 22: // normal intensity
                                        case 24: // underline off
                                            break;
                                        case 25: // blink off
                                            break;
                                        case 27: // image - postive
                                        case 28: // reveal
                                        case 29: // not crossed out
                                        case 30: color = ConsoleColor.Black | (color & ConsoleColor.DarkGray); break;
                                        case 31: color = ConsoleColor.DarkRed | (color & ConsoleColor.DarkGray); break;
                                        case 32: color = ConsoleColor.DarkGreen | (color & ConsoleColor.DarkGray); break;
                                        case 33: color = ConsoleColor.DarkYellow | (color & ConsoleColor.DarkGray); break;
                                        case 34: color = ConsoleColor.DarkBlue | (color & ConsoleColor.DarkGray); break;
                                        case 35: color = ConsoleColor.DarkMagenta | (color & ConsoleColor.DarkGray); break;
                                        case 36: color = ConsoleColor.DarkCyan | (color & ConsoleColor.DarkGray); break;
                                        case 37: color = ConsoleColor.Gray | (color & ConsoleColor.DarkGray); break;
                                        case 38: // xterm 286 background color
                                        case 39: // default text color
                                            color = _outColor;
                                            break;
                                        case 40: // background colors
                                        case 41: 
                                        case 42: 
                                        case 43: 
                                        case 44: 
                                        case 45: 
                                        case 46:
                                        case 47: break;
                                        case 90: color = ConsoleColor.DarkGray; break;
                                        case 91: color = ConsoleColor.Red; break;
                                        case 92: color = ConsoleColor.Green; break;
                                        case 93: color = ConsoleColor.Yellow; break;
                                        case 94: color = ConsoleColor.Blue; break;
                                        case 95: color = ConsoleColor.Magenta; break;
                                        case 96: color = ConsoleColor.Cyan; break;
                                        case 97: color = ConsoleColor.White; break;
                                    }
                                }
                            }
                            break;
                        } else {
                            // unknown char, invalid escape
                            break;
                        }
                    }

                    escape = text.IndexOf('\x1b', escape + 1);
                }// else not an escape sequence, process as text

            } while (escape != -1);
            if (start != text.Length) {
                AppendText(text.Substring(start), kind, color);
            }
        }

        private void AppendText(string text, OutputEntryKind kind, ConsoleColor color) {
            var newProps = new OutputEntryProperties(kind, color);
            if (_outputEntries.Count == 0 || _outputEntries[_outputEntries.Count - 1].Properties != newProps) {
                _outputEntries.Add(new OutputEntry(newProps));
            }
            var buffer = _outputEntries[_outputEntries.Count - 1].Buffer;
            buffer.Append(text);
        }

        public bool ProcessAnsiEscapes {
            get { return _processAnsiEscapes; }
            set { _processAnsiEscapes = value; }
        }

        /// <summary>
        /// Flushes the buffer, should always be called from the UI thread.
        /// </summary>
        public void Flush() {
            // if we're rapidly outputting grow the output buffer.
            long curTime = _stopwatch.ElapsedMilliseconds;
            if (curTime - _lastFlush < 1000) {
                if (_maxSize < 1024 * 1024) {
                    _maxSize *= 2;
                }
            }
            _lastFlush = _stopwatch.ElapsedMilliseconds;

            OutputEntry[] entries;
            lock (_lock) {
                entries = _outputEntries.ToArray();

                _outputEntries.Clear();
                _bufferLength = 0;
                _timer.IsEnabled = false;
            }

            if (entries.Length > 0) {
                var colors = new List<ColoredSpan>();
                var text = new StringBuilder();
                foreach (var entry in entries) {
                    int start = text.Length;
                    text.Append(entry.Buffer.ToString());
                    colors.Add(new ColoredSpan(new Span(start, entry.Buffer.Length), entry.Properties.Color));
                }
                _window.AppendOutput(colors, text.ToString());

                _window.TextView.Caret.EnsureVisible();
            }
        }

        public void Dispose() {
            _timer.IsEnabled = false;
        }

        struct OutputEntry {
            public readonly StringBuilder Buffer;
            public readonly OutputEntryProperties Properties;

            public OutputEntry(OutputEntryProperties properties) {
                Properties = properties;
                Buffer = new StringBuilder();
            }
        }

        /// <summary>
        /// Properties for a run of text - includes destination (stdout/stderr) and color
        /// </summary>
        struct OutputEntryProperties {
            public readonly OutputEntryKind Kind;
            public readonly ConsoleColor Color;

            public OutputEntryProperties(OutputEntryKind kind, ConsoleColor color) {
                Kind = kind;
                Color = color;
            }

            public static bool operator ==(OutputEntryProperties l, OutputEntryProperties r) {
                return l.Kind == r.Kind && l.Color == r.Color; 
            }

            public static bool operator !=(OutputEntryProperties l, OutputEntryProperties r) {
                return l.Kind != r.Kind || l.Color != r.Color;
            }

            public override bool Equals(object obj) {
                if (!(obj is OutputEntryProperties)) {
                    return false;
                }

                var other = (OutputEntryProperties)obj;
                return other == this;
            }

            public override int GetHashCode() {
                return (int)Kind  ^ ((int)Color) << 1;
            }
        }

        enum OutputEntryKind {
            StdOut,
            StdErr
        }
    }
}
