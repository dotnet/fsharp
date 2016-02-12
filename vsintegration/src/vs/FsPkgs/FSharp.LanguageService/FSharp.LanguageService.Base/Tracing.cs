// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.FSharp.LanguageService {
    internal static class Tracing {

        [ConditionalAttribute("LANGTRACE")]
        public static void TraceRef(object obj, string msg) {
            if (obj == null) return;
            IntPtr pUnk = Marshal.GetIUnknownForObject(obj);
            obj = null;
            Marshal.Release(pUnk);
            TraceRef(pUnk, msg);
        }
        [ConditionalAttribute("LANGTRACE")]
        public static void TraceRef(IntPtr pUnk, string msg) {
            GC.Collect(); // collect any outstanding RCW or CCW's.
            if (pUnk == IntPtr.Zero) return;
            Marshal.AddRef(pUnk);
            int count = Marshal.Release(pUnk);
            Trace.WriteLine(msg + ": 0x" + pUnk.ToString("x") + "(ref=" + count + ")");
        }
    }
}
