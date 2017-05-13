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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

#if NTVS_FEATURE_INTERACTIVEWINDOW
using Microsoft.VisualStudio;
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    /// <summary>
    /// Provides implementation of a Repl Window built on top of the VS editor using projection buffers.
    /// 
    /// TODO: We should condense committed language buffers into a single language buffer and save the
    /// classifications from the previous language buffer if the perf of having individual buffers
    /// starts having problems w/ a large number of inputs.
    /// </summary>
    [Guid(ReplWindow.TypeGuid)]
    class ReplWindow : ToolWindowPane, IOleCommandTarget, IReplWindow3, IVsFindTarget {
#if NTVS_FEATURE_INTERACTIVEWINDOW
        public const string TypeGuid = "2153A414-267E-4731-B891-E875ADBA1993";
#else
        public const string TypeGuid = "5adb6033-611f-4d39-a193-57a717115c0f";
#endif

        private bool _adornmentToMinimize = false;
        private bool _showOutput, _useSmartUpDown;
        
        // true iff code is being executed:
        private bool _isRunning;

        private Stopwatch _sw;
        private DispatcherTimer _executionTimer;
        private Cursor _oldCursor;
        private List<IReplCommand> _commands;
        private IWpfTextViewHost _textViewHost;
        private IEditorOperations _editorOperations;
        private readonly History/*!*/ _history;
        private TaskScheduler _uiScheduler;
        private PropertyCollection _properties;

        //
        // Services
        // 
        private readonly IComponentModel/*!*/ _componentModel;
        private readonly Guid _langSvcGuid;
        private readonly string _replId;
        private readonly IContentType/*!*/ _languageContentType;
        private readonly string[] _roles;
        private readonly IClassifierAggregatorService _classifierAgg;
        private ReplAggregateClassifier _primaryClassifier;
        private readonly IReplEvaluator/*!*/ _evaluator;
        private IVsFindTarget _findTarget;
        private IVsTextView _view;
        private ISmartIndent _languageIndenter;

        //
        // Command filter chain: 
        // window -> VsTextView -> ... -> pre-language -> language services -> post-language -> editor services -> preEditor -> editor
        //
        private IOleCommandTarget _preLanguageCommandFilter;
        private IOleCommandTarget _languageServiceCommandFilter;
        private IOleCommandTarget _postLanguageCommandFilter;
        private IOleCommandTarget _preEditorCommandFilter;
        private IOleCommandTarget _editorServicesCommandFilter;
        private IOleCommandTarget _editorCommandFilter;

        //
        // A list of scopes if this REPL is multi-scoped
        // 
        private string[] _currentScopes;
        private bool _scopeListVisible;

        //
        // Buffer composition.
        // 
        private readonly ITextBufferFactoryService _bufferFactory;                          // Factory for creating output, std input, prompt and language buffers.
        private IProjectionBuffer _projectionBuffer;
        private ITextBuffer _outputBuffer;
        private ITextBuffer _stdInputBuffer;
        private ITextBuffer _currentLanguageBuffer;
        private string _historySearch;

        // Read-only regions protecting initial span of the corresponding buffers:
        private IReadOnlyRegion[] _stdInputProtection = new IReadOnlyRegion[2];
        private IReadOnlyRegion[] _outputProtection = new IReadOnlyRegion[2];
        
        // List of projection buffer spans - the projection buffer doesn't allow us to enumerate spans so we need to track them manually:
        private readonly List<ReplSpan> _projectionSpans = new List<ReplSpan>();

        // Maps line numbers in projection buffer to indices of projection spans corresponding to primary and stdin prompts.
        // All but the last item correspond to submitted prompts that never change their line position.
        private List<KeyValuePair<int, int>> _promptLineMapping = new List<KeyValuePair<int, int>>();                      

        //
        // Submissions.
        //

        // Pending submissions to be processed whenever the REPL is ready to accept submissions.
        private Queue<string> _pendingSubmissions;

        //
        // Standard input.
        //

        // non-null if reading from stdin - position in the _inputBuffer where we map stdin
        private int? _stdInputStart;
        private bool _readingStdIn;
        private int _currentInputId = 1;
        private string _inputValue;
        private string _uncommittedInput;
        private readonly AutoResetEvent _inputEvent = new AutoResetEvent(false);
        
        //
        // Output buffering.
        //
        private readonly OutputBuffer _buffer;
        private readonly List<ColoredSpan> _outputColors = new List<ColoredSpan>();
        private bool _addedLineBreakOnLastOutput;

        private string _commandPrefix = "%";
        private string _prompt = "» ";        // prompt for primary input
        private string _secondPrompt = String.Empty;    // prompt for 2nd and additional lines
        private string _stdInputPrompt = String.Empty;  // prompt for standard input
        private bool _displayPromptInMargin, _formattedPrompts;

        private static readonly char[] _whitespaceChars = new[] { '\r', '\n', ' ', '\t' };
        private const string _boxSelectionCutCopyTag = "MSDEVColumnSelect";

        public ReplWindow(IComponentModel/*!*/ model, IReplEvaluator/*!*/ evaluator, IContentType/*!*/ contentType, string[] roles, string/*!*/ title, Guid languageServiceGuid, string replId) {
            Contract.Assert(evaluator != null);
            Contract.Assert(contentType != null);
            Contract.Assert(title != null);
            Contract.Assert(model != null);
            
            _properties = new PropertyCollection();

            _replId = replId;
            _langSvcGuid = languageServiceGuid;
            _buffer = new OutputBuffer(this);

            // Set the window title reading it from the resources.z
            Caption = title;

            _componentModel = model;
            _evaluator = evaluator;
            _languageContentType = contentType;
            _roles = roles;
            
            Contract.Requires(_commandPrefix != null && _prompt != null);
            
            _history = new History();

            _bufferFactory = model.GetService<ITextBufferFactoryService>();
            _classifierAgg = model.GetService<IClassifierAggregatorService>();

            _showOutput = true;
        }

        #region Initialization

        protected override void OnCreate() {
            CreateTextViewHost();

            var textView = _textViewHost.TextView;

            _view = ComponentModel.GetService<IVsEditorAdaptersFactoryService>().GetViewAdapter(textView);
            _findTarget = _view as IVsFindTarget;

            _postLanguageCommandFilter = new CommandFilter(this, CommandFilterLayer.PostLanguage);
            ErrorHandler.ThrowOnFailure(_view.AddCommandFilter(_postLanguageCommandFilter, out _editorServicesCommandFilter));

            // may add command filters
            foreach (var listener in GetCreationListeners(ComponentModel, _languageContentType.TypeName)) {
                listener.ReplWindowCreated(this);
            }

            // by this time all applicable language filters should be attached:
            _preLanguageCommandFilter = new CommandFilter(this, CommandFilterLayer.PreLanguage);
            ErrorHandler.ThrowOnFailure(_view.AddCommandFilter(_preLanguageCommandFilter, out _languageServiceCommandFilter));

            textView.Options.SetOptionValue(DefaultTextViewHostOptions.HorizontalScrollBarId, false);
            textView.Options.SetOptionValue(DefaultTextViewHostOptions.LineNumberMarginId, false);
            textView.Options.SetOptionValue(DefaultTextViewHostOptions.OutliningMarginId, false);
            textView.Options.SetOptionValue(DefaultTextViewHostOptions.GlyphMarginId, false);
            textView.Options.SetOptionValue(DefaultTextViewOptions.WordWrapStyleId, WordWrapStyles.WordWrap);

            var editorOperationsFactory = ComponentModel.GetService<IEditorOperationsFactoryService>();
            _editorOperations = editorOperationsFactory.GetEditorOperations(textView);
            _languageIndenter = GetIndenter(ComponentModel, textView, _languageContentType.TypeName);

            _commands = CreateCommands();

            textView.TextBuffer.Properties.AddProperty(typeof(IReplEvaluator), _evaluator);

            _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            // Anything that reads options should wait until after this call so the evaluator can set the options first
            (Evaluator.Initialize(this) ?? ExecutionResult.Failed).ContinueWith(FinishInitialization, _uiScheduler);
        }

        private void FinishInitialization(Task<ExecutionResult> completedTask) {
            Debug.Assert(Dispatcher.CheckAccess());

            _buffer.Flush();

            // TODO: remove
            InitializeScopeList();

            // may add command filters
            PrepareForInput();
        }

        private void CreateTextViewHost() {
            var adapterFactory = _componentModel.GetService<IVsEditorAdaptersFactoryService>();
            var provider = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)GetService(typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider));

            var contentTypeRegistry = _componentModel.GetService<IContentTypeRegistryService>();
            var textContentType = contentTypeRegistry.GetContentType("text");
            var replContentType = contentTypeRegistry.GetContentType(ReplConstants.ReplContentTypeName);
            var replOutputContentType = contentTypeRegistry.GetContentType(ReplConstants.ReplOutputContentTypeName);

            _outputBuffer = _bufferFactory.CreateTextBuffer(replOutputContentType);
            _outputBuffer.Properties.AddProperty(ReplOutputClassifier.ColorKey, _outputColors);
            _stdInputBuffer = _bufferFactory.CreateTextBuffer();

            var projectionFactory = _componentModel.GetService<IProjectionBufferFactoryService>();

            var projBuffer = projectionFactory.CreateProjectionBuffer(
                new EditResolver(this),
                new object[0],
                ProjectionBufferOptions.None,
                replContentType);

            var bufferAdapter = adapterFactory.CreateVsTextBufferAdapterForSecondaryBuffer(provider, projBuffer);

            // we need to set IReplProptProvider property before TextViewHost is instantiated so that ReplPromptTaggerProvider can bind to it 
            projBuffer.Properties.AddProperty(typeof(ReplWindow), this);

            // Create and inititalize text view adapter.
            // WARNING: This might trigger various services like IntelliSense, margins, taggers, etc.
            IVsTextView textViewAdapter = adapterFactory.CreateVsTextViewAdapter(provider, CreateRoleSet());

            // make us a code window so we'll have the same colors as a normal code window.
            IVsTextEditorPropertyContainer propContainer;
            ErrorHandler.ThrowOnFailure(((IVsTextEditorPropertyCategoryContainer)textViewAdapter).GetPropertyCategory(Microsoft.VisualStudio.Editor.DefGuidList.guidEditPropCategoryViewMasterSettings, out propContainer));
            propContainer.SetProperty(VSEDITPROPID.VSEDITPROPID_ViewComposite_AllCodeWindowDefaults, true);
            propContainer.SetProperty(VSEDITPROPID.VSEDITPROPID_ViewGlobalOpt_AutoScrollCaretOnTextEntry, true);

            // editor services are initialized in textViewAdapter.Initialize - hook underneath them:
            _preEditorCommandFilter = new CommandFilter(this, CommandFilterLayer.PreEditor);
            ErrorHandler.ThrowOnFailure(textViewAdapter.AddCommandFilter(_preEditorCommandFilter, out _editorCommandFilter));

            textViewAdapter.Initialize(
                (IVsTextLines)bufferAdapter,
                IntPtr.Zero,
                (uint)TextViewInitFlags.VIF_HSCROLL | (uint)TextViewInitFlags.VIF_VSCROLL | (uint)TextViewInitFlags3.VIF_NO_HWND_SUPPORT,
                new[] { new INITVIEW { fSelectionMargin = 0, fWidgetMargin = 0, fVirtualSpace = 0, fDragDropMove = 1 } }
            );                         


            // disable change tracking because everything will be changed
            var res = adapterFactory.GetWpfTextViewHost(textViewAdapter);
            var options = res.TextView.Options;
            
            // propagate language options to our text view
            IVsTextManager textMgr = (IVsTextManager)ReplWindowPackage.GetGlobalService(typeof(SVsTextManager));
            var langPrefs = new LANGPREFERENCES[1];
            langPrefs[0].guidLang = LanguageServiceGuid;
            ErrorHandler.ThrowOnFailure(textMgr.GetUserPreferences(null, null, langPrefs, null));

            options.SetOptionValue(DefaultTextViewHostOptions.ChangeTrackingId, false);
            options.SetOptionValue(DefaultOptions.ConvertTabsToSpacesOptionId, langPrefs[0].fInsertTabs == 0);
            options.SetOptionValue(DefaultOptions.TabSizeOptionId, (int)langPrefs[0].uTabSize);
            options.SetOptionValue(DefaultOptions.IndentSizeOptionId, (int)langPrefs[0].uIndentSize);

            res.HostControl.Name = MakeHostControlName(Title);
            
            _projectionBuffer = projBuffer;
            InitializeProjectionBuffer();
            
            _projectionBuffer.Changed += new EventHandler<TextContentChangedEventArgs>(ProjectionBufferChanged);
            
            // get our classifier...
            _primaryClassifier = projBuffer.Properties.GetProperty<ReplAggregateClassifier>(typeof(ReplAggregateClassifier));

            // aggreggate output classifications
            var outputClassifier = _classifierAgg.GetClassifier(_outputBuffer);
            _primaryClassifier.AddClassifier(_projectionBuffer, _outputBuffer, outputClassifier);

            _textViewHost = res;
        }

        private static IEnumerable<IReplWindowCreationListener> GetCreationListeners(IComponentModel model, string contentType) {
            return
                from export in model.DefaultExportProvider.GetExports<IReplWindowCreationListener, IContentTypeMetadata>()
                from exportedContentType in export.Metadata.ContentTypes
                where exportedContentType == contentType
                select export.Value;
        }

        private static ISmartIndent GetIndenter(IComponentModel model, ITextView view, string contentType) {
            var provider = (
                from export in model.DefaultExportProvider.GetExports<ISmartIndentProvider, IContentTypeMetadata>()
                from exportedContentType in export.Metadata.ContentTypes
                where exportedContentType == contentType
                select export.Value
            ).FirstOrDefault();

            return (provider != null) ? provider.CreateSmartIndent(view) : null;
        }

        private bool IsCommandApplicable(IReplCommand command) {
            bool applicable = true;
            string[] commandRoles = command.GetType().GetCustomAttributes(typeof(ReplRoleAttribute), true).Select(r => ((ReplRoleAttribute)r).Name).ToArray();
            if (_roles.Length > 0) {
                // The window has one or more roles, so the following commands will be applicable:
                // - commands that don't specify a role
                // - commands that specify a role that matches one of the window roles
                if (commandRoles.Length > 0) {
                    applicable = _roles.Intersect(commandRoles).Count() > 0;
                }
            } else {
                // The window doesn't have any role, so the following commands will be applicable:
                // - commands that don't specify any role
                applicable = commandRoles.Length == 0;
            }

            return applicable;
        }

        private List<IReplCommand>/*!*/ CreateCommands() {
            var commands = new List<IReplCommand>();
            var commandTypes = new HashSet<Type>();
            foreach (var command in _componentModel.GetExtensions<IReplCommand>()) {
                if (!IsCommandApplicable(command)) {
                    continue;
                }

                // avoid duplicate commands
                if (commandTypes.Contains(command.GetType())) {
                    continue;
                } else {
                    commandTypes.Add(command.GetType());
                }

                commands.Add(command);
            }
            return commands;
        }

        public override void OnToolWindowCreated() {
            Guid commandUiGuid = Microsoft.VisualStudio.VSConstants.GUID_TextEditorFactory;
            ((IVsWindowFrame)Frame).SetGuidProperty((int)__VSFPROPID.VSFPROPID_InheritKeyBindings, ref commandUiGuid);

            base.OnToolWindowCreated();

            // add our toolbar which  is defined in our VSCT file
            var frame = (IVsWindowFrame)Frame;
            object otbh;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(frame.GetProperty((int)__VSFPROPID.VSFPROPID_ToolbarHost, out otbh));
            IVsToolWindowToolbarHost tbh = otbh as IVsToolWindowToolbarHost;
            Guid guidPerfMenuGroup = Guids.guidReplWindowCmdSet;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(tbh.AddToolbar(VSTWT_LOCATION.VSTWT_TOP, ref guidPerfMenuGroup, PkgCmdIDList.menuIdReplToolbar));
        }

        private ITextViewRoleSet/*!*/ CreateRoleSet() {
            var textEditorFactoryService = ComponentModel.GetService<ITextEditorFactoryService>();
            return textEditorFactoryService.CreateTextViewRoleSet(
                PredefinedTextViewRoles.Analyzable,
                PredefinedTextViewRoles.Editable,
                PredefinedTextViewRoles.Interactive,
                PredefinedTextViewRoles.Zoomable,
                ReplConstants.ReplTextViewRole
            );
        }

        /// <summary>
        /// Produces a name which is compatible with x:Name requirements (starts with a letter/underscore, contains
        /// only letter, numbers, or underscores).
        /// </summary>
        private static string MakeHostControlName(string title) {
            if (title.Length == 0) {
                return "InteractiveWindowHost";
            }

            StringBuilder res = new StringBuilder();
            if (!Char.IsLetter(title[0])) {
                res.Append('_');
            }

            foreach (char c in title) {
                if (Char.IsLetter(c) || Char.IsDigit(c) || c == '_') {
                    res.Append(c);
                }
            }
            res.Append("Host");
            return res.ToString();
        }

        protected override void OnClose() {
            TextView.Close();
            _evaluator.Dispose();
            base.OnClose();
        }

        #endregion

        #region Misc Helpers

        public string ReplId {
            get { return _replId; }
        }

        public Guid LanguageServiceGuid {
            get { return _langSvcGuid; }
        }

        public IEditorOperations EditorOperations { 
            get { return _editorOperations; }
        }

        public IComponentModel ComponentModel {
            get { return _componentModel; }
        }

        public ITextBuffer/*!*/ TextBuffer {
            get { return TextView.TextBuffer; }
        }

        public ITextSnapshot CurrentSnapshot {
            get { return TextBuffer.CurrentSnapshot; }
        }

        internal ISmartIndent LanguageIndenter {
            get { return _languageIndenter; }
        }

        internal IContentType LanguageContentType {
            get { return _languageContentType; }
        }

        public ITextBuffer CurrentLanguageBuffer {
            get { return _currentLanguageBuffer; }
        }

        private void RequiresLanguageBuffer() {
            if (_currentLanguageBuffer == null) {
                throw new InvalidOperationException("Language buffer not available");
            }
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                Evaluator.Dispose();

                _buffer.Dispose();
                _inputEvent.Dispose();

                _commands = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// This property returns the control that should be hosted in the Tool Window.
        /// It can be either a FrameworkElement (for easy creation of toolwindows hosting WPF content), 
        /// or it can be an object implementing one of the IVsUIWPFElement or IVsUIWin32Element interfaces.
        /// </summary>
        public override object Content {
            get {
                Debug.Assert(_textViewHost != null);
                return _textViewHost;
            }
            set { }
        }

        public static ReplWindow FromBuffer(ITextBuffer buffer) {
            object result;
            buffer.Properties.TryGetProperty(typeof(ReplWindow), out result);
            return result as ReplWindow;
        }
        
        #endregion

        #region IReplWindow

        public event Action ReadyForInput;

        /// <summary>
        /// See IReplWindow
        /// </summary>
        public IWpfTextView/*!*/ TextView {
            get { 
                return _textViewHost.TextView; 
            }
        }

        /// <summary>
        /// See IReplWindow
        /// </summary>
        public IReplEvaluator/*!*/ Evaluator {
            get {
                return _evaluator;
            }
        }

        /// <summary>
        /// See IReplWindow
        /// </summary>
        public string/*!*/ Title {
            get {
                return Caption;
            }
        }

        public void ClearHistory() {
            if (!CheckAccess()) {
                Dispatcher.Invoke(new Action(ClearHistory));
                return;
            }
            _history.Clear();
        }

        /// <summary>
        /// See IReplWindow
        /// </summary>
        public void ClearScreen() {
            ClearScreen(insertInputPrompt: false);
        }

        private void ClearScreen(bool insertInputPrompt) {
            if (!CheckAccess()) {
                Dispatcher.Invoke(new Action(ClearScreen));
                return;
            }

            if (_stdInputStart != null) {
                CancelStandardInput();
            }

            _adornmentToMinimize = false;
            InlineReplAdornmentProvider.RemoveAllAdornments(TextView);

            // remove all the spans except our initial span from the projection buffer
            _promptLineMapping = new List<KeyValuePair<int,int>>();
            _currentInputId = 1;
            _uncommittedInput = null;
            _outputColors.Clear();

            // Clear the projection and buffers last as this might trigger events that might access other state of the REPL window:
            RemoveProtection(_outputBuffer, _outputProtection);
            RemoveProtection(_stdInputBuffer, _stdInputProtection);

            using (var edit = _outputBuffer.CreateEdit(EditOptions.None, null, SuppressPromptInjectionTag)) {
                edit.Delete(0, _outputBuffer.CurrentSnapshot.Length);
                edit.Apply();
            }
            _addedLineBreakOnLastOutput = false;
            using (var edit = _stdInputBuffer.CreateEdit(EditOptions.None, null, SuppressPromptInjectionTag)) {
                edit.Delete(0, _stdInputBuffer.CurrentSnapshot.Length);
                edit.Apply();
            }

            ClearProjection();

            if (insertInputPrompt) {
                PrepareForInput();
            }
        }

        /// <summary>
        /// See IReplWindow
        /// </summary>
        public void Focus() {
            var textView = TextView;

            IInputElement input = textView as IInputElement;
            if (input != null) {
                Keyboard.Focus(input);
            }
        }

        public void InsertCode(string text) {
            if (!CheckAccess()) {
                Dispatcher.BeginInvoke(new Action(() => InsertCode(text)));
                return;
            }

            if (_stdInputStart == null) {
                if (_isRunning) {
                    AppendUncommittedInput(text);
                } else {
                    if (!TextView.Selection.IsEmpty) {
                        CutOrDeleteSelection(false);
                    }
                    _editorOperations.InsertText(text);
                }
            }
        }

        public void Submit(IEnumerable<string> inputs) {
            if (!CheckAccess()) {
                Dispatcher.BeginInvoke(new Action(() => Submit(inputs)));
                return;
            }

            if (_stdInputStart == null) {
                if (!_isRunning && _currentLanguageBuffer != null) {
                    StoreUncommittedInput();
                    PendSubmissions(inputs);
                    ProcessPendingSubmissions();
                } else {
                    PendSubmissions(inputs);
                }
            }
        }

        private void PendSubmissions(IEnumerable<string> inputs) {
            if (_pendingSubmissions == null) {
                _pendingSubmissions = new Queue<string>();
            }

            foreach (var input in inputs) {
                _pendingSubmissions.Enqueue(input);
            }
        }
        
        /// <summary>
        /// See IReplWindow
        /// </summary>
        public Task<ExecutionResult> Reset() {
            if (_stdInputStart != null) {
                UIThread(CancelStandardInput);
            }
            
            return Evaluator.Reset().
                ContinueWith(completed => {
                    // flush output produced by the process before it was killed:
                    _buffer.Flush();

                    return completed.Result;
                }, _uiScheduler);
        }

        public void AbortCommand() {
            if (_isRunning) {
                Evaluator.AbortCommand();
            } else {
                UIThread(() => {
                    // finish line of the current std input buffer or language buffer:
                    if (InStandardInputRegion(new SnapshotPoint(CurrentSnapshot, CurrentSnapshot.Length))) {
                        CancelStandardInput();
                    } else if (_currentLanguageBuffer != null) {
                        AppendLineNoPromptInjection(_currentLanguageBuffer);
                        PrepareForInput();
                    }
                });
            }
        }

        /// <summary>
        /// Sets the current value for the specified option.
        /// </summary>
        public void SetOptionValue(ReplOptions option, object value) {
            ExceptionDispatchInfo toThrow = null;
            UIThread(() => {
                try {
                    switch (option) {
                        case ReplOptions.CommandPrefix: 
                            _commandPrefix = CheckOption<string>(option, value); 
                            break;

                        case ReplOptions.DisplayPromptInMargin:
                            bool prevValue = _displayPromptInMargin;
                            _displayPromptInMargin = CheckOption<bool>(option, value);

                            if (prevValue != _displayPromptInMargin) {
                                var marginChanged = MarginVisibilityChanged;
                                if (marginChanged != null) {
                                    marginChanged();
                                }

                                if (_displayPromptInMargin) {
                                    UpdatePrompts(ReplSpanKind.Prompt, _prompt, String.Empty);
                                    UpdatePrompts(ReplSpanKind.SecondaryPrompt, _secondPrompt, String.Empty);
                                } else {
                                    UpdatePrompts(ReplSpanKind.Prompt, String.Empty, _prompt);
                                    UpdatePrompts(ReplSpanKind.SecondaryPrompt, String.Empty, _secondPrompt);
                                }
                            }
                            break;

                        case ReplOptions.CurrentPrimaryPrompt:
                            string oldPrompt = _prompt;
                            _prompt = CheckOption<string>(option, value);
                            if (!_isRunning && !_displayPromptInMargin) {
                                // we need to update the current prompt though
                                UpdatePrompts(ReplSpanKind.Prompt, oldPrompt, _prompt, currentOnly: true);
                            }
                            break;
                        case ReplOptions.CurrentSecondaryPrompt:
                            oldPrompt = _secondPrompt;
                            _secondPrompt = CheckOption<string>(option, value) ?? String.Empty;
                            if (!_isRunning && !_displayPromptInMargin) {
                                // we need to update the current prompt though
                                UpdatePrompts(ReplSpanKind.SecondaryPrompt, oldPrompt, _secondPrompt, currentOnly: true);
                            }
                            break;
                        case ReplOptions.PrimaryPrompt:
                            if (value == null) {
                                throw new InvalidOperationException("Primary prompt cannot be null");
                            }
                            oldPrompt = _prompt;
                            _prompt = CheckOption<string>(option, value);
                            if (!_displayPromptInMargin) {
                                // update the prompts
                                UpdatePrompts(ReplSpanKind.Prompt, oldPrompt, _prompt);
                            }
                            break;

                        case ReplOptions.SecondaryPrompt:
                            oldPrompt = _secondPrompt;
                            _secondPrompt = CheckOption<string>(option, value) ?? String.Empty;
                            if (!_displayPromptInMargin) {
                                UpdatePrompts(ReplSpanKind.SecondaryPrompt, oldPrompt, _secondPrompt);
                            }
                            break;

                        case ReplOptions.StandardInputPrompt:
                            if (value == null) {
                                throw new InvalidOperationException("Primary prompt cannot be null");
                            }
                            oldPrompt = _stdInputPrompt;
                            _stdInputPrompt = CheckOption<string>(option, value);
                            if (!_displayPromptInMargin) {
                                // update the prompts
                                UpdatePrompts(ReplSpanKind.StandardInputPrompt, oldPrompt, _stdInputPrompt);
                            }
                            break;

                        case ReplOptions.UseSmartUpDown: 
                            _useSmartUpDown = CheckOption<bool>(option, value); 
                            break;

                        case ReplOptions.ShowOutput:
                            _buffer.Flush();
                            _showOutput = CheckOption<bool>(option, value);
                            break;

                        case ReplOptions.SupportAnsiColors:
                            _buffer.Flush();
                            _buffer.ProcessAnsiEscapes = CheckOption<bool>(option, value);
                            break;

                        case ReplOptions.FormattedPrompts:
                            bool oldFormattedPrompts = _formattedPrompts;
                            _formattedPrompts = CheckOption<bool>(option, value);
                            if (oldFormattedPrompts != _formattedPrompts) {
                                UpdatePrompts(ReplSpanKind.StandardInputPrompt, null, _stdInputPrompt);
                                UpdatePrompts(ReplSpanKind.Prompt, null, _prompt);
                                UpdatePrompts(ReplSpanKind.SecondaryPrompt, null, _secondPrompt);
                            }
                            break;

                        default:
                            throw new InvalidOperationException(String.Format("Unknown option: {0}", option));
                    }
                } catch (Exception e) {
                    toThrow = ExceptionDispatchInfo.Capture(e);
                }
            });
            if (toThrow != null) {
                // throw exception on original thread, not the UI thread.
                toThrow.Throw();
            }
        }

        private T CheckOption<T>(ReplOptions option, object o) {
            if (!(o is T)) {
                throw new InvalidOperationException(String.Format(
                    "Got wrong type ({0}) for option {1}",
                    o == null ? "null" : o.GetType().Name,
                    option.ToString())
                );
            }

            return (T)o;
        }

        /// <summary>
        /// Gets the current value for the specified option.
        /// </summary>
        public object GetOptionValue(ReplOptions option) {
            switch (option) {
                case ReplOptions.CommandPrefix: return _commandPrefix;
                case ReplOptions.DisplayPromptInMargin: return _displayPromptInMargin;
                case ReplOptions.CurrentPrimaryPrompt:
                case ReplOptions.PrimaryPrompt: return _prompt;
                case ReplOptions.CurrentSecondaryPrompt:
                case ReplOptions.SecondaryPrompt: return _secondPrompt;
                case ReplOptions.StandardInputPrompt: return _stdInputPrompt;
                case ReplOptions.ShowOutput: return _showOutput;
                case ReplOptions.UseSmartUpDown: return _useSmartUpDown;
                case ReplOptions.SupportAnsiColors: return _buffer.ProcessAnsiEscapes;
                case ReplOptions.FormattedPrompts: return _formattedPrompts;
                default:
                    throw new InvalidOperationException(String.Format("Unknown option: {0}", option));
            }
        }
        
        internal string GetPromptText(ReplSpanKind kind) {
            switch (kind) {
                case ReplSpanKind.Prompt:
                    return _prompt;

                case ReplSpanKind.SecondaryPrompt:
                    return _secondPrompt;

                case ReplSpanKind.StandardInputPrompt:
                    return _stdInputPrompt;

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Trigerred when prompt margin visibility should change.
        /// </summary>
        internal event Action MarginVisibilityChanged;

        internal bool DisplayPromptInMargin {
            get { return _displayPromptInMargin; }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Clears the current input
        /// </summary>
        public void Cancel() {
            ClearInput();
            _editorOperations.MoveToEndOfDocument(false);
            _uncommittedInput = null;
        }

        private void HistoryPrevious(string search = null) {
            RequiresLanguageBuffer();

            var previous = _history.GetPrevious(search);
            if (previous != null) {
                if (String.IsNullOrWhiteSpace(search)) {
                    // don't store search as an uncommited history item
                    StoreUncommittedInputForHistory();
                }
                SetActiveCode(previous);
            }
        }

        private void HistoryNext(string search = null) {
            RequiresLanguageBuffer();

            var next = _history.GetNext(search);
            if (next != null) {
                if (String.IsNullOrWhiteSpace(search)) {
                    // don't store search as an uncommited history item
                    StoreUncommittedInputForHistory();
                }
                SetActiveCode(next);
            } else {
                string code = _history.UncommittedInput;
                _history.UncommittedInput = null;
                if (!String.IsNullOrEmpty(code)) {
                    SetActiveCode(code);
                }
            }
        }

        public void SearchHistoryPrevious() {
            if (_historySearch == null) {
                _historySearch = GetActiveCode();
            }

            HistoryPrevious(_historySearch);
        }

        public void SearchHistoryNext() {
            RequiresLanguageBuffer();

            if (_historySearch == null) {
                _historySearch = GetActiveCode();
            }

            HistoryNext(_historySearch);
        }

        private void StoreUncommittedInputForHistory() {
            if (_history.UncommittedInput == null) {
                string activeCode = GetActiveCode();
                if (activeCode.Length > 0) {
                    _history.UncommittedInput = activeCode;
                }
            }
        }

        /// <summary>
        /// Moves to the beginning of the line.
        /// </summary>
        private void Home(bool extendSelection) {
            var caret = Caret;

            // map the end of the current language line (if applicable).
            var langLineEndPoint = TextView.BufferGraph.MapDownToFirstMatch(
                caret.Position.BufferPosition.GetContainingLine().End,
                PointTrackingMode.Positive,
                x => x.TextBuffer.ContentType == _languageContentType,
                PositionAffinity.Successor);

            if (langLineEndPoint == null) {
                // we're on some random line that doesn't include language buffer, just go to the start of the buffer
                _editorOperations.MoveToStartOfLine(extendSelection);
            } else {
                var projectionLine = caret.Position.BufferPosition.GetContainingLine();
                ITextSnapshotLine langLine = langLineEndPoint.Value.Snapshot.GetLineFromPosition(langLineEndPoint.Value.Position);

                var projectionPoint = TextView.BufferGraph.MapUpToBuffer(
                    langLine.Start, 
                    PointTrackingMode.Positive, 
                    PositionAffinity.Successor, 
                    _projectionBuffer
                );
                if (projectionPoint == null) {
                    throw new InvalidOperationException("Could not map langLine to buffer");
                }

                //
                // If the caret is already at the first non-whitespace character or
                // the line is entirely whitepsace, move to the start of the view line.
                // See (EditorOperations.MoveToHome).
                //
                // If the caret is in the prompt move the caret to the begining of the language line.
                //
                        
                int firstNonWhiteSpace = IndexOfNonWhiteSpaceCharacter(langLine);
                SnapshotPoint moveTo;
                if (firstNonWhiteSpace == -1 || 
                    projectionPoint.Value.Position + firstNonWhiteSpace == caret.Position.BufferPosition ||
                    caret.Position.BufferPosition < projectionPoint.Value.Position) {
                    moveTo = projectionPoint.Value;
                } else {
                    moveTo = projectionPoint.Value + firstNonWhiteSpace;
                }

                if (extendSelection) {
                    VirtualSnapshotPoint anchor = TextView.Selection.AnchorPoint;
                    caret.MoveTo(moveTo);
                    TextView.Selection.Select(anchor.TranslateTo(TextView.TextSnapshot), TextView.Caret.Position.VirtualBufferPosition);
                } else {
                    TextView.Selection.Clear();
                    caret.MoveTo(moveTo);
                }
            }                
        }

        /// <summary>
        /// Moves to the end of the line.
        /// </summary>
        private void End(bool extendSelection) {
            var caret = Caret;

            // map the end of the current language line (if applicable).
            var langLineEndPoint = TextView.BufferGraph.MapDownToFirstMatch(
                caret.Position.BufferPosition.GetContainingLine().End,
                PointTrackingMode.Positive,
                x => x.TextBuffer.ContentType == _languageContentType,
                PositionAffinity.Successor);

            if (langLineEndPoint == null) {
                // we're on some random line that doesn't include language buffer, just go to the start of the buffer
                _editorOperations.MoveToEndOfLine(extendSelection);
            } else {
                var projectionLine = caret.Position.BufferPosition.GetContainingLine();
                ITextSnapshotLine langLine = langLineEndPoint.Value.Snapshot.GetLineFromPosition(langLineEndPoint.Value.Position);

                var projectionPoint = TextView.BufferGraph.MapUpToBuffer(
                    langLine.End,
                    PointTrackingMode.Positive,
                    PositionAffinity.Successor,
                    _projectionBuffer
                );
                if (projectionPoint == null) {
                    throw new InvalidOperationException("Could not map langLine to buffer");
                }

                var moveTo = projectionPoint.Value;

                if (extendSelection) {
                    VirtualSnapshotPoint anchor = TextView.Selection.AnchorPoint;
                    caret.MoveTo(moveTo);
                    TextView.Selection.Select(anchor.TranslateTo(TextView.TextSnapshot), TextView.Caret.Position.VirtualBufferPosition);
                } else {
                    TextView.Selection.Clear();
                    caret.MoveTo(moveTo);
                }
            }
        }

        private void SelectAll() {
            SnapshotSpan? span = GetContainingRegion(TextView.Caret.Position.BufferPosition);

            // if the span is already selected select all text in the projection buffer:
            if (span == null || TextView.Selection.SelectedSpans.Count == 1 && TextView.Selection.SelectedSpans[0] == span.Value) {
                span = new SnapshotSpan(TextBuffer.CurrentSnapshot, new Span(0, TextBuffer.CurrentSnapshot.Length));
            }

            TextView.Selection.Select(span.Value, isReversed: false);
        }

        /// <summary>
        /// Given a point in projection buffer calculate a span that includes the point and comprises of 
        /// subsequent projection spans forming a region, i.e. a sequence of output spans in between two subsequent submissions,
        /// a language input block, or standard input block.
        /// 
        /// Internal for testing.
        /// </summary>
        internal SnapshotSpan? GetContainingRegion(SnapshotPoint point) {
            if (_promptLineMapping == null || _promptLineMapping.Count == 0 || _projectionSpans.Count == 0) {
                return null;
            }

            int closestPrecedingPrimaryPromptIndex = GetPromptMappingIndex(point.GetContainingLine().LineNumber);
            ReplSpan projectionSpan = _projectionSpans[_promptLineMapping[closestPrecedingPrimaryPromptIndex].Value + 1];

            Debug.Assert(projectionSpan.Kind == ReplSpanKind.Language || projectionSpan.Kind == ReplSpanKind.StandardInput);
            var inputSnapshot = projectionSpan.TrackingSpan.TextBuffer.CurrentSnapshot;

            // Language input block is a projection of the entire snapshot;
            // std input block is a projection of a single span:
            SnapshotPoint inputBufferEnd = (projectionSpan.Kind == ReplSpanKind.Language) ?
                new SnapshotPoint(inputSnapshot, inputSnapshot.Length) :
                projectionSpan.TrackingSpan.GetEndPoint(inputSnapshot);

            SnapshotPoint projectedInputBufferEnd = TextView.BufferGraph.MapUpToBuffer(
                inputBufferEnd,
                PointTrackingMode.Positive,
                PositionAffinity.Predecessor,
                TextBuffer
            ).Value;

            // point is between the primary prompt (including) and the last character of the corresponding language/stdin buffer:
            if (point <= projectedInputBufferEnd) {
                var projectedLanguageBufferStart = TextView.BufferGraph.MapUpToBuffer(
                    new SnapshotPoint(inputSnapshot, 0),
                    PointTrackingMode.Positive,
                    PositionAffinity.Successor,
                    TextBuffer
                ).Value;

                var promptProjectionSpan = _projectionSpans[_promptLineMapping[closestPrecedingPrimaryPromptIndex].Value];
                if (point < projectedLanguageBufferStart - promptProjectionSpan.Length) {
                    // cursor is before the first language buffer:
                    return new SnapshotSpan(new SnapshotPoint(TextBuffer.CurrentSnapshot, 0), projectedLanguageBufferStart - promptProjectionSpan.Length);
                }

                // cursor is within the language buffer:
                return new SnapshotSpan(projectedLanguageBufferStart, projectedInputBufferEnd);
            }

            // this was the last primary/stdin prompt - select the part of the projection buffer behind the end of the language/stdin buffer:
            if (closestPrecedingPrimaryPromptIndex + 1 == _promptLineMapping.Count) {
                return new SnapshotSpan(
                    projectedInputBufferEnd,
                    new SnapshotPoint(TextBuffer.CurrentSnapshot, TextBuffer.CurrentSnapshot.Length)
                );
            }

            ReplSpan lastSpanBeforeNextPrompt = _projectionSpans[_promptLineMapping[closestPrecedingPrimaryPromptIndex + 1].Value - 1];
            Debug.Assert(lastSpanBeforeNextPrompt.Kind == ReplSpanKind.Output);

            // select all text in between the language buffer and the next prompt:
            var trackingSpan = lastSpanBeforeNextPrompt.TrackingSpan;
            return new SnapshotSpan(
                projectedInputBufferEnd,
                TextView.BufferGraph.MapUpToBuffer(
                    trackingSpan.GetEndPoint(trackingSpan.TextBuffer.CurrentSnapshot),
                    PointTrackingMode.Positive,
                    PositionAffinity.Predecessor,
                    TextBuffer
                ).Value
            );
        }
        
        /// <summary>
        /// Pastes from the clipboard into the text view
        /// </summary>
        public bool PasteClipboard() {
            return UIThread(() => {
                string format = _evaluator.FormatClipboard();
                if (format != null) {
                    InsertCode(format);
                } else if (Clipboard.ContainsText()) {
                    InsertCode(Clipboard.GetText());
                } else {
                    return false;
                }
                return true;
            });
        }

        /// <summary>
        /// Indents the line where the caret is currently located.
        /// </summary>
        /// <remarks>
        /// We don't send this command to the editor since smart indentation doesn't work along with BufferChanged event.
        /// Instead, we need to implement indentation ourselves. We still use ISmartIndentProvider provided by the languge.
        /// </remarks>
        private void IndentCurrentLine() {
            Debug.Assert(_currentLanguageBuffer != null);

            var langCaret = TextView.BufferGraph.MapDownToBuffer(
                Caret.Position.BufferPosition,
                PointTrackingMode.Positive,
                _currentLanguageBuffer,
                PositionAffinity.Successor);

            if (langCaret == null) {
                return;
            }

            ITextSnapshotLine langLine = langCaret.Value.GetContainingLine();
            int? langIndentation = _languageIndenter.GetDesiredIndentation(langLine);

            if (langIndentation != null) {
                if (langCaret.Value == langLine.End) {
                    // create virtual space:
                    TextView.Caret.MoveTo(new VirtualSnapshotPoint(Caret.Position.BufferPosition, langIndentation.Value));
                } else {
                    // insert whitespace indentation:
                    string whitespace = GetWhiteSpaceForVirtualSpace(langIndentation.Value);
                    _currentLanguageBuffer.Insert(langCaret.Value, whitespace);
                }
            }
        }

        // Mimics EditorOperations.GetWhiteSpaceForPositionAndVirtualSpace.
        private string GetWhiteSpaceForVirtualSpace(int virtualSpaces) {
            string textToInsert;
            if (!TextView.Options.IsConvertTabsToSpacesEnabled()) {
                int tabSize = TextView.Options.GetTabSize();

                int spacesAfterPreviousTabStop = virtualSpaces % tabSize;
                int columnOfPreviousTabStop = virtualSpaces - spacesAfterPreviousTabStop;

                int requiredTabs = (columnOfPreviousTabStop + tabSize - 1) / tabSize;

                if (requiredTabs > 0) {
                    textToInsert = new string('\t', requiredTabs) + new string(' ', spacesAfterPreviousTabStop);
                } else {
                    textToInsert = new string(' ', virtualSpaces);
                }
            } else {
                textToInsert = new string(' ', virtualSpaces);
            }

            return textToInsert;
        }

        /// <summary>
        /// Deletes characters preceeding the current caret position in the current language buffer.
        /// </summary>
        private void DeletePreviousCharacter() {
            SnapshotPoint? point = MapToEditableBuffer(TextView.Caret.Position.BufferPosition);

            // We are not in an editable buffer, or we are at the start of the buffer, nothing to delete.
            if (point == null || point.Value == 0) {
                return;
            }

            var line = point.Value.GetContainingLine();
            int characterSize;
            if (line.Start.Position == point.Value.Position) {
                Debug.Assert(line.LineNumber != 0);
                characterSize = line.Snapshot.GetLineFromLineNumber(line.LineNumber - 1).LineBreakLength;
            } else {
                characterSize = 1;
            }

            point.Value.Snapshot.TextBuffer.Delete(new Span(point.Value.Position - characterSize, characterSize));
        }

        /// <summary>
        /// Deletes currently selected text from the language buffer and optionally saves it to the clipboard.
        /// </summary>
        private void CutOrDeleteSelection(bool isCut) {
            Debug.Assert(_currentLanguageBuffer != null);

            StringBuilder deletedText = null;

            // split into multiple deletes that only affect the language/input buffer:
            ITextBuffer affectedBuffer = (_stdInputStart != null) ? _stdInputBuffer : _currentLanguageBuffer;
            using (var edit = affectedBuffer.CreateEdit()) {
                foreach (var projectionSpan in TextView.Selection.SelectedSpans) {
                    var spans = TextView.BufferGraph.MapDownToBuffer(projectionSpan, SpanTrackingMode.EdgeInclusive, affectedBuffer);
                    foreach (var span in spans) {
                        if (isCut) {
                            if (deletedText == null) {
                                deletedText = new StringBuilder();
                            }
                            deletedText.Append(span.GetText());
                        }
                        edit.Delete(span);
                    }
                }
                edit.Apply();
            }

            // copy the deleted text to the clipboard:
            if (deletedText != null) {
                var data = new DataObject();
                if (TextView.Selection.Mode == TextSelectionMode.Box) {
                    data.SetData(_boxSelectionCutCopyTag, new object());
                }

                data.SetText(deletedText.ToString());
                Clipboard.SetDataObject(data, true);
            }

            // if the selection spans over prompts the prompts remain selected, so clear manually:
            TextView.Selection.Clear();
        }

        public void ShowContextMenu() {
            var uishell = (IVsUIShell)GetService(typeof(SVsUIShell));
            if (uishell != null) {
                var pt = System.Windows.Forms.Cursor.Position;
                var pnts = new[] { new POINTS { x = (short)pt.X, y = (short)pt.Y } };
                var guid = Guids.guidReplWindowCmdSet;
                int hr = uishell.ShowContextMenu(
                    0,
                    ref guid,
                    0x2100,
                    pnts,
                    TextView as IOleCommandTarget);

                ErrorHandler.ThrowOnFailure(hr);
            }
        }

        #endregion

        #region Command Filters

        // LineBreak is sent as RETURN to language services. We set this flag to distinguish LineBreak from RETURN 
        // when we receive it back in post-language command filter.
        private bool ReturnIsLineBreak;

        private const uint CommandEnabled = (uint)(OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_DEFHIDEONCTXTMENU);
        private const uint CommandDisabled = (uint)(OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_DEFHIDEONCTXTMENU);
        private const uint CommandDisabledAndHidden = (uint)(OLECMDF.OLECMDF_INVISIBLE | OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_DEFHIDEONCTXTMENU);

        private enum CommandFilterLayer {
            PreLanguage,
            PostLanguage,
            PreEditor
        }

        private sealed class CommandFilter : IOleCommandTarget {
            private readonly ReplWindow _replWindow;
            private readonly CommandFilterLayer _layer;

            public CommandFilter(ReplWindow vsReplWindow, CommandFilterLayer layer) {
                _replWindow = vsReplWindow;
                _layer = layer;
            }

            public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {
                switch (_layer) {
                    case CommandFilterLayer.PreLanguage:
                        return _replWindow.PreLanguageCommandFilterQueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);

                    case CommandFilterLayer.PostLanguage:
                        return _replWindow.PostLanguageCommandFilterQueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);

                    case CommandFilterLayer.PreEditor:
                        return _replWindow.PreEditorCommandFilterQueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
                }

                throw new InvalidOperationException();
            }

            public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
                switch (_layer) {
                    case CommandFilterLayer.PreLanguage:
                        return _replWindow.PreLanguageCommandFilterExec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

                    case CommandFilterLayer.PostLanguage:
                        return _replWindow.PostLanguageCommandFilterExec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

                    case CommandFilterLayer.PreEditor:
                        return _replWindow.PreEditorCommandFilterExec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
                }

                throw new InvalidOperationException();
            }
        }

        #region Window IOleCommandTarget

        private IOleCommandTarget TextViewCommandFilterChain {
            get {
                // Non-character command processing starts with WindowFrame which calls ReplWindow.Exec.
                // We need to invoke the view's Exec method in order to invoke its full command chain 
                // (features add their filters to the view).
                return (IOleCommandTarget)_view;
            }
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {
            var nextTarget = TextViewCommandFilterChain;

            if (pguidCmdGroup == Guids.guidReplWindowCmdSet) {
                switch (prgCmds[0].cmdID) {
                    case PkgCmdIDList.cmdidReplSearchHistoryNext:
                    case PkgCmdIDList.cmdidReplSearchHistoryPrevious:
                    case PkgCmdIDList.cmdidReplHistoryNext:
                    case PkgCmdIDList.cmdidReplHistoryPrevious:
                    case PkgCmdIDList.cmdidSmartExecute:
                        prgCmds[0].cmdf = (_currentLanguageBuffer != null) ? CommandEnabled : CommandDisabledAndHidden;
                        return VSConstants.S_OK;

                    case PkgCmdIDList.comboIdReplScopes:
                        prgCmds[0].cmdf = _scopeListVisible ? CommandEnabled : CommandDisabledAndHidden;
                        return VSConstants.S_OK;

                    case PkgCmdIDList.cmdidBreakRepl:
                        prgCmds[0].cmdf = _isRunning || _stdInputStart != null ? CommandEnabled : CommandDisabled;
                        return VSConstants.S_OK;

                    case PkgCmdIDList.cmdidResetRepl:
                        prgCmds[0].cmdf = _commands.OfType<ResetReplCommand>().Count() != 0 ? CommandEnabled : CommandDisabledAndHidden;
                        return VSConstants.S_OK;
                }
            }

            return nextTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            var nextTarget = TextViewCommandFilterChain;

            if (pguidCmdGroup == Guids.guidReplWindowCmdSet) {
                switch (nCmdID) {
                    case PkgCmdIDList.cmdidBreakRepl:
                        AbortCommand();
                        return VSConstants.S_OK;

                    case PkgCmdIDList.cmdidResetRepl:
                        Reset();
                        return VSConstants.S_OK;

                    case PkgCmdIDList.cmdidSmartExecute:
                        Debug.Assert(_currentLanguageBuffer != null);
                        SmartExecute();
                        return VSConstants.S_OK;

                    case PkgCmdIDList.cmdidReplHistoryNext:
                        Debug.Assert(_currentLanguageBuffer != null);
                        HistoryNext();
                        return VSConstants.S_OK;

                    case PkgCmdIDList.cmdidReplHistoryPrevious:
                        Debug.Assert(_currentLanguageBuffer != null);
                        HistoryPrevious();
                        return VSConstants.S_OK;

                    case PkgCmdIDList.cmdidReplSearchHistoryNext:
                        Debug.Assert(_currentLanguageBuffer != null);
                        SearchHistoryNext();
                        return VSConstants.S_OK;

                    case PkgCmdIDList.cmdidReplSearchHistoryPrevious:
                        Debug.Assert(_currentLanguageBuffer != null);
                        SearchHistoryPrevious();
                        return VSConstants.S_OK;

                    case PkgCmdIDList.cmdidReplClearScreen:
                        ClearScreen(insertInputPrompt: !_isRunning);
                        return VSConstants.S_OK;

                    case PkgCmdIDList.comboIdReplScopes:
                        ScopeComboBoxHandler(pvaIn, pvaOut);
                        return VSConstants.S_OK;

                    case PkgCmdIDList.comboIdReplScopesGetList:
                        ScopeComboBoxGetList(pvaOut);
                        return VSConstants.S_OK;
                }
            }

            return nextTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        #endregion

        #region Pre-langauge service IOleCommandTarget

        private int PreLanguageCommandFilterQueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {
            var nextTarget = _languageServiceCommandFilter;

            if (pguidCmdGroup == Guids.guidReplWindowCmdSet) {
                switch (prgCmds[0].cmdID) {
                    case PkgCmdIDList.cmdidBreakLine:
                        prgCmds[0].cmdf = CommandEnabled;
                        return VSConstants.S_OK;
                }
            }

            return nextTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        private int PreLanguageCommandFilterExec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            var nextTarget = _languageServiceCommandFilter;

            if (pguidCmdGroup == VSConstants.VSStd2K) {
                switch ((VSConstants.VSStd2KCmdID)nCmdID) {
                    case VSConstants.VSStd2KCmdID.RETURN:
                        _historySearch = null;
                        if (_stdInputStart != null) {
                            if (InStandardInputRegion(TextView.Caret.Position.BufferPosition)) {
                                SubmitStandardInput();
                            }
                            return VSConstants.S_OK;
                        }
                        
                        ReturnIsLineBreak = false;
                        break;

                    case VSConstants.VSStd2KCmdID.SHOWCONTEXTMENU:
                        ShowContextMenu();
                        return VSConstants.S_OK;

                    case VSConstants.VSStd2KCmdID.TYPECHAR:
                        _historySearch = null;
                        char typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
                        if (!(_stdInputStart != null ? CaretInStandardInputRegion : CaretInActiveCodeRegion)) {
                            MoveCaretToCurrentInputEnd();
                        }

                        if (!TextView.Selection.IsEmpty) {
                            // delete selected text first
                            CutOrDeleteSelection(false);
                        }
                        break;
                }
            } else if (pguidCmdGroup == Guids.guidReplWindowCmdSet) {
                switch (nCmdID) {
                    case PkgCmdIDList.cmdidBreakLine:
                        // map to RETURN, so that IntelliSense and other services treat this as a new line
                        Guid group = VSConstants.VSStd2K;
                        ReturnIsLineBreak = true;
                        return nextTarget.Exec(ref group, (int)VSConstants.VSStd2KCmdID.RETURN, 0, IntPtr.Zero, IntPtr.Zero);
                }
            }

            return nextTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        /// <summary>
        /// Moves the caret to the end of the current input.  Called when the user types
        /// outside of an input line.
        /// </summary>
        private void MoveCaretToCurrentInputEnd() {
            // TODO (tomat): this is strange - we should rather find the next writable span
            EditorOperations.MoveToEndOfDocument(false);
        }

        #endregion

        #region Post-language service IOleCommandTarget

        private int PostLanguageCommandFilterQueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {
            return _editorServicesCommandFilter.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        private int PostLanguageCommandFilterExec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            var nextTarget = _editorServicesCommandFilter;
            return nextTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        #endregion

        #region Pre-Editor service IOleCommandTarget

        private int PreEditorCommandFilterQueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText) {
            return _editorCommandFilter.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        private int PreEditorCommandFilterExec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            var nextTarget = _editorCommandFilter;

            if (pguidCmdGroup == VSConstants.VSStd2K) {
                switch ((VSConstants.VSStd2KCmdID)nCmdID) {
                    case VSConstants.VSStd2KCmdID.RETURN:
                        if (_currentLanguageBuffer == null) {
                            break;
                        }

                        // RETURN might be sent by LineBreak command:
                        bool trySubmit = !ReturnIsLineBreak;
                        ReturnIsLineBreak = false;

                        // RETURN that is not handled by any language or editor service is a "try submit" command

                        var langCaret = TextView.BufferGraph.MapDownToBuffer(
                            Caret.Position.BufferPosition,
                            PointTrackingMode.Positive,
                            _currentLanguageBuffer,
                            PositionAffinity.Successor
                        );

                        if (langCaret != null) {
                            if (trySubmit && CanExecuteActiveCode()) {
                                Submit();
                                return VSConstants.S_OK;
                            }

                            // insert new line (triggers secondary prompt injection in buffer changed event):
                            _currentLanguageBuffer.Insert(langCaret.Value.Position, GetLineBreak());
                            IndentCurrentLine();

                            return VSConstants.S_OK;
                        } else if (!CaretInStandardInputRegion) {
                            MoveCaretToCurrentInputEnd();
                            return VSConstants.S_OK;
                        }
                        break;

                    // TODO: 
                    //case VSConstants.VSStd2KCmdID.DELETEWORDLEFT:
                    //case VSConstants.VSStd2KCmdID.DELETEWORDRIGHT:
                    //    break;

                    case VSConstants.VSStd2KCmdID.BACKSPACE:
                        if (!TextView.Selection.IsEmpty) {
                            CutOrDeleteSelection(false);
                            return VSConstants.S_OK;
                        }

                        if (TextView.Caret.Position.VirtualSpaces == 0) {
                            DeletePreviousCharacter();
                            return VSConstants.S_OK;
                        }

                        break;

                    case VSConstants.VSStd2KCmdID.UP:
                        // UP at the end of input or with empty input rotate history:
                        if (_currentLanguageBuffer != null && !_isRunning && CaretAtEnd && _useSmartUpDown) {
                            HistoryPrevious();
                            return VSConstants.S_OK;
                        }

                        break;

                    case VSConstants.VSStd2KCmdID.DOWN:
                        // DOWN at the end of input or with empty input rotate history:
                        if (_currentLanguageBuffer != null && !_isRunning && CaretAtEnd && _useSmartUpDown) {
                            HistoryNext();
                            return VSConstants.S_OK;
                        }

                        break;

                    case VSConstants.VSStd2KCmdID.CANCEL:
                        _historySearch = null;
                        Cancel();
                        break;

                    case VSConstants.VSStd2KCmdID.BOL:
                        Home(false);
                        return VSConstants.S_OK;

                    case VSConstants.VSStd2KCmdID.BOL_EXT:
                        Home(true);
                        return VSConstants.S_OK;

                    case VSConstants.VSStd2KCmdID.EOL:
                        End(false);
                        return VSConstants.S_OK;

                    case VSConstants.VSStd2KCmdID.EOL_EXT:
                        End(true);
                        return VSConstants.S_OK;
                }
            } else if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97) {
                switch ((VSConstants.VSStd97CmdID)nCmdID) {
                    case VSConstants.VSStd97CmdID.Paste:
                        if (!(_stdInputStart != null ? CaretInStandardInputRegion : CaretInActiveCodeRegion)) {
                            MoveCaretToCurrentInputEnd();
                        }
                        PasteClipboard();
                        return VSConstants.S_OK;

                    case VSConstants.VSStd97CmdID.Cut:
                        if (!TextView.Selection.IsEmpty) {
                            CutOrDeleteSelection(true);
                            return VSConstants.S_OK;
                        }
                        break;

                    case VSConstants.VSStd97CmdID.Delete:
                        if (!TextView.Selection.IsEmpty) {
                            CutOrDeleteSelection(false);
                            return VSConstants.S_OK;
                        }
                        break;

                    case VSConstants.VSStd97CmdID.SelectAll:
                        SelectAll();
                        return VSConstants.S_OK;
                }
            }

            return nextTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        #endregion
        
        #endregion

        #region Caret and Cursor

        private ITextCaret Caret {
            get { return TextView.Caret; }
        }

        private bool CaretAtEnd {
            get { return Caret.Position.BufferPosition.Position == CurrentSnapshot.Length; }
        }

        public bool CaretInActiveCodeRegion {
            get {
                if (_currentLanguageBuffer == null) {
                    return false;
                }

                var point = TextView.BufferGraph.MapDownToBuffer(
                    Caret.Position.BufferPosition,
                    PointTrackingMode.Positive,
                    _currentLanguageBuffer,
                    PositionAffinity.Successor
                );

                return point != null;
            }
        }

        public bool CaretInStandardInputRegion {
            get {
                if (_stdInputBuffer == null) {
                    return false;
                }

                var point = TextView.BufferGraph.MapDownToBuffer(
                    Caret.Position.BufferPosition,
                    PointTrackingMode.Positive,
                    _stdInputBuffer,
                    PositionAffinity.Successor
                );

                return point != null;
            }
        }

        /// <summary>
        /// Maps point to the current language buffer or standard input buffer.
        /// </summary>
        private SnapshotPoint? MapToEditableBuffer(SnapshotPoint projectionPoint) {
            SnapshotPoint? result = null;

            if (_currentLanguageBuffer != null) {
                result = TextView.BufferGraph.MapDownToBuffer(
                    projectionPoint, PointTrackingMode.Positive, _currentLanguageBuffer, PositionAffinity.Successor
                );
            }

            if (result != null) {
                return result;
            }

            if (_stdInputBuffer != null) {
                result = TextView.BufferGraph.MapDownToBuffer(
                    projectionPoint, PointTrackingMode.Positive, _stdInputBuffer, PositionAffinity.Successor
                );
            }

            return result;
        }

        private ITextSnapshot GetCodeSnapshot(SnapshotPoint projectionPosition) {
            var pt = TextView.BufferGraph.MapDownToFirstMatch(
                projectionPosition,
                PointTrackingMode.Positive,
                x => x.TextBuffer.ContentType == _languageContentType,
                PositionAffinity.Successor
            );

            return pt != null ? pt.Value.Snapshot : null;
        }

        /// <summary>
        /// Returns the insertion point relative to the current language buffer.
        /// </summary>
        private int GetActiveCodeInsertionPosition() {
            Debug.Assert(_currentLanguageBuffer != null);

            var langPoint = _textViewHost.TextView.BufferGraph.MapDownToBuffer(
                new SnapshotPoint(
                    _projectionBuffer.CurrentSnapshot,
                    Caret.Position.BufferPosition.Position
                ),
                PointTrackingMode.Positive,
                _currentLanguageBuffer,
                PositionAffinity.Predecessor
            );

            if (langPoint != null) {
                return langPoint.Value;
            }

            return _currentLanguageBuffer.CurrentSnapshot.Length;
        }

        private void ResetCursor() {
            if (_executionTimer != null) {
                _executionTimer.Stop();
            }
            if (_oldCursor != null) {
                ((ContentControl)TextView).Cursor = _oldCursor;
            }
            /*if (_oldCaretBrush != null) {
                CurrentView.Caret.RegularBrush = _oldCaretBrush;
            }*/

            _oldCursor = null;
            //_oldCaretBrush = null;
            _executionTimer = null;
        }

        private void StartCursorTimer() {
            // Save the old value of the caret brush so it can be restored
            // after execution has finished
            //_oldCaretBrush = CurrentView.Caret.RegularBrush;

            // Set the caret's brush to transparent so it isn't shown blinking
            // while code is executing in the REPL
            //CurrentView.Caret.RegularBrush = Brushes.Transparent;

            var timer = new DispatcherTimer();
            timer.Tick += SetRunningCursor;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            _executionTimer = timer;
            timer.Start();
        }

        private void SetRunningCursor(object sender, EventArgs e) {
            var view = (ContentControl)TextView;

            // Save the old value of the cursor so it can be restored
            // after execution has finished
            _oldCursor = view.Cursor;

            // TODO: Design work to come up with the correct cursor to use
            // Set the repl's cursor to the "executing" cursor
            view.Cursor = Cursors.Wait;

            // Stop the timer so it doesn't fire again
            if (_executionTimer != null) {
                _executionTimer.Stop();
            }
        }

        #endregion

        #region Active Code and Standard Input

        /// <summary>
        /// Returns the full text of the current active input.
        /// </summary>
        private string GetActiveCode() {
            return _currentLanguageBuffer.CurrentSnapshot.GetText();
        }
        
        /// <summary>
        /// Sets the active code to the specified text w/o executing it.
        /// </summary>
        private void SetActiveCode(string text) {
            _currentLanguageBuffer.Replace(new Span(0, _currentLanguageBuffer.CurrentSnapshot.Length), text);
        }

        /// <summary>
        /// Appends given text to the last input span (standard input or active code input).
        /// </summary>
        private void AppendInput(string text) {
            Debug.Assert(CheckAccess());

            var inputSpan = _projectionSpans[_projectionSpans.Count - 1];
            Debug.Assert(inputSpan.Kind == ReplSpanKind.Language || inputSpan.Kind == ReplSpanKind.StandardInput);
            Debug.Assert(inputSpan.TrackingSpan.TrackingMode == SpanTrackingMode.Custom);
                
            var buffer = inputSpan.TrackingSpan.TextBuffer;
            var span = inputSpan.TrackingSpan.GetSpan(buffer.CurrentSnapshot);
            using (var edit = buffer.CreateEdit()) {
                edit.Insert(edit.Snapshot.Length, text);
                edit.Apply();
            }

            var replSpan = new ReplSpan(
                new CustomTrackingSpan(
                    buffer.CurrentSnapshot,
                    new Span(span.Start, span.Length + text.Length),
                    PointTrackingMode.Negative, 
                    PointTrackingMode.Positive
                ),
                inputSpan.Kind
            );

            ReplaceProjectionSpan(_projectionSpans.Count - 1, replSpan);

            Caret.EnsureVisible();
        }

        private void ClearInput() {
            Debug.Assert(_projectionSpans.Count > 0);

            // Finds the last primary prompt (standard input or code input).
            // Removes all spans following the primary prompt from the projection buffer.
            int i = _projectionSpans.Count - 1;
            while (i >= 0) {
                if (_projectionSpans[i].Kind == ReplSpanKind.Prompt || _projectionSpans[i].Kind == ReplSpanKind.StandardInputPrompt) {
                    Debug.Assert(i != _projectionSpans.Count - 1);
                    break;
                } 
                i--;
            }

            if (_projectionSpans[i].Kind != ReplSpanKind.StandardInputPrompt) {
                _currentLanguageBuffer.Delete(new Span(0, _currentLanguageBuffer.CurrentSnapshot.Length));
            } else {
                Debug.Assert(_stdInputStart != null);
                _stdInputBuffer.Delete(Span.FromBounds(_stdInputStart.Value, _stdInputBuffer.CurrentSnapshot.Length));
            }
        }
        
        private void PrepareForInput() {
            _buffer.Flush();
            _buffer.ResetColors();

            AddLanguageBuffer();
            
            _isRunning = false;
            ResetCursor();

            // we are prepared for processing any postponed submissions there might have been:
            ProcessPendingSubmissions();
        }

        private void ProcessPendingSubmissions() {
            Debug.Assert(_currentLanguageBuffer != null);

            if (_pendingSubmissions == null || _pendingSubmissions.Count == 0) {
                RestoreUncommittedInput();

                // move to the end (it migth have been in virtual space):
                Caret.MoveTo(GetLastLine().End);
                Caret.EnsureVisible();

                var ready = ReadyForInput;
                if (ready != null) {
                    ready();
                }

                return;
            }

            string submission = _pendingSubmissions.Dequeue();

            // queue new work item:
            Dispatcher.Invoke(new Action(() => {
                SetActiveCode(submission);
                Submit();
            }));
        }

        public void Submit() {
            RequiresLanguageBuffer();
            AppendLineNoPromptInjection(_currentLanguageBuffer);
            ApplyProtection(_currentLanguageBuffer, regions: null);
            ExecuteActiveCode();
        }

        private void StoreUncommittedInput() {
            if (_uncommittedInput == null) {
                string activeCode = GetActiveCode();
                if (!String.IsNullOrEmpty(activeCode)) {
                    _uncommittedInput = activeCode;
                }
            }
        }

        private void AppendUncommittedInput(string text) {
            if (String.IsNullOrEmpty(_uncommittedInput)) {
                _uncommittedInput = text;
            } else {
                _uncommittedInput += text;
            }
        }

        private void RestoreUncommittedInput() {
            if (_uncommittedInput != null) {
                SetActiveCode(_uncommittedInput);
                _uncommittedInput = null;
            }
        }

        public string ReadStandardInput() {
            // shouldn't be called on the UI thread because we'll hang
            Debug.Assert(!CheckAccess());

            bool wasRunning = _isRunning;
            _readingStdIn = true;
            UIThread(() => {
                _buffer.Flush();

                if (_isRunning) {
                    _isRunning = false;
                } else if (_projectionSpans.Count > 0 && _projectionSpans[_projectionSpans.Count - 1].Kind == ReplSpanKind.Language) {
                    // we need to remove our input prompt.
                    RemoveLastInputPrompt();
                }

                AddStandardInputSpan();

                Caret.EnsureVisible();
                ResetCursor();

                _isRunning = false;
                _uncommittedInput = null;
                _stdInputStart = _stdInputBuffer.CurrentSnapshot.Length;
            });

            var ready = ReadyForInput;
            if (ready != null) {
                ready();
            }

            _inputEvent.WaitOne();
            _stdInputStart = null;
            _readingStdIn = false;

            UIThread(() => {
                // if the user cleared the screen we cancelled the input, so we won't have our span here.
                // We can also have an interleaving output span, so we'll search back for the last input span.
                int i = IndexOfLastStandardInputSpan();
                if (i != -1) {
                    RemoveProtection(_stdInputBuffer, _stdInputProtection);

                    // replace previous span w/ a span that won't grow...
                    var newSpan = new ReplSpan(
                        new CustomTrackingSpan(
                            _stdInputBuffer.CurrentSnapshot,
                            _projectionSpans[i].TrackingSpan.GetSpan(_stdInputBuffer.CurrentSnapshot),
                            PointTrackingMode.Negative,
                            PointTrackingMode.Negative
                        ),
                        ReplSpanKind.StandardInput
                    );

                    ReplaceProjectionSpan(i, newSpan);
                    ApplyProtection(_stdInputBuffer, _stdInputProtection, allowAppend: true);

                    if (wasRunning) {
                        _isRunning = true;
                    } else {
                        PrepareForInput();
                    }
                }
            });

            // input has been cancelled:
            if (_inputValue != null) {
                _history.Add(_inputValue);
            }

            return _inputValue;
        }

        private int IndexOfLastStandardInputSpan() {
            for (int i = _projectionSpans.Count - 1; i >= 0; i--) {
                if (_projectionSpans[i].Kind == ReplSpanKind.StandardInput) {
                    return i;
                }
            }
            return -1;
        }

        private bool InStandardInputRegion(SnapshotPoint point) {
            if (_stdInputStart == null) {
                return false;
            }

            var stdInputPoint = TextView.BufferGraph.MapDownToBuffer(
                point,
                PointTrackingMode.Positive,
                _stdInputBuffer,
                PositionAffinity.Successor
            );

            return stdInputPoint != null && stdInputPoint.Value.Position >= _stdInputStart.Value;
        }

        private void CancelStandardInput() {
            AppendLineNoPromptInjection(_stdInputBuffer);
            _inputValue = null;
            _inputEvent.Set();
        }

        private void SubmitStandardInput() {
            AppendLineNoPromptInjection(_stdInputBuffer);
            _inputValue = _stdInputBuffer.CurrentSnapshot.GetText(_stdInputStart.Value, _stdInputBuffer.CurrentSnapshot.Length - _stdInputStart.Value);
            _inputEvent.Set();
        }

        #endregion

        #region Output

        /// <summary>
        /// See IReplWindow
        /// </summary>
        public void WriteLine(string text) {
            _buffer.Write(text + GetLineBreak());
        }

        public void WriteOutput(object output) {
            Write(output);
        }

        public void WriteError(object output) {
            Write(output, error: true);
        }

        private void Write(object text, bool error = false) {
            if (_showOutput && !TryShowObject(text)) {
                // buffer the text
                if (text != null) {
                    _buffer.Write(text.ToString(), error);
                }
            }
        }

        /// <summary>
        /// Appends text to the output buffer and updates projection buffer to include it.
        /// </summary>
        internal void AppendOutput(IEnumerable<ColoredSpan> colors, string text) {
            int oldBufferLength = _outputBuffer.CurrentSnapshot.Length;
            int oldLineCount = _outputBuffer.CurrentSnapshot.LineCount;

            RemoveProtection(_outputBuffer, _outputProtection);

            // append the text to output buffer and make sure it ends with a line break:
            int newOutputLength = text.Length;
            using (var edit = _outputBuffer.CreateEdit()) {
                if (_addedLineBreakOnLastOutput) {
                    // appending additional output, remove the line break we previously injected
                    var lineBreak = GetLineBreak();
                    var deleteSpan = new Span(_outputBuffer.CurrentSnapshot.Length - lineBreak.Length, lineBreak.Length);
                    Debug.Assert(_outputBuffer.CurrentSnapshot.GetText(deleteSpan) == lineBreak);
                    edit.Delete(deleteSpan);
                    oldBufferLength -= lineBreak.Length;
                }

                edit.Insert(oldBufferLength, text);
                if (!_readingStdIn && !EndsWithLineBreak(text)) {
                    var lineBreak = GetLineBreak();
                    edit.Insert(oldBufferLength, lineBreak);
                    newOutputLength += lineBreak.Length;
                    _addedLineBreakOnLastOutput = true;
                } else {
                    _addedLineBreakOnLastOutput = false;
                }
                
                edit.Apply();
            }

            ApplyProtection(_outputBuffer, _outputProtection);

            int newLineCount = _outputBuffer.CurrentSnapshot.LineCount;

            var span = new Span(oldBufferLength, newOutputLength);
            var trackingSpan = new CustomTrackingSpan(
                _outputBuffer.CurrentSnapshot,
                span,
                PointTrackingMode.Negative,
                PointTrackingMode.Negative
            );

            var outputSpan = new ReplSpan(trackingSpan, ReplSpanKind.Output);
            _outputColors.AddRange(colors.Select(cs => new ColoredSpan(
                new Span(cs.Span.Start + oldBufferLength, cs.Span.Length),
                cs.Color
            )));

            bool appended = false;

            if (!_isRunning) {
                // insert output span immediately before the last primary span

                int lastPrimaryPrompt, lastPrompt;
                IndexOfLastPrompt(out lastPrimaryPrompt, out lastPrompt);

                // If the last prompt is STDIN prompt insert output before it, otherwise before the primary prompt:
                int insertBeforePrompt = (lastPrompt != -1 && _projectionSpans[lastPrompt].Kind == ReplSpanKind.StandardInputPrompt) ? lastPrompt : lastPrimaryPrompt;

                if (insertBeforePrompt >= 0) {
                    if (oldLineCount != newLineCount) {
                        int delta = newLineCount - oldLineCount;
                        Debug.Assert(delta > 0);

                        // update line -> projection span index mapping for the last primary prompt
                        var lastMaplet = _promptLineMapping.Last();
                        _promptLineMapping[_promptLineMapping.Count - 1] = new KeyValuePair<int, int>(
                            lastMaplet.Key + delta,
                            lastMaplet.Value + 1
                        );
                    }

                    // Projection buffer change might trigger events that access prompt line mapping, so do it last:
                    InsertProjectionSpan(insertBeforePrompt, outputSpan);
                    appended = true;
                }
            }

            if (!appended) {
                AppendProjectionSpan(outputSpan);
            }
        }

        /// <summary>
        /// Gets a line of the projection buffer for given language span. 
        /// </summary>
        private ITextSnapshotLine GetSpanProjectionLine(ReplSpan span) {
            Debug.Assert(!span.Kind.IsPrompt());

            var pt = TextView.BufferGraph.MapUpToBuffer(
                span.TrackingSpan.GetStartPoint(span.TrackingSpan.TextBuffer.CurrentSnapshot),
                PointTrackingMode.Positive,
                PositionAffinity.Successor,
                _projectionBuffer
            );
            Debug.Assert(pt != null);

            return pt.Value.GetContainingLine();
        }

        private bool TryShowObject(object obj) {
            UIElement element = obj as UIElement;
            if (element == null) {
                return false;
            }

            UIThread(() => {
                _buffer.Flush();

                // figure out where we're inserting the image
                SnapshotPoint targetPoint = new SnapshotPoint(
                    TextView.TextBuffer.CurrentSnapshot,
                    TextView.TextBuffer.CurrentSnapshot.Length
                );

                for (int i = _projectionSpans.Count - 1; i >= 0; i--) {
                    if (_projectionSpans[i].Kind == ReplSpanKind.Output ||
                        (_projectionSpans[i].Kind == ReplSpanKind.Language && _isRunning)) {
                        // we've had some output during the execution and we hit that buffer.
                        // OR we hit a language input buffer, and we're running, and no output
                        // has been produced yet.

                        // In either case, this is where the image goes.
                        break;
                    }

                    // adjust where we're going to insert based upon the length of the span
                    targetPoint -= _projectionSpans[i].Length;

                    if (_projectionSpans[i].Kind == ReplSpanKind.Prompt) {
                        // we just walked past the primary input prompt, we want to put the
                        // image right before it.
                        break;
                    }
                }

                InlineReplAdornmentProvider.AddInlineAdornment(TextView, element, OnAdornmentLoaded, targetPoint);
                OnInlineAdornmentAdded();
                WriteLine(String.Empty);
                WriteLine(String.Empty);
            });
            return true;
        }

        private void OnAdornmentLoaded(object source, EventArgs e) {
            ((ZoomableInlineAdornment)source).Loaded -= OnAdornmentLoaded;
            // Make sure the caret line is rendered
            DoEvents();
            Caret.EnsureVisible();
        }
        
        private void OnInlineAdornmentAdded() {
            _adornmentToMinimize = true;
        }

        #endregion

        #region Execution

        private bool CanExecuteActiveCode() {
            Debug.Assert(_currentLanguageBuffer != null);

            var input = GetActiveCode();
            if (input.Trim().Length == 0) {
                // Always allow "execution" of a blank line.
                // This will just close the current prompt and start a new one
                return true;
            }

            // Ignore any whitespace past the insertion point when determining
            // whether or not we're at the end of the input
            var pt = GetActiveCodeInsertionPosition();
            var atEnd = (pt == input.Length) || (pt >= 0 && input.Substring(pt).Trim().Length == 0);
            if (!atEnd) {
                return false;
            }

            // A command is never multi-line, so always try to execute something which looks like a command
            if (input.StartsWith(_commandPrefix)) {
                return true;
            }

            return Evaluator.CanExecuteText(input);
        }

        /// <summary>
        /// Execute and then call the callback function with the result text.
        /// </summary>
        /// <param name="processResult"></param>
        private void ExecuteActiveCode() {
            UIThread(() => {
                // Ensure that the REPL doesn't try to execute if it is already
                // executing.  If this invariant can no longer be maintained more of
                // the code in this method will need to be bullet-proofed
                if (_isRunning) {
                    return;
                }

                var text = GetActiveCode();

                if (_adornmentToMinimize) {
                    InlineReplAdornmentProvider.MinimizeLastInlineAdornment(TextView);
                    _adornmentToMinimize = false;
                }
                
                TextView.Selection.Clear();

                _history.UncommittedInput = null;
                if (text.Trim().Length == 0) {
                    PrepareForInput();
                } else {
                    _history.Add(text.TrimEnd(_whitespaceChars));

                    _isRunning = true;

                    // Following method assumes that _isRunning will be cleared before 
                    // the following method is called again.
                    StartCursorTimer();

                    _sw = Stopwatch.StartNew();

                    Task<ExecutionResult> task = ExecuteCommand(text, updateHistory: true) ?? Evaluator.ExecuteText(text) ?? ExecutionResult.Failed;
                    
                    task.ContinueWith(FinishExecute, _uiScheduler);
                }
            });
        }

        private void FinishExecute(Task<ExecutionResult> result) {
            Debug.Assert(CheckAccess());

            _sw.Stop();
            _buffer.Flush();
            
            if (_history.Last != null) {
                _history.Last.Duration = _sw.Elapsed.Seconds;
            }

            if (result.IsCanceled || result.Exception != null || !result.Result.IsSuccessful) {
                if (_history.Last != null) {
                    _history.Last.Failed = true;
                }

                if (_pendingSubmissions != null && _pendingSubmissions.Count > 0) {
                    // there was an error with the last execution, clear the
                    // input queue due to the error.
                    _pendingSubmissions.Clear();
                }
            }
            _addedLineBreakOnLastOutput = false;
            PrepareForInput();
        }

        private void SmartExecute() {
            Debug.Assert(CheckAccess());

            if (CaretInActiveCodeRegion) {
                Submit();
                return;
            } 
            
            if (!CaretInStandardInputRegion) {
                var code = GetCodeSnapshot(Caret.Position.BufferPosition);
                if (code != null) {
                    _editorOperations.MoveToEndOfDocument(false);
                    SetActiveCode(TrimTrailingEmptyLines(code));
                }
            }
        }

        // Returns null if the text isn't recognized as a command
        public Task<ExecutionResult> ExecuteCommand(string text) {
            return ExecuteCommand(text, updateHistory: false);
        }

        private Task<ExecutionResult> ExecuteCommand(string text, bool updateHistory) {
            if (!text.StartsWith(_commandPrefix)) {
                return null;
            }

            string commandLine = text.Substring(_commandPrefix.Length).Trim();
            string command = commandLine.Split(' ')[0];
            string args = commandLine.Substring(command.Length).Trim();

            // TODO: no special casing, these should all be commands

            if (command == _commandPrefix) {
                // REPL-level comment; do nothing
                return ExecutionResult.Succeeded;
            }

            if (commandLine.Length == 0 || command == "help") {
                ShowReplHelp();
                return ExecutionResult.Succeeded;
            }

            IReplCommand commandHandler = _commands.Find(x => x.Command == command);
            if (commandHandler == null) {
                commandHandler = _commands.OfType<IReplCommand2>().FirstOrDefault(x => x.Aliases.Contains(command));
                if (commandHandler == null) {
                    return null;
                }
            }

            if (updateHistory) {
                _history.Last.Command = true;
            }

            try {
                return commandHandler.Execute(this, args) ?? ExecutionResult.Failed;
            } catch (Exception e) {
                WriteError(String.Format("Command '{0}' failed: {1}", command, e.Message));
                return ExecutionResult.Failed;
            }
        }

        private void ShowReplHelp() {
            var cmdnames = new List<IReplCommand>(_commands.Where(x => x.Command != null));
            cmdnames.Sort((x, y) => String.Compare(x.Command, y.Command));

            const string helpFmt = "  {0,-24}  {1}";
            WriteLine(string.Format(helpFmt, _commandPrefix + "help", "Show a list of REPL commands"));

            foreach (var cmd in cmdnames) {
                WriteLine(string.Format(helpFmt, GetCommandNameWithAliases(cmd), cmd.Description));
            }
        }

        private string GetCommandNameWithAliases(IReplCommand cmd) {
            var cmd2 = cmd as IReplCommand2;
            if (cmd2 != null && cmd2.Aliases != null) {
                string aliases = string.Join(",", cmd2.Aliases.Select(x => _commandPrefix + x));
                if (aliases.Length > 0) {
                    return string.Join(",", _commandPrefix + cmd.Command, aliases);
                }
            }

            return _commandPrefix + cmd.Command;
        }

        #endregion
        
        public PropertyCollection Properties {
            get { return _properties; }
        }


        #region Scopes

        private void InitializeScopeList() {
            IMultipleScopeEvaluator multiScopeEval = _evaluator as IMultipleScopeEvaluator;
            if (multiScopeEval != null) {
                _scopeListVisible = IsMultiScopeEnabled();
                multiScopeEval.AvailableScopesChanged += UpdateScopeList;
                multiScopeEval.MultipleScopeSupportChanged += MultipleScopeSupportChanged;
            }
        }

        internal void SetCurrentScope(string newItem) {
            string activeCode = GetActiveCode();
            ((IMultipleScopeEvaluator)_evaluator).SetScope(newItem);
            SetActiveCode(activeCode);
        }

        private void UpdateScopeList(object sender, EventArgs e) {
            if (!CheckAccess()) {
                Dispatcher.BeginInvoke(new Action(() => UpdateScopeList(sender, e)));
                return;
            }

            _currentScopes = ((IMultipleScopeEvaluator)_evaluator).GetAvailableScopes().ToArray();
        }

        private bool IsMultiScopeEnabled() {
            var multiScope = Evaluator as IMultipleScopeEvaluator;
            return multiScope != null && multiScope.EnableMultipleScopes;
        }

        private void MultipleScopeSupportChanged(object sender, EventArgs e) {
            _scopeListVisible = IsMultiScopeEnabled();
        }

        /// <summary>
        /// Handles getting or setting the current value of the combo box.
        /// </summary>
        private void ScopeComboBoxHandler(IntPtr newValue, IntPtr outCurrentValue) {
            // getting the current value
            if (outCurrentValue != IntPtr.Zero) {
                Marshal.GetNativeVariantForObject(((IMultipleScopeEvaluator)Evaluator).CurrentScopeName, outCurrentValue);
            }

            // setting the current value
            if (newValue != IntPtr.Zero) {
                SetCurrentScope((string)Marshal.GetObjectForNativeVariant(newValue));
            }
        }

        /// <summary>
        /// Gets the list of scopes that should be available in the combo box.
        /// </summary>
        private void ScopeComboBoxGetList(IntPtr outList) {
            Debug.Assert(outList != IntPtr.Zero);
            Marshal.GetNativeVariantForObject(_currentScopes, outList);
        }

        #endregion

        #region Buffers, Spans and Prompts

        private ReplSpan CreateStandardInputPrompt() {
            return CreatePrompt(_stdInputPrompt, ReplSpanKind.StandardInputPrompt);
        }

        private ReplSpan CreatePrimaryPrompt() {
            var result = CreatePrompt(_prompt, ReplSpanKind.Prompt);
            _currentInputId++;
            return result;
        }

        private ReplSpan CreatePrompt(string prompt, ReplSpanKind promptKind) {
            Debug.Assert(promptKind == ReplSpanKind.Prompt || promptKind == ReplSpanKind.StandardInputPrompt);

            var lastLine = GetLastLine();
            _promptLineMapping.Add(new KeyValuePair<int, int>(lastLine.LineNumber, _projectionSpans.Count));

            prompt = _displayPromptInMargin ? String.Empty : FormatPrompt(prompt, _currentInputId);
            return new ReplSpan(prompt, promptKind);
        }

        private void RemoveLastInputPrompt() {
            var prompt = _projectionSpans[_projectionSpans.Count - SpansPerLineOfInput];
            Debug.Assert(prompt.Kind.IsPrompt());
            if (prompt.Kind == ReplSpanKind.Prompt || prompt.Kind == ReplSpanKind.StandardInputPrompt) {
                _promptLineMapping.RemoveAt(_promptLineMapping.Count - 1);
            }

            // projection buffer update must be the last operation as it might trigger event that accesses prompt line mapping:
            RemoveProjectionSpans(_projectionSpans.Count - SpansPerLineOfInput, SpansPerLineOfInput);
        }

        private ReplSpan CreateSecondaryPrompt() {
            string secondPrompt = _displayPromptInMargin ? String.Empty : FormatPrompt(_secondPrompt, _currentInputId - 1);
            return new ReplSpan(secondPrompt, ReplSpanKind.SecondaryPrompt);
        }
        
        private void UpdatePrompts(ReplSpanKind promptKind, string oldPrompt, string newPrompt, bool currentOnly = false) {
            if ((oldPrompt != newPrompt || oldPrompt == null) && _projectionSpans.Count > 0)  {
                // TODO (tomat): build the entire replacement upfront and perform a single projection edit

                // find and replace all the prompts
                int curInput = 1;
                for (int i = _projectionSpans.Count - 1; i >= 0 ; i--) {
                    if (_projectionSpans[i].Kind == promptKind) {
                        string prompt = FormatPrompt(newPrompt, promptKind == ReplSpanKind.Prompt ? curInput : curInput - 1);
                        ReplaceProjectionSpan(i, new ReplSpan(prompt, promptKind));
                    }

                    if (_projectionSpans[i].Kind == ReplSpanKind.Prompt) {
                        curInput++;
                        if (currentOnly) {
                            break;
                        }
                    }
                }
            }
        }

        private string FormatPrompt(string prompt, int currentInput) {
            if (!_formattedPrompts) {
                return prompt;
            }

            StringBuilder res = null;
            for (int i = 0; i < prompt.Length; i++) {
                if (prompt[i] == '\\' && i < prompt.Length - 1) {
                    if (res == null) {
                        res = new StringBuilder(prompt, 0, i, prompt.Length);
                    }
                    switch (prompt[++i]) {
                        case '\\': res.Append('\\'); break;
                        case '#': res.Append(currentInput.ToString()); break;
                        case 'D': res.Append(DateTime.Today.ToString()); break;
                        case 'T': res.Append(DateTime.Now.ToString()); break;
                        default:
                            res.Append('\\');
                            res.Append(prompt[i + 1]);
                            break;
                    }
                    
                } else if (res != null) {
                    res.Append(prompt[i]);
                }
            }

            if (res != null) {
                return res.ToString();
            }
            return prompt;
        }

        internal string/*!*/ Prompt {
            get { return _prompt; }
        }

        internal string/*!*/ SecondaryPrompt {
            get { return _secondPrompt; }
        }

        internal string/*!*/ InputPrompt {
            get { return _stdInputPrompt; }
        }

        internal Control/*!*/ HostControl {
            get { return _textViewHost.HostControl; }
        }

        /// <summary>
        /// Enumerates input prompts that overlap given span. 
        /// Returns an empty set if we are in the middle of operation changing the mapping and/or projection buffer.
        /// </summary>
        internal IEnumerable<KeyValuePair<ReplSpanKind, SnapshotPoint>> GetOverlappingPrompts(SnapshotSpan span) {
            if (_promptLineMapping == null || _promptLineMapping.Count == 0 || _projectionSpans.Count == 0) {
                yield break;
            }

            var currentSnapshotSpan = span.TranslateTo(CurrentSnapshot, SpanTrackingMode.EdgeInclusive);
            var startLine = currentSnapshotSpan.Start.GetContainingLine();
            var endLine = currentSnapshotSpan.End.GetContainingLine();

            var promptMappingIndex = GetPromptMappingIndex(startLine.LineNumber);
            
            do {
                int lineNumber = _promptLineMapping[promptMappingIndex].Key;
                int promptIndex = _promptLineMapping[promptMappingIndex].Value;

                // no overlapping prompts will be found beyond the last line of the span:
                if (lineNumber > endLine.LineNumber) {
                    break;
                }

                // enumerate all prompts of the input block (primary and secondary):
                do {
                    var line = CurrentSnapshot.GetLineFromLineNumber(lineNumber);
                    ReplSpan projectionSpan = _projectionSpans[promptIndex];
                    Debug.Assert(projectionSpan.Kind.IsPrompt());

                    if (line.Start.Position >= currentSnapshotSpan.Span.Start || line.Start.Position < currentSnapshotSpan.Span.End) {
                        yield return new KeyValuePair<ReplSpanKind, SnapshotPoint>(
                            projectionSpan.Kind, 
                            new SnapshotPoint(CurrentSnapshot, line.Start)
                        );
                    }

                    promptIndex += SpansPerLineOfInput;
                    lineNumber++;
                } while (promptIndex < _projectionSpans.Count && _projectionSpans[promptIndex].Kind == ReplSpanKind.SecondaryPrompt);

                // next input block:
                promptMappingIndex++;
            } while (promptMappingIndex < _promptLineMapping.Count);
        }

        /// <summary>
        /// Binary search for a prompt located on given line number. If there is no such span returns the closest preceding span.
        /// </summary>
        internal int GetPromptMappingIndex(int lineNumber) {
            int start = 0;
            int end = _promptLineMapping.Count - 1;
            while (true) {
                Debug.Assert(start <= end);

                int mid = start + ((end - start) >> 1);
                int key = _promptLineMapping[mid].Key;

                if (lineNumber == key) {
                    return mid;
                }

                if (mid == start) {
                    Debug.Assert(start == end || start == end - 1);
                    return (lineNumber >= _promptLineMapping[end].Key) ? end : mid;
                }

                if (lineNumber > key) {
                    start = mid;
                } else {
                    end = mid;
                }
            }
        }

        private void IndexOfLastPrompt(out int lastPrimary, out int last) {
            last = -1;
            lastPrimary = -1;
            for (int i = _projectionSpans.Count - 1; i >= 0; i--) {
                switch (_projectionSpans[i].Kind) {
                    case ReplSpanKind.Prompt:
                        lastPrimary = i;
                        if (last == -1) {
                            last = i;
                        }
                        return;

                    case ReplSpanKind.SecondaryPrompt:
                    case ReplSpanKind.StandardInputPrompt:
                        if (last == -1) {
                            last = i;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Creates and adds a new language buffer to the projection buffer.
        /// </summary>
        private void AddLanguageBuffer() {
            var buffer = _bufferFactory.CreateTextBuffer(_languageContentType);
            buffer.Properties.AddProperty(typeof(IReplEvaluator), _evaluator);

            // get the real classifier, and have our classifier start listening and forwarding events            
            var contentClassifier = _classifierAgg.GetClassifier(buffer);
            _primaryClassifier.AddClassifier(_projectionBuffer, buffer, contentClassifier);

            var previousBuffer = _currentLanguageBuffer;
            _currentLanguageBuffer = buffer;

            _evaluator.ActiveLanguageBufferChanged(buffer, previousBuffer);

            // add the whole buffer to the projection buffer and set it up to expand to the right as text is appended
            ReplSpan promptSpan = CreatePrimaryPrompt();
            ReplSpan languageSpan = new ReplSpan(CreateLanguageTrackingSpan(new Span(0, 0)), ReplSpanKind.Language);

            // projection buffer update must be the last operation as it might trigger event that accesses prompt line mapping:
            AppendProjectionSpans(promptSpan, languageSpan);
        }

        /// <summary>
        /// Creates the language span for the last line of the active input.  This span
        /// is effectively edge inclusive so it will grow as the user types at the end.
        /// </summary>
        private ITrackingSpan CreateLanguageTrackingSpan(Span span) {
            return new CustomTrackingSpan(
                _currentLanguageBuffer.CurrentSnapshot,
                span, 
                PointTrackingMode.Negative, 
                PointTrackingMode.Positive);
        }

        /// <summary>
        /// Creates the tracking span for a line previous in the input.  This span
        /// is negative tracking on the end so when the user types at the beginning of
        /// the next line we don't grow with the change.
        /// </summary>
        /// <param name="span"></param>
        /// <returns></returns>
        private ITrackingSpan CreateOldLanguageTrackingSpan(Span span) {
            return new CustomTrackingSpan(
                _currentLanguageBuffer.CurrentSnapshot,
                span,
                PointTrackingMode.Negative,
                PointTrackingMode.Negative
            );
        }

        /// <summary>
        /// Add a zero-width tracking span at the end of the projection buffer mapping to the end of the standard input buffer.
        /// </summary>
        private void AddStandardInputSpan() {
            ReplSpan promptSpan = CreateStandardInputPrompt();

            var stdInputSpan = new CustomTrackingSpan(
                _stdInputBuffer.CurrentSnapshot,
                new Span(_stdInputBuffer.CurrentSnapshot.Length, 0),
                PointTrackingMode.Negative, 
                PointTrackingMode.Positive
            );

            ReplSpan inputSpan = new ReplSpan(stdInputSpan, ReplSpanKind.StandardInput);

            AppendProjectionSpans(promptSpan, inputSpan);
        }

        /// <summary>
        /// Marks the entire buffer as readonly.
        /// </summary>
        private static void ApplyProtection(ITextBuffer buffer, IReadOnlyRegion[] regions, bool allowAppend = false) {
            using (var readonlyEdit = buffer.CreateReadOnlyRegionEdit()) {
                int end = buffer.CurrentSnapshot.Length;
                Span span = new Span(0, end);

                var region0 = allowAppend ?
                    readonlyEdit.CreateReadOnlyRegion(span, SpanTrackingMode.EdgeExclusive, EdgeInsertionMode.Allow) :
                    readonlyEdit.CreateReadOnlyRegion(span, SpanTrackingMode.EdgeExclusive, EdgeInsertionMode.Deny);

                // Create a second read-only region to prevent insert at start of buffer.
                var region1 = (end > 0) ? readonlyEdit.CreateReadOnlyRegion(new Span(0, 0), SpanTrackingMode.EdgeExclusive, EdgeInsertionMode.Deny) : null;

                readonlyEdit.Apply();

                if (regions != null) {
                    regions[0] = region0;
                    regions[1] = region1;
                }
            }
        }

        /// <summary>
        /// Removes readonly region from buffer.
        /// </summary>
        private static void RemoveProtection(ITextBuffer buffer, IReadOnlyRegion[] regions) {
            if (regions[0] != null) {
                Debug.Assert(regions[1] != null);

                foreach (var region in regions) {
                    using (var readonlyEdit = buffer.CreateReadOnlyRegionEdit()) {
                        readonlyEdit.RemoveReadOnlyRegion(region);
                        readonlyEdit.Apply();
                    }
                }
            }
        }

        private const int SpansPerLineOfInput = 2;
        private static object SuppressPromptInjectionTag = new object();

        private struct SpanRangeEdit {
            public int Start;
            public int Count;
            public ReplSpan[] Replacement;

            public SpanRangeEdit(int start, int count, ReplSpan[] replacement) {
                Start = start;
                Count = count;
                Replacement = replacement;
            }
        }

        private void ProjectionBufferChanged(object sender, TextContentChangedEventArgs e) {
            // this is an edit performed in this event:
            if (e.EditTag == SuppressPromptInjectionTag) {
                return;
            }

            // projection buffer is changed before language buffer is created (for example, output might be printed out during initialization):
            if (_currentLanguageBuffer == null) {
                return;
            }

            int oldProjectionSpansCount = _projectionSpans.Count;
            List<SpanRangeEdit> spanEdits = new List<SpanRangeEdit>();

            // a span in the new snapshot that includes all line changes:
            int newMinPosition = Int32.MaxValue;
            int newMaxPosition = -1;

            // changes are sorted by position
            foreach (var change in e.Changes) {
                int projectionOldStartLineNumber = e.Before.GetLineFromPosition(change.OldPosition).LineNumber;
                int projectionOldEndLineNumber = e.Before.GetLineFromPosition(change.OldEnd).LineNumber;

                if (change.LineCountDelta == 0 && projectionOldStartLineNumber == projectionOldEndLineNumber) {
                    continue;
                }

                if (change.NewPosition < newMinPosition) {
                    newMinPosition = change.NewPosition;
                }
                if (change.NewEnd > newMaxPosition) {
                    newMaxPosition = change.NewEnd;
                }

                var languageNewSpans = TextView.BufferGraph.MapDownToBuffer(
                    new SnapshotSpan(e.After, change.NewSpan), SpanTrackingMode.EdgeInclusive, _currentLanguageBuffer
                );

                Debug.Assert(languageNewSpans.Count <= 1);
                if (languageNewSpans.Count == 0) {
                    continue;
                }

                // calculate the projection span range to remove:
                int startSpanIndex = oldProjectionSpansCount - (e.Before.LineCount - projectionOldStartLineNumber) * SpansPerLineOfInput + 1;
                int endSpanIndex = oldProjectionSpansCount - (e.Before.LineCount - projectionOldEndLineNumber) * SpansPerLineOfInput + 1;
                Debug.Assert(_projectionSpans[startSpanIndex].Kind == ReplSpanKind.Language);
                Debug.Assert(_projectionSpans[endSpanIndex].Kind == ReplSpanKind.Language);

                int spansToReplace = endSpanIndex - startSpanIndex + 1;
                Debug.Assert(spansToReplace >= 1);

                var languageStartLine = languageNewSpans[0].Start.GetContainingLine();
                var languageEndLine = languageNewSpans[0].End.GetContainingLine();

                int i = 0;
                var newSpans = new ReplSpan[
                    (languageEndLine.LineNumber - languageStartLine.LineNumber) * SpansPerLineOfInput  // number of line breaks * 2
                    + 1
                ];

                var languageLine = languageStartLine;
                while (true) {
                    if (languageLine.LineNumber != languageStartLine.LineNumber) {
                        newSpans[i++] = CreateSecondaryPrompt();
                    }

                    newSpans[i++] = CreateLanguageSpanForLine(languageLine);
                    if (languageLine.LineNumber == languageEndLine.LineNumber) {
                        break;
                    }
                    languageLine = languageLine.Snapshot.GetLineFromLineNumber(languageLine.LineNumber + 1);
                }
                Debug.Assert(i == newSpans.Length);
                spanEdits.Add(new SpanRangeEdit(startSpanIndex, spansToReplace, newSpans));
            }

            if (spanEdits.Count > 0) {
                ReplaceProjectionSpans(spanEdits);
            }
        }

        private void ReplaceProjectionSpans(List<SpanRangeEdit> spanEdits) {
            Debug.Assert(spanEdits.Count > 0);

            int start = spanEdits.First().Start;
            int end = spanEdits.Last().Start + spanEdits.Last().Count;

            var replacement = new List<ReplSpan>();
            replacement.AddRange(spanEdits[0].Replacement);
            int lastEnd = start + spanEdits[0].Count;

            for (int i = 1; i < spanEdits.Count; i++) {
                SpanRangeEdit edit = spanEdits[i];

                int gap = edit.Start - lastEnd;

                // there is always at least prompt span in between subsequent edits
                Debug.Assert(gap != 0);
                // spans can't share more then one span
                Debug.Assert(gap >= -1);

                if (gap == -1) {
                    replacement.AddRange(edit.Replacement.Skip(1));
                } else {
                    replacement.AddRange(_projectionSpans.Skip(lastEnd).Take(gap));
                    replacement.AddRange(edit.Replacement);
                }
                lastEnd = edit.Start + edit.Count;
            }
            ReplaceProjectionSpans(start, end - start, replacement, SuppressPromptInjectionTag);
        }

        private ReplSpan CreateLanguageSpanForLine(ITextSnapshotLine languageLine) {
            ITrackingSpan languageSpan;
            if (languageLine.LineNumber == languageLine.Snapshot.LineCount - 1) {
                languageSpan = CreateLanguageTrackingSpan(languageLine.ExtentIncludingLineBreak);
            } else {
                languageSpan = CreateOldLanguageTrackingSpan(languageLine.ExtentIncludingLineBreak);
            }
            return new ReplSpan(languageSpan, ReplSpanKind.Language);
        }

        private void AppendLineNoPromptInjection(ITextBuffer buffer) {
            using (var edit = buffer.CreateEdit(EditOptions.None, null, SuppressPromptInjectionTag)) {
                edit.Insert(buffer.CurrentSnapshot.Length, GetLineBreak());
                edit.Apply();
            }
        }
        
        // 
        // WARNING: When updating projection spans we need to update _projectionSpans list first and 
        // then projection buffer, since the projection buffer update might trigger events that might 
        // access the projection spans.
        //

        private void ClearProjection() {
            int count = _projectionSpans.Count;
            _projectionSpans.Clear();
            _projectionBuffer.DeleteSpans(0, count);
            InitializeProjectionBuffer();
        }

        private void InitializeProjectionBuffer() {
            // we need at least one non-inert span due to bugs in projection buffer, so insert an empty output span:
            var trackingSpan = new CustomTrackingSpan(
                _outputBuffer.CurrentSnapshot,
                new Span(0, 0),
                PointTrackingMode.Negative,
                PointTrackingMode.Negative
            );

            AppendProjectionSpan(new ReplSpan(trackingSpan, ReplSpanKind.Output));
        }

        private void AppendProjectionSpan(ReplSpan span) {
            InsertProjectionSpan(_projectionSpans.Count, span);
        }

        private void AppendProjectionSpans(ReplSpan span1, ReplSpan span2) {
            InsertProjectionSpans(_projectionSpans.Count, span1, span2);
        }

        private void InsertProjectionSpan(int index, ReplSpan span) {
            _projectionSpans.Insert(index, span);
            _projectionBuffer.ReplaceSpans(index, 0, new[] { span.Span }, EditOptions.None, null);
        }

        private void InsertProjectionSpans(int index, ReplSpan span1, ReplSpan span2) {
            _projectionSpans.Insert(index, span1);
            _projectionSpans.Insert(index + 1, span2);
            _projectionBuffer.ReplaceSpans(index, 0, new[] { span1.Span, span2.Span }, EditOptions.None, null);
        }

        private void ReplaceProjectionSpan(int spanToReplace, ReplSpan newSpan) {
            _projectionSpans[spanToReplace] = newSpan;
            _projectionBuffer.ReplaceSpans(spanToReplace, 1, new[] { newSpan.Span }, EditOptions.None, null);
        }

        private void ReplaceProjectionSpans(int position, int count, IList<ReplSpan> newSpans, object editTag) {
            _projectionSpans.RemoveRange(position, count);
            _projectionSpans.InsertRange(position, newSpans);

            object[] trackingSpans = new object[newSpans.Count];
            for (int i = 0; i < trackingSpans.Length; i++) {
                trackingSpans[i] = newSpans[i].Span;
            }
            _projectionBuffer.ReplaceSpans(position, count, trackingSpans, EditOptions.None, editTag);
        }

        private void RemoveProjectionSpans(int index, int count) {
            _projectionSpans.RemoveRange(index, count);
            _projectionBuffer.DeleteSpans(index, count);
        }

        #endregion

        #region Editor Helpers

        private ITextSnapshotLine GetLastLine() {
            return GetLastLine(CurrentSnapshot);
        }

        private static ITextSnapshotLine GetLastLine(ITextSnapshot snapshot) {
            return snapshot.GetLineFromLineNumber(snapshot.LineCount - 1);
        }
        private static ITextSnapshotLine GetPreviousLine(ITextSnapshotLine line) {
            return line.LineNumber > 0 ? line.Snapshot.GetLineFromLineNumber(line.LineNumber - 1) : null;
        }

        private string GetLineBreak() {
            return _textViewHost.TextView.Options.GetNewLineCharacter();
        }

        private static bool EndsWithLineBreak(string str) {
            return str.Length > 0 && (str[str.Length - 1] == '\n' || str[str.Length - 1] == '\r');
        }

        private static int IndexOfNonWhiteSpaceCharacter(ITextSnapshotLine line) {
            var snapshot = line.Snapshot;
            int start = line.Start.Position;
            int count = line.Length;
            for (int i = 0; i < count; i++) {
                if (!Char.IsWhiteSpace(snapshot[start + i])) {
                    return i;
                }
            }
            return -1;
        }

        private static string TrimTrailingEmptyLines(ITextSnapshot snapshot) {
            var line = GetLastLine(snapshot);
            while (line != null && line.Length == 0) {
                line = GetPreviousLine(line);
            }

            if (line == null) {
                return String.Empty;
            }

            return line.Snapshot.GetText(0, line.ExtentIncludingLineBreak.End.Position);
        }

        private sealed class EditResolver : IProjectionEditResolver {
            private readonly ReplWindow _replWindow;

            public EditResolver(ReplWindow replWindow) {
                _replWindow = replWindow;
            }

            // We always favor the last buffer of our language type.  This handles cases where we're on a boundary between a prompt and a language 
            // buffer - we favor the language buffer because the prompts cannot be edited.  In the case of two language buffers this also works because
            // our spans are laid out like:
            // <lang span 1 including newline>
            // <prompt span><lang span 2>
            // 
            // In the case where the prompts are in the margin we have an insertion conflict between the two language spans.  But because
            // lang span 1 includes the new line in order to be oun the boundary we need to be on lang span 2's line.
            // 
            // This works the same way w/ our input buffer where the input buffer present instead of <lang span 2>.

            public void FillInInsertionSizes(SnapshotPoint projectionInsertionPoint, ReadOnlyCollection<SnapshotPoint> sourceInsertionPoints, string insertionText, IList<int> insertionSizes) {
                int index = IndexOfEditableBuffer(sourceInsertionPoints);
                if (index != -1) {
                    insertionSizes[index] = insertionText.Length;
                }
            }

            public int GetTypicalInsertionPosition(SnapshotPoint projectionInsertionPoint, ReadOnlyCollection<SnapshotPoint> sourceInsertionPoints) {
                int index = IndexOfEditableBuffer(sourceInsertionPoints);
                return index != -1 ? index : 0;
            }

            public void FillInReplacementSizes(SnapshotSpan projectionReplacementSpan, ReadOnlyCollection<SnapshotSpan> sourceReplacementSpans, string insertionText, IList<int> insertionSizes) {
                int index = IndexOfEditableBuffer(sourceReplacementSpans);
                if (index != -1) {
                    insertionSizes[index] = insertionText.Length;
                }
            }

            private int IndexOfEditableBuffer(ReadOnlyCollection<SnapshotPoint> sourceInsertionPoints) {
                for (int i = sourceInsertionPoints.Count - 1; i >= 0; i--) {
                    var insertionBuffer = sourceInsertionPoints[i].Snapshot.TextBuffer;
                    if (insertionBuffer.ContentType == _replWindow._languageContentType || insertionBuffer == _replWindow._stdInputBuffer) {
                        return i;
                    }
                }

                return -1;
            }

            private int IndexOfEditableBuffer(ReadOnlyCollection<SnapshotSpan> sourceInsertionPoints) {
                for (int i = sourceInsertionPoints.Count - 1; i >= 0; i--) {
                    var insertionBuffer = sourceInsertionPoints[i].Snapshot.TextBuffer;
                    if (insertionBuffer.ContentType == _replWindow._languageContentType || insertionBuffer == _replWindow._stdInputBuffer) {
                        return i;
                    }
                }

                return -1;
            }
        }

        #endregion

        #region IVsFindTarget Members

        public int Find(string pszSearch, uint grfOptions, int fResetStartPoint, IVsFindHelper pHelper, out uint pResult) {
            if (_findTarget != null) {
                return _findTarget.Find(pszSearch, grfOptions, fResetStartPoint, pHelper, out pResult);
            }
            pResult = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int GetCapabilities(bool[] pfImage, uint[] pgrfOptions) {
            if (_findTarget != null && pgrfOptions != null && pgrfOptions.Length > 0 && _projectionSpans.Count > 0) {
                return _findTarget.GetCapabilities(pfImage, pgrfOptions);
            }
            return VSConstants.E_NOTIMPL;
        }

        public int GetCurrentSpan(TextSpan[] pts) {
            if (_findTarget != null) {
                return _findTarget.GetCurrentSpan(pts);
            }
            return VSConstants.E_NOTIMPL;
        }

        public int GetFindState(out object ppunk) {
            if (_findTarget != null) {
                return _findTarget.GetFindState(out ppunk);
            }
            ppunk = null;
            return VSConstants.E_NOTIMPL;

        }

        public int GetMatchRect(RECT[] prc) {
            if (_findTarget != null) {
                return _findTarget.GetMatchRect(prc);
            }
            return VSConstants.E_NOTIMPL;
        }

        public int GetProperty(uint propid, out object pvar) {
            if (_findTarget != null) {
                return _findTarget.GetProperty(propid, out pvar);
            }
            pvar = null;
            return VSConstants.E_NOTIMPL;
        }

        public int GetSearchImage(uint grfOptions, IVsTextSpanSet[] ppSpans, out IVsTextImage ppTextImage) {
            if (_findTarget != null) {
                return _findTarget.GetSearchImage(grfOptions, ppSpans, out ppTextImage);
            }
            ppTextImage = null;
            return VSConstants.E_NOTIMPL;
        }

        public int MarkSpan(TextSpan[] pts) {
            if (_findTarget != null) {
                return _findTarget.MarkSpan(pts);
            }
            return VSConstants.E_NOTIMPL;
        }

        public int NavigateTo(TextSpan[] pts) {
            if (_findTarget != null) {
                return _findTarget.NavigateTo(pts);
            }
            return VSConstants.E_NOTIMPL;
        }

        public int NotifyFindTarget(uint notification) {
            if (_findTarget != null) {
                return _findTarget.NotifyFindTarget(notification);
            }
            return VSConstants.E_NOTIMPL;
        }

        public int Replace(string pszSearch, string pszReplace, uint grfOptions, int fResetStartPoint, IVsFindHelper pHelper, out int pfReplaced) {
            if (_findTarget != null) {
                return _findTarget.Replace(pszSearch, pszReplace, grfOptions, fResetStartPoint, pHelper, out pfReplaced);
            }
            pfReplaced = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int SetFindState(object pUnk) {
            if (_findTarget != null) {
                return _findTarget.SetFindState(pUnk);
            }
            return VSConstants.E_NOTIMPL;
        }

        #endregion

        #region UI Dispatcher Helpers

        private Dispatcher Dispatcher {
            get { return ((FrameworkElement)TextView).Dispatcher; }
        }

        private bool CheckAccess() {
            return Dispatcher.CheckAccess();
        }

        private T UIThread<T>(Func<T> func) {
            if (!CheckAccess()) {
                return (T)Dispatcher.Invoke(func);
            }
            return func();
        }

        private void UIThread(Action action) {
            if (!CheckAccess()) {
                try {
                    Dispatcher.Invoke(action);
                } catch (OperationCanceledException) {
                    // VS is shutting down
                }
                return;
            }
            action();
        }

        private static void DoEvents() {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action<DispatcherFrame>(f => f.Continue = false),
                frame
                );
            Dispatcher.PushFrame(frame);
        }

        #endregion

        #region Testing

        internal List<ReplSpan> ProjectionSpans {
            get { return _projectionSpans; }
        }

        #endregion

    }
}
