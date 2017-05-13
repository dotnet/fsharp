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
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace TestUtilities.Mocks {
    public class MockActivityLog : IVsActivityLog {
        public readonly List<string> Items = new List<string>();

        private readonly static Dictionary<uint, string> ActivityType = new Dictionary<uint, string> {
            { (uint)__ACTIVITYLOG_ENTRYTYPE.ALE_ERROR, "Error" },
            { (uint)__ACTIVITYLOG_ENTRYTYPE.ALE_WARNING, "Warning" },
            { (uint)__ACTIVITYLOG_ENTRYTYPE.ALE_INFORMATION, "Information" }
        };

        public IEnumerable<string> AllItems {
            get {
                return Items.Select(t => Regex.Replace(t, "(\\r\\n|\\r|\\n)", "\\n"));
            }
        }

        public IEnumerable<string> Errors {
            get {
                return Items
                    .Where(t => t.StartsWith("Error"))
                    .Select(t => Regex.Replace(t, "(\\r\\n|\\r|\\n)", "\\n"));
            }
        }

        public IEnumerable<string> ErrorsAndWarnings {
            get {
                return Items
                    .Where(t => t.StartsWith("Error") || t.StartsWith("Warning"))
                    .Select(t => Regex.Replace(t, "(\\r\\n|\\r|\\n)", "\\n"));
            }
        }

        public int LogEntry(uint actType, string pszSource, string pszDescription) {
            var item = string.Format("{0}//{1}//{2}", ActivityType[actType], pszSource, pszDescription);
            Debug.WriteLine(item);
            Items.Add(item);
            return VSConstants.S_OK;
        }

        public int LogEntryGuid(uint actType, string pszSource, string pszDescription, Guid guid) {
            var item = string.Format("{0}//{1}//{2}//{3:B}", ActivityType[actType], pszSource, pszDescription, guid);
            Debug.WriteLine(item);
            Items.Add(item);
            return VSConstants.S_OK;
        }

        public int LogEntryGuidHr(uint actType, string pszSource, string pszDescription, Guid guid, int hr) {
            var item = string.Format("{0}//{1}//{2}//{3:B}//{4:X8}", ActivityType[actType], pszSource, pszDescription, guid, hr);
            Debug.WriteLine(item);
            Items.Add(item);
            return VSConstants.S_OK;
        }

        public int LogEntryGuidHrPath(uint actType, string pszSource, string pszDescription, Guid guid, int hr, string pszPath) {
            var item = string.Format("{0}//{1}//{2}//{3:B}//{4:X8}//{5}", ActivityType[actType], pszSource, pszDescription, guid, hr, pszPath);
            Debug.WriteLine(item);
            Items.Add(item);
            return VSConstants.S_OK;
        }

        public int LogEntryGuidPath(uint actType, string pszSource, string pszDescription, Guid guid, string pszPath) {
            var item = string.Format("{0}//{1}//{2}//{3:B}//{4}", ActivityType[actType], pszSource, pszDescription, guid, pszPath);
            Debug.WriteLine(item);
            Items.Add(item);
            return VSConstants.S_OK;
        }

        public int LogEntryHr(uint actType, string pszSource, string pszDescription, int hr) {
            var item = string.Format("{0}//{1}//{2}//{3:X8}", ActivityType[actType], pszSource, pszDescription, hr);
            Debug.WriteLine(item);
            Items.Add(item);
            return VSConstants.S_OK;
        }

        public int LogEntryHrPath(uint actType, string pszSource, string pszDescription, int hr, string pszPath) {
            var item = string.Format("{0}//{1}//{2}//{3:X8}//{4}", ActivityType[actType], pszSource, pszDescription, hr, pszPath);
            Debug.WriteLine(item);
            Items.Add(item);
            return VSConstants.S_OK;
        }

        public int LogEntryPath(uint actType, string pszSource, string pszDescription, string pszPath) {
            var item = string.Format("{0}//{1}//{2}//{3}", ActivityType[actType], pszSource, pszDescription, pszPath);
            Debug.WriteLine(item);
            Items.Add(item);
            return VSConstants.S_OK;
        }
    }
}
