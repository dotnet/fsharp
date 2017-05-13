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
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Input;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using TestUtilities;
using TestUtilities.Mocks;
using Thread = System.Threading.Thread;

namespace Microsoft.VisualStudioTools.MockVsTests {
    public sealed class MockVs : IComponentModel, IDisposable, IVisualStudioInstance {
        internal static CachedVsInfo CachedInfo = CreateCachedVsInfo();
        public CompositionContainer Container;
        private IContentTypeRegistryService _contentTypeRegistry;
        private Dictionary<Guid, Package> _packages = new Dictionary<Guid, Package>();
        internal readonly MockVsTextManager TextManager;
        internal readonly MockActivityLog ActivityLog = new MockActivityLog();
        internal readonly MockSettingsManager SettingsManager = new MockSettingsManager();
        internal readonly MockLocalRegistry LocalRegistry = new MockLocalRegistry();
        internal readonly MockVsDebugger Debugger = new MockVsDebugger();
        internal readonly MockVsTrackProjectDocuments TrackDocs = new MockVsTrackProjectDocuments();
        internal readonly MockVsShell Shell = new MockVsShell();
        internal readonly MockVsUIShell UIShell;
        public readonly MockVsSolution Solution = new MockVsSolution();
        private readonly MockVsServiceProvider _serviceProvider;
        private readonly List<MockVsTextView> _views = new List<MockVsTextView>();
        private readonly MockVsProfferCommands _proferredCommands = new MockVsProfferCommands();
        private readonly MockOleComponentManager _compManager = new MockOleComponentManager();
        private readonly MockOutputWindow _outputWindow = new MockOutputWindow();
        private readonly MockVsBuildManagerAccessor _buildManager = new MockVsBuildManagerAccessor();
        private readonly MockUIHierWinClipboardHelper _hierClipHelper = new MockUIHierWinClipboardHelper();
        internal readonly MockVsMonitorSelection _monSel;
        internal readonly MockVsUIHierarchyWindow _uiHierarchy;
        private readonly MockVsQueryEditQuerySave _queryEditSave = new MockVsQueryEditQuerySave();
        private readonly MockVsRunningDocumentTable _rdt;
        private readonly MockVsUIShellOpenDocument _shellOpenDoc = new MockVsUIShellOpenDocument();
        private readonly MockVsSolutionBuildManager _slnBuildMgr = new MockVsSolutionBuildManager();
        private readonly MockVsExtensibility _extensibility = new MockVsExtensibility();
        private readonly MockDTE _dte;
        private bool _shutdown;
        private AutoResetEvent _uiEvent = new AutoResetEvent(false);
        private readonly List<Action> _uiEvents = new List<Action>();
        private readonly Thread _throwExceptionsOn;
        private ExceptionDispatchInfo _edi;

        internal IOleCommandTarget
            /*_contextTarget, */    // current context menu
            /*_toolbarTarget, */    // current toolbar
            _focusTarget,       // current IVsWindowFrame that has focus
            _docCmdTarget,      // current document
            _projectTarget/*,   // current IVsHierarchy
            _shellTarget*/;

        private readonly Thread UIThread;

