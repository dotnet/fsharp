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
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Win32;

#if NTVS_FEATURE_INTERACTIVEWINDOW
using Microsoft.VisualStudio;
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    [Export(typeof(IReplWindowProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal sealed class ReplWindowProvider : IReplWindowProvider {
        private readonly IReplEvaluatorProvider[] _evaluators;
        private readonly Dictionary<int, ReplWindow> _windows = new Dictionary<int, ReplWindow>();

        [ImportingConstructor]
        public ReplWindowProvider([ImportMany]IReplEvaluatorProvider[] evaluators) {
            _evaluators = evaluators;
        }

        #region IReplWindowProvider Members

        public IReplWindow CreateReplWindow(IContentType contentType, string/*!*/ title, Guid languageServiceGuid, string replId) {
            int curId = 0;

            ReplWindow window;
            do {
                curId++;
                window = FindReplWindowInternal(curId);
            } while (window != null);

            foreach (var provider in _evaluators) {
                var evaluator = provider.GetEvaluator(replId);
                if (evaluator != null) {
                    string[] roles = evaluator.GetType().GetCustomAttributes(typeof(ReplRoleAttribute), true).Select(r => ((ReplRoleAttribute)r).Name).ToArray();
                    window = CreateReplWindowInternal(evaluator, contentType, roles, curId, title, languageServiceGuid, replId);
                    if ((null == window) || (null == window.Frame)) {
                        throw new NotSupportedException(Resources.CanNotCreateWindow);
                    }

                    return window;
                }
            }

            throw new InvalidOperationException(String.Format("ReplId {0} was not provided by an IReplWindowProvider", replId));
        }

        public IReplWindow FindReplWindow(string replId) {
            foreach (var idAndWindow in _windows) {
                var window = idAndWindow.Value;
                if (window.ReplId == replId) {
                    return window;
                }
            }
            return null;
        }

        public IEnumerable<IReplWindow> GetReplWindows() {
            return _windows.Values;
        }

        private static IReplEvaluator GetReplEvaluator(IComponentModel model, string replId, out string[] roles) {
            roles = new string[0];
            foreach (var provider in model.GetExtensions<IReplEvaluatorProvider>()) {
                var evaluator = provider.GetEvaluator(replId);

                if (evaluator != null) {
                    roles = evaluator.GetType().GetCustomAttributes(typeof(ReplRoleAttribute), true).Select(r => ((ReplRoleAttribute)r).Name).ToArray();
                    return evaluator;
                }
            }
            return null;
        }

        #endregion

        #region Registry Serialization

        private const string ActiveReplsKey = "ActiveRepls";
        private const string ContentTypeKey = "ContentType";
        private const string RolesKey = "Roles";
        private const string TitleKey = "Title";
        private const string ReplIdKey = "ReplId";
        private const string LanguageServiceGuidKey = "LanguageServiceGuid";

        private static RegistryKey GetRegistryRoot() {
            return VSRegistry.RegistryRoot(__VsLocalRegistryType.RegType_UserSettings, writable: true).CreateSubKey(ActiveReplsKey);
        }

        private void SaveReplInfo(int id, IReplEvaluator evaluator, IContentType contentType, string[] roles, string title, Guid languageServiceGuid, string replId) {
            using (var root = GetRegistryRoot()) {
                if (root != null) {
                    using (var replInfo = root.CreateSubKey(id.ToString())) {
                        replInfo.SetValue(ContentTypeKey, contentType.TypeName);
                        replInfo.SetValue(TitleKey, title);
                        replInfo.SetValue(ReplIdKey, replId.ToString());
                        replInfo.SetValue(LanguageServiceGuidKey, languageServiceGuid.ToString());
                    }
                }
            }
        }

        internal bool CreateFromRegistry(IComponentModel model, int id) {
            string contentTypeName, title, replId, languageServiceId;

            using (var root = GetRegistryRoot()) {
                if (root == null) {
                    return false;
                }

                using (var replInfo = root.OpenSubKey(id.ToString())) {
                    if (replInfo == null) {
                        return false;
                    }

                    contentTypeName = replInfo.GetValue(ContentTypeKey) as string;
                    if (contentTypeName == null) {
                        return false;
                    }

                    title = replInfo.GetValue(TitleKey) as string;
                    if (title == null) {
                        return false;
                    }

                    replId = replInfo.GetValue(ReplIdKey) as string;
                    if (replId == null) {
                        return false;
                    }

                    languageServiceId = replInfo.GetValue(LanguageServiceGuidKey) as string;
                    if (languageServiceId == null) {
                        return false;
                    }
                }
            }

            Guid languageServiceGuid;
            if (!Guid.TryParse(languageServiceId, out languageServiceGuid)) {
                return false;
            }
            
            var contentTypes = model.GetService<IContentTypeRegistryService>();
            var contentType = contentTypes.GetContentType(contentTypeName);
            if (contentType == null) {
                return false;
            }

            string[] roles;
            var evaluator = GetReplEvaluator(model, replId, out roles);
            if (evaluator == null) {
                return false;
            }

            CreateReplWindow(evaluator, contentType, roles, id, title, languageServiceGuid, replId);
            return true;
        }

        #endregion

        #region Implementation Details

        private ReplWindow FindReplWindowInternal(int id) {
            ReplWindow res;
            if (_windows.TryGetValue(id, out res)) {
                return res;
            }
            return null;
        }

        public IReplWindow CreateReplWindow(IReplEvaluator/*!*/ evaluator, IContentType/*!*/ contentType, string[] roles, int id, string/*!*/ title, Guid languageServiceGuid, string replId) {
            return CreateReplWindowInternal(evaluator, contentType, roles, id, title, languageServiceGuid, replId);
        }

        private ReplWindow CreateReplWindowInternal(IReplEvaluator evaluator, IContentType contentType, string[] roles, int id, string title, Guid languageServiceGuid, string replId) {
            var service = (IVsUIShell)ServiceProvider.GlobalProvider.GetService(typeof(SVsUIShell));
            var model = (IComponentModel)ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel));

            SaveReplInfo(id, evaluator, contentType, roles, title, languageServiceGuid, replId);

            var replWindow = new ReplWindow(model, evaluator, contentType, roles, title, languageServiceGuid, replId);

            Guid clsId = replWindow.ToolClsid;
            Guid toolType = typeof(ReplWindow).GUID;
            Guid empty = Guid.Empty;
            IVsWindowFrame frame;

            // we don't pass __VSCREATETOOLWIN.CTW_fMultiInstance because multi instance panes are
            // destroyed when closed.  We are really multi instance but we don't want to be closed.  This
            // seems to work fine.
            var creationFlags = __VSCREATETOOLWIN.CTW_fInitNew;
            if (!roles.Contains("DontPersist")) {
                creationFlags |= __VSCREATETOOLWIN.CTW_fForceCreate;
            }
            ErrorHandler.ThrowOnFailure(
                service.CreateToolWindow(
                    (uint)(creationFlags),
                    (uint)id,
                    replWindow.GetIVsWindowPane(),
                    ref clsId,
                    ref toolType,
                    ref empty,
                    null,
                    title,
                    null,
                    out frame
                )
            );

            replWindow.Frame = frame;

            replWindow.OnToolBarAdded();
            _windows[id] = replWindow;

            return replWindow;
        }

        #endregion
    }
}
