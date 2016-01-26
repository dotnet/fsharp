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
using System.Linq;
using Microsoft.VisualStudio.Debugger.Interop;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudioTools {
    /// <summary>
    /// Registers an exception in the Debug->Exceptions window.
    /// 
    /// Supports hierarchy registration but all elements of the hierarchy also need
    /// to be registered independently (to provide their code/state settings).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    class ProvideDebugExceptionAttribute : RegistrationAttribute {
        // EXCEPTION_STATE flags that are valid for DKM exception entries (directly under the engine key)
        private const enum_EXCEPTION_STATE DkmValidFlags =
            enum_EXCEPTION_STATE.EXCEPTION_STOP_FIRST_CHANCE |
            enum_EXCEPTION_STATE.EXCEPTION_STOP_SECOND_CHANCE |
            enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_FIRST_CHANCE |
            enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_UNCAUGHT;

        private readonly string _engineGuid;
        private readonly string _category;
        private readonly string[] _path;
        private int _code;
        private enum_EXCEPTION_STATE _state;

        public ProvideDebugExceptionAttribute(string engineGuid, string category, params string[] path) {
            _engineGuid = engineGuid;
            _category = category;
            _path = path;
            _state = enum_EXCEPTION_STATE.EXCEPTION_JUST_MY_CODE_SUPPORTED | enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_UNCAUGHT;
        }

        public int Code {
            get {
                return _code;
            }
            set {
                _code = value;
            }
        }

        public enum_EXCEPTION_STATE State {
            get {
                return _state;
            }
            set {
                _state = value;
            }
        }

        public bool BreakByDefault {
            get {
                return _state.HasFlag(enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_UNCAUGHT);
            }
            set {
                if (value) {
                    _state |= enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_UNCAUGHT;
                } else {
                    _state &= ~enum_EXCEPTION_STATE.EXCEPTION_STOP_USER_UNCAUGHT;
                }
            }
        }

        public override void Register(RegistrationAttribute.RegistrationContext context) {
            var engineKey = context.CreateKey("AD7Metrics\\Exception\\" + _engineGuid);

            var key = engineKey.CreateSubkey(_category);
            foreach (var pathElem in _path) {
                key = key.CreateSubkey(pathElem);
            }
            key.SetValue("Code", _code);
            key.SetValue("State", (int)_state);

            string name = _path.LastOrDefault() ?? "*";
            engineKey.SetValue(name, (int)(_state & DkmValidFlags));
        }

        public override void Unregister(RegistrationAttribute.RegistrationContext context) {
        }
    }
}