        public MockVs() {
            TextManager = new MockVsTextManager(this);
            Container = CreateCompositionContainer();
            var serviceProvider = _serviceProvider = Container.GetExportedValue<MockVsServiceProvider>();
            UIShell = new MockVsUIShell(this);
            _monSel = new MockVsMonitorSelection(this);
            _uiHierarchy = new MockVsUIHierarchyWindow(this);
            _rdt = new MockVsRunningDocumentTable(this);
            _dte = new MockDTE(this);
            _serviceProvider.AddService(typeof(SVsTextManager), TextManager);
            _serviceProvider.AddService(typeof(SVsActivityLog), ActivityLog);
            _serviceProvider.AddService(typeof(SVsSettingsManager), SettingsManager);
            _serviceProvider.AddService(typeof(SLocalRegistry), LocalRegistry);
            _serviceProvider.AddService(typeof(SComponentModel), this);
            _serviceProvider.AddService(typeof(IVsDebugger), Debugger);
            _serviceProvider.AddService(typeof(SVsSolution), Solution);
            _serviceProvider.AddService(typeof(SVsRegisterProjectTypes), Solution);
            _serviceProvider.AddService(typeof(SVsCreateAggregateProject), Solution);
            _serviceProvider.AddService(typeof(SVsTrackProjectDocuments), TrackDocs);
            _serviceProvider.AddService(typeof(SVsShell), Shell);
            _serviceProvider.AddService(typeof(SOleComponentManager), _compManager);
            _serviceProvider.AddService(typeof(SVsProfferCommands), _proferredCommands);
            _serviceProvider.AddService(typeof(SVsOutputWindow), _outputWindow);
            _serviceProvider.AddService(typeof(SVsBuildManagerAccessor), _buildManager);
            _serviceProvider.AddService(typeof(SVsUIHierWinClipboardHelper), _hierClipHelper);
            _serviceProvider.AddService(typeof(IVsUIShell), UIShell);
            _serviceProvider.AddService(typeof(IVsMonitorSelection), _monSel);
            _serviceProvider.AddService(typeof(SVsQueryEditQuerySave), _queryEditSave);
            _serviceProvider.AddService(typeof(SVsRunningDocumentTable), _rdt);
            _serviceProvider.AddService(typeof(SVsUIShellOpenDocument), _shellOpenDoc);
            _serviceProvider.AddService(typeof(SVsSolutionBuildManager), _slnBuildMgr);
            _serviceProvider.AddService(typeof(EnvDTE.IVsExtensibility), _extensibility);
            _serviceProvider.AddService(typeof(EnvDTE.DTE), _dte);

            UIShell.AddToolWindow(new Guid(ToolWindowGuids80.SolutionExplorer), new MockToolWindow(_uiHierarchy));

            uint dummy;
            ErrorHandler.ThrowOnFailure(
                _monSel.AdviseSelectionEvents(
                    new SelectionEvents(this),
                    out dummy
                )
            );

            _throwExceptionsOn = Thread.CurrentThread;

            using (var e = new AutoResetEvent(false)) {
                UIThread = new Thread(UIThreadWorker);
                UIThread.Name = "Mock UI Thread";
                UIThread.Start((object)e);
                // Wait for UI thread to start before returning. This ensures that
                // any packages we have are loaded and have published their services
                e.WaitOne();
            }
            ThrowPendingException();
        }

        class SelectionEvents : IVsSelectionEvents {
            private readonly MockVs _vs;

            public SelectionEvents(MockVs vs) {
                _vs = vs;
            }

            public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive) {
                return VSConstants.S_OK;
            }

            public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew) {
                return VSConstants.S_OK;
            }

