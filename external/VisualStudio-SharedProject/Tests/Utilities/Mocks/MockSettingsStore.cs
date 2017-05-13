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
using System.Linq;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace TestUtilities.Mocks {
    public class MockSettingsStore : IVsSettingsStore, IVsWritableSettingsStore {
        private readonly List<Tuple<string, string, object>> Settings = new List<Tuple<string, string, object>>();

        public void AddSetting(string path, string name, object value) {
            if (string.IsNullOrEmpty(path)) {
                path = Settings.Last().Item1;
            } else if (path.StartsWith(" ")) {
                path = Settings.Last().Item1 + "\\" + path.TrimStart();
            }

            Settings.Add(Tuple.Create(path, name, value));
        }

        public void Clear() {
            Settings.Clear();
        }

        private int FindValue<T>(string collectionPath, string propertyName, ref T value) {
            var tuple = Settings.FirstOrDefault(t => t.Item1 == collectionPath && t.Item2 == propertyName);
            if (tuple == null) {
                return VSConstants.S_FALSE;
            }
            if (!(tuple.Item3 is T)) {
                return VSConstants.E_INVALIDARG;
            }
            value = (T)tuple.Item3;
            return VSConstants.S_OK;
        }

        public int CollectionExists(string collectionPath, out int pfExists) {
            var collectionSeq = collectionPath.Split('\\');
            pfExists = Settings
                .Select(t => t.Item1.Split('\\'))
                .Where(seq => seq.Length >= collectionSeq.Length)
                .Any(seq => seq.Take(collectionSeq.Length).SequenceEqual(collectionSeq)) ? 1 : 0;
            return VSConstants.S_OK;
        }

        public int GetBinary(string collectionPath, string propertyName, uint byteLength, byte[] pBytes = null, uint[] actualByteLength = null) {
            throw new NotImplementedException();
        }

        public int GetBool(string collectionPath, string propertyName, out int value) {
            return GetBoolOrDefault(collectionPath, propertyName, 0, out value);
        }

        public int GetBoolOrDefault(string collectionPath, string propertyName, int defaultValue, out int value) {
            bool res = (defaultValue != 0);
            var hr = FindValue(collectionPath, propertyName, ref res);
            value = res ? 1 : 0;
            return hr;
        }

        public int GetInt(string collectionPath, string propertyName, out int value) {
            value = 0;
            return FindValue(collectionPath, propertyName, ref value);
        }

        public int GetInt64(string collectionPath, string propertyName, out long value) {
            value = 0;
            return FindValue(collectionPath, propertyName, ref value);
        }

        public int GetInt64OrDefault(string collectionPath, string propertyName, long defaultValue, out long value) {
            value = defaultValue;
            return FindValue(collectionPath, propertyName, ref value);
        }

        public int GetIntOrDefault(string collectionPath, string propertyName, int defaultValue, out int value) {
            value = defaultValue;
            return FindValue(collectionPath, propertyName, ref value);
        }

        public int GetLastWriteTime(string collectionPath, SYSTEMTIME[] lastWriteTime) {
            throw new NotImplementedException();
        }

        public int GetPropertyCount(string collectionPath, out uint propertyCount) {
            propertyCount = (uint)Settings.Count(t => t.Item1 == collectionPath);
            return VSConstants.S_OK;
        }

        public int GetPropertyName(string collectionPath, uint index, out string propertyName) {
            try {
                propertyName = Settings.Where(t => t.Item1 == collectionPath).ElementAt((int)index).Item2;
                return VSConstants.S_OK;
            } catch (InvalidOperationException) {
                propertyName = null;
                return VSConstants.E_INVALIDARG;
            }
        }

        public int GetPropertyType(string collectionPath, string propertyName, out uint type) {
            throw new NotImplementedException();
        }

        public int GetString(string collectionPath, string propertyName, out string value) {
            value = null;
            return FindValue(collectionPath, propertyName, ref value);
        }

        public int GetStringOrDefault(string collectionPath, string propertyName, string defaultValue, out string value) {
            value = defaultValue;
            return FindValue(collectionPath, propertyName, ref value);
        }

        public int GetSubCollectionCount(string collectionPath, out uint subCollectionCount) {
            var collectionSeq = collectionPath.Split('\\');
            if (!Settings
                .Select(t => t.Item1.Split('\\'))
                .Where(seq => seq.Length >= collectionSeq.Length)
                .Where(seq => seq.Take(collectionSeq.Length).SequenceEqual(collectionSeq))
                .Any()) {
                subCollectionCount = 0;
                return VSConstants.E_INVALIDARG;
            }
            
            subCollectionCount = (uint)Settings
                .Select(t => t.Item1.Split('\\'))
                .Where(seq => seq.Length > collectionSeq.Length)
                .Where(seq => seq.Take(collectionSeq.Length).SequenceEqual(collectionSeq))
                .Select(seq => seq[collectionSeq.Length])
                .Distinct()
                .Count();
            return VSConstants.S_OK;
        }

        public int GetSubCollectionName(string collectionPath, uint index, out string subCollectionName) {
            var collectionSeq = collectionPath.Split('\\');
            if (!Settings
                .Select(t => t.Item1.Split('\\'))
                .Where(seq => seq.Length >= collectionSeq.Length)
                .Where(seq => seq.Take(collectionSeq.Length).SequenceEqual(collectionSeq))
                .Any()) {
                subCollectionName = null;
                return VSConstants.E_INVALIDARG;
            }

            subCollectionName = Settings
                .Select(t => t.Item1.Split('\\'))
                .Where(seq => seq.Length > collectionSeq.Length)
                .Where(seq => seq.Take(collectionSeq.Length).SequenceEqual(collectionSeq))
                .Select(seq => seq[collectionSeq.Length])
                .Distinct()
                .ElementAt((int)index);
            return VSConstants.S_OK;
        }

        public int GetUnsignedInt(string collectionPath, string propertyName, out uint value) {
            value = 0;
            return FindValue(collectionPath, propertyName, ref value);
        }

        public int GetUnsignedInt64(string collectionPath, string propertyName, out ulong value) {
            value = 0;
            return FindValue(collectionPath, propertyName, ref value);
        }

        public int GetUnsignedInt64OrDefault(string collectionPath, string propertyName, ulong defaultValue, out ulong value) {
            value = defaultValue;
            return FindValue(collectionPath, propertyName, ref value);
        }

        public int GetUnsignedIntOrDefault(string collectionPath, string propertyName, uint defaultValue, out uint value) {
            value = defaultValue;
            return FindValue(collectionPath, propertyName, ref value);
        }

        public int PropertyExists(string collectionPath, string propertyName, out int pfExists) {
            pfExists = Settings.Any(t => t.Item1 == collectionPath && t.Item2 == propertyName) ? 1 : 0;
            return VSConstants.S_OK;
        }


        public int CreateCollection(string collectionPath) {
            int exists;
            int hr = CollectionExists(collectionPath, out exists);
            if (ErrorHandler.Failed(hr)) {
                return hr;
            }
            if (exists == 0) {
                AddSetting(collectionPath, string.Empty, null);
            }
            return VSConstants.S_OK;
        }

        public int DeleteCollection(string collectionPath) {
            if (string.IsNullOrEmpty(collectionPath)) {
                return VSConstants.S_FALSE;
            }

            var collectionSeq = collectionPath.Split('\\');
            return Settings.RemoveAll(t => {
                var seq = t.Item1.Split('\\');
                return seq.Length >= collectionSeq.Length &&
                    seq.Take(collectionSeq.Length).SequenceEqual(collectionSeq);
            }) > 0 ? VSConstants.S_OK : VSConstants.S_FALSE;
        }

        public int DeleteProperty(string collectionPath, string propertyName) {
            if (string.IsNullOrEmpty(collectionPath) || string.IsNullOrEmpty(propertyName)) {
                return VSConstants.S_FALSE;
            }

            var collectionSeq = collectionPath.Split('\\');
            return Settings.RemoveAll(t => t.Item1 == collectionPath && t.Item2 == propertyName) > 0 ?
                VSConstants.S_OK :
                VSConstants.S_FALSE;
        }

        public int SetBinary(string collectionPath, string propertyName, uint byteLength, byte[] pBytes) {
            if (DeleteProperty(collectionPath, propertyName) == VSConstants.S_FALSE) {
                int exists;
                if (ErrorHandler.Failed(CollectionExists(collectionPath, out exists)) || exists == 0) {
                    return VSConstants.E_INVALIDARG;
                }
            }
            Settings.Add(Tuple.Create(collectionPath, propertyName, (object)pBytes));
            return VSConstants.S_OK;
        }

        public int SetBool(string collectionPath, string propertyName, int value) {
            if (DeleteProperty(collectionPath, propertyName) == VSConstants.S_FALSE) {
                int exists;
                if (ErrorHandler.Failed(CollectionExists(collectionPath, out exists)) || exists == 0) {
                    return VSConstants.E_INVALIDARG;
                }
            }
            Settings.Add(Tuple.Create(collectionPath, propertyName, (object)value));
            return VSConstants.S_OK;
        }

        public int SetInt(string collectionPath, string propertyName, int value) {
            if (DeleteProperty(collectionPath, propertyName) == VSConstants.S_FALSE) {
                int exists;
                if (ErrorHandler.Failed(CollectionExists(collectionPath, out exists)) || exists == 0) {
                    return VSConstants.E_INVALIDARG;
                }
            }
            Settings.Add(Tuple.Create(collectionPath, propertyName, (object)value));
            return VSConstants.S_OK;
        }

        public int SetInt64(string collectionPath, string propertyName, long value) {
            if (DeleteProperty(collectionPath, propertyName) == VSConstants.S_FALSE) {
                int exists;
                if (ErrorHandler.Failed(CollectionExists(collectionPath, out exists)) || exists == 0) {
                    return VSConstants.E_INVALIDARG;
                }
            }
            Settings.Add(Tuple.Create(collectionPath, propertyName, (object)value));
            return VSConstants.S_OK;
        }

        public int SetString(string collectionPath, string propertyName, string value) {
            if (DeleteProperty(collectionPath, propertyName) == VSConstants.S_FALSE) {
                int exists;
                if (ErrorHandler.Failed(CollectionExists(collectionPath, out exists)) || exists == 0) {
                    return VSConstants.E_INVALIDARG;
                }
            }
            Settings.Add(Tuple.Create(collectionPath, propertyName, (object)value));
            return VSConstants.S_OK;
        }

        public int SetUnsignedInt(string collectionPath, string propertyName, uint value) {
            if (DeleteProperty(collectionPath, propertyName) == VSConstants.S_FALSE) {
                int exists;
                if (ErrorHandler.Failed(CollectionExists(collectionPath, out exists)) || exists == 0) {
                    return VSConstants.E_INVALIDARG;
                }
            }
            Settings.Add(Tuple.Create(collectionPath, propertyName, (object)value));
            return VSConstants.S_OK;
        }

        public int SetUnsignedInt64(string collectionPath, string propertyName, ulong value) {
            if (DeleteProperty(collectionPath, propertyName) == VSConstants.S_FALSE) {
                int exists;
                if (ErrorHandler.Failed(CollectionExists(collectionPath, out exists)) || exists == 0) {
                    return VSConstants.E_INVALIDARG;
                }
            }
            Settings.Add(Tuple.Create(collectionPath, propertyName, (object)value));
            return VSConstants.S_OK;
        }
    }
}