            public int OnSelectionChanged(IVsHierarchy pHierOld, uint itemidOld, IVsMultiItemSelect pMISOld, ISelectionContainer pSCOld, IVsHierarchy pHierNew, uint itemidNew, IVsMultiItemSelect pMISNew, ISelectionContainer pSCNew) {
                _vs._projectTarget = pHierNew as IOleCommandTarget;
                _vs._focusTarget = pSCNew as IOleCommandTarget;
                return VSConstants.S_OK;
            }
        }

        class MockSyncContext : SynchronizationContext {
            private readonly MockVs _vs;
            public MockSyncContext(MockVs vs) {
                _vs = vs;
            }

            public override void Post(SendOrPostCallback d, object state) {
                _vs.Invoke(() => d(state));
            }

            public override void Send(SendOrPostCallback d, object state) {
                _vs.Invoke<object>(() => { d(state); return null; });
            }
        }

        public void AssertUIThread() {
            Assert.AreEqual(UIThread, Thread.CurrentThread);
        }

        private void UIThreadWorker(object evt) {
            try {
                SynchronizationContext.SetSynchronizationContext(new MockSyncContext(this));
                var packages = new List<IMockPackage>();
                foreach (var package in Container.GetExportedValues<IMockPackage>()) {
                    packages.Add(package);
                    package.Initialize();
                }

                ((AutoResetEvent)evt).Set();
                try {
                    RunMessageLoop();
                } finally {
                    foreach (var package in packages) {
                        package.Dispose();
                    }
                }
            } catch (Exception ex) {
                Trace.TraceError("Captured exception on mock UI thread: {0}", ex);
                _edi = ExceptionDispatchInfo.Capture(ex);
            }
        }

        internal void RunMessageLoop(AutoResetEvent dialogEvent = null) {
            WaitHandle[] handles;
            if (dialogEvent == null) {
                handles = new[] { _uiEvent };
            } else {
                handles = new[] { _uiEvent, dialogEvent };
            }

            while (!_shutdown) {
                if (WaitHandle.WaitAny(handles) == 1) {
                    // dialog is closing...
                    break;
                }
                Action[] events;
                do {
                    lock (_uiEvents) {
                        events = _uiEvents.ToArray();
                        _uiEvents.Clear();
                    }
                    foreach (var action in events) {
                        action();
                    }
                } while (events.Length > 0);
            }
        }

        private void ThrowPendingException(bool checkThread = true) {
            if (!checkThread || _throwExceptionsOn == Thread.CurrentThread) {
                var edi = Interlocked.Exchange(ref _edi, null);
                if (edi != null) {
                    edi.Throw();
                }
            }
        }

        public void Invoke(Action action) {
            ThrowPendingException();
            if (Thread.CurrentThread == UIThread) {
                action();
                return;
            }
            lock (_uiEvents) {
                _uiEvents.Add(action);
                _uiEvent.Set();
            }
        }

        public void InvokeSync(Action action) {
            Invoke<int>(() => {
                action();
                return 0;
            });
        }

        public T Invoke<T>(Func<T> func) {
            ThrowPendingException();
            if (Thread.CurrentThread == UIThread) {
                return func();
            }

            T res = default(T);
            using (var tmp = new AutoResetEvent(false)) {
                Action action = () => {
                    try {
                        res = func();
                    } finally {
                        tmp.Set();
                    }
                };

                lock (_uiEvents) {
                    _uiEvents.Add(action);
                    _uiEvent.Set();
                }

                while (!tmp.WaitOne(100)) {
                    if (!UIThread.IsAlive) {
                        ThrowPendingException(checkThread: false);
                        Debug.Fail("UIThread was terminated");
                        return res;
                    }
                }
            }
            ThrowPendingException(checkThread: false);
            return res;
        }


        public IServiceContainer ServiceProvider {
            get {
                return _serviceProvider;
            }
        }

        public IComponentModel ComponentModel {
            get {
                return this;
            }
        }

        public void DoIdle() {
        }

        public MockVsTextView CreateTextView(
            string contentType,
            string content,
            Action<MockVsTextView> onCreate = null,
            string file = null
        ) {
            return Invoke(() => CreateTextViewWorker(contentType, content, onCreate, file));
        }

        private MockVsTextView CreateTextViewWorker(
            string contentType,
            string content,
            Action<MockVsTextView> onCreate,
            string file = null
        ) {
            var buffer = new MockTextBuffer(content, ContentTypeRegistry.GetContentType(contentType), file);

            var view = new MockTextView(buffer);
            var res = new MockVsTextView(_serviceProvider, this, view);
            view.Properties[typeof(MockVsTextView)] = res;
            if (onCreate != null) {
                onCreate(res);
            }

            foreach (var classifier in Container.GetExports<IClassifierProvider, IContentTypeMetadata>()) {
                foreach (var targetContentType in classifier.Metadata.ContentTypes) {
                    if (buffer.ContentType.IsOfType(targetContentType)) {
                        classifier.Value.GetClassifier(buffer).GetClassificationSpans(
                            new SnapshotSpan(buffer.CurrentSnapshot, 0, buffer.CurrentSnapshot.Length)
                        );
                    }
                }
            }

            // Initialize code window
            LanguageServiceInfo info;
            if (CachedInfo.LangServicesByName.TryGetValue(contentType, out info)) {
                var id = info.Attribute.LanguageServiceSid;
                var serviceProvider = Container.GetExportedValue<MockVsServiceProvider>();
                var langInfo = (IVsLanguageInfo)serviceProvider.GetService(id);
                IVsCodeWindowManager mgr;
                var codeWindow = new MockCodeWindow(serviceProvider, view);
                view.Properties[typeof(MockCodeWindow)] = codeWindow;
                if (ErrorHandler.Succeeded(langInfo.GetCodeWindowManager(codeWindow, out mgr))) {
                    if (ErrorHandler.Failed(mgr.AddAdornments())) {
                        Console.WriteLine("Failed to add adornments to text view");
                    }
                }
            }

            // Initialize intellisense imports
            var providers = Container.GetExports<IIntellisenseControllerProvider, IContentTypeMetadata>();
            foreach (var provider in providers) {
                foreach (var targetContentType in provider.Metadata.ContentTypes) {
                    if (buffer.ContentType.IsOfType(targetContentType)) {
                        provider.Value.TryCreateIntellisenseController(
                            view,
                            new[] { buffer }
                        );
                        break;
                    }
                }
            }

            // tell the world we have a new view...
            foreach (var listener in Container.GetExports<IVsTextViewCreationListener, IContentTypeMetadata>()) {
                foreach (var targetContentType in listener.Metadata.ContentTypes) {
                    if (buffer.ContentType.IsOfType(targetContentType)) {
                        listener.Value.VsTextViewCreated(res);
                    }
                }
            }

            OnDispose(() => res.Close());
            return res;
        }

        public IContentTypeRegistryService ContentTypeRegistry {
            get {
                if (_contentTypeRegistry == null) {
                    _contentTypeRegistry = Container.GetExport<IContentTypeRegistryService>().Value;
                    var contentDefinitions = Container.GetExports<ContentTypeDefinition, IContentTypeDefinitionMetadata>();
                    foreach (var contentDef in contentDefinitions) {
                        _contentTypeRegistry.AddContentType(
                            contentDef.Metadata.Name,
                            contentDef.Metadata.BaseDefinition
                        );
                    }

                }
                return _contentTypeRegistry;
            }
        }

        #region Composition Container Initialization

        private CompositionContainer CreateCompositionContainer() {
            var container = new CompositionContainer(CachedInfo.Catalog);
            container.ComposeExportedValue<MockVs>(this);
            var batch = new CompositionBatch();

            container.Compose(batch);

            return container;
        }

        private static CachedVsInfo CreateCachedVsInfo() {
            var runningLoc = Path.GetDirectoryName(typeof(MockVs).Assembly.Location);
            // we want to pick up all of the MEF exports which are available, but they don't
            // depend upon us.  So if we're just running some tests in the IDE when the deployment
            // happens it won't have the DLLS with the MEF exports.  So we copy them here.
            TestData.Deploy(null, includeTestData: false);

            // load all of the available DLLs that depend upon TestUtilities into our catalog
            List<AssemblyCatalog> catalogs = new List<AssemblyCatalog>();
            List<Type> packageTypes = new List<Type>();
            foreach (var file in Directory.GetFiles(runningLoc, "*.dll")) {
                if (file.EndsWith("VsLogger.dll", StringComparison.OrdinalIgnoreCase)) {
                    continue;
                }

                Assembly asm;
                try {
                    asm = Assembly.Load(Path.GetFileNameWithoutExtension(file));
                } catch {
                    continue;
                }

                Console.WriteLine("Including {0}", file);
                try {
                    foreach (var type in asm.GetTypes()) {
                        if (type.IsDefined(typeof(PackageRegistrationAttribute), false)) {
                            packageTypes.Add(type);
                        }
                    }
                    catalogs.Add(new AssemblyCatalog(asm));
                } catch (TypeInitializationException tix) {
                    Console.WriteLine(tix);
                } catch (ReflectionTypeLoadException tlx) {
                    Console.WriteLine(tlx);
                } catch (IOException iox) {
                    Console.WriteLine(iox);
                }
            }

            return new CachedVsInfo(
                new AggregateCatalog(catalogs.ToArray()),
                packageTypes
            );
        }

        #endregion

        public ITreeNode WaitForItemRemoved(params string[] path) {
            ITreeNode item = null;
            for (int i = 0; i < 400; i++) {
                item = FindItem(path);
                if (item == null) {
                    break;
                }
                System.Threading.Thread.Sleep(25);
            }
            return item;
        }

        ITreeNode IVisualStudioInstance.WaitForItem(params string[] items) {
            var res = WaitForItem(items);
            if (res.IsNull) {
                return null;
            }
            return new MockTreeNode(this, res);
        }

        public ITreeNode FindItem(params string[] items) {
            var res = WaitForItem(items);
            if (res.IsNull) {
                return null;
            }
            return new MockTreeNode(this, res);
        }

        /// <summary>
        /// Gets an item from solution explorer.
        /// 
        /// First item is the project name, additional items are the name of the displayed caption in
        /// Solution Explorer.
        /// </summary>
        public HierarchyItem WaitForItem(params string[] items) {
            return Invoke(() => WaitForItemWorker(items));
        }

        private HierarchyItem WaitForItemWorker(string[] items) {
            IVsHierarchy hierarchy;
            if (ErrorHandler.Failed(Solution.GetProjectOfUniqueName(items[0], out hierarchy))) {
                return new HierarchyItem();
            }
            if (items.Length == 1) {
                return new HierarchyItem(hierarchy, (uint)VSConstants.VSITEMID.Root);
            }

            var firstItem = items[1];
            var firstHierItem = new HierarchyItem();
            foreach (var item in hierarchy.GetHierarchyItems()) {
                if (item.Caption == firstItem) {
                    firstHierItem = item;
                    break;
                }
            }

            if (firstHierItem.IsNull) {
                return new HierarchyItem();
            }

            for (int i = 2; i < items.Length; i++) {
                bool found = false;
                foreach (var item in firstHierItem.Children) {
                    if (item.Caption == items[i]) {
                        firstHierItem = item;
                        found = true;
                        break;
                    }
                }

                if (!found) {
                    firstHierItem = new HierarchyItem();
                    break;
                }
            }

            return firstHierItem;
        }

        IEditor IVisualStudioInstance.OpenItem(string project, params string[] path) {
            return OpenItem(project, path);
        }

        public MockVsTextView OpenItem(string project, params string[] path) {
            return Invoke(() => OpenItemWorker(project, path));
        }

        private MockVsTextView OpenItemWorker(string project, string[] path) {
            // matching the API of VisualStudioSolution.OpenItem
            string[] temp = new string[path.Length + 1];
            temp[0] = project;
            Array.Copy(path, 0, temp, 1, path.Length);
            var item = WaitForItem(temp);
            if (item.IsNull) {
                return null;
            }

            string languageName;
            if (!CachedInfo._languageNamesByExtension.TryGetValue(Path.GetExtension(item.CanonicalName), out languageName)) {
                languageName = "code";
            }

            var res = CreateTextView(languageName, File.ReadAllText(item.CanonicalName), null, item.CanonicalName);
            if (_docCmdTarget != null) {
                ((IFocusable)_docCmdTarget).LostFocus();
            }
            _docCmdTarget = res;
            ((IFocusable)res).GetFocus();

            uint cookie;
            IVsTextLines lines;
            ErrorHandler.ThrowOnFailure(((IVsTextView)res).GetBuffer(out lines));
            IntPtr linesPtr = Marshal.GetIUnknownForObject(lines);
            try {
                ErrorHandler.ThrowOnFailure(
                    _rdt.RegisterAndLockDocument(
                        (uint)_VSRDTFLAGS.RDT_NoLock,
                        item.CanonicalName,
                        item.Hierarchy,
                        item.ItemId,
                        linesPtr,
                        out cookie
                    )
                );
            } finally {
                Marshal.Release(linesPtr);
            }
            return res;
        }

        ComposablePartCatalog IComponentModel.DefaultCatalog {
            get { throw new NotImplementedException(); }
        }

        ICompositionService IComponentModel.DefaultCompositionService {
            get { throw new NotImplementedException(); }
        }

        ExportProvider IComponentModel.DefaultExportProvider {
            get { throw new NotImplementedException(); }
        }

        ComposablePartCatalog IComponentModel.GetCatalog(string catalogName) {
            throw new NotImplementedException();
        }

        IEnumerable<T> IComponentModel.GetExtensions<T>() {
            return Container.GetExportedValues<T>();
        }

        T IComponentModel.GetService<T>() {
            return Container.GetExportedValue<T>();
        }

        public void Dispose() {
            _shutdown = true;
            _uiEvent.Set();
            ThrowPendingException();
            AssertListener.ThrowUnhandled();
        }


        public void Type(Key key) {
            Invoke(() => TypeWorker(key));
        }

        private void TypeWorker(Key key) {
            Guid guid;
            switch (key) {
                case Key.F2:
                    guid = VSConstants.GUID_VSStandardCommandSet97;
                    Exec(ref guid, (int)VSConstants.VSStd97CmdID.Rename, 0, IntPtr.Zero, IntPtr.Zero);
                    break;
                case Key.Enter:
                    guid = VSConstants.VSStd2K;
                    Exec(ref guid, (int)VSConstants.VSStd2KCmdID.RETURN, 0, IntPtr.Zero, IntPtr.Zero);
                    break;
                case Key.Tab:
                    guid = VSConstants.VSStd2K;
                    Exec(ref guid, (int)VSConstants.VSStd2KCmdID.TAB, 0, IntPtr.Zero, IntPtr.Zero);
                    break;
                case Key.Delete: 
                    guid = VSConstants.VSStd2K;
                    Exec(ref guid, (int)VSConstants.VSStd2KCmdID.DELETE, 0, IntPtr.Zero, IntPtr.Zero);
                    break;
                default:
                    throw new InvalidOperationException("Unmapped key " + key);
            }
        }


        private IEnumerable<IOleCommandTarget> Targets {
            get {
                if (_focusTarget != null) {
                    yield return _focusTarget;
                }
                if (_docCmdTarget != null) {
                    yield return _docCmdTarget;
                }
                if (_projectTarget != null) {
                    yield return _projectTarget;
                }
            }
        }

        private int Exec(ref Guid cmdGroup, uint cmdId, uint cmdExecopt, IntPtr pvaIn, IntPtr pvaOut) {
            int hr = NativeMethods.OLECMDERR_E_NOTSUPPORTED;
            foreach (var target in Targets) {
                hr = target.Exec(
                    cmdGroup,
                    cmdId,
                    cmdExecopt,
                    pvaIn,
                    pvaOut
                );
                if (hr == NativeMethods.OLECMDERR_E_CANCELED ||
                    (hr != NativeMethods.OLECMDERR_E_NOTSUPPORTED &&
                    hr != NativeMethods.OLECMDERR_E_UNKNOWNGROUP)) {
                    if (hr == NativeMethods.OLECMDERR_E_CANCELED) {
                        hr = VSConstants.S_OK;
                    }
                    break;
                }
            }
            return hr;
        }

        private void ContinueRouting() {
        }

        public void ControlX() {
            Invoke(() => ControlXWorker());
        }

        private void ControlXWorker() {
            var guid = VSConstants.GUID_VSStandardCommandSet97;
            Exec(ref guid, (int)VSConstants.VSStd97CmdID.Cut, 0, IntPtr.Zero, IntPtr.Zero);
        }

        public void ControlC() {
            Invoke(() => ControlCWorker());
        }

        private void ControlCWorker() {
            var guid = VSConstants.GUID_VSStandardCommandSet97;
            Exec(ref guid, (int)VSConstants.VSStd97CmdID.Copy, 0, IntPtr.Zero, IntPtr.Zero);
        }

        public void Type(string p) {
            Invoke(() => TypeWorker(p));
        }

        public void PressAndRelease(Key key, params Key[] modifier) {
            throw new NotImplementedException();
        }

        private void TypeWorker(string p) {
            if (UIShell.Dialogs.Count != 0) {
                UIShell.Dialogs.Last().Type(p);
                return;
            }

            TypeCmdTarget(p);
        }

        private void TypeCmdTarget(string text) {
            var guid = VSConstants.VSStd2K;
            var variantMem = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(VARIANT)));
            try {
                for (int i = 0; i < text.Length; i++) {
                    switch (text[i]) {
                        case '\r': TypeWorker(Key.Enter); break;
                        case '\t': TypeWorker(Key.Tab); break;
                        default:
                            Marshal.GetNativeVariantForObject((ushort)text[i], variantMem);
                            Exec(
                                ref guid,
                                (int)VSConstants.VSStd2KCmdID.TYPECHAR,
                                0,
                                variantMem,
                                IntPtr.Zero
                            );
                            break;
                    }

                }
            } finally {
                Marshal.FreeCoTaskMem(variantMem);
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = 16)]
        struct VARIANT {
            [FieldOffset(0)]
            public ushort vt;
            [FieldOffset(8)]
            public IntPtr pdispVal;
            [FieldOffset(8)]
            public byte ui1;
            [FieldOffset(8)]
            public ushort ui2;
            [FieldOffset(8)]
            public uint ui4;
            [FieldOffset(8)]
            public ulong ui8;
            [FieldOffset(8)]
            public float r4;
            [FieldOffset(8)]
            public double r8;
        }

        public void ControlV() {
            Invoke(() => ControlVWorker());
        }

        private void ControlVWorker() {
            var guid = VSConstants.GUID_VSStandardCommandSet97;
            Exec(ref guid, (int)VSConstants.VSStd97CmdID.Paste, 0, IntPtr.Zero, IntPtr.Zero);
        }

        public void CheckMessageBox(params string[] text) {
            CheckMessageBox(MessageBoxButton.Cancel, text);
        }

        public void CheckMessageBox(MessageBoxButton button, params string[] text) {
            UIShell.CheckMessageBox(button, text);
        }

        public void Sleep(int ms) {
        }


        public void ExecuteCommand(string command) {
            throw new NotImplementedException();
        }

        public string SolutionFilename {
            get {
                return Solution.SolutionFile;
            }
        }

        public string SolutionDirectory {
            get {
                return Path.GetDirectoryName(SolutionFilename);
            }
        }

        public IntPtr WaitForDialog() {
            return UIShell.WaitForDialog();
        }

        public void WaitForDialogDismissed() {
            UIShell.WaitForDialogDismissed();
        }

        public void AssertFileExists(params string[] path) {
            Assert.IsNotNull(FindItem(path));

            var basePath = Path.Combine(new[] { SolutionDirectory }.Concat(path).ToArray());
            Assert.IsTrue(File.Exists(basePath), "File doesn't exist: " + basePath);
        }

        public void AssertFileDoesntExist(params string[] path) {
            Assert.IsNull(FindItem(path));

            var basePath = Path.Combine(new[] { SolutionDirectory }.Concat(path).ToArray());
            Assert.IsFalse(File.Exists(basePath), "File exists: " + basePath);
        }

        public void AssertFolderExists(params string[] path) {
            Assert.IsNotNull(FindItem(path));

            var basePath = Path.Combine(new[] { SolutionDirectory }.Concat(path).ToArray());
            Assert.IsTrue(Directory.Exists(basePath), "Folder doesn't exist: " + basePath);
        }

        public void AssertFolderDoesntExist(params string[] path) {
            Assert.IsNull(FindItem(path));

            var basePath = Path.Combine(new[] { SolutionDirectory }.Concat(path).ToArray());
            Assert.IsFalse(Directory.Exists(basePath), "Folder exists: " + basePath);
        }

        public void AssertFileExistsWithContent(string content, params string[] path) {
            Assert.IsNotNull(FindItem(path));

            var basePath = Path.Combine(new[] { SolutionDirectory }.Concat(path).ToArray());
            Assert.IsTrue(File.Exists(basePath), "File doesn't exist: " + basePath);
            Assert.AreEqual(File.ReadAllText(basePath), content);
        }

        public void CloseActiveWindow(vsSaveChanges save) {
            throw new NotImplementedException();
        }
        public void WaitForOutputWindowText(string name, string containsText, int timeout = 5000) {
            throw new NotImplementedException();
        }

        public IntPtr OpenDialogWithDteExecuteCommand(string commandName, string commandArgs = "") {
            throw new NotImplementedException();
        }

        public void SelectSolutionNode() {
        }

        public Project GetProject(string projectName) {
            throw new NotImplementedException();
        }

        public void SelectProject(Project project) {
            throw new NotImplementedException();
        }

        public IEditor GetDocument(string filename) {
            throw new NotImplementedException();
        }

        public IAddExistingItem AddExistingItem() {
            throw new NotImplementedException();
        }

        public IOverwriteFile WaitForOverwriteFileDialog() {
            throw new NotImplementedException();
        }

        public IAddNewItem AddNewItem() {
            throw new NotImplementedException();
        }

        public void WaitForMode(dbgDebugMode dbgDebugMode) {
            throw new NotImplementedException();
        }

        public List<IVsTaskItem> WaitForErrorListItems(int expectedCount) {
            throw new NotImplementedException();
        }


        public DTE Dte {
            get { return _dte; }
        }

        public void OnDispose(Action action) {

        }
    }
}
